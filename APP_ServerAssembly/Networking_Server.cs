using System;
using System.Data.Common;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Valve.Sockets;

namespace Avril_FSD.ServerAssembly
{
    public class Networking_Server
    {
        static private Address address_SERVER;
        static private Address address_CLIENT;
        static private NetworkingSockets _server_SOCKET;
        static private NetworkingUtils _utils;
        static private uint _listenSocket;
        static private uint _pollGroup;

        public Networking_Server()
        {
  
        }

        public void Initialise_networking_Server()
        {
            Valve.Sockets.Library.Initialize();
            _server_SOCKET = new NetworkingSockets();
            while (_server_SOCKET == null) { }
            address_SERVER = new Address();
            address_SERVER.SetAddress("192.168.8.100", 27000);
            address_CLIENT = new Address();
            address_CLIENT.SetAddress("192.168.8.215", 27001);
            _listenSocket = _server_SOCKET.CreateListenSocket(ref address_SERVER);
            _utils = new NetworkingUtils();
            while (_utils == null) { }
            _listenSocket = 0;
        }
        public void Thread_IO_Server(byte threadId)
        {
            Avril_FSD.ServerAssembly.Framework_Server obj = Avril_FSD.ServerAssembly.Program.Get_framework_Server();
            _listenSocket = 0;
            obj.Get_server().Get_execute().Get_execute_Control().Set_flag_ThreadInitialised(threadId, false);

            System.Console.WriteLine("Thread Initalised => Thread_IO_Server()" + (threadId).ToString());//TESTBENCH
            while (obj.Get_server().Get_execute().Get_execute_Control().Get_flag_isInitialised_ServerShell() == true)
            {

            }
            System.Console.WriteLine("Thread Starting => Thread_IO_Server()");//TestBench
            while (obj.Get_server().Get_execute().Get_execute_Control().Get_flag_isInitialised_ServerShell() == false)
            {
                if (obj.Get_server().Get_data().Get_data_Control().Get_flag_IsLoaded_Stack_OutputAction())
                {
                    uint pollGroup = _server_SOCKET.CreatePollGroup();

                    StatusCallback status = (ref StatusInfo info) => {
                        switch (info.connectionInfo.state)
                        {
                            case ConnectionState.None:
                                break;

                            case ConnectionState.Connecting:
                                _server_SOCKET.AcceptConnection(info.connection);
                                _server_SOCKET.SetConnectionPollGroup(pollGroup, info.connection);
                                break;

                            case ConnectionState.Connected:
                                Console.WriteLine("Client connected - ID: " + info.connection + ", IP: " + info.connectionInfo.address.GetIP());
                                Avril_FSD.Library_For_WriteEnableForThreadsAt_SERVEROUTPUTRECIEVE.Write_Start(Avril_FSD.Library_For_Server_Concurrency.Get_program_WriteEnableStack_ServerOutputRecieve(), 0);
                                byte[] data = new byte[64];
                                var output = obj.Get_server().Get_data().Get_output_Instnace().Get_FRONT_outputDoubleBuffer(obj);
                                output.Get_output_Control().SelectSetOutputSubset(obj, output.Get_praiseEventId());
                                obj.Get_server().Get_algorithms().Get_io_ListenRespond().Encode_NetworkingSteam_At_Server_Output(obj, output, data);
                                address_CLIENT.SetAddress(info.connectionInfo.address.GetIP(), 27001);
                                uint connection = _server_SOCKET.Connect(ref address_CLIENT);
                                _server_SOCKET.SendMessageToConnection(connection, data);
                                _server_SOCKET.CloseConnection(info.connection);
                                Avril_FSD.Library_For_WriteEnableForThreadsAt_SERVEROUTPUTRECIEVE.Write_End(Avril_FSD.Library_For_Server_Concurrency.Get_program_WriteEnableStack_ServerOutputRecieve(), 0);
                                break;
                        }
                    };

                    _utils.SetStatusCallback(status);



#if VALVESOCKETS_SPAN
	MessageCallback message = (in NetworkingMessage netMessage) => {
		Console.WriteLine("Message received from - ID: " + netMessage.connection + ", Channel ID: " + netMessage.channel + ", Data length: " + netMessage.length);
	};
#else
                    const int maxMessages = 20;

                    NetworkingMessage[] netMessages = new NetworkingMessage[maxMessages];
#endif

                    while (!Console.KeyAvailable)
                    {
                        _server_SOCKET.RunCallbacks();

#if VALVESOCKETS_SPAN
		server.ReceiveMessagesOnPollGroup(pollGroup, message, 20);
#else
                        int netMessagesCount = _server_SOCKET.ReceiveMessagesOnPollGroup(pollGroup, netMessages, maxMessages);

                        if (netMessagesCount > 0)
                        {
                            for (int i = 0; i < netMessagesCount; i++)
                            {
                                ref NetworkingMessage netMessage = ref netMessages[i];

                                Console.WriteLine("Message received from - ID: " + netMessage.connection + ", Channel ID: " + netMessage.channel + ", Data length: " + netMessage.length);
                                Avril_FSD.Library_For_WriteEnableForThreadsAt_SERVERINPUTACTION.Write_Start(Avril_FSD.Library_For_Server_Concurrency.Get_program_WriteEnableStack_ServerInputAction(), 0);
                                byte[] buffer = new byte[1024];
                                netMessage.CopyTo(buffer);
                                Avril_FSD.Library_For_Server_Concurrency.Select_Set_Intput_Subset(obj.Get_server().Get_execute().Get_program_ServerConcurrency(), buffer[0]);
                                obj.Get_server().Get_algorithms().Get_io_ListenRespond().Decode_NetworkingSteam_At_Server_Input(obj, obj.Get_server().Get_data().Get_input_Instnace().Get_FRONT_inputDoubleBuffer(obj), buffer);
                                Avril_FSD.Library_For_Server_Concurrency.Flip_InBufferToWrite(obj.Get_server().Get_execute().Get_program_ServerConcurrency());
                                Avril_FSD.Library_For_Server_Concurrency.Push_Stack_InputPraises(obj.Get_server().Get_execute().Get_program_ServerConcurrency());
                                if (Avril_FSD.Library_For_LaunchEnableForConcurrentThreadsAt_SERVER.Get_Flag_ConcurrentCoreState(obj.Get_server().Get_execute().Get_program_ServerConcurrency(), Avril_FSD.Library_For_LaunchEnableForConcurrentThreadsAt_SERVER.Get_coreId_To_Launch(obj.Get_server().Get_execute().Get_program_ServerConcurrency())) == Avril_FSD.Library_For_LaunchEnableForConcurrentThreadsAt_SERVER.Get_Flag_Idle(obj.Get_server().Get_execute().Get_program_ServerConcurrency()))
                                {
                                    Avril_FSD.Library_For_LaunchEnableForConcurrentThreadsAt_SERVER.Request_Wait_Launch(obj.Get_server().Get_execute().Get_program_ServerConcurrency(), Avril_FSD.Library_For_LaunchEnableForConcurrentThreadsAt_SERVER.Get_coreId_To_Launch(obj.Get_server().Get_execute().Get_program_ServerConcurrency()));
                                }
                                Avril_FSD.Library_For_WriteEnableForThreadsAt_SERVERINPUTACTION.Write_End(Avril_FSD.Library_For_Server_Concurrency.Get_program_WriteEnableStack_ServerInputAction(), 0);
                                netMessage.Destroy();
                            }
                        }
#endif
                        Thread.Sleep(15);
                    }
                    _server_SOCKET.DestroyPollGroup(pollGroup);
                }
            }
        }
        public NetworkingUtils Get_utils()
        {
            return _utils;
        }
        public uint Get_pollGroup()
        {
            return _pollGroup;
        }
    }
}
