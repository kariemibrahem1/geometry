using CGUtilities;
using System;
using System.Collections.Generic;

namespace CGAlgorithms.Algorithms.ConvexHull
{
	public class QuickHull : Algorithm //hat min and max (x) & min and max (y) now we have 4 points
		//check if points cw or ccw with 4 vectors it decide point in or out
		//with every vector we will chose further point of it
		//recursive
	{

		public List<Point> quickHull(List<Point> points, Point min_x, Point max_x, string direction)
		{
			var segment = Enums.TurnType.Left;
			if (direction == "Right")
			{
				segment = Enums.TurnType.Right;
			}
			else
			{
				segment = Enums.TurnType.Left;
			}
			int index = -1;
			double max = -1;
			List<Point> result = new List<Point>();

			if (points.Count == 0)
				return result;

			for (int i = 0; i < points.Count; i++)
			{
				double x = Distance(min_x, max_x, points[i]);

				if (CGUtilities.HelperMethods.CheckTurn(new Line(min_x.X, min_x.Y, max_x.X, max_x.Y), points[i]) == segment && x > max)
				{
					index = i;
					max = x;
				}
			}

			if (index == -1)
			{
				result.Add(min_x);
				result.Add(max_x);
				return result;
			}

			List<Point> p1, p2;

			if (CGUtilities.HelperMethods.CheckTurn(new Line(points[index].X, points[index].Y, min_x.X, min_x.Y), max_x) == Enums.TurnType.Right)
			{
				p1 = quickHull(points, points[index], min_x, "Left");
			}
			else
			{
				p1 = quickHull(points, points[index], min_x, "Right");
			}

			if (CGUtilities.HelperMethods.CheckTurn(new Line(points[index].X, points[index].Y, max_x.X, max_x.Y), min_x) == Enums.TurnType.Right)
			{
				p2 = quickHull(points, points[index], max_x, "Left");
			}
			else
			{
				p2 = quickHull(points, points[index], max_x, "Right");
			}

			for (int i = 0; i < p2.Count; i++)
			{
				p1.Add(p2[i]);
			}

			return p1;
		}
		public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
		{
			Point max_x = new Point(-1000, 0);
			Point min_x = new Point(1000, 0);

			foreach (Point point in points)
			{
				if (point.X < min_x.X)
					min_x = point;
				if (point.X > max_x.X)
					max_x = point;
			}

			List<Point> right = quickHull(points, min_x, max_x, "Right");
			List<Point> left = quickHull(points, min_x, max_x, "Left");

			for (int i = 0; i < left.Count; i++)
			{
				right.Add(left[i]);
			}
			for (int i = 0; i < right.Count; i++)
			{
				if (!outPoints.Contains(right[i]))
					outPoints.Add(right[i]);
			}
		}
		public double Distance(Point first, Point second, Point third)
		{
			return Math.Abs((third.Y - first.Y) * (second.X - first.X) - (second.Y - first.Y) * (third.X - first.X));
		}


		public override string ToString()
		{
			return "Convex Hull - Quick Hull";
		}
	}
}
