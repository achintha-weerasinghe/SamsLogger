using Microsoft.EntityFrameworkCore;

namespace samsLoggerTestApi.Data
{
    public class MyDbContext : DbContext
    {
        public DbSet<MyLog> MyLogs { get; set; }
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }
    }
}