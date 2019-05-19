using System;
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

            // scene model3d
            var b1 = new MeshBuilder();            
            b1.AddSphere(new Vector3(0, 0, 0), 0.5);
            b1.AddBox(new Vector3(0, 0, 0), 1, 0.5, 2, BoxFaces.All);
           
            var meshGeometry = b1.ToMeshGeometry3D();
            meshGeometry.Colors = new Color4Collection(meshGeometry.TextureCoordinates.Select(x => x.ToColor4()));
            Model = meshGeometry;

            // lines model3d
            var e1 = new LineBuilder();
            e1.AddBox(new Vector3(0, 0, 0), 1, 0.5, 2);

            var textBuilder = new MeshBuilder();
            //textBuilder.ExtrudeText("HelixToolkit.SharpDX", "Arial", System.Windows.FontStyles.Normal, System.Windows.FontWeights.Bold,
            //    14, new Vector3(1, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 1));

       
            RedMaterial = PhongMaterials.Red;
        
            //var diffColor = this.RedMaterial.DiffuseColor;
            //diffColor.Alpha = 0.5f;
            //this.RedMaterial.DiffuseColor = diffColor;   

            Points = new PointGeometry3D();
           

            BackgroundTexture =
                BitmapExtensions.CreateLinearGradientBitmapStream(EffectsManager, 128, 128, Direct2DImageFormat.Bmp,
                new Vector2(0, 0), new Vector2(0, 128), new SharpDX.Direct2D1.GradientStop[]
                {
                    new SharpDX.Direct2D1.GradientStop(){ Color = Color.White.ToColor4(), Position = 0f },
                    new SharpDX.Direct2D1.GradientStop(){ Color = Color.DarkGray.ToColor4(), Position = 1f }
                });

            GenerateFibonacciSphereCommand = new RelayCommand(() => CreateFibonacciSphereMesh());
        }

        public RelayCommand GenerateFibonacciSphereCommand { get; private set; }

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