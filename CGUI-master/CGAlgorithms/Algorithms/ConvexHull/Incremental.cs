using CGUtilities;
using System;
using System.Collections.Generic;

namespace CGAlgorithms.Algorithms.ConvexHull
{
	public class Incremental : Algorithm
	{

		private double DistanceBetweenTwoPoints(Point p1, Point p2)
		{
			return Math.Sqrt(Math.Pow(p1.Y - p2.Y, 2) + Math.Pow(p1.X - p2.X, 2));
		}
		private Point FindLowestLeftmostPoint(List<Point> points)
		{
			Point lowestLeftmostPoint = points[0];

			for (int i = 1; i < points.Count; i++)
			{
				if (points[i].Y < lowestLeftmostPoint.Y || (points[i].Y == lowestLeftmostPoint.Y && points[i].X < lowestLeftmostPoint.X))
				{
					lowestLeftmostPoint = points[i];
				}
			}

			return lowestLeftmostPoint;
		}
		public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
		{
			int numOfPoints = points.Count;
			if (numOfPoints < 3)
			{
				outPoints = points;
				return;
			}

			Point startPoint = FindLowestLeftmostPoint(points);
			Point currentPoint = startPoint;

			List<Point> convexHull = new List<Point>();

			while (true)
			{
				convexHull.Add(currentPoint);
				Point nextPoint = points[0];
				double maxDistance = 0;

				int i = 1;
				while (i < numOfPoints)
				{
					Line segment = new Line(currentPoint, nextPoint);
					Enums.TurnType turn = HelperMethods.CheckTurn(segment, points[i]);

					if (nextPoint == currentPoint || turn == Enums.TurnType.Left)
					{
						nextPoint = points[i];
					}
					else if (turn == Enums.TurnType.Colinear)
					{
						if (!HelperMethods.PointOnSegment(points[i], segment.Start, segment.End))
						{
							double distance = DistanceBetweenTwoPoints(currentPoint, points[i]);
							double currentMaxDistance = DistanceBetweenTwoPoints(currentPoint, nextPoint);

							if (distance > currentMaxDistance)
							{
								maxDistance = distance;
								nextPoint = points[i];
							}
						}
					}
					i++;
				}

				currentPoint = nextPoint;

				if (currentPoint == startPoint)
				{
					break;
				}
			}

			outPoints = convexHull;
		}

		public override string ToString()
		{
			return "Convex Hull - Incremental";
		}
	}

}