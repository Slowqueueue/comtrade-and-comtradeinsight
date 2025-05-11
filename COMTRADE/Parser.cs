using COMTRADE.Helpers;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace COMTRADE
{
    /// <summary>
    /// Синтаксический анализатор файлов формата COMTRADE.
    /// </summary>
    public class Parser : IDisposable
    {
        #region [ Members ]

        // Поля
        private Schema? m_schema; // Схема конфигурационного файла
        private string? m_fileName; // Имя информационного файла
        private FileStream[]? m_fileStreams; // Потоки информационных файлов
        private StreamReader[]? m_fileReaders; // Объекты для считывания символов из потоков
        private int m_streamIndex; // Номер потока
        private double[]? m_primaryValues; // Первичные значения
        private double[]? m_secondaryValues; // Вторичные значения
        private uint m_initialSample; // Номер первой выборки в наборе данных
        private bool m_disposed; // Флаг поднят, если выполнялось освобождение неуправляемых ресурсов

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Освобождает неуправляемые ресурсы до того, как объект <see cref="Parser"/> перейдет под управление <see cref="GC"/>.
        /// </summary>
        ~Parser() => Dispose(false);

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Получает или устанавливает схему конфигурационного файла формата COMTRADE для данного <see cref="Parser"/>.
        /// </summary>
        /// <remarks>
        /// После установки свойства любое значение <see cref="COMTRADE.Schema.FileName"/> будет использоваться
        /// для первичной инициализации свойства <see cref="FileName"/>, если будет обнаружено
        /// существование связанного со схемой информационного файла расширения ".cff", ".dat" или ".d00".
        /// </remarks>
        public Schema? Schema
        {
            get => m_schema;
            set
            {
                m_schema = value;

                if (m_schema is null)
                {
                    Values = null; // Значения, приведенные к единицам измерения
                }
                else
                {
                    if (m_schema.TotalChannels > 0)
                    {
                        Values = new double[m_schema.TotalChannels];
                    }
                    else
                        throw new InvalidOperationException("Недопустимая схема: общее количество каналов, указанных в схеме, равно нулю.");

                    if (m_schema.TotalSampleRates == 0)
                        InferTimeFromSampleRates = false; // Спуск флага означает, что время не будет вычисляться по частотам дискретизации

                    // Если имя информационного файла еще не установлено, предположим, что работа идет с файлом расширения .cff,
                    // в противном случае попытаемся найти информационный файл в той же директории и с тем же именем, что
                    // и конфигурационный файл, но с раширением .dat или .d00
                    if (string.IsNullOrWhiteSpace(m_fileName) && !string.IsNullOrWhiteSpace(m_schema.FileName))
                    {
                        IsCombinedFileFormat = m_schema.IsCombinedFileFormat; // Флаг поднимается если расширение конфигурационного файла .cff

                        if (IsCombinedFileFormat)
                        {
                            m_fileName = m_schema.FileName;
                        }
                        else
                        {
                            string directory = Path.GetDirectoryName(m_schema.FileName)!;
                            string rootFileName = Path.GetFileNameWithoutExtension(m_schema.FileName);
                            string dataFile1 = Path.Combine(directory, $"{rootFileName}.dat");
                            string dataFile2 = Path.Combine(directory, $"{rootFileName}.d00");

                            if (File.Exists(dataFile1))
                                m_fileName = dataFile1;
                            else if (File.Exists(dataFile2))
                                m_fileName = dataFile2;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Получает или устанавливает имя информационного файла формата COMTRADE. Если в наборе больше одного информационного файла, это свойство должно быть установлено на имя первого файла в наборе.
        /// </summary>
        public string? FileName
        {
            get => m_fileName;
            set
            {
                m_fileName = value;
                IsCombinedFileFormat = Schema.HasCFFExtension(m_fileName);
            }
        }

        /// <summary>
        /// Получает флаг, определяющий, является ли <see cref="FileName"/> файлом комбинированного формата (.cff).
        /// </summary>
        public bool IsCombinedFileFormat { get; private set; }

        /// <summary>
        /// Получает количество байтов, проанализированных из файла комбинированного формата (.cff), имеющего двоичный тип.
        /// </summary>
        public long BinaryByteCount { get; private set; }

        /// <summary>
        /// Получает или устанавливает флаг, определяющий, будет ли время вычисляться по частотам дискретизации.
        /// </summary>
        public bool InferTimeFromSampleRates { get; set; } = true;

        /// <summary>
        /// Получает временную метку текущей выборки.
        /// </summary>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Получает значения текущей выборки, приведенные к единицам измерения.
        /// </summary>
        public double[]? Values { get; private set; }

        /// <summary>
        /// Получает значения текущей выборки, при этом вторичные значения аналоговых каналов приводятся к первичным.
        /// </summary>

        public double[]? PrimaryValues
        {
            get
            {
                if (m_primaryValues is null && Values is not null && m_schema is not null)
                {
                    m_primaryValues = new double[Values.Length];

                    for (int i = 0; i < m_primaryValues.Length; i++)
                    {
                        double value = Values[i];

                        if (i < m_schema.AnalogChannels?.Length) // Приведение выполняется только для аналоговых каналов
                        {
                            if (char.ToUpper(m_schema.AnalogChannels[i].ScalingIdentifier) == 'S')
                                value *= m_schema.AnalogChannels[i].PrimaryRatio / m_schema.AnalogChannels[i].SecondaryRatio;
                        }

                        m_primaryValues[i] = value;
                    }
                }

                return m_primaryValues;
            }
        }

        /// <summary>
        /// Получает значения текущей выборки, при этом первичные значения аналоговых каналов приводятся к вторичным.
        /// </summary>
        public double[]? SecondaryValues
        {
            get
            {
                if (m_secondaryValues is null && Values is not null && m_schema is not null)
                {
                    m_secondaryValues = new double[Values.Length];

                    for (int i = 0; i < m_secondaryValues.Length; i++)
                    {
                        double value = Values[i];

                        if (i < m_schema.AnalogChannels?.Length) // Приведение выполняется только для аналоговых каналов
                        {
                            if (char.ToUpper(m_schema.AnalogChannels[i].ScalingIdentifier) == 'P')
                                value *= m_schema.AnalogChannels[i].SecondaryRatio / m_schema.AnalogChannels[i].PrimaryRatio;
                        }

                        m_secondaryValues[i] = value;
                    }
                }

                return m_secondaryValues;
            }
        }

        /// <summary>
        /// Получает или устанавливает флаг, определяющий, должны ли схемы, содержащие смещение по UTC для временных меток, применять корректировку, чтобы временные метки анализировались как UTC.
        /// </summary>
        public bool AdjustToUTC { get; set; } = true;

        /// <summary>
        /// Получает или устанавливает единицу измерения угла для всех аналоговых каналов, которые являются углами.
        /// </summary>
        /// <remarks>
        /// После установки свойства, любой встреченный при анализе угол будет приведен к указанной единице измерения.
        /// </remarks>
        public AngleUnit? TargetAngleUnit { get; set; }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Освобождает все ресурсы, используемые объектом <see cref="Parser"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Освобождает неуправляемые ресурсы, используемые объектом <see cref="Parser"/>, и опционально освобождает управляемые ресурсы.
        /// </summary>
        /// <param name="disposing"><c>true</c> для освобождения управляемых и неуправляемых ресурсов; <c>false</c> - для освобождения только неуправляемых ресурсов.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (m_disposed)
                return;

            try
            {
                if (disposing)
                    CloseFiles();
            }
            finally
            {
                m_disposed = true; // Предотвращение повторного освобождения
            }
        }

        /// <summary>
        /// Открывает все потоки информационных файлов формата COMTRADE.
        /// </summary>
        public void OpenFiles()
        {
            if (string.IsNullOrWhiteSpace(FileName))
                throw new InvalidOperationException("Имя первого информационного файла формата COMTRADE не указано, открытие файлов невозможно.");

            if (!File.Exists(FileName))
                throw new FileNotFoundException($"Указанный информационный файл формата COMTRADE \"{FileName}\" не найден, открытие файлов невозможно.");

            string[] fileNames;

            if (IsCombinedFileFormat)
            {
                fileNames = new[] { FileName };
            }
            else
            {
                // Получение всех информационных файлов в наборе
                const string FileRegex = @"(?:\.dat|\.d\d\d)$"; // .dat или .dXX, где XX - это две цифры
                string directory = Path.GetDirectoryName(FileName)!;
                string fileNamePattern = $"{Path.GetFileNameWithoutExtension(FileName)}.d*";

                fileNames = GetFileList(Path.Combine(directory, fileNamePattern))
                    .Where(fileName => Regex.IsMatch(fileName, FileRegex, RegexOptions.IgnoreCase))
                    .OrderBy(fileName => fileName, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
            }

            // Создание нового файлового потока для каждого файла
            m_fileStreams = new FileStream[fileNames.Length];

            for (int i = 0; i < fileNames.Length; i++)
                m_fileStreams[i] = new FileStream(fileNames[i], FileMode.Open, FileAccess.Read, FileShare.Read);

            m_streamIndex = 0;
            m_initialSample = uint.MaxValue; // Использование максимального значения в качестве маркера неинициализированного значения

            if (IsCombinedFileFormat)
            {
                // Переход к информационной секции файла (объект StreamReader уничтожать не нужно - это приведет к уничтожению основного потока)
                StreamReader fileReader = new(m_fileStreams[0]);
                Encoding utf8 = new UTF8Encoding(false);
                long position = 0;

                do
                {
                    string? line = fileReader.ReadLine();

                    if (line is null)
                        break;

                        position += utf8.GetBytes(line).Length + 2; // CR и LF - это 2 управляющих символа, поэтому добавляем 2 байта

                    if (Schema.IsFileSectionSeparator(line, out string? sectionType, out long byteCount) && (sectionType?.StartsWith("DAT") ?? false))
                    {
                        BinaryByteCount = byteCount;
                        m_fileStreams[0].Position = position; // Сохраняем позицию чтения в потоке
                        break;
                    }
                }
                while (true);
            }
        }

        /// <summary>
        /// Закрывает все потоки информационных файлов формата COMTRADE.
        /// </summary>
        public void CloseFiles()
        {
            if (m_fileStreams is not null)
            {
                foreach (FileStream fileStream in m_fileStreams)
                    fileStream.Dispose();
            }

            m_fileStreams = null;
        }

        /// <summary>
        /// Считывает следующую выборку формата COMTRADE.
        /// </summary>
        /// <returns><c>true</c>, если считывание прошло успешно; в противном случае, <c>false</c>, если был достигнут конец набор данных.</returns>
        public bool ReadNext()
        {
            if (m_fileStreams is null)
                throw new InvalidOperationException("Информационные файлы формата COMTRADE не открыты, считывание следующей выборки невозможно.");

            if (m_schema is null)
                throw new InvalidOperationException("Схема формата COMTRADE не определена, чтение записей невозможно.");

            if (m_streamIndex > m_fileStreams.Length)
                throw new EndOfStreamException("Все выборки формата COMTRADE прочитаны, дальнейшее чтение невозможно.");

            m_primaryValues = null;
            m_secondaryValues = null;

            return m_schema.FileType switch
            {
                FileType.Ascii => ReadNextAscii(),
                FileType.Binary => ReadNextBinary(),
                FileType.Binary32 => ReadNextBinary32(),
                FileType.Float32 => ReadNextFloat32(),
                _ => false,
            };
        }

        // Обработка чтения файлов типа ASCII
        private bool ReadNextAscii()
        {
            // Для файлов типа ASCII, мы оборачиваем файловые потоки файловыми считывателями
            if (m_fileReaders is null)
            {
                m_fileReaders = new StreamReader[m_fileStreams!.Length];

                for (int i = 0; i < m_fileStreams.Length; i++)
                    m_fileReaders[i] = new StreamReader(m_fileStreams[i]);
            }

            // Считываем следующую строку со значениями выборки
            StreamReader reader = m_fileReaders[m_streamIndex];
            string? line = reader.ReadLine();
            string[]? elems = line?.Split(','); // Разделение номера выборки, времени и значений каналов

            // Проверяем, дошли ли мы до конца файла
            if (elems is null || elems.Length != Values!.Length + 2) // Количество каналов + 2 (номер выборки и время)
            {
                if (reader.EndOfStream)
                    return ReadNextFile();

                throw new InvalidOperationException("Схема формата COMTRADE не соответствует количеству элементов, найденных в информационном файле типа ASCII.");
            }

            // Анализ номера текущей выборки
            uint sample = uint.Parse(elems[0], CultureInfo.InvariantCulture);

            // Захват номера первой выборки в наборе данных - для случаев, когда отсчет номеров выборки начинается не с нуля
            if (m_initialSample == uint.MaxValue)
                m_initialSample = sample;

            // Смещение на номер первой выборки в наборе данных
            sample -= m_initialSample;

            // Получение временной метки текущей выборки
            Timestamp = DateTime.MinValue;

            // Если частоты дисретизации определены, то это предпочтительный метод получения временной метки
            if (InferTimeFromSampleRates && m_schema!.SampleRates?.Length > 0)
            {
                // Находим частоту дискретизации текущей выборки
                SampleRate sampleRate = m_schema.SampleRates.LastOrDefault(sr => sample <= sr.EndSample);

                if (sampleRate.Rate > 0.0D)
                    Timestamp = new DateTime(Ticks.FromSeconds(1.0D / sampleRate.Rate * sample) + m_schema.StartTime.Value); // time = 1 / frequency
            }

            // Другой метод - это добавление времени, прошедшего от начала осциллограммы к стартовому времени
            if (Timestamp == DateTime.MinValue)
                Timestamp = new DateTime(Ticks.FromMicroseconds(double.Parse(elems[1], CultureInfo.InvariantCulture) * m_schema!.TimeFactor) + m_schema.StartTime.Value);

            // Применение смещения по UTC для временной метки в целях восстановления часового пояса
            if (AdjustToUTC)
            {
                TimeOffset offset = m_schema!.TimeCode;
                Timestamp = new DateTime(Timestamp.Ticks + offset.TickOffset, DateTimeKind.Utc);
            }

            // Анализ всех значений каналов в выборке
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i] = double.Parse(elems[i + 2], CultureInfo.InvariantCulture); // Смещение на 2 элемента строки, так как в начале идет номер выборки и время

                if (i < m_schema!.AnalogChannels?.Length) // Приведение выполняется только для аналоговых каналов
                    Values[i] = AdjustValue(Values[i], i);
            }

            return true;
        }

        // Обработка чтения файлов двоичного типа, размер аналогового канала = uint16
        private bool ReadNextBinary()
        {
            FileStream currentFile = m_fileStreams![m_streamIndex];
            int recordLength = m_schema!.BinaryRecordLength;
            byte[] buffer = new byte[recordLength];

            // Считываем следующую выборку из файла
            int bytesRead = currentFile.Read(buffer, 0, recordLength);

            // Проверяем, дошли ли мы до конца файла
            if (bytesRead == 0)
                return ReadNextFile();

            if (bytesRead == recordLength)
            {
                // Считываем временную метку и получаем позицию в буфере на конец считывания
                int index = ReadTimestamp(buffer); 

                // Считываем значения аналоговых каналов и получаем позицию в буфере на конец считывания
                index = ReadAnalogValues(buffer, index, ReadInt16, 2); // ReadInt16 - это ссылка на метод, преобразующий 2 байта из буфера в значение int16, а затем в double
                
                // Считываем значения дискретных каналов
                ReadDigitalValues(buffer, index);
            }
            else
            {
                if (buffer[bytesRead - 1] == 0x1A) // Проверка на символ конца двоичного файла
                    return ReadNextFile();

                throw new InvalidOperationException($"Не удалось прочитать определенное в схеме количество байтов для выборки из двоичного файла c размером аналогового канала = uint16 формата COMTRADE - возможно, несоответствие схемы и информационного файла или повреждение файла. Позиция сбоя = {currentFile.Position:N0}");
            }

            return true;
        }


        // Обработка чтения файлов двоичного типа, размер аналогового канала = uint32
        private bool ReadNextBinary32()
        {
            FileStream currentFile = m_fileStreams![m_streamIndex];
            int recordLength = m_schema!.Binary32RecordLength;
            byte[] buffer = new byte[recordLength];

            // Считываем следующую выборку из файла
            int bytesRead = currentFile.Read(buffer, 0, recordLength);

            // Проверяем, дошли ли мы до конца файла
            if (bytesRead == 0)
                return ReadNextFile();

            if (bytesRead == recordLength)
            {
                // Считываем временную метку и получаем позицию в буфере на конец считывания
                int index = ReadTimestamp(buffer);

                // Считываем значения аналоговых каналов и получаем позицию в буфере на конец считывания
                index = ReadAnalogValues(buffer, index, ReadInt32, 4); // ReadInt32 - это ссылка на метод, преобразующий 4 байта из буфера в значение int32, а затем в double

                // Считываем значения дискретных каналов
                ReadDigitalValues(buffer, index);
            }
            else
            {
                if (buffer[bytesRead - 1] == 0x1A) // Проверка на символ конца двоичного файла
                    return ReadNextFile();

                throw new InvalidOperationException($"Не удалось прочитать определенное в схеме количество байтов для выборки из двоичного файла c размером аналогового канала = uint32 формата COMTRADE - возможно, несоответствие схемы и информационного файла или повреждение файла. Позиция сбоя = {currentFile.Position:N0}");
            }

            return true;
        }

        // Обработка чтения файлов двоичного типа, размер аналогового канала = float32
        private bool ReadNextFloat32()
        {
            FileStream currentFile = m_fileStreams![m_streamIndex];
            int recordLength = m_schema!.Float32RecordLength;
            byte[] buffer = new byte[recordLength];

            // Считываем следующую выборку из файла
            int bytesRead = currentFile.Read(buffer, 0, recordLength);

            // Проверяем, дошли ли мы до конца файла
            if (bytesRead == 0)
                return ReadNextFile();

            if (bytesRead == recordLength)
            {
                // Считываем временную метку и получаем позицию в буфере на конец считывания
                int index = ReadTimestamp(buffer);

                // Считываем значения аналоговых каналов и получаем позицию в буфере на конец считывания
                index = ReadAnalogValues(buffer, index, ReadFloat, 4); // ReadFloat - это ссылка на метод, преобразующий 4 байта из буфера в значение float32, а затем в double

                // Считываем значения дискретных каналов
                ReadDigitalValues(buffer, index);
            }
            else
            {
                if (buffer[bytesRead - 1] == 0x1A) // Проверка на символ конца двоичного файла
                    return ReadNextFile();

                throw new InvalidOperationException($"Не удалось прочитать определенное в схеме количество байтов для выборки из двоичного файла c размером аналогового канала = float32 формата COMTRADE - возможно, несоответствие схемы и информационного файла или повреждение файла. Позиция сбоя = {currentFile.Position:N0}");
            }

            return true;
        }

        private bool ReadNextFile()
        {
            m_streamIndex++;

            // Проверяем, есть ли еще информационный файл для чтения
            return m_streamIndex < m_fileStreams!.Length && ReadNext(); // ReadNext() вызывается, только если выполнилось первое условие
        }

        private int ReadTimestamp(byte[] buffer)
        {
            int index = 0; // Позиция в буфере

            // Считываем номер выборки
            uint sample = LittleEndian.ToUInt32(buffer, index); // Преобразуем 4 байта из буфера в uint32
            index += 4; // Так как uint32 - это 4 байта

            // Захват номера первой выборки в наборе данных - для случаев, когда отсчет номеров выборки начинается не с нуля
            if (m_initialSample == uint.MaxValue)
                m_initialSample = sample;

            // Смещение на номер первой выборки в наборе данных
            sample -= m_initialSample;

            // Получение временной метки текущей выборки
            Timestamp = DateTime.MinValue;

            // Если частоты дискретизации определены, то это предпочтительный метод получения временной метки
            if (InferTimeFromSampleRates && m_schema!.SampleRates?.Length > 0)
            {
                // Находим частоту дискретизации текущей выборки
                SampleRate sampleRate = m_schema.SampleRates.LastOrDefault(sr => sample <= sr.EndSample);

                if (sampleRate.Rate > 0.0D)
                    Timestamp = new DateTime(Ticks.FromSeconds(1.0D / sampleRate.Rate * sample) + m_schema.StartTime.Value); // time = 1 / frequency
            }

            // Считываем количество микросекунд из файла, принцип такой же как и с номером выборки
            uint microseconds = LittleEndian.ToUInt32(buffer, index);
            index += 4;

            // Другой метод - это добавление времени, прошедшего от начала осциллограммы к стартовому времени
            if (Timestamp == DateTime.MinValue)
                Timestamp = new DateTime(Ticks.FromMicroseconds(microseconds * m_schema!.TimeFactor) + m_schema.StartTime.Value);

            // Применение смещения по UTC для временной метки в целях восстановления часового пояса
            if (AdjustToUTC)
            {
                TimeOffset offset = m_schema!.TimeCode;
                Timestamp = new DateTime(Timestamp.Ticks + offset.TickOffset, DateTimeKind.Utc);
            }

            return index;
        }

        private int ReadAnalogValues(byte[] buffer, int index, Func<byte[], int, double> byteConverter, int byteSize)
        {
            // Анализ всех значений аналоговых каналов в выборке
            for (int i = 0; i < m_schema!.AnalogChannels?.Length; i++)
            {
                // Считываем следующее аналоговое значение
                Values![i] = AdjustValue(byteConverter(buffer, index), i); // byteConverter - это делегат, преобразующий байты в значение double
                index += byteSize; // Добавляем размер аналогового значения в байтах к позиции в буфере
            }

            return index;
        }

        private double AdjustValue(double value, int channelIndex)
        {
            AnalogChannel? channel = m_schema!.AnalogChannels?[channelIndex];

            if (channel is null)
                return value;

            value = value * channel.Multiplier + channel.Adder; // Выполняем приведение

            if (channel.SignalKind == SignalKind.Angle && TargetAngleUnit.HasValue) 
                value = Angle.ConvertFrom(value, channel.AngleUnit).ConvertTo(TargetAngleUnit.Value); // Приведение фазового угла

            return value;
        }

        private void ReadDigitalValues(byte[] buffer, int index)
        {
            int valueIndex = m_schema!.AnalogChannels?.Length ?? 0; // Номер первого дискретного канала в выборке

            // Анализ всех значений дискретных каналов в выборке
            for (int i = 0; i < m_schema.DigitalWords; i++)
            {
                // Считываем следующее машинное слово
                ushort digitalWord = LittleEndian.ToUInt16(buffer, index); // Преобразуем 2 байта из буфера в uint16
                index += 2; //Так как uint16 - это 2 байта

                // Распределение каждого бита машинного слова по следующим 16 дискретным значениям
                for (int j = 0; j < 16 && valueIndex < Values!.Length; j++, valueIndex++)
                    Values[valueIndex] = ((digitalWord & (1u << j)) != 0) ? 1.0D : 0.0D; // Проверка установки бита
            }
        }


        #endregion

        #region [ Static ]

        // Статические методы
        private static double ReadInt16(byte[] buffer, int startIndex) =>
            LittleEndian.ToInt16(buffer, startIndex);

        private static double ReadInt32(byte[] buffer, int startIndex) =>
            LittleEndian.ToInt32(buffer, startIndex);

        private static double ReadFloat(byte[] buffer, int startIndex) =>
            LittleEndian.ToSingle(buffer, startIndex);

        private static string[] GetFileList(string path)
        {
            string directory = Path.GetDirectoryName(path)!;
            string filePattern = Path.GetFileName(path);
            SearchOption options = SearchOption.TopDirectoryOnly;

            // Если шаблон не указан, берем все файлы
            if (string.IsNullOrEmpty(filePattern))
                filePattern = "*.*";

            if (new DirectoryInfo(directory).Name == "*")
            {
                // Шаблон используется для указания опции включения подкаталогов
                options = SearchOption.AllDirectories;
                directory = directory.Remove(directory.LastIndexOf("*", StringComparison.OrdinalIgnoreCase));
            }

            return Directory.GetFiles(directory, filePattern, options);
        }

        #endregion
    }
}
