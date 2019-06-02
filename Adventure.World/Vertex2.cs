using MIConvexHull;

namespace Adventure.World
{
    public class Vertex2 : IVertex
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
    }
}
