using System;

using Microsoft.EntityFrameworkCore;


namespace CRUDelicious.Models
{
    public class DishesContext : DbContext
    {
        public DishesContext(DbContextOptions options) : base(options) { }

        public DbSet<Dish> Dishes { get; set; }
    }
}
