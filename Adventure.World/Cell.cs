using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MIConvexHull;

namespace Adventure.World
{
    /// <summary>
    /// A vertex is a simple class that stores the postion of a Vertex2, node or vertex.
    /// </summary>
    public class Cell : TriangulationCell<Vertex2, Cell>
    {
        private double Det(double[,] m) => 
            m[0, 0] * (m[1, 1] * m[2, 2] - m[2, 1] * m[1, 2]) - m[0, 1] * (m[1, 0] * m[2, 2] - m[2, 0] * m[1, 2]) + m[0, 2] * (m[1, 0] * m[2, 1] - m[2, 0] * m[1, 1]);

        private double LengthSquared(double[] v)
        {
            double norm = 0;
            for (int i = 0; i < v.Length; i++)
            {
                var t = v[i];
                norm += t * t;
            }
            return norm;
        }

        private Vertex2 GetCircumcenter()
        {
            // From MathWorld: http://mathworld.wolfram.com/Circumcircle.html
            var Vertex2s = Vertices;

            double[,] m = new double[3, 3];

            // x, y, 1
            for (int i = 0; i < 3; i++)
            {
                m[i, 0] = Vertex2s[i].Position[0];
                m[i, 1] = Vertex2s[i].Position[1];
                m[i, 2] = 1;
            }
            var a = Det(m);

            // size, y, 1
            for (int i = 0; i < 3; i++)
            {
                m[i, 0] = LengthSquared(Vertex2s[i].Position);
            }
            var dx = -Det(m);

            // size, x, 1
            for (int i = 0; i < 3; i++)
            {
                m[i, 1] = Vertex2s[i].Position[0];
            }
            var dy = Det(m);

            // size, x, y
            for (int i = 0; i < 3; i++)
            {
                m[i, 2] = Vertex2s[i].Position[1];
            }
            var c = -Det(m);

            var s = -1.0 / (2.0 * a);
            var r = System.Math.Abs(s) * System.Math.Sqrt(dx * dx + dy * dy - 4 * a * c);
            return new Vertex2(s * dx, s * dy);
        }

        private Vertex2 GetCentroid() => 
            new Vertex2(Vertices.Select(v => v.Position[0]).Average(), Vertices.Select(v => v.Position[1]).Average());

        private Vertex2 _circumCenter;

        public Vertex2 Circumcenter
        {
            get
            {
                _circumCenter = _circumCenter ?? GetCircumcenter();
                return _circumCenter;
            }
        }

        private Vertex2 _centroid;
        public Vertex2 Centroid
        {
            get
            {
                _centroid = _centroid ?? GetCentroid();
                return _centroid;
            }
        }
    }
}
