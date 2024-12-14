using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CGAlgorithms.Algorithms.SegmentIntersection
{
	class SweepLine : Algorithm
	{
		List<Line> lineSegments;

		public override void Run(List<Point> inputPoints, List<Line> inputLines, List<Polygon> inputPolygons, ref List<Point> outputPoints, ref List<Line> outputLines, ref List<Polygon> outputPolygons)
		{
			lineSegments = inputLines
				.Select(line => new Line(new Point(line.Start.X, line.Start.Y), new Point(line.End.X, line.End.Y)))
				.ToList();

			foreach (var segment in lineSegments.Where(seg => seg.Start.X > seg.End.X))
			{
				SwapPoints(segment);
			}

			foreach (var segment in lineSegments)
			{
				foreach (var otherSegment in lineSegments.SkipWhile(s => s != segment).Skip(1))
				{
					Point intersection = FindIntersection(segment, otherSegment);
					if (intersection != null)
						outputPoints.Add(intersection);
				}
			}
		}

		private void SwapPoints(Line line)
		{
			Point temp = new Point(line.Start.X, line.Start.Y);
			line.Start.X = line.End.X;
			line.Start.Y = line.End.Y;
			line.End.X = temp.X;
			line.End.Y = temp.Y;
		}

		private Point FindIntersection(Line a, Line b)
		{
			double x1 = a.Start.X, y1 = a.Start.Y, x2 = a.End.X, y2 = a.End.Y;
			double x3 = b.Start.X, y3 = b.Start.Y, x4 = b.End.X, y4 = b.End.Y;

			double den = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

			if (Math.Abs(den) < Constants.Epsilon)
				return null;

			double px = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / den;
			double py = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / den;

			Point intersection = new Point(px, py);

			if (IsPointOnSegment(a, intersection) && IsPointOnSegment(b, intersection))
				return intersection;

			return null;
		}

		private bool IsPointOnSegment(Line segment, Point point)
		{
			double minX = Math.Min(segment.Start.X, segment.End.X);
			double maxX = Math.Max(segment.Start.X, segment.End.X);
			double minY = Math.Min(segment.Start.Y, segment.End.Y);
			double maxY = Math.Max(segment.Start.Y, segment.End.Y);

			if (point.X < minX || point.X > maxX || point.Y < minY || point.Y > maxY)
				return false;

			double dx = segment.End.X - segment.Start.X;
			double dy = segment.End.Y - segment.Start.Y;
			double tolerance = Constants.Epsilon;

			if (Math.Abs(dx) > tolerance)
			{
				double a = dy / dx;
				double b = segment.Start.Y - a * segment.Start.X;
				return Math.Abs(point.Y - (a * point.X + b)) < tolerance;
			}

			return Math.Abs(point.X - segment.Start.X) < tolerance;
		}

		public override string ToString()
		{
			return "Sweep Line";
		}
	}
}