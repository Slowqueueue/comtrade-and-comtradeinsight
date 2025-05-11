using COMTRADE.Helpers;
using System.Globalization;
using System.Text;

namespace COMTRADE
{
    /// <summary>
    /// Представляет собой временную метку, соответствующую формату COMTRADE, IEEE Std C37.111-1999.
    /// </summary>
    public struct Timestamp
    {
        #region [ Members ]

        // Поля

        /// <summary>
        /// Временная метка в тактах.
        /// </summary>
        public Ticks Value;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Создает новый экземпляр <see cref="Timestamp"/> из существующей строки с информацией о временной метке.
        /// </summary>
        /// <param name="lineToParse">Строка с информацией о временной метке.</param>
        public Timestamp(string lineToParse)
        {
            string[] parts = lineToParse.Split(':'); // Разделение единиц измерения времени

            double milliseconds = 0.0D;

            // В файлах формата COMTRADE можно встретить несколько разных форматов записи времени. Самое важное отличие - это
            // способ записи миллисекунд, они либо записываются как дробная часть секунд, либо через : после секунд
            if (parts.Length == 4)
            {
                TryParse(parts[^1], out milliseconds); // В формате с разделяющим : последний элемент массива - это миллисекунды
                parts = new[] { parts[0], parts[1], parts[2] }; // Инициализируем еще раз, но без миллисекунд
            }

            TryParse(parts[^1], out double seconds); // В формате с дробной записью последний элемент массива - это секунды

            seconds += milliseconds;

            parts[^1] = seconds.ToString("00.000000", CultureInfo.InvariantCulture);

            lineToParse = RemoveCharacters(string.Join(":", parts), char.IsWhiteSpace); // Теперь запись стандартизирована

            DateTime.TryParseExact(lineToParse, new[]
            {
                "d/M/yyyy,H:mm:ss",
                "d/M/yyyy,H:mm:ss.fff",
                "d/M/yyyy,H:mm:ss.ffffff",
                "M/d/yyyy,H:mm:ss",
                "M/d/yyyy,H:mm:ss.fff",
                "M/d/yyyy,H:mm:ss.ffffff"
            },
            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result);

            Value = result.Ticks;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Преобразует <see cref="Timestamp"/> к строковому формату.
        /// </summary>
        public override string ToString() =>
            //              dd/mm/yyyy,hh:mm:ss.ssssss
            Value.ToString("dd/MM/yyyy,HH:mm:ss.ffffff", CultureInfo.InvariantCulture);

        #endregion

        #region [ Static ]

        // Статические методы
        private static bool TryParse(string s, out double result)
        {
            NumberStyles style = NumberStyles.Float | NumberStyles.AllowThousands;
            return double.TryParse(s, style, CultureInfo.InvariantCulture, out result);
        }

        private static string RemoveCharacters(string? value, Func<char, bool> characterTestFunction)
        {
            Func<char, bool> characterTestFunction2 = characterTestFunction;
            if (characterTestFunction2 == null)
            {
                throw new ArgumentNullException("characterTestFunction");
            }

            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (char item in value.Where((char character) => !characterTestFunction2(character)))
            {
                stringBuilder.Append(item);
            }

            return stringBuilder.ToString();
        }

        #endregion
    }
}
