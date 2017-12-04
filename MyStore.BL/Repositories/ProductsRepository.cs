using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStore.BL.Models;
using MyStore.Data;

namespace MyStore.BL.Repositories
{
    public class ProductsRepository : IDataRepository<Product>
    {
        /// <summary>
        /// Returns an enumeration of all products
        /// </summary>
        /// <returns></returns>
        List<Product> IDataRepository<Product>.GetAll()
        {
            using (var ctx = new StoreContext())
            {
                return ctx.Products.ToList();
            }
        }

        /// <summary>
        /// Returns a single product by id
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The requested product or null if not found</returns>
        Product IDataRepository<Product>.GetById(uint id)
        {
            using (var ctx = new StoreContext())
            {
                return ctx.Products.FirstOrDefault(p => p.ProductId == id);
            }
        }

        /// <summary>
        /// Filters products based on a predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        List<Product> IDataRepository<Product>.GetByPredicate(
            Func<Product, bool> predicate)
        {
            using (var ctx = new StoreContext())
            {
                return ctx.Products.Where(predicate).ToList();
            }
        }

        /// <summary>
        /// Inserts a new product
        /// </summary>
        /// <param name="product">The product to add</param>
        /// <exception cref="ArgumentException">If the product already exists</exception>
        /// <returns>The inserted product</returns>
        Product IDataRepository<Product>.Add(Product product)
        {
            using (var ctx = new StoreContext())
            {
                if (ctx.Products.Contains(product))
                    throw new ArgumentException("Product already exists");

                ctx.Products.Add(product);
                ctx.SaveChanges();

                return product;
            }
        }

        /// <summary>
        /// Updates a product
        /// </summary>
        /// <param name="product">The product to update</param>
        /// <exception cref="ArgumentException">If the product does not exist</exception>
        /// <returns>Returns true upon success, false upon failure</returns>
        bool IDataRepository<Product>.Update(Product product)
        {
            using (var ctx = new StoreContext())
            {
                if (!ctx.Products.Contains(product))
                    throw new ArgumentException("Product does not exist");

                ctx.Products.Update(product);

                int res = ctx.SaveChanges();
                return res > 0 ? true : false;
            }
        }

        /// <summary>
        /// Removes a single product
        /// </summary>
        /// <param name="product"></param>
        /// <exception cref="ArgumentException">If the product does not exist</exception>
        /// <returns>Returns true upon success, false upon failure</returns>
        bool IDataRepository<Product>.Delete(Product product)
        {
            using (var ctx = new StoreContext())
            {
                if (!ctx.Products.Contains(product))
                    throw new ArgumentException("Product does not exist");

                ctx.Products.Remove(product);

                int res = ctx.SaveChanges();
                return res > 0 ? true : false;
            }
        }

        /// <summary>
        /// Deletes all products
        /// </summary>
        /// <returns>The number of deleted products</returns>
        int IDataRepository<Product>.Clear()
        {
            using (var ctx = new StoreContext())
            {
                ctx.Products.RemoveRange(ctx.Products.ToList());
                return ctx.SaveChanges();
            }
        }
    }
}
