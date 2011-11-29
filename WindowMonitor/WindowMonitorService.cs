namespace WindowMonitor
{
    using System;
    using System.ServiceProcess;
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
        /// Initializes a new instance of the <see cref="WindowMonitorService"/> class. 
        /// </summary>
        public WindowMonitorService()
        {
            this.InitializeComponent();
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
            Console.WriteLine(this.monitor.GetWindowFileName(handle));
            Console.WriteLine();
        }
    }
}
