using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStore.BL.Models
{
    public class Catalog : Equatable<Catalog>, IEquatable<Catalog>
    {
        public Catalog()
        {
            Products = new Collection<Product>();
        }

        [Key]
        public uint CatalogId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Column(TypeName = "date")]
        public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.Now;

        [Column(TypeName = "date")]
        public DateTimeOffset? LastEdited { get; set; }

        public virtual ICollection<Product> Products { get; set; }


        bool IEquatable<Catalog>.Equals(Catalog other)
        {
            return CatalogId == other.CatalogId;
        }

        public override int GetHashCode()
        {
            return CatalogId.GetHashCode();
        }
    }
}
