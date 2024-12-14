using System;
using System.Collections.Generic;
using System.Linq;
using CGUtilities;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
	class MonotoneTriangulation : Algorithm
	{
		public class ChainPoint
		{
			public Point Point;
			public int Chain;
			public ChainPoint(Point point, int chain)
			{
				Point = point;
				Chain = chain;
			}
		}

		public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
		{
			if (polygons.Count == 0)
				return;

			List<Point> polygonPoints = polygons[0].lines.Select(line => line.Start).ToList();
			CheckPolygon(polygonPoints);

			List<ChainPoint> chainPoints = CheckMonotone(polygonPoints);
			if (chainPoints == null)
				return;

			chainPoints = Sort(chainPoints);

			Stack<ChainPoint> stack = new Stack<ChainPoint>();
			stack.Push(chainPoints[0]);
			stack.Push(chainPoints[1]);

			int i = 2;

			while (i < polygonPoints.Count - 1)
			{
				ChainPoint top = stack.Peek();
				ChainPoint top2;

				switch (chainPoints[i].Chain)
				{
					case var chain when chain == top.Chain:
						stack.Pop();
						top2 = stack.Peek();
						Enums.TurnType turnType = HelperMethods.CheckTurn(new Line(top2.Point, top.Point), chainPoints[i].Point);

						switch (turnType)
						{
							case Enums.TurnType.Right when top.Chain == 1:
							case Enums.TurnType.Left when top.Chain == -1:
								outLines.Add(new Line(chainPoints[i].Point, top2.Point));
								break;
							default:
								stack.Push(top);
								stack.Push(chainPoints[i]);
								i++;
								break;
						}
						break;

					default:
						while (stack.Count != 1)
						{
							top2 = stack.Pop();
							outLines.Add(new Line(chainPoints[i].Point, top2.Point));
						}
						stack.Pop();
						stack.Push(top);
						stack.Push(chainPoints[i]);
						i++;
						break;
				}
			}
		}

		private List<ChainPoint> Sort(List<ChainPoint> chainPoints)
		{
			List<ChainPoint> result = new List<ChainPoint> { chainPoints[0] };
			int left = 1;
			int right = chainPoints.Count - 1;

			while (chainPoints[left].Chain != 0 || chainPoints[right].Chain != 0)
			{
				switch (chainPoints[left].Point.Y.CompareTo(chainPoints[right].Point.Y))
				{
					case 1:
						result.Add(chainPoints[left]);
						left = (left + 1) % chainPoints.Count;
						break;
					default:
						result.Add(chainPoints[right]);
						right = (right + chainPoints.Count - 1) % chainPoints.Count;
						break;
				}
			}

			result.Add(chainPoints[left]);
			return result;
		}

		public List<ChainPoint> CheckMonotone(List<Point> points)
		{
			int maxIndex = points.Select((point, index) => new { Index = index, Y = point.Y }).OrderByDescending(item => item.Y).First().Index;
			int minIndex = points.Select((point, index) => new { Index = index, Y = point.Y }).OrderBy(item => item.Y).First().Index;

			double prev;

			List<ChainPoint> chainPoints = new List<ChainPoint> { new ChainPoint(points[maxIndex], 0) };

			for (int i = (maxIndex + 1) % points.Count; i != minIndex; i = (i + 1) % points.Count)
			{
				prev = points[i].Y;
				chainPoints.Add(new ChainPoint(points[i], -1));

				switch (prev < points[i].Y)
				{
					case true:
						return null;
				}
			}

			prev = points[minIndex].Y;
			chainPoints.Add(new ChainPoint(points[minIndex], 0));

			for (int i = (minIndex + 1) % points.Count; i != maxIndex; i = (i + 1) % points.Count)
			{
				prev = points[i].Y;
				chainPoints.Add(new ChainPoint(points[i], 1));

				switch (prev > points[i].Y)
				{
					case true:
						return null;
				}
			}

			return chainPoints;
		}

		public void CheckPolygon(List<Point> points)
		{
			int minIndex = points
				.Select((point, index) => new { Index = index, X = point.X })
				.OrderBy(item => item.X)
				.First()
				.Index;

			int prev = (minIndex - 1 + points.Count) % points.Count;
			int next = (minIndex + 1 + points.Count) % points.Count;

			switch (HelperMethods.CheckTurn(new Line(points[prev], points[next]), points[minIndex]))
			{
				case Enums.TurnType.Left:
					points.Reverse();
					break;
			}
		}
		public override string ToString()
		{
			return "Monotone Triangulation";
		}
	}
}