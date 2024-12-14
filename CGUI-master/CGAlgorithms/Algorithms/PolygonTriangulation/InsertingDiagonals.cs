using System.Collections.Generic;
using System.Linq;
using CGUtilities;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
	class DiagonalInsertionAlgorithm : Algorithm //check if it ccw
	{
		public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
		{
			if (polygons.Count == 0)
				return;

			List<Point> polygonPoints = polygons[0].lines.Select(line => line.Start).ToList();
			CheckPolygon(polygonPoints);
			outLines = InsertDiagonals(polygonPoints);
		}

		public void CheckPolygon(List<Point> points)
		{
			int minIndex = 0;

			foreach (var point in points)
			{
				if (point.X < points[minIndex].X)
				{
					minIndex = points.IndexOf(point);
				}
			}


			int nextIndex = (minIndex + 1 + points.Count) % points.Count;
			int previousIndex = (minIndex - 1 + points.Count) % points.Count;

			if (HelperMethods.CheckTurn(new Line(points[previousIndex], points[nextIndex]), points[minIndex]) == Enums.TurnType.Left)
				points.Reverse();
		}
		private List<Line> InsertDiagonals(List<Point> points)
		{
			if (points.Count > 3)
			{
				List<Point> part1 = new List<Point>();
				List<Point> part2 = new List<Point>();
				int convexPointIndex = GetConvexPointIndex(points);
				List<Line> resultLines = new List<Line>();

				if (convexPointIndex == -1)
					return new List<Line>();

				int previousIndex = (convexPointIndex - 1 + points.Count) % points.Count;
				int nextIndex = (convexPointIndex + 1) % points.Count;
				int maxPointIndex = FindMaxPointIndex(points, previousIndex, nextIndex, convexPointIndex);

				if (maxPointIndex == -1)
					resultLines.Add(new Line(points[previousIndex], points[nextIndex]));
				else
					resultLines.Add(new Line(points[convexPointIndex], points[maxPointIndex]));

				
				int startIndex = maxPointIndex == -1 ? nextIndex : convexPointIndex;
				int endIndex = maxPointIndex == -1 ? previousIndex : maxPointIndex;

				if (startIndex < endIndex)
				{
					part1 = points.GetRange(startIndex, endIndex - startIndex + 1);
					part2.AddRange(points.GetRange(0, startIndex + 1));
					part2.AddRange(points.GetRange(endIndex, points.Count - endIndex));
				}
				else
				{
					part1.AddRange(points.GetRange(startIndex, points.Count - startIndex));
					part1.AddRange(points.GetRange(0, endIndex + 1));
					part2 = points.GetRange(endIndex, startIndex - endIndex + 1);
				}

				resultLines.AddRange(InsertDiagonals(part1));
				resultLines.AddRange(InsertDiagonals(part2));
				return resultLines;
			}

			return new List<Line>();
		}

		private int GetConvexPointIndex(List<Point> points)
		{
			int index = 0;

			foreach (var point in points)
			{
				if (IsConvex(points, index))
					return index;

				index++;
			}


			return -1;
		}
		private bool IsConvex(List<Point> points, int index)
		{
			int nextIndex = (index + 1) % points.Count;
			int previousIndex = (index - 1 + points.Count) % points.Count;

			if (HelperMethods.CheckTurn(new Line(points[previousIndex], points[nextIndex]), points[index]) == Enums.TurnType.Right)
				return true;

			return false;
		}



		private int FindMaxPointIndex(List<Point> points, int previousIndex, int nextIndex, int currentIndex)
		{
			int maxIndex = -1;
			double maxDistance = -1e6;

			foreach (var point in points)
			{
				if (HelperMethods.PointInTriangle(point, points[currentIndex], points[previousIndex], points[nextIndex]) == Enums.PointInPolygon.Inside)
				{
					double distance = HelperMethods.LinePointDist(new Line(points[previousIndex], points[nextIndex]), point);
					if (distance > maxDistance)
					{
						maxDistance = distance;
						maxIndex = points.IndexOf(point);
					}
				}
			}


			return maxIndex;
		}


		public override string ToString()
		{
			return "Inserting Diagonals";
		}
	}
}
