using Nop.Core.Configuration;
using Nop.Plugin.Misc.CategoryHierarchyManager.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.CategoryHierarchyManager
{
    // Plugin Settings
    public class CategoryHierarchyManagerSettings : ISettings
    {
        public bool AutoSyncEnabled { get; set; } = true;
        public bool SyncOnProductUpdate { get; set; } = true;
        public bool SyncOnCategoryUpdate { get; set; } = true;
    }

    // Plugin ana sınıfı
    public class CategoryHierarchyManagerPlugin : BasePlugin, IMiscPlugin
    {
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        public CategoryHierarchyManagerPlugin(
            ISettingService settingService,
            ILocalizationService localizationService)
        {
            _settingService = settingService;
            _localizationService = localizationService;
        }

        public override string GetConfigurationPageUrl()
        {
            return CategoryHierarchyDefaults.ConfigurationUrl;
        }

        public override async Task InstallAsync()
        {
            var settings = new CategoryHierarchyManagerSettings
            {
                AutoSyncEnabled = true,
                SyncOnProductUpdate = true,
                SyncOnCategoryUpdate = true
            };

            await _settingService.SaveSettingAsync(settings);

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Misc.CategoryHierarchyManager.Title"] = "Category Hierarchy Manager",
                ["Plugins.Misc.CategoryHierarchyManager.AutoSync"] = "Auto Sync Enabled",
                ["Plugins.Misc.CategoryHierarchyManager.SyncOnProductUpdate"] = "Sync on Product Update",
                ["Plugins.Misc.CategoryHierarchyManager.SyncOnCategoryUpdate"] = "Sync on Category Update",
                ["Plugins.Misc.CategoryHierarchyManager.SyncNow"] = "Sync Now",
                ["Plugins.Misc.CategoryHierarchyManager.SyncAll"] = "Sync All Categories",
                ["Plugins.Misc.CategoryHierarchyManager.ViewReport"] = "View Report",
                ["Plugins.Misc.CategoryHierarchyManager.Report"] = "Category Hierarchy Report",
                ["Plugins.Misc.CategoryHierarchyManager.Success"] = "Categories synchronized successfully!",
                ["Plugins.Misc.CategoryHierarchyManager.Error"] = "An error occurred during synchronization.",
                ["Plugins.Misc.CategoryHierarchyManager.TotalCategories"] = "Total Categories",
                ["Plugins.Misc.CategoryHierarchyManager.SyncedCategories"] = "Synced Categories",
                ["Plugins.Misc.CategoryHierarchyManager.OutOfSyncCategories"] = "Out of Sync",
                ["Plugins.Misc.CategoryHierarchyManager.TotalProducts"] = "Total Products",
                ["Plugins.Misc.CategoryHierarchyManager.CategoryHierarchy"] = "Category Hierarchy",
                ["Plugins.Misc.CategoryHierarchyManager.ShowOnlyOutOfSync"] = "Show Only Out of Sync Categories",
                ["Plugins.Misc.CategoryHierarchyManager.DirectProductCount"] = "Direct Products",
                ["Plugins.Misc.CategoryHierarchyManager.TotalProductCount"] = "Total Products",
                ["Plugins.Misc.CategoryHierarchyManager.ExpectedProductCount"] = "Expected Products",
                ["Plugins.Misc.CategoryHierarchyManager.IsSync"] = "Synchronized",
                ["Plugins.Misc.CategoryHierarchyManager.CategoryPath"] = "Category Path"
            });

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await _settingService.DeleteSettingAsync<CategoryHierarchyManagerSettings>();
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.CategoryHierarchyManager");
            await base.UninstallAsync();
        }
    }
}