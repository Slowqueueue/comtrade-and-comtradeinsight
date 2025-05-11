using System.Globalization;

namespace COMTRADE
{
    /// <summary>
    /// Представляет собой частоту дискретизации, сопряженную с ее номером последней выборки в соответствии с форматом COMTRADE, IEEE Std C37.111-1999.
    /// </summary>
    public struct SampleRate
    {
        #region [ Members ]

        // Поля
        /// <summary>
        /// Частота дискретизации в герцах (Гц).
        /// </summary>
        public double Rate;

        /// <summary>
        /// Номер последней выборки для частоты дискретизации.
        /// </summary>
        public long EndSample;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Создает новый экземпляр <see cref="SampleRate"/> из существующей строки с параметрами дискретизации.
        /// </summary>
        /// <param name="lineToParse">Строка с параметрами дискретизации</param>
        /// <param name="useRelaxedValidation">Указывает, следует ли использовать нестрогую проверку на количество элементов в строке.</param>
        public SampleRate(string lineToParse, bool useRelaxedValidation = false)
        {
            // samp,endsamp идут в файле через запятую (пример: 1200,40)
            string[] parts = lineToParse.Split(','); // Разделение samp,endsamp

            if (parts.Length < 2 || (!useRelaxedValidation && parts.Length != 2))
                throw new InvalidOperationException($"Непредвиденное количество элементов строки с параметрами дискретизации: {parts.Length} - Ожидаемое количество: 2{Environment.NewLine}Полученная строка: \"{lineToParse}\"");

            Rate = double.Parse(parts[0].Trim(), CultureInfo.InvariantCulture);
            EndSample = long.Parse(parts[1].Trim(), CultureInfo.InvariantCulture);
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Преобразует <see cref="SampleRate"/> к строковому формату.
        /// </summary>
        public override string ToString() => // samp,endsamp
            Format($"{Rate},{EndSample}");

        #endregion

        #region [ Static ]

        // Статические методы
        private static string Format(FormattableString formattableString) =>
            formattableString.ToString(CultureInfo.InvariantCulture);

        #endregion
    }
}
