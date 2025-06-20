using System.Numerics;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

using static WebShowcase.Pages.VoronoiDiagramDelaunay;

namespace WebShowcase.Pages;

[SupportedOSPlatform("browser")]
public partial class VoronoiDiagramDelaunay
{
	[JSImport("getSVGClickPosition", "VoronoiDiagramDelaunay")]
	internal static partial double[] GetSVGClickPosition(double clientX, double clientY);
}
public struct Circle
{
	public Vector2 Center { get; init; }
	public float Radius { get; init; }

	public bool ContainsPoint(Vector2 point) => Vector2.Distance(point, Center) < Radius;
}

public class Triangle(Vector2 a, Vector2 b, Vector2 c)
{
	public Vector2 a = a;
	public Vector2 b = b;
	public Vector2 c = c;

	public override string ToString() => $"({a}, {b}, {c})";

	public IEnumerable<(Vector2, Vector2)> Edges => [(a, b), (b, c), (a, c)];

	public bool ContainsPoint(Vector2 point)
	{
		static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
		{
			return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
		}

		float d1, d2, d3;
		bool hasNeg, hasPos;

		d1 = Sign(point, this.a, this.b);
		d2 = Sign(point, this.b, this.c);
		d3 = Sign(point, this.c, this.a);

		hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
		hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);
		return !(hasNeg && hasPos);
	}

	public bool HasEdge((Vector2 a, Vector2 b) edge) => Edges.Contains(edge) || Edges.Contains((edge.b, edge.a));

	public bool HasVertex(Vector2 v) => a == v || b == v || c == v;

	public Circle Circumcircle()
	{
		// Midpoints of the sides
		Vector2 midAB = (a + b) / 2;
		Vector2 midBC = (b + c) / 2;

		// Perpendicular bisectors direction
		Vector2 vAB = new(-(b.Y - a.Y), b.X - a.X);
		Vector2 vBC = new(-(c.Y - b.Y), c.X - b.X);

		// Solve for intersection of bisectors (circumcenter)
		float det = vAB.X * (-vBC.Y) - (-vBC.X) * vAB.Y;
		//if (Math.Abs(det) < 1e-6) throw new InvalidOperationException("Triangle points are collinear!");

		Vector2 bVector = midBC - midAB;
		float t = (bVector.X * (-vBC.Y) - bVector.Y * (-vBC.X)) / det;
		Vector2 circumcenter = midAB + t * vAB;

		// Compute circumradius
		float circumradius = Vector2.Distance(a, circumcenter);

		return new() { Center = circumcenter, Radius = circumradius };
	}
}

public static class ExtensionMethods
{
	public static List<Triangle> Triangulate(this IEnumerable<Vector2> pointList)
	{
		List<Triangle> triangulation = [];
		triangulation.Add(new(new(-57.74f, 0),new(157.74f, 0),new(50, 186.60f)));
		// Add all the points one at a time to the triangulation.
		foreach (Vector2 point in pointList)
		{
			List<Triangle> badTriangles = [];
			// First find all the triangles that are no longer valid due to the insertion
			foreach (Triangle triangle in triangulation)
			{
				if (triangle.Circumcircle().ContainsPoint(point))
				{
					badTriangles.Add(triangle);
				}
			}
			// List of edges.
			List<(Vector2, Vector2)> polygon = [];
			foreach (Triangle triangle in badTriangles)
			{
				foreach ((Vector2, Vector2) edge in triangle.Edges)
				{
					if (!badTriangles.Where(t => t != triangle).Any(triangle => triangle.HasEdge(edge)))
					{
						polygon.Add(edge);
					}
				}
			}
			foreach (Triangle triangle in badTriangles)
			{
				triangulation.Remove(triangle);
			}
			foreach ((Vector2, Vector2) edge in polygon)
			{
				var newTriangle = new Triangle(edge.Item1, edge.Item2, point);
				triangulation.Add(newTriangle);
			}
		}
		// Clear initial triangle residue
		List<Triangle> residue = [];
		foreach (Triangle triangle in triangulation)
		{
			if (triangle.HasVertex(new(-57.74f, 0)) || triangle.HasVertex(new(157.74f, 0)) || triangle.HasVertex(new(50, 186.60f)))
			{
				residue.Add(triangle);
				//triangulation.Remove(triangle);
			}
		}
		foreach (Triangle triangle in residue)
		{
			triangulation.Remove(triangle);
		}
		return triangulation;
	}
}
