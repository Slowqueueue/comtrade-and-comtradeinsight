using COMTRADE.Helpers;
using System.Globalization;
using System.Text.Json.Serialization;

namespace COMTRADE
{
    /// <summary>
    /// Представляет собой смещение по UTC, соответствующее формату COMTRADE - формат времени HhMM.
    /// </summary>
    public class TimeOffset
    {
        #region [ Members ]

        // Поля
        private int m_hours; // Часовой компонент смещения по UTC
        private int m_minutes; // Минутный компонент смещения по UTC

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Создает новый экземпляр <see cref="TimeOffset"/>.
        /// </summary>
        public TimeOffset()
        {
        }

        /// <summary>
        /// Создает новый экземпляр <see cref="TimeOffset"/> из существующей строки с информацией о смещении по UTC.
        /// </summary>
        /// <param name="lineToParse">Строка с информацией о смещении по UTC.</param>
        public TimeOffset(string lineToParse)
        {
            bool validFormat;
            int hours = 0, minutes = 0;

            if (string.Compare(lineToParse, "x", StringComparison.OrdinalIgnoreCase) == 0)
            {
                // Значение "x" говорит о том, что смещение не применяется
                NotApplicable = true;
                validFormat = true; // Флаг поднят, если формат времени является допустимым
            }
            else
            {
                // Пример строки: -5h30
                string[] parts = lineToParse.Split('h');

                switch (parts.Length)
                {
                    case 1:
                        validFormat = int.TryParse(lineToParse, NumberStyles.Integer, CultureInfo.InvariantCulture, out hours);
                        break;
                    case 2:
                    {
                        validFormat = int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out hours);

                        if (validFormat)
                            validFormat = int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out minutes);
                        break;
                    }
                    default:
                        validFormat = false;
                        break;
                }
            }

            if (validFormat)
            {
                Hours = hours;
                Minutes = minutes;                
            }
            else
            {
                throw new FormatException($"Недопустимый формат времени в строке с информацией о смещении по UTC - Ожидаемый формат: \"[+/-]HhMM\"{Environment.NewLine}Полученная строка: \"{lineToParse}\"");
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Получает или устанавливает часовой компонент смещения по UTC.
        /// </summary>
        public int Hours
        {
            get => m_hours;
            set
            {
                if (Math.Abs(value) > 23)
                    throw new ArgumentOutOfRangeException(nameof(value), "Максимальное значение для часов: 23.");

                m_hours = value;
            }
        }

        /// <summary>
        /// Получает или устанавливает минутный компонент смещения по UTC.
        /// </summary>
        public int Minutes
        {
            get => m_minutes;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Минуты не могут быть отрицательными.");

                if (value > 59)
                    throw new ArgumentOutOfRangeException(nameof(value), "Максимальное значение для минут: 59.");

                m_minutes = value;
            }
        }

        /// <summary>
        /// Получает или устанавливает флаг, определяющий, применяется ли смещение по UTC.
        /// </summary>
        /// <remarks>
        /// Смещение по UTC не применяется, если свойство имеет значение <c>true</c>.
        /// </remarks>
        public bool NotApplicable { get; set; }

        /// <summary>
        /// Получает общее значение <see cref="TimeOffset"/> в тактах.
        /// </summary>
        [JsonIgnore]
        public long TickOffset
        {
            get
            {
                long hourOffset = m_hours * Ticks.PerHour;
                long minuteOffset = m_minutes * Ticks.PerMinute;
                return hourOffset < 0 ? hourOffset - minuteOffset : hourOffset + minuteOffset;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Преобразует <see cref="TimeOffset"/> к строковому формату.
        /// </summary>
        public override string ToString() => 
            NotApplicable ? "x" : Format($"{Hours}h{Minutes:00}");

        #endregion

        #region [ Static ]

        // Статические методы
        private static string Format(FormattableString formattableString) =>
            formattableString.ToString(CultureInfo.InvariantCulture);

        #endregion
    }
}
