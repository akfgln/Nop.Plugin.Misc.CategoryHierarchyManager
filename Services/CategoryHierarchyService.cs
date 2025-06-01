using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.CategoryHierarchyManager.Models;
using Nop.Services.Catalog;

namespace Nop.Plugin.Misc.CategoryHierarchyManager.Services
{
    public class CategoryHierarchyService : ICategoryHierarchyService
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;

        public CategoryHierarchyService(
            ICategoryService categoryService,
            IProductService productService,
            IWorkContext workContext)
        {
            _categoryService = categoryService;
            _productService = productService;
            _workContext = workContext;
        }

        public async Task SyncCategoryHierarchyAsync()
        {
            // Tüm kategorileri al (showHidden: true olarak ayarlandı)
            var allCategories = await _categoryService.GetAllCategoriesAsync(showHidden: true);
            var categoryDict = allCategories.ToDictionary(c => c.Id, c => c);

            foreach (var category in allCategories.Where(c => c.ParentCategoryId > 0))
            {
                await SyncCategoryProductsAsync(category, categoryDict);
            }
        }

        public async Task SyncProductCategoriesAsync(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
                return;

            // showHidden: true ile tüm kategorileri al
            var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(productId, showHidden: true);
            var allCategories = await _categoryService.GetAllCategoriesAsync(showHidden: true);
            var categoryDict = allCategories.ToDictionary(c => c.Id, c => c);

            foreach (var pc in productCategories)
            {
                var category = categoryDict.GetValueOrDefault(pc.CategoryId);
                if (category?.ParentCategoryId > 0)
                {
                    await EnsureProductInParentCategoriesAsync(productId, category, categoryDict);
                }
            }
        }

        /// <summary>
        /// Belirtilen kategori ve tüm alt kategorilerindeki ürünleri üst kategorilere senkronize eder
        /// </summary>
        private async Task SyncCategoryProductsAsync(Category category, Dictionary<int, Category> categoryDict)
        {
            // 1. Bu kategorideki direkt ürünleri al ve üst kategorilere ekle
            var productCategories = await _categoryService.GetProductCategoriesByCategoryIdAsync(
                category.Id,
                pageIndex: 0,
                pageSize: int.MaxValue,
                showHidden: true);

            foreach (var pc in productCategories)
            {
                await EnsureProductInParentCategoriesAsync(pc.ProductId, category, categoryDict);
            }

            // 2. Alt kategorileri bul
            var subCategories = categoryDict.Values
                .Where(c => c.ParentCategoryId == category.Id)
                .ToList();

            // 3. Her alt kategori için rekursif olarak işlemi tekrarla
            foreach (var subCategory in subCategories)
            {
                // Alt kategorideki ürünleri al ve üst kategorilere ekle
                var subProductCategories = await _categoryService.GetProductCategoriesByCategoryIdAsync(
                    subCategory.Id,
                    pageIndex: 0,
                    pageSize: int.MaxValue,
                    showHidden: true);

                foreach (var pc in subProductCategories)
                {
                    await EnsureProductInParentCategoriesAsync(pc.ProductId, subCategory, categoryDict);
                }

                // İsterseniz alt kategorileri de rekursif olarak işleyebilirsiniz
                await SyncCategoryProductsAsync(subCategory, categoryDict);
            }
        }

        private async Task EnsureProductInParentCategoriesAsync(int productId, Category childCategory, Dictionary<int, Category> categoryDict)
        {
            var currentCategory = childCategory;

            while (currentCategory.ParentCategoryId > 0)
            {
                var parentCategory = categoryDict.GetValueOrDefault(currentCategory.ParentCategoryId);
                if (parentCategory == null)
                    break;

                // Ürünün parent kategoride olup olmadığını kontrol et
                var allProductCategories = await _categoryService.GetProductCategoriesByProductIdAsync(productId, showHidden: true);
                var existingMapping = _categoryService.FindProductCategory(allProductCategories, productId, parentCategory.Id);

                if (existingMapping == null)
                {
                    // Ürünü parent kategoriye ekle
                    var newMapping = new ProductCategory
                    {
                        ProductId = productId,
                        CategoryId = parentCategory.Id,
                        IsFeaturedProduct = false,
                        DisplayOrder = 0
                    };

                    await _categoryService.InsertProductCategoryAsync(newMapping);
                }

                currentCategory = parentCategory;
            }
        }

        public async Task<List<string>> GetSyncReportAsync()
        {
            var report = new List<string>();
            var allCategories = await _categoryService.GetAllCategoriesAsync(showHidden: true);

            foreach (var category in allCategories.Where(c => c.ParentCategoryId > 0))
            {
                var productCategoriesPage = await _categoryService.GetProductCategoriesByCategoryIdAsync(
                    category.Id,
                    pageIndex: 0,
                    pageSize: int.MaxValue,
                    showHidden: true);

                var productCount = productCategoriesPage.TotalCount;
                report.Add($"Category: {category.Name} - Products: {productCount}");
            }

            return report;
        }

