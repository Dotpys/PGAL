using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace RandomExtensions;

/*
Written in 2025 by Julien Russo based on the original code by
David Blackman and Sebastiano Vigna (vigna@acm.org)
*/

/*
This is xoroshiro128+ 1.0, our best and fastest small-state generator
for floating-point numbers, but its state space is large enough only
for mild parallelism. We suggest to use its upper bits for
floating-point generation, as it is slightly faster than
xoroshiro128++/xoroshiro128**. It passes all tests we are aware of
except for the four lower bits, which might fail linearity tests (and
just those), so if low linear complexity is not considered an issue (as
it is usually the case) it can be used to generate 64-bit outputs, too;
moreover, this generator has a very mild Hamming-weight dependency
making our test (http://prng.di.unimi.it/hwd.php) fail after 5 TB of
output; we believe this slight bias cannot affect any application. If
you are concerned, use xoroshiro128++, xoroshiro128** or xoshiro256+.

We suggest to use a sign test to extract a random Boolean value, and
right shifts to extract subsets of bits.

The state must be seeded so that it is not everywhere zero. If you have
a 64-bit seed, we suggest to seed a splitmix64 generator and use its
output to fill s. 

NOTE: the parameters (a=24, b=16, b=37) of this version give slightly
better results in our test than the 2016 version (a=55, b=14, c=36).
*/

public sealed class RandomXoshiro128Plus : RandomNumberGeneratorBase
{
	private static readonly ulong[] JUMP = [0xdf900294d8f554a5ul, 0x170865df4b3201fcul];
	private static readonly ulong[] LONG_JUMP = [0xd2a98b26625eee7bul, 0xdddf9b1090aa7ac1ul];

	private ulong _s0, _s1;

	public RandomXoshiro128Plus()
	{
		Span<byte> buffer = stackalloc byte[16];
		do
		{
			// Kinda ironic.
			RandomNumberGenerator.Fill(buffer);
			_s0 = BitConverter.ToUInt64(buffer[..8]);
			_s1 = BitConverter.ToUInt64(buffer[8..]);
		}
		while ((_s0 | _s1) == 0);
	}

	public RandomXoshiro128Plus(ulong s0, ulong s1)
	{
		_s0 = s0;
		_s1 = s1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override ulong Next()
	{
		ulong s0 = _s0, s1 = _s1;

		ulong result = s0 + s1;

		s1 ^= s0;

		_s0 = BitOperations.RotateLeft(s0, 24) ^ s1 ^ (s1 << 16);
		_s1 = BitOperations.RotateLeft(s1, 37);

		return result;
	}


	/// <summary>
	/// This is equivalent to 2^64 calls to <see cref="Next"/>;
	/// it can be used to generate 2^64 non-overlapping subsequences for parallel computations.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Jump()
	{
		ulong s0 = 0, s1 = 0;

		for (int i = 0; i < JUMP.Length; i++)
		{
			for (int b = 0; b < 64; b++)
			{
				if ((JUMP[i] & (1ul << b)) != 0)
				{
					s0 ^= _s0;
					s1 ^= _s1;
				}
				Next();
			}
		}
		_s0 = s0;
		_s1 = s1;
	}

	/// <summary>
	/// This is equivalent to 2^96 calls to <see cref="Next"/>; it can be used to generate 2^32
	/// starting points, from each of which <see cref="Next"/> will generate 2^32 non-overlapping
	/// subsequences for parallel distributed computations.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void LongJump()
	{
		ulong s0 = 0, s1 = 0;

		for (int i = 0; i < LONG_JUMP.Length; i++)
		{
			for (int b = 0; b < 64; b++)
			{
				if ((LONG_JUMP[i] & (1ul << b)) != 0)
				{
					s0 ^= _s0;
					s1 ^= _s1;
				}
				Next();
			}
		}
		_s0 = s0;
		_s1 = s1;
	}
}
