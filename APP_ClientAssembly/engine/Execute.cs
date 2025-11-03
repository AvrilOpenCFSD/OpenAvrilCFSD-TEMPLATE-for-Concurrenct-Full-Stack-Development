using System;
using System.Threading;

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
            obj.Get_client().Get_execute().Get_execute_Control().Set_flag_ThreadInitialised(threadIdCounter, false);
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
