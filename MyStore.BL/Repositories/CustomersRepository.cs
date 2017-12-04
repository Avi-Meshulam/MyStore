using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStore.BL.Models;
using MyStore.Data;

namespace MyStore.BL.Repositories
{
    public class CustomersRepository : IDataRepository<Customer>
    {
        /// <summary>
        /// Returns an enumeration of all Customers
        /// </summary>
        /// <returns></returns>
        List<Customer> IDataRepository<Customer>.GetAll()
        {
            using (var ctx = new StoreContext())
            {
                return ctx.Customers.ToList();
            }
        }

        /// <summary>
        /// Returns a customer by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>The requested customer or null if not found</returns>
        Customer IDataRepository<Customer>.GetById(uint id)
        {
            using (var ctx = new StoreContext())
            {
                return ctx.Customers
                    .Where(c => c.CustomerId == id)
                    .SingleOrDefault();
            }
        }

        /// <summary>
        /// Filters customers based on a predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        List<Customer> IDataRepository<Customer>.GetByPredicate(
            Func<Customer, bool> predicate)
        {
            using (var ctx = new StoreContext())
            {
                return ctx.Customers.Where(predicate).ToList();
            }
        }

        /// <summary>
        /// Inserts a new Customer to store
        /// </summary>
        /// <param name="customer">The customer to add</param>
        /// <exception cref="ArgumentException">If the customer already exists</exception>
        /// <returns>The inserted customer or null in case of a failure</returns>
        Customer IDataRepository<Customer>.Add(Customer customer)
        {
            using (var ctx = new StoreContext())
            {
                if (ctx.Customers.Contains(customer))
                    throw new ArgumentException("Customer already exists");

                ctx.Customers.Add(customer);
                ctx.SaveChanges();

                return customer;
            }
        }

        /// <summary>
        /// Updates a Customer
        /// </summary>
        /// <param name="customer">The Customer to update</param>
        /// <exception cref="ArgumentException">If the customer does not exist</exception>
        /// <returns>Returns true upon success, false upon failure</returns>
        bool IDataRepository<Customer>.Update(Customer customer)
        {
            using (var ctx = new StoreContext())
            {
                if (!ctx.Customers.Contains(customer))
                    throw new ArgumentException("Customer does not exist");

                ctx.Customers.Update(customer);

                int res = ctx.SaveChanges();
                return res > 0 ? true : false;
            }
        }

        /// <summary>
        /// Removes a customer
        /// </summary>
        /// <param name="customer"></param>
        /// <exception cref="ArgumentException">If the customer does not exist</exception>
        /// <returns>Returns true upon success, false upon failure</returns>
        bool IDataRepository<Customer>.Delete(Customer customer)
        {
            using (var ctx = new StoreContext())
            {
                if (!ctx.Customers.Contains(customer))
                    throw new ArgumentException("Customer does not exist");

                ctx.Customers.Remove(customer);

                int res = ctx.SaveChanges();
                return res > 0 ? true : false;
            }
        }

        /// <summary>
        /// Deletes all customers
        /// </summary>
        /// <returns>The number of deleted customers</returns>
        int IDataRepository<Customer>.Clear()
        {
            using (var ctx = new StoreContext())
            {
                ctx.Customers.RemoveRange(ctx.Customers.ToList());
                return ctx.SaveChanges();
            }
        }
    }
}
