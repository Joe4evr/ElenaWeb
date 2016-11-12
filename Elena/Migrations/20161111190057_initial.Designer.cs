using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Elena.Models;

namespace Elena.Migrations
{
    [DbContext(typeof(ElenaDbContext))]
    [Migration("20161111190057_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1");

            modelBuilder.Entity("Elena.Models.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City")
                        .IsRequired();

                    b.Property<string>("Country")
                        .IsRequired();

                    b.Property<string>("Number")
                        .IsRequired();

                    b.Property<string>("PostalCode")
                        .IsRequired();

                    b.Property<string>("Streetname")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Addresses");

                    b.HasAnnotation("SqlServer:TableName", "address");
                });

            modelBuilder.Entity("Elena.Models.Customer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AddressId")
                        .IsRequired();

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("VatNumber");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.ToTable("Customers");

                    b.HasAnnotation("SqlServer:TableName", "customer");
                });

            modelBuilder.Entity("Elena.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CustomerId")
                        .IsRequired();

                    b.Property<DateTime>("Date");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("Orders");

                    b.HasAnnotation("SqlServer:TableName", "order");
                });

            modelBuilder.Entity("Elena.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BuyerId");

                    b.Property<string>("Description");

                    b.Property<string>("ImagePath");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int?>("OrderId");

                    b.Property<decimal>("Price");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("BuyerId");

                    b.HasIndex("OrderId");

                    b.ToTable("Products");

                    b.HasAnnotation("SqlServer:TableName", "product");
                });

            modelBuilder.Entity("Elena.Models.Customer", b =>
                {
                    b.HasOne("Elena.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Elena.Models.Order", b =>
                {
                    b.HasOne("Elena.Models.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Elena.Models.Product", b =>
                {
                    b.HasOne("Elena.Models.Customer", "Buyer")
                        .WithMany()
                        .HasForeignKey("BuyerId");

                    b.HasOne("Elena.Models.Order")
                        .WithMany("Products")
                        .HasForeignKey("OrderId");
                });
        }
    }
}
