using COMTRADE.Helpers;
using System.Globalization;
using System.Text;

namespace COMTRADE
{
    /// <summary>
    /// Генератор конфигурационных и информационных файлов формата COMTRADE.
    /// </summary>
    public static class Writer
    {
        /// <summary>
        /// Определяет максимальный размер файла для текущего процесса записи файлов формата COMTRADE.
        /// </summary>
        /// <remarks>
        /// Значение составляет 256ТБ, произвольно выбранный максимальный размер файла.
        /// </remarks>
        public const long MaxFileSize = 281474976710656L;

        private static readonly string s_maxByteCountString = new('0', MaxFileSize.ToString(CultureInfo.InvariantCulture).Length);

        /// <summary>
        /// Определяет максимальный номер последней выборки информационного файла COMTRADE.
        /// </summary>
        public const long MaxEndSample = 9999999999L;

        /// <summary>
        /// Определяет константу, содержащую символы возврата каретки и перевода строки, то eсть, <c>"\r\n"</c>.
        /// </summary>
        /// <remarks>
        /// Стандартное свойство платформы .NET "Environment.NewLine" на некоторых ОС может производить лишь перевод строки, то есть, <c>"\n"</c>,
        /// а формат COMTRADE стандартизирует то, что конец строки должен маркироваться как переводом строки, так и возвратом каретки, то есть, <c>"\r\n"</c>.
        /// </remarks>
        public const string CRLF = "\r\n";