        public async Task SyncSpecificCategoryAsync(int categoryId)
        {
            var allCategories = await _categoryService.GetAllCategoriesAsync(showHidden: true);
            var categoryDict = allCategories.ToDictionary(c => c.Id, c => c);

            var category = categoryDict.GetValueOrDefault(categoryId);
            if (category == null)
                return;

            await SyncCategoryProductsAsync(category, categoryDict);
        }

        public async Task<List<CategoryReportItem>> GetDetailedSyncReportAsync()
        {
            var allCategories = await _categoryService.GetAllCategoriesAsync(showHidden: true);
            var categoryDict = allCategories.ToDictionary(c => c.Id, c => c);

            // Kategorilerdeki tüm ürünleri önden toplayalım
            Dictionary<int, HashSet<int>> categoryProductsMap = new Dictionary<int, HashSet<int>>();

            foreach (var category in allCategories)
            {
                var productCategories = await _categoryService.GetProductCategoriesByCategoryIdAsync(
                    category.Id, 0, int.MaxValue, showHidden: true);

                // Her kategori için benzersiz ürün kimliklerini saklayalım
                categoryProductsMap[category.Id] = new HashSet<int>();

                foreach (var pc in productCategories)
                {
                    categoryProductsMap[category.Id].Add(pc.ProductId);
                }
            }

            var reportItems = new List<CategoryReportItem>();

            // Root kategorileri al
            var rootCategories = allCategories.Where(c => c.ParentCategoryId == 0).OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name);

            foreach (var rootCategory in rootCategories)
            {
                var rootItem = await BuildCategoryReportItemAsync(rootCategory, categoryDict, categoryProductsMap, 0);
                reportItems.Add(rootItem);
            }

            return reportItems;
        }

        private async Task<CategoryReportItem> BuildCategoryReportItemAsync(
            Category category,
            Dictionary<int, Category> categoryDict,
            Dictionary<int, HashSet<int>> categoryProductsMap,
            int level)
        {
            // Bu kategorideki direkt ürün sayısı (zaten hesaplanmış)
            var directProducts = categoryProductsMap[category.Id];

            var item = new CategoryReportItem
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
                ParentCategoryId = category.ParentCategoryId,
                Level = level,
                DirectProductCount = directProducts.Count,
                CategoryPath = await _categoryService.GetFormattedBreadCrumbAsync(category, null, " > ")
            };

            // Alt kategorileri al
            var subCategories = categoryDict.Values
                .Where(c => c.ParentCategoryId == category.Id)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name);

            // Alt kategorilerin ürünlerinin toplamı için benzersiz küme kullan
            HashSet<int> allSubCategoryProducts = new HashSet<int>();

            foreach (var subCategory in subCategories)
            {
                var subItem = await BuildCategoryReportItemAsync(subCategory, categoryDict, categoryProductsMap, level + 1);
                item.SubCategories.Add(subItem);

                // Alt kategorinin tüm benzersiz ürünlerini topla
                // GetAllProductsFromCategory metodunu yazacağız
                var subCategoryProducts = GetAllProductsFromCategory(subCategory.Id, categoryDict, categoryProductsMap);
                foreach (var productId in subCategoryProducts)
                {
                    allSubCategoryProducts.Add(productId);
                }
            }

            // Beklenen ürün sayısı (alt kategorilerdeki benzersiz ürünlerin sayısı)
            item.ExpectedProductCount = allSubCategoryProducts.Count;

            // Toplam ürün sayısı (direkt + alt kategorilerdeki benzersiz ürünler)
            HashSet<int> totalProducts = new HashSet<int>(directProducts);
            foreach (var productId in allSubCategoryProducts)
            {
                totalProducts.Add(productId);
            }
            item.TotalProductCount = totalProducts.Count;

            // Senkronize olup olmadığını kontrol et
            // Burada tüm alt kategorilerdeki benzersiz ürünlerin, direkt ürünler içinde olup olmadığını kontrol ediyoruz
            item.IsSync = allSubCategoryProducts.IsSubsetOf(directProducts);

            return item;
        }

        // Bir kategori ve alt kategorilerindeki tüm benzersiz ürünleri döndüren yardımcı metod
        private HashSet<int> GetAllProductsFromCategory(
            int categoryId,
            Dictionary<int, Category> categoryDict,
            Dictionary<int, HashSet<int>> categoryProductsMap)
        {
            HashSet<int> result = new HashSet<int>();

            // Bu kategorinin ürünlerini ekle
            if (categoryProductsMap.ContainsKey(categoryId))
            {
                foreach (var productId in categoryProductsMap[categoryId])
                {
                    result.Add(productId);
                }
            }

            // Alt kategorileri bul
            var subCategories = categoryDict.Values
                .Where(c => c.ParentCategoryId == categoryId)
                .ToList();

            // Alt kategorilerin ürünlerini ekle
            foreach (var subCategory in subCategories)
            {
                var subProducts = GetAllProductsFromCategory(subCategory.Id, categoryDict, categoryProductsMap);
                foreach (var productId in subProducts)
                {
                    result.Add(productId);
                }
            }

            return result;
        }
    }
}