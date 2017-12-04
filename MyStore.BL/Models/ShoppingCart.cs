using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStore.BL.Models
{
    public class ShoppingCart : Equatable<ShoppingCart>, IEquatable<ShoppingCart>
    {
        public ShoppingCart()
        {
            Items = new HashSet<ShoppingCartItem>();
        }

        public ShoppingCart(Customer customer) : this()
        {
            CustomerId = customer.CustomerId;
        }

        [Key]
        public uint ShoppingCartId { get; set; }

        [ForeignKey("Customer")]
        public uint CustomerId { get; set; }

        [Column(TypeName = "date")]
        public DateTimeOffset? LastEdited { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual ICollection<ShoppingCartItem> Items { get; set; }


        bool IEquatable<ShoppingCart>.Equals(ShoppingCart other)
        {
            return ShoppingCartId == other.ShoppingCartId;
        }

        public override int GetHashCode()
        {
            return ShoppingCartId.GetHashCode();
        }
    }
}
