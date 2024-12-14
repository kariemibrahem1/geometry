using CGUtilities;
using System;
using System.Collections.Generic;

namespace CGAlgorithms.Algorithms.ConvexHull
{
	public class JarvisMarch : Algorithm //bngeb minimum point on y axis //bnshof l no2ta elly 3leha eldoor //break lma nwsl l awl no2ta
	{

		public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
		{
			int NumberOfPoints = points.Count;

			if (NumberOfPoints < 3)
			{
				outPoints = points;
				return;
			}


			double MinimumY = double.MaxValue;
			double MinimumX = 0;

			foreach (Point point in points)
			{
				if (point.Y < MinimumY)
				{
					MinimumY = point.Y;
					MinimumX = point.X;
				}
			}


			Point MinimumPoint = new Point(MinimumX, MinimumY);
			outPoints.Add(MinimumPoint);
			Point StartPoint = MinimumPoint;


			Point extraPoint = new Point(MinimumX - 20, MinimumY);


			while (true)
			{
				double largestTheta = 0;
				double distance = 0;
				double largestDistance = 0;
				Point nextPoint = MinimumPoint;


				foreach (Point point in points)
				{

					Point minPoint_extraPoint = new Point(MinimumPoint.X - extraPoint.X, MinimumPoint.Y - extraPoint.Y);
					Point minPoint_nextPoint = new Point(point.X - MinimumPoint.X, point.Y - MinimumPoint.Y);


					double dotProduct = DotProduct(minPoint_extraPoint, minPoint_nextPoint);
					double crossProduct = HelperMethods.CrossProduct(minPoint_extraPoint, minPoint_nextPoint);


					distance = Distance(MinimumPoint, point);


					double theta = TheTa(crossProduct, dotProduct);


					if (theta < 0)
					{
						theta += 2 * Math.PI;
					}

					if (theta > largestTheta)
					{
						largestTheta = theta;
						largestDistance = distance;
						nextPoint = point;
					}
					else if (theta == largestTheta && distance > largestDistance)
					{
						largestDistance = distance;
						nextPoint = point;
					}
				}


				if (StartPoint.X == nextPoint.X && StartPoint.Y == nextPoint.Y)
					break;


				outPoints.Add(nextPoint);


				extraPoint = MinimumPoint;
				MinimumPoint = nextPoint;
			}
		}


		private double Distance(Point Point1, Point Point2)
		{
			return Math.Sqrt(Math.Pow(Point1.X - Point2.X, 2) + Math.Pow(Point1.Y - Point2.Y, 2));
		}


		private double TheTa(double Cross_Product, double Dot_Product)
		{
			return Math.Atan2(Cross_Product, Dot_Product);
		}
		public double DotProduct(Point Point1, Point Point2)
		{
			return Point1.X * Point2.X + Point1.Y * Point2.Y;
		}

		public override string ToString()
		{
			return "Convex Hull - Jarvis March";
		}
	}
}
