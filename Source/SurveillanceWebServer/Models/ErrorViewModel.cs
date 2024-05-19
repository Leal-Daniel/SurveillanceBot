// <copyright file="ErrorViewModel.cs" company="Daniel-Leal">
// Copyright (c) Daniel-Leal. All rights reserved.
// </copyright>

namespace SurveillanceWebServer.Models;

/// <summary>
/// Error View Model class.
/// </summary>
public class ErrorViewModel
{
  /// <summary>
  /// Gets or sets the request ID.
  /// </summary>
  public string? RequestId { get; set; }

  /// <summary>
  /// Gets a value indicating whether <see cref="RequestId"/> is not null or empty.
  /// </summary>
  public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);
}
