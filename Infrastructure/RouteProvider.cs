using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.CategoryHierarchyManager.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            // Category Hierarchy Manager Configuration Route
            endpointRouteBuilder.MapControllerRoute(
                CategoryHierarchyDefaults.ConfigurationRouteName,
                "Admin/CategoryHierarchyManager/Configure",
                new { controller = "CategoryHierarchyManager", action = "Configure" });

            // Manual Sync Route
            endpointRouteBuilder.MapControllerRoute(
                CategoryHierarchyDefaults.SyncNowRouteName,
                "Admin/CategoryHierarchyManager/SyncNow",
                new { controller = "CategoryHierarchyManager", action = "SyncNow" });

            // Sync Report Route
            endpointRouteBuilder.MapControllerRoute(
                CategoryHierarchyDefaults.ReportRouteName,
                "Admin/CategoryHierarchyManager/Report",
                new { controller = "CategoryHierarchyManager", action = "Report" });

            // Category Sync Route (with parameter)
            endpointRouteBuilder.MapControllerRoute(
                name: CategoryHierarchyDefaults.CategorySyncRouteName,
                pattern: "Admin/CategoryHierarchyManager/SyncCategory/{categoryId:min(0)}",
                defaults: new { controller = "CategoryHierarchyManager", action = "SyncCategory" });

            // Product Sync Route (with parameter)
            endpointRouteBuilder.MapControllerRoute(
                name: CategoryHierarchyDefaults.ProductSyncRouteName,
                pattern: "Admin/CategoryHierarchyManager/SyncProduct/{productId:min(0)}",
                defaults: new { controller = "CategoryHierarchyManager", action = "SyncProduct" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}