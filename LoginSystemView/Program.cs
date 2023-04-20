using NToastNotify;
using AspNetCoreHero.ToastNotification;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection.Extensions;

using LoginSystemView.Services.IServices;
using LoginSystemView.Services;
using AspNetCoreHero.ToastNotification.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//Authentication using cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/UserRegister/Index";//"/Forbidden/";
       // options.LoginPath = new PathString("/UserRegister/Index");

    });
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(20);
});
//builder.Services.AddRazorPages().AddNToastNotifyNoty(new NotyOptions
//{
//    ProgressBar = true,
//    Timeout = 1000
//});
builder.Services.AddNotyf(configure =>
{
    configure.DurationInSeconds = 5;
    configure.IsDismissable = true;
    configure.Position = NotyfPosition.TopRight;

});

builder.Services.AddHttpClient<IUserRoles, UserRoles>();
builder.Services.AddScoped<IUserRoles, UserRoles>();
builder.Services.AddHttpClient<IUsersPofiles, UsersProfiles>();
builder.Services.AddScoped<IUsersPofiles, UsersProfiles>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

//app.MapRazorPages();
app.UseNotyf();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=UserRegister}/{action=Index}/{id?}");

app.Run();
