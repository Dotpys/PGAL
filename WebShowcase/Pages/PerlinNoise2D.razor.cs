using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace WebShowcase.Pages;

[SupportedOSPlatform("browser")]
public partial class PerlinNoise2D
{
	[JSImport("loadImage", "PerlinNoise2D")]
	internal static partial void LoadImage(string canvasId, byte[] data, int textureSize);
}
