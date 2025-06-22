using backend.Models.Entity;
using Microsoft.AspNetCore.Identity;

namespace backend.Data;

public class DBSeed
{
    public static async Task SeedData(IApplicationBuilder app,ConfigurationManager config)
{

    using var scope = app.ApplicationServices.CreateScope();


    var logger = scope.ServiceProvider.GetRequiredService<ILogger<DBSeed>>();

    try
    {
        //creating roles and superuser/adminuser
        var UserManager = scope.ServiceProvider.GetService<UserManager<ApplicationUserModel>>();
        var RoleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

        String email = config.GetSection("UserSettings")["UserEmail"];
        String password = config.GetSection("UserSettings")["UserPassword"];
        
        string[] roleNames = { "Admin", "AppUsers" };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {

            var roleExist = await RoleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var poweruser = new ApplicationUserModel
        {
            UserName = email,
            Email = email
        };

        string userPassword = password;
        var _user = await UserManager.FindByEmailAsync(email);
        var code = await UserManager.GenerateEmailConfirmationTokenAsync(poweruser);
        var result = await UserManager.ConfirmEmailAsync(poweruser, code);

        if (_user == null)
        {
            var createProwerUser = await UserManager.CreateAsync(poweruser, userPassword);
            if (createProwerUser.Succeeded)
            {
                await UserManager.AddToRoleAsync(poweruser, "Admin");
            }
        }
        
        //prepare database


        
        
    }

      catch (Exception ex)
    {
        logger.LogCritical(ex.Message);
    }

    }
}