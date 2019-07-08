using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using MIConvexHull;

namespace Adventure.World
{
    public static class Utils
    {
        private static readonly Random Random = new Random();

        public static (double X, double Y, double Z) InverseStereographicProjection(double u, double v)
        {
            var powerOfuAndvPlusOne = Math.Pow(u, 2) + Math.Pow(v, 2) + 1;
            var powerOfuAndvMinusOne = Math.Pow(u, 2) + Math.Pow(v, 2) - 1;

            return (2 * u / powerOfuAndvPlusOne, 2 * v / powerOfuAndvPlusOne,
                powerOfuAndvMinusOne / powerOfuAndvPlusOne);
        }

        public static (double U, double V) StereographicProjection(double x, double y, double z) =>
            (x / (1 + z), y / (1 + z));

        public static IEnumerable<DefaultTriangulationCell<Vertex2>> VoronoiTriangles(IEnumerable<Vertex2> points)
        {            
            var voronoiMesh = MIConvexHull.VoronoiMesh.Create(points.ToList());
            return voronoiMesh.Vertices;
        }

        public static VoronoiMesh<Vertex2, Cell, VoronoiEdge<Vertex2, Cell>> VoronoiMesh(IEnumerable<Vertex2> points) => 
            MIConvexHull.VoronoiMesh.Create<Vertex2, Cell>(points.ToList());

        public static IEnumerable<DefaultTriangulationCell<Vertex2>> DelaunayTriangles(IEnumerable<Vertex2> points)
        {            
            var triangulation = DelaunayTriangulation<Vertex2, DefaultTriangulationCell<Vertex2>>.Create(points.ToList(),0.0000000001);
            return triangulation.Cells;
        }

        //credit: adapted from here - https://stackoverflow.com/a/26127012/320103
        public static IEnumerable<Vertex3> FibonacciSphere(int samples = 1000, bool randomize = true, float radius = 1.0f)
        {
            var rnd = 1.0f;
            if(randomize)
                rnd = Random.Next(samples) * samples;
            var offset = 2.0f / samples;

            var increment = Math.PI * (3.0f - Math.Sqrt(5.0f));
            for (int i = 0; i < samples; i++)
            {
                var y =  (i * offset) - 1 + offset / 2;
                var r = Math.Sqrt(1 - Math.Pow(y, 2));
                var phi = (i + rnd) % samples * increment;

                var x = (Math.Cos(phi) * r) * radius;
                var z = (Math.Sin(phi) * r) * radius;
                
                yield return new Vertex3(x,y,z);
            }
        }
    }
}
