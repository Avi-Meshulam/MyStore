using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStore.BL.Models
{
    public class Customer : Equatable<Customer>, IEquatable<Customer>
    {
        [Key]
        public uint CustomerId { get; set; }

        [Required]
        public string NonRoamableId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Column(TypeName = "date")]
        public DateTimeOffset DateEnlisted { get; set; } = DateTimeOffset.Now;

        [Column(TypeName = "date")]
        public DateTimeOffset? BirthDate { get; set; }

        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; }

        // Orders made by customer
        public virtual ICollection<Order> Orders { get; set; }


        bool IEquatable<Customer>.Equals(Customer other)
        {
            return CustomerId == other.CustomerId;
        }

        public override int GetHashCode()
        {
            return CustomerId.GetHashCode();
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }
}