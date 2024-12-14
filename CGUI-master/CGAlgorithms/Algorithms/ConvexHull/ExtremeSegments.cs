using CGUtilities;
using System.Collections.Generic;

namespace CGAlgorithms.Algorithms.ConvexHull
{

	public class ExtremeSegments : Algorithm
	{
		public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
		{
			if (points.Count <= 2)
			{
				outPoints = points;
				return;
			}

			int firstIndex = 0;
			do
			{
				int secondIndex = 0;
				do
				{
					int leftCount = 0, rightCount = 0;
					int testIndex = 0;
					do
					{
						Line testLine = new Line(points[firstIndex], points[secondIndex]);
						if (testIndex != firstIndex && testIndex != secondIndex)
						{
							Enums.TurnType turnTest = HelperMethods.CheckTurn(testLine, points[testIndex]);
							if (turnTest == Enums.TurnType.Left)
								leftCount++;
							else if (turnTest == Enums.TurnType.Right)
								rightCount++;
						}
						testIndex++;
					} while (testIndex < points.Count);

					if ((leftCount == 0 && rightCount > 0) || (rightCount == 0 && leftCount > 0) && firstIndex != secondIndex)
					{
						if (!outPoints.Contains(points[firstIndex]))
							outPoints.Add(points[firstIndex]);
						if (!outPoints.Contains(points[secondIndex]))
							outPoints.Add(points[secondIndex]);
					}
					secondIndex++;
				} while (secondIndex < points.Count);

				firstIndex++;
			} while (firstIndex < points.Count);

			outPoints = delSegPoint(outPoints);
		}

		private List<Point> delSegPoint(List<Point> points)
		{

			int i = 0;
			while (i < points.Count)
			{
				bool isExtreme = true;
				int j = 0;
				while (j < points.Count && isExtreme)
				{
					int k = 0;
					while (k < points.Count && isExtreme)
					{
						if (points[i] != points[j] && points[i] != points[k])
						{
							bool isOnSegment = HelperMethods.PointOnSegment(points[i], points[j], points[k]);
							if (isOnSegment)
							{
								points.Remove(points[i]);
								isExtreme = false;
							}
						}
						k++;
					}
					j++;
				}
				if (!isExtreme)
					i--;
				i++;
			}
			return points;
		}
		public override string ToString()
		{
			return "Convex Hull - Extreme Segments";
		}
	}
}
