using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Adventure.World
{
    public class IcosahedronCreator
    {
        private static readonly float GoldenRatio = (float) ((1 + Math.Sqrt(5)) / 2);
        private readonly int _recursionLevel;
        private readonly float _radius;
        private List<Vector3[]> _faces;

        public IcosahedronCreator(int recursionLevel, float radius = 1.0f)
        {
            _recursionLevel = recursionLevel;
            _radius = radius;
        }

        public void Generate() => 
            _faces = SubdivideIcosahedron(_recursionLevel, BuildInitialIcosahedron());

        public IEnumerable<Vector3[]> Faces => _faces ?? Enumerable.Empty<Vector3[]>();

        private List<Vector3[]> SubdivideIcosahedron(int recursionLevel, List<Vector3[]> icosohedronData)
        {
            for (int i = 0; i < recursionLevel; i++)
            {
                var faces2 = new List<Vector3[]>();
                foreach (var tri in icosohedronData)
                {
                    // replace triangle by 4 triangles
                    var vec1 = GetMiddle(tri[0], tri[1]);
                    var vec2 = GetMiddle(tri[1], tri[2]);
                    var vec3 = GetMiddle(tri[2], tri[0]);

                    faces2.Add(new []{tri[0], vec1, vec3});
                    faces2.Add(new []{tri[1], vec2, vec1});
                    faces2.Add(new []{tri[2], vec3, vec2});
                    faces2.Add(new []{vec1, vec2, vec3});
                }
                icosohedronData = faces2;
            }
            return icosohedronData;
        }

        private Vector3 GetMiddle(Vector3 v1, Vector3 v2)
        {
            var temp = ((v2 - v1) * 0.5f) + v1;
            temp = Vector3.Normalize(temp);

            return temp * (float)(Math.Sqrt(GoldenRatio * GoldenRatio + 1) * _radius);
        }

        private List<Vector3[]> BuildInitialIcosahedron()
        {
            var faceList = new List<Vector3[]>();

            //Define the points needed to build a icosahedron, stolen from article
            var vertices = new Vector3 [12];
            vertices[0] = new Vector3(-_radius, GoldenRatio * _radius, 0);
            vertices[1] = new Vector3(_radius, GoldenRatio * _radius, 0);
            vertices[2] = new Vector3(-_radius, -GoldenRatio * _radius, 0);
            vertices[3] = new Vector3(_radius, -GoldenRatio * _radius, 0);

            vertices[4] = new Vector3(0, -_radius, GoldenRatio * _radius);
            vertices[5] = new Vector3(0, _radius, GoldenRatio * _radius);
            vertices[6] = new Vector3(0, -_radius, -GoldenRatio * _radius);
            vertices[7] = new Vector3(0, _radius, -GoldenRatio * _radius);

            vertices[8] = new Vector3(GoldenRatio * _radius, 0, -_radius);
            vertices[9] = new Vector3(GoldenRatio * _radius, 0, _radius);
            vertices[10] = new Vector3(-GoldenRatio * _radius, 0, -_radius);
            vertices[11] = new Vector3(-GoldenRatio * _radius, 0, _radius);

            void AddFace(List<Vector3[]> list, Vector3 v1, Vector3 v2, Vector3 v3) => 
                list.Add(new [] {v1, v2, v3 });

            // 5 faces around point 0
            AddFace(faceList,vertices[0], vertices[11], vertices[5]);
            AddFace(faceList,vertices[0], vertices[5], vertices[1]);
            AddFace(faceList,vertices[0], vertices[1], vertices[7]);
            AddFace(faceList,vertices[0], vertices[7], vertices[10]);
            AddFace(faceList,vertices[0], vertices[10], vertices[11]);

            // 5 adjacent faces
            AddFace(faceList,vertices[1], vertices[5], vertices[9]);
            AddFace(faceList,vertices[5], vertices[11], vertices[4]);
            AddFace(faceList,vertices[11], vertices[10], vertices[2]);
            AddFace(faceList,vertices[10], vertices[7], vertices[6]);
            AddFace(faceList,vertices[7], vertices[1], vertices[8]);

            // 5 faces around point 3
            AddFace(faceList,vertices[3], vertices[9], vertices[4]);
            AddFace(faceList,vertices[3], vertices[4], vertices[2]);
            AddFace(faceList,vertices[3], vertices[2], vertices[6]);
            AddFace(faceList,vertices[3], vertices[6], vertices[8]);
            AddFace(faceList,vertices[3], vertices[8], vertices[9]);

            // 5 adjacent faces
            AddFace(faceList,vertices[4], vertices[9], vertices[5]);
            AddFace(faceList,vertices[2], vertices[4], vertices[11]);
            AddFace(faceList,vertices[6], vertices[2], vertices[10]);
            AddFace(faceList,vertices[8], vertices[6], vertices[7]);
            AddFace(faceList,vertices[9], vertices[8], vertices[1]);

            return faceList;
        }
    }
}
