using FluentAssertions;
using Kerbero.Domain.SmartLockKeys.Entities;
using Kerbero.Domain.SmartLockKeys.Managers;
using Kerbero.Domain.SmartLockKeys.Models.PresentationRequests;

namespace Kerbero.Domain.Tests.SmartLockKeys.Managers;

public class SmartLockKeyGeneratorManagerTest
{
	[Fact]
	public void GenerateSmartLockKey_Success()
	{
		var expiryDate = DateTime.Now + TimeSpan.FromDays(7);
		var expectedSmartLockKey = new SmartLockKey
		{
			Id = Guid.Empty,
			ExpiryDate = expiryDate,
			IsDisabled = false,
			UsageCounter = 0,
			NukiSmartLockId = 1
		};

		var smartLockKeyGeneratorManager = new SmartLockKeyGeneratorManager();

		var smartLockKey = smartLockKeyGeneratorManager.GenerateSmartLockKey(1, expiryDate);
		var guidToken = new Guid(Convert.FromBase64String(smartLockKey.Value.Token));
		
		smartLockKey.IsSuccess.Should().BeTrue();
		smartLockKey.Value.CreationDate.Should().BeBefore(DateTime.Now);
		smartLockKey.Value.IsDisabled.Should().BeFalse();
		smartLockKey.Value.UsageCounter.Should().Be(expectedSmartLockKey.UsageCounter);
		smartLockKey.Value.NukiSmartLockId.Should().Be(1);
		guidToken.Should().NotBeEmpty();
	}
}
