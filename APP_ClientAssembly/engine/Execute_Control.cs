
namespace Avril_FSD.ClientAssembly
{
    public class Execute_Control
    {
        private bool _flag_SystemInitialised;
        private bool[] _flag_ThreadInitialised;

        public Execute_Control(int numberOfCores)
        {
            Set_flag_SystemInitialised(true);

            _flag_ThreadInitialised = new bool[numberOfCores];
            for(byte index = 0; index < numberOfCores; index++)
            {
                Set_flag_ThreadInitialised(index, true);
            }
        }

        private void Calc_flag_isInitialised_ClientApp()
        {
            Set_flag_SystemInitialised(false);
            for (byte index = 0; index < _flag_ThreadInitialised.Length; index++)
            {
                if (Get_flag_ThreadInitialised(index) == true)
                {
                    Set_flag_SystemInitialised(true);
                }
            }
        }
        public bool Get_flag_SystemInitialised()
        {
            return _flag_SystemInitialised;
        }
        public bool Get_flag_ThreadInitialised(byte coreId)
        {   
            return _flag_ThreadInitialised[coreId];
        }

        private void Set_flag_SystemInitialised(bool flag)
        {
            _flag_SystemInitialised = flag;
        }
        public void Set_flag_ThreadInitialised(byte coreId, bool value)
        {
            System.Console.WriteLine("_flag_ThreadInitialised[ " + coreId + " ] = " + value);//TestBench
            _flag_ThreadInitialised[coreId] = value;
            Calc_flag_isInitialised_ClientApp();
        }
    }
}
