﻿namespace Kerbero.Domain.NukiCredentials.Models;

public class NukiCredentialModel
{
  public int Id { get; set; }
  public required string? Token { get; set; }
  public Guid UserId { get; set; }
  
  public required string NukiEmail { get; set; }
  
  public bool IsRefreshable { get; set; }
}
