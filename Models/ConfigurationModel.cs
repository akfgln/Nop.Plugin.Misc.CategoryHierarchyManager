using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.CategoryHierarchyManager.Models
{
    // Configuration Model
    public record ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Misc.CategoryHierarchyManager.AutoSync")]
        public bool AutoSyncEnabled { get; set; }

        [NopResourceDisplayName("Plugins.Misc.CategoryHierarchyManager.SyncOnProductUpdate")]
        public bool SyncOnProductUpdate { get; set; }

        [NopResourceDisplayName("Plugins.Misc.CategoryHierarchyManager.SyncOnCategoryUpdate")]
        public bool SyncOnCategoryUpdate { get; set; }
    }

    // Report Models
    public record ReportModel : BaseNopModel
    {
        public List<CategoryReportItem> Categories { get; set; } = new List<CategoryReportItem>();
    }

    public record CategoryReportItem : BaseNopModel
    {
        public int CategoryId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Name")]
        public string CategoryName { get; set; }

        public int ParentCategoryId { get; set; }

        public int Level { get; set; }

        [NopResourceDisplayName("Plugins.Misc.CategoryHierarchyManager.DirectProductCount")]
        public int DirectProductCount { get; set; }

        [NopResourceDisplayName("Plugins.Misc.CategoryHierarchyManager.TotalProductCount")]
        public int TotalProductCount { get; set; }

        [NopResourceDisplayName("Plugins.Misc.CategoryHierarchyManager.ExpectedProductCount")]
        public int ExpectedProductCount { get; set; }

        [NopResourceDisplayName("Plugins.Misc.CategoryHierarchyManager.IsSync")]
        public bool IsSync { get; set; }

        public List<CategoryReportItem> SubCategories { get; set; } = new List<CategoryReportItem>();

        [NopResourceDisplayName("Plugins.Misc.CategoryHierarchyManager.CategoryPath")]
        public string CategoryPath { get; set; }
    }
}