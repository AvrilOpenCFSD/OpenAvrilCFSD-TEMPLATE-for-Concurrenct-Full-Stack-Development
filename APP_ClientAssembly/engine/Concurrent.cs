
namespace Avril_FSD.ClientAssembly
{
    public class Concurrent
    {
        static private Avril_FSD.ClientAssembly.Concurrent_Control _concurrent_Control;
        static private Object _algorithm_Subset;
        public Concurrent() 
        {
            _concurrent_Control = null;
            _algorithm_Subset = null;
        } 
        public void Initialise_Control()
        {
            _concurrent_Control = new Avril_FSD.ClientAssembly.Concurrent_Control();
            while (_concurrent_Control == null) { /* Wait while is created */ }
        }

        public Object Get_algorithm_Subset()
        {
            return _algorithm_Subset;
        }

        public Avril_FSD.ClientAssembly.Concurrent_Control Get_concurrent_Control()
        {
            return _concurrent_Control;
        }

        public void Set_Algorithm_Subset(Avril_FSD.ClientAssembly.Praise_Files.Praise0_Algorithm value)
        {
            _algorithm_Subset = (Object)value;
        }
        public void Set_Algorithm_Subset(Avril_FSD.ClientAssembly.Praise_Files.Praise1_Algorithm value)
        {
            _algorithm_Subset = (Object)value;
        }
        public void Set_Algorithm_Subset(Avril_FSD.ClientAssembly.Praise_Files.Praise2_Algorithm value)
        {
            _algorithm_Subset = (Object)value;
        }
        public void Thread_Concurrent(byte threadId)
        {
            byte _concurrentThreadId = (byte)(threadId - (byte)2);
            Avril_FSD.ClientAssembly.Framework_Client obj = Avril_FSD.ClientAssembly.Program.Get_framework_Client();
            obj.Get_client().Get_execute().Get_execute_Control().Set_flag_ThreadInitialised(threadId, false);
            System.Console.WriteLine("Thread Initalised => Thread_Concurrent()" + (threadId).ToString());//TESTBENCH
            while (obj.Get_client().Get_execute().Get_execute_Control().Get_flag_SystemInitialised() == true)
            {

            }
            System.Console.WriteLine("Thread Starting => Thread_Concurrent()" + (threadId).ToString());//TESTBENCH
            while (obj.Get_client().Get_execute().Get_execute_Control().Get_flag_SystemInitialised() == false)
            {
                if (obj.Get_client().Get_data().Get_data_Control().Get_flag_IsLoaded_Stack_OutputRecieve())
                {
                    Avril_FSD.Library_For_WriteEnableForThreadsAt_CLIENTOUTPUTRECIEVE.Write_Start(obj.Get_client().Get_execute().Get_program_WriteQue_C_OR(), (byte)(_concurrentThreadId + (byte)1));
                    obj.Get_client().Get_algorithms().Get_concurrent(_concurrentThreadId).Get_concurrent_Control().SelectSet_Algorithm_Subset(obj, obj.Get_client().Get_data().Get_output_Instnace().Get_BACK_outputDoubleBuffer(obj).Get_praiseEventId(), _concurrentThreadId);
                    obj.Get_client().Get_data().Get_data_Control().Pop_Stack_OutputRecieve(obj.Get_client().Get_data().Get_output_Instnace().Get_BACK_outputDoubleBuffer(obj), obj.Get_client().Get_data().Get_output_Instnace().Get_stack_Client_OutputRecieves());
                    obj.Get_client().Get_data().Flip_OutBufferToWrite();
                    obj.Get_client().Get_data().Get_data_Control().Do_Store_PraiseOutputRecieve_To_GameInstanceData(obj, obj.Get_client().Get_data().Get_output_Instnace().Get_stack_Client_OutputRecieves().ElementAt(1));
                    obj.Get_client().Get_data().Get_data_Control().Set_isPraiseActive(obj.Get_client().Get_data().Get_output_Instnace().Get_FRONT_outputDoubleBuffer(obj).Get_praiseEventId(), false);
                    Avril_FSD.Library_For_LaunchEnableForConcurrentThreadsAt_CLIENT.Thread_End(obj.Get_client().Get_execute().Get_program_ConcurrentQue_C(), _concurrentThreadId);
                    if (obj.Get_client().Get_data().Get_data_Control().Get_flag_IsLoaded_Stack_OutputRecieve())
                    {
                        if(Avril_FSD.Library_For_LaunchEnableForConcurrentThreadsAt_CLIENT.Get_State_LaunchBit(obj.Get_client().Get_execute().Get_program_ConcurrentQue_C()) == Avril_FSD.Library_For_LaunchEnableForConcurrentThreadsAt_CLIENT.Get_Flag_Idle(obj.Get_client().Get_execute().Get_program_ConcurrentQue_C()))
                        {
                            Avril_FSD.Library_For_LaunchEnableForConcurrentThreadsAt_CLIENT.Request_Wait_Launch(obj.Get_client().Get_execute().Get_program_ConcurrentQue_C(), Avril_FSD.Library_For_LaunchEnableForConcurrentThreadsAt_CLIENT.Get_coreId_To_Launch(obj.Get_client().Get_execute().Get_program_ConcurrentQue_C()));
                        }
                    }
                    Avril_FSD.Library_For_WriteEnableForThreadsAt_CLIENTOUTPUTRECIEVE.Write_End(obj.Get_client().Get_execute().Get_program_WriteQue_C_OR(), (byte)(_concurrentThreadId + (byte)1));
                }
            }
        }
    }
}
