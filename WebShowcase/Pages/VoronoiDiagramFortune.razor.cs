using System.Numerics;
using System.Runtime.Versioning;

namespace WebShowcase.Pages;

[SupportedOSPlatform("browser")]
public partial class VoronoiDiagram
{
}

public class Voronoi
{
	public void GenerateDiagram(IEnumerable<(float x, float y)> sites)
	{
		PriorityQueue<VoronoiEvent, float> eventQueue = new();

		foreach (var site in sites)
		{
			eventQueue.Enqueue(new VoronoiSiteEvent(), site.y);
		}

		while(eventQueue.TryDequeue(out VoronoiEvent? e, out float y))
		{
			if (e is VoronoiSiteEvent)
			{
				//InsertParabola(point)
				/*
					if root = null
						root = new Parabola(p)
						return

					if root.isLeaf AND root.site.y - p.y < 1
						// Handle edge case

					parabola = GetParabolaByX(point.x)
				 */
			}
			else
			{
				//RemoveParabola(event);
			}
		}
	}
}

public abstract class VoronoiEvent
{

}

public sealed class VoronoiSiteEvent : VoronoiEvent
{

}

public sealed class VoronoiVertexEvent : VoronoiEvent
{

}
