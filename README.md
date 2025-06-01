# NopCommerce Category Hierarchy Manager Plugin

[![NopCommerce Version](https://img.shields.io/badge/NopCommerce-4.60%2B-blue.svg)](https://www.nopcommerce.com)
[![.NET Version](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Plugin Type](https://img.shields.io/badge/Plugin-Miscellaneous-orange.svg)](https://docs.nopcommerce.com/en/developer/plugins/)

A powerful NopCommerce plugin that automatically manages category hierarchy by ensuring products assigned to sub-categories are also properly assigned to their parent categories. This plugin maintains consistency in your category structure and improves product discoverability across your e-commerce store.

## 🚀 Features

### ✨ Core Functionality
- **Automatic Hierarchy Sync**: Automatically assigns products from sub-categories to parent categories
- **Real-time Updates**: Triggers synchronization when products or categories are modified
- **Manual Synchronization**: On-demand sync for immediate category hierarchy management
- **Selective Sync**: Synchronize specific categories or products individually
- **Event-Driven Architecture**: Responds to product and category changes in real-time

### 📊 Advanced Reporting
- **Hierarchical Category Tree**: Visual representation of your category structure
- **Sync Status Indicators**: Color-coded status showing synchronized vs. out-of-sync categories
- **Statistical Dashboard**: Overview cards showing total categories, sync status, and product counts
- **Search & Filter**: Find specific categories and filter by sync status
- **Expandable Tree View**: Collapse/expand category branches for better navigation

### 🎨 User Interface
- **Native NopCommerce Design**: Seamlessly integrated with the admin panel
- **Responsive Layout**: Works perfectly on desktop, tablet, and mobile devices
- **AJAX Operations**: Smooth, non-blocking operations for better user experience
- **Localization Support**: Multi-language support with resource strings
- **Permission-Based Access**: Integrated with NopCommerce permission system

## 📋 Requirements

- **NopCommerce**: Version 4.60 or higher
- **.NET**: Version 8.0 or higher
- **Database**: SQL Server, MySQL, or PostgreSQL (depending on your NopCommerce setup)
- **Permissions**: Administrator access to install and configure plugins

## 🛠️ Installation

### Method 1: Manual Installation

1. **Download the Plugin**
   ```bash
   git clone https://github.com/akfgln/Nop.Plugin.Misc.CategoryHierarchyManager.git
   ```

2. **Create Plugin Directory**
   ```bash
   mkdir -p /src/Plugins/Nop.Plugin.Misc.CategoryHierarchyManager
   ```

3. **Copy Files**
   Copy all plugin files to the created directory following the file structure below.

4. **Build the Plugin**
   ```bash
   cd /src/Plugins/Nop.Plugin.Misc.CategoryHierarchyManager
   dotnet build
   ```

5. **Install via Admin Panel**
   - Navigate to **Administration → Configuration → Local Plugins**
   - Find **"Category Hierarchy Manager"** in the plugin list
   - Click **"Install"**
   - Restart your application if prompted

### Method 2: NuGet Package (Coming Soon)
```bash
Install-Package Nop.Plugin.Misc.CategoryHierarchyManager
```

## 📁 File Structure

```
Nop.Plugin.Misc.CategoryHierarchyManager/
├── 📄 plugin.json                              # Plugin metadata
├── 📄 CategoryHierarchyManagerPlugin.cs        # Main plugin class
├── 📄 Nop.Plugin.Misc.CategoryHierarchyManager.csproj
├── 📁 Controllers/
│   └── 📄 CategoryHierarchyManagerController.cs
├── 📁 Services/
│   ├── 📄 ICategoryHierarchyService.cs
│   ├── 📄 CategoryHierarchyService.cs
│   └── 📄 EventConsumers.cs
├── 📁 Models/
│   └── 📄 ConfigurationModel.cs
├── 📁 Infrastructure/
│   ├── 📄 CategoryHierarchyDefaults.cs
│   ├── 📄 NopStartup.cs
│   └── 📄 RouteProvider.cs
└── 📁 Views/
    ├── 📄 Configure.cshtml
    └── 📄 Report.cshtml
```

## ⚙️ Configuration

### Initial Setup

1. **Access Plugin Configuration**
   - Go to **Administration → Configuration → Local Plugins**
   - Find **"Category Hierarchy Manager"** and click **"Configure"**

2. **Configure Settings**
   - **Auto Sync Enabled**: Enable/disable automatic synchronization
   - **Sync on Product Update**: Trigger sync when products are modified
   - **Sync on Category Update**: Trigger sync when categories are modified

### Settings Overview

| Setting | Description | Default |
|---------|-------------|---------|
| Auto Sync Enabled | Enables automatic synchronization | `true` |
| Sync on Product Update | Syncs when products are updated | `true` |
| Sync on Category Update | Syncs when categories are updated | `true` |

## 🎯 Usage

### Automatic Synchronization

The plugin automatically maintains category hierarchy in the following scenarios:

1. **Product Assignment**: When a product is assigned to a sub-category
2. **Product Updates**: When product category assignments are modified
3. **Category Changes**: When category parent-child relationships are updated

### Manual Synchronization

#### Full System Sync
1. Navigate to the plugin configuration page
2. Click **"Sync Now"** button
3. Confirm the operation
4. Wait for completion notification

#### Category-Specific Sync
1. Go to **Category Hierarchy Report**
2. Find the desired category
3. Click the **"Sync"** button next to out-of-sync categories
4. Confirm the operation

### Viewing Reports

1. **Access Report Page**
   - Go to plugin configuration
   - Click **"View Report"** button

2. **Understanding the Report**
   - **Green Categories**: Fully synchronized
   - **Yellow Categories**: Out of sync (missing products in parent categories)
   - **Statistics Cards**: Overview of your category health

3. **Using Filters**
   - Search by category name
   - Show only out-of-sync categories
   - Expand/collapse category branches

## 🔧 API Reference

### Service Interface

```csharp
public interface ICategoryHierarchyService
{
    Task SyncCategoryHierarchyAsync();
    Task SyncProductCategoriesAsync(int productId);
    Task SyncSpecificCategoryAsync(int categoryId);
    Task<List<string>> GetSyncReportAsync();
    Task<List<CategoryReportItem>> GetDetailedSyncReportAsync();
}
```

### Controller Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/Admin/CategoryHierarchyManager/Configure` | GET/POST | Configuration page |
| `/Admin/CategoryHierarchyManager/Report` | GET | Detailed report page |
| `/Admin/CategoryHierarchyManager/SyncNow` | POST | Manual full sync |
| `/Admin/CategoryHierarchyManager/SyncCategory/{id}` | POST | Sync specific category |
| `/Admin/CategoryHierarchyManager/SyncProduct/{id}` | POST | Sync specific product |

### Event Consumers

The plugin responds to the following NopCommerce events:

- `EntityUpdatedEvent<Product>`: Product updates
- `EntityInsertedEvent<ProductCategory>`: Product-category assignments
- `EntityUpdatedEvent<Category>`: Category modifications

## 🎨 Customization

### Adding Custom Logic

You can extend the plugin by implementing custom event consumers:

```csharp
public class CustomEventConsumer : IConsumer<EntityUpdatedEvent<Product>>
{
    public async Task HandleEventAsync(EntityUpdatedEvent<Product> eventMessage)
    {
        // Your custom logic here
    }
}
```

### Modifying UI

The plugin uses standard NopCommerce view patterns. You can override views by:

1. Creating custom views in your theme
2. Following NopCommerce view override conventions
3. Maintaining the same model structure

### Extending Services

Inherit from `CategoryHierarchyService` to add custom functionality:

```csharp
public class ExtendedCategoryHierarchyService : CategoryHierarchyService
{
    // Add your custom methods
}
```

## 🔍 Troubleshooting

### Common Issues

#### Plugin Not Appearing
- Ensure the plugin is built correctly
- Check that all dependencies are resolved
- Verify the `plugin.json` file is valid
- Restart the application

#### Sync Not Working
- Check plugin configuration settings
- Verify auto-sync is enabled
- Review application logs for errors
- Ensure proper permissions are set

#### Performance Issues
- Monitor database performance during large syncs
- Consider running manual syncs during off-peak hours
- Check for circular category references

### Debugging

Enable detailed logging by modifying `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Nop.Plugin.Misc.CategoryHierarchyManager": "Debug"
    }
  }
}
```

### Log Locations

Check the following locations for plugin logs:
- `App_Data/Logs/` (default NopCommerce log location)
- Application Event Viewer (Windows)
- Console output (during development)

## 📈 Performance Considerations

### Best Practices

1. **Large Catalogs**: For stores with 10,000+ products, consider:
   - Running manual syncs during maintenance windows
   - Monitoring database performance
   - Using database indexing on category tables

2. **Frequent Updates**: If you frequently update products/categories:
   - Monitor sync frequency
   - Consider batch operations
   - Use selective sync when possible

3. **Database Optimization**: Ensure proper indexing on:
   - `Product_Category_Mapping` table
   - `Category` table parent-child relationships
   - Product and Category primary keys

### Performance Metrics

| Store Size | Sync Time (Estimated) | Memory Usage |
|------------|----------------------|--------------|
| < 1,000 products | < 30 seconds | < 50 MB |
| 1,000 - 10,000 products | 1-5 minutes | 50-200 MB |
| > 10,000 products | 5-30 minutes | 200+ MB |

## 🤝 Contributing

We welcome contributions! Please follow these guidelines:

### Development Setup

1. **Fork the Repository**
2. **Clone Your Fork**
   ```bash
   git clone https://github.com/akfgln/Nop.Plugin.Misc.CategoryHierarchyManager.git
   ```
3. **Create Feature Branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```
4. **Make Changes and Test**
5. **Submit Pull Request**

### Code Standards

- Follow NopCommerce coding conventions
- Include unit tests for new features
- Update documentation for API changes
- Ensure backward compatibility

### Testing

Run the test suite before submitting:

```bash
dotnet test
```

## 📄 License


## 🙏 Acknowledgments

- **NopCommerce Team**: For providing an excellent e-commerce platform
- **Community Contributors**: For feedback and feature suggestions
- **Beta Testers**: For helping identify and resolve issues

## 📞 Support

### Getting Help

1. **Documentation**: Check this README first
2. **Issues**: Create a GitHub issue for bugs
3. **Discussions**: Use GitHub Discussions for questions
4. **Email**: contact@yourcompany.com

### Commercial Support

For enterprise support, custom features, or professional services:
- Email: enterprise@yourcompany.com
- Website: [Your Company Website]

## 🔄 Changelog

### Version 1.0.0 (Latest)
- ✅ Initial release
- ✅ Automatic category hierarchy management
- ✅ Real-time synchronization
- ✅ Advanced reporting dashboard
- ✅ Manual sync capabilities
- ✅ Multi-language support

### Upcoming Features
- 🔄 Scheduled sync tasks
- 🔄 Bulk operations API
- 🔄 Category sync rules engine
- 🔄 Performance analytics
- 🔄 Import/export configurations

---

**Made with ❤️ for the NopCommerce Community**