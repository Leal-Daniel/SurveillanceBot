// <copyright file="HomeController.cs" company="Daniel-Leal">
// Copyright (c) Daniel-Leal. All rights reserved.
// </copyright>

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using OpenCvSharp;
using SurveillanceWebServer.Models;

namespace SurveillanceWebServer.Controllers;

/// <summary>
/// Home Controller class.
/// </summary>
/// <param name="logger">The logger.</param>
public class HomeController(ILogger<HomeController> logger) : Controller
{
  private VideoCapture? Capture { get; set; }
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
  /// Route to the video feed live stream.
  /// </summary>
  /// <returns>The video feed task.</returns>
  [HttpGet]
  [Route("/VideoFeed")]
  public async Task VideoFeed()
  {
    this.Response.Headers.Append("Content-Type", "multipart/x-mixed-replace; boundary=frame");

    if (!this.isInitialized)
    {
      this.Capture = new VideoCapture(0, VideoCaptureAPIs.DSHOW);
      this.isInitialized = true;
    }

    while (this.Capture is not null && this.Capture.IsOpened())
    {
      using var frame = new Mat();
      this.Capture.Read(frame);

      if (!frame.Empty())
      {
        using var stream = new MemoryStream();
        var bytes = frame.ImEncode(".jpg");
        await stream.WriteAsync(bytes);

        var initial = Encoding.UTF8.GetBytes("--frame\r\n");
        await this.Response.Body.WriteAsync(initial);

        var contentType = Encoding.UTF8.GetBytes("Content-Type: image/jpeg\r\n\r\n");
        await this.Response.Body.WriteAsync(contentType);

        var frameBytes = stream.ToArray();
        await this.Response.Body.WriteAsync(frameBytes);

        var final = Encoding.UTF8.GetBytes("\r\n");
        await this.Response.Body.WriteAsync(final);
      }
    }

    this.isInitialized = false;
  }
}
