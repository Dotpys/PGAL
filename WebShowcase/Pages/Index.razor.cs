using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace WebShowcase.Pages;

[SupportedOSPlatform("browser")]
public partial class Index
{
	[JSImport("loadImage", "Index")]
	internal static partial void LoadImage(byte[] data);
}
