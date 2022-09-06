using System.ComponentModel.DataAnnotations;
using Kerbero.NET.Domain.Entities.Account;
using Kerbero.NET.Domain.Entities.Device;

namespace Kerbero.NET.Domain.Entities.RentInformation;
/// <summary>
/// A Reservation is downloaded from the Rent provider API and associated with a smart-lock 
/// </summary>
public class Reservation
{
  public int Id { get; set; }
  
  public DateTime CheckInDate { get; set; }
  
  public DateTime CheckOutDate { get; set;}
  
  public string GuestsEmails { get; set;} = null!;
  
  public string StructureInfos { get; set; } = null!;
  
  public SmartLock SmartLock { get; set; } = null!;
  
  public RentProviderAccount RentProviderAccount { get; set; } = null!;
}