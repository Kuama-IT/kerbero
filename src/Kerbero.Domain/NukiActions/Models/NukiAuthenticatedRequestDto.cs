namespace Kerbero.Domain.NukiActions.Models;

public class NukiAuthenticatedRequestDto<TPayload>
{
	public string KerberoAccountId { get; set; }

	public TPayload RequestPayload { get; set; }
}

public class NukiAuthenticatedRequestDto
{
	public int KerberoAccountId { get; set; }
}
