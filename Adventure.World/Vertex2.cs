using System;
using MIConvexHull;

namespace Adventure.World
{
    public class Vertex2 : IVertex, IEquatable<Vertex2>
    {
        private readonly double[] _coords;

        public Vertex2(double x, double y)
        {
            _coords = new[] { x, y };
        }

        public double X
        {
            get => _coords[0];
            set => _coords[0] = value;
        }
        public double Y
        {
            get => _coords[1];
            set => _coords[1] = value;
        }
 
        public double[] Position => _coords;

        public bool Equals(Vertex2 other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (_coords.Length != other._coords.Length) //precaution
                return false;

            for (int i = 0; i < _coords.Length; i++)
            {
                if (Math.Abs(_coords[i] - other._coords[i]) > double.Epsilon)
                    return false;
            }

            return true;

        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Vertex2) obj);
        }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 31 + X.GetHashCode();
            hash = hash * 31 + Y.GetHashCode();
            return hash;
        }

        public static bool operator ==(Vertex2 left, Vertex2 right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Vertex2 left, Vertex2 right)
        {
            return !Equals(left, right);
        }

        public static Vertex2 operator+ (Vertex2 left, Vertex2 right) => 
            new Vertex2(left.X + right.X, left.Y + right.Y);

        public static Vertex2 operator- (Vertex2 left, Vertex2 right) => 
            new Vertex2(left.X - right.X, left.Y - right.Y);

        public static Vertex2 operator /(Vertex2 v, int scalar) =>
            new Vertex2(v.X / scalar, v.Y / scalar);

        public static Vertex2 operator* (Vertex2 v, int scalar) => 
            new Vertex2(v.X * scalar, v.Y * scalar);

    }
}
