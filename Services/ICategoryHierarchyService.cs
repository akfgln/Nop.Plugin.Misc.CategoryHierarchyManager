using Nop.Plugin.Misc.CategoryHierarchyManager.Models;

namespace Nop.Plugin.Misc.CategoryHierarchyManager.Services
{
    public interface ICategoryHierarchyService
    {
        Task SyncCategoryHierarchyAsync();
        Task SyncProductCategoriesAsync(int productId);
        Task SyncSpecificCategoryAsync(int categoryId);
        Task<List<string>> GetSyncReportAsync();
        Task<List<CategoryReportItem>> GetDetailedSyncReportAsync();
    }
}