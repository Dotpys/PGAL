using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

using RandomExtensions;

using System;
using System.Collections.Generic;
using System.Text;

namespace Benchmark;

[SimpleJob(RuntimeMoniker.Net10_0, baseline: true)]
//[SimpleJob(RuntimeMoniker.Wasm)]
[RPlotExporter]
public class PRNG
{
	private System.Random random_sys = new();
	private RandomXoshiro128Plus random_xoshiro128plus = new();
	private RandomXoshiro256StarStar random_xoshiro256starstar = new();

	[Params(1_000_000, 1_000_000_000)]
	public int n;

	[GlobalSetup]
	public void Setup()
	{
		
	}

	[Benchmark]
	public void RandomSys()
	{
		float sum = 0;
		for (int i = 0; i < n; i++)
		{
			sum += random_sys.NextSingle();
		}
		Console.WriteLine(sum);
	}

	[Benchmark]
	public void RandomCustomXoshiro128Plus()
	{
		float sum = 0;
		for (int i = 0; i < n; i++)
		{
			sum += random_xoshiro128plus.NextSingle();
		}
		Console.WriteLine(sum);
	}

	[Benchmark]
	public void RandomCustomXoshiro256StarStar()
	{
		float sum = 0;
		for (int i = 0; i < n; i++)
		{
			sum += random_xoshiro256starstar.NextSingle();
		}
		Console.WriteLine(sum);
	}
}
