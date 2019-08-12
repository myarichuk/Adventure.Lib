using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Media3D;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using Color = SharpDX.Color;
using Material = HelixToolkit.Wpf.SharpDX.Material;
using MeshGeometry3D = HelixToolkit.Wpf.SharpDX.MeshGeometry3D;
using PerspectiveCamera = HelixToolkit.Wpf.SharpDX.PerspectiveCamera;

namespace Adventure.World.Visualizer.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            FibonacciSamples = 1000;
            //GenerateFibonacciSphereCommand = new RelayCommand(() => CreateFibonacciSphereMesh(),() => true,true);
         EffectsManager = new DefaultEffectsManager();
            // titles
            // camera setup
            Camera = new PerspectiveCamera { 
                Position = new Point3D(3,3,5),
                LookDirection = new Vector3D(-3,-3,-5), 
                UpDirection = new Vector3D(0, 1, 0),
                FarPlaneDistance = 5000000
            };

            // setup lighting            
            AmbientLightColor = Color.White;
            DirectionalLightColor = Color.White;
            DirectionalLightDirection = new Vector3D(-2, -5, -2);

            //// scene model3d
            //var b1 = new MeshBuilder();            
            //b1.AddSphere(new Vector3(0, 0, 0), 0.5);
            //b1.AddBox(new Vector3(0, 0, 0), 1, 0.5, 2, BoxFaces.All);
           
            //var meshGeometry = b1.ToMeshGeometry3D();
            //meshGeometry.Colors = new Color4Collection(meshGeometry.TextureCoordinates.Select(x => x.ToColor4()));
            //Model = meshGeometry;

            //// lines model3d
            var e1 = new LineBuilder();
            e1.AddBox(new Vector3(0, 0, 0), 1, 0.5, 2);

            VertexColorMaterial = new VertColorMaterial();
            RedMaterial = PhongMaterials.BlackPlastic;
        
            //var diffColor = this.RedMaterial.DiffuseColor;
            //diffColor.Alpha = 0.5f;
            //this.RedMaterial.DiffuseColor = diffColor;   

            Points = new PointGeometry3D();

            BackgroundTexture =
                BitmapExtensions.CreateLinearGradientBitmapStream(EffectsManager, 128, 128, Direct2DImageFormat.Bmp,
                new Vector2(0, 0), new Vector2(0, 128), new SharpDX.Direct2D1.GradientStop[]
                {
                    new SharpDX.Direct2D1.GradientStop(){ Color = Color.DarkGray, Position = 0f },
                    new SharpDX.Direct2D1.GradientStop(){ Color = Color.Black.ToColor4(), Position = 1.0f }
                });

            GenerateFibonacciSphereCommand = new RelayCommand(CreateFibonacciSphereMesh);
            GenerateStereographicProjectionCommand = new RelayCommand(CreateStereographicProjection);
            DelaunayMeshCommand = new RelayCommand(CreateDelaunayMesh);
            VoronoiMeshCommand = new RelayCommand(CreateVoronoiMesh);
            CentroidMeshCommand = new RelayCommand(CreateCentroidMesh);
            SubdividedIcosahedronCommand = new RelayCommand(CreateSubdividedIcosahedron);
        }

        public RelayCommand GenerateFibonacciSphereCommand { get; private set; }
        public RelayCommand GenerateStereographicProjectionCommand { get; private set; }        
        public RelayCommand DelaunayMeshCommand { get; private set; }
        public RelayCommand VoronoiMeshCommand { get; private set; }
        public RelayCommand CentroidMeshCommand { get; private set; }
        public RelayCommand SubdividedIcosahedronCommand { get; private set; }

        private void CreateSubdividedIcosahedron()
        {
            ClearModels();
            var builder = new MeshBuilder();

            var icosahedronData = BuildInitialIcosahedron();

            icosahedronData = SubdivideIcosahedron(IcosahedronSubdivision, icosahedronData);
            foreach (var trianglePositions in icosahedronData)
                builder.AddTriangle(trianglePositions[0], trianglePositions[1], trianglePositions[2]);
            
            var mesh = builder.ToMeshGeometry3D();
            var random = new Random();
            mesh.Colors = new Color4Collection(mesh.TextureCoordinates.Select(x => random.NextColor().ToColor4()));
            Model = mesh;
            RaisePropertyChanged(nameof(Model));
        }

        private Vector3 GetMiddle(Vector3 v1, Vector3 v2, float size = 1.0f)
        {
            Vector3 Scale(Vector3 v, float d)
            {
                return new Vector3(v.X * d, v.Y * d, v.Z * d);
            }

            //The golden ratio
            var t = (1 + Math.Sqrt(5)) / 2;
            //Calculate the middle
            var temporaryVector = Scale(v2 - v1, 0.5f) + v1;

            //Offset point
            temporaryVector.Normalize();
            temporaryVector = Scale(temporaryVector, (float)(Math.Sqrt(t * t + 1) * size));

            return temporaryVector;
        }

        List<Vector3[]> SubdivideIcosahedron(int recursionLevel, List<Vector3[]> icosohedronData)
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

        List<Vector3[]> BuildInitialIcosahedron(float size = 1.0f)
        {
            var faceList = new List<Vector3[]>();

            //The golden ratio
            var t = (float) ((1 +Math.Sqrt(5))/ 2);

            //Define the points needed to build a icosahedron, stolen from article
            var vertices = new Vector3 [12];
            vertices[0] = new Vector3(-size, t*size, 0);
            vertices[1] = new Vector3(size, t*size, 0);
            vertices[2] = new Vector3(-size, -t*size, 0);
            vertices[3] = new Vector3(size, -t*size, 0);

            vertices[4] = new Vector3(0, -size, t*size);
            vertices[5] = new Vector3(0, size, t*size);
            vertices[6] = new Vector3(0, -size, -t*size);
            vertices[7] = new Vector3(0, size, -t*size);

            vertices[8] = new Vector3(t*size, 0, -size);
            vertices[9] = new Vector3(t*size, 0, size);
            vertices[10] = new Vector3(-t*size, 0, -size);
            vertices[11] = new Vector3(-t*size, 0, size);

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

        private void CreateDelaunayMesh()
        {
            ClearModels();
      
            var voronoiVertices = Utils.VoronoiTriangles(
                Utils.FibonacciSphere(FibonacciSamples, false)
                .Select(v => Utils.StereographicProjection(v.X,v.Y,v.Z))
                .Select(v => new Vertex2(v.U, v.V))).ToList();
            
            var builder = new MeshBuilder();
            
            foreach (var cell in voronoiVertices)
            {
                var polygonData = cell.Vertices
                    .Select(v => Utils.InverseStereographicProjection(v.X, v.Y))
                    .Select(v => new Vector3((float)v.X,(float)v.Y,(float)v.Z)).ToList();
                builder.AddPolygon(polygonData);
            }
            var mesh = builder.ToMeshGeometry3D();
            var random = new Random();
            mesh.Colors = new Color4Collection(mesh.TextureCoordinates.Select(x => random.NextColor().ToColor4()));
            Model = mesh;
            RaisePropertyChanged(nameof(Model));
        }

        
        private void CreateCentroidMesh()
        {
            ClearModels();
            var voronoiMesh = Utils.VoronoiMesh(
                Utils.FibonacciSphere(FibonacciSamples, false)
                    .Select(v => Utils.StereographicProjection(v.X,v.Y,v.Z))
                    .Select(v => new Vertex2(v.U, v.V)).ToList());
            
            var meshBuilder = new MeshBuilder();
            
            var voronoiEdges = new List<(Vertex2 from, Vertex2 to)>();
            foreach (var edge in voronoiMesh.Edges)
            {
                var from = edge.Source.Centroid;
                var to = edge.Target.Centroid;
                voronoiEdges.Add((from, to));
            }
            foreach (var cell in voronoiMesh.Vertices)
            {                
                for (int i = 0; i < 3; i++)
                {
                    if (cell.Adjacency[i] == null)
                    {
                        var from = cell.Centroid;
                        var t = cell.Vertices.Where((_, j) => j != i).ToArray();
                        var factor = 100 * Utils.IsLeft(t[0], t[1], from) * Utils.IsLeft(t[0], t[1], Utils.Center(cell));
                        var dir = new Vertex2(0.5 * (t[0].Position[0] + t[1].Position[0]), 0.5 * (t[0].Position[1] + t[1].Position[1])) - from;
                        var to = from + (dir * factor);
                        voronoiEdges.Add((from, to));
                        
                    }
                }
              
            }

            var polygons = Utils.Polygonize(voronoiEdges).ToList();
            var colors = new List<Color4>();
            var random = new Random();
            foreach (var polygon in polygons)
            {
                var builder = new MeshBuilder();
                var polygonData = polygon
                    .Select(v => Utils.InverseStereographicProjection(v.X, v.Y))
                    .Select(v => new Vector3((float) v.X, (float) v.Y, (float) v.Z))
                    .ToList();

                var normal = new Vector3(polygonData[0].X, polygonData[0].Y, polygonData[0].Z);
                for (var i=1; i<polygonData.Count; i++)
                {
                    normal = Vector3.Cross(normal, polygonData[i]);
                }

                builder.AddTriangleFan(polygonData,
                    Enumerable.Repeat(normal, polygonData.Count).ToList(),
                    polygonData.Select(v => new Vector2((float) (0.5),(float) (0.5))).ToList());
                
                meshBuilder.Append(builder);
                colors.AddRange(Enumerable.Repeat(random.NextColor().ToColor4(),polygonData.Count));
           
            }
            var mesh = meshBuilder.ToMesh();
            mesh.Colors = new Color4Collection(colors);
            Model = mesh;
            RaisePropertyChanged(nameof(Model));
        }

        private void CreateVoronoiMesh()
        {
            ClearModels();
      
            var voronoiMesh = Utils.VoronoiMesh(
                Utils.FibonacciSphere(FibonacciSamples, false)
                    .Select(v => Utils.StereographicProjection(v.X,v.Y,v.Z))
                    .Select(v => new Vertex2(v.U, v.V)).ToList());
            
            var meshBuilder = new MeshBuilder();
            
            var voronoiEdges = new List<(Vertex2 from, Vertex2 to)>();
            foreach (var edge in voronoiMesh.Edges)
            {
                var from = edge.Source.Circumcenter;
                var to = edge.Target.Circumcenter;
                voronoiEdges.Add((from, to));
            }
                
            foreach (var cell in voronoiMesh.Vertices)
            {                
                for (int i = 0; i < 3; i++)
                {
                    if (cell.Adjacency[i] == null)
                    {
                        var from = cell.Circumcenter;
                        var t = cell.Vertices.Where((_, j) => j != i).ToArray();
                        var factor = 100 * Utils.IsLeft(t[0], t[1], from) * Utils.IsLeft(t[0], t[1], Utils.Center(cell));
                        var dir = new Vertex2(0.5 * (t[0].Position[0] + t[1].Position[0]), 0.5 * (t[0].Position[1] + t[1].Position[1])) - from;
                        var to = from + (dir * factor);
                        voronoiEdges.Add((from, to));
                        
                    }
                }
              
            }

            var polygons = Utils.Polygonize(voronoiEdges).ToList();
            var colors = new List<Color4>();
            var random = new Random();
            foreach (var polygon in polygons)
            {
                var builder = new MeshBuilder();
                var polygonData = polygon
                    .Select(v => Utils.InverseStereographicProjection(v.X, v.Y))
                    .Select(v => new Vector3((float) v.X, (float) v.Y, (float) v.Z))
                    .ToList();

                var normal = new Vector3(polygonData[0].X, polygonData[0].Y, polygonData[0].Z);
                for (var i=1; i<polygonData.Count; i++)
                {
                    normal = Vector3.Cross(normal, polygonData[i]);
                }

                builder.AddTriangleFan(polygonData,
                    Enumerable.Repeat(normal, polygonData.Count).ToList(),
                    polygonData.Select(v => new Vector2((float) (0.5),(float) (0.5))).ToList());
                
                meshBuilder.Append(builder);
                colors.AddRange(Enumerable.Repeat(random.NextColor().ToColor4(),polygonData.Count));
           
            }
            var mesh = meshBuilder.ToMesh();
            mesh.Colors = new Color4Collection(colors);
            Model = mesh;
            RaisePropertyChanged(nameof(Model));
        }

        private void CreateStereographicProjection()
        {
            ClearModels();

            var ptPos = new Vector3Collection();
            var ptIdx = new IntCollection();

            var projectionPoints = GetStereographicProjectionPoints().Select(p => new Vector3((float)p.X,(float)p.Y,(float)p.Z));

            foreach (var p in projectionPoints)
            {
                ptIdx.Add(ptPos.Count);
                ptPos.Add(p);
            }

            Points.Positions = ptPos;
            Points.Indices = ptIdx;
        }

  

        private IEnumerable<Vertex3> GetStereographicProjectionPoints() =>
            Utils.FibonacciSphere(FibonacciSamples)
                .Select(p => Utils.StereographicProjection(p.X, p.Y, p.Z))
                .Select(p => new Vertex3(p.U, p.V, 0.0f));

        private void CreateFibonacciSphereMesh()
        {
            ClearModels();

            var ptPos = new Vector3Collection();
            var ptIdx = new IntCollection();

            var fibonacciPoints = Utils.FibonacciSphere(FibonacciSamples).Select(p => new SharpDX.Vector3((float)p.X, (float)p.Y, (float)p.Z));
            foreach (var p in fibonacciPoints)
            {
                ptIdx.Add(ptPos.Count);
                ptPos.Add(p);
            }

            Points.Positions = ptPos;
            Points.Indices = ptIdx;
        }

        public int FibonacciSamples { get; set; }
        public int IcosahedronSubdivision { get; set; }

        private void ClearModels()
        {
            Points.Positions = new Vector3Collection();
            Points.Indices = new IntCollection();

            Model = new MeshGeometry3D();
            RaisePropertyChanged(nameof(Model));
        }

        public PerspectiveCamera Camera { get; private set; }

        public MeshGeometry3D Model { get; private set; }
        public PointGeometry3D Points { get; private set; }

        public PhongMaterial RedMaterial { get; private set; }
        public Material VertexColorMaterial { get; private set; }

        public EffectsManager EffectsManager { get; private set; }
        public Vector3D DirectionalLightDirection { get; private set; }
        public Color DirectionalLightColor { get; private set; }
        public Color AmbientLightColor { get; private set; }

        public Vector3D UpDirection { set; get; } = new Vector3D(0, 1, 0);
        public Stream BackgroundTexture { get; }
    }
}