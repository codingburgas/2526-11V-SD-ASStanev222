using Microsoft.EntityFrameworkCore;
using ProjectLMS.Data;
using ProjectLMS.Services.Implementations;
using ProjectLMS.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add Entity Framework Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register application services
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<ITestService, TestService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
