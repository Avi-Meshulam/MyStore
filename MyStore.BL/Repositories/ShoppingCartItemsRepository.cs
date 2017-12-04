using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStore.BL.Models;
using MyStore.Data;
using Microsoft.EntityFrameworkCore;

namespace MyStore.BL.Repositories
{
    public class ShoppingCartItemsRepository : IDataRepository<ShoppingCartItem>
    {
        /// <summary>
        /// Returns an enumeration of all shopping cart items
        /// </summary>
        /// <returns></returns>
        List<ShoppingCartItem> IDataRepository<ShoppingCartItem>.GetAll()
        {
            using (var ctx = new StoreContext())
            {
                return ctx.ShoppingCartItems
                    //.Include(i => i.ShoppingCart)
                    //.Include(i => i.Product)
                    .ToList();
            }
        }

        /// <summary>
        /// Throws <see cref="NotImplementedException"/>. Use GetByPredicate instead.
        /// </summary>
        /// <param name="Id"></param>
        /// <exception cref="NotImplementedException"></exception>
        ShoppingCartItem IDataRepository<ShoppingCartItem>.GetById(uint id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a single shopping cart item by key
        /// </summary>
        /// <param name="shoppingCartId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public ShoppingCartItem GetByKey(uint shoppingCartId, uint productId)
        {
            using (var ctx = new StoreContext())
            {
                var shoppingCartItem = ctx.ShoppingCartItems
                    .Include(i => i.ShoppingCart)
                    .Include(i => i.Product)
                    .SingleOrDefault(i => i.ShoppingCartId == shoppingCartId 
                        && i.ProductId == productId);

                return shoppingCartItem;
            }
        }

        /// <summary>
        /// Filters shopping cart items based on a predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        List<ShoppingCartItem> IDataRepository<ShoppingCartItem>.GetByPredicate(
            Func<ShoppingCartItem, bool> predicate)
        {
            using (var ctx = new StoreContext())
            {
                return ctx.ShoppingCartItems
                    .Include(i => i.ShoppingCart)
                    .Include(i => i.Product)
                    .Where(predicate).ToList();
            }
        }

        /// <summary>
        /// Inserts a new item to shopping cart. Updates quantity if item already exists.
        /// </summary>
        /// <param name="shoppingCartItem">The item to add to shopping cart</param>
        /// <returns>The inserted item or null in case of a failure</returns>
        ShoppingCartItem IDataRepository<ShoppingCartItem>.Add(ShoppingCartItem shoppingCartItem)
        {
            using (var ctx = new StoreContext())
            {
                var item = ctx.ShoppingCartItems.SingleOrDefault(i => i.Equals(shoppingCartItem));

                if (item != null)
                    item.Quantity += shoppingCartItem.Quantity;
                else
                    ctx.ShoppingCartItems.Add(shoppingCartItem);

                ctx.SaveChanges();

                return item ?? shoppingCartItem;
            }
        }

        /// <summary>
        /// Updates a shopping cart item
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item to update</param>
        /// <exception cref="ArgumentException">If item does not exist in shopping cart</exception>
        /// <returns>Returns true upon success, false upon failure</returns>
        bool IDataRepository<ShoppingCartItem>.Update(ShoppingCartItem shoppingCartItem)
        {
            using (var ctx = new StoreContext())
            {
                if (!ctx.ShoppingCartItems.Contains(shoppingCartItem))
                    throw new ArgumentException("Item does not exist in shopping cart");

                ctx.ShoppingCartItems.Update(shoppingCartItem);

                int res = ctx.SaveChanges();
                return res > 0 ? true : false;
            }
        }

        /// <summary>
        /// Deletes a shopping cart item
        /// </summary>
        /// <param name="shoppingCartItem"></param>
        /// <exception cref="ArgumentException">If item does not exist in shopping cart</exception>
        /// <returns>Returns true upon success, false upon failure</returns>
        bool IDataRepository<ShoppingCartItem>.Delete(ShoppingCartItem shoppingCartItem)
        {
            using (var ctx = new StoreContext())
            {
                if (!ctx.ShoppingCartItems.Contains(shoppingCartItem))
                    throw new ArgumentException("Item does not exist in shopping cart");

                ctx.ShoppingCartItems.Remove(shoppingCartItem);
                int res = ctx.SaveChanges();
                return res > 0 ? true : false;
            }
        }

        /// <summary>
        /// Deletes all shopping cart items
        /// </summary>
        /// <returns>The number of deleted items</returns>
        int IDataRepository<ShoppingCartItem>.Clear()
        {
            using (var ctx = new StoreContext())
            {
                ctx.ShoppingCartItems.RemoveRange(ctx.ShoppingCartItems.ToList());
                return ctx.SaveChanges();
            }
        }
    }
}
