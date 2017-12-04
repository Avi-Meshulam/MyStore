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
    /// <summary>
    /// Items repository of a specific order
    /// </summary>
    public class OrderItemsRepository : IDataRepository<OrderItem>
    {
        /// <summary>
        /// Returns an enumeration of all order items
        /// </summary>
        /// <returns></returns>
        List<OrderItem> IDataRepository<OrderItem>.GetAll()
        {
            using (var ctx = new StoreContext())
            {
                return ctx.OrderItems
                    //.Include(i => i.Order)
                    //.Include(i => i.Product)
                    .ToList();
            }
        }

        /// <summary>
        /// Throws <see cref="NotImplementedException"/>. Use GetByPredicate instead.
        /// </summary>
        /// <param name="Id"></param>
        /// <exception cref="NotImplementedException"></exception>
        OrderItem IDataRepository<OrderItem>.GetById(uint id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a single order item by key
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public OrderItem GetByKey(uint orderId, uint productId)
        {
            using (var ctx = new StoreContext())
            {
                var orderItem = ctx.OrderItems
                    .Include(i => i.Order)
                    .Include(i => i.Product)
                    .SingleOrDefault(i => i.OrderId == orderId && i.ProductId == productId);
                return orderItem;
            }
        }

        /// <summary>
        /// Filters order items based on a predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        List<OrderItem> IDataRepository<OrderItem>.GetByPredicate(
            Func<OrderItem, bool> predicate)
        {
            using (var ctx = new StoreContext())
            {
                return ctx.OrderItems
                    .Include(i => i.Order)
                    .Include(i => i.Product)
                    .Where(predicate).ToList();
            }
        }

        /// <summary>
        /// Inserts a new order item. Updates quantity if item already exists.
        /// </summary>
        /// <param name="orderItem">The ordder item to add</param>
        /// <returns>The inserted product or null in case of a failure</returns>
        OrderItem IDataRepository<OrderItem>.Add(OrderItem orderItem)
        {
            using (var ctx = new StoreContext())
            {
                var item = ctx.OrderItems.SingleOrDefault(i => i.Equals(orderItem));

                if (item != null)
                    item.Quantity += orderItem.Quantity;
                else
                    ctx.OrderItems.Add(orderItem);

                ctx.SaveChanges();

                return item ?? orderItem;
            }
        }

        /// <summary>
        /// Updates an order item
        /// </summary>
        /// <param name="orderItem">The order item to update</param>
        /// <exception cref="ArgumentException">If order item does not exist</exception>
        /// <returns>Returns true upon success, false upon failure</returns>
        bool IDataRepository<OrderItem>.Update(OrderItem orderItem)
        {
            using (var ctx = new StoreContext())
            {
                if (!ctx.OrderItems.Contains(orderItem))
                    throw new ArgumentException("Item does not exist in order");

                ctx.OrderItems.Update(orderItem);

                int res = ctx.SaveChanges();
                return res > 0 ? true : false;
            }
        }

        /// <summary>
        /// Deletes an order item
        /// </summary>
        /// <param name="orderItem"></param>
        /// <exception cref="ArgumentException">If order item does not exist</exception>
        /// <returns>Returns true upon success, false upon failure</returns>
        bool IDataRepository<OrderItem>.Delete(OrderItem orderItem)
        {
            using (var ctx = new StoreContext())
            {
                if (!ctx.OrderItems.Contains(orderItem))
                    throw new ArgumentException("Item does not exist in order");

                ctx.OrderItems.Remove(orderItem);

                int res = ctx.SaveChanges();
                return res > 0 ? true : false;
            }
        }

        /// <summary>
        /// Deletes all orders items
        /// </summary>
        /// <returns>The number of deleted order items</returns>
        int IDataRepository<OrderItem>.Clear()
        {
            using (var ctx = new StoreContext())
            {
                ctx.OrderItems.RemoveRange(ctx.OrderItems.ToList());
                return ctx.SaveChanges();
            }
        }
    }
}
