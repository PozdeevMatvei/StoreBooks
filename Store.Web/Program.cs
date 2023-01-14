using Store;
using Store.Contractors;
using Store.DTO.EF;
using Store.Messages;
using Store.Web.App.Middlewares;
using Store.Web.Contractors;
using Store.YandexKassa;
using Store.Web.App.Services;
using Store.Web.App;
using System.Security.Claims;
using Store.Web.App.services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddEF(builder.Configuration.GetConnectionString("Store")!);
builder.Services.AddIdentityOptions();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Authorization/Index";
    options.AccessDeniedPath = "/Home/Index"; 
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("user", builder =>
    {
        builder.RequireClaim(ClaimTypes.Role, "user");
    });
});

builder.Services.AddSingleton<IBookRepository, BookRepository>();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IDeliveryService, PostamateDeliveryService>();
builder.Services.AddSingleton<IPaymentService, CashPaymentService>();
builder.Services.AddSingleton<INotificationService, DebugNotificationService>();
builder.Services.AddSingleton<IPaymentService, YandexKassaPaymentService>();
builder.Services.AddSingleton<IWebContractorService, YandexKassaPaymentService>();
builder.Services.AddSingleton<BookService>();
builder.Services.AddSingleton<OrderService>();
builder.Services.AddScoped<RegistrationService>(); 
builder.Services.AddScoped<AuthorizationService>();
builder.Services.AddSingleton<UserService>();

var app = builder.Build();

//app.UseMiddleware<ExceptionHendlingMiddleware>();
app.UseStaticFiles();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
//TODO ƒобавить страницу пользовател€
//TODO ѕроверить работоспособность регистрации на ошибки
//TODO ѕоправить вид страниц авторизации 
//TODO —делать возможность оформлени€ заказа без заполнени€ полей пользовател€ если авторизован