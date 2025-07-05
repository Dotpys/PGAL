using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace RandomExtensions;

/*
Written in 2025 by Julien Russo.
*/

public sealed class Splitmix64 : RandomNumberGeneratorBase
{
	private ulong _s;

	public Splitmix64(ulong s)
	{
		_s = s;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override ulong Next()
	{
		_s += 0x9e3779b97f4a7c15ul;
		ulong t = _s;
		t = (t ^ (t >> 30)) * 0xbf58476d1ce4e5b9ul;
		t = (t ^ (t >> 27)) * 0x94d049bb133111ebul;
		return t ^ (t >> 31);
	}
}
