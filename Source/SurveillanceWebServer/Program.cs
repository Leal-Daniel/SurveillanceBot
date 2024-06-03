// <copyright file="Program.cs" company="Daniel-Leal">
// Copyright (c) Daniel-Leal. All rights reserved.
// </copyright>

using SurveillanceWebServer.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Configure web host to listen on all IPs.
builder.WebHost.ConfigureKestrel((context, options) => options.ListenAnyIP(1225));

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the Video Feed Controller explicitly.
builder.Services.AddScoped<VideoFeedController>();

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
app.Map("/ws", async context =>
{
  var videoFeed = context.RequestServices.GetRequiredService<VideoFeedController>();
  await videoFeed.BeginVideo(context);
});
app.Run();