using Microsoft.AspNetCore.Authentication.Cookies;
using SchoolManagement.Application.Services;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagement.Core.ServiceInterfaces;
using SchoolManagement.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder ( args );

// Add services to the container.
builder.Services.AddControllersWithViews ()
    .AddViewOptions ( options =>
    {
        options.HtmlHelperOptions.ClientValidationEnabled = true;
    } );

//.AddNewtonsoftJson ( options =>
//{
//    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
//} );


// Register HttpClient and HttpContextAccessor
builder.Services.AddHttpContextAccessor ();
builder.Services.AddTransient<JwtTokenHandler> ();

builder.Services.AddHttpClient ( "AuthorizedClient" )
    .AddHttpMessageHandler<JwtTokenHandler> ();

// Register session services.
builder.Services.AddDistributedMemoryCache ();
builder.Services.AddSession ( options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes ( 30 );
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
} );

// Enable Cookie Authentication.
builder.Services.AddAuthentication ( CookieAuthenticationDefaults.AuthenticationScheme )
    .AddCookie ( options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    } );

var app = builder.Build ();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment ())
{
    app.UseExceptionHandler ( "/Home/Error" );
    app.UseHsts ();
}

app.UseHttpsRedirection ();
app.UseStaticFiles ();
app.UseRouting ();

// Session middleware must be added before Authentication.
app.UseSession ();
app.UseAuthentication ();
app.UseAuthorization ();

app.MapControllerRoute (
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}" );

app.Run ();
