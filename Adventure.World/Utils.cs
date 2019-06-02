using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MIConvexHull;

namespace Adventure.World
{
    public static class Utils
    {
        private static readonly Random Random = new Random();

        //(u,v) -> (x,y,z) = (2u/(R^2 + 1), 2v/(R^2 + 1), (R^2 - 1)/(R^2 + 1)) 
        public static (float X, float Y, float Z) InverseStereographicProjection(float u, float v, float r)
        {
            return (2 * u / ((float)Math.Pow(r, 2) + 1), 2 * v / ((float)Math.Pow(r, 2) + 1),
                (((float)Math.Pow(r, 2) - 1) / ((float)Math.Pow(r, 2) + 1)));
        }

        public static (float U, float V) StereographicProjection(float x, float y, float z) =>
            (x / (1 - z), y / (1 - z));

        public static IEnumerable<DefaultTriangulationCell<Vertex3>> VoronoiTriangles(IEnumerable<Vector3> points)
        {
            var voronoiMesh = VoronoiMesh.Create(points.Select(p => new Vertex3(p.X, p.Y, p.Z)).ToList());
            return voronoiMesh.Vertices;
        }

        //credit: adapted from here - https://stackoverflow.com/a/26127012/320103
        public static IEnumerable<Vector3> FibonacciSphere(int samples = 1000, bool randomize = true, float radius = 1.0f)
        {
            var rnd = 1.0f;
            if(randomize)
                rnd = Random.Next() * samples;
            var offset = 2.0f / samples;

            var increment = Math.PI * (3.0f - Math.Sqrt(5.0f));
            for (int i = 0; i < samples; i++)
            {
                var y =  i * offset - 1 + offset / 2;
                var r = Math.Sqrt(1 - Math.Pow(y, 2));
                var phi = (i + rnd) % samples * increment;

                var x = (float)(Math.Cos(phi) * r) * radius;
                var z = (float)(Math.Sin(phi) * r) * radius;
                
                yield return new Vector3(x,y,z);
            }
        }
    }
}
