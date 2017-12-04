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
    public class OrdersRepository : IDataRepository<Order>
    {
        /// <summary>
        /// Returns an enumeration of all orders
        /// </summary>
        /// <returns></returns>
        List<Order> IDataRepository<Order>.GetAll()
        {
            using (var ctx = new StoreContext())
            {
                return ctx.Orders
                    //.Include(o => o.Customer)
                    //.Include(o => o.Items)
                        //.ThenInclude(i => i.Product)
                    .ToList();
            }
        }

        /// <summary>
        /// Returns a single order by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>The requested order or null if not found</returns>
        Order IDataRepository<Order>.GetById(uint id)
        {
            using (var ctx = new StoreContext())
            {
                var order = ctx.Orders
                    //.Include(o => o.Customer)
                    .Include(o => o.Items)
                        //.ThenInclude(i => i.Product)
                    .SingleOrDefault(p => p.OrderId == id);

                return order;
            }
        }

        /// <summary>
        /// Filters orders based on a predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        List<Order> IDataRepository<Order>.GetByPredicate(Func<Order, bool> predicate)
        {
            using (var ctx = new StoreContext())
            {
                return ctx.Orders
                    //.Include(o => o.Customer)
                    .Include(o => o.Items)
                        //.ThenInclude(i => i.Product)
                    .Where(predicate).ToList();
            }
        }

        /// <summary>
        /// Inserts a new order
        /// </summary>
        /// <param name="order">The order to add</param>
        /// <exception cref="ArgumentException">If the order already exists</exception>
        /// <returns>The inserted order or null in case of a failure</returns>
        Order IDataRepository<Order>.Add(Order order)
        {
            using (var ctx = new StoreContext())
            {
                if (ctx.Orders.Contains(order))
                    throw new ArgumentException("Order already exists");

                ctx.OrderItems.Sync(order.Items);

                ctx.Orders.Add(order);
                ctx.SaveChanges();

                return order;
            }
        }

        /// <summary>
        /// Updates an order
        /// </summary>
        /// <param name="order">The order to update</param>
        /// <exception cref="ArgumentException">If the order does not exist</exception>
        /// <returns>Returns true upon success, false upon failure</returns>
        bool IDataRepository<Order>.Update(Order order)
        {
            using (var ctx = new StoreContext())
            {
                if (!ctx.Orders.Contains(order))
                    throw new ArgumentException("Order does not exist");

                ctx.OrderItems.Sync(order.Items);

                order.LastEdited = DateTimeOffset.Now;

                ctx.Orders.Update(order);

                int res = ctx.SaveChanges();
                return res > 0 ? true : false;
            }
        }

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="order"></param>
        /// <exception cref="ArgumentException">If the order does not exist</exception>
        /// <returns>Returns true upon success, false upon failure</returns>
        bool IDataRepository<Order>.Delete(Order order)
        {
            using (var ctx = new StoreContext())
            {
                if (!ctx.Orders.Contains(order))
                    throw new ArgumentException("Order does not exist");

                ctx.Orders.Remove(order);

                int res = ctx.SaveChanges();
                return res > 0 ? true : false;
            }
        }

        /// <summary>
        /// Deletes all orders
        /// </summary>
        /// <returns>The number of deleted orders</returns>
        int IDataRepository<Order>.Clear()
        {
            using (var ctx = new StoreContext())
            {
                ctx.Orders.RemoveRange(ctx.Orders.ToList());
                return ctx.SaveChanges();
            }
        }
    }
}
