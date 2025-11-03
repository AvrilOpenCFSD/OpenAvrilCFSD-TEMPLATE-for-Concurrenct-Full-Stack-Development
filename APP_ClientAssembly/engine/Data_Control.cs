namespace Avril_FSD.ClientAssembly
{
    public class Data_Control
    {
        private bool _flag_IsLoaded_Stack_InputAction;
        private bool _flag_IsLoaded_Stack_OutputRecieve;
        private bool _flag_isNewInputDataReady;
        private bool _flag_isNewOutputDataReady;
        private bool[] _isPraiseActive;

        public Data_Control()
        {
            Set_flag_IsLoaded_Stack_InputAction(false);
            Set_flag_IsLoaded_Stack_OutputRecieve(false);
        }
        public void Initialise(Avril_FSD.ClientAssembly.Framework_Client obj)
        {
            _isPraiseActive = new bool[obj.Get_client().Get_global().Get_numberOfPraises()];
            for (int index = 0; index < obj.Get_client().Get_global().Get_numberOfPraises(); index++)
            {
                Set_isPraiseActive(index, false);
            }
        }

        public void Push_Stack_Client_InputAction(
            List<Avril_FSD.ClientAssembly.Inputs.Input> stack_Client_InputSend,
            Avril_FSD.ClientAssembly.Inputs.Input BACK_inputDoubleBuffer
        )
        {
            System.Console.WriteLine("entered => Push_Stack_Client_InputAction()");//TestBench
            stack_Client_InputSend.Add(BACK_inputDoubleBuffer);
            if(stack_Client_InputSend.Count >= 2)
            {
                Set_flag_IsLoaded_Stack_InputAction(true);
            }
            else
            {
                Set_flag_IsLoaded_Stack_InputAction(false);
            }
            System.Console.WriteLine("exiting => Push_Stack_Client_InputAction()");//TestBench
        }
        public void Push_Stack_Client_OutputRecieve(
            List<Avril_FSD.ClientAssembly.Outputs.Output> stack_Client_OutputRecieves,
            Avril_FSD.ClientAssembly.Outputs.Output FRONT_outputDoubleBuffer
        )
        {
            stack_Client_OutputRecieves.Add(FRONT_outputDoubleBuffer);
            if (stack_Client_OutputRecieves.Count >= 2)
            {
                Set_flag_IsLoaded_Stack_OutputRecieve(true);
            }
            else
            {
                Set_flag_IsLoaded_Stack_OutputRecieve(false);
            }
        }
        public void Pop_Stack_InputAction(
            Avril_FSD.ClientAssembly.Inputs.Input FRONT_inputDoubleBuffer,
            List<Avril_FSD.ClientAssembly.Inputs.Input> stack_Client_InputSend
        )
        {
            FRONT_inputDoubleBuffer = stack_Client_InputSend.ElementAt(1);
            stack_Client_InputSend.RemoveAt(1);
            if (stack_Client_InputSend.Count >= 2)
            {
                Set_flag_IsLoaded_Stack_InputAction(true);
            }
            else
            {
                Set_flag_IsLoaded_Stack_InputAction(false);
            }
        }
        public void Pop_Stack_OutputRecieve(
            Avril_FSD.ClientAssembly.Outputs.Output buffer_Output_Recieve_Reference_ForCore,
            List<Avril_FSD.ClientAssembly.Outputs.Output> stack_Client_OutputRecieves
        )
        {
            buffer_Output_Recieve_Reference_ForCore = stack_Client_OutputRecieves.ElementAt(1);
            stack_Client_OutputRecieves.RemoveAt(1);
            if (stack_Client_OutputRecieves.Count >= 2)
            {
                Set_flag_IsLoaded_Stack_OutputRecieve(true);
            }
            else
            {
                Set_flag_IsLoaded_Stack_OutputRecieve(false);
            }
        }

        public void Do_Store_PraiseOutputRecieve_To_GameInstanceData(Avril_FSD.ClientAssembly.Framework_Client obj, Avril_FSD.ClientAssembly.Outputs.Output stackSlot)
        {
            switch (stackSlot.Get_praiseEventId())
            {
                case 0:
                    break;

                case 1:
                    var output_Subset_Praise1 = (Avril_FSD.ClientAssembly.Praise_Files.Praise1_Output)stackSlot.Get_praiseOutputBuffer_Subset();
                    obj.Get_client().Get_data().Get_gameInstance().Get_gameObjectFactory().Get_player().Get_CameraFP().Set_fowards(output_Subset_Praise1.Get_fowards());
                    obj.Get_client().Get_data().Get_gameInstance().Get_gameObjectFactory().Get_player().Get_CameraFP().Set_right(output_Subset_Praise1.Get_right());
                    obj.Get_client().Get_data().Get_gameInstance().Get_gameObjectFactory().Get_player().Get_CameraFP().Set_up(output_Subset_Praise1.Get_up());
                    obj.Get_client().Get_data().Get_data_Control().Set_isPraiseActive(1, false);
                    break;
            }
        }
        public bool Get_flag_IsLoaded_Stack_InputAction()
        {
            return _flag_IsLoaded_Stack_InputAction;
        }
        public bool Get_flag_IsLoaded_Stack_OutputRecieve()
        {
            return _flag_IsLoaded_Stack_OutputRecieve;
        }
        public bool Get_flag_isNewInputDataReady()
        {
            return _flag_isNewInputDataReady;
        }
        public bool Get_flag_isNewOutputDataReady()
        {
            return _flag_isNewOutputDataReady;
        }
        public bool Get_isPraiseActive(int praiseEventId)
        {
            return _isPraiseActive[praiseEventId];
        }

        public void Set_flag_IsLoaded_Stack_InputAction(bool value)
        {
            _flag_IsLoaded_Stack_InputAction = value;
        }
        public void Set_flag_IsLoaded_Stack_OutputRecieve(bool value)
        {
            _flag_IsLoaded_Stack_OutputRecieve = value;
        }
        public void Set_flag_isNewInputDataReady(bool value)
        {
            _flag_isNewInputDataReady = value;
        }
        public void Set_flag_isNewOutputDataReady(bool value)
        {
            _flag_isNewOutputDataReady = value;
        }
        public void Set_isPraiseActive(int praiseEventId, bool value)
        {
            _isPraiseActive[praiseEventId] = value; 
        }
    }
}
