namespace WindowMonitor
{
    using System;
    using System.Configuration;
    using System.ServiceProcess;

    /// <summary>
    /// The main entry point. Starts the service either as a console app or
    /// registered with the service manager.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            var service = new WindowMonitorService(ConfigurationManager.AppSettings["ProcessMatcher"]);

            if (Environment.UserInteractive)
            {
                service.Startup();
                Console.WriteLine("Service started; Press <enter> to stop.");
                Console.ReadLine();
                service.Shutdown();
            }
            else
            {
                ServiceBase.Run(new ServiceBase[] 
                { 
                    service
                });                
            }
        }
    }
}
