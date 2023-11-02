using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthProvider.Data
{
    public class AspNetIdentityDbContext: IdentityDbContext
    {
        public AspNetIdentityDbContext(DbContextOptions<AspNetIdentityDbContext> options):base(options)
        {
            
        }
    }
}
