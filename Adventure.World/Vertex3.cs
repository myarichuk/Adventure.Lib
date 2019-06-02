using MIConvexHull;

namespace Adventure.World
{
    public class Vertex3 : IVertex
    {
        private readonly double[] _coords;

        public Vertex3(double x, double y, double z)
        {
            _coords = new[] { x, y, z };
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

        public double Z
        {
            get => _coords[2];
            set => _coords[2] = value;
        }

        public double[] Position => _coords;
    }
}