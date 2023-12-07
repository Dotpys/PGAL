using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace WebShowcase.Pages;

[SupportedOSPlatform("browser")]
public partial class Home
{
	[JSImport("loadImage", "Home")]
	internal static partial void LoadImage(byte[] data);
}
