namespace WindowMonitor
{
    using System;
    using System.Linq;
    using System.ServiceProcess;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Timers;

    /// <summary>
    /// Background service to monitor window status.
    /// </summary>
    public partial class WindowMonitorService : ServiceBase
    {
        /// <summary>
        /// Timer to use to schedule window checks.
        /// </summary>
        private Timer timer;

        /// <summary>
        /// The window monitor to use.
        /// </summary>
        private WindowMonitor monitor;

        /// <summary>
        /// The regex to use to extract image names.
        /// </summary>
        private Regex filterRegex;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowMonitorService"/> class. 
        /// </summary>
        /// <param name="imageFilter">
        /// The filter to use to extract image names.
        /// </param>
        public WindowMonitorService(string imageFilter)
        {
            this.InitializeComponent();
            this.filterRegex = new Regex(imageFilter);
        }

        /// <summary>
        /// Starts the service.
        /// </summary>
        public void Startup()
        {
            this.monitor = new WindowMonitor();
            this.timer = new Timer(1000);
            this.timer.Elapsed += this.OnTimerElapsed;
            this.timer.Start();
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        public void Shutdown()
        {
            this.timer.Stop();
            this.timer.Elapsed -= this.OnTimerElapsed;
            this.timer.Dispose();
            this.timer = null;
            this.monitor = null;
        }

        /// <summary>
        /// Called when the service is being started by the Windows host.
        /// </summary>
        /// <param name="args">
        /// The arguments given to the service.
        /// </param>
        protected override void OnStart(string[] args)
        {
            this.Startup();
        }

        /// <summary>
        /// Called when the service is being stopped by the Windows host.
        /// </summary>
        protected override void OnStop()
        {
            this.Shutdown();
        }

        /// <summary>
        /// Called when our timer elapses, triggering us to do some work.
        /// </summary>
        /// <param name="sender">
        /// The timer that sent the event.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            var handle = this.monitor.GetActiveWindowHandle();
            Console.WriteLine(this.monitor.GetWindowTitle(handle));

            var fileName = this.monitor.GetWindowFileName(handle);
            Console.WriteLine(fileName);

            var matches = this.filterRegex.Match(fileName);
            var builder = new StringBuilder();

            for (var i = 1; i <= matches.Groups.Count; i++)
            {
                if (builder.Length > 0)
                {
                    builder.Append(' ');
                }

                builder.Append(matches.Groups[i].Value);
            }

            Console.WriteLine(builder);
            Console.WriteLine();
        }
    }
}
