// <copyright file="Program.cs" company="Daniel-Leal">
// Copyright (c) Daniel-Leal. All rights reserved.
// </copyright>

using SurveillanceWebServer.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Configure web host to listen on all IPs.
builder.WebHost.ConfigureKestrel((context, options) => options.ListenAnyIP(5001));

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the HomeController explicitly.
builder.Services.AddScoped<HomeController>();

// Configure the HTTP request pipeline.
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}

app.UseWebSockets();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

// If websocket is requested, begin video feed for client.
app.Map("/ws", async context => await context.RequestServices.GetRequiredService<HomeController>().TriggerLiveStream(context));
app.Run();