namespace PGAL.Noise;

public class PerlinNoiseF
{
	private uint CellCount { get; init; }
	private uint GridSize { get => CellCount + 1; }
	private float[,,] GradientMatrix { get; init; }
	private Func<float, float> InterpolationFunction { get; init; }

	public PerlinNoiseF(int seed, InterpolationFunction function = Noise.InterpolationFunction.Polynomial, uint cellCount = 16, bool wrap = true)
	{
		CellCount = cellCount;
		GradientMatrix = new float[GridSize, GridSize, 2];
		InterpolationFunction = function switch
		{
			Noise.InterpolationFunction.Linear => Interpolation.Linear,
			Noise.InterpolationFunction.Cosine => Interpolation.Cosine,
			Noise.InterpolationFunction.Polynomial => Interpolation.FastPolynomial,
			_ => Interpolation.FastPolynomial
		};
		Random r = new(seed);
		for (int y = 0; y < GridSize; y++)
		{
			for (int x = 0; x < GridSize; x++)
			{
				float angle = r.NextSingle() * 2 * MathF.PI;
				(GradientMatrix[x, y, 0], GradientMatrix[x, y, 1]) = MathF.SinCos(angle);
			}
		}
		if (wrap)
		{
			// Right Side = Left Side
			for (int y=0; y<GridSize; y++)
			{
				GradientMatrix[cellCount, y, 0] = GradientMatrix[0, y, 0];
				GradientMatrix[cellCount, y, 1] = GradientMatrix[0, y, 1];
			}
			// Bottom Side = Top Side
			for (int x = 0; x < GridSize; x++)
			{
				GradientMatrix[x, cellCount, 0] = GradientMatrix[x, 0, 0];
				GradientMatrix[x, cellCount, 1] = GradientMatrix[x, 0, 1];
			}
		}
	}

	public float At(float ix, float iy)
	{
		float x = ix % CellCount;
		float y = iy % CellCount;

		int x0 = (int)Math.Floor(x);
		int y0 = (int)Math.Floor(y);
		int x1 = x0 + 1;
		int y1 = y0 + 1;

		float v00 = EvaluateFromVertex(x, y, x0, y0);
		float v10 = EvaluateFromVertex(x, y, x1, y0);
		float v11 = EvaluateFromVertex(x, y, x1, y1);
		float v01 = EvaluateFromVertex(x, y, x0, y1);

		float wx = x - x0;
		float wy = y - y0;
		float e0 = Interpolate(wx, v00, v10, InterpolationFunction);
		float e1 = Interpolate(wx, v01, v11, InterpolationFunction);
		return Interpolate(wy, e0, e1, InterpolationFunction) / MathF.Sqrt(2) + 0.5f;
	}

	// Prodotto scalare tra il gradiente del veretice (vx, vy) e il punto (x,y)
	private float EvaluateFromVertex(float x, float y, int vx, int vy) => (x - vx) * GradientMatrix[vx, vy, 0] + (y - vy) * GradientMatrix[vx, vy, 1];

	private float Interpolate(float w, float v0, float v1, Func<float, float> interpolationFunction)
	{
		w = interpolationFunction(w);
		return (1.0f - w) * v0 + w * v1;
	}
}

public static class Interpolation
{
	public static float Linear(float x) => x;
	public static float Cosine(float x) => (1 - MathF.Cos(x * MathF.PI)) / 2;
	public static float SlowPolynomial(float x) => (6 * x * x * x * x * x - 15 * x * x * x * x + 10 * x * x * x);
	public static float FastPolynomial(float x) => (3 * x * x - 2 * x * x * x);
}

public enum InterpolationFunction
{
	Linear,
	Cosine,
	Polynomial
}
