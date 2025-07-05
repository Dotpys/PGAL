using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace RandomExtensions;

/*
Written in 2025 by Julien Russo based on the original code by
David Blackman and Sebastiano Vigna (vigna@acm.org)
*/

/*
This is xoshiro256+ 1.0, our best and fastest generator for floating-point
numbers. We suggest to use its upper bits for floating-point
generation, as it is slightly faster than xoshiro256++/xoshiro256**. It
passes all tests we are aware of except for the lowest three bits,
which might fail linearity tests (and just those), so if low linear
complexity is not considered an issue (as it is usually the case) it
can be used to generate 64-bit outputs, too.

We suggest to use a sign test to extract a random Boolean value, and
right shifts to extract subsets of bits.

The state must be seeded so that it is not everywhere zero. If you have
a 64-bit seed, we suggest to seed a splitmix64 generator and use its
output to fill s.
*/

public sealed class RandomXoshiro256Plus : RandomNumberGeneratorBase
{
	private static readonly ulong[] JUMP = [0x180ec6d33cfd0abaul, 0xd5a61266f0c9392cul, 0xa9582618e03fc9aaul, 0x39abdc4529b1661cul];
	private static readonly ulong[] LONG_JUMP = [0x76e15d3efefdcbbful, 0xc5004e441c522fb3ul, 0x77710069854ee241ul, 0x39109bb02acbe635ul];

	private ulong _s0, _s1, _s2, _s3;

	public RandomXoshiro256Plus()
	{
		Span<byte> buffer = stackalloc byte[32];
		do
		{
			// Kinda ironic.
			RandomNumberGenerator.Fill(buffer);
			_s0 = BitConverter.ToUInt64(buffer[ 0.. 8]);
			_s1 = BitConverter.ToUInt64(buffer[ 8..16]);
			_s2 = BitConverter.ToUInt64(buffer[16..24]);
			_s3 = BitConverter.ToUInt64(buffer[24..32]);
		}
		while ((_s0 | _s1 | _s2 | _s3) == 0);
	}

	public RandomXoshiro256Plus(ulong s0, ulong s1, ulong s2, ulong s3)
	{
		_s0 = s0;
		_s1 = s1;
		_s2 = s2;
		_s3 = s3;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override ulong Next()
	{
		ulong result = _s0 + _s3;

		ulong t = _s1 << 17;

		_s2 ^= _s0;
		_s3 ^= _s1;
		_s1 ^= _s2;
		_s0 ^= _s3;

		_s2 ^= t;

		_s3 = BitOperations.RotateLeft(_s3, 45);

		return result;
	}


	/// <summary>
	/// This is equivalent to 2^128 calls to <see cref="Next"/>;
	/// it can be used to generate 2^128 non-overlapping subsequences for parallel computations.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Jump()
	{
		ulong s0 = 0, s1 = 0, s2 = 0, s3 = 0;

		for (int i = 0; i < JUMP.Length; i++)
		{
			for (int b = 0; b < 64; b++)
			{
				if ((JUMP[i] & (1ul << b)) != 0)
				{
					s0 ^= _s0;
					s1 ^= _s1;
					s2 ^= _s2;
					s3 ^= _s3;
				}
				Next();
			}
		}
		_s0 = s0;
		_s1 = s1;
		_s2 = s2;
		_s3 = s3;
	}

	/// <summary>
	/// This is equivalent to 2^192 calls to <see cref="Next"/>; it can be used to generate 2^64
	/// starting points, from each of which <see cref="Next"/> will generate 2^64 non-overlapping
	/// subsequences for parallel distributed computations.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void LongJump()
	{
		ulong s0 = 0, s1 = 0, s2 = 0, s3 = 0;

		for (int i = 0; i < LONG_JUMP.Length; i++)
		{
			for (int b = 0; b < 64; b++)
			{
				if ((LONG_JUMP[i] & (1ul << b)) != 0)
				{
					s0 ^= _s0;
					s1 ^= _s1;
					s2 ^= _s2;
					s3 ^= _s3;
				}
				Next();
			}
		}
		_s0 = s0;
		_s1 = s1;
		_s2 = s2;
		_s3 = s3;
	}
}
