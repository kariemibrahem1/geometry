using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
	class SubtractingEars : Algorithm
	{
		public override void Run(List<Point> inputPoints, List<Line> inputLines, List<Polygon> inputPolygons, ref List<Point> outputPoints, ref List<Line> outputLines, ref List<Polygon> outputPolygons)
		{
			outputLines = inputPolygons
				.SelectMany(polygon => RemoveEarsAndGetLines(polygon.lines.Select(line => line.Start).ToList()))
				.ToList();
		}

		private List<Line> RemoveEarsAndGetLines(List<Point> vertices)
		{
			var result = new List<Line>();
			SortVertices(ref vertices);
			var earsIndices = Enumerable.Range(0, vertices.Count).Where(i => IsEar(i, vertices)).ToList();

			while (earsIndices.Any() && vertices.Count > 3)
			{
				result.Add(RemoveEarVertex(earsIndices.First(), ref earsIndices, ref vertices));
			}

			return result;
		}

		private Line RemoveEarVertex(int index, ref List<int> earsIndices, ref List<Point> vertices)
		{
			int count = vertices.Count;
			Line diagonal = new Line(vertices[(index - 1 + count) % count], vertices[(index + 1) % count]);
			count--;
			vertices.RemoveAt(index);
			earsIndices.Remove(index);
			earsIndices = earsIndices.Where(i => i != index && i != (index - 1 + count) % count).Select(i => i > index ? i - 1 : i).ToList();

			AddOrRemoveEar(index % count, vertices, earsIndices);
			AddOrRemoveEar((index - 1 + count) % count, vertices, earsIndices);

			return diagonal;
		}

		private void AddOrRemoveEar(int index, List<Point> vertices, List<int> earsIndices)
		{
			var isEar = IsEar(index, vertices);
			if (isEar && !earsIndices.Contains(index))
				earsIndices.Add(index);
			else if (!isEar && earsIndices.Contains(index))
				earsIndices.Remove(index);
		}

		private bool IsEar(int index, List<Point> vertices)
		{
			int count = vertices.Count;
			if (!IsConvexVertex(vertices[index], vertices[(index - 1 + count) % count], vertices[(index + 1) % count]))
				return false;

			return vertices.Where((_, i) => i != index && i != (index - 1 + count) % count && i != (index + 1) % count)
						   .All(point => HelperMethods.PointInTriangle(point, vertices[(index - 1 + count) % count], vertices[index], vertices[(index + 1) % count]) == Enums.PointInPolygon.Outside);
		}

		private void SortVertices(ref List<Point> vertices)
		{
			int index = vertices.IndexOf(vertices.OrderBy(p => p.X).ThenBy(p => p.Y).First());
			int count = vertices.Count;
			if (HelperMethods.CheckTurn(new Line(vertices[(index - 1 + count) % count], vertices[(index + 1) % count]), vertices[index]) == Enums.TurnType.Right)
				return;

			vertices.Reverse();
		}

		private bool IsConvexVertex(Point p, Point pPrev, Point pNext)
		{
			return HelperMethods.CheckTurn(new Line(pPrev, p), pNext) == Enums.TurnType.Left;
		}

		public override string ToString()
		{
			return "Subtracting Ears";
		}
	}
}