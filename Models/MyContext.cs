using Microsoft.EntityFrameworkCore;
 
namespace ActivityCenter.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users {get;set;}
        public DbSet<Actividad> Actividads {get;set;}
        public DbSet<GuestList> GuestLists {get;set;}
    }
}