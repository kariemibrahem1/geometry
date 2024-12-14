using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;
using CGAlgorithms;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{

	public class PointEqualityComparer : IEqualityComparer<Point>
	{

		public bool Equals(Point p1, Point p2)
		{
			return Math.Abs(p1.X - p2.X) < Constants.Epsilon && Math.Abs(p1.Y - p2.Y) < Constants.Epsilon;
		}

		public int GetHashCode(Point p)
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 23 + p.X.GetHashCode();
				hash = hash * 23 + p.Y.GetHashCode();
				return hash;
			}
		}
	}

	public class LineEqualityComparer : IEqualityComparer<Line>
	{

		public bool Equals(Line l1, Line l2)
		{
			return (l1.Start == l2.Start && l1.End == l2.End);
		}

		public int GetHashCode(Line l)
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 23 + l.Start.GetHashCode();
				hash = hash * 23 + l.End.GetHashCode();
				return hash;
			}
		}
	}

	enum Vertex
	{
		Start,
		Split,
		End,
		Merge,
		Regular
	}
	class MonotonePartitioning : Algorithm
	{
		public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
		{
			List<Point> y = new List<Point>();
			Dictionary<int, Vertex> type = new Dictionary<int, Vertex>();
			Dictionary<int, List<int>> neighbours = new Dictionary<int, List<int>>();


			foreach (Line line in lines)
			{
				y.Add(line.Start);
				y.Add(line.End);
				//Console.WriteLine($"start = ({line.Start.X}, {line.Start.Y})   end = ({line.End.X}, {line.End.Y})");
			}

			y = y.Distinct(new PointEqualityComparer()).ToList();
			y = y.OrderBy(i => i.Y).ThenBy(i => i.X).ToList();
			y.Reverse();

			for (int i = 0; i < lines.Count; i++)
			{
				int startIdx = y.IndexOf(lines[i].Start), endIdx = y.IndexOf(lines[i].End);
				if (!neighbours.ContainsKey(startIdx))
					neighbours[startIdx] = new List<int>();
				if (!neighbours.ContainsKey(endIdx))
					neighbours[endIdx] = new List<int>();
				neighbours[startIdx].Insert(0, endIdx);
				neighbours[endIdx].Add(startIdx);
			}

			foreach (Point point in y)
			{
				int idx = y.IndexOf(point);
				double angle = getAngle(y[neighbours[idx][1]], point, y[neighbours[idx][0]]) * 180 / Math.PI;

				if (point.Y > y[neighbours[idx][1]].Y && point.Y > y[neighbours[idx][0]].Y && angle > 0)
					type[idx] = Vertex.Start;
				else if (point.Y > y[neighbours[idx][1]].Y && point.Y > y[neighbours[idx][0]].Y && angle < 0)
					type[idx] = Vertex.Split;
				else if (point.Y < y[neighbours[idx][1]].Y && point.Y < y[neighbours[idx][0]].Y && angle > 0)
					type[idx] = Vertex.End;
				else if (point.Y < y[neighbours[idx][1]].Y && point.Y < y[neighbours[idx][0]].Y && angle < 0)
					type[idx] = Vertex.Merge;
				else
					type[idx] = Vertex.Regular;
			}

			//for (int i = 0; i < y.Count; i++)
			//{
			//    if (type[i] == Vertex.Regular)
			//        outPoints.Add(y[i]);
			//}

			for (int i = 0; i < y.Count; i++)
			{
				Point point = y[i];
				Console.Write($"({point.X}, {point.Y}) : ");
				foreach (int neighbor in neighbours[i])
				{
					Console.Write($"({y[neighbor].X}, {y[neighbor].Y})  ");
				}
				Console.WriteLine();
			}

			List<Line> t = new List<Line>();
			Dictionary<Line, Point> helper = new Dictionary<Line, Point>(new LineEqualityComparer());
			for (int i = 0; i < y.Count; i++)
			{
				if (type[i] == Vertex.Start)
				{
					helper[new Line(y[i], y[neighbours[i][1]])] = y[i];
					t.Add(new Line(y[i], y[neighbours[i][1]]));
				}
				else if (type[i] == Vertex.Split)
				{
					Point tmp = new Point(0, 0);
					int k = -1;
					for (int j = i - 1; j >= 0; j--)
					{
						if (y[j].X < y[i].X)
						{
							k = j;
							tmp = y[j];
							break;
						}
					}
					if (k != -1)
					{
						outLines.Add(new Line(y[i], tmp));
						t.Add(new Line(tmp, y[neighbours[y.IndexOf(tmp)][1]]));
						helper[new Line(tmp, y[neighbours[y.IndexOf(tmp)][1]])] = y[i];
					}
				}
				else if (type[i] == Vertex.Regular)
				{

					if (rightOrLeft(y, y[i]) == 0 && t.Count > 0)
					{
						Line lastEdge = t[t.Count - 1];
						Point h = helper[lastEdge];
						int idx = y.IndexOf(h);

						t.RemoveAt(t.Count - 1);
						t.Add(new Line(y[i], y[neighbours[i][1]]));
						helper[new Line(y[i], y[neighbours[i][1]])] = y[i];

						if (type[idx] == Vertex.Merge)
							outLines.Add(new Line(y[i], h));
					}
					else
					{
						Point tmp = new Point(0, 0);
						int k = -1;
						for (int j = i - 1; j >= 0; j--)
						{
							if (y[j].X < y[i].X)
							{
								k = j;
								tmp = y[j];
								break;
							}
						}
						if (k != -1)
						{
							Line l = new Line(tmp, y[neighbours[y.IndexOf(tmp)][1]]);
							if (helper.ContainsKey(l))
							{
								Point h = helper[l];
								int idx = y.IndexOf(h);

								if (type[idx] == Vertex.Merge)
								{
									outLines.Add(new Line(y[i], h));
									helper[l] = y[i];
								}
							}

						}
					}

				}
				else if (type[i] == Vertex.Merge)
				{
					Line lastEdge = t[t.Count - 1];
					Point h = helper[lastEdge];
					int idx = y.IndexOf(h);

					if (type[idx] == Vertex.Merge)
					{
						outLines.Add(new Line(y[i], h));
					}

					t.RemoveAt(t.Count - 1);
					Point tmp = new Point(0, 0);
					int k = -1;
					for (int j = i - 1; j >= 0; j--)
					{
						if (y[j].X < y[i].X)
						{
							k = j;
							tmp = y[j];
							break;
						}
					}
					if (k != -1)
					{
						Line l = new Line(tmp, y[neighbours[k][1]]);
						if (!helper.ContainsKey(l))
							continue;
						h = helper[l];
						idx = y.IndexOf(h);
						if (type[idx] == Vertex.Merge)
						{
							outLines.Add(new Line(y[i], h));
						}
						helper[l] = y[i];
					}
				}
				else if (type[i] == Vertex.End && t.Count > 0)
				{
					Line lastEdge = t[t.Count - 1];
					Point h = helper[lastEdge];
					int idx = y.IndexOf(h);

					if (type[idx] == Vertex.Merge)
					{
						outLines.Add(new Line(y[i], h));
					}
					t.RemoveAt(t.Count - 1);
				}

			}



		}

		public override string ToString()
		{
			return "Monotone Partitioning";
		}

		// 0 if left
		// 1 if right
		int rightOrLeft(List<Point> y, Point p)
		{
			int leftCounter = 0, rightCounter = 0;
			for (int i = 0; i < y.Count; i++)
			{
				if (y[i] == p)
					continue;
				if (p.X > y[i].X)
					leftCounter++;
				else
					rightCounter++;
			}
			if (leftCounter > rightCounter)
				return 0;
			return 1;
		}

		public static double getAngle(Point p, Point x, Point y)
		{
			Point v1 = x.Vector(p);
			Point v2 = x.Vector(y);

			double dotProduct = v1.X * v2.X + v1.Y * v2.Y;
			double magnitudeProduct = v1.Magnitude() * v2.Magnitude();

			double angle = Math.Acos(dotProduct / magnitudeProduct);

			if (HelperMethods.CheckTurn(v1, v2) == Enums.TurnType.Right)
				return -angle;
			else
				return angle;
		}


	}
}