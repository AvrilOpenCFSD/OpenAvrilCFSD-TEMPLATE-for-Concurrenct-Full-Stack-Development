namespace Avril_FSD.ClientAssembly
{
    public class Framework_Client
    {
        static private Avril_FSD.ClientAssembly.Client _client;
        //private Networking networkingClient;

        private Int16 threadId = 0;

        public Framework_Client() 
        {
            Set_client(new Avril_FSD.ClientAssembly.Client());
            while (Get_client() == null){ /* Wait whileis created */ }
            System.Console.WriteLine("Created = > Avril_FSD.ClientAssembly.Client()");//TESTBENCH
        }
        public void Initialise(Avril_FSD.ClientAssembly.Framework_Client obj)
        {
            obj.Get_client().Get_algorithms().Initialise(obj.Get_client().Get_global().Get_numberOfCores());
            System.Console.WriteLine("alpha");//TESTBENCH
            obj.Get_client().Get_data().Get_data_Control().Initialise(obj);
            System.Console.WriteLine("bravo");//TESTBENCH
            obj.Get_client().Get_execute().Initialise_NetworkingPipes(obj);
            System.Console.WriteLine("charlie");//TESTBENCH
            obj.Get_client().Get_execute().Initialise_Libraries();
            System.Console.WriteLine("delta");//TESTBENCH
            obj.Get_client().Get_execute().Create_And_Run_Graphics(obj);
            System.Console.WriteLine("echo");//TESTBENCH

        }
        static public Avril_FSD.ClientAssembly.Client Get__client()
        {
            return _client;
        }
        public Avril_FSD.ClientAssembly.Client Get_client()
        {
            return _client;
        }
        private void Set_client(Avril_FSD.ClientAssembly.Client value)
        {
            _client = value;
        }
    }
}
