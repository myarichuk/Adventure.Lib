using System;
using System.Collections.Generic;
using System.Linq;
using MIConvexHull;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.Operation.Polygonize;

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

        public static IEnumerable<Vertex2>[] Polygonize(IEnumerable<(Vertex2 from, Vertex2 to)> lines)
        {
            var polygonizer = new Polygonizer();
            var geometryFactory = new GeometryFactory();
            var wktReader = new WKTReader(geometryFactory);
            
            foreach (var line in lines)
                polygonizer.Add(wktReader.Read($"LINESTRING ({line.@from.X} {line.@from.Y}, {line.to.X} {line.to.Y})"));

            return polygonizer.GetPolygons().Select(p => p.Coordinates.Select(c => new Vertex2(c.X, c.Y))).ToArray();
        }

        public static (double U, double V) StereographicProjection(double x, double y, double z) =>
            (x / (1 + z), y / (1 + z));

        public static IEnumerable<Cell> VoronoiTriangles(IEnumerable<Vertex2> points)
        {            
            var voronoiMesh = MIConvexHull.VoronoiMesh.Create<Vertex2, Cell>(points.ToList());
            return voronoiMesh.Vertices;
        }

        public static int IsLeft(Vertex2 a, Vertex2 b, Vertex2 c) => 
            (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X) > 0 ? 1 : -1;

        public static Vertex2 Center(Cell c)
        {
            var v1 = c.Vertices[0];
            var v2 = c.Vertices[1];
            var v3 = c.Vertices[2];

            return (v1 + v2 + v3) / 3;
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
