using CGUtilities;
using System.Collections.Generic;
using System.Linq;

namespace CGAlgorithms.Algorithms.ConvexHull
{
	public class DivideAndConquer : Algorithm
	{

		public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
		{
			points = points?.OrderBy(p => p.X)?.ThenBy(p => p.Y)?.ToList();
			outPoints = Divide(points);

		}
		public List<Point> Divide(List<Point> inputPoints)
		{
			List<Point> partitionB = new List<Point>();
			List<Point> partitionA = new List<Point>();

			if (inputPoints.Count <= 1)
			{
				return inputPoints;
			}

			int midpoint = inputPoints.Count / 2;

			for (int i = 0; i < midpoint; i++)
			{
				partitionA.Add(inputPoints[i]);
			}

			for (int i = midpoint; i < inputPoints.Count; i++)
			{
				partitionB.Add(inputPoints[i]);
			}

			List<Point> mergedPoints = Combine(Divide(partitionA), Divide(partitionB));

			return mergedPoints;
		}
		public List<Point> Combine(List<Point> setA, List<Point> setB)
		{
			int mostRightIndexB = 0,
			mostLeftIndexA = 0,
			countSetB = setB.Count,
			countSetA = setA.Count;

			for (var index = 1; index < countSetA; index++)
				if (setA[index].X > setA[mostRightIndexB].X || (setA[index].X == setA[mostRightIndexB].X && setA[index].Y > setA[mostRightIndexB].Y))
					mostRightIndexB = index;

			for (var index = 1; index < countSetB; index++)
				if (setB[index].X < setB[mostLeftIndexA].X || (setB[index].X == setB[mostLeftIndexA].X && setB[index].Y < setB[mostLeftIndexA].Y))
					mostLeftIndexA = index;


			bool foundUpper = false;
			int upperTanB = mostLeftIndexA;
			int upperTanA = mostRightIndexB;

			while (!foundUpper)
			{
				foundUpper = true;

				for (; CGUtilities.HelperMethods.CheckTurn(new Line(setB[upperTanB].X, setB[upperTanB].Y, setA[upperTanA].X, setA[upperTanA].Y),
		setA[(upperTanA + 1) % countSetA]) == Enums.TurnType.Right; foundUpper = false)
				{
					upperTanA = (upperTanA + 1) % countSetA;
				}


				if (CGUtilities.HelperMethods.CheckTurn(
		new Line(setB[upperTanB].X, setB[upperTanB].Y, setA[upperTanA].X, setA[upperTanA].Y),
		setA[(upperTanA + 1) % countSetA]) == Enums.TurnType.Colinear)
					upperTanA = (upperTanA + 1) % countSetA;


				for (; CGUtilities.HelperMethods.CheckTurn(new Line(setA[upperTanA].X, setA[upperTanA].Y, setB[upperTanB].X, setB[upperTanB].Y),
			setB[(countSetB + upperTanB - 1) % countSetB]) == Enums.TurnType.Left; foundUpper = false)
				{
					upperTanB = (countSetB + upperTanB - 1) % countSetB;
				}


				if (CGUtilities.HelperMethods.CheckTurn(
	new Line(setA[upperTanA].X, setA[upperTanA].Y, setB[upperTanB].X, setB[upperTanB].Y),
	setB[(upperTanB - 1 + countSetB) % countSetB]) == Enums.TurnType.Colinear)
					upperTanB = (upperTanB - 1 + countSetB) % countSetB;

			}

			bool foundLower = false;
			int lowerTanB = mostLeftIndexA;
			int lowerTanA = mostRightIndexB;

			do
			{
				foundLower = true;

				while (CGUtilities.HelperMethods.CheckTurn(new Line(setB[lowerTanB].X, setB[lowerTanB].Y, setA[lowerTanA].X, setA[lowerTanA].Y),
					setA[(lowerTanA + countSetA - 1) % countSetA]) == Enums.TurnType.Left)
				{
					foundLower = false;
					lowerTanA = (lowerTanA + countSetA - 1) % countSetA;
				}

				if (CGUtilities.HelperMethods.CheckTurn(new Line(setB[lowerTanB].X, setB[lowerTanB].Y, setA[lowerTanA].X, setA[lowerTanA].Y),
						setA[(lowerTanA + countSetA - 1) % countSetA]) == Enums.TurnType.Colinear)
				{
					lowerTanA = (lowerTanA + countSetA - 1) % countSetA;
				}

				while (CGUtilities.HelperMethods.CheckTurn(new Line(setA[lowerTanA].X, setA[lowerTanA].Y, setB[lowerTanB].X, setB[lowerTanB].Y),
					setB[(lowerTanB + 1) % countSetB]) == Enums.TurnType.Right)
				{
					foundLower = false;
					lowerTanB = (lowerTanB + 1) % countSetB;
				}

				if (CGUtilities.HelperMethods.CheckTurn(new Line(setA[lowerTanA].X, setA[lowerTanA].Y, setB[lowerTanB].X, setB[lowerTanB].Y),
					setB[(lowerTanB + 1) % countSetB]) == Enums.TurnType.Colinear)
				{
					lowerTanB = (lowerTanB + 1) % countSetB;
				}
			} while (!foundLower);

			List<Point> convexHull = new List<Point>();

			convexHull.Add(setA[upperTanA]);

			while (upperTanA != lowerTanA)
			{
				upperTanA = (upperTanA + 1) % countSetA;

				if (!convexHull.Contains(setA[upperTanA]))
				{
					convexHull.Add(setA[upperTanA]);
				}
			}

			convexHull.Add(setB[lowerTanB]);

			while (lowerTanB != upperTanB)
			{
				lowerTanB = (lowerTanB + 1) % countSetB;

				if (!convexHull.Contains(setB[lowerTanB]))
				{
					convexHull.Add(setB[lowerTanB]);
				}
			}

			return convexHull;
		}


		public override string ToString()
		{
			return "Convex Hull - Divide & Conquer";
		}

	}
}