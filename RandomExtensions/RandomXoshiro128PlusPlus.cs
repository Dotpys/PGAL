using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace RandomExtensions;

/*
Written in 2025 by Julien Russo based on the original code by
David Blackman and Sebastiano Vigna (vigna@acm.org)
*/

/*
This is xoroshiro128++ 1.0, one of our all-purpose, rock-solid,
small-state generators. It is extremely (sub-ns) fast and it passes all
tests we are aware of, but its state space is large enough only for
mild parallelism.

For generating just floating-point numbers, xoroshiro128+ is even
faster (but it has a very mild bias, see notes in the comments).

The state must be seeded so that it is not everywhere zero. If you have
a 64-bit seed, we suggest to seed a splitmix64 generator and use its
output to fill s.
*/

public sealed class RandomXoshiro128PlusPlus : RandomNumberGeneratorBase
{
	private static readonly ulong[] JUMP = [0x2bd7a6a6e99c2ddcul, 0x0992ccaf6a6fca05ul];
	private static readonly ulong[] LONG_JUMP = [0x360fd5f2cf8d5d99ul, 0x9c6e6877736c46e3ul];

	private ulong _s0, _s1;

	public RandomXoshiro128PlusPlus()
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

	public RandomXoshiro128PlusPlus(ulong s0, ulong s1)
	{
		_s0 = s0;
		_s1 = s1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override ulong Next()
	{
		ulong s0 = _s0, s1 = _s1;

		ulong result = BitOperations.RotateLeft(s0 + s1, 17) + s1;

		s1 ^= s0;
		_s0 = BitOperations.RotateLeft(s0, 49) ^ s1 ^ (s1 << 21);
		_s1 = BitOperations.RotateLeft(s1, 28);

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
