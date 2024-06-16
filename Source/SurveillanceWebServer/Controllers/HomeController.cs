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
}
