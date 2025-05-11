using System.Globalization;
using System.Text.Json.Serialization;

namespace COMTRADE
{
    /// <summary>
    /// Представляет собой дискретный канал, существующий в <see cref="Schema"/>.
    /// </summary>
    public class DigitalChannel
    {
        #region [ Members ]

        // Поля
        private string? m_stationName; // Название станции
        private string? m_channelName; // Название канала
        private string? m_phaseID; // Идентификатор сигнала
        private string? m_circuitComponent; // Участок цепи

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Создает новый экземпляр <see cref="DigitalChannel"/>.
        /// </summary>
        /// <param name="version">Версия формата COMTRADE данной схемы.</param>
        public DigitalChannel(int version = 1999)
        {
            Version = version;
        }

        /// <summary>
        /// Создает новый экземпляр <see cref="DigitalChannel"/> из существующей строки с информацией о дискретном канале.
        /// </summary>
        /// <param name="lineToParse">Строка с информацией о дискретном канале.</param>
        /// <param name="version">Версия формата COMTRADE данной схемы.</param>
        /// <param name="useRelaxedValidation">Указывает, следует ли использовать нестрогую проверку на количество элементов в строке.</param>
        public DigitalChannel(string lineToParse, int version = 1999, bool useRelaxedValidation = false)
        {
            // Dn,ch_id,ph,ccbm,y
            string[] parts = lineToParse.Split(','); // Разделение параметров дискретного канала

            Version = version;

            if (parts.Length < 3 || (!useRelaxedValidation && parts.Length != 3 && parts.Length != 5))
                throw new InvalidOperationException($"Непредвиденное количество элементов строки с информацией о дискретном канале: {parts.Length} - Ожидаемое количество: 3 или 5{Environment.NewLine}Полученная строка: \"{lineToParse}\"");

            if (parts.Length >= 5)
            {
                Index = int.Parse(parts[0].Trim(), CultureInfo.InvariantCulture); // Номер канала
                Name = parts[1]; // Название канала
                PhaseID = parts[2]; // Идентификатор сигнала
                CircuitComponent = parts[3]; // Участок цепи
                bool.TryParse(parts[4].Trim(), out bool normalState);
                NormalState = normalState; // Нормальное состояние канала
            }
            else
            {
                Index = int.Parse(parts[0].Trim(), CultureInfo.InvariantCulture);
                Name = parts[1];
                bool.TryParse(parts[2].Trim(), out bool normalState);
                NormalState = normalState;
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Получает или устанавливает номер данного <see cref="DigitalChannel"/>.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Получает или устанавливает название данного <see cref="DigitalChannel"/> в формате название_станции:название_канала.
        /// </summary>
        /// <exception cref="FormatException">Название должно быть в формате название_станции:название_канала.</exception>
        public string? Name
        {
            get => string.IsNullOrWhiteSpace(m_stationName) ? m_channelName : $"{m_stationName}:{m_channelName}";
            set
            {
                string[] parts = value?.Split(':') ?? Array.Empty<string>();

                if (parts.Length == 2)
                {
                    m_stationName = parts[0].Trim();
                    m_channelName = parts[1].Trim();
                }
                else
                {
                    m_stationName = string.Empty;
                    m_channelName = parts[0].Trim();
                }
            }
        }

        /// <summary>
        /// Получает или устанавливает название станции данного <see cref="DigitalChannel"/>.
        /// </summary>
        public string? StationName
        {
            get => m_stationName;
            set => m_stationName = value?.Replace(":", "_").Trim();
        }

        /// <summary>
        /// Получает или устанавливает название канала данного <see cref="DigitalChannel"/>.
        /// </summary>
        public string? ChannelName
        {
            get => m_channelName;
            set => m_channelName = value?.Replace(":", "_").Trim();
        }

        /// <summary>
        /// Получает или устанавливает идентификатор сигнала данного <see cref="DigitalChannel"/>.
        /// </summary>
        public string? PhaseID
        {
            get => m_phaseID;
            set
            {
                m_phaseID = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

                // Хотя формат COMTRADE стандартизирует, что идентификатор сигнала должен быть максимум 2 символа в длину,
                // во многих информационных файлах встречаются 3-символьные идентификаторы дискретных сигналов, например: T10
                if (m_phaseID.Length > 3) //Обрезаем до первых 3 символов
                    m_phaseID = m_phaseID[..3];
            }
        }

        /// <summary>
        /// Получает или устанавливает участок цепи данного <see cref="DigitalChannel"/>.
        /// </summary>
        public string? CircuitComponent
        {
            get => m_circuitComponent;
            set
            {
                m_circuitComponent = !string.IsNullOrWhiteSpace(value) ? value.Trim() : string.Empty;

                if (m_circuitComponent.Length > 64) // Если название слишком длинное, обрезаем до первых 64 символов
                    m_circuitComponent = m_circuitComponent[..64];
            }
        }

        /// <summary>
        /// Получает или устанавливает нормальное состояние данного <see cref="DigitalChannel"/>.
        /// </summary>
        public bool NormalState { get; set; }

        /// <summary>
        /// Получает номер версии формата COMTRADE, IEEE Std C37.111 для данной схемы.
        /// </summary>
        [JsonIgnore]
        public int Version { get; }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Преобразует <see cref="DigitalChannel"/> к строковому формату.
        /// </summary>
        public override string ToString()
        {
            List<string> values;

            if (Version >= 1999)
            {
                // ,ch_id,ph,ccbm,y
                values = new List<string>
                {
                    Index.ToString(CultureInfo.InvariantCulture),
                    Name ?? string.Empty,
                    PhaseID ?? string.Empty,
                    CircuitComponent ?? string.Empty,
                    NormalState ? "1" : "0"
                };
            }
            else
            {
                // Dn,ch_id,y
                values = new List<string>
                {
                    Index.ToString(CultureInfo.InvariantCulture),
                    Name ?? string.Empty,
                    NormalState ? "1" : "0"
                };
            }

            return string.Join(",", values);
        }

        #endregion
    }
}
