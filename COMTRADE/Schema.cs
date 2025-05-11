using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;

namespace COMTRADE
{
    #region [ Enumerations ]

    /// <summary>
    /// Тип файлов для <see cref="Schema"/>.
    /// </summary>
    public enum FileType
    {
        /// <summary>
        /// Тип файлов ASCII.
        /// </summary>
        Ascii,
        /// <summary>
        /// Двоичный тип файлов, размер аналогового канала = uint16.
        /// </summary>
        Binary,
        /// <summary>
        /// Двоичный тип файлов, размер аналогового канала = uint32.
        /// </summary>
        Binary32,
        /// <summary>
        /// Двоичный тип файлов, размер аналогового канала = float32.
        /// </summary>
        Float32
    }

    /// <summary>
    /// Индикатор качества временных меток для <see cref="Schema"/>.
    /// </summary>
    public enum TimeQualityIndicatorCode : byte
    {
        /// <summary>
        /// Время синхронизировано, нормальный режим  работы.
        /// </summary>
        Locked = 0x0,
        /// <summary>
        /// Сбой в работе часов, временные показатели не надежны.
        /// </summary>
        Failure = 0xF,
        /// <summary>
        /// Время не синхронизировано, отклонение в пределах 10^1с.
        /// </summary>
        Unlocked10Seconds = 0xB,
        /// <summary>
        /// Время не синхронизировано, отклонение в пределах 10^0с.
        /// </summary>
        Unlocked1Second = 0xA,
        /// <summary>
        /// Время не синхронизировано, отклонениев пределах 10^-1с.
        /// </summary>
        UnlockedPoint1Seconds = 0x9,
        /// <summary>
        /// Время не синхронизировано, отклонениев пределах 10^-2с.
        /// </summary>
        UnlockedPoint01Seconds = 0x8,
        /// <summary>
        /// Время не синхронизировано, отклонение в пределах 10^-3с.
        /// </summary>
        UnlockedPoint001Seconds = 0x7,
        /// <summary>
        /// Время не синхронизировано, отклонение в пределах 10^-4с.
        /// </summary>
        UnlockedPoint0001Seconds = 0x6,
        /// <summary>
        /// Время не синхронизировано, отклонение в пределах 10^-5с.
        /// </summary>
        UnlockedPoint00001Seconds = 0x5,
        /// <summary>
        /// Время не синхронизировано, отклонение в пределах 10^-6с.
        /// </summary>
        UnlockedPoint000001Seconds = 0x4,
        /// <summary>
        /// Время не синхронизировано, отклонение в пределах 10^-7с.
        /// </summary>
        UnlockedPoint0000001Seconds = 0x3,
        /// <summary>
        /// Время не синхронизировано, отклонение в пределах 10^-8с.
        /// </summary>
        UnlockedPoint00000001Seconds = 0x2,
        /// <summary>
        /// Время не синхронизировано, отклонение в пределах 10^-9с.
        /// </summary>
        UnlockedPoint000000001Seconds = 0x1
    }

    /// <summary>
    /// Индикатор дополнительной секунды для <see cref="Schema"/>.
    /// </summary>
    public enum LeapSecondIndicator : byte
    {
        /// <summary>
        /// Дополнительная секунда не использовалась при записи.
        /// </summary>
        NoLeapSecondAdjustment = 0,
        /// <summary>
        /// Дополнительная секунда была добавлена при записи.
        /// </summary>
        LeapSecondWasAdded = 1,
        /// <summary>
        /// Дополнительная секунда была отнята при записи.
        /// </summary>
        LeapSecondWasSubtracted = 2,
        /// <summary>
        /// Использование дополнительной секунды невозможно.
        /// </summary>
        NoLeapSecondCapacity = 3
    }

    #endregion

    /// <summary>
    /// Представляет собой схему конфигурационного файла формата COMTRADE, IEEE Std C37.111-1999/2013.
    /// </summary>
    public class Schema
    {
        #region [ Members ]

        //Поля
        private double m_nominalFrequency; // Частота сети
        private SampleRate[]? m_sampleRates; // Массив структур, содержащих частоты дискретизации и номера последних выборок для конкретных частот дискретизации

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Создает новый экземпляр <see cref="Schema"/>.
        /// </summary>
        public Schema()
        {
            Version = 1999; // Версия формата COMTRADE
            m_nominalFrequency = 60.0D; // Частота сети
            FileType = FileType.Binary; // Тип файлов
            TimeFactor = 1.0D; // Коэффициент умножения времени
            SampleRates = null; // Массив частот дискретизации. По умолчанию null
        }

        /// <summary>
        /// Создает новый экземпляр <see cref="Schema"/> из имени существующего конфигурационного файла.
        /// </summary>
        /// <param name="fileName">Имя конфигурационного файла для анализа.</param>
        public Schema(string fileName) // По умолчанию useRelaxedValidation = false
            : this(fileName, false)
        {
        }

        /// <summary>
        /// Создает новый экземпляр <see cref="Schema"/> из имени существующего конфигурационного файла и флага нестрогой проверки.
        /// </summary>
        /// <param name="fileName">Имя конфигурационного файла для анализа.</param>
        /// <param name="useRelaxedValidation">Указывает, следует ли использовать нестрогую проверку на количество элементов в строке.</param>
        public Schema(string fileName, bool useRelaxedValidation)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException($"Конфигурационный файл \"{fileName}\" не существует.");

            FileName = fileName;
            IsCombinedFileFormat = HasCFFExtension(FileName); // Флаг поднимается если расширение конфигурационного файла .cff

            string[] lines; // Массив строк конфигурационного файла

            if (IsCombinedFileFormat)
            {
                // Считываем конфигурационную секцию файла
                using StreamReader fileReader = new(FileName);

                List<string> fileLines = new();
                bool firstLine = true; // Флаг поднят, если идет обработка первой строки

                do
                {
                    string? line = fileReader.ReadLine();

                    if (firstLine)
                    {
                        if (!IsFileSectionSeparator(line, out string? sectionType) || sectionType != "CFG")
                            throw new InvalidOperationException($"Непредвиденный разделитель секций комбинированного файла (.cff) - Ожидаемый разделитель: \"--- file type: CFG ---\"{Environment.NewLine}Полученный разделитель: \"{line}\"");

                        firstLine = false;
                        continue;
                    }

                    if (line is null || IsFileSectionSeparator(line))
                        break;

                    fileLines.Add(line);
                }
                while (true);

                lines = fileLines.ToArray();
            }
            else
            {
                lines = File.ReadAllLines(fileName);
            }

            int lineNumber = 0;

            // Анализ строки с названием станции, идентификатором регистратора и версией COMTRADE
            string[] parts = lines[lineNumber++].Split(','); // Разделение названия станции, идентификатора регистратора и версии COMTRADE

            if (parts.Length < 2 || (!useRelaxedValidation && parts.Length != 2 && parts.Length != 3))
                throw new InvalidOperationException($"Непредвиденное количество элементов первой строки конфигурационного файла: {parts.Length} - Ожидаемое количество: 2 или 3{Environment.NewLine}Полученная строка: \"{lines[lineNumber - 1]}\"");

            StationName = parts[0].Trim();
            DeviceID = parts[1].Trim();

            if (parts.Length >= 3 && !string.IsNullOrWhiteSpace(parts[2])) // Если указана версия формата, считываем ее
                Version = int.Parse(parts[2].Trim(), CultureInfo.InvariantCulture);
            else
                Version = 1991; // По умолчанию 1991

            // Анализ строки с информацией о каналах
            parts = lines[lineNumber++].Split(','); // Разделение общего количества каналов, количества аналоговых и количества дискретных каналов

            if (parts.Length < 3 || (!useRelaxedValidation && parts.Length != 3))
                throw new InvalidOperationException($"Непредвиденное количество элементов второй строки конфигурационного файла: {parts.Length} - Ожидаемое количество: 3{Environment.NewLine}Полученная строка: \"{lines[lineNumber - 1]}\"");

            int totalChannels = int.Parse(parts[0].Trim(), CultureInfo.InvariantCulture);
            int totalAnalogChannels = int.Parse(parts[1].Trim().Split('A')[0], CultureInfo.InvariantCulture);
            int totalDigitalChannels = int.Parse(parts[2].Trim().Split('D')[0], CultureInfo.InvariantCulture);

            if (totalChannels != totalAnalogChannels + totalDigitalChannels) // Проверка общего кол-ва каналов
                throw new InvalidOperationException($"Общее количество каналов должно равняться сумме количеств аналоговых и дискретных каналов{Environment.NewLine}Полученная строка: \"{lines[lineNumber - 1]}\"");

            // Сохраняем строки с информацией об аналоговых каналах. Анализ произведем после определения типа файла
            List<string> analogLines = new();

            for (int i = 0; i < totalAnalogChannels; i++) // Сколько аналоговых каналов, столько и добавляем строк
                analogLines.Add(lines[lineNumber++]);

            // Анализ строк с информацией о дискретных каналах
            List<DigitalChannel> digitalChannels = new();

            for (int i = 0; i < totalDigitalChannels; i++) // Сколько дискретных каналов, столько и добавляем в список объектов типа DigitalChannel
                digitalChannels.Add(new DigitalChannel(lines[lineNumber++], Version, useRelaxedValidation));

            DigitalChannels = digitalChannels.ToArray();

            // Анализ строки с частотой сети
            NominalFrequency = double.Parse(lines[lineNumber++], CultureInfo.InvariantCulture);

            // Анализ строки с количеством различных частот дискретизации 
            int totalSampleRates = int.Parse(lines[lineNumber++], CultureInfo.InvariantCulture);

            if (totalSampleRates == 0) // 0 говорит о том, что различные частоты дискретизации не использовались, то есть всё записывалось с одной частотой
                totalSampleRates = 1;

            // Анализ строк с параметрами дискретизации
            List<SampleRate> sampleRates = new();

            for (int i = 0; i < totalSampleRates; i++) // Сколько частот дискретизации, столько и добавляем в список объектов типа SampleRate
                sampleRates.Add(new SampleRate(lines[lineNumber++], useRelaxedValidation));

            SampleRates = sampleRates.ToArray();

            // Анализ строк с отметками даты и времени
            StartTime = new Timestamp(lines[lineNumber++]); // Время начала записи
            TriggerTime = new Timestamp(lines[lineNumber++]); // Время момента запуска

            // Анализ строки с типом файла
            System.Enum.TryParse(lines[lineNumber++], true, out FileType fileType);
            FileType = fileType;

            // Анализ строк с информацией об аналоговых каналах. Делаем это после того, как узнали тип файла для более точного определения минимальных и максимальных величин выборки
            bool targetFloatingPoint = fileType == FileType.Float32;

            AnalogChannels = analogLines.Select(lineToParse => new AnalogChannel(lineToParse, Version, targetFloatingPoint, useRelaxedValidation)).ToArray();

            // Анализ строки с коэффициентом умножения времени
            TimeFactor = lineNumber < lines.Length ? double.Parse(lines[lineNumber++], CultureInfo.InvariantCulture) : 1; // Записываем коэффициент умножения времени, если строка с ним существует, если нет, то TimeFactor = 1

            // Анализ строки с информацией о смещениях по UTC
            if (lineNumber < lines.Length) // Если строка существует
            {
                parts = lines[lineNumber++].Split(','); // Разделяем смещения

                string timeCode = parts[0].Trim(); // Смещение по UTC для записей с временными метками
                string localCode = parts[1].Trim(); // Смещение по UTC для часового пояса регистратора

                if (useRelaxedValidation && timeCode.EndsWith("t")) // Если кончается на t, убираем ее
                    timeCode = timeCode.Substring(0, timeCode.Length - 1);

                if (useRelaxedValidation && localCode.EndsWith("t")) // Если кончается на t, убираем ее
                    localCode = localCode.Substring(0, localCode.Length - 1);

                if (parts.Length > 0)
                    TimeCode = new TimeOffset(timeCode);

                if (parts.Length > 1)
                    LocalCode = new TimeOffset(localCode);
            }

            // Анализ строки с индикаторами временных параметров
            if (lineNumber < lines.Length)
            {
                parts = lines[lineNumber].Split(',');

                if (parts.Length > 0) // Если указан, считываем индикатор качества временных меток
                    TimeQualityIndicatorCode = (TimeQualityIndicatorCode)byte.Parse(parts[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                if (parts.Length > 1) // Если указан, считываем индикатор дополнительной секунды
                    LeapSecondIndicator = (LeapSecondIndicator)byte.Parse(parts[1], CultureInfo.InvariantCulture);
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Получает имя файла, который был использован для создания данной <see cref="Schema"/>, если таковой использовался; в противном случае, <c>null</c>.
        /// </summary>
        public string? FileName { get; }

        /// <summary>
        /// Получает флаг, определяющий, является ли <see cref="FileName"/> файлом комбинированного формата (.cff).
        /// </summary>
        public bool IsCombinedFileFormat { get; }

        /// <summary>
        /// Получает название станции для данной <see cref="Schema"/>.
        /// </summary>
        public string? StationName { get; set; }

        /// <summary>
        /// Получает или устанавливает идентификатор регистратора для данной <see cref="Schema"/>.
        /// </summary>
        public string? DeviceID { get; set; }

        /// <summary>
        /// Получает или устанавливает номер версии формата COMTRADE, IEEE Std C37.111 для данной <see cref="Schema"/>.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Получает общее количество каналов для данной <see cref="Schema"/>.
        /// </summary>
        public int TotalChannels => 
            TotalAnalogChannels + TotalDigitalChannels;

        /// <summary>
        /// Получает количество аналоговых каналов для данной <see cref="Schema"/>.
        /// </summary>
        public int TotalAnalogChannels => 
            AnalogChannels?.Length ?? 0;

        /// <summary>
        /// Получает количество дискретных каналов для данной <see cref="Schema"/>.
        /// </summary>
        public int TotalDigitalChannels => 
            DigitalChannels?.Length ?? 0;

        /// <summary>
        /// Получает или устанавливает аналоговые каналы для данной <see cref="Schema"/>.
        /// </summary>
        public AnalogChannel[]? AnalogChannels { get; set; }

        /// <summary>
        /// Получает или устанавливает дискретные каналы для данной <see cref="Schema"/>.
        /// </summary>
        public DigitalChannel[]? DigitalChannels { get; set; }

        /// <summary>
        /// Получает или устанавливает частоту сети для данной <see cref="Schema"/>.
        /// </summary>
        public double NominalFrequency
        {
            get => m_nominalFrequency;
            set
            {
                m_nominalFrequency = value;

                // Обновление частоты сети для каждого аналогового канала
                if (AnalogChannels is not null)
                {
                    foreach (AnalogChannel analogChannel in AnalogChannels)
                        analogChannel.NominalFrequency = m_nominalFrequency;
                }
            }
        }

        /// <summary>
        /// Получает или устанавливает частоты дискретизации для данной <see cref="Schema"/>. Обычно запись ведется на одной статичной частоте дискретизации, так что чаще всего частота будет одна.
        /// </summary>
        public SampleRate[]? SampleRates
        {
            get => m_sampleRates;
            set => m_sampleRates = value ?? new[] { new SampleRate { Rate = 0, EndSample = 1 } }; // Если value - null, то ставится заглушка
        }

        /// <summary>
        /// Получает общее количество частот дискретизации для данной <see cref="Schema"/>.
        /// </summary>
        public int TotalSampleRates => 
            m_sampleRates is null ? 0 : m_sampleRates.Length == 1 && m_sampleRates[0].Rate == 0.0D ? 0 : m_sampleRates.Length; // Заглушка не считается частотой дискретизации

        /// <summary>
        /// Получает общее количество выборок, то есть количество строк с временными метками, согласно структурам с параметрами дискретизации.
        /// </summary>
        public long TotalSamples => 
            m_sampleRates?.Max(sampleRate => sampleRate.EndSample) ?? 0L;

        /// <summary>
        /// Получает общее количество канальных значений, то есть <c>TotalChannels * TotalSamples</c>.
        /// </summary>
        public long TotalChannelValues => 
            TotalChannels * TotalSamples;

        /// <summary>
        /// Получает или устанавливает время начала записи для данной <see cref="Schema"/>.
        /// </summary>
        public Timestamp StartTime { get; set; }

        /// <summary>
        /// Получает или устанавливает время момента запуска для данной <see cref="Schema"/>.
        /// </summary>
        public Timestamp TriggerTime { get; set; }

        /// <summary>
        /// Получает или устанавливает тип файлов для данной <see cref="Schema"/>.
        /// </summary>
        public FileType FileType { get; set; }

        /// <summary>
        /// Получает или устанавливает коэффициент умножения времени для данной <see cref="Schema"/>.
        /// </summary>
        public double TimeFactor { get; set; }

        /// <summary>
        /// Получает или устанавливает смещение по UTC для записей с временными метками - формат HhMM.
        /// </summary>
        public TimeOffset TimeCode { get; set; } = new("0h00"); //По умолчанию "0h00"

        /// <summary>
        /// Получает или устанавливает смещение по UTC для часового пояса регистратора, значение "x" говорит о том, что смещение не применяется - формат HhMM.
        /// </summary>
        public TimeOffset LocalCode { get; set; } = new("x"); //По умолчанию "x"

        /// <summary>
        /// Получает или устанавливает индикатор качества временных меток для записанного набора данных.
        /// </summary>
        public TimeQualityIndicatorCode TimeQualityIndicatorCode { get; set; } = TimeQualityIndicatorCode.Locked; // По умолчанию "Время синхронизировано, нормальный режим работы"

        /// <summary>
        /// Получает или устанавливает индикатор дополнительной секунды для записанного набора данных.
        /// </summary>
        public LeapSecondIndicator LeapSecondIndicator { get; set; } = LeapSecondIndicator.NoLeapSecondAdjustment; // По умолчанию "Дополнительная секунда не использовалась при записи"

        /// <summary>
        /// Получает общее количество машинных слов (16 бит), необходимых для записи заданного числа дискретных значений, если тип файлов - двоичный.
        /// </summary>
        public int DigitalWords => 
            DigitalChannels is null ? 0 : (int)Math.Ceiling(DigitalChannels.Length / 16.0D); // Пример: 8 дискретных каналов. Ceiling(8/16) = 1, то есть для записи 8 значений достаточно одного слова

        /// <summary>
        /// Вычисляет размер записи в байтах, если тип файлов двоичный, а размер аналогового канала = uint16.
        /// </summary>
        public int BinaryRecordLength => 
            AnalogChannels is null ? 0 : 8 + 2 * AnalogChannels.Length + 2 * DigitalWords;

        /// <summary>
        /// Вычисляет размер записи в байтах, если тип файлов двоичный, а размер аналогового канала = uint32.
        /// </summary>
        public int Binary32RecordLength => 
            AnalogChannels is null ? 0 : 8 + 4 * AnalogChannels.Length + 2 * DigitalWords;

        /// <summary>
        /// Вычисляет размер записи в байтах, если тип файлов двоичный, а размер аналогового канала = float32.
        /// </summary>
        public int Float32RecordLength => 
            AnalogChannels is null ? 0 : 8 + 4 * AnalogChannels.Length + 2 * DigitalWords;

        /// <summary>
        /// Получает содержимое конфигурационного файла для данной <see cref="Schema"/>.
        /// </summary>
        [JsonIgnore]
        public string FileContent
        {
            get
            {
                StringBuilder fileContent = new();

                void appendLine(FormattableString line)
                {
                    // Стандартное свойство платформы .NET "Environment.NewLine" на некоторых ОС может производить лишь перевод строки,
                    // а формат COMTRADE стандартизирует то, что конец строки должен маркироваться как переводом строки, так и возвратом каретки.
                    fileContent.Append(line.ToString(CultureInfo.InvariantCulture));
                    fileContent.Append(Writer.CRLF);
                }

                // Запись строки с названием, обозначением станциии и версией COMTRADE
                appendLine($"{StationName},{DeviceID}{(Version >= 1999 ? $",{Version}" : string.Empty)}");

                // Запись строки с информацией о каналах
                appendLine($"{TotalChannels},{TotalAnalogChannels}A,{TotalDigitalChannels}D");

                // Запись строк с информацией об аналоговых каналах
                for (int i = 0; i < TotalAnalogChannels; i++)
                    appendLine($"{AnalogChannels![i]}");

                // Запись строк с информацией о дискретных каналах
                for (int i = 0; i < TotalDigitalChannels; i++)
                    appendLine($"{DigitalChannels![i]}");

                // Запись строки с частотой сети
                appendLine($"{NominalFrequency}");

                // Запись строки с количеством различных частот дискретизации
                appendLine($"{TotalSampleRates}");

                int totalSampleRates = TotalSampleRates;

                if (totalSampleRates == 0) // 0 говорит о том, что различные частоты дискретизации не использовались, то есть всё записывалось с одной частотой
                    totalSampleRates = 1;

                // Запись строк с параметрами дискретизации
                for (int i = 0; i < totalSampleRates; i++)
                    appendLine($"{SampleRates![i]}");

                // Запись строк с отметками даты и времени
                appendLine($"{StartTime}");
                appendLine($"{TriggerTime}");

                //Запись строки с типом файла
                appendLine($"{FileType.ToString().ToUpper()}");

                // Запись строки с коэффициентом умножения времени
                if (Version >= 1999)
                    appendLine($"{TimeFactor}");

                // Запись строк с информацией о смещениях по UTC и индикаторами временных параметров
                if (Version >= 2013)
                {
                    appendLine($"{TimeCode},{LocalCode}");
                    appendLine($"{(byte)TimeQualityIndicatorCode:X},{(byte)LeapSecondIndicator}");
                }

                return fileContent.ToString();
            }
        }

        #endregion

        #region [ Static ]

        // Статические методы
        internal static bool HasCFFExtension(string? fileName) =>
            fileName is not null && string.Equals(Path.GetExtension(fileName), ".cff", StringComparison.OrdinalIgnoreCase);

        internal static bool IsFileSectionSeparator(string? line) =>
            IsFileSectionSeparator(line, out _, out _);

        internal static bool IsFileSectionSeparator(string? line, out string? sectionType) =>
            IsFileSectionSeparator(line, out sectionType, out _);

        internal static bool IsFileSectionSeparator(string? line, out string? sectionType, out long byteCount)
        {
            if (line?.Trim().StartsWith("---") ?? false)
            {
                string[] parts = line.Replace("---", string.Empty).Trim().Split(':'); // Первая часть - file type, вторая - CFG (пример)

                if (parts.Length >= 2 && string.Equals(parts[0].Trim(), "file type", StringComparison.OrdinalIgnoreCase))
                {
                    sectionType = parts[1].Trim().ToUpperInvariant();

                    if (parts.Length > 2 && sectionType == "DAT BINARY") // Информационная секция файла двоичного типа может содержать размер секции в байтах
                        long.TryParse(parts[2].Trim(), out byteCount);
                    else
                        byteCount = default;

                    return true;
                }
            }

            sectionType = default;
            byteCount = default;
            return false;
        }

        #endregion
    }
}
