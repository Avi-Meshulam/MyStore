using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStore.BL.Models
{
    public class ShoppingCartItem : Equatable<ShoppingCartItem>, IEquatable<ShoppingCartItem>, ICollectionItem
    {
        public ShoppingCartItem()
        { }

        public ShoppingCartItem(ShoppingCart shoppingCart, Product product, int quantity)
        {
            ShoppingCartId = shoppingCart.ShoppingCartId;
            ProductId = product.ProductId;
            Quantity = quantity;
        }

        // [Key]
        [ForeignKey("ShoppingCart")]
        public uint ShoppingCartId { get; set; }

        // [Key]
        [ForeignKey("Product")]
        public uint ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public virtual ShoppingCart ShoppingCart { get; set; }

        public virtual Product Product { get; set; }

        [NotMapped]
        uint ICollectionItem.ParentId => ShoppingCartId;

        bool IEquatable<ShoppingCartItem>.Equals(ShoppingCartItem other)
        {
            return ProductId == other.ProductId;
        }

        public override int GetHashCode()
        {
            return ProductId.GetHashCode();
        }
    }
}