using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Split_Receipt.Areas.Identity.Data;
using Split_Receipt.Data;
using Split_Receipt.Services;
using Split_Receipt.Services.Interfaces;
using Split_Receipt.Services.Mappers;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AuthDbContextConnection") ?? throw new InvalidOperationException("Connection string 'AuthDbContextConnection' not found.");

builder.Services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<AuthDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
});

builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<ICheckoutService,  CheckoutService>();
builder.Services.AddScoped<GroupMapper, GroupMapper>();
builder.Services.AddScoped<CheckoutMapper, CheckoutMapper>();
builder.Services.AddHttpClient("currency-api", c => 
{
    c.BaseAddress = new Uri("https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/latest/");
    c.DefaultRequestHeaders.Add("Accept", "application/json");
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
/*
app.MapControllerRoute(
    name: "checkout",
    pattern: "Checkouts/SaveCheckout/{groupId?}",
    defaults: new { controller = "Checkout", action = "SaveCheckout" });

*/
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "checkout",
        pattern: "Checkouts/SaveCheckout/{groupId?}",
        defaults: new { controller = "Checkout", action = "SaveCheckout" });

    endpoints.MapControllers();
    endpoints.MapRazorPages();
});

app.Run();
