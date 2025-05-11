using System.Runtime.Versioning;

namespace COMTRADEInsight
{
    internal static class Program
    {
        /// <summary>
        ///  Главная точка входа в приложение.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainWindowForm());
        }
    }
}