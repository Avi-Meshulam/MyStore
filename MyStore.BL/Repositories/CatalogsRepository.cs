using Microsoft.EntityFrameworkCore;
using MyStore.BL.Models;
using MyStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStore.BL.Repositories
{
    public class CatalogsRepository : IDataRepository<Catalog>
    {
        /// <summary>
        /// Returns an enumeration of all Catalogs
        /// </summary>
        /// <returns></returns>
        List<Catalog> IDataRepository<Catalog>.GetAll()
        {
            using (var ctx = new StoreContext())
            {
                return ctx.Catalogs
                    //.Include(c => c.Products)
                    .ToList();
            }
        }

        /// <summary>
        /// Returns a catalog by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>The requested catalog or null if not found</returns>
        Catalog IDataRepository<Catalog>.GetById(uint id)
        {
            using (var ctx = new StoreContext())
            {
                return ctx.Catalogs
                    .Include(c => c.Products)
                    .Where(c => c.CatalogId == id)
                    .SingleOrDefault();
            }
        }

        /// <summary>
        /// Filters catalogs based on a predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        List<Catalog> IDataRepository<Catalog>.GetByPredicate(
            Func<Catalog, bool> predicate)
        {
            using (var ctx = new StoreContext())
            {
                return ctx.Catalogs
                    //.Include(c => c.Products)
                    .Where(predicate)
                    .ToList();
            }
        }

        /// <summary>
        /// Inserts a new Catalog to store
        /// </summary>
        /// <param name="catalog">The catalog to add</param>
        /// <exception cref="ArgumentException">If the catalog already exists</exception>
        /// <returns>The inserted catalog or null in case of a failure</returns>
        Catalog IDataRepository<Catalog>.Add(Catalog catalog)
        {
            using (var ctx = new StoreContext())
            {
                if (ctx.Catalogs.Contains(catalog))
                    throw new ArgumentException("Catalog already exists");

                ctx.Products.Sync(catalog.Products);

                ctx.Catalogs.Add(catalog);
                ctx.SaveChanges();

                return catalog;
            }
        }

        /// <summary>
        /// Updates a Catalog
        /// </summary>
        /// <param name="catalog">The Catalog to update</param>
        /// <exception cref="ArgumentException">If the catalog does not exist</exception>
        /// <returns>Returns true upon success, false upon failure</returns>
        bool IDataRepository<Catalog>.Update(Catalog catalog)
        {
            using (var ctx = new StoreContext())
            {
                if (!ctx.Catalogs.Contains(catalog))
                    throw new ArgumentException("Catalog does not exist");

                ctx.Products.Sync(catalog.Products);

                catalog.LastEdited = DateTimeOffset.Now;

                ctx.Catalogs.Update(catalog);

                int res = ctx.SaveChanges();
                return res > 0 ? true : false;
            }
        }

        /// <summary>
        /// Removes a catalog
        /// </summary>
        /// <param name="catalog"></param>
        /// <exception cref="ArgumentException">If the catalog does not exist</exception>
        /// <returns>Returns true upon success, false upon failure</returns>
        bool IDataRepository<Catalog>.Delete(Catalog catalog)
        {
            using (var ctx = new StoreContext())
            {
                if (!ctx.Catalogs.Contains(catalog))
                    throw new ArgumentException("Catalog does not exist");

                ctx.Catalogs.Remove(catalog);

                int res = ctx.SaveChanges();
                return res > 0 ? true : false;
            }
        }

        /// <summary>
        /// Deletes all catalogs
        /// </summary>
        /// <returns>The number of deleted catalogs</returns>
        int IDataRepository<Catalog>.Clear()
        {
            using (var ctx = new StoreContext())
            {
                ctx.Catalogs.RemoveRange(ctx.Catalogs.ToList());
                return ctx.SaveChanges();
            }
        }
    }
}
