using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CGAlgorithms.Algorithms.ConvexHull
{
	public class GrahamScan : Algorithm //qeueue //bngeb lowest point //hnrtb arkamhom hsb el angels //nwsl ben el arkam ccw //lw fe no2ta cc n3mlha pop
	{
		public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
		{
			int numOfPoints = points.Count;
			if (numOfPoints < 3)
			{
				outPoints = points;
				return;
			}


			Point lowestPoint = FindLowestPoint(points);
			List<Point> sortedPoints = SortByPolarAngle(lowestPoint, points);

			Stack<Point> convexHull = new Stack<Point>();
			convexHull.Push(sortedPoints[0]);
			convexHull.Push(sortedPoints[1]);

			for (int i = 2; i < numOfPoints; i++)
			{
				while (convexHull.Count >= 2 && !IsLeftTurn(convexHull.ElementAt(1), convexHull.Peek(), sortedPoints[i]))
				{
					convexHull.Pop();
				}

				convexHull.Push(sortedPoints[i]);
			}

			outPoints = convexHull.ToList();
		}

		private Point FindLowestPoint(List<Point> points)
		{
			return points.OrderBy(p => p.Y).ThenBy(p => p.X).First();
		}

		private List<Point> SortByPolarAngle(Point reference, List<Point> points)
		{
			return points.OrderBy(p => Math.Atan2(p.Y - reference.Y, p.X - reference.X))
						 .ThenBy(p => DistanceSquared(reference, p))
						 .ToList();
		}

		private bool IsLeftTurn(Point a, Point b, Point c)
		{
			return HelperMethods.CheckTurn(new Line(a, b), c) == Enums.TurnType.Left;
		}

		private double DistanceSquared(Point a, Point b)
		{
			double dx = a.X - b.X;
			double dy = a.Y - b.Y;
			return dx * dx + dy * dy;
		}

		public override string ToString()
		{
			return "Convex Hull - Graham Scan";
		}
	}
}