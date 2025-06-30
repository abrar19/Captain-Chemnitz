using backend.Models.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class APIDbContext: IdentityDbContext

{
    public  APIDbContext(DbContextOptions<APIDbContext> options):base(options)
    {
        
    }
    
    public DbSet<ApplicationUserModel> applicationUsers { get; set; }
    
    public DbSet<ProfileModel> profiles { get; set; }
}