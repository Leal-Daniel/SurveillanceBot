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
  private VideoCapture? capture;
  private bool isInitialized;

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
    this.InitializeCapture();

    if (!context.WebSockets.IsWebSocketRequest || this.capture == null)
    {
      context.Response.StatusCode = 400;
      return;
    }

    var frame = new Mat();
    var websocket = await context.WebSockets.AcceptWebSocketAsync();
    try
    {
      while (true)
      {
        this.capture.Read(frame);
        if (frame.IsEmpty) continue;
        if (websocket.State is WebSocketState.CloseSent
          or WebSocketState.CloseReceived
          or WebSocketState.Closed
          or WebSocketState.Aborted)
        {
          this.Dispose();
          break;
        }

        var bitmap = frame.ToBitmap();
        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Jpeg);
        var bytes = stream.ToArray();

        await websocket.SendAsync(bytes, WebSocketMessageType.Binary, true, CancellationToken.None);
      }
    }
    catch (Exception ex)
    {
      this.logger.LogError("{ExType} was thrown with message: {Message}", ex.GetType(), ex.Message);
    }
    finally
    {
      this.capture?.Release();
      this.capture?.Dispose();
      this.isInitialized = false;
    }
  }

  private void InitializeCapture()
  {
    if (!this.isInitialized)
    {
      this.capture = new VideoCapture(0, VideoCapture.API.DShow);
      this.isInitialized = true;
    }
  }
}
