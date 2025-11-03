using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Valve.Sockets;


namespace Avril_FSD.ClientAssembly
{
    public class Networking_Client
    {
        static private Address address_SERVER;
        static private Address address_CLIENT;
        static private NetworkingSockets _client_SOCKET;
        static private NetworkingUtils _utils;
        static private uint _connection;

        public Networking_Client()
        {
            
        }

        public void Initialise_networking_Client()
        {
            Valve.Sockets.Library.Initialize();
            _client_SOCKET = new NetworkingSockets();
            address_SERVER = new Address();
            address_SERVER.SetAddress("192.168.8.100", 27000);
            address_CLIENT = new Address();
            address_CLIENT.SetAddress("192.168.8.215", 27001);
            _utils = new NetworkingUtils();
            _connection = 0;
        }
        public void Thread_IO_Client(byte threadId)
        {
            Avril_FSD.ClientAssembly.Framework_Client obj = Avril_FSD.ClientAssembly.Program.Get_framework_Client();
            _connection = 0;
            obj.Get_client().Get_execute().Get_execute_Control().Set_flag_ThreadInitialised(threadId, false);

            System.Console.WriteLine("Thread Initalised => Thread_Input_Send()" + (threadId).ToString());//TESTBENCH
            while (obj.Get_client().Get_execute().Get_execute_Control().Get_flag_SystemInitialised() == true)
            {

            }
            System.Console.WriteLine("Thread Starting => Thread_Input_Send()");//TestBench
            while (obj.Get_client().Get_execute().Get_execute_Control().Get_flag_SystemInitialised() == false)
            {
                if (obj.Get_client().Get_data().Get_data_Control().Get_flag_IsLoaded_Stack_InputAction())
                {
                    System.Console.WriteLine("Thread[" + (threadId).ToString() + "] => ALPHA");//TestBench
                    StatusCallback status = (ref StatusInfo info) => {
                        switch (info.connectionInfo.state)
                        {
                            case ConnectionState.None:
                                break;

                            case ConnectionState.Connected:
                                Console.WriteLine("Client connected to server - ID: " + _connection);
                                Avril_FSD.Library_For_WriteEnableForThreadsAt_CLIENTINPUTACTION.Write_Start(obj.Get_client().Get_execute().Get_program_WriteQue_C_IA(), 1);
                                byte[] data = new byte[64];
                                obj.Get_client().Get_data().Get_data_Control().Pop_Stack_InputAction(obj.Get_client().Get_data().Get_input_Instnace().Get_FRONT_inputDoubleBuffer(obj), obj.Get_client().Get_data().Get_input_Instnace().Get_stack_Client_InputSend());
                                obj.Get_client().Get_data().Flip_InBufferToWrite();
                                obj.Get_client().Get_algorithms().Get_io_ListenRespond().Encode_NetworkingSteam_At_Client_Input(obj, obj.Get_client().Get_data().Get_input_Instnace().Get_BACK_inputDoubleBuffer(obj), data);
                                if (_client_SOCKET.GetListenSocketAddress(_connection, ref address_SERVER))
                                {
                                    _client_SOCKET.SendMessageToConnection(_connection, data);
                                }
                                Avril_FSD.Library_For_WriteEnableForThreadsAt_CLIENTINPUTACTION.Write_End(obj.Get_client().Get_execute().Get_program_WriteQue_C_IA(), 1);
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



                    _connection = _client_SOCKET.Connect(ref address_CLIENT);

#if VALVESOCKETS_SPAN
	MessageCallback message = (in NetworkingMessage netMessage) => {
		Console.WriteLine("Message received from server - Channel ID: " + netMessage.channel + ", Data length: " + netMessage.length);
	};
#else
                    const int maxMessages = 20;

                    NetworkingMessage[] netMessages = new NetworkingMessage[maxMessages];
#endif

                    while (!Console.KeyAvailable)
                    {
                        _client_SOCKET.RunCallbacks();

#if VALVESOCKETS_SPAN
		client.ReceiveMessagesOnConnection(connection, message, 20);
#else
                        int netMessagesCount = _client_SOCKET.ReceiveMessagesOnConnection(_connection, netMessages, maxMessages);

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
                                obj.Get_client().Get_data().Get_data_Control().Push_Stack_Client_OutputRecieve(obj.Get_client().Get_data().Get_output_Instnace().Get_stack_Client_OutputRecieves(), obj.Get_client().Get_data().Get_output_Instnace().Get_FRONT_outputDoubleBuffer(obj));
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
        }

        public NetworkingSockets Get_client_SOCKET()
        {
            return _client_SOCKET;
        }
        public NetworkingUtils Get_utils()
        {
            return _utils;
        }
    }
}
