using System;
using Microsoft.EntityFrameworkCore;

using MyApp.Models;

namespace MyApp.Models
{
    public class FruitsContext : DbContext
    {
        public FruitsContext(DbContextOptions<FruitsContext> options) : base(options)
        {

        }

        public DbSet<Fruit> Fruits { get; set; }

    }
}
