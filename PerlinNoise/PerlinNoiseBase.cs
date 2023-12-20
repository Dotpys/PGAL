namespace PGAL.Noise;

public abstract class PerlinNoiseBase
{
	protected uint CellCount { get; init; }
	protected uint GridSize => CellCount + 1;

	protected static float Interpolate(float w, float v0, float v1)
	{
		// Fast Polynomial interpolation method.
		w = 3 * w * w - 2 * w * w * w;
		return (1.0f - w) * v0 + w * v1;
	}
}
