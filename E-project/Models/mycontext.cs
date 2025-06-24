using Microsoft.EntityFrameworkCore;

namespace E_project.Models
{
    public class mycontext : DbContext
    {
        public mycontext(DbContextOptions<mycontext> options) : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Advertiser> Advertisers { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Admin> Admins { get; set; }
    }
}
