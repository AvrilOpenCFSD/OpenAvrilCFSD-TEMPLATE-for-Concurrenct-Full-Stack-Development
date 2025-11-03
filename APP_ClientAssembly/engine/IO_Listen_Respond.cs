using OpenTK;
using Valve.Sockets;

namespace Avril_FSD.ClientAssembly
{ 
    public class IO_Listen_Respond
    {
        private Avril_FSD.ClientAssembly.IO_Listen_Respond_Control _io_Control;
        
        public IO_Listen_Respond() 
        {
            Set_io_Control(null);
        }
        public void InitialiseControl()
        {
            Set_io_Control(new Avril_FSD.ClientAssembly.IO_Listen_Respond_Control());
            while (Get_io_Control() == null) { }
        }
        public void Encode_NetworkingSteam_At_Client_Input(Avril_FSD.ClientAssembly.Framework_Client obj, Avril_FSD.ClientAssembly.Inputs.Input input, byte[] data)
        {
            data[0] = input.Get_praiseEventId();
            data[1] = input.Get_playerId();
            switch (input.Get_praiseEventId())
            {
                case 0:
                    break;

                case 1:
                    var input_Subset_Praise1 = (Avril_FSD.ClientAssembly.Praise_Files.Praise1_Input)input.Get_praiseInputBuffer_Subset();
                    byte[] byteArray = new byte[4];
                    byteArray = BitConverter.GetBytes(input_Subset_Praise1.Get_Mouse_X());
                    for (byte index = 0; index < 4; index++)
                    {
                        data[index + 2] = byteArray[index];
                    }
                    byteArray = BitConverter.GetBytes(input_Subset_Praise1.Get_Mouse_Y());
                    for (byte index = 0; index < 4; index++)
                    {
                        data[index + 6] = byteArray[index];
                    }
                    break;
            }
        }
        public void Decode_NetworkingSteam_At_Client_Recieve(Avril_FSD.ClientAssembly.Framework_Client obj, Avril_FSD.ClientAssembly.Outputs.Output output, byte[] buffer)
        {
            output.Set_praiseEventId(buffer[0]);
            output.Set_playerId(buffer[1]);
            switch (output.Get_praiseEventId())
            {
                case 0:

                    break;

                case 1:
                    var subset = (Avril_FSD.ClientAssembly.Praise_Files.Praise1_Output)output.Get_praiseOutputBuffer_Subset();
                    Vector3 tempVector = new Vector3(0);
                    tempVector.X = BitConverter.ToSingle(buffer, 2);
                    tempVector.Y = BitConverter.ToSingle(buffer, 6);
                    tempVector.Z = BitConverter.ToSingle(buffer, 10);
                    subset.Set_fowards(tempVector);
                    tempVector.X = BitConverter.ToSingle(buffer, 14);
                    tempVector.Y = BitConverter.ToSingle(buffer, 18);
                    tempVector.Z = BitConverter.ToSingle(buffer, 22);
                    subset.Set_right(tempVector);
                    tempVector.X = BitConverter.ToSingle(buffer, 26);
                    tempVector.Y = BitConverter.ToSingle(buffer, 30);
                    tempVector.Z = BitConverter.ToSingle(buffer, 34);
                    subset.Set_up(tempVector);
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
