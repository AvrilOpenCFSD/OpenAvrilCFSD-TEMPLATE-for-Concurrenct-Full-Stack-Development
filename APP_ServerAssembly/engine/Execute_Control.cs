
namespace Avril_FSD.ServerAssembly
{
    public class Execute_Control
    {
        private bool _flag_SystemInitialised;
        private bool[] _flag_ThreadInitialised;

        public Execute_Control(int numberOfCores)
        {
            _flag_ThreadInitialised = new bool[numberOfCores];
            for(byte index = 0; index < numberOfCores; index++)
            {
                Set_flag_ThreadInitialised(index, true);
            }
            _flag_SystemInitialised = true;
        }
        private void Calc_flag_SystemInitialised()
        {
            _flag_SystemInitialised = false;
            for (byte index = 0; index < _flag_ThreadInitialised.Length; index++)
            {
                if (Get_flag_ThreadInitialised(index) == true)
                {
                    _flag_SystemInitialised = true;
                }
            }
        }
        public bool Get_flag_isInitialised_ServerShell()
        {
            return _flag_SystemInitialised;
        }
        public bool Get_flag_ThreadInitialised(byte coreId)
        {
            return _flag_ThreadInitialised[coreId];
        }
        public void SetConditionCodeOfThisThreadedCore(byte coreId)
        {
            Set_flag_ThreadInitialised(coreId, false);
        }
        public void Set_flag_ThreadInitialised(byte coreId, bool value)
        {
            _flag_ThreadInitialised[coreId] = false;
            Calc_flag_SystemInitialised();
        }
    }
}
