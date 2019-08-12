using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;

namespace Adventure.World
{
    public class World
    {
        private readonly IReadOnlyList<IGeometry> _polygons;

        //TODO: use coordinate translation to make the coordinates geographic (lon,lat) and not cartesian (default)
        public World(int samples = 10000)
        {
            var voronoiMesh = Utils.VoronoiMesh(
                Utils.FibonacciSphere(samples, false)
                    .Select(v => Utils.StereographicProjection(v.X,v.Y,v.Z))
                    .Select(v => new Vertex2(v.U, v.V)).ToList());


        }
    }
}
