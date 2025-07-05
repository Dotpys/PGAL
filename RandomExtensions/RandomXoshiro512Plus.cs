using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace RandomExtensions;

/*
Written in 2025 by Julien Russo based on the original code by
David Blackman and Sebastiano Vigna (vigna@acm.org)
*/

/*
This is xoshiro512+ 1.0, our generator for floating-point numbers with
increased state size. We suggest to use its upper bits for
floating-point generation, as it is slightly faster than xoshiro512**.
It passes all tests we are aware of except for the lowest three bits,
which might fail linearity tests (and just those), so if low linear
complexity is not considered an issue (as it is usually the case) it
can be used to generate 64-bit outputs, too.

We suggest to use a sign test to extract a random Boolean value, and
right shifts to extract subsets of bits.

The state must be seeded so that it is not everywhere zero. If you have
a 64-bit seed, we suggest to seed a splitmix64 generator and use its
output to fill s.
*/

public sealed class RandomXoshiro512Plus : RandomNumberGeneratorBase
{
	private static readonly ulong[] JUMP = [0x33ed89b6e7a353f9ul, 0x760083d7955323beul, 0x2837f2fbb5f22faeul, 0x4b8c5674d309511cul, 0xb11ac47a7ba28c25ul, 0xf1be7667092bcc1cul, 0x53851efdb6df0aaful, 0x1ebbc8b23eaf25dbul];
	private static readonly ulong[] LONG_JUMP = [0x11467fef8f921d28ul, 0xa2a819f2e79c8ea8ul, 0xa8299fc284b3959aul, 0xb4d347340ca63ee1ul, 0x1cb0940bedbff6ceul, 0xd956c5c4fa1f8e17ul, 0x915e38fd4eda93bcul, 0x5b3ccdfa5d7daca5ul];

	private ulong _s0, _s1, _s2, _s3, _s4, _s5, _s6, _s7;

	public RandomXoshiro512Plus()
	{
		Span<byte> buffer = stackalloc byte[64];
		do
		{
			// Kinda ironic.
			RandomNumberGenerator.Fill(buffer);
			_s0 = BitConverter.ToUInt64(buffer[0..8]);
			_s1 = BitConverter.ToUInt64(buffer[8..16]);
			_s2 = BitConverter.ToUInt64(buffer[16..24]);
			_s3 = BitConverter.ToUInt64(buffer[24..32]);
			_s4 = BitConverter.ToUInt64(buffer[32..40]);
			_s5 = BitConverter.ToUInt64(buffer[40..48]);
			_s6 = BitConverter.ToUInt64(buffer[48..56]);
			_s7 = BitConverter.ToUInt64(buffer[56..64]);
		}
		while ((_s0 | _s1 | _s2 | _s3 | _s4 | _s5 | _s6 | _s7) == 0);
	}

	public RandomXoshiro512Plus(ulong s0, ulong s1, ulong s2, ulong s3, ulong s4, ulong s5, ulong s6, ulong s7)
	{
		_s0 = s0;
		_s1 = s1;
		_s2 = s2;
		_s3 = s3;
		_s4 = s4;
		_s5 = s5;
		_s6 = s6;
		_s7 = s7;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override ulong Next()
	{
		ulong result = _s1 + _s2;

		ulong t = _s1 << 11;

		_s2 ^= _s0;
		_s5 ^= _s1;
		_s1 ^= _s2;
		_s7 ^= _s3;
		_s3 ^= _s4;
		_s4 ^= _s5;
		_s0 ^= _s6;
		_s6 ^= _s7;

		_s6 ^= t;

		_s7 = BitOperations.RotateLeft(_s7, 21);

		return result;
	}

	/// <summary>
	/// This is equivalent to 2^256 calls to <see cref="Next"/>;
	/// it can be used to generate 2^256 non-overlapping subsequences for parallel computations.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Jump()
	{
		ulong s0 = 0, s1 = 0, s2 = 0, s3 = 0, s4 = 0, s5 = 0, s6 = 0, s7 = 0;
		

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
					s4 ^= _s4;
					s5 ^= _s5;
					s6 ^= _s6;
					s7 ^= _s7;
				}
				Next();
			}
		}
		_s0 = s0;
		_s1 = s1;
		_s2 = s2;
		_s3 = s3;
		_s4 = s4;
		_s5 = s5;
		_s6 = s6;
		_s7 = s7;
	}

	/// <summary>
	/// This is equivalent to 2^384 calls to <see cref="Next"/>; it can be used to generate 2^128
	/// starting points, from each of which <see cref="Next"/> will generate 2^128 non-overlapping
	/// subsequences for parallel distributed computations.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void LongJump()
	{
		ulong s0 = 0, s1 = 0, s2 = 0, s3 = 0, s4 = 0, s5 = 0, s6 = 0, s7 = 0;

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
					s4 ^= _s4;
					s5 ^= _s5;
					s6 ^= _s6;
					s7 ^= _s7;
				}
				Next();
			}
		}
		_s0 = s0;
		_s1 = s1;
		_s2 = s2;
		_s3 = s3;
		_s4 = s4;
		_s5 = s5;
		_s6 = s6;
		_s7 = s7;
	}
}
