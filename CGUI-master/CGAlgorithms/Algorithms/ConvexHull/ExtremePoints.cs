using CGUtilities;
using System.Collections.Generic;

namespace CGAlgorithms.Algorithms.ConvexHull
{

	public class ExtremePoints : Algorithm
	{
		public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
		{
			int currentPointIndex = 0;

			do
			{
				bool isPointRemoved = false;
				int point1Index = 0;

				do
				{
					int point2Index = 0;

					do
					{
						int point3Index = 0;

						do
						{
							if (point1Index != currentPointIndex && point2Index != currentPointIndex && point3Index != currentPointIndex)
							{
								Enums.PointInPolygon insideTriangleTest = HelperMethods.PointInTriangle(points[currentPointIndex], points[point1Index], points[point2Index], points[point3Index]);

								if (insideTriangleTest.Equals(Enums.PointInPolygon.Inside) || insideTriangleTest.Equals(Enums.PointInPolygon.OnEdge))
								{
									points.RemoveAt(currentPointIndex);
									isPointRemoved = true;
								}
							}
							point3Index++;
						} while (point3Index < points.Count && !isPointRemoved);

						point2Index++;
					} while (point2Index < points.Count && !isPointRemoved);

					point1Index++;
				} while (point1Index < points.Count && !isPointRemoved);

				if (isPointRemoved)
					currentPointIndex--;

				currentPointIndex++;
			} while (currentPointIndex < points.Count);

			outPoints = points;

		}

		public override string ToString()
		{
			return "Convex Hull - Extreme Points";
		}
	}
}
