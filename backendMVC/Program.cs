var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// HttpClient to call the Web API
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];
if (string.IsNullOrWhiteSpace(apiBaseUrl))
{
    apiBaseUrl = "http://localhost:5137/";
}

builder.Services.AddHttpClient("TaskApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Session for storing JWT token server-side
builder.Services.AddSession(options =>
{
    options.IdleTimeout        = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly    = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name:    "default",
    pattern: "{controller=Tasks}/{action=Login}/{id?}");

app.Run();
