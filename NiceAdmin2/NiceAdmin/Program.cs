using NiceAdmin;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IEmailSender, EmailSender>();

// Add services to the container.
builder.Services.AddControllersWithViews();

/*// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Mark the session cookie as essential
});*/

//Login
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable session middleware before authentication and authorization
app.UseSession();

/*app.UseAuthentication();*/
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=UserLogin}/{id?}");

app.Run();
