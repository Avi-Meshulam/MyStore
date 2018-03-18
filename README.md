# MyStore
### A UWP MVVM shopping App, with Entity Framework Core, Prism.Unity and SQLite. Built with VS 2017.
![Startup Screen](https://github.com/PrisonerM13/MyStore/blob/master/gif/Start.gif "Startup Screen")

The application consists of 2 projects:
+ MyStore - UWP App client
+ MyStore.BL - UWP class library (contains both business logic and data access logic (via EF core))

## Features/Tools:
+ DB: [SQLite](https://www.sqlite.org/)
+ DB Access: [EF Core](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tools/)
+ MVVM/IoC: [Prism.Unity](https://www.nuget.org/packages/Prism.Unity/6.3.0)
+ [Adaptive Display](https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.visualstatemanager)
		
	![Adaptive Display](https://github.com/PrisonerM13/MyStore/blob/master/gif/AdaptiveDisplay.gif "Adaptive Display")
+ Notifications via [NotificationsExtensions](https://www.nuget.org/packages/NotificationsExtensions.Win10/ "Notifications Extensions") library:
	- Live Tiles
		
	![Live Tile](https://github.com/PrisonerM13/MyStore/blob/master/gif/LiveTile.gif "Live Tile")
	- Badge Notifications
		
	![Badge Notifications](https://github.com/PrisonerM13/MyStore/blob/master/gif/Badges.gif "Badge Notifications")
	- Background Tasks (updating tiles & badges)

## Views
- **Main**: Container for all other views. Hosts application's header (app title, menu button and shopping cart button) and an extractable side menu. The rest of the view is a placeholder frame for other views.
- **Catalog**: Displays a product card (picture, title, price) for each product in catalog, while beneath each card there's a button, allowing to add it to shopping cart.
- **Shopping Cart**: Lists all shopping cart items in a table view, allowing to manipulate quantities, delete items and commit a checkout (place an order).
		
	![Shopping Cart](https://github.com/PrisonerM13/MyStore/blob/master/gif/ShoppingCart.gif "Shopping Cart")

	#### App & View Objects
	![App & Views](https://github.com/PrisonerM13/MyStore/blob/master/images/Views.png "App & Views")
		
## DB structure
![ERD](https://github.com/PrisonerM13/MyStore/blob/master/images/ERD.png "ERD")
		
| Table             | Remarks   
| ----------------- | ------------- 
| Catalogs          | A Catalog is a group of products that share a common subject
| Products          | A Product is associated with a catalog (currently only one catalog per product(1))
| Customers         | A customer is based on the current user logged in Windows
| ShoppingCarts     | A shopping cart is associated with a customer
| ShoppingCartItems | A shopping cart item is associated with a shopping cart and a product.
| Orders            | An order is basically a copy of a shopping cart after checkout, and is also associated with a customer.
| OrderItems        | An order item is a copy of a shopping cart item after checkout, and is associated with an order and a product.

> (1) if you need to associate a product with multiple catalogs, it is adviced to
> create a new table, e.g. CatalogsProducts, which will link between catalogs & products.
> The new table will replace the field CatalogId in Products table.
		
#### EF Core DB Context Class
![DB Context](https://github.com/PrisonerM13/MyStore/blob/master/images/DBContext.png "DB Context")

### Each table is associated with a:
- **Model** - Reflects the table fields and inherits from Equatable&lt;T&gt; (also implements IEquatable&lt;T&gt; in order to allow explicit implementation).
- **View Model** - Inherits from ViewModeBase&lt;T&gt;, which in turn inherits from Prism.Windows.Mvvm.ViewModelBase.
- **Data Repository/Controller** - Implements IDataRepository&lt;T&gt;.
		
### Models
![Models](https://github.com/PrisonerM13/MyStore/blob/master/images/Models.png "Models")

### View Models
![View Models](https://github.com/PrisonerM13/MyStore/blob/master/images/ViewModels.png "View Models")

### Repositories
![Repositories](https://github.com/PrisonerM13/MyStore/blob/master/images/Repositories.png "Repositories")

## Notes
> In case the application terminates unexpectedly, and the following error apears in Windows Event Viewer - 
> try the following solution to fix that.

Event Viewer Log Message:
> The machine-default permission settings do not grant Local Activation permission for the COM Server application 
> with CLSID {CLSID} and APPID {APPID} to the user from address LocalHost (Using LRPC) running in the application 
> container Microsoft.Windows.Cortana...
> This security permission can be modified using the Component Services administrative tool.

Solution: [Edit Registry and Component Services permissions](https://answers.microsoft.com/en-us/windows/forum/windows8_1-winapps/weather-application/e4630db3-50c2-4cc5-9813-f089494a1145?auth=1)
