using FluentAssertions;
using Kerbero.Identity.Common;
using Xunit;

namespace Kerbero.Identity.Tests.Common;

public class SlugifyOutboundParameterTransformerTest
{
  [Theory]
  [InlineData("RandomText","random-text")]
  [InlineData("RandomText/SubRes","random-text/sub-res")]
  [InlineData(null,"")]
  public void TransformOutbound_ValidInput_ReturnCorrectResult(object input,string expected)
  {
    var actual = new SlugifyOutboundParameterTransformer()
      .TransformOutbound(input);

    actual.Should().BeEquivalentTo(expected);
  }
}