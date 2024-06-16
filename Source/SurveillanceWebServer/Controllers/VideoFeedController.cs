// <copyright file="VideoFeedController.cs" company="Daniel-Leal">
// Copyright (c) Daniel-Leal. All rights reserved.
// </copyright>

using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Net;
using System.Net.WebSockets;
using System.Text.Json;

namespace SurveillanceWebServer.Controllers;

/// <summary>
/// Video Feed Controller class.
/// </summary>
public class VideoFeedController(ILogger<VideoFeedController> logger) : Controller
{
  private readonly ILogger logger = logger;
  private VideoCapture? capture;
  private bool isInitialized;
  private readonly object initializationLock = new();

  /// <summary>
  /// Initializes, begins, and finalizes the video feed.
  /// </summary>
  /// <param name="context">The HTTP context.</param>
  /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
  public async Task BeginVideo(HttpContext context)
  {
    var initialized = await this.InitializeVideo(context);
    if (initialized)
    {
      await this.StreamVideo(context);
    }
  }

  private async Task<bool> InitializeVideo(HttpContext context)
  {
    var (ip, location) = await AcquireUserInfo(context);
    lock (this.initializationLock)
    {
      if (this.isInitialized)
        return true;

      // Initialize camera capture for client.
      this.capture = new VideoCapture(0, VideoCapture.API.DShow);
      this.isInitialized = true;

      // Log the time, IP address, and location of client's entry.
      var time = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);
      this.logger.LogInformation("{Time} | Client {Ip} has joined from {Location}.", time, ip, location);

      return this.capture is not null;
    }
  }

  [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Only running on Windows PC for now.")]
  private async Task StreamVideo(HttpContext context)
  {
    // Do not stream if initialization was unsuccessful.
    if (!this.isInitialized)
    {
      return;
    }

    // Get frame and websocket.
    var frame = new Mat();
    var websocket = await context.WebSockets.AcceptWebSocketAsync();

    // Create a single reusable stream to avoid memory leaks.
    using var stream = new MemoryStream();

    try
    {
      // Continuously stream when requested.
      while (websocket.State is WebSocketState.Open)
      {
        // Read the camera frames, continue if empty.
        this.capture?.Read(frame);
        if (frame.IsEmpty)
          break;

        // Send frames to websocket.
        stream.SetLength(0);
        frame.ToBitmap().Save(stream, ImageFormat.Jpeg);
        var bytes = stream.ToArray();

        if (bytes.Length > 0)
          await websocket.SendAsync(bytes, WebSocketMessageType.Binary, true, CancellationToken.None);
      }
    }
    catch (Exception ex)
    {
      this.logger.LogError("{Type} with message {Message} was thrown when attempting to stream video.", ex.GetType(), ex.Message);
    }
    finally
    {
      await this.FinalizeVideo(context);
      websocket.Dispose();
    }
  }

  private async Task FinalizeVideo(HttpContext context)
  {
    this.capture?.Stop();
    this.capture?.Release();
    this.capture?.Dispose();
    this.isInitialized = false;

    var (ip, location) = await AcquireUserInfo(context);
    var time = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);
    this.logger.LogInformation("{Time} | Client with IP {Ip} has left your stream from {Location}.", time, ip, location);
  }

  private static async Task<(string Ip, string Location)> AcquireUserInfo(HttpContext context)
  {
    using var client = new HttpClient();

    // Get IP address of client.
    var ip = context.Connection.RemoteIpAddress?.ToString().Split("ffff:")[1] ?? "[IP not found]";

    // Get city and region of client.
    var locationStatus = await client.GetAsync($"https://ipinfo.io/{ip}/json");
    var locationJson = locationStatus.IsSuccessStatusCode ? await locationStatus.Content.ReadAsStringAsync() : null;
    if (locationJson is not null)
    {
      var data = JsonDocument.Parse(locationJson);

      var city = data.RootElement.GetProperty("city").GetString()
        ?? throw new InvalidOperationException("Could not get property 'city' from JSON.");

      var region = data.RootElement.GetProperty("region").GetString()
        ?? throw new InvalidOperationException("Could not get property 'region' from JSON");

      return (ip, $"{city}, {region}");
    }

    return (ip, "[Location not found]");
  }
}