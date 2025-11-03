using System.Diagnostics;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using Avril_FSD.ServerAssembly.Graphics.Cameras;
using Avril_FSD.ServerAssembly.Graphics.GameObjects;
using Avril_FSD.ServerAssembly.Graphics.Renderables;
using Avril_FSD.ServerAssembly.Graphics;

namespace Avril_FSD.ServerAssembly
{
    public sealed class Game_Instance : GameWindow
    {
        private bool done_once;
        private byte _coreId;
        private readonly string _title;
        private double _time;
        private readonly Color4 _backColor = new Color4(0.1f, 0.1f, 0.3f, 1.0f);
        private FirstPersonCamera _cameraFP;
        private ThirdPersonCamera _cameraTP;
        private Matrix4 _projectionMatrix;
        private float _fov = 45f;

        private KeyboardState _lastKeyboardState;
        private MouseState _lastMouseState;

        private GameObjectFactory _gameObjectFactory;
        private readonly List<AGameObject> _gameObjects = new List<AGameObject>();
        
        
        private ShaderProgram _texturedProgram;
        private ShaderProgram _solidProgram;
        
        private bool cameraSelector = false;
       
        public Game_Instance()
            : base(960, // initial width
                540, // initial height
                GraphicsMode.Default,
                "",  // initial title
                GameWindowFlags.FixedWindow,
                DisplayDevice.Default,
                4, // OpenGL major version
                5, // OpenGL minor version
                GraphicsContextFlags.ForwardCompatible)
        {
            _title += "dreamstatecoding.blogspot.com: OpenGL Version: " + GL.GetString(StringName.Version);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            CreateProjection();
        }

