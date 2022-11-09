using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

namespace Kerbero.Identity.Common;

public class SlugifyOutboundParameterTransformer : IOutboundParameterTransformer
{
  public string TransformOutbound(object? value)
  {
    // Slugify value
    return Regex.Replace(value?.ToString() ?? "", "([a-z])([A-Z])", "$1-$2").ToLower();
  }
}