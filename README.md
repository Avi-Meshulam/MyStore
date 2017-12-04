![alt text](https://github.com/PrisonerM13/MyStore/blob/master/Start.gif "Start Screen")

The application contains 2 projects:
+ MyStore - UWP App client
+ MyStore.BL - UWP class library (contains both business logic and data access logic (via EF core))

### Features/Tools:
+ DB: [SQLite](https://www.sqlite.org/)
+ DB Access: [EF Core](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tools/)
+ MVVM/IoC: [Prism.Unity](https://www.nuget.org/packages/Prism.Unity/6.3.0)
+ [Adaptive Display](https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.visualstatemanager)
![alt text](https://github.com/PrisonerM13/MyStore/blob/master/AdaptiveDisplay.gif "Adaptive Display")
+ Notifications via [NotificationsExtensions](https://www.nuget.org/packages/NotificationsExtensions.Win10/ "Notifications Extensions") library:
	- Adaptive Tiles
	- Badge Notifications
	- Background Tasks (updating tiles & badges)
		
	![alt text](https://github.com/PrisonerM13/MyStore/blob/master/LiveTile.gif "Live Tile")

### Operations:
1. Catalog view - Add items to shopping cart
![alt text](https://github.com/PrisonerM13/MyStore/blob/master/Badges.gif "Add Items")

2. Shopping Cart view - Change quantities, Remove items and Checkout (set an order)
![alt text](https://github.com/PrisonerM13/MyStore/blob/master/ShoppingCart.gif "Shopping Cart")

### DB structure:
| Table             | Remarks   
| ----------------- | ------------- 
| Catalogs          | A Catalog is a group of products that share a common subject
| Products          | A Product is associated with one a catalog (currently only one catalog per product(1))
| Customers         | A customer is based on the current user logged in Windows
| ShoppingCarts     | A shopping cart is associated with a customer
| ShoppingCartItems | A shopping cart item is associated with a shopping cart and a product.
| Orders            | An order is basically a copy of a shopping cart after checkout, and is also associated with a customer.
| OrderItems        | An order item is a copy of a shopping cart item after checkout, and is associated with an order and a product.

> (1) if you need to be able to associate a product with multiple catalogs, 
> you are adviced to create a new table, e.g. CatalogsProducts, which will link between catalogs & products.
> The new table will replace the field CatalogId in Products table.

Each table is associated with a:
- Model - Reflects the table fields and inherits from Equatable<T> (also implements IEquatable<T> in order to allow explicit implementation).
- View Model - Inherits from ViewModeBase<T>, which in turn inherits from Prism.Windows.Mvvm.ViewModelBase.
- Data Repository/Controller - Implements IDataRepository<T>.

```C#
public interface IDataRepository<T>
{
	List<T> GetAll();
	List<T> GetByPredicate(Func<T, bool> predicate);
	T GetById(uint id);
	T Add(T obj);
	bool Update(T obj);
	bool Delete(T obj);
	int Clear();
}
```

### Views
- Main: Container for all other views. Contains app header (app title, menu button and shopping cart button) and the extractable side menu. The rest of the view is a placeholder frame for other views.
- Catalog: Displays a product card (picture, title, price) for each product in catalog, while beneath each card there's a button, allowing to add it to shopping cart.
- Shopping Cart: Lists all shopping cart items in a table view, and allowing to manipulate quantities, delete items and checkout.

### Notes
> In case the application terminates unexpectedly, and in Windows Event Viewer you see an application log error
> similar to the following - try one of the following solutions to fix that.

Error Log:
> The machine-default permission settings do not grant Local Activation permission for the COM Server application 
> with CLSID {CLSID} and APPID {APPID} to the user from address LocalHost (Using LRPC) running in the application 
> container Microsoft.Windows.Cortana_1.8.12.15063_neutral_neutral_cw5n1h2txyewy SID ({SID}). 
> This security permission can be modified using the Component Services administrative tool.

[Solution 1](https://answers.microsoft.com/en-us/windows/forum/windows8_1-winapps/weather-application/e4630db3-50c2-4cc5-9813-f089494a1145?auth=1):
Edit Registry and Component Services permissions
1. Open Regedit.
2. Go to HKEY_Classes_Root\CLSID\*CLSID*.
	Note: *CLSID* stand for the ID that appears in your event viewer error. In your case, it's {C2F03A33-21F5-47FA-B4BB-156362A2F239}. 
3. Right click on it then select permission. 
4. Click Advance and change the owner to Administrators group. Also click the box that will appear below the owner line.
5. Apply full control.
6. Close the tab then go to HKEY_LocalMachine\Software\Classes\AppID\*APPID*.
	Note: *AppID* is the ID that appears in your event viewer. In your case it's {316CDED5-E4AE-4B15-9113-7055D84DCC97}.
7. Right click on it then select permission.
8. Click Advance and change the owner to Administrators group.
9. Click the box that will appear below the owner line.
10. Click Apply and grant full control to Administrators group.
11. Close all tabs and go to Administrative tool.
12. Open component services as an Administrator.
13. Click Computer, click my computer, then click DCOM.
14. Look for the corresponding service that appears on the error viewer.
	Note: For this step, look for the one that appeared at the right panel of the RegEdit. For example, the AppID Registry (316CDED5-E4AE-4B15-9113-7055D84DCC97) contains the "Immersive Shell" Data with a (Default) as a name. Now look for "Immersive Shell".
15. Right click on it then click properties.
16. Click security tab then click Customize in the Launch and Activation permissions section. Click Edit. Click Add. Add Local Service. Then apply.
17. Tick the Local Activation box.

[Solution 2](https://social.technet.microsoft.com/Forums/en-US/7742f039-70af-49b5-b37e-9597da743971/event-id-10016-the-applicationspecific-permission-settings-do-not-grant-local-activation?forum=win10itprogeneral):
Reset DCOM permissions

> The DCOM ACLs are stored in the registry under the key HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Ole, 
> in the following binary values:
+ DefaultAccessPermission
+ DefaultLaunchPermission
+ MachineAccessRestriction
+ MachineLaunchRestriction

> Backup the registry first, then delete all the values listed avove.
> DCOM will load the default settings if there are no values referenced.
