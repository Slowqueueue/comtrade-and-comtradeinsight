using ScottPlot;

namespace COMTRADEInsight.Models
{
    /// <summary>
    /// Модель главного окна.
    /// </summary>
    public class MainWindowModel
    {
        #region [ Properties ]

        // Свойства
        /// <summary>
        /// Получает название программы.
        /// </summary>
        public string ApplicationTitle { get; } = "COMTRADE Insight";

        /// <summary>
        /// Получает пользовательскую палитру цветов из библиотеки ScottPlot в формате словаря.
        /// </summary>
        public List<ScottPlot.Color> ColorMap { get; } = 
        [
            Colors.Purple,
            Colors.Green,
            Colors.CornflowerBlue,
            Colors.Orange,
            Colors.Sienna,
            Colors.Lime,
            Colors.Magenta,
            Colors.DarkCyan,
            Colors.MediumPurple,
            Colors.OrangeRed,
        ];

        /// <summary>
        /// Получает список кратных и дольных единиц измерения силы тока.
        /// </summary>
        public List<string> CurrentUnits { get; } =
        [
            "A",
            "mA",
            "µA",
            "nA",
            "kA",
            "MA",
        ];

        /// <summary>
        /// Получает список кратных и дольных единиц измерения напряжения.
        /// </summary>
        public List<string> VoltageUnits { get; } =
        [
            "V",
            "mV",
            "µV",
            "nV",
            "kV",
            "MV",
        ];

        #endregion
    }
}
