using OpenTK;
using System.Threading;
using Valve.Sockets;

namespace Avril_FSD.ServerAssembly
{
    public class IO_Listen_Respond
    {
        private Avril_FSD.ServerAssembly.IO_Listen_Respond_Control _io_Control;

        public IO_Listen_Respond()
        {
            Set_io_Control(null);
        }
        public void InitialiseControl()
        {
            Set_io_Control(new Avril_FSD.ServerAssembly.IO_Listen_Respond_Control());
            while (Get_io_Control() == null) { }
        }

        public void Decode_NetworkingSteam_At_Server_Input(Avril_FSD.ServerAssembly.Framework_Server obj, Avril_FSD.ServerAssembly.Inputs.Input input, byte[] buffer)
        {
            input.Set_praiseEventId(buffer[0]);
            input.Set_in_playerId(buffer[1]);
            input.Get_input_Control().SelectSetIntputSubset(obj, input.Get_praiseEventId());
            switch (input.Get_praiseEventId())
            {
                case 0:
                    break;

                case 1:
                    var in_subset_praise1 = (Avril_FSD.ServerAssembly.Praise_Files.Praise1_Input)input.Get_praiseInputBuffer_Subset();
                    in_subset_praise1.Set_Mouse_X(BitConverter.ToSingle(buffer, 2));
                    in_subset_praise1.Set_Mouse_Y(BitConverter.ToSingle(buffer, 6));
                    break;
            }
        }
       
        public void Encode_NetworkingSteam_At_Server_Output(Avril_FSD.ServerAssembly.Framework_Server obj, Avril_FSD.ServerAssembly.Outputs.Output output, byte[] data)
        {
            data[0] = output.Get_praiseEventId();
            data[1] = output.Get_out_playerId();
            switch (output.Get_praiseEventId())
            {
                case 0:
                    break;

                case 1:
                    var subset = (Avril_FSD.ServerAssembly.Praise_Files.Praise1_Output)output.GetOutputBufferSubset();
                    byte[] tempFloat = BitConverter.GetBytes(subset.Get_fowards().X);
                    for (byte index = 0; index < tempFloat.Length; index++)
                    {
                        data[index + 2] = tempFloat[index];
                    }
                    tempFloat = BitConverter.GetBytes(subset.Get_fowards().Y);
                    for (byte index = 0; index < tempFloat.Length; index++)
                    {
                        data[index + 6] = tempFloat[index];
                    }
                    tempFloat = BitConverter.GetBytes(subset.Get_fowards().Z);
                    for (byte index = 0; index < tempFloat.Length; index++)
                    {
                        data[index + 10] = tempFloat[index];
                    }
                    tempFloat = BitConverter.GetBytes(subset.Get_right().X);
                    for (byte index = 0; index < tempFloat.Length; index++)
                    {
                        data[index + 14] = tempFloat[index];
                    }
                    tempFloat = BitConverter.GetBytes(subset.Get_right().Y);
                    for (byte index = 0; index < tempFloat.Length; index++)
                    {
                        data[index + 18] = tempFloat[index];
                    }
                    tempFloat = BitConverter.GetBytes(subset.Get_right().Z);
                    for (byte index = 0; index < tempFloat.Length; index++)
                    {
                        data[index + 22] = tempFloat[index];
                    }
                    tempFloat = BitConverter.GetBytes(subset.Get_up().X);
                    for (byte index = 0; index < tempFloat.Length; index++)
                    {
                        data[index + 26] = tempFloat[index];
                    }
                    tempFloat = BitConverter.GetBytes(subset.Get_up().Y);
                    for (byte index = 0; index < tempFloat.Length; index++)
                    {
                        data[index + 30] = tempFloat[index];
                    }
                    tempFloat = BitConverter.GetBytes(subset.Get_up().Z);
                    for (byte index = 0; index < tempFloat.Length; index++)
                    {
                        data[index + 34] = tempFloat[index];
                    }
                    break;

            }
        }
        public IO_Listen_Respond_Control Get_io_Control()
        {
            return _io_Control;
        }
        public void Set_io_Control(IO_Listen_Respond_Control io_control)
        {
            _io_Control = io_control;
        }
    }
}
