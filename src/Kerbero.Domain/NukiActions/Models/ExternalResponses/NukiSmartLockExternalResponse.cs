using System.Text.Json.Serialization;

namespace Kerbero.Domain.NukiActions.Models.ExternalResponses;

public class NukiSmartLockExternalResponse
{
	[JsonPropertyName("smartlockId")]
	public int SmartLockId { get; set; }
	
	[JsonPropertyName("accountId")]
	public int AccountId { get; set; }
	
	[JsonPropertyName("type")]
	public int Type { get; set; }
	
	[JsonPropertyName("lmType")]
	public int LmType { get; set; }
	
	[JsonPropertyName("authId")]
	public int AuthId { get; set; }
	
	[JsonPropertyName("name")]
	public string? Name { get; set; }
	
	[JsonPropertyName("favorite")]
	public bool Favourite { get; set; }
	
	[JsonPropertyName("state")]
	public NukiSmartLockStateExternalResponse? State { get; set; }
}
