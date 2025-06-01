using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Services.Configuration;
using Nop.Services.Events;

namespace Nop.Plugin.Misc.CategoryHierarchyManager.Services
{
    // Ürün güncellendiğinde tetiklenir
    public class ProductUpdatedEventConsumer : IConsumer<EntityUpdatedEvent<Product>>
    {
        private readonly ICategoryHierarchyService _categoryHierarchyService;
        private readonly ISettingService _settingService;

        public ProductUpdatedEventConsumer(
            ICategoryHierarchyService categoryHierarchyService,
            ISettingService settingService)
        {
            _categoryHierarchyService = categoryHierarchyService;
            _settingService = settingService;
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<Product> eventMessage)
        {
            var settings = await _settingService.LoadSettingAsync<CategoryHierarchyManagerSettings>();

            if (settings.AutoSyncEnabled && settings.SyncOnProductUpdate)
            {
                await _categoryHierarchyService.SyncProductCategoriesAsync(eventMessage.Entity.Id);
            }
        }
    }

    // Ürün kategoriye eklendiğinde tetiklenir
    public class ProductCategoryInsertedEventConsumer : IConsumer<EntityInsertedEvent<ProductCategory>>
    {
        private readonly ICategoryHierarchyService _categoryHierarchyService;
        private readonly ISettingService _settingService;

        public ProductCategoryInsertedEventConsumer(
            ICategoryHierarchyService categoryHierarchyService,
            ISettingService settingService)
        {
            _categoryHierarchyService = categoryHierarchyService;
            _settingService = settingService;
        }

        public async Task HandleEventAsync(EntityInsertedEvent<ProductCategory> eventMessage)
        {
            var settings = await _settingService.LoadSettingAsync<CategoryHierarchyManagerSettings>();

            if (settings.AutoSyncEnabled && settings.SyncOnProductUpdate)
            {
                await _categoryHierarchyService.SyncProductCategoriesAsync(eventMessage.Entity.ProductId);
            }
        }
    }

    // Kategori güncellendiğinde tetiklenir
    public class CategoryUpdatedEventConsumer : IConsumer<EntityUpdatedEvent<Category>>
    {
        private readonly ICategoryHierarchyService _categoryHierarchyService;
        private readonly ISettingService _settingService;

        public CategoryUpdatedEventConsumer(
            ICategoryHierarchyService categoryHierarchyService,
            ISettingService settingService)
        {
            _categoryHierarchyService = categoryHierarchyService;
            _settingService = settingService;
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<Category> eventMessage)
        {
            var settings = await _settingService.LoadSettingAsync<CategoryHierarchyManagerSettings>();

            if (settings.AutoSyncEnabled && settings.SyncOnCategoryUpdate)
            {
                // Kategori hiyerarşisi değişmişse tüm sistemi senkronize et
                await _categoryHierarchyService.SyncCategoryHierarchyAsync();
            }
        }
    }
}