Execute Server
https://github.com/OpenFSD/Avril_Full_Stack_Development_Template/blob/master/APP_ServerAssembly/engine/Execute.cs
````
namespace Avril_FSD.ServerAssembly
{
    public class Execute
    {
        private Avril_FSD.ServerAssembly.Execute_Control _execute_Control;
        private Avril_FSD.ServerAssembly.Networking_Server _networking_Server;
        private IntPtr _program_ServerConcurrency;
        private Thread[] _threads = {null, null};
        
        public Execute(int numberOfCores) 
        {
            Set_execute_Control(null);
        }

        public void Initialise_Control(int numberOfCores, Global global)
        {
            Set_execute_Control(new Avril_FSD.ServerAssembly.Execute_Control(numberOfCores));
            while (Get_execute_Control() == null) { }
        }

        public void Initialise_Libraries(Avril_FSD.ServerAssembly.Framework_Server obj)
        {
            obj.Get_server().Get_execute().Set_program_ServerConcurrency(Avril_FSD.Library_For_Server_Concurrency.Initialise_Server_Concurrency());
            System.Console.WriteLine("created server concurrency.");//TESTBENCH
        }

        public void Initialise_NetworkingPipes(Avril_FSD.ServerAssembly.Framework_Server obj)
        {
            obj.Get_server().Get_execute().Set_networking_Server(new Avril_FSD.ServerAssembly.Networking_Server());
            obj.Get_server().Get_execute().Get_networking_Server().Initialise_networking_Server();
        }

        public void Initialise_Threads(Avril_FSD.ServerAssembly.Framework_Server obj)
        {
            byte threadIdCounter = 0;
            obj.Get_server().Get_execute().Set_thread(threadIdCounter, Thread.CurrentThread);
            
            threadIdCounter++;
            obj.Get_server().Get_execute().Set_thread(threadIdCounter, new Thread(() => obj.Get_server().Get_execute().Get_networking_Server().Thread_IO_Server(threadIdCounter)));
            obj.Get_server().Get_execute().Get_thread(threadIdCounter).Start();
            System.Console.WriteLine("starting = > Thread_IO_Server on core " + (threadIdCounter).ToString());//TESTBENCH
        }

        public void Create_And_Run_Graphics(Avril_FSD.ServerAssembly.Framework_Server obj)
        {
            System.Console.WriteLine("starting = > gameInstance");//TESTBENCH
            using (Avril_FSD.ServerAssembly.Game_Instance gameInstance = new Avril_FSD.ServerAssembly.Game_Instance())
            {
                gameInstance.Run(obj.Get_server().Get_data().Get_settings().Get_refreshRate());
            }
        }

        public Avril_FSD.ServerAssembly.Execute_Control Get_execute_Control()
        {
            return _execute_Control;
        }
        public Avril_FSD.ServerAssembly.Networking_Server Get_networking_Server()
        {
            return _networking_Server;
        }

        public Thread Get_thread(int index)
        {
            return _threads[index];
        }
        public IntPtr Get_program_ServerConcurrency()
        {
            return _program_ServerConcurrency;
        }

        private void Set_execute_Control(Avril_FSD.ServerAssembly.Execute_Control execute_Control)
        {
            _execute_Control = execute_Control;
        }
        public void Set_networking_Server(Avril_FSD.ServerAssembly.Networking_Server networking_Server)
        {
            _networking_Server = networking_Server;
        }
        private void Set_thread(byte index, Thread thread) 
        {
            _threads[index] = thread;
        }

        private void Set_program_ServerConcurrency(IntPtr handle)
        {
            _program_ServerConcurrency = handle;
        }
    }   
}
````

Execute Client 
https://github.com/OpenFSD/Avril_Full_Stack_Development_Template/blob/master/APP_ClientAssembly/engine/Execute.cs
````
namespace Avril_FSD.ClientAssembly
{
    public class Execute
    {
        private Avril_FSD.ClientAssembly.Execute_Control _execute_Control;
        private Avril_FSD.ClientAssembly.Networking_Client _networking_Client;
        private IntPtr programId_ConcurrentQue_C;
        private IntPtr programId_WriteQue_C_IA;
        private IntPtr programId_WriteQue_C_OR;

        private Thread[] _threads = {null, null, null, null, null, null};//number of app shell threads.

        public Execute(int numberOfCores) 
        {
            Set_execute_Control(null);
        }

        public void Initialise_Control(int numberOfCores, Global global)
        {
            Set_execute_Control(new Avril_FSD.ClientAssembly.Execute_Control(numberOfCores));
            while (Get_execute_Control() == null) { }
        }

        public void Initialise_NetworkingPipes(Avril_FSD.ClientAssembly.Framework_Client obj)
        {
            Set_networking_Client(new Avril_FSD.ClientAssembly.Networking_Client());
            Get_networking_Client().Initialise_networking_Client();
        }

        public void Initialise_Libraries()
        {
            programId_ConcurrentQue_C = Avril_FSD.Library_For_LaunchEnableForConcurrentThreadsAt_CLIENT.Initialise_LaunchEnableForConcurrentThreadsAt();
            System.Console.WriteLine("created Library_For_LaunchEnableForConcurrentThreadsAt_CLIENT");

            programId_WriteQue_C_IA = Avril_FSD.Library_For_WriteEnableForThreadsAt_CLIENTINPUTACTION.Initialise_WriteEnable();
            System.Console.WriteLine("created Library_For_WriteEnableForThreadsAt_CLIENTINPUTACTION");

            programId_WriteQue_C_OR = Avril_FSD.Library_For_WriteEnableForThreadsAt_CLIENTOUTPUTRECIEVE.Initialise_WriteEnable();
            System.Console.WriteLine("created Library_For_WriteEnableForThreadsAt_CLIENTOUTPUTRECIEVE");

        }
        public void Initialise_Threads(Avril_FSD.ClientAssembly.Framework_Client obj)
        {
            byte threadIdCounter = 0;
            obj.Get_client().Get_execute().Set_thread(threadIdCounter, Thread.CurrentThread);
            obj.Get_client().Get_execute().Get_execute_Control().Set_flag_ThreadInitialised(obj, threadIdCounter, false);
            System.Console.WriteLine("Thread Initalised => CurrentThread()" + (threadIdCounter).ToString());//TESTBENCH

            threadIdCounter++;
            obj.Get_client().Get_execute().Set_thread(threadIdCounter, new Thread(() => _networking_Client.Thread_IO_Client(threadIdCounter)));
            obj.Get_client().Get_execute().Get_thread(threadIdCounter).Start();
            System.Console.WriteLine("Thread Initalised => Thread_IO_Client on core " + (threadIdCounter).ToString());//TESTBENCH

            threadIdCounter++;
            while (threadIdCounter < obj.Get_client().Get_global().Get_numberOfCores())
            {
                obj.Get_client().Get_execute().Set_thread(threadIdCounter, new Thread(() => obj.Get_client().Get_algorithms().Get_concurrent((byte)(threadIdCounter - (byte)2)).Thread_Concurrent(threadIdCounter)));
                obj.Get_client().Get_execute().Get_thread(threadIdCounter).Start();
                System.Console.WriteLine("Thread Initalised => Thread_Concurrent on core " + (threadIdCounter).ToString());//TESTBENCH
                threadIdCounter++;
            }
        }

        public void Create_And_Run_Graphics(Avril_FSD.ClientAssembly.Framework_Client obj)
        {
            System.Console.WriteLine("starting = > gameInstance");//TESTBENCH
            using (Avril_FSD.ClientAssembly.Game_Instance gameInstance = new Avril_FSD.ClientAssembly.Game_Instance())
            {
                gameInstance.Run(obj.Get_client().Get_data().Get_settings().Get_refreshRate());
            }
        }

        public Avril_FSD.ClientAssembly.Execute_Control Get_execute_Control()
        {
            return _execute_Control;
        }
        public Avril_FSD.ClientAssembly.Networking_Client Get_networking_Client()
        {
            return _networking_Client;
        }


        public IntPtr Get_program_ConcurrentQue_C()
        {
            return programId_ConcurrentQue_C;

        }
        public IntPtr Get_program_WriteQue_C_IA()
        {
            return programId_WriteQue_C_IA;
        }
        public IntPtr Get_program_WriteQue_C_OR()
        {
            return programId_WriteQue_C_OR;
        }
        public Thread Get_thread(byte index)
        {
            return _threads[index];
        }

        private void Set_execute_Control(Avril_FSD.ClientAssembly.Execute_Control execute_Control)
        {
            _execute_Control = execute_Control;
        }
        private void Set_networking_Client(Avril_FSD.ClientAssembly.Networking_Client networking_Client)
        {
            _networking_Client = networking_Client;
        }
        private void Set_program_ConcurrentQue_C(IntPtr programID)
        {
            programId_ConcurrentQue_C = programID;
        }
        private void Set_programId_WriteQue_C_IA(IntPtr programId)
        {
            programId_WriteQue_C_IA = programId;
        }
        private void Set_programId_WriteQue_C_OR(IntPtr programId)
        {
            programId_WriteQue_C_OR = programId;
        }
        private void Set_thread(byte index, Thread thread) 
        {
            _threads[index] = thread;
        }
    }   
}
````

Loading Server Shell Framework
https://github.com/OpenFSD/Avril_Full_Stack_Development_Template/blob/master/APP_ServerAssembly/gameInstance/Game_Instance.cs
````
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
            Avril_FSD.ServerAssembly.Framework_Server obj = Avril_FSD.ServerAssembly.Program.Get_framework_Server();
            obj.Get_server().Get_execute().Get_execute_Control().Set_exitApplication(true);
            obj.Get_server().Get_execute().Get_networking_Server().DeInitialise_networking_Server();
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
            if(Avril_FSD.ServerAssembly.Program.Get_framework_Server().Get_server().Get_execute().Get_execute_Control().Get_exitApplication() == false)
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
            //Console.WriteLine("TESTBENCH => HandleMouse");
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
````

Loading Client Shell Framwork - Scanner For Mouse and Keyboard
https://github.com/OpenFSD/Avril_Full_Stack_Development_Template/blob/master/APP_ClientAssembly/gameInstance/Game_Instance.cs
````
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
            Avril_FSD.ClientAssembly.Framework_Client obj = Avril_FSD.ClientAssembly.Program.Get_framework_Client();
            obj.Get_client().Get_execute().Get_execute_Control().Set_exitApplication(true);
            obj.Get_client().Get_execute().Get_networking_Client().DeInitialise_networking_Server();
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
            if (obj.Get_client().Get_execute().Get_execute_Control().Get_exitApplication() == false)
            {
                Console.WriteLine("TESTBENCH => Get_exitApplication() = " + obj.Get_client().Get_execute().Get_execute_Control().Get_exitApplication());
                Avril_FSD.Library_For_WriteEnableForThreadsAt_CLIENTINPUTACTION.Write_Start(obj.Get_client().Get_execute().Get_program_WriteQue_C_IA(), 0);
                HandleKeyboard(obj, e.Time);
                HandleMouse(obj);
                Avril_FSD.Library_For_WriteEnableForThreadsAt_CLIENTINPUTACTION.Write_End(obj.Get_client().Get_execute().Get_program_WriteQue_C_IA(), 0);
                _time += e.Time;
                foreach (var item in _gameObjects)
                {
                    item.Update(_time, e.Time);
                }
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
                Console.WriteLine("TESTBENCH => Do PRAISE 1 start");
                obj.Get_client().Get_data().Get_data_Control().Set_isPraiseActive(1, true);
                buffer.Set_playerId(0);
                buffer.Set_praiseEventId(1);
                buffer.Get_input_Control().SelectSetIntputSubset(obj, buffer.Get_praiseEventId());
                var subset = (Avril_FSD.ClientAssembly.Praise_Files.Praise1_Input)buffer.Get_praiseInputBuffer_Subset();
                subset.Set_Mouse_X(mouseState.X);
                subset.Set_Mouse_Y(mouseState.Y);
                obj.Get_client().Get_data().Flip_InBufferToWrite();
                obj.Get_client().Get_data().Get_data_Control().Push_Stack_Client_InputAction(obj, obj.Get_client().Get_data().Get_input_Instnace().Get_stack_Client_InputSend(), obj.Get_client().Get_data().Get_input_Instnace().Get_BACK_inputDoubleBuffer(obj));
                Console.WriteLine("TESTBENCH => Do PRAISE 1 end");
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
````

STACK Client Input Action
line 9:	private List<Avril_FSD.ClientAssembly.Inputs.Input> _stack_Client_InputSend;
https://github.com/OpenFSD/Avril_Full_Stack_Development_Template/blob/master/APP_ClientAssembly/engine/Input_Instance.cs

NETWORKING Client Input Action Send - Client Shell Input/Output (IO) Thread
https://github.com/OpenFSD/Avril_Full_Stack_Development_Template/blob/master/APP_ClientAssembly/Networking_Client.cs
````
public void Thread_IO_Client(byte threadId)
        {
            Avril_FSD.ClientAssembly.Framework_Client obj = Avril_FSD.ClientAssembly.Program.Get_framework_Client();
            bool doneOnce = false;
            while (obj.Get_client().Get_execute().Get_execute_Control().Get_flag_SystemInitialised() == true)
            {
                if (doneOnce == false)
                {
                    doneOnce = true;
                    obj.Get_client().Get_execute().Get_execute_Control().Set_flag_ThreadInitialised(obj, threadId, false);
                }
            }
            while (obj.Get_client().Get_execute().Get_execute_Control().Get_exitApplication() == false)
            {
                StatusCallback status = (ref StatusInfo info) => {
                    switch (info.connectionInfo.state)
                    {
                        case ConnectionState.None:
                            break;

                        case ConnectionState.Connected:
                            Console.WriteLine("Client connected to server - ID: " + _connection);
                            if (obj.Get_client().Get_data().Get_data_Control().Get_flag_IsLoaded_Stack_InputAction() == true)
                            {
                                System.Console.WriteLine("Thread[" + (threadId).ToString() + "] => IsLoaded_Stack_InputAction = " + obj.Get_client().Get_data().Get_data_Control().Get_flag_IsLoaded_Stack_InputAction());//TestBench
                                Avril_FSD.Library_For_WriteEnableForThreadsAt_CLIENTINPUTACTION.Write_Start(obj.Get_client().Get_execute().Get_program_WriteQue_C_IA(), 1);
                                byte[] data = new byte[64];
                                obj.Get_client().Get_data().Get_data_Control().Pop_Stack_InputAction(obj, obj.Get_client().Get_data().Get_input_Instnace().Get_FRONT_inputDoubleBuffer(obj), obj.Get_client().Get_data().Get_input_Instnace().Get_stack_Client_InputSend());
                                obj.Get_client().Get_data().Flip_InBufferToWrite();
                                obj.Get_client().Get_algorithms().Get_io_ListenRespond().Encode_NetworkingSteam_At_Client_Input(obj, obj.Get_client().Get_data().Get_input_Instnace().Get_BACK_inputDoubleBuffer(obj), data);
                                _client_SOCKET.SendMessageToConnection(_connection, data);
                                Avril_FSD.Library_For_WriteEnableForThreadsAt_CLIENTINPUTACTION.Write_End(obj.Get_client().Get_execute().Get_program_WriteQue_C_IA(), 1);
                            }
                            break;

                        case ConnectionState.ClosedByPeer:
                        case ConnectionState.ProblemDetectedLocally:
                            _client_SOCKET.CloseConnection(_connection);
                            Console.WriteLine("Client disconnected from server");
                            break;
                    }
                };
                _utils = new NetworkingUtils();
                _utils.SetStatusCallback(status);
                    
                _connection = _client_SOCKET.Connect(ref address_SERVER);
                System.Console.WriteLine("Thread[" + (threadId).ToString() + "] :: _connection = " + (_connection).ToString());//TestBench
#if VALVESOCKETS_SPAN
MessageCallback message = (in NetworkingMessage netMessage) => {
	Console.WriteLine("Message received from server - Channel ID: " + netMessage.channel + ", Data length: " + netMessage.length);
};
#else
                const int maxMessages = 20;
                NetworkingMessage[] netMessages = new NetworkingMessage[maxMessages];
#endif
                System.Console.WriteLine("Thread[" + (threadId).ToString() + "] :: ALPHA");//TestBench
                while (!Console.KeyAvailable)
                {
                    System.Console.WriteLine("Thread[" + (threadId).ToString() + "] :: BRAVO");//TestBench
                    _client_SOCKET.RunCallbacks();

#if VALVESOCKETS_SPAN
	client.ReceiveMessagesOnConnection(connection, message, 20);
#else
                    int netMessagesCount = _client_SOCKET.ReceiveMessagesOnConnection(_connection, netMessages, maxMessages);
                    System.Console.WriteLine("Thread[" + (threadId).ToString() + "] :: netMessagesCount = " + netMessagesCount);//TestBench
                    if (netMessagesCount > 0)
                    {
                        for (int i = 0; i < netMessagesCount; i++)
                        {
                            ref NetworkingMessage netMessage = ref netMessages[i];

                            Console.WriteLine("Message received from server - Channel ID: " + netMessage.channel + ", Data length: " + netMessage.length);
                            Avril_FSD.Library_For_WriteEnableForThreadsAt_CLIENTOUTPUTRECIEVE.Write_Start(obj.Get_client().Get_execute().Get_program_WriteQue_C_OR(), 1);
                            byte[] buffer = new byte[1024];
                            netMessage.CopyTo(buffer);
                            obj.Get_client().Get_data().Get_output_Instnace().Get_BACK_outputDoubleBuffer(obj).Set_praiseOutputBuffer_Subset(buffer[0]);
                            obj.Get_client().Get_algorithms().Get_io_ListenRespond().Decode_NetworkingSteam_At_Client_Recieve(obj, obj.Get_client().Get_data().Get_output_Instnace().Get_BACK_outputDoubleBuffer(obj), buffer);
                            obj.Get_client().Get_data().Flip_OutBufferToWrite();
                            obj.Get_client().Get_data().Get_data_Control().Push_Stack_Client_OutputRecieve(obj, obj.Get_client().Get_data().Get_output_Instnace().Get_stack_Client_OutputRecieves(), obj.Get_client().Get_data().Get_output_Instnace().Get_FRONT_outputDoubleBuffer(obj));
                            if (obj.Get_client().Get_data().Get_data_Control().Get_flag_IsLoaded_Stack_OutputRecieve())
                            {
                                if (Avril_FSD.Library_For_LaunchEnableForConcurrentThreadsAt_CLIENT.Get_State_LaunchBit(obj.Get_client().Get_execute().Get_program_ConcurrentQue_C()))
                                {
                                    Avril_FSD.Library_For_LaunchEnableForConcurrentThreadsAt_CLIENT.Request_Wait_Launch(obj.Get_client().Get_execute().Get_program_ConcurrentQue_C(), Avril_FSD.Library_For_LaunchEnableForConcurrentThreadsAt_CLIENT.Get_coreId_To_Launch(obj.Get_client().Get_execute().Get_program_ConcurrentQue_C()));
                                }
                            }
                            Avril_FSD.Library_For_WriteEnableForThreadsAt_CLIENTOUTPUTRECIEVE.Write_End(obj.Get_client().Get_execute().Get_program_WriteQue_C_OR(), 1);
                            netMessage.Destroy();
                        }
                    }
#endif

                    Thread.Sleep(15);
                }
            }
        }

````

Launch A Concurrency Thread

