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
        public Dictionary<int, ScottPlot.Color> ColorMap { get; } = new Dictionary<int, ScottPlot.Color>()
        {
            { 0, Colors.Purple },
            { 1, Colors.Green },
            { 2, Colors.CornflowerBlue },
            { 3, Colors.Orange },
            { 4, Colors.Sienna },
            { 5, Colors.Lime },
            { 6, Colors.Magenta },
            { 7, Colors.DarkCyan },
            { 8, Colors.MediumPurple },
            { 9, Colors.OrangeRed },
        };

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