        /// <summary>
        /// Создает новую <see cref="Schema"/> конфигурационного файла формата COMTRADE.
        /// </summary>
        /// <param name="metadata">Записи <see cref="ChannelMetadata"/> для схемы.</param>
        /// <param name="stationName">Название станции для схемы.</param>
        /// <param name="deviceID">Идентификатор регистратора для схемы.</param>
        /// <param name="dataStartTime">Время начала записи.</param>
        /// <param name="sampleCount">Общее количество выборок, то есть общее количество строк информационного файла.</param>
        /// <param name="version">Номер версии формата COMTRADE для схемы - по умолчанию 1999.</param>
        /// <param name="fileType">Определяет тип файлов для схемы.</param>
        /// <param name="timeFactor">Коэффициент умножения времени для схемы - по умолчанию 1.</param>
        /// <param name="samplingRate">Желаемая частота дискретизации - по умолчанию 30 Гц.</param>
        /// <param name="nominalFrequency">Частота сети - по умолчанию 60 Гц.</param>
        /// <param name="includeFracSecDefinition">Указывает, следует ли вести запись дискретных значений FRACSEC - по умолчанию <c>true</c>.</param>
        /// <returns>Новая <see cref="Schema"/> конфигурационного файла формата COMTRADE.</returns>
        /// <remarks>
        /// <para>
        /// Эта функция в первую очередь предназначена для создания схем конфигурационных файлов на основе данных синхрофазоров
        /// (см. приложение H: Schema for Phasor Data Using the COMTRADE File Standard в стандарте IEEE C37.111-2010),
        /// однако для некоторых нужд может возникнуть необходимость в ручном создании объекта схемы конфигурационного файла COMTRADE.
        /// Вы можете вызвать свойство <see cref="Schema.FileContent"/>, чтобы получить строку с содержимым конфигурационного файла,
        /// а затем записать ее в файл, и таким образом создать конфигурационный файл.
        /// </para>
        /// <para>
        /// Линейные коэффициенты приведения для аналоговых каналов, то есть, множители значений и добавочные значения,
        /// будут установлены на приемлемые значения в зависимости от типа сигнала канала. Их следует корректировать
        /// по мере необходимости, исходя из фактического диапазона значений канала. Обратите внимание,
        /// что для <see cref="FileType.Float32"/> множитель значений будет равен <c>1.0</c>,
        /// а добавочное значение - <c>0.0</c> для всех аналоговых значений.
        /// </para>
        /// </remarks>
        public static Schema CreateSchema(IEnumerable<ChannelMetadata> metadata, string stationName, string deviceID, Ticks dataStartTime, long sampleCount, int version = 1999, FileType fileType = FileType.Binary, double timeFactor = 1.0D, double samplingRate = 30.0D, double nominalFrequency = 60.0D, bool includeFracSecDefinition = true)
        {
            Schema schema = new()
            {
                StationName = stationName,
                DeviceID = deviceID,
                Version = version
            };

            SampleRate samplingFrequency = new()
            {
                Rate = samplingRate,
                EndSample = sampleCount
            };

            schema.SampleRates = new[] { samplingFrequency };

            Timestamp startTime;
            startTime.Value = dataStartTime;
            schema.StartTime = startTime;
            schema.TriggerTime = startTime;

            schema.FileType = fileType;
            schema.TimeFactor = timeFactor;

            List<AnalogChannel> analogChannels = new();
            List<DigitalChannel> digitalChannels = new();
            bool targetFloatingPoint = fileType == FileType.Float32;

            int analogIndex = 1; // Номер аналогового канала
            int digitalIndex = 1; // Номер дискретного канала

            if (includeFracSecDefinition)
            {
                // Добавление дискретных индикаторов качества временных меток по умолчанию для машинного слова FRACSEC стандарта IEEE C37.118.
                // Обратите внимание, что эти флаги, как определено в приложении H стандарта IEEE C37.111-2010, предполагают, что весь процесс
                // экспортирования данных происходил с одного регистратора-источника. Это довольно грубое предположение, поскольку данные
                // могут быть экспортированы из предыдущего набора данных для любого количества точек измерения, которые могли быть получены
                // от любого количества регистраторов с различными значениями FRACSEC. Несмотря на это, существует только одно определение
                // FRACSEC, и, если оно используется, оно должно быть первым набором дискретных каналов в конфигурации формата COMTRADE.

                // Первые 8 дискретных каналов отведены под флаги качества временных меток, TQ - Time Quality
                for (int i = 0; i < 4; i++)
                {
                    digitalChannels.Add(new DigitalChannel(schema.Version)
                    {
                        Index = digitalIndex,
                        Name = "TQ_CNT" + i,
                        PhaseID = "T" + digitalIndex++
                    });
                }

                digitalChannels.Add(new DigitalChannel(schema.Version)
                {
                    Index = digitalIndex,
                    Name = "TQ_LSPND",
                    PhaseID = "T" + digitalIndex++
                });


                digitalChannels.Add(new DigitalChannel(schema.Version)
                {
                    Index = digitalIndex,
                    Name = "TQ_LSOCC",
                    PhaseID = "T" + digitalIndex++
                });

                digitalChannels.Add(new DigitalChannel(schema.Version)
                {
                    Index = digitalIndex,
                    Name = "TQ_LSDIR",
                    PhaseID = "T" + digitalIndex++
                });

                digitalChannels.Add(new DigitalChannel(schema.Version)
                {
                    Index = digitalIndex,
                    Name = "TQ_RSV",
                    PhaseID = "T" + digitalIndex++
                });

                for (int i = 1; i < 9; i++) // Следующие 8 дискретных каналов зарезервированы под будущее использование и для сохранения 16-битной структуры машинного слова
                {
                    digitalChannels.Add(new DigitalChannel(schema.Version)
                    {
                        Index = digitalIndex,
                        Name = "RESV" + i,
                        PhaseID = "T" + digitalIndex++
                    });
                }
            }

            // Добавление метаданных для выбранных точек измерения, отсортированных следующим образом: сначала аналоговые значения, затем флаги состояний, затем дискретные значения, затем флаги качества
            foreach (ChannelMetadata record in metadata.OrderBy(m => m, ChannelMetadataSorter.Default)) // Сортировка ведется по типу сигнала канала
            {
                if (record.IsDigital)
                {
                    switch (record.SignalType)
                    {
                        case SignalType.FLAG: // Флаги состояний
                            // Добавление специальных дискретных каналов - флагов состояний синхрофазора
                            int statusIndex = 0;

                            for (int i = 1; i < 5; i++)
                            {
                                digitalChannels.Add(new DigitalChannel(schema.Version)
                                {
                                    Index = digitalIndex++,
                                    Name = record.Name + ":TRG" + i,
                                    PhaseID = "S" + statusIndex++.ToString("X")
                                });
                            }

                            for (int i = 1; i < 3; i++)
                            {
                                digitalChannels.Add(new DigitalChannel(schema.Version)
                                {
                                    Index = digitalIndex++,
                                    Name = record.Name + ":UNLK" + i,
                                    PhaseID = "S" + statusIndex++.ToString("X")
                                });
                            }

                            for (int i = 1; i < 5; i++)
                            {
                                digitalChannels.Add(new DigitalChannel(schema.Version)
                                {
                                    Index = digitalIndex++,
                                    Name = record.Name + ":SEC" + i,
                                    PhaseID = "S" + statusIndex++.ToString("X")
                                });
                            }

                            digitalChannels.Add(new DigitalChannel(schema.Version)
                            {
                                Index = digitalIndex++,
                                Name = record.Name + ":CFGCH",
                                PhaseID = "S" + statusIndex++.ToString("X")
                            });

                            digitalChannels.Add(new DigitalChannel(schema.Version)
                            {
                                Index = digitalIndex++,
                                Name = record.Name + ":PMUTR",
                                PhaseID = "S" + statusIndex++.ToString("X")
                            });

                            digitalChannels.Add(new DigitalChannel(schema.Version)
                            {
                                Index = digitalIndex++,
                                Name = record.Name + ":SORT",
                                PhaseID = "S" + statusIndex++.ToString("X")
                            });

                            digitalChannels.Add(new DigitalChannel(schema.Version)
                            {
                                Index = digitalIndex++,
                                Name = record.Name + ":SYNC",
                                PhaseID = "S" + statusIndex++.ToString("X")
                            });

                            digitalChannels.Add(new DigitalChannel(schema.Version)
                            {
                                Index = digitalIndex++,
                                Name = record.Name + ":PMUERR",
                                PhaseID = "S" + statusIndex++.ToString("X")
                            });

                            digitalChannels.Add(new DigitalChannel(schema.Version)
                            {
                                Index = digitalIndex++,
                                Name = record.Name + ":DTVLD",
                                PhaseID = "S" + statusIndex.ToString("X")
                            });
                            break;
                        default:
                            // Каждый дискретный канал синхрофазора представляет собой 16 бит
                            for (int i = 0; i < 16; i++)
                            {
                                digitalChannels.Add(new DigitalChannel(schema.Version)
                                {
                                    Index = digitalIndex++,
                                    Name = record.Name,
                                    PhaseID = "B" + i.ToString("X"),
                                    CircuitComponent = record.CircuitComponent
                                });
                            }
                            break;
                    }
                }
                else
                {
                    switch (record.SignalType)
                    {
                        case SignalType.IPHM: // Сила тока
                            analogChannels.Add(new AnalogChannel(schema.Version, targetFloatingPoint)
                            {
                                Index = analogIndex++,
                                Name = record.Name,
                                Units = record.Units ?? "A",
                                PhaseID = "Pm",
                                CircuitComponent = record.CircuitComponent,
                                Multiplier = targetFloatingPoint ? 1.0D : AnalogChannel.DefaultCurrentMagnitudeMultiplier
                            });
                            break;
                        case SignalType.VPHM: // Напряжение
                            analogChannels.Add(new AnalogChannel(schema.Version, targetFloatingPoint)
                            {
                                Index = analogIndex++,
                                Name = record.Name,
                                Units = record.Units ?? "V",
                                PhaseID = "Pm",
                                CircuitComponent = record.CircuitComponent,
                                Multiplier = targetFloatingPoint ? 1.0D : AnalogChannel.DefaultVoltageMagnitudeMultiplier
                            });
                            break;
                        case SignalType.IPHA: // Фазовый угол силы тока
                        case SignalType.VPHA: // Фазовый угол напряжения
                            analogChannels.Add(new AnalogChannel(schema.Version, targetFloatingPoint)
                            {
                                Index = analogIndex++,
                                Name = record.Name,
                                Units = record.Units ?? "Rads",
                                PhaseID = "Pa",
                                CircuitComponent = record.CircuitComponent,
                                Multiplier = targetFloatingPoint ? 1.0D : AnalogChannel.DefaultPhaseAngleMultiplier
                            });
                            break;
                        case SignalType.FREQ: // Частота
                            analogChannels.Add(new AnalogChannel(schema.Version, targetFloatingPoint)
                            {
                                Index = analogIndex++,
                                Name = record.Name,
                                Units = record.Units ?? "Hz",
                                PhaseID = "F",
                                CircuitComponent = record.CircuitComponent,
                                Adder = targetFloatingPoint ? 0.0D : nominalFrequency,
                                Multiplier = targetFloatingPoint ? 1.0D : AnalogChannel.DefaultFrequencyMultiplier
                            });
                            break;
                        case SignalType.DFDT: // Скорость изменения частоты (dF/dt)
                            analogChannels.Add(new AnalogChannel(schema.Version, targetFloatingPoint)
                            {
                                Index = analogIndex++,
                                Name = record.Name,
                                Units = record.Units ?? "Hz/s",
                                PhaseID = "dF",
                                CircuitComponent = record.CircuitComponent,
                                Multiplier = targetFloatingPoint ? 1.0D : AnalogChannel.DefaultDfDtMultiplier
                            });
                            break;
                        default: // Все остальные сигналы рассматриваются как аналоговые значения
                            analogChannels.Add(new AnalogChannel(schema.Version, targetFloatingPoint)
                            {
                                Index = analogIndex++,
                                Name = record.Name,
                                PhaseID = string.Empty,
                                Units = record.Units,
                                CircuitComponent = record.CircuitComponent,
                                Multiplier = targetFloatingPoint ? 1.0D : AnalogChannel.DefaultAnalogMultiplier
                            });
                            break;
                    }
                }
            }

            schema.AnalogChannels = analogChannels.ToArray();
            schema.DigitalChannels = digitalChannels.ToArray();
            schema.NominalFrequency = nominalFrequency;

            return schema;
        }

        /// <summary>
        /// Создает новый поток комбинированного файла (.cff) формата COMTRADE.
        /// </summary>
        /// <param name="fileName">Имя файла. Файл должен иметь расширение ".cff".</param>
        /// <param name="schema">Схема конфигурации для записи в поток.</param>
        /// <param name="infLines">Строки секции "INF" для записи в поток, если таковые имеются.</param>
        /// <param name="hdrLines">Строки секции "HDR" для записи в поток, если таковые имеются.</param>
        /// <param name="encoding">Кодировка; значение <c>null</c> устанавливает кодировку по умолчанию, то есть UTF-8 (без BOM).</param>
        /// <returns>Новый поток комбинированного файла (.cff) формата COMTRADE, готовый к записи информационной секции.</returns>
        public static FileStream CreateCFFStream(string fileName, Schema schema, string[]? infLines = null, string[]? hdrLines = null, Encoding? encoding = null) => 
            CreateCFFStream(fileName, schema, infLines, hdrLines, encoding, out _);

        /// <summary>
        /// Создает новый поток комбинированного файла (.cff) формата COMTRADE, ориентированный на файлы типа ASCII.
        /// </summary>
        /// <param name="fileName">Имя файла. Файл должен иметь расширение ".cff".</param>
        /// <param name="schema">Схема конфигурации для записи в поток.</param>
        /// <param name="infLines">Строки секции "INF" для записи в поток, если таковые имеются.</param>
        /// <param name="hdrLines">Строки секции "HDR" для записи в поток, если таковые имеются.</param>
        /// <param name="encoding">Кодировка; значение <c>null</c> устанавливает кодировку по умолчанию, то есть UTF-8 (без BOM).</param>
        /// <returns>Новый объект для записи символов в поток комбинированного файла (.cff) формата COMTRADE, ориентированный на файлы типа ASCII, готовый к записи информационной секции.</returns>
        /// <remarks>
        /// Для версий формата COMTRADE, появившихся после 2001 года, любое использование термина ASCII также подразумевает
        /// использование Unicode UTF-8. Если параметр <paramref name="encoding"/> имеет значение <c>null</c>, то по умолчанию
        /// для записи текста будет использоваться кодировка UTF-8. Если по соображениям обратной совместимости необходимо 
        /// использовать кодировку ASCII, то параметр <paramref name="encoding"/> должен быть установлен в значение <see cref="Encoding.ASCII"/>.
        /// </remarks>
        public static StreamWriter CreateCFFStreamAscii(string fileName, Schema schema, string[]? infLines = null, string[]? hdrLines = null, Encoding? encoding = null)
        {
            if (schema.FileType != FileType.Ascii)
                throw new ArgumentException($"Невозможно создать поток CFF файла типа ASCII, используя схему конфигурации, предназначенную для файлов типа {schema.FileType.ToString().ToUpperInvariant()}", nameof(schema));

            CreateCFFStream(fileName, schema, infLines, hdrLines, encoding, out StreamWriter writer);
            return writer;
        }

        private static FileStream CreateCFFStream(string fileName, Schema schema, string[]? infLines, string[]? hdrLines, Encoding? encoding, out StreamWriter writer)
        {
            if (fileName is null)
                throw new ArgumentNullException(nameof(fileName));

            if (!Schema.HasCFFExtension(fileName))
                throw new ArgumentException("Указанное имя файла не содержит стандартного для COMTRADE расширения файла комбинированного формата: \".cff\".", nameof(fileName));

            FileStream stream = File.Create(fileName);

            CreateCFFStream(stream, schema, infLines, hdrLines, encoding, out writer);

            return stream;
        }

        /// <summary>
        /// Создает новый поток комбинированного файла (.cff) формата COMTRADE..
        /// </summary>
        /// <param name="stream">Поток комбинированного файла.</param>
        /// <param name="schema">Схема конфигурации для записи в поток.</param>
        /// <param name="infLines">Строки секции "INF" для записи в поток, если таковые имеются.</param>
        /// <param name="hdrLines">Строки секции "HDR" для записи в поток, если таковые имеются.</param>
        /// <param name="encoding">Кодировка; значение <c>null</c> устанавливает кодировку по умолчанию, то есть UTF-8 (без BOM).</param>
        public static void CreateCFFStream(Stream stream, Schema schema, string[]? infLines = null, string[]? hdrLines = null, Encoding? encoding = null) =>
            CreateCFFStream(stream, schema, infLines, hdrLines, encoding, out _);

