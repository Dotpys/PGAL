namespace PGAL.Noise;

public sealed class PerlinNoise3D : PerlinNoiseBase
{
	private float[,,,] GradientMatrix { get; init; }

	public PerlinNoise3D(int seed, uint cellCount = 16, bool wrap = true)
	{
		CellCount = cellCount;
		GradientMatrix = new float[GridSize, GridSize, GridSize, 3];
		Random r = new(seed);
		for (int z = 0; z < GridSize; z++)
		{
			for (int y = 0; y < GridSize; y++)
			{
				for (int x = 0; x < GridSize; x++)
				{
					float azimuth = r.NextSingle() * 2 * MathF.PI;
					float polar = r.NextSingle() * MathF.PI;
					var (azimuthSin, azimuthCos) = MathF.SinCos(azimuth);
					var (polarSin, polarCos) = MathF.SinCos(polar);
					// Polar to Cartesian coordinate conversion.
					(
						GradientMatrix[x, y, z, 0],
						GradientMatrix[x, y, z, 1],
						GradientMatrix[x, y, z, 2]
					) = (
						azimuthCos * polarSin,
						azimuthSin * polarSin,
						polarCos
					);
				}
			}
		}
		// TODO: Wrapping
	}

	public float At(float ix, float iy, float iz)
	{
		float x = ix % CellCount;
		float y = iy % CellCount;
		float z = iz % CellCount;

		int x0 = (int)Math.Floor(x);
		int y0 = (int)Math.Floor(y);
		int z0 = (int)Math.Floor(z);
		int x1 = x0 + 1;
		int y1 = y0 + 1;
		int z1 = z0 + 1;

		float v000 = EvaluateFromVertex(x, y, z, x0, y0, z0);
		float v001 = EvaluateFromVertex(x, y, z, x0, y0, z1);
		float v010 = EvaluateFromVertex(x, y, z, x0, y1, z0);
		float v011 = EvaluateFromVertex(x, y, z, x0, y1, z1);
		float v100 = EvaluateFromVertex(x, y, z, x1, y0, z0);
		float v101 = EvaluateFromVertex(x, y, z, x1, y0, z1);
		float v110 = EvaluateFromVertex(x, y, z, x1, y1, z0);
		float v111 = EvaluateFromVertex(x, y, z, x1, y1, z1);

		float wx = x - x0;
		float wy = y - y0;
		float wz = z - z0;

		// Interpolate in x direction
		float e0 = Interpolate(wx, v000, v100);
		float e1 = Interpolate(wx, v001, v101);
		float e2 = Interpolate(wx, v010, v110);
		float e3 = Interpolate(wx, v011, v111);
		// Interpolate in y direction
		float r0 = Interpolate(wy, e0, e2);
		float r1 = Interpolate(wy, e1, e3);
		// Interpolate in z direction  // TODO: Not sure 'bout this resize function.
		return Interpolate(wz, r0, r1) / MathF.Sqrt(2) + 0.5f;
	}

	private float EvaluateFromVertex(float x, float y, float z, int vx, int vy, int vz) =>
		(x - vx) * GradientMatrix[vx, vy, vz, 0] +
		(y - vy) * GradientMatrix[vx, vy, vz, 1] +
		(z - vz) * GradientMatrix[vx, vy, vz, 2];
}
