using System;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Windows.Media.Media3D;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using SharpDX.Direct2D1.Effects;
using Color = SharpDX.Color;
using MeshGeometry3D = HelixToolkit.Wpf.SharpDX.MeshGeometry3D;
using PerspectiveCamera = HelixToolkit.Wpf.SharpDX.PerspectiveCamera;
using Transform3D = SharpDX.Direct2D1.Effects.Transform3D;

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
            AmbientLightColor = Color.DimGray;
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

            GenerateFibonacciSphereCommand = new RelayCommand(() => CreateFibonacciSphereMesh());
            GenerateStereographicProjectionCommand = new RelayCommand(()=> CreateStereographicProjection());
            GenerateVoronoiMeshCommand = new RelayCommand(() => CreateVoronoiMesh());
        }

        public RelayCommand GenerateFibonacciSphereCommand { get; private set; }
        public RelayCommand GenerateStereographicProjectionCommand { get; private set; }        
        public RelayCommand GenerateVoronoiMeshCommand { get; private set; }
        //
        private void CreateVoronoiMesh()
        {
            ClearModels();
      
            var voronoi = Utils.VoronoiTriangles(
                Utils.FibonacciSphere(FibonacciSamples) 
                           .Select(p => new System.Numerics.Vector3(p.X, p.Y, p.Z)))
                           .ToList();
            
            var builder = new MeshBuilder();
            foreach (var cell in voronoi)
            {
                var polygonData = cell.Vertices
                    .Select(v => new Vector3((float)v.X,(float)v.Y,(float)v.Z)).ToList();
                builder.AddPolygon(polygonData);
            }
            var mesh = builder.ToMeshGeometry3D();
            mesh.Colors = new Color4Collection(mesh.TextureCoordinates.Select(x => x.ToColor4()));
            Model = mesh;
            RaisePropertyChanged(nameof(Model));
        }

        private void CreateStereographicProjection()
        {
            ClearModels();

            var ptPos = new Vector3Collection();
            var ptIdx = new IntCollection();

            var projectionPoints = Utils.FibonacciSphere(FibonacciSamples)
                .Select(p => Utils.StereographicProjection(p.X,p.Y,p.Z))
                .Select(p => new Vector3(p.U, p.V, 0.0f));
            foreach (var p in projectionPoints)
            {
                ptIdx.Add(ptPos.Count);
                ptPos.Add(p);
            }

            Points.Positions = ptPos;
            Points.Indices = ptIdx;
        }

        private void CreateFibonacciSphereMesh()
        {
            ClearModels();

            var ptPos = new Vector3Collection();
            var ptIdx = new IntCollection();

            var fibonacciPoints = Utils.FibonacciSphere(FibonacciSamples).Select(p => new SharpDX.Vector3(p.X, p.Y, p.Z));
            foreach (var p in fibonacciPoints)
            {
                ptIdx.Add(ptPos.Count);
                ptPos.Add(p);
            }

            Points.Positions = ptPos;
            Points.Indices = ptIdx;
        }

        public int FibonacciSamples { get; set; }

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

        public EffectsManager EffectsManager { get; private set; }
        public Vector3D DirectionalLightDirection { get; private set; }
        public Color DirectionalLightColor { get; private set; }
        public Color AmbientLightColor { get; private set; }

        public Vector3D UpDirection { set; get; } = new Vector3D(0, 1, 0);
        public Stream BackgroundTexture { get; }
    }
}