        /// <summary>
        /// Создает новый поток комбинированного файла (.cff) формата COMTRADE, ориентированный на файлы типа ASCII.
        /// </summary>
        /// <param name="stream">Поток комбинированного файла.</param>
        /// <param name="schema">Схема конфигурации для записи в поток.</param>
        /// <param name="infLines">Строки секции "INF" для записи в поток, если таковые имеются.</param>
        /// <param name="hdrLines">Строки секции "HDR" для записи в поток, если таковые имеются.</param>
        /// <param name="encoding">Кодировка; значение <c>null</c> устанавливает кодировку по умолчанию, то есть UTF-8 (без BOM).</param>
        /// <returns>Новый объект для записи символов в поток комбинированного файла (.cff) формата COMTRADE, ориентированный на файлы типа ASCII, готовый к записи информационной секции.</returns>
        /// <remarks>
        /// Для версий формата COMTRADE, появившихся после 2001 года, любое использование термина ASCII также подразумевает
        /// использование Unicode UTF-8. Если параметр <paramref name="encoding"/> имеет значение <c>null</c>, то по умолчанию
        /// для записи текста будет использоваться кодировка UTF-8. Если по соображениям обратной совместимости необходимо 
        /// использовать кодировку ASCII, то параметр <paramref name="encoding"/> должен быть установлен в значение <see cref="Encoding.ASCII"/>.
        /// </remarks>
        public static StreamWriter CreateCFFStreamAscii(Stream stream, Schema schema, string[]? infLines = null, string[]? hdrLines = null, Encoding? encoding = null)
        {
            if (schema.FileType != FileType.Ascii)
                throw new ArgumentException($"Невозможно создать поток CFF файла типа ASCII, используя схему конфигурации, предназначенную для файлов типа {schema.FileType.ToString().ToUpperInvariant()}", nameof(schema));

            CreateCFFStream(stream, schema, infLines, hdrLines, encoding, out StreamWriter? writer);
            return writer;
        }

        private static void CreateCFFStream(Stream stream, Schema schema, string[]? infLines, string[]? hdrLines, Encoding? encoding, out StreamWriter writer)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            if (schema is null)
                throw new ArgumentNullException(nameof(schema));

            if (schema.Version < 2013)
                throw new ArgumentException("Минимальная версия формата COMTRADE для комбинированного файла (.cff) - 2013", nameof(schema));

            writer = new StreamWriter(stream, encoding ?? new UTF8Encoding(false)) { NewLine = CRLF }; // Задаем кодировку и символы конца строки

            writer.WriteLine("--- file type: CFG ---");
            writer.WriteLine(schema.FileContent);

            writer.WriteLine("--- file type: INF ---");
            writer.WriteLine(string.Join(CRLF, infLines ?? Array.Empty<string>()));

            writer.WriteLine("--- file type: HDR ---");
            writer.WriteLine(string.Join(CRLF, hdrLines ?? Array.Empty<string>()));

            // Резервируем место для счетчика байтов
            writer.WriteLine($"--- file type: DAT {(schema.FileType == FileType.Ascii ? "ASCII" : $"BINARY: {s_maxByteCountString}")} ---");

            // Объект StreamWriter уничтожать не нужно - это приведет к уничтожению основного потока
            writer.Flush();
        }

        /// <summary>
        /// Обновляет поток комбинированного, либо конфигурационного файла формата COMTRADE, указывая номер последней выборки.
        /// </summary>
        /// <param name="stream">Поток комбинированного, либо конфигурационного файла.</param>
        /// <param name="endSample">Номер последней выборки.</param>
        /// <param name="isCombinedFileFormat">Указывает формат файла. По умолчанию - <c>true</c>, то есть комбинированный формат.</param>
        /// <param name="rateIndex">Индекс частоты дискретизации для обновления, нумерация с нуля.</param>
        /// <param name="encoding">Кодировка; значение <c>null</c> устанавливает кодировку по умолчанию, то есть UTF-8 (без BOM).</param>
        public static void UpdateStreamEndSample(Stream stream, long endSample, bool isCombinedFileFormat = true, int rateIndex = 0, Encoding? encoding = null)
        {
            if (endSample > MaxEndSample)
                throw new ArgumentOutOfRangeException(nameof(endSample), $"Максимальный номер последней выборки формата COMTRADE - {MaxEndSample:N0}");

            if (rateIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(rateIndex), "Индекс частоты дискретизации не может быть отрицательным");

            stream.Position = 0;

            // Объект StreamReader уничтожать не нужно - это приведет к уничтожению основного потока
            StreamReader fileReader = new(stream);
            Encoding utf8 = new UTF8Encoding(false);
            string? lastLine = null;
            long position = 0;

            string readLine()
            {
                if (lastLine?.Length > 0)
                    position += utf8.GetBytes(lastLine).Length + 2; // CR и LF - это 2 управляющих символа, поэтому добавляем 2 байта

                string nextLine = fileReader.ReadLine() ?? throw new InvalidOperationException("Неожиданный конец конфигурационной секции");

                return lastLine = nextLine;
            }

            string line;

            // Считываем разделитель конфигурационной секции, если он есть
            if (isCombinedFileFormat)
            {
                line = readLine();

                if (!Schema.IsFileSectionSeparator(line, out string? sectionType) || sectionType != "CFG")
                    throw new InvalidOperationException($"Непредвиденный разделитель секций комбинированного файла (.cff) - Ожидаемый разделитель: \"--- file type: CFG ---\"{Environment.NewLine}Полученный разделитель: \"{line}\"");
            }

            // Пропускаем строку с названием станции, идентификатором регистратора и версией COMTRADE
            readLine();

            // Анализ строки с информацией о каналах
            line = readLine();
            string[] parts = line.Split(','); // Разделение общего количества каналов, количества аналоговых и количества дискретных каналов

            if (parts.Length < 3)
                throw new InvalidOperationException($"Непредвиденное количество элементов второй строки конфигурационного файла: {parts.Length} - Ожидаемое количество: 3{Environment.NewLine}Полученная строка: \"{line}\"");

            int totalAnalogChannels = int.Parse(parts[1].Trim().Split('A')[0], CultureInfo.InvariantCulture);
            int totalDigitalChannels = int.Parse(parts[2].Trim().Split('D')[0], CultureInfo.InvariantCulture);

            // Пропускаем строки с информацией об аналоговых каналах
            for (int i = 0; i < totalAnalogChannels; i++)
                readLine();

            // Пропускаем строки с информацией о дискретных каналах
            for (int i = 0; i < totalDigitalChannels; i++)
                readLine();

            // Пропускаем строку с частотой сети
            readLine();

            // Анализ строки с количеством различных частот дискретизации 
            int totalSampleRates = int.Parse(readLine(), CultureInfo.InvariantCulture);

            if (totalSampleRates == 0) // 0 говорит о том, что различные частоты дискретизации не использовались, то есть всё записывалось с одной частотой
                totalSampleRates = 1;

            if (rateIndex > totalSampleRates - 1)
                throw new ArgumentOutOfRangeException(nameof(rateIndex), $"Индекс частоты дискретизации {rateIndex:N0} превышает доступное количество частот дискретизации: {totalSampleRates:N0}");

            // Переходим к нужной строке с параметрами дискретизации
            for (int i = 0; i < rateIndex; i++)
                readLine();

            // Анализ строки с параметрами дискретизации
            line = readLine();
            SampleRate sampleRate = new(line) { EndSample = endSample };

            // Сохранение "хвоста" файла для последующей записи после номера последней выборки
            byte[] buffer = new byte[stream.Length - position - line.Length];
            stream.Position = position + line.Length;
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            // Запись частоты дискретизации с обновленным номером последней выборки
            byte[] bytes = (encoding ?? utf8).GetBytes(sampleRate.ToString().PadRight(line.Length));
            stream.Position = position;
            stream.Write(bytes, 0, bytes.Length);

            // Запись "хвоста" файла
            stream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Обновляет поток комбинированного файла (.cff) формата COMTRADE, указывая окончательное количество байтов.
        /// </summary>
        /// <param name="stream">Поток комбинированного файла.</param>
        /// <param name="byteCount">Количество байтов.</param>
        /// <param name="encoding">Кодировка; значение <c>null</c> устанавливает кодировку по умолчанию, то есть UTF-8 (без BOM).</param>
        public static void UpdateCFFStreamBinaryByteCount(Stream stream, long byteCount, Encoding? encoding = null)
        {
            if (byteCount > MaxFileSize)
                throw new ArgumentOutOfRangeException(nameof(byteCount), $"Максимальный размер файла сейчас составляет 256ТБ ({MaxFileSize:N0} байтов)");

            stream.Position = 0;

            // Переходим к информационной секции файла (Объект StreamReader уничтожать не нужно - это приведет к уничтожению основного потока)
            StreamReader fileReader = new(stream);
            Encoding utf8 = new UTF8Encoding(false);
            long position = 0;

            do
            {
                string? line = fileReader.ReadLine();

                if (line is null)
                    break;

                if (Schema.IsFileSectionSeparator(line, out string? sectionType, out _) && sectionType == "DAT BINARY")
                {
                    byte[] bytes = (encoding ?? utf8).GetBytes($"{byteCount} ---".PadRight($"{s_maxByteCountString} ---".Length));
                    stream.Position = position + "--- file type: DAT BINARY: ".Length;
                    stream.Write(bytes, 0, bytes.Length);
                    break;
                }

                position += utf8.GetBytes(line).Length + 2; // CR и LF - это 2 управляющих символа, поэтому добавляем 2 байта
            }
            while (true);
        }

        /// <summary>
        /// Запиcывает следующую выборку COMTRADE в формате ASCII.
        /// </summary>
        /// <param name="output">Поток для записи выборки.</param>
        /// <param name="schema">Схема конфигурации.</param>
        /// <param name="timestamp">Временная метка выборки (возможно неявное приведение <see cref="DateTime"/>).</param>
        /// <param name="values">Значения для записи.</param>
        /// <param name="sample">Номер выборки.</param>
        /// <param name="injectFracSecValue">Указывает, следует ли автоматически добавлять значения FRACSEC в поток в качестве первого набора дискретных каналов - по умолчанию <c>true</c>.</param>
        /// <param name="fracSecValue">Машинное слово FRACSEC для добавления в поток - по умолчанию 0x0000.</param>
        /// <remarks>
        /// Эта функция в первую очередь предназначена для записи информационных выборок COMTRADE в формате ASCII на основе данных синхрофазоров
        /// (см. приложение H: Schema for Phasor Data Using the COMTRADE File Standard в стандарте IEEE C37.111-2010),однако для некоторых нужд 
        /// может возникнуть необходимость ручной записи информационных выборок формата COMTRADE (например, не 16-битных машинных слов).
        /// </remarks>
        public static void WriteNextRecordAscii(StreamWriter output, Schema schema, Ticks timestamp, double[] values, uint sample, bool injectFracSecValue = true, ushort fracSecValue = 0x0000)
        {
            if (schema.FileType != FileType.Ascii)
                throw new ArgumentException($"Невозможно записать выборку формата ASCII, так как схема конфигурации содержит тип файла {schema.FileType.ToString().ToUpperInvariant()}", nameof(schema));

            // Установка временной метки относительно начала записи
            timestamp -= schema.StartTime.Value;

            uint microseconds = (uint)(timestamp.ToMicroseconds() / schema.TimeFactor);
            StringBuilder line = new();
            bool isFirstDigital = true; // Флаг поднят, если идет запись первого дискретного значения

            void append(FormattableString formattableString) =>
                line.Append(formattableString.ToString(CultureInfo.InvariantCulture));

            append($"{sample},{microseconds}");

            for (int i = 0; i < values.Length; i++)
            {
                double value = values[i];

                if (i < schema.AnalogChannels?.Length) // Обратное приведение выполняется только для аналоговых каналов
                {
                    value -= schema.AnalogChannels[i].Adder;
                    value /= schema.AnalogChannels[i].Multiplier;
                    value = Math.Truncate(value);

                    append($",{value}");
                }
                else
                {
                    if (isFirstDigital)
                    {
                        // Автоматическое добавление значений IEEE C37.118 FRACSEC, если требуется
                        isFirstDigital = false;

                        if (injectFracSecValue)
                        {
                            for (int j = 0; j < 16; j++)
                            {
                                int fracSecBit = ((fracSecValue & (1u << j)) != 0) ? 1 : 0;
                                append($",{fracSecBit}");
                            }
                        }
                    }

                    ushort digitalWord = (ushort)value;

                    if (digitalWord < 2) 
                        append($",{digitalWord}"); // Если значение дискретного канала было передано отдельным значением
                    else
                    {
                        for (int j = 0; j < 16; j++) // Если было передано 16-битное машинное слово, содержащее значения дискретных каналов
                        {
                            int digitalValue = ((digitalWord & (1u << j)) != 0) ? 1 : 0;
                            append($",{digitalValue}");
                        }
                    }
                }
            }

            // Добавлям значения FRACSEC, даже если в схеме нет дискретных каналов
            if (isFirstDigital && injectFracSecValue)
            {
                for (int j = 0; j < 16; j++)
                {
                    int fracSecBit = ((fracSecValue & (1u << j)) != 0) ? 1 : 0;
                    append($",{fracSecBit}");
                }
            }

            output.WriteLine(line.ToString());
        }

        /// <summary>
        /// Запиcывает следующую выборку COMTRADE в двоичном формате BINARY.
        /// </summary>
        /// <param name="output">Поток для записи выборки.</param>
        /// <param name="schema">Схема конфигурации.</param>
        /// <param name="timestamp">Временная метка выборки (возможно неявное приведение <see cref="DateTime"/>).</param>
        /// <param name="values">Значения для записи - значения дискретных каналов должны быть представлены в виде 16-битных машинных слов, каждое в отдельном значении double.</param>
        /// <param name="sample">Номер выборки.</param>
        /// <param name="injectFracSecValue">Указывает, следует ли автоматически добавлять значения FRACSEC в поток в качестве первого набора дискретных каналов - по умолчанию <c>true</c>.</param>
        /// <param name="fracSecValue">Машинное слово FRACSEC для добавления в поток - по умолчанию 0x0000.</param>
        /// <remarks>
        /// Эта функция в первую очередь предназначена для записи информационных выборок COMTRADE в формате BINARY на основе данных синхрофазоров
        /// (см. приложение H: Schema for Phasor Data Using the COMTRADE File Standard в стандарте IEEE C37.111-2010),однако для некоторых нужд 
        /// может возникнуть необходимость ручной записи информационных выборок формата COMTRADE (например, не 16-битных машинных слов).
        /// </remarks>
        public static void WriteNextRecordBinary(Stream output, Schema schema, Ticks timestamp, double[] values, uint sample, bool injectFracSecValue = true, ushort fracSecValue = 0x0000)
        {
            if (schema.FileType != FileType.Binary)
                throw new ArgumentException($"Невозможно записать выборку формата BINARY, так как схема конфигурации содержит тип файла {schema.FileType.ToString().ToUpperInvariant()}", nameof(schema));

            // Установка временной метки относительно начала записи
            timestamp -= schema.StartTime.Value;

            uint microseconds = (uint)(timestamp.ToMicroseconds() / schema.TimeFactor);
            bool isFirstDigital = true;

            output.Write(LittleEndian.GetBytes(sample), 0, 4); // Номер выборки - это uint32, поэтому 4 байта
            output.Write(LittleEndian.GetBytes(microseconds), 0, 4); // Аналогично

            for (int i = 0; i < values.Length; i++) 
            {
                double value = values[i];

                if (i < schema.AnalogChannels?.Length) // Обратное приведение выполняется только для аналоговых каналов
                {
                    value -= schema.AnalogChannels[i].Adder;
                    value /= schema.AnalogChannels[i].Multiplier;
                }
                else if (isFirstDigital)
                {
                    // Автоматическое добавление значений IEEE C37.118 FRACSEC, если требуется
                    isFirstDigital = false;

                    if (injectFracSecValue)
                        output.Write(LittleEndian.GetBytes(fracSecValue), 0, 2); // Машинное слово формата COMTRADE - это 2 байта
                }

                output.Write(LittleEndian.GetBytes((short)value), 0, 2); // В типе BINARY аналоговое значение занимает 2 байта      , столько же сколько занимает машинное слово, содержащее значения дискретных каналов
            }

            // Добавлям значения FRACSEC, даже если в схеме нет дискретных каналов
            if (isFirstDigital && injectFracSecValue)
                output.Write(LittleEndian.GetBytes(fracSecValue), 0, 2); // Машинное слово формата COMTRADE - это 2 байта
        }

        /// <summary>
        /// Запиcывает следующую выборку COMTRADE в двоичном формате BINARY32.
        /// </summary>
        /// <param name="output">Поток для записи выборки.</param>
        /// <param name="schema">Схема конфигурации.</param>
        /// <param name="timestamp">Временная метка выборки (возможно неявное приведение <see cref="DateTime"/>).</param>
        /// <param name="values">Значения для записи - значения дискретных каналов должны быть представлены в виде 16-битных машинных слов, каждое в отдельном значении double.</param>
        /// <param name="sample">Номер выборки.</param>
        /// <param name="injectFracSecValue">Указывает, следует ли автоматически добавлять значения FRACSEC в поток в качестве первого набора дискретных каналов - по умолчанию <c>true</c>.</param>
        /// <param name="fracSecValue">Машинное слово FRACSEC для добавления в поток - по умолчанию 0x0000.</param>
        /// <remarks>
        /// Эта функция в первую очередь предназначена для записи информационных выборок COMTRADE в формате BINARY32 на основе данных синхрофазоров
        /// (см. приложение H: Schema for Phasor Data Using the COMTRADE File Standard в стандарте IEEE C37.111-2010),однако для некоторых нужд 
        /// может возникнуть необходимость ручной записи информационных выборок формата COMTRADE (например, не 16-битных машинных слов).
        /// </remarks>
        public static void WriteNextRecordBinary32(Stream output, Schema schema, Ticks timestamp, double[] values, uint sample, bool injectFracSecValue = true, ushort fracSecValue = 0x0000)
        {
            if (schema.FileType != FileType.Binary32)
                throw new ArgumentException($"Невозможно записать выборку формата BINARY32, так как схема конфигурации содержит тип файла {schema.FileType.ToString().ToUpperInvariant()}", nameof(schema));

            // Установка временной метки относительно начала записи
            timestamp -= schema.StartTime.Value;

            uint microseconds = (uint)(timestamp.ToMicroseconds() / schema.TimeFactor);
            bool isFirstDigital = true;

            output.Write(LittleEndian.GetBytes(sample), 0, 4); // Номер выборки - это uint32, поэтому 4 байта
            output.Write(LittleEndian.GetBytes(microseconds), 0, 4); // Аналогично

            for (int i = 0; i < values.Length; i++)
            {
                double value = values[i];

                if (i < schema.AnalogChannels?.Length) // Обратное приведение выполняется только для аналоговых каналов
                {
                    value -= schema.AnalogChannels[i].Adder;
                    value /= schema.AnalogChannels[i].Multiplier;

                    output.Write(LittleEndian.GetBytes((int)value), 0, 4); // В формате BINARY32 одно аналоговое значение занимает 4 байта
                }
                else
                {
                    if (isFirstDigital)
                    {
                        // Добавлям значения FRACSEC, даже если в схеме нет дискретных каналов
                        isFirstDigital = false;

                        if (injectFracSecValue)
                            output.Write(LittleEndian.GetBytes(fracSecValue), 0, 2); // Машинное слово формата COMTRADE - это 2 байта
                    }

                    output.Write(LittleEndian.GetBytes((ushort)value), 0, 2); // В формате BINARY32 машинное слово, содержащее значения дискретных каналов занимает все так же 2 байта
                }
            }

            // Добавлям значения FRACSEC, даже если в схеме нет дискретных каналов
            if (isFirstDigital && injectFracSecValue)
                output.Write(LittleEndian.GetBytes(fracSecValue), 0, 2); // Машинное слово формата COMTRADE - это 2 байта
        }

        /// <summary>
        /// Запиcывает следующую выборку COMTRADE в двоичном формате FLOAT32.
        /// </summary>
        /// <param name="output">Поток для записи выборки.</param>
        /// <param name="schema">Схема конфигурации.</param>
        /// <param name="timestamp">Временная метка выборки (возможно неявное приведение <see cref="DateTime"/>).</param>
        /// <param name="values">Значения для записи - значения дискретных каналов должны быть представлены в виде 16-битных машинных слов, каждое в отдельном значении double.</param>
        /// <param name="sample">Номер выборки.</param>
        /// <param name="injectFracSecValue">Указывает, следует ли автоматически добавлять значения FRACSEC в поток в качестве первого набора дискретных каналов - по умолчанию <c>true</c>.</param>
        /// <param name="fracSecValue">Машинное слово FRACSEC для добавления в поток - по умолчанию 0x0000.</param>
        /// <remarks>
        /// Эта функция в первую очередь предназначена для записи информационных выборок COMTRADE в формате FLOAT32 на основе данных синхрофазоров
        /// (см. приложение H: Schema for Phasor Data Using the COMTRADE File Standard в стандарте IEEE C37.111-2010),однако для некоторых нужд 
        /// может возникнуть необходимость ручной записи информационных выборок формата COMTRADE (например, не 16-битных машинных слов).
        /// </remarks>
        public static void WriteNextRecordFloat32(Stream output, Schema schema, Ticks timestamp, double[] values, uint sample, bool injectFracSecValue = true, ushort fracSecValue = 0x0000)
        {
            if (schema.FileType != FileType.Float32)
                throw new ArgumentException($"Невозможно записать выборку формата FLOAT32, так как схема конфигурации содержит тип файла {schema.FileType.ToString().ToUpperInvariant()}", nameof(schema));

            // Установка временной метки относительно начала записи
            timestamp -= schema.StartTime.Value;

            uint microseconds = (uint)(timestamp.ToMicroseconds() / schema.TimeFactor);
            bool isFirstDigital = true;

            output.Write(LittleEndian.GetBytes(sample), 0, 4); // Номер выборки - это uint32, поэтому 4 байта
            output.Write(LittleEndian.GetBytes(microseconds), 0, 4); // Аналогично

            for (int i = 0; i < values.Length; i++)
            {
                double value = values[i];

                if (i < schema.AnalogChannels?.Length) // Обратное приведение выполняется только для аналоговых каналов
                {
                    value -= schema.AnalogChannels[i].Adder;
                    value /= schema.AnalogChannels[i].Multiplier;

                    output.Write(LittleEndian.GetBytes((float)value), 0, 4); // В формате FLOAT32 одно аналоговое значение занимает 4 байта
                }
                else
                {
                    if (isFirstDigital)
                    {
                        // Автоматическое добавление значений IEEE C37.118 FRACSEC, если требуется
                        isFirstDigital = false;

                        if (injectFracSecValue)
                            output.Write(LittleEndian.GetBytes(fracSecValue), 0, 2); // Машинное слово формата COMTRADE - это 2 байта
                    }

                    output.Write(LittleEndian.GetBytes((ushort)value), 0, 2); // В формате FLOAT32 машинное слово, содержащее значения дискретных каналов занимает все так же 2 байта
                }
            }

            // Добавлям значения FRACSEC, даже если в схеме нет дискретных каналов
            if (isFirstDigital && injectFracSecValue)
                output.Write(LittleEndian.GetBytes(fracSecValue), 0, 2); // Машинное слово формата COMTRADE - это 2 байта
        }

        /// <summary>
        /// Запиcывает следующую выборку COMTRADE в формате ASCII.
        /// </summary>
        /// <param name="output">Поток для записи выборки.</param>
        /// <param name="schema">Схема конфигурации.</param>
        /// <param name="timestamp">Временная метка выборки (возможно неявное приведение <see cref="DateTime"/>).</param>
        /// <param name="analogValues">Аналоговые значения для записи.</param>
        /// <param name="digitalValues">Дискретные значения для записи.</param>
        /// <param name="sample">Номер выборки.</param>
        /// <param name="injectFracSecValue">Указывает, следует ли автоматически добавлять значения FRACSEC в поток в качестве первого набора дискретных каналов - по умолчанию <c>false</c>.</param>
        /// <param name="fracSecValue">Машинное слово FRACSEC для добавления в поток - по умолчанию 0x0000.</param>
        /// <remarks>
        /// Эта функция в первую очередь предназначена для записи информационных выборок COMTRADE в формате ASCII на основе данных синхрофазоров
        /// (см. приложение H: Schema for Phasor Data Using the COMTRADE File Standard в стандарте IEEE C37.111-2010),однако для некоторых нужд 
        /// может возникнуть необходимость ручной записи информационных выборок формата COMTRADE (например, не 16-битных машинных слов).
        /// </remarks>
        public static void WriteNextRecordAscii(StreamWriter output, Schema schema, Ticks timestamp, double[] analogValues, bool[] digitalValues, uint sample, bool injectFracSecValue = false, ushort fracSecValue = 0x0000)
        {
            double[] values = analogValues
                .Concat(digitalValues.Select(b => b ? 1.0D : 0.0D))
                .ToArray();

            WriteNextRecordAscii(output, schema, timestamp, values, sample, injectFracSecValue, fracSecValue);
        }

        /// <summary>
        /// Запиcывает следующую выборку COMTRADE в двоичном формате BINARY.
        /// </summary>
        /// <param name="output">Поток для записи выборки.</param>
        /// <param name="schema">Схема конфигурации.</param>
        /// <param name="timestamp">Временная метка выборки (возможно неявное приведение <see cref="DateTime"/>).</param>
        /// <param name="analogValues">Аналоговые значения для записи.</param>
        /// <param name="digitalValues">Дискретные значения для записи.</param>
        /// <param name="sample">Номер выборки.</param>
        /// <param name="injectFracSecValue">Указывает, следует ли автоматически добавлять значения FRACSEC в поток в качестве первого набора дискретных каналов - по умолчанию <c>false</c>.</param>
        /// <param name="fracSecValue">Машинное слово FRACSEC для добавления в поток - по умолчанию 0x0000.</param>
        /// <remarks>
        /// Эта функция в первую очередь предназначена для записи информационных выборок COMTRADE в формате BINARY на основе данных синхрофазоров
        /// (см. приложение H: Schema for Phasor Data Using the COMTRADE File Standard в стандарте IEEE C37.111-2010),однако для некоторых нужд 
        /// может возникнуть необходимость ручной записи информационных выборок формата COMTRADE (например, не 16-битных машинных слов).
        /// </remarks>
        public static void WriteNextRecordBinary(Stream output, Schema schema, Ticks timestamp, double[] analogValues, bool[] digitalValues, uint sample, bool injectFracSecValue = false, ushort fracSecValue = 0x0000)
        {
            double[] digitalWords = GroupByWord(digitalValues); // Группируем значения дискретных каналов в 16-битные машинные слова
            double[] values = analogValues.Concat(digitalWords).ToArray();
            WriteNextRecordBinary(output, schema, timestamp, values, sample, injectFracSecValue, fracSecValue);
        }

        /// <summary>
        /// Запиcывает следующую выборку COMTRADE в двоичном формате BINARY32.
        /// </summary>
        /// <param name="output">>Поток для записи выборки</param>
        /// <param name="schema">Схема конфигурации.</param>
        /// <param name="timestamp">Временная метка выборки (возможно неявное приведение <see cref="DateTime"/>).</param>
        /// <param name="analogValues">Аналоговые значения для записи.</param>
        /// <param name="digitalValues">Дискретные значения для записи.</param>
        /// <param name="sample">Номер выборки.</param>
        /// <param name="injectFracSecValue">Указывает, следует ли автоматически добавлять значения FRACSEC в поток в качестве первого набора дискретных каналов - по умолчанию <c>false</c>.</param>
        /// <param name="fracSecValue">Машинное слово FRACSEC для добавления в поток - по умолчанию 0x0000.</param>
        /// <remarks>
        /// Эта функция в первую очередь предназначена для записи информационных выборок COMTRADE в формате BINARY32 на основе данных синхрофазоров
        /// (см. приложение H: Schema for Phasor Data Using the COMTRADE File Standard в стандарте IEEE C37.111-2010),однако для некоторых нужд 
        /// может возникнуть необходимость ручной записи информационных выборок формата COMTRADE (например, не 16-битных машинных слов).
        /// </remarks>
        public static void WriteNextRecordBinary32(Stream output, Schema schema, Ticks timestamp, double[] analogValues, bool[] digitalValues, uint sample, bool injectFracSecValue = false, ushort fracSecValue = 0x0000)
        {
            double[] digitalWords = GroupByWord(digitalValues); // Группируем значения дискретных каналов в 16-битные машинные слова
            double[] values = analogValues.Concat(digitalWords).ToArray();
            WriteNextRecordBinary32(output, schema, timestamp, values, sample, injectFracSecValue, fracSecValue);
        }

        /// <summary>
        /// Запиcывает следующую выборку COMTRADE в двоичном формате FLOAT32.
        /// </summary>
        /// <param name="output">Поток для записи выборки.</param>
        /// <param name="schema">Схема конфигурации.</param>
        /// <param name="timestamp">Временная метка выборки (возможно неявное приведение <see cref="DateTime"/>).</param>
        /// <param name="analogValues">Аналоговые значения для записи.</param>
        /// <param name="digitalValues">Дискретные значения для записи.</param>
        /// <param name="sample">Номер выборки.</param>
        /// <param name="injectFracSecValue">Указывает, следует ли автоматически добавлять значения FRACSEC в поток в качестве первого набора дискретных каналов - по умолчанию <c>false</c>.</param>
        /// <param name="fracSecValue">Машинное слово FRACSEC для добавления в поток - по умолчанию 0x0000.</param>
        /// <remarks>
        /// Эта функция в первую очередь предназначена для записи информационных выборок COMTRADE в формате FLOAT32 на основе данных синхрофазоров
        /// (см. приложение H: Schema for Phasor Data Using the COMTRADE File Standard в стандарте IEEE C37.111-2010),однако для некоторых нужд 
        /// может возникнуть необходимость ручной записи информационных выборок формата COMTRADE (например, не 16-битных машинных слов).
        /// </remarks>
        public static void WriteNextRecordFloat32(Stream output, Schema schema, Ticks timestamp, double[] analogValues, bool[] digitalValues, uint sample, bool injectFracSecValue = false, ushort fracSecValue = 0x0000)
        {
            double[] digitalWords = GroupByWord(digitalValues); // Группируем значения дискретных каналов в 16-битные машинные слова
            double[] values = analogValues.Concat(digitalWords).ToArray();
            WriteNextRecordFloat32(output, schema, timestamp, values, sample, injectFracSecValue, fracSecValue);
        }

        private static double[] GroupByWord(bool[] digitalValues)
        {
            int index = 0;

            return digitalValues
                .Select(b => b ? 1 : 0)
                .GroupBy(_ => (index++) / 16)
                .Select(grouping => grouping.Select((bit, i) => bit << i))
                .Select(grouping => grouping.Aggregate((ushort)0, (word, bit) => (ushort)(word | bit)))
                .Select(word => (double)word)
                .ToArray();
        }
    }
}
