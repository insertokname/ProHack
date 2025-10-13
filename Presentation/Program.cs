using Infrastructure;
using Infrastructure.Database;

namespace Presentation
{
    public static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.


            try
            {
                Database.Load();
            }
            catch
            {
                Database.Tables = new DatabaseTables();
            }
            Database.Save();

            ApplicationConfiguration.Initialize();
            System.Windows.Forms.Application.ThreadException
                += new ThreadExceptionEventHandler(ThreadExceptionEventHandler);
            System.Windows.Forms.Application.Run(new Form1());
        }

        public static event EventHandler<ThreadExceptionEventArgs>? OnThreadException;

        private static void ThreadExceptionEventHandler(object sender, ThreadExceptionEventArgs e)
        {
            OnThreadException?.Invoke(sender, e);
        }
    }
}