using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AutomatedEducationProgram.Data;
using System.Net.Http;
using Microsoft.AspNetCore.Identity;
using AutomatedEducationProgram.Areas.Data;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set a suitable timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<SchoolContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolContext") ?? throw new InvalidOperationException("Connection string 'SchoolContext' not found.")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add HttpClient to the services
builder.Services.AddHttpClient();
var connectionString = builder.Configuration.GetConnectionString("AutomatedEducationProgramContextConnection");
builder.Services.AddDbContext<AutomatedEducationProgramUserContext>(options =>
    options.UseSqlServer(connectionString ?? throw new InvalidOperationException("Connection string not found.")));
builder.Services.AddDbContext<AutomatedEducationProgramContext>(options =>
    options.UseSqlServer(connectionString ?? throw new InvalidOperationException("Connection string not found.")));

builder.Services.AddIdentity<AEPUser, IdentityRole>()
                .AddEntityFrameworkStores<AutomatedEducationProgramUserContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();
builder.Services.TryAddScoped<SignInManager<AEPUser>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AutomatedEducationProgramContext>();
    context.Database.EnsureCreated();
    //DbInitializer.Initialize(context);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); 
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
