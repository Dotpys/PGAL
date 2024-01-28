using System.Numerics;
using System.Runtime.Versioning;

namespace WebShowcase.Pages;

[SupportedOSPlatform("browser")]
public partial class VoronoiDiagram
{
}

public static class ExtensionMethods
{
	public static IEnumerable<IEnumerable<T>> DifferentCombinations<T>(this IEnumerable<T> elements, int k)
	{
		return k == 0 ? new[] { Array.Empty<T>() } : elements.SelectMany((e, i) => elements.Skip(i + 1).DifferentCombinations(k - 1).Select(c => (new[] { e }).Concat(c)));
	}
}
