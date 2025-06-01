using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.CategoryHierarchyManager.Models;
using Nop.Plugin.Misc.CategoryHierarchyManager.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.CategoryHierarchyManager.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    public class CategoryHierarchyManagerController : BasePluginController
    {
        private readonly ICategoryHierarchyService _categoryHierarchyService;
        private readonly ISettingService _settingService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        public CategoryHierarchyManagerController(
            ICategoryHierarchyService categoryHierarchyService,
            ISettingService settingService,
            INotificationService notificationService,
            ILocalizationService localizationService,
            IPermissionService permissionService)
        {
            _categoryHierarchyService = categoryHierarchyService;
            _settingService = settingService;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _permissionService = permissionService;
        }

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var settings = await _settingService.LoadSettingAsync<CategoryHierarchyManagerSettings>();
            var model = new ConfigurationModel
            {
                AutoSyncEnabled = settings.AutoSyncEnabled,
                SyncOnProductUpdate = settings.SyncOnProductUpdate,
                SyncOnCategoryUpdate = settings.SyncOnCategoryUpdate
            };

            return View("~/Plugins/Misc.CategoryHierarchyManager/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var settings = await _settingService.LoadSettingAsync<CategoryHierarchyManagerSettings>();
            settings.AutoSyncEnabled = model.AutoSyncEnabled;
            settings.SyncOnProductUpdate = model.SyncOnProductUpdate;
            settings.SyncOnCategoryUpdate = model.SyncOnCategoryUpdate;

            await _settingService.SaveSettingAsync(settings);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

            return await Configure();
        }

        [HttpPost]
        public async Task<IActionResult> SyncNow()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            try
            {
                await _categoryHierarchyService.SyncCategoryHierarchyAsync();
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.CategoryHierarchyManager.Success"));
            }
            catch (Exception ex)
            {
                _notificationService.ErrorNotification($"{await _localizationService.GetResourceAsync("Plugins.Misc.CategoryHierarchyManager.Error")} {ex.Message}");
            }

            return await Configure();
        }

        public async Task<IActionResult> Report()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            try
            {
                var reportData = await _categoryHierarchyService.GetDetailedSyncReportAsync();
                var model = new ReportModel
                {
                    Categories = reportData
                };

                return View("~/Plugins/Misc.CategoryHierarchyManager/Views/Report.cshtml", model);
            }
            catch (Exception ex)
            {
                _notificationService.ErrorNotification($"Error loading report: {ex.Message}");
                return RedirectToAction("Configure");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SyncCategory(int categoryId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return Json(new { success = false, message = "Access denied" });

            try
            {
                await _categoryHierarchyService.SyncSpecificCategoryAsync(categoryId);
                return Json(new { success = true, message = "Category synchronized successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SyncProduct(int productId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return Json(new { success = false, message = "Access denied" });

            try
            {
                await _categoryHierarchyService.SyncProductCategoriesAsync(productId);
                return Json(new { success = true, message = "Product synchronized successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}