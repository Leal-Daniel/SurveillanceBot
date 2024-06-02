// <copyright file="HomeController.cs" company="Daniel-Leal">
// Copyright (c) Daniel-Leal. All rights reserved.
// </copyright>

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;
using System.Net.WebSockets;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.AspNetCore.Mvc;
using SurveillanceWebServer.Models;

namespace SurveillanceWebServer.Controllers;

/// <summary>
/// Home Controller class.
/// </summary>
/// <param name="logger">The logger.</param>
public class HomeController(ILogger<HomeController> logger) : Controller
{
  private readonly ILogger logger = logger;
  private readonly VideoCapture capture = new(0, VideoCapture.API.DShow);

  /// <summary>
  /// Goes to index view.
  /// </summary>
  /// <returns>View instance.</returns>
  public IActionResult Index()
    => this.View();

  /// <summary>
  /// Goes to resources view.
  /// </summary>
  /// <returns>View instance.</returns>
  public IActionResult Resources()
    => this.View();

  /// <summary>
  /// Goes to error view.
  /// </summary>
  /// <returns>View instance.</returns>
  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error()
  {
    var errorVm = new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier };
    return this.View(errorVm);
  }

  /// <summary>
  /// Function that is triggered to start live stream.
  /// </summary>
  /// <param name="context">The HTTP context of the call.</param>
  /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
  public async Task TriggerLiveStream(HttpContext context)
  {
    if (!context.WebSockets.IsWebSocketRequest)
    {
      context.Response.StatusCode = 400;
    }

    var websocket = await context.WebSockets.AcceptWebSocketAsync();
    if (this.capture == null)
    {
      context.Response.StatusCode = 500;
      return;
    }

    var frame = new Mat();
    while (websocket.State is WebSocketState.Open)
    {
      this.capture.Read(frame);
      if (frame.IsEmpty) continue;

      var bitmap = frame.ToBitmap();
      using var stream = new MemoryStream();
      bitmap.Save(stream, ImageFormat.Jpeg);
      var bytes = stream.ToArray();

      await websocket.SendAsync(bytes, WebSocketMessageType.Binary, true, CancellationToken.None);
    }
  }

  /// <summary>
  /// Gracefully closes and disposes the camera.
  /// </summary>
  public void CloseCamera()
  {
    this.capture.Release();
    this.capture.Dispose();
  }
}