        protected override void OnLoad(EventArgs e)
        {
            System.Console.WriteLine("OnLoad");
            VSync = VSyncMode.Off;
            CreateProjection();
#if DEBUG
            _solidProgram = new ShaderProgram();
            _solidProgram.AddShader(ShaderType.VertexShader, "..\\..\\..\\APP_ServerAssembly\\graphics\\Shaders\\1Vert\\simplePipeVert.c");
            _solidProgram.AddShader(ShaderType.FragmentShader, "..\\..\\..\\APP_ServerAssembly\\graphics\\Shaders\\5Frag\\simplePipeFrag.c");
            _solidProgram.Link();

            _texturedProgram = new ShaderProgram();
            _texturedProgram.AddShader(ShaderType.VertexShader, "..\\..\\..\\APP_ServerAssembly\\graphics\\Shaders\\1Vert\\simplePipeTexVert.c");
            _texturedProgram.AddShader(ShaderType.FragmentShader, "..\\..\\..\\APP_ServerAssembly\\graphics\\Shaders\\5Frag\\simplePipeTexFrag.c");
            _texturedProgram.Link();

            var models = new Dictionary<string, ARenderable>();
            models.Add("Player", new MipMapGeneratedRenderObject(RenderObjectFactory.CreateTexturedCube6(1, 1, 1), _texturedProgram.Id, "..\\..\\..\\APP_ServerAssembly\\graphics\\Textures\\gameover.png", 8));
            models.Add("Wooden", new MipMapGeneratedRenderObject(new IcoSphereFactory().Create(3), _texturedProgram.Id, "..\\..\\..\\APP_ServerAssembly\\graphics\\Textures\\wooden.png", 8));
            models.Add("Golden", new MipMapGeneratedRenderObject(new IcoSphereFactory().Create(3), _texturedProgram.Id, "..\\..\\..\\APP_ServerAssembly\\graphics\\Textures\\golden.bmp", 8));
            models.Add("Asteroid", new MipMapGeneratedRenderObject(new IcoSphereFactory().Create(3), _texturedProgram.Id, "..\\..\\..\\APP_ServerAssembly\\graphics\\Textures\\moonmap1k.jpg", 8));
            models.Add("Spacecraft", new MipMapGeneratedRenderObject(RenderObjectFactory.CreateTexturedCube6(1, 1, 1), _texturedProgram.Id, "..\\..\\..\\APP_ServerAssembly\\graphics\\Textures\\spacecraft.png", 8));
            models.Add("Gameover", new MipMapGeneratedRenderObject(RenderObjectFactory.CreateTexturedCube6(1, 1, 1), _texturedProgram.Id, "..\\..\\..\\APP_ServerAssembly\\graphics\\Textures\\gameover.png", 8));
            models.Add("Bullet", new MipMapGeneratedRenderObject(new IcoSphereFactory().Create(3), _texturedProgram.Id, "..\\..\\..\\APP_ServerAssembly\\graphics\\Textures\\dotted.png", 8));
#else
            _solidProgram = new ShaderProgram();
            _solidProgram.AddShader(ShaderType.VertexShader, "..\\..\\..\\APP_ServerAssembly\\graphics\\Shaders\\1Vert\\simplePipeVert.c");
            _solidProgram.AddShader(ShaderType.FragmentShader, "..\\..\\..\\APP_ServerAssembly\\graphics\\Shaders\\5Frag\\simplePipeFrag.c");
            _solidProgram.Link();

            _texturedProgram = new ShaderProgram();
            _texturedProgram.AddShader(ShaderType.VertexShader, "..\\..\\..\\APP_ServerAssembly\\graphics\\Shaders\\1Vert\\simplePipeTexVert.c");
            _texturedProgram.AddShader(ShaderType.FragmentShader, "..\\..\\..\\APP_ServerAssembly\\graphics\\Shaders\\5Frag\\simplePipeTexFrag.c");
            _texturedProgram.Link();

            var models = new Dictionary<string, ARenderable>();
            models.Add("Player", new MipMapGeneratedRenderObject(RenderObjectFactory.CreateTexturedCube6(1, 1, 1), _texturedProgram.Id, "..\\..\\..\\APP_ServerAssembly\\graphics\\Textures\\gameover.png", 8));
            models.Add("Wooden", new MipMapGeneratedRenderObject(new IcoSphereFactory().Create(3), _texturedProgram.Id, "..\\..\\..\\APP_ServerAssembly\\graphics\\Textures\\wooden.png", 8));
            models.Add("Golden", new MipMapGeneratedRenderObject(new IcoSphereFactory().Create(3), _texturedProgram.Id, "..\\..\\..\\APP_ServerAssembly\\graphics\\Textures\\golden.bmp", 8));
            models.Add("Asteroid", new MipMapGeneratedRenderObject(new IcoSphereFactory().Create(3), _texturedProgram.Id, "..\\..\\..\\APP_ServerAssembly\\graphics\\Textures\\moonmap1k.jpg", 8));
            models.Add("Spacecraft", new MipMapGeneratedRenderObject(RenderObjectFactory.CreateTexturedCube6(1, 1, 1), _texturedProgram.Id, "..\\..\\..\\APP_ServerAssembly\\graphics\\Textures\\spacecraft.png", 8));
            models.Add("Gameover", new MipMapGeneratedRenderObject(RenderObjectFactory.CreateTexturedCube6(1, 1, 1), _texturedProgram.Id, "..\\..\\..\\APP_ServerAssembly\\graphics\\Textures\\gameover.png", 8));
            models.Add("Bullet", new MipMapGeneratedRenderObject(new IcoSphereFactory().Create(3), _texturedProgram.Id, "..\\..\\..\\APP_ServerAssembly\\graphics\\Textures\\dotted.png", 8));
#endif
            //models.Add("TestObject", new TexturedRenderObject(RenderObjectFactory.CreateTexturedCube(1, 1, 1), _texturedProgram.Id, "..\\..\\..\\graphics\Textures\asteroid texture one side.jpg"));
            //models.Add("TestObjectGen", new MipMapGeneratedRenderObject(RenderObjectFactory.CreateTexturedCube(1, 1, 1), _texturedProgram.Id, "..\\..\\..\\graphics\Textures\asteroid texture one side.jpg", 8));
            //models.Add("TestObjectPreGen", new MipMapManualRenderObject(RenderObjectFactory.CreateTexturedCube(1, 1, 1), _texturedProgram.Id, "..\\..\\..\\graphics\Textures\asteroid texture one side mipmap levels 0 to 8.bmp", 9));

            _gameObjectFactory = new GameObjectFactory(models);

            _gameObjectFactory.Create_PlayerOnClient();
            _gameObjects.Add(_gameObjectFactory.Get_player());

            _gameObjects.Add(_gameObjectFactory.CreateSphericalAsteroid("Wooden", new Vector3(10f, 0f, 0f), new Vector3(1f)));
            _gameObjects.Add(_gameObjectFactory.CreateSphericalAsteroid("Wooden", new Vector3(-10f, 0f, 0f), new Vector3(1f)));
            _gameObjects.Add(_gameObjectFactory.CreateSphericalAsteroid("Golden", new Vector3(0f, 10f, 0f), new Vector3(1f)));
            _gameObjects.Add(_gameObjectFactory.CreateSphericalAsteroid("Golden", new Vector3(0f, -10f, 0f), new Vector3(1f)));
            _gameObjects.Add(_gameObjectFactory.CreateSphericalAsteroid("Asteroid", new Vector3(0f, 0f, 10f), new Vector3(1f)));
            _gameObjects.Add(_gameObjectFactory.CreateSphericalAsteroid("Asteroid", new Vector3(0f, 0f, -10f), new Vector3(1f)));

            //_camera = new StaticCamera();

            _gameObjectFactory.Get_player().Create_Cameras();

            CursorVisible = false;

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 3);
            GL.PointSize(3);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);            
            Closed += OnClosed;

