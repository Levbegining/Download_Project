var builder = WebApplication.CreateBuilder(args);
// Настройка URL, на котором приложение будет слушать (все интерфейсы, порт 5000)
builder.WebHost.UseUrls("http://0.0.0.0:5001");// LINUX SERVER FOR!!!
// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=FileUpload}/{action=Index}/{id?}");

app.Run();
