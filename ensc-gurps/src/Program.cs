using ensc_gurps.controller;
using ensc_gurps.model;
using ensc_gurps.view;
using ensc_gurps.view.console;
using System;

namespace ensc_gurps
{
    class Program
    {
        private static string _appTitle = "ensc-gurps";
        private static string _appVersion = "1.0";

        static void Main(string[] args)
        {
            Model model = new Model();
            ConsoleView  view = new ConsoleView(string.Format("{0} v{1}", _appTitle, _appVersion));
            
            Controller controller = new Controller(model, view);
            controller.Run();
        }
    }
}
