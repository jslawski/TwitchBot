using UnityEngine;
using System.Collections;

namespace PolarCoordinates 
{

	public enum Orientation {XY, XZ};

	public class PolarCoordinate 
	{
		public float radius;
		private float angle;  //In Radians

		public float angleInRadians
		{
			get { return angle; }
		}

		public float angleInDegrees
		{
			get { return angle * Mathf.Rad2Deg; }
		}

		private float ConvertAngleTo360(float angle)
		{
			return ((2*Mathf.PI + angle) % (2*Mathf.PI));
		}

		public PolarCoordinate(float newRadius, Vector3 cartesianPoint) 
		{
			radius = newRadius;
			angle = this.ConvertAngleTo360(Mathf.Atan2(cartesianPoint.y, cartesianPoint.x));
		}

		public PolarCoordinate(float newRadius, Vector2 cartesianPoint) 
		{
			radius = newRadius;
			angle = this.ConvertAngleTo360(Mathf.Atan2(cartesianPoint.y, cartesianPoint.x));
		}

		public PolarCoordinate(Vector3 cartesianPoint, Orientation orientation = Orientation.XY) 
		{
			radius = 1;

			if (orientation == Orientation.XY)
			{
				angle = this.ConvertAngleTo360(Mathf.Atan2(cartesianPoint.y, cartesianPoint.x));
			}
			else
			{
				angle = this.ConvertAngleTo360(Mathf.Atan2(cartesianPoint.z, cartesianPoint.x));
			}
		}

		public PolarCoordinate(Vector2 cartesianPoint) 
		{
			radius = 1;
			angle = this.ConvertAngleTo360(Mathf.Atan2(cartesianPoint.y, cartesianPoint.x));
		}

		public PolarCoordinate(float newRadius, float newAngle) 
		{
			radius = newRadius;
			angle = newAngle;
		}

		public Vector3 PolarToCartesian(Orientation orientation = Orientation.XY) 
		{
			if (orientation == Orientation.XY) 
			{
				return new Vector3 (radius * Mathf.Cos (angle), radius * Mathf.Sin (angle), 0);
			}

			return new Vector3(radius * Mathf.Cos(angle), 0, radius * Mathf.Sin(angle));
		}

		public static PolarCoordinate CartesianToPolar(Vector3 cart, Orientation orientation = Orientation.XY) 
		{
			if (orientation == Orientation.XY) 
			{
				return new PolarCoordinate(Mathf.Sqrt(Mathf.Pow(cart.x, 2) + Mathf.Pow(cart.y, 2)), cart);
			}

			return new PolarCoordinate(Mathf.Sqrt(Mathf.Pow(cart.x, 2) + Mathf.Pow(cart.z, 2)), cart);
		}
	}
}
