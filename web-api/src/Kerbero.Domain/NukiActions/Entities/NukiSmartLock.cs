using Kerbero.Domain.NukiAuthentication.Entities;

namespace Kerbero.Domain.NukiActions.Entities;

public class NukiSmartLock
{
	public int Id { get; set; }
	
    public int ExternalSmartLockId { get; set; }
	
    public int Type { get; set; }
	
    public int AuthId { get; set; }
	
    public string? Name { get; set; }
	
    public bool Favourite { get; set; }

    public int NukiAccountId { get; set; }
    
    public NukiAccount Account { get; set; } = null!;
    
    public NukiSmartLockState? State { get; set; }
}