            Avril_FSD.ServerAssembly.Framework_Server obj = Avril_FSD.ServerAssembly.Program.Get_framework_Server();
            obj.Get_server().Get_execute().Initialise_Threads(obj);

            System.Console.WriteLine("OnLoad .. done");
        }

        private void OnClosed(object sender, EventArgs eventArgs)
        {
            Exit();
        }

        public override void Exit()
        {
            System.Console.WriteLine("Exit called");
            _gameObjectFactory.Dispose();
            _solidProgram.Dispose();
            _texturedProgram.Dispose();
            base.Exit();
        }

        private void CreateProjection()
        {

            var aspectRatio = (float)Width / Height;
            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
                _fov * ((float)Math.PI / 180f), // field of view angle, in radians
                aspectRatio,                // current window aspect ratio
                0.1f,                       // near plane
                4000f);                     // far plane
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            _time += e.Time;
            foreach (var item in _gameObjects)
            {
                item.Update(_time, e.Time);
            }
            if(Avril_FSD.ServerAssembly.Program.Get_framework_Server().Get_server().Get_execute().Get_execute_Control().Get_flag_isInitialised_ServerShell())
            {
                HandleKeyboard(e.Time);
                HandleMouse();
                switch (cameraSelector)
                {
                    case false:
                        Get_gameObjectFactory().Get_player().Get_CameraFP().Update(_time, e.Time);
                        break;

                    case true:
                        Get_gameObjectFactory().Get_player().Get_CameraTP().Update(_time, e.Time);
                        break;
                }
            }
        }
        private void HandleMouse()
        {
            Console.WriteLine("TESTBENCH => HandleMouse");
            MouseState mouseState = Mouse.GetCursorState();
        }
        private void HandleKeyboard(double dt)
        {
            var keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Key.Escape))
            {
                Exit();
            }
            if (keyState.IsKeyDown(Key.W))
            {
                
            }
            if (keyState.IsKeyDown(Key.S))
            {
                
            }

            if (keyState.IsKeyDown(Key.A))
            {
                
            }
            if (keyState.IsKeyDown(Key.D))
            {
                
            }
            _lastKeyboardState = keyState;
            /*
                    Vector3 fowards = new Vector3(0);
                    Vector3 backwards = new Vector3(0);
                    Vector3 left = new Vector3(0);
                    Vector3 right = new Vector3(0);
                    if (keyState.IsKeyDown(Key.W))
                    {
                        Avril_FSD.ServerAssembly.Framework.GetGameServer().GetData().GetData_Control().SetIsPraiseEvent(2, true);
                        fowards = _gameObjectFactory.Get_player().Get_CameraFPOP().Get_fowards();
                    }
                    if (keyState.IsKeyDown(Key.S))
                    {
                        Avril_FSD.ServerAssembly.Framework.GetGameServer().GetData().GetData_Control().SetIsPraiseEvent(3, true);
                        backwards = - _gameObjectFactory.Get_player().Get_CameraFPOP().Get_fowards();
                    }
                    if (keyState.IsKeyDown(Key.A))
                    {
                        Avril_FSD.ServerAssembly.Framework.GetGameServer().GetData().GetData_Control().SetIsPraiseEvent(4, true);
                        right = _gameObjectFactory.Get_player().Get_CameraFPOP().Get_right();
                    }
                    if (keyState.IsKeyDown(Key.D))
                    {
                        Avril_FSD.ServerAssembly.Framework.GetGameServer().GetData().GetData_Control().SetIsPraiseEvent(5, true);
                        left = - _gameObjectFactory.Get_player().Get_CameraFPOP().Get_right();
                    }
                    if (Avril_FSD.ServerAssembly.Framework.GetGameServer().GetData().GetData_Control().GetFlag_IsPraiseEvent(2) == true
                        || Avril_FSD.ServerAssembly.Framework.GetGameServer().GetData().GetData_Control().GetFlag_IsPraiseEvent(3) == true
                        || Avril_FSD.ServerAssembly.Framework.GetGameServer().GetData().GetData_Control().GetFlag_IsPraiseEvent(4) == true
                        || Avril_FSD.ServerAssembly.Framework.GetGameServer().GetData().GetData_Control().GetFlag_IsPraiseEvent(5) == true
                    )
                    {
                        var player = _gameObjectFactory.Get_player();
                        var camera = _gameObjectFactory.Get_player().Get_CameraFPOP();

                        _gameObjectFactory.Get_player().Set_direction(_gameObjectFactory.Get_player().Get_position() + new Vector3(fowards + backwards + right + left).Normalized());
                        _gameObjectFactory.Get_player().Set_newPosition(_gameObjectFactory.Get_player().Get_position() + (Vector3.Multiply(_gameObjectFactory.Get_player().Get_direction(), (float)(_gameObjectFactory.Get_player().Get_cameraSpeed() * dt))));
                        _gameObjectFactory.Get_player().Set_newPosition(_gameObjectFactory.Get_player().Get_position().Normalized() * 101f);
                        
                        player.Clamp_Rotations(camera.Calculate_Position_Rotations(_gameObjectFactory.Get_player().Get_position()));
                        
                        Quaternion quart = Quaternion.FromEulerAngles(player.Get_Rotation().X, player.Get_Rotation().Y, player.Get_Rotation().Z);
                        camera.Set_fowards(Vector3.Transform(camera.Get_fowards(), quart));
                        camera.Set_up(Vector3.Transform(camera.Get_up(), quart));
                        camera.Set_right(Vector3.Cross(camera.Get_fowards(), camera.Get_up()));

                        OpenTK.Input.Mouse.SetPosition((double)(Avril_FSD.ServerAssembly.Framework.GetGameServer().GetData().GetSettings().Get_ScreenSize_X() / 2), (double)(Avril_FSD.ServerAssembly.Framework.GetGameServer().GetData().GetSettings().Get_ScreenSize_Y() / 2));

                    }
                    for (int index = 2; index < 6; index++)
                    {
                        Avril_FSD.ServerAssembly.Framework.GetGameServer().GetData().GetData_Control().SetIsPraiseEvent(index, false);
                    }*/
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Title = $"{_title}: FPS:{1f / e.Time:0000.0}, obj:{_gameObjects.Count}";
            GL.ClearColor(Color.Black);// _backColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            int lastProgram = -1;
            foreach (var obj in _gameObjects)
            {
                var program = obj.Model.Program;
                if (lastProgram != program)
                    GL.UniformMatrix4(20, false, ref _projectionMatrix);
                lastProgram = obj.Model.Program;
                switch (cameraSelector)
                {
                    case false:
                        obj.Render(Get_gameObjectFactory().Get_player().Get_CameraFP());
                        break;

                    case true:
                        obj.Render(Get_gameObjectFactory().Get_player().Get_CameraTP());
                        break;
                }
                

            }
            SwapBuffers();
        }
        public byte Get_coreId()
        {
            return _coreId;
        }
        public bool Get_cameraSelector()
        {
            return cameraSelector;
        }
        public GameObjectFactory Get_gameObjectFactory()
        {
            return _gameObjectFactory;
        }
        public byte Set_coreId(byte value)
        {
            return _coreId = value;
        }
    }
}
