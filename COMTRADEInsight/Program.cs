using System.Runtime.Versioning;

namespace COMTRADEInsight
{
    internal static class Program
    {
        /// <summary>
        ///  ������� ����� ����� � ����������.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainWindowForm());
        }
    }
}