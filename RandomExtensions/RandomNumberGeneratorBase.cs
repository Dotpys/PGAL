using System.Runtime.CompilerServices;

namespace RandomExtensions;

public abstract class RandomNumberGeneratorBase
{
	public abstract ulong Next();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float NextSingle() => (Next() >> 40) * (1.0f / (1u << 24));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public double NextDouble() => (Next() >> 11) * (1.0 / (1ul << 53));
}
