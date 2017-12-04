using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStore.BL.Repositories;

namespace MyStore.BL.Models
{
    public class Product : Equatable<Product>, IEquatable<Product>, ICollectionItem
    {
        public Product()
        { }

        public Product(Catalog catalog)
        {
            CatalogId = catalog.CatalogId;
        }

        [Key]
        public uint ProductId { get; set; }

        [ForeignKey("Catalog")]
        public uint CatalogId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Column(TypeName = "blob")]
        public byte[] Image { get; set; }

        [Column(TypeName = "date")]
        public DateTimeOffset DatePublished { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// The current price of the product
        /// </summary>
        [Column(TypeName = "money")]
        public decimal ListPrice { get; set; }

        [Range(0, 100)]
        [Column(TypeName = "real")]
        public decimal DiscountPercentage { get; set; }

        public virtual Catalog Catalog { get; set; }

        [NotMapped]
        uint ICollectionItem.ParentId => CatalogId;

        bool IEquatable<Product>.Equals(Product other)
        {
            return ProductId == other.ProductId;
        }

        public override int GetHashCode()
        {
            return ProductId.GetHashCode();
        }
    }
}
