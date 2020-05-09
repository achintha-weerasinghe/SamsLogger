using Microsoft.EntityFrameworkCore;

namespace samsLoggerTestApi.Data
{
    public class ApiDbContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }
    }
}