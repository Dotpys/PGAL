namespace PGAL.Noise;

public sealed class PerlinNoise2D : PerlinNoiseBase
{
	private float[,,] GradientMatrix { get; init; }

	public PerlinNoise2D(int seed, uint cellCount = 16, bool wrap = true)
	{
		CellCount = cellCount;
		GradientMatrix = new float[GridSize, GridSize, 2];
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
			for (int y = 0; y < GridSize; y++)
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
		float e0 = Interpolate(wx, v00, v10);
		float e1 = Interpolate(wx, v01, v11);
		return Interpolate(wy, e0, e1) / MathF.Sqrt(2) + 0.5f;
	}

	private float EvaluateFromVertex(float x, float y, int vx, int vy) =>
		(x - vx) * GradientMatrix[vx, vy, 0] +
		(y - vy) * GradientMatrix[vx, vy, 1];
}
