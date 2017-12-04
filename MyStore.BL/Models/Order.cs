using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStore.BL.Models
{
    public class Order : Equatable<Order>, IEquatable<Order>
    {
        public Order()
        {
            Items = new HashSet<OrderItem>();
        }

        public Order(Customer customer) : this()
        {
            CustomerId = customer.CustomerId;
        }

        [Key]
        public uint OrderId { get; set; }

        [ForeignKey("Customer")]
        public uint CustomerId { get; set; }

        [Column(TypeName = "date")]
        public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.Now;

        [Column(TypeName = "date")]
        public DateTimeOffset? LastEdited { get; set; }

        [Required]
        public OrderState State { get; set; } = OrderState.Open;
        
        public virtual Customer Customer { get; set; }

        public virtual ICollection<OrderItem> Items { get; set; }


        bool IEquatable<Order>.Equals(Order other)
        {
            return OrderId == other.OrderId;
        }

        public override int GetHashCode()
        {
            return OrderId.GetHashCode();
        }
    }
}