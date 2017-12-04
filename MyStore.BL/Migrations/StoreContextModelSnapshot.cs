using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using MyStore.Data;
using MyStore.BL.Models;

namespace MyStore.BL.Migrations
{
    [DbContext(typeof(StoreContext))]
    partial class StoreContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("MyStore.BL.Models.Catalog", b =>
                {
                    b.Property<uint>("CatalogId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("DateCreated")
                        .HasColumnType("date");

                    b.Property<string>("Description")
                        .HasMaxLength(500);

                    b.Property<DateTimeOffset?>("LastEdited")
                        .HasColumnType("date");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("CatalogId");

                    b.ToTable("Catalogs");
                });

            modelBuilder.Entity("MyStore.BL.Models.Customer", b =>
                {
                    b.Property<uint>("CustomerId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset?>("BirthDate")
                        .HasColumnType("date");

                    b.Property<DateTimeOffset>("DateEnlisted")
                        .HasColumnType("date");

                    b.Property<string>("Email")
                        .HasMaxLength(50);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("NonRoamableId")
                        .IsRequired();

                    b.HasKey("CustomerId");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("MyStore.BL.Models.Order", b =>
                {
                    b.Property<uint>("OrderId")
                        .ValueGeneratedOnAdd();

                    b.Property<uint>("CustomerId");

                    b.Property<DateTimeOffset>("DateCreated")
                        .HasColumnType("date");

                    b.Property<DateTimeOffset?>("LastEdited")
                        .HasColumnType("date");

                    b.Property<int>("State");

                    b.HasKey("OrderId");

                    b.HasIndex("CustomerId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("MyStore.BL.Models.OrderItem", b =>
                {
                    b.Property<uint>("OrderId");

                    b.Property<uint>("ProductId");

                    b.Property<decimal>("DiscountPercentage")
                        .HasColumnType("real");

                    b.Property<decimal>("ListPrice")
                        .HasColumnType("money(18,0)");

                    b.Property<int>("Quantity");

                    b.HasKey("OrderId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("MyStore.BL.Models.Product", b =>
                {
                    b.Property<uint>("ProductId")
                        .ValueGeneratedOnAdd();

                    b.Property<uint>("CatalogId");

                    b.Property<DateTimeOffset>("DatePublished")
                        .HasColumnType("date");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<decimal>("DiscountPercentage")
                        .HasColumnType("real");

                    b.Property<byte[]>("Image")
                        .HasColumnType("blob");

                    b.Property<decimal>("ListPrice")
                        .HasColumnType("money(18,0)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("ProductId");

                    b.HasIndex("CatalogId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("MyStore.BL.Models.ShoppingCart", b =>
                {
                    b.Property<uint>("ShoppingCartId")
                        .ValueGeneratedOnAdd();

                    b.Property<uint>("CustomerId");

                    b.Property<DateTimeOffset?>("LastEdited")
                        .HasColumnType("date");

                    b.HasKey("ShoppingCartId");

                    b.HasIndex("CustomerId");

                    b.ToTable("ShoppingCarts");
                });

            modelBuilder.Entity("MyStore.BL.Models.ShoppingCartItem", b =>
                {
                    b.Property<uint>("ShoppingCartId");

                    b.Property<uint>("ProductId");

                    b.Property<int>("Quantity");

                    b.HasKey("ShoppingCartId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("ShoppingCartItems");
                });

            modelBuilder.Entity("MyStore.BL.Models.Order", b =>
                {
                    b.HasOne("MyStore.BL.Models.Customer", "Customer")
                        .WithMany("Orders")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyStore.BL.Models.OrderItem", b =>
                {
                    b.HasOne("MyStore.BL.Models.Order", "Order")
                        .WithMany("Items")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyStore.BL.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyStore.BL.Models.Product", b =>
                {
                    b.HasOne("MyStore.BL.Models.Catalog", "Catalog")
                        .WithMany("Products")
                        .HasForeignKey("CatalogId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyStore.BL.Models.ShoppingCart", b =>
                {
                    b.HasOne("MyStore.BL.Models.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyStore.BL.Models.ShoppingCartItem", b =>
                {
                    b.HasOne("MyStore.BL.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyStore.BL.Models.ShoppingCart", "ShoppingCart")
                        .WithMany("Items")
                        .HasForeignKey("ShoppingCartId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
