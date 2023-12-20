using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace WebShowcase.Pages;

[SupportedOSPlatform("browser")]
public partial class PerlinNoise3D
{
	[JSImport("loadFrame", "PerlinNoise3D")]
	internal static partial string LoadFrame(byte[] data);
}
