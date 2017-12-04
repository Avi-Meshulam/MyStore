using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStore.BL.Models;
using MyStore.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MyStore.BL.Repositories
{
    public class ShoppingCartsRepository : IDataRepository<ShoppingCart>
    {
        /// <summary>
        /// Returns an enumeration of all shopping carts
        /// </summary>
        /// <returns></returns>
        List<ShoppingCart> IDataRepository<ShoppingCart>.GetAll()
        {
            using (var ctx = new StoreContext())
            {
                return ctx.ShoppingCarts
                    //.Include(sh => sh.Customer)
                    //.Include(sh => sh.Items)
                        //.ThenInclude(i => i.Product)
                    .ToList();
            }
        }

        /// <summary>
        /// Returns a single shopping cart by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>The requested shopping cart or null if not found</returns>
        ShoppingCart IDataRepository<ShoppingCart>.GetById(uint id)
        {
            using (var ctx = new StoreContext())
            {
                var shoppingCart = ctx.ShoppingCarts
                    //.Include(sh => sh.Customer)
                    .Include(sh => sh.Items)
                        .ThenInclude(i => i.Product)
                    .SingleOrDefault(p => p.ShoppingCartId == id);

                return shoppingCart;
            }
        }

        /// <summary>
        /// Filters shopping carts based on a predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        List<ShoppingCart> IDataRepository<ShoppingCart>.GetByPredicate(
            Func<ShoppingCart, bool> predicate)
        {
            using (var ctx = new StoreContext())
            {
                return ctx.ShoppingCarts
                    //.Include(sh => sh.Customer)
                    .Include(sh => sh.Items)
                        .ThenInclude(i => i.Product)
                    .Where(predicate).ToList();
            }
        }

        /// <summary>
        /// Inserts a new shopping cart
        /// </summary>
        /// <param name="shoppingCart">The shoppingCart to add</param>
        /// <exception cref="ArgumentException">If the shopping cart already exists</exception>
        /// <returns>The inserted shopping cart or null in case of a failure</returns>
        ShoppingCart IDataRepository<ShoppingCart>.Add(ShoppingCart shoppingCart)
        {
            using (var ctx = new StoreContext())
            {
                if (ctx.ShoppingCarts.Contains(shoppingCart))
                    throw new ArgumentException("Shopping cart already exists");

                ctx.ShoppingCartItems.Sync(shoppingCart.Items);

                ctx.ShoppingCarts.Add(shoppingCart);
                ctx.SaveChanges();

                return shoppingCart;
            }
        }

        /// <summary>
        /// Updates a shopping cart
        /// </summary>
        /// <param name="shoppingCart">The shoppingCart to update</param>
        /// <exception cref="ArgumentException">If the shopping cart does not exist</exception>
        /// <returns>Returns true upon success, false upon failure</returns>
        bool IDataRepository<ShoppingCart>.Update(ShoppingCart shoppingCart)
        {
            using (var ctx = new StoreContext())
            {
                if (!ctx.ShoppingCarts.Contains(shoppingCart))
                    throw new ArgumentException("Shopping cart does not exist");

                ctx.ShoppingCartItems.Sync(shoppingCart.Items);

                shoppingCart.LastEdited = DateTimeOffset.Now;

                ctx.ShoppingCarts.Update(shoppingCart);

                int res = ctx.SaveChanges();
                return res > 0 ? true : false;
            }
        }

        /// <summary>
        /// Deletes a shopping cart
        /// </summary>
        /// <param name="shoppingCart"></param>
        /// <exception cref="ArgumentException">If the shopping cart does not exist</exception>
        /// <returns>Returns true upon success, false upon failure</returns>
        bool IDataRepository<ShoppingCart>.Delete(ShoppingCart shoppingCart)
        {
            using (var ctx = new StoreContext())
            {
                if (!ctx.ShoppingCarts.Contains(shoppingCart))
                    throw new ArgumentException("Shopping cart does not exist");

                ctx.ShoppingCarts.Remove(shoppingCart);

                int res = ctx.SaveChanges();
                return res > 0 ? true : false;
            }
        }

        /// <summary>
        /// Deletes all shopping carts
        /// </summary>
        /// <returns>The number of deleted shopping carts</returns>
        int IDataRepository<ShoppingCart>.Clear()
        {
            using (var ctx = new StoreContext())
            {
                ctx.ShoppingCarts.RemoveRange(ctx.ShoppingCarts.ToList());
                return ctx.SaveChanges();
            }
        }
    }
}
