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

            SubdividedIcosahedronCommand = new RelayCommand(CreateSubdividedIcosahedron);
        }

        public RelayCommand SubdividedIcosahedronCommand { get; private set; }

        private void CreateSubdividedIcosahedron()
        {
            ClearModels();
            var builder = new MeshBuilder();

            Vector3 ToVector3(System.Numerics.Vector3 vec) => 
                new Vector3(vec.X, vec.Y, vec.Z);

            var icosahedronCreator = new IcosahedronCreator(IcosahedronSubdivision);
            icosahedronCreator.Generate();
            foreach (var trianglePositions in icosahedronCreator.Faces)
            {
                builder.AddTriangle(ToVector3(trianglePositions[0]), ToVector3(trianglePositions[1]), ToVector3(trianglePositions[2]));
            }

            var mesh = builder.ToMeshGeometry3D();
            var random = new Random();
            mesh.Colors = new Color4Collection(mesh.TextureCoordinates.Select(x => random.NextColor().ToColor4()));
            Model = mesh;
            RaisePropertyChanged(nameof(Model));
        }

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