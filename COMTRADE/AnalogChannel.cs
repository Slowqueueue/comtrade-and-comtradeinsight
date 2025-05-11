using COMTRADE.Helpers;
using System.Globalization;
using System.Text.Json.Serialization;

namespace COMTRADE
{
    #region [ Enumerations ]

    /// <summary>
    /// Система координат.
    /// </summary>
    [Serializable]
    public enum CoordinateFormat : byte
    {
        /// <summary>
        /// Прямоугольная система координат.
        /// </summary>
        Rectangular,
        /// <summary>
        /// Полярная система координат.
        /// </summary>
        Polar
    }

    /// <summary>
    /// Основные виды сигналов для стандартных измерений, которые отражают именно вид (поведение) сигнала, а не явный тип, в отличие от
    /// перечисления <see cref="SignalType"/>, которое дополнительно определяет явный тип сигнала (например, напряжение или силу тока для угла).
    /// </summary>
    [Serializable]
    public enum SignalKind
    {
        /// <summary>
        /// Фазовый угол.
        /// </summary>
        Angle,
        /// <summary>
        /// Амплитуда сигнала.
        /// </summary>
        Magnitude,
        /// <summary>
        /// Частота.
        /// </summary>
        Frequency,
        /// <summary>
        /// Скорость изменения частоты (dF/dt).
        /// </summary>
        DfDt,
        /// <summary>
        /// Флаги состояний.
        /// </summary>
        Status,
        /// <summary>
        /// Дискретное значение.
        /// </summary>
        Digital,
        /// <summary>
        /// Аналоговое значение.
        /// </summary>
        Analog,
        /// <summary>
        /// Вычисленное значение.
        /// </summary>
        Calculation,
        /// <summary>
        /// Статистическое значение.
        /// </summary>
        Statistic,
        /// <summary>
        /// Сигнальное значение.
        /// </summary>
        Alarm,
        /// <summary>
        /// Флаги качества.
        /// </summary>
        Quality,
        /// <summary>
        /// Неустановленный вид сигнала.
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Основные типы сигналов для стандартных измерений, которые отражают явный тип сигнала, а не абстрактный вид, в отличие от
    /// перечисления <see cref="SignalKind"/>, которое определяет лишь поведение сигнала (например, амплитуду или угол).
    /// </summary>
    [Serializable]
    public enum SignalType
    {
        /// <summary>
        /// Сила тока.
        /// </summary>
        IPHM = 1,
        /// <summary>
        /// Фазовый угол силы тока.
        /// </summary>
        IPHA = 2,
        /// <summary>
        /// Напряжение.
        /// </summary>
        VPHM = 3,
        /// <summary>
        /// Фазовый угол напряжения.
        /// </summary>
        VPHA = 4,
        /// <summary>
        /// Частота.
        /// </summary>
        FREQ = 5,
        /// <summary>
        /// Скорость изменения частоты (dF/dt).
        /// </summary>
        DFDT = 6,
        /// <summary>
        /// Аналоговое значение.
        /// </summary>
        ALOG = 7,
        /// <summary>
        /// Флаги состояний.
        /// </summary>
        FLAG = 8,
        /// <summary>
        /// Дискретное значение.
        /// </summary>
        DIGI = 9,
        /// <summary>
        /// Вычисленное значение.
        /// </summary>
        CALC = 10,
        /// <summary>
        /// Статистическое значение.
        /// </summary>
        STAT = 11,
        /// <summary>
        /// Сигнальное значение.
        /// </summary>
        ALRM = 12,
        /// <summary>
        /// Флаги качества.
        /// </summary>
        QUAL = 13,
        /// <summary>
        /// Неустановленный вид сигнала.
        /// </summary>
        NONE = -1
    }

    /// <summary>
    /// Величины сигнала.
    /// </summary>
    [Serializable]
    public enum SignalQuantity : byte
    {
        /// <summary>
        /// Напряжение.
        /// </summary>
        Voltage,
        /// <summary>
        /// Сила тока.
        /// </summary>
        Current
    }

    #endregion

    /// <summary>
    /// Представляет собой аналоговый канал, существующий в <see cref="Schema"/>.
    /// </summary>
    public class AnalogChannel
    {
        #region [ Members ]

        // Константы

        /// <summary>
        /// Множитель по умолчанию для значений силы тока.
        /// </summary>
        public const double DefaultCurrentMagnitudeMultiplier = 0.05D;

        /// <summary>
        /// Множитель по умолчанию для значений напряжения.
        /// </summary>
        public const double DefaultVoltageMagnitudeMultiplier = 5.77362D;

        /// <summary>
        /// Множитель по умолчанию для значений фазового угла.
        /// </summary>
        public const double DefaultPhaseAngleMultiplier = 1.0E-4D;

        /// <summary>
        /// Множитель по умолчанию для значений частоты.
        /// </summary>
        public const double DefaultFrequencyMultiplier = 0.001D;

        /// <summary>
        /// Множитель по умолчанию для значений скорости изменения частоты (dF/dt).
        /// </summary>
        public const double DefaultDfDtMultiplier = 0.01D;

        /// <summary>
        /// Множитель по умолчанию для аналогового значения.
        /// </summary>
        public const double DefaultAnalogMultiplier = 0.04D;

        // Поля
        private readonly bool m_targetFloatingPoint; // Флаг поднят, если тип файла Float32
        private string? m_stationName; // Название станции
        private string? m_channelName; // Название канала
        private char m_phaseDesignation; // Маркировка фазы
        private SignalKind m_signalKind; // Вид сигнала
        private double m_nominalFrequency; // Частота сети
        private string? m_circuitComponent; // Участок цепи
        private string? m_units; // Единица измерения
        private AngleUnit? m_angleUnit; // Единица измерения угла
        private char m_scalingIdentifier; // Идентификатор первичной (p) или вторичной (s) величины коэффициента трансформации

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Создает новый экземпляр <see cref="AnalogChannel"/>.
        /// </summary>
        /// <param name="version">Версия формата COMTRADE данной схемы.</param>
        /// <param name="targetFloatingPoint">Определяет, является ли тип файла типом с плавающей точкой (Float32).</param>
        public AnalogChannel(int version = 1999, bool targetFloatingPoint = false)
        {
            Version = version;
            m_targetFloatingPoint = targetFloatingPoint;
            m_phaseDesignation = char.MinValue;
            m_signalKind = SignalKind.Analog;
            CoordinateFormat = CoordinateFormat.Polar; // Система координат
            Multiplier = targetFloatingPoint ? 1.0D : DefaultAnalogMultiplier; // Множитель значений (a)
            Adder = 0.0D; // Добавочное значение (b)
            m_nominalFrequency = 60.0D;
            MinValue = targetFloatingPoint ? float.MinValue : -99999; // Минимальная величина для выборок канала
            MaxValue = targetFloatingPoint ? float.MaxValue : 99998; // Максимальная величина для выборок канала
            PrimaryRatio = 1.0D; // Первичная величина коэффициента трансформации
            SecondaryRatio = 1.0D; // Вторичная величина коэффициента трансформации
            m_scalingIdentifier = 'P'; // Идентификатор первичной(p) или вторичной(s) величины коэффициента трансформации
        }

        /// <summary>
        /// Создает новый экземпляр <see cref="AnalogChannel"/> из существующей строки с информацией об аналогом канале.
        /// </summary>
        /// <param name="lineToParse">Строка с информацией об аналоговом канале.</param>
        /// <param name="version">Версия формата COMTRADE данной схемы.</param>
        /// <param name="targetFloatingPoint">Определяет, является ли тип файла типом с плавающей точкой (Float32).</param>
        /// <param name="useRelaxedValidation">Указывает, следует ли использовать нестрогую проверку на количество элементов в строке.</param>
        public AnalogChannel(string lineToParse, int version = 1999, bool targetFloatingPoint = false, bool useRelaxedValidation = false)
        {
            // An,ch_id,ph,ccbm,uu,a,b,skew,min,max,primary,secondary,PS
            string[] parts = lineToParse.Split(','); // Разделение параметров аналогового канала

            Version = version;
            m_targetFloatingPoint = targetFloatingPoint;

            if (parts.Length < 10 || !useRelaxedValidation && parts.Length != 10 && parts.Length != 13)
                throw new InvalidOperationException($"Непредвиденное количество элементов строки с информацией об аналоговом канале: {parts.Length} - Ожидаемое количество: 10 или 13{Environment.NewLine}Полученная строка: \"{lineToParse}\"");

            Index = int.Parse(parts[0].Trim(), CultureInfo.InvariantCulture); // Номер канала
            Name = parts[1]; // Название канала
            Units = parts[4]; // Единица измерения
            PhaseID = parts[2]; // Идентификатор сигнала
            CircuitComponent = parts[3]; // Участок цепи
            Multiplier = double.Parse(parts[5].Trim(), CultureInfo.InvariantCulture);
            Adder = double.Parse(parts[6].Trim(), CultureInfo.InvariantCulture);
            Skew = double.Parse(parts[7].Trim(), CultureInfo.InvariantCulture); // Сдвиг времени с начала отсчета
            MinValue = double.Parse(parts[8].Trim(), CultureInfo.InvariantCulture);
            MaxValue = double.Parse(parts[9].Trim(), CultureInfo.InvariantCulture);

            if (parts.Length >= 13)
            {
                PrimaryRatio = double.Parse(parts[10].Trim(), CultureInfo.InvariantCulture); // Первичная величина коэффициента трансформации
                SecondaryRatio = double.Parse(parts[11].Trim(), CultureInfo.InvariantCulture); // Вторичная величина коэффициента трансформации
                ScalingIdentifier = parts[12].Trim()[0]; // Идентификатор первичной(p) или вторичной(s) величины коэффициента трансформации
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Получает или устанавливает номер данного <see cref="AnalogChannel"/>.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Получает или устанавливает название данного <see cref="AnalogChannel"/> в формате название_станции:название_канала.
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
        /// Получает или устанавливает название станции данного <see cref="AnalogChannel"/>.
        /// </summary>
        public string? StationName
        {
            get => m_stationName;
            set => m_stationName = value?.Replace(":", "_").Trim();
        }

        /// <summary>
        /// Получает или устанавливает название канала данного <see cref="AnalogChannel"/>.
        /// </summary>
        public string? ChannelName
        {
            get => m_channelName;
            set => m_channelName = value?.Replace(":", "_").Trim();
        }

        /// <summary>
        /// Получает или устанавливает идентификатор сигнала данного <see cref="AnalogChannel"/>.
        /// </summary>
        public string PhaseID
        {
            get
            {
                switch (m_signalKind)
                {
                    case SignalKind.Magnitude: // Амплитуда сигнала (сила тока, либо напряжение)
                        if (m_phaseDesignation != char.MinValue)
                        {
                            if (CoordinateFormat == CoordinateFormat.Rectangular)
                                return m_phaseDesignation + "r";

                            return m_phaseDesignation + "m";
                        }
                        break;
                    case SignalKind.Angle: // Фазовый угол
                        if (m_phaseDesignation != char.MinValue)
                        {
                            if (CoordinateFormat == CoordinateFormat.Rectangular)
                                return m_phaseDesignation + "i";

                            return m_phaseDesignation + "a";
                        }
                        break;
                    case SignalKind.Frequency: // Частота
                        return "F";
                    case SignalKind.DfDt: // Скорость изменения частоты
                        return "df";
                }

                return PhaseDesignation; // Возвращаем маркировку фазы, если остальные кейсы не подходят
            }
            set
            {
                value = value.Trim();

                if (string.IsNullOrEmpty(value)) // Если строка пустая, задаем параметры как в конструкторе по умолчанию
                {
                    m_phaseDesignation = char.MinValue;
                    SignalKind = SignalKind.Analog;
                    CoordinateFormat = CoordinateFormat.Polar;
                }
                else // Каждому идентификатору соответствующие параметры
                {
                    if (string.Compare(value, "F", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        m_phaseDesignation = char.MinValue;
                        SignalKind = SignalKind.Frequency;
                        CoordinateFormat = CoordinateFormat.Polar;
                    }
                    else if (string.Compare(value, "df", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        m_phaseDesignation = char.MinValue;
                        SignalKind = SignalKind.DfDt;
                        CoordinateFormat = CoordinateFormat.Polar;
                    }
                    else if (value.Length > 1) // Если указаны и маркировка фазы и идентификатор
                    {
                        PhaseDesignation = value[0].ToString(); // Маркировка фазы
                        char component = char.ToLowerInvariant(value[1]); // Идентификатор

                        switch (component)
                        {
                            case 'r':
                                SignalKind = SignalKind.Magnitude;
                                CoordinateFormat = CoordinateFormat.Rectangular;
                                break;
                            case 'i':
                                SignalKind = SignalKind.Angle;
                                CoordinateFormat = CoordinateFormat.Rectangular;
                                break;
                            case 'm':
                                SignalKind = SignalKind.Magnitude;
                                CoordinateFormat = CoordinateFormat.Polar;
                                break;
                            case 'a':
                                SignalKind = SignalKind.Angle;
                                CoordinateFormat = CoordinateFormat.Polar;
                                break;
                        }
                    }
                    else // Если неизвестный идентификатор
                    {
                        PhaseDesignation = value[0].ToString();
                        SignalKind = SignalKind.Analog;
                        CoordinateFormat = CoordinateFormat.Polar;
                    }
                }
            }
        }

        /// <summary>
        /// Получает или устанавливает маркировку фазы данного <see cref="AnalogChannel"/>.
        /// </summary>
        /// <exception cref="ArgumentException">Значение не подходит под определение маркировки фазы.</exception>
        public string PhaseDesignation
        {
            get
            {
                string? phaseDesignation = m_phaseDesignation.ToString();
                if (string.IsNullOrEmpty(phaseDesignation))
                    return string.Empty;

                int nullPos = phaseDesignation.IndexOf('\0');

                return nullPos > 1 ? phaseDesignation[..nullPos] : phaseDesignation;
            }
            set
            {
                value = value.Trim();

                if (string.IsNullOrEmpty(value))
                {
                    m_phaseDesignation = char.MinValue;
                }
                else
                {
                    char phaseDesignation = char.ToUpper(value[0]);

                    // Преобразование разных стандартов под стандарт ANSI/IEEE
                    m_phaseDesignation = phaseDesignation switch
                    {
                        'A' or 'R' or '1' => 'A',
                        'B' or 'S' or '2' => 'B',
                        'C' or 'T' or '3' => 'C',
                        'P' or '+' => 'P',
                        'N' or '-' => '-',
                        'Z' or '0' => '0',
                        _ => char.MinValue,
                    };
                }
            }
        }

        /// <summary>
        /// Получает или устанавливает величину сигнала данного <see cref="AnalogChannel"/>.
        /// </summary>
        public SignalQuantity SignalQuantity { get; set; }

        /// <summary>
        /// Получает или устанавливает частоту сети данного <see cref="AnalogChannel"/>.
        /// </summary>
        public double NominalFrequency
        {
            get => m_nominalFrequency;
            set
            {
                m_nominalFrequency = value;

                if (m_signalKind == SignalKind.Frequency)
                    Adder = m_nominalFrequency;
            }
        }

        /// <summary>
        /// Получает или устанавливает вид сигнала данного <see cref="AnalogChannel"/>.
        /// </summary>
        /// <exception cref="ArgumentException">Значение не подходит под определение вида аналогового сигнала.</exception>
        public SignalKind SignalKind
        {
            get => m_signalKind;
            set
            {
                if (s_validAnalogSignalKinds.BinarySearch(value) < 0)
                    throw new ArgumentException(value + " не подходит под определение подходящего вида аналогового сигнала.");

                m_signalKind = value;

                if (m_targetFloatingPoint) // Если тип файла Float32
                {
                    switch (m_signalKind)
                    {
                        case SignalKind.Angle:
                            Angle minAngle = new(-Math.PI);
                            Angle maxAngle = new(Math.PI);
                            MinValue = minAngle.ConvertTo(AngleUnit);
                            MaxValue = maxAngle.ConvertTo(AngleUnit);
                            break;
                        case SignalKind.Frequency:
                            MinValue = m_nominalFrequency - 4.0D;
                            MaxValue = m_nominalFrequency + 4.0D;
                            break;
                    }

                    return;
                }

                switch (m_signalKind) // Установка множителя значений (a) и добавочного значения (b) в зависимости от вида сигнала
                {
                    case SignalKind.Angle:
                        Multiplier = DefaultPhaseAngleMultiplier;
                        Adder = 0.0D;
                        break;
                    case SignalKind.Magnitude:
                        if (SignalQuantity == SignalQuantity.Current)
                            Multiplier = DefaultCurrentMagnitudeMultiplier;
                        else
                            Multiplier = DefaultVoltageMagnitudeMultiplier;
                        Adder = 0.0D;
                        break;
                    case SignalKind.Frequency:
                        Multiplier = DefaultFrequencyMultiplier;
                        Adder = m_nominalFrequency;
                        break;
                    case SignalKind.DfDt:
                        Multiplier = DefaultDfDtMultiplier;
                        Adder = 0.0D;
                        break;
                    case SignalKind.Status:
                    case SignalKind.Digital:
                        Multiplier = 1.0D;
                        Adder = 0.0D;
                        break;
                    default:
                        Multiplier = DefaultAnalogMultiplier;
                        Adder = 0.0D;
                        break;
                }
            }
        }

        /// <summary>
        /// Получает или устанавливает систему координат данного <see cref="AnalogChannel"/>.
        /// </summary>
        public CoordinateFormat CoordinateFormat { get; set; }

        /// <summary>
        /// Получает или устанавливает участок цепи данного <see cref="AnalogChannel"/>.
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
        /// Получает или устанавливает единицу измерения данного <see cref="AnalogChannel"/>.
        /// </summary>
        public string? Units
        {
            get => m_units;
            set
            {
                m_units = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

                if (m_units.Length > 32) // Если название слишком длинное, обрезаем до первых 32 символов
                    m_units = m_units[..32];

                m_angleUnit = null; // Сбрасываем единицу измерения угла
            }
        }

        /// <summary>
        /// Получает или устанавливает множитель значений данного <see cref="AnalogChannel"/>.
        /// </summary>
        public double Multiplier { get; set; }

        /// <summary>
        /// Получает или устанавливает добавочное значение данного <see cref="AnalogChannel"/>.
        /// </summary>
        public double Adder { get; set; }

        /// <summary>
        /// Получает или устанавливает сдвиг времени с начала отсчета данного <see cref="AnalogChannel"/>.
        /// </summary>
        public double Skew { get; set; }

        /// <summary>
        /// Получает или устанавливает немасштабированную минимальную величину для выборок данного <see cref="AnalogChannel"/>.
        /// </summary>
        public double MinValue { get; set; }

        /// <summary>
        /// Получает или устанавливает немасштабированную максимальную величину для выборок данного <see cref="AnalogChannel"/>.
        /// </summary>
        public double MaxValue { get; set; }

        /// <summary>
        /// Получает или устанавливает первичную величину коэффициента трансформации тока или напряжения данного <see cref="AnalogChannel"/>.
        /// </summary>
        public double PrimaryRatio { get; set; }

        /// <summary>
        /// Получает или устанавливает вторичную величину коэффициента трансформации тока или напряжения данного <see cref="AnalogChannel"/>.
        /// </summary>
        public double SecondaryRatio { get; set; }

        /// <summary>
        /// Получает или устанавливает идентификатор первичной(p) или вторичной(s) величины коэффициента трансформации данного <see cref="AnalogChannel"/>.
        /// </summary>
        public char ScalingIdentifier
        {
            get => m_scalingIdentifier;
            set
            {
                value = char.ToUpper(value);

                if (value != 'P' && value != 'S')
                    throw new ArgumentException(value + " не подходит под определение подходящего идентификатора первичной или вторичной величины коэффициента трансформации - Ожидаемый идентификатор \'P\' или \'S\'.");

                m_scalingIdentifier = value;
            }
        }

        /// <summary>
        /// Получает <see cref="AngleUnit"/> вытекающий из <see cref="Units"/>, если применимо к типу канала.
        /// </summary>
        public AngleUnit AngleUnit => 
            m_angleUnit ?? (m_angleUnit = GetAngleUnit(Units ?? "Degrees")).Value; // Если m_angleUnit - null, то вызываем метод GetAngleUnit. Если Units - null, то метод принимает градусы

        /// <summary>
        /// Получает номер версии формата COMTRADE, IEEE Std C37.111 для данной схемы.
        /// </summary>
        [JsonIgnore]
        public int Version { get; }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Преобразует <see cref="AnalogChannel"/> к строковому формату.
        /// </summary>
        public override string ToString()
        {
            // An,ch_id,ph,ccbm,uu,a,b,skew,min,max
            List<string> values = new()
            {
                Index.ToString(CultureInfo.InvariantCulture),
                Name ?? string.Empty,
                PhaseID,
                CircuitComponent ?? string.Empty,
                Units ?? "Degrees",
                Multiplier.ToString(CultureInfo.InvariantCulture),
                Adder.ToString(CultureInfo.InvariantCulture),
                Skew.ToString(CultureInfo.InvariantCulture),
                MinValue.ToString(CultureInfo.InvariantCulture),
                MaxValue.ToString(CultureInfo.InvariantCulture)
            };

            // ...,primary,secondary,PS
            if (Version >= 1999)
            {
                values.Add(PrimaryRatio.ToString(CultureInfo.InvariantCulture));
                values.Add(SecondaryRatio.ToString(CultureInfo.InvariantCulture));
                values.Add(ScalingIdentifier.ToString());
            }

            return string.Join(",", values);
        }

        #endregion

        #region [ Static ]

        // Статические поля
        private static readonly List<SignalKind> s_validAnalogSignalKinds; // Список подходящих видов аналогового сигнала

        // Статический конструктор
        static AnalogChannel()
        {
            s_validAnalogSignalKinds = new List<SignalKind>(new[]
            { 
                SignalKind.Analog, 
                SignalKind.Angle, 
                SignalKind.Calculation, 
                SignalKind.DfDt, 
                SignalKind.Frequency, 
                SignalKind.Magnitude, 
                SignalKind.Statistic
            });

            s_validAnalogSignalKinds.Sort();
        }

        // Статические методы

        // Попытка проанализировать строку, содержащую единицу измерения угла, как значение перечесления AngleUnit
        private static AngleUnit GetAngleUnit(string units)
        {
            if (!Enum.TryParse(units, true, out AngleUnit angleUnit))
            {
                // Проверяем строку на самые распространенные единицы измерения угла
                if (units.StartsWith("deg", StringComparison.OrdinalIgnoreCase))
                    angleUnit = AngleUnit.Degrees;
                else if (units.StartsWith("grad", StringComparison.OrdinalIgnoreCase) || units.StartsWith("gon", StringComparison.OrdinalIgnoreCase))
                    angleUnit = AngleUnit.Grads;
                else if (units.StartsWith("arcm", StringComparison.OrdinalIgnoreCase) || units.StartsWith("min", StringComparison.OrdinalIgnoreCase) || units.StartsWith("moa", StringComparison.OrdinalIgnoreCase))
                    angleUnit = AngleUnit.ArcMinutes;
                else if (units.StartsWith("arcs", StringComparison.OrdinalIgnoreCase) || units.StartsWith("sec", StringComparison.OrdinalIgnoreCase))
                    angleUnit = AngleUnit.ArcSeconds;
                else if (units.StartsWith("ang", StringComparison.OrdinalIgnoreCase) || units.StartsWith("mil", StringComparison.OrdinalIgnoreCase))
                    angleUnit = AngleUnit.AngularMil;
                else // Радианы
                    angleUnit = AngleUnit.Radians;
            }

            return angleUnit;
        }

        #endregion
    }
}
