using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inzynierka
{
    public static class Program
    {

        [STAThread]
        public static int Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                // TODO: Add your code to run in command line mode
                Console.WriteLine("Hello world. ");
                Console.ReadLine();
                return 0;
            }
            else
            {
              //  FreeConsole();
                var app = new App();
                return app.Run();
            }
        }
    }
}
