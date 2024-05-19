// <copyright file="HomeController.cs" company="Daniel-Leal">
// Copyright (c) Daniel-Leal. All rights reserved.
// </copyright>

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
  /// Goes to privacy view.
  /// </summary>
  /// <returns>View instance.</returns>
  public IActionResult Privacy()
    => this.View();

  /// <summary>
  /// Goes to error view.
  /// </summary>
  /// <returns>View instance.</returns>
  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error()
    => this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
}
