var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession(options =>
{
    // Set session timeout to 30 minutes
    options.IdleTimeout = TimeSpan.FromMinutes(5);
});

// Add services to the container.
builder.Services.AddControllersWithViews();

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
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "Skill_Sheet_Manager",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
