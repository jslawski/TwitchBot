using UnityEngine;
using System.Collections;

namespace PolarCoordinates 
{
	public enum Orientation {XY, XZ};

	public class PolarCoordinate 
	{
		public float radius;
		private float angle;  //In Radians
        private Vector3 originPoint = Vector3.zero;

		public float angleInRadians
		{
			get { return angle; }
            set { angle = value; }
		}

		public float angleInDegrees
		{
			get { return angle * Mathf.Rad2Deg; }
            set { angle = (value * Mathf.Rad2Deg); }
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

        public PolarCoordinate(float newRadius, Vector3 cartesianPoint, Vector3 originPoint)
        {
            radius = newRadius;
            angle = this.ConvertAngleTo360(Mathf.Atan2(cartesianPoint.y, cartesianPoint.x));
            this.originPoint = originPoint;
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

        public PolarCoordinate(float newRadius, float newAngle, Vector3 newOrigin) 
		{
			radius = newRadius;
			angle = newAngle;
            this.originPoint = newOrigin;
		}

		public Vector3 PolarToCartesian(Orientation orientation = Orientation.XY) 
		{
			if (orientation == Orientation.XY) 
			{
				return new Vector3 (radius * Mathf.Cos(angle) + this.originPoint.x, radius * Mathf.Sin(angle) + +this.originPoint.y, 0);
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

        public static PolarCoordinate CartesianToPolar(Vector3 cart, Vector3 origin, Orientation orientation = Orientation.XY)
        {
            if (orientation == Orientation.XY)
            {
                return new PolarCoordinate(Mathf.Sqrt(Mathf.Pow(cart.x + origin.x, 2) + Mathf.Pow(cart.y + origin.y, 2)), cart, origin);
            }

            return new PolarCoordinate(Mathf.Sqrt(Mathf.Pow(cart.x + origin.x, 2) + Mathf.Pow(cart.z + origin.z, 2)), cart, origin);
        }
    }
}
