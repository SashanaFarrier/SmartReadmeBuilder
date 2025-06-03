using DotNetEnv;
using SmartReadmeBuilder.api;
using SmartReadmeBuilder.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout to 30 minutes
    options.Cookie.HttpOnly = true; // Make the session cookie HTTP-only
    options.Cookie.IsEssential = true; // Make the session cookie essential for the application
});

Env.Load("./.env"); // Load environment variables from .env file

// Register GitHubToken as a singleton
//var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN") ?? throw new Exception("Token not found");
//builder.Services.AddSingleton<GitHubToken>(new GitHubToken { Token = token });

//builder.Services.AddTransient<GithubClient_API>(); 


//open ai api key - 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

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



app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
