using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace WebShowcase.Pages;

[SupportedOSPlatform("browser")]
public partial class SimplexNoise
{
	[JSImport("loadImage", "SimplexNoise")]
	internal static partial void LoadImage(string canvasId, byte[] data, int textureSize);
}
