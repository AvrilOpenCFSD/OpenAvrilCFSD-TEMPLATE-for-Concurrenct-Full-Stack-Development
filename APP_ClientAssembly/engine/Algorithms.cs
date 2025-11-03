namespace Avril_FSD.ClientAssembly
{
    public class Algorithms
    {
        private Avril_FSD.ClientAssembly.IO_Listen_Respond _io_ListenRespond;
        private Avril_FSD.ClientAssembly.Concurrent[] _concurrency;
        private Avril_FSD.ClientAssembly.Praise_Files.User_Alg _user_Alg;

        public Algorithms(int numberOfCores) 
        {
            Set_user_Alg(new Praise_Files.User_Alg());
            while (Get_user_Alg() == null) { }
            System.Console.WriteLine("Avril_FSD.ClientAssembly: Algorithms");//TEST
        }

        public void Initialise(int numberOfCores)
        {
            Set_io_ListenRespond(new Avril_FSD.ClientAssembly.IO_Listen_Respond());
            while (Get_io_ListenRespond() == null) { }
            Get_io_ListenRespond().InitialiseControl();

            _concurrency = new Avril_FSD.ClientAssembly.Concurrent[(byte)(numberOfCores - (byte)2)];
            for (byte index = 0; index < (byte)(numberOfCores - (byte)2); index++)
            {
                Set_concurrent(index, new Avril_FSD.ClientAssembly.Concurrent());
                while (Get_concurrent(index) == null) { }
                Get_concurrent(index).Initialise_Control();
            }
        }

        public Avril_FSD.ClientAssembly.IO_Listen_Respond Get_io_ListenRespond()
        {
            return _io_ListenRespond;
        }
        public Concurrent Get_concurrent(byte concurrenctCoreId)
        {
            return _concurrency[concurrenctCoreId];
        }
        public Praise_Files.User_Alg Get_user_Alg()
        {
            return _user_Alg;
        }

        private void Set_io_ListenRespond(Avril_FSD.ClientAssembly.IO_Listen_Respond value)
        {
            _io_ListenRespond = value;
        }
        private void Set_concurrent(byte concurrenctCoreId, Concurrent value)
        {
            _concurrency[concurrenctCoreId] = value;
        }
        private void Set_user_Alg(Praise_Files.User_Alg value)
        {
            _user_Alg = value;
        }
    }
}
