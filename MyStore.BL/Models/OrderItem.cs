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
    public class OrderItem : Equatable<OrderItem>, IEquatable<OrderItem>, ICollectionItem
    {
        public OrderItem()
        { }

        public OrderItem(Order order, Product product, int quantity)
        {
            OrderId = order.OrderId;
            ProductId = product.ProductId;
            ListPrice = product.ListPrice;
            DiscountPercentage = product.DiscountPercentage;
            Quantity = quantity;
        }

        // [Key]
        [ForeignKey("Order")]
        public uint OrderId { get; set; }

        // [Key]
        [ForeignKey("Product")]
        public uint ProductId { get; set; }

        /// <summary>
        /// The price of the product as for the time of the order
        /// </summary>
        [Column(TypeName = "money")]
        public decimal ListPrice { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, 100)]
        [Column(TypeName = "real")]
        public decimal DiscountPercentage { get; set; }

        public virtual Order Order { get; set; }

        public virtual Product Product { get; set; }

        [NotMapped]
        uint ICollectionItem.ParentId => OrderId;

        bool IEquatable<OrderItem>.Equals(OrderItem other)
        {
            return (OrderId == other.OrderId && ProductId == other.ProductId);
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 31 + OrderId.GetHashCode();
            hash = hash * 31 + ProductId.GetHashCode();
            return hash;
        }
    }
}
