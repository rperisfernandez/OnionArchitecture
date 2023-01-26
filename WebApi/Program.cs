using Application;
using Identity;
using Microsoft.OpenApi.Models;
using Persistence;
using Shared;
using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the containe
{
    var services = builder.Services;      
        
    services.AddApplicationLayer();
    services.AddPersistenceInfraestructure(builder.Configuration);
    services.AddSharedInfraestructure(builder.Configuration);
    services.AddIdentityInfrastructure(builder.Configuration);
    services.AddControllers();
    services.AddApiVersioningExtension();
    services.AddEndpointsApiExplorer();    
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPI", Version = "v1" });
    });
    services.AddMvc();
    
}

var app = builder.Build();

// Configure the HTTP request pipeline
{
    // global cors policy
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();        
    }
    else 
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI");
        });
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();
    app.UseErrorHandlingMiddleware();

    app.MapControllers();

    app.MigrateIdentity();
        
    //using var scope = app.Services.CreateScope();
    //var services2 = scope.ServiceProvider;
    //try
    //{
    //    var userManager = services2.GetRequiredService<UserManager<ApplicationUser>>();
    //    var roleManager = services2.GetRequiredService<RoleManager<IdentityRole>>();

    //    await DefaultRoles.SeedAsync(userManager, roleManager);
    //    await DefaultAdminUser.SeedAsync(userManager, roleManager);
    //    await DefaultBasicUser.SeedAsync(userManager, roleManager);
    //}
    //catch (Exception ex)
    //{
    //    throw;
    //}

}

app.Run();
