using SistemaVentas.Web.Services.Interfaces;
using SistemaVentas.Web.Services.Implementation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configurar sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Registrar HttpClient
builder.Services.AddHttpClient();

// Registrar servicios
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<IProductoApiService, ProductoApiService>();
builder.Services.AddScoped<IVentaApiService, VentaApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); 

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();