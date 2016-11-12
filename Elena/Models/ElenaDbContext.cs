using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Elena.Models
{
    public class ElenaDbContext : DbContext
    {
        public ElenaDbContext(DbContextOptions<ElenaDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ForSqlServerToTable("product");
            modelBuilder.Entity<Customer>().ForSqlServerToTable("customer");
            modelBuilder.Entity<Address>().ForSqlServerToTable("address");
            modelBuilder.Entity<Order>().ForSqlServerToTable("order");
        }
    }
}
