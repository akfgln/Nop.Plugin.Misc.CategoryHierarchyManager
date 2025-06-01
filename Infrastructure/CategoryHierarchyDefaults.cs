namespace Nop.Plugin.Misc.CategoryHierarchyManager.Infrastructure
{
    public static class CategoryHierarchyDefaults
    {
        /// <summary>
        /// Plugin system name
        /// </summary>
        public static string PluginSystemName => "Misc.CategoryHierarchyManager";

        /// <summary>
        /// Configuration route name
        /// </summary>
        public static string ConfigurationRouteName => "Plugin.Misc.CategoryHierarchyManager.Configure";

        /// <summary>
        /// Sync now route name
        /// </summary>
        public static string SyncNowRouteName => "Plugin.Misc.CategoryHierarchyManager.SyncNow";

        /// <summary>
        /// Report route name
        /// </summary>
        public static string ReportRouteName => "Plugin.Misc.CategoryHierarchyManager.Report";

        /// <summary>
        /// Category sync route name
        /// </summary>
        public static string CategorySyncRouteName => "Plugin.Misc.CategoryHierarchyManager.SyncCategory";

        /// <summary>
        /// Product sync route name
        /// </summary>
        public static string ProductSyncRouteName => "Plugin.Misc.CategoryHierarchyManager.SyncProduct";

        /// <summary>
        /// Configuration page URL
        /// </summary>
        public static string ConfigurationUrl => "/Admin/CategoryHierarchyManager/Configure";
    }
}