using System.Diagnostics;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using Avril_FSD.ClientAssembly.Graphics.Cameras;
using Avril_FSD.ClientAssembly.Graphics.GameObjects;
using Avril_FSD.ClientAssembly.Graphics.Renderables;
using Avril_FSD.ClientAssembly.Graphics;
using Valve.Sockets;

namespace Avril_FSD.ClientAssembly
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
        
        private Outputs.Output _gameData_Output;

        private ShaderProgram _texturedProgram;
        private ShaderProgram _solidProgram;
        
        private bool cameraSelector = false;
       
        public Game_Instance()
            : base(1920, // initial width
                1080, // initial height
                GraphicsMode.Default,
                "",  // initial title
                GameWindowFlags.Fullscreen,
                DisplayDevice.Default,
                4, // OpenGL major version
                5, // OpenGL minor version
                GraphicsContextFlags.ForwardCompatible)
        {
            _title += "dreamstatecoding.blogspot.com: OpenGL Version: " + GL.GetString(StringName.Version);
            _gameData_Output = new Outputs.Output();
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
            _solidProgram.AddShader(ShaderType.VertexShader, "..\\..\\..\\APP_ClientAssembly\\graphics\\Shaders\\1Vert\\simplePipeVert.c");
            _solidProgram.AddShader(ShaderType.FragmentShader, "..\\..\\..\\APP_ClientAssembly\\graphics\\Shaders\\5Frag\\simplePipeFrag.c");
            _solidProgram.Link();

            _texturedProgram = new ShaderProgram();
            _texturedProgram.AddShader(ShaderType.VertexShader, "..\\..\\..\\APP_ClientAssembly\\graphics\\Shaders\\1Vert\\simplePipeTexVert.c");
            _texturedProgram.AddShader(ShaderType.FragmentShader, "..\\..\\..\\APP_ClientAssembly\\graphics\\Shaders\\5Frag\\simplePipeTexFrag.c");
            _texturedProgram.Link();

            var models = new Dictionary<string, ARenderable>();
            models.Add("Player", new MipMapGeneratedRenderObject(RenderObjectFactory.CreateTexturedCube6(1, 1, 1), _texturedProgram.Id, "..\\..\\..\\APP_ClientAssembly\\graphics\\Textures\\gameover.png", 8));
            models.Add("Wooden", new MipMapGeneratedRenderObject(new IcoSphereFactory().Create(3), _texturedProgram.Id, "..\\..\\..\\APP_ClientAssembly\\graphics\\Textures\\wooden.png", 8));
            models.Add("Golden", new MipMapGeneratedRenderObject(new IcoSphereFactory().Create(3), _texturedProgram.Id, "..\\..\\..\\APP_ClientAssembly\\graphics\\Textures\\golden.bmp", 8));
            models.Add("Asteroid", new MipMapGeneratedRenderObject(new IcoSphereFactory().Create(3), _texturedProgram.Id, "..\\..\\..\\APP_ClientAssembly\\graphics\\Textures\\moonmap1k.jpg", 8));
            models.Add("Spacecraft", new MipMapGeneratedRenderObject(RenderObjectFactory.CreateTexturedCube6(1, 1, 1), _texturedProgram.Id, "..\\..\\..\\APP_ClientAssembly\\graphics\\Textures\\spacecraft.png", 8));
            models.Add("Gameover", new MipMapGeneratedRenderObject(RenderObjectFactory.CreateTexturedCube6(1, 1, 1), _texturedProgram.Id, "..\\..\\..\\APP_ClientAssembly\\graphics\\Textures\\gameover.png", 8));
            models.Add("Bullet", new MipMapGeneratedRenderObject(new IcoSphereFactory().Create(3), _texturedProgram.Id, "..\\..\\..\\APP_ClientAssembly\\graphics\\Textures\\dotted.png", 8));
#else
            _solidProgram = new ShaderProgram();
            _solidProgram.AddShader(ShaderType.VertexShader, "..\\..\\..\\APP_ClientAssembly\\graphics\\Shaders\\1Vert\\simplePipeVert.c");
            _solidProgram.AddShader(ShaderType.FragmentShader, "..\\..\\..\\APP_ClientAssembly\\graphics\\Shaders\\5Frag\\simplePipeFrag.c");
            _solidProgram.Link();

            _texturedProgram = new ShaderProgram();
            _texturedProgram.AddShader(ShaderType.VertexShader, "..\\..\\..\\APP_ClientAssembly\\graphics\\Shaders\\1Vert\\simplePipeTexVert.c");
            _texturedProgram.AddShader(ShaderType.FragmentShader, "..\\..\\..\\APP_ClientAssembly\\graphics\\Shaders\\5Frag\\simplePipeTexFrag.c");
            _texturedProgram.Link();

            var models = new Dictionary<string, ARenderable>();
            models.Add("Player", new MipMapGeneratedRenderObject(RenderObjectFactory.CreateTexturedCube6(1, 1, 1), _texturedProgram.Id, "..\\..\\..\\APP_ClientAssembly\\graphics\\Textures\\gameover.png", 8));
            models.Add("Wooden", new MipMapGeneratedRenderObject(new IcoSphereFactory().Create(3), _texturedProgram.Id, "..\\..\\..\\APP_ClientAssembly\\graphics\\Textures\\wooden.png", 8));
            models.Add("Golden", new MipMapGeneratedRenderObject(new IcoSphereFactory().Create(3), _texturedProgram.Id, "..\\..\\..\\APP_ClientAssembly\\graphics\\Textures\\golden.bmp", 8));
            models.Add("Asteroid", new MipMapGeneratedRenderObject(new IcoSphereFactory().Create(3), _texturedProgram.Id, "..\\..\\..\\APP_ClientAssembly\\graphics\\Textures\\moonmap1k.jpg", 8));
            models.Add("Spacecraft", new MipMapGeneratedRenderObject(RenderObjectFactory.CreateTexturedCube6(1, 1, 1), _texturedProgram.Id, "..\\..\\..\\APP_ClientAssembly\\graphics\\Textures\\spacecraft.png", 8));
            models.Add("Gameover", new MipMapGeneratedRenderObject(RenderObjectFactory.CreateTexturedCube6(1, 1, 1), _texturedProgram.Id, "..\\..\\..\\APP_ClientAssembly\\graphics\\Textures\\gameover.png", 8));
            models.Add("Bullet", new MipMapGeneratedRenderObject(new IcoSphereFactory().Create(3), _texturedProgram.Id, "..\\..\\..\\APP_ClientAssembly\\graphics\\Textures\\dotted.png", 8));
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

            Avril_FSD.ClientAssembly.Framework_Client obj = Avril_FSD.ClientAssembly.Program.Get_framework_Client();
            obj.Get_client().Get_execute().Initialise_Threads(obj);

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
            Avril_FSD.ClientAssembly.Framework_Client obj = Program.Get_framework_Client();
            if (obj.Get_client().Get_execute().Get_execute_Control().Get_flag_SystemInitialised() == false)
            {
                _time += e.Time;
                foreach (var item in _gameObjects)
                {
                    item.Update(_time, e.Time);
                }
                Avril_FSD.Library_For_WriteEnableForThreadsAt_CLIENTINPUTACTION.Write_Start(obj.Get_client().Get_execute().Get_program_WriteQue_C_IA(), 0);
                HandleKeyboard(obj, e.Time);
                HandleMouse(obj);
                Avril_FSD.Library_For_WriteEnableForThreadsAt_CLIENTINPUTACTION.Write_End(obj.Get_client().Get_execute().Get_program_WriteQue_C_IA(), 0);
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
        private void HandleMouse(Avril_FSD.ClientAssembly.Framework_Client obj)
        {
            //Console.WriteLine("TESTBENCH => HandleMouse");
            MouseState mouseState = Mouse.GetCursorState();
            var buffer = obj.Get_client().Get_data().Get_input_Instnace().Get_FRONT_inputDoubleBuffer(obj);
            //Console.WriteLine("Get_isPraiseActive() = " + obj.Get_client().Get_data().Get_data_Control().Get_isPraiseActive(1));
            if (obj.Get_client().Get_data().Get_data_Control().Get_isPraiseActive(1) == false)
            {
                //Console.WriteLine("TESTBENCH => Do PRAISE 1 start");
                obj.Get_client().Get_data().Get_data_Control().Set_isPraiseActive(1, true);
                buffer.Set_playerId(0);
                buffer.Set_praiseEventId(1);
                buffer.Get_input_Control().SelectSetIntputSubset(obj, buffer.Get_praiseEventId());
                var subset = (Avril_FSD.ClientAssembly.Praise_Files.Praise1_Input)buffer.Get_praiseInputBuffer_Subset();
                subset.Set_Mouse_X(mouseState.X);
                subset.Set_Mouse_Y(mouseState.Y);
                obj.Get_client().Get_data().Flip_InBufferToWrite();
                obj.Get_client().Get_data().Get_data_Control().Push_Stack_Client_InputAction(obj.Get_client().Get_data().Get_input_Instnace().Get_stack_Client_InputSend(), obj.Get_client().Get_data().Get_input_Instnace().Get_BACK_inputDoubleBuffer(obj));
                //Console.WriteLine("TESTBENCH => Do PRAISE 1 end");
            }
        }
        private void HandleKeyboard(Avril_FSD.ClientAssembly.Framework_Client obj, double dt)
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
        public Outputs.Output Get_gameData_Output()
        {
            return _gameData_Output;
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
