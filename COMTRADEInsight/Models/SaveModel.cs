using COMTRADE;
using System.Text;
using COMTRADE.Helpers;

namespace COMTRADEInsight.Models
{
    /// <summary>
    /// Модель окна сохранения файла.
    /// </summary>
    public static class SaveModel
    {
        #region [ Classes ]

        // Вложенные классы
        public class SaveSettings
        {
            public string? FilePath { get; set; }  // Путь к папке для сохранения файла
            public string? FileName { get; set; }  // Имя сохраняемого файла
            public FileFormat Format { get; set; } // Формат файла (CFF, CFG)
            public FileType Type { get; set; } // Тип файла  (ASCII, BINARY, BINARY32, FLOAT32)
            public bool DontWriteDatFile { get; set; } // Флаг, указывающий, стоит ли пропускать информационный файл при сохранении
        }

        #endregion

        #region [ Enumerations ]

        // Перечисления
        public enum FileFormat
        {
            /// <summary>
            /// Комбинированный формат файла.
            /// </summary>
            CFF,
            /// <summary>
            /// Классический формат из нескольких файлов.
            /// </summary>
            CFG
        }

        #endregion

        #region [ Static ]

        // Статические методы

        // Запись файлов формата COMTRADE
        public static void WriteComtradeFiles(Tuple<Schema, DateTime, List<List<double>>> data, SaveSettings settings)
        {
            // Деконструкция кортежа
            Schema schema = data.Item1;
            Ticks firstTimeStamp = data.Item2;
            List<List<double>> unTransposeChannelsData = data.Item3;

            // Проверка на наличие данных для записи
            if (unTransposeChannelsData.Count == 0)
            {
                MessageBox.Show("Данные для сохранения отсутствуют");
                return;
            }

            // Транспонирование данных. Для удобства работы при открытии файла значения каналов помещаются в список списков следующим образом:
            // размер внешнего списка равен количеству каналов, а размер внутреннего - количеству выборок. Таким образом, если в конфигурации
            // 4 канала и 30 выборок, получается матрица 4x30, что очень удобно в контексте построения графиков и диаграмм, но не соответствует
            // формату COMTRADE. Для записи данных в файл их необходимо транспонировать к виду 30x4.
            List<List<double>> channelsData = new();

            for (int j = 0; j < unTransposeChannelsData[0].Count; j++)
            {
                List<double> newRow = new();
                for (int i = 0; i < unTransposeChannelsData.Count; i++)
                {
                    newRow.Add(unTransposeChannelsData[i][j]);
                }
                channelsData.Add(newRow);
            }

            // Подготовка данных к записи
            long numberOfSamples = channelsData.Count;
            double timeAdder = 10000000 / schema.SampleRates![0].Rate; // time = 1 / frequency. В секунде 10000000 тактов
            uint sample = 1;

            ArraySegment<double> analogValues;
            bool[] digitalValues;
            int valueIndex;
            Ticks ticksAdder;

            void prepareDataforWriting(int i)
            {
                analogValues = new ArraySegment<double>(channelsData[i].ToArray(), 0, schema.TotalAnalogChannels);
                digitalValues = new bool[schema.TotalDigitalChannels];
                valueIndex = schema.TotalAnalogChannels; // Номер первого дискретного канала в выборке

                for (int j = 0; j < schema.TotalDigitalChannels; j++, valueIndex++)
                    digitalValues[j] = channelsData[i][valueIndex] == 0 ? false : true;

                if (Math.Round(timeAdder * i) % 10 == 7)
                    ticksAdder = (Ticks)Math.Round(timeAdder * i) + 3;
                else
                    ticksAdder = (Ticks)Math.Round(timeAdder * i);
            }

            // Запись
            string filePath = settings.FilePath + "\\" + settings.FileName;
            schema.FileType = settings.Type;

            if (settings.Format == FileFormat.CFF)
            {
                filePath += ".cff";
                switch (schema.FileType)
                {
                    case FileType.Ascii:
                        {
                            using (StreamWriter myWriter = Writer.CreateCFFStreamAscii(filePath, schema))
                            {
                                for (int i = 0; i < numberOfSamples; i++)
                                {
                                    prepareDataforWriting(i);
                                    Writer.WriteNextRecordAscii(myWriter, schema, new DateTime(firstTimeStamp + ticksAdder), analogValues.ToArray(), digitalValues, sample, false);
                                    sample++;
                                }
                            }
                            break;
                        }
                    case FileType.Binary:
                        {
                            using (FileStream myWriter = Writer.CreateCFFStream(filePath, schema))
                            {
                                for (int i = 0; i < numberOfSamples; i++)
                                {
                                    prepareDataforWriting(i);
                                    Writer.WriteNextRecordBinary(myWriter, schema, new DateTime(firstTimeStamp + ticksAdder), analogValues.ToArray(), digitalValues, sample, false);
                                    sample++;
                                }
                                Writer.UpdateCFFStreamBinaryByteCount(myWriter, schema.BinaryRecordLength * numberOfSamples);
                            }
                            break;
                        }
                    case FileType.Binary32:
                        {
                            using (FileStream myWriter = Writer.CreateCFFStream(filePath, schema))
                            {
                                for (int i = 0; i < numberOfSamples; i++)
                                {
                                    prepareDataforWriting(i);
                                    Writer.WriteNextRecordBinary32(myWriter, schema, new DateTime(firstTimeStamp + ticksAdder), analogValues.ToArray(), digitalValues, sample, false);
                                    sample++;
                                }
                                Writer.UpdateCFFStreamBinaryByteCount(myWriter, schema.Binary32RecordLength * numberOfSamples);
                            }
                            break;
                        }
                    case FileType.Float32:
                        {
                            using (FileStream myWriter = Writer.CreateCFFStream(filePath, schema))
                            {
                                for (int i = 0; i < numberOfSamples; i++)
                                {
                                    prepareDataforWriting(i);
                                    Writer.WriteNextRecordFloat32(myWriter, schema, new DateTime(firstTimeStamp + ticksAdder), analogValues.ToArray(), digitalValues, sample, false);
                                    sample++;
                                }
                                Writer.UpdateCFFStreamBinaryByteCount(myWriter, schema.Float32RecordLength * numberOfSamples);
                            }
                            break;
                        }
                }

                // Обновление номера последней выборки
                if (numberOfSamples != schema.TotalSamples)
                {
                    using (FileStream fileStreamWrite = new(filePath, FileMode.Open, FileAccess.ReadWrite))
                    {
                        Writer.UpdateStreamEndSample(fileStreamWrite, numberOfSamples);
                    }
                }
            }
            else
            {
                if (settings.DontWriteDatFile)
                    File.WriteAllText(filePath + ".cfg", schema.FileContent, new UTF8Encoding(false));
                else
                {
                    File.WriteAllText(filePath + ".cfg", schema.FileContent, new UTF8Encoding(false));

                    switch (schema.FileType)
                    {
                        case FileType.Ascii:
                            {
                                using (FileStream stream = new(filePath + ".dat", FileMode.Create, FileAccess.ReadWrite))
                                {
                                    using (StreamWriter myWriter = new(stream))
                                    {
                                        for (int i = 0; i < numberOfSamples; i++)
                                        {
                                            prepareDataforWriting(i);
                                            Writer.WriteNextRecordAscii(myWriter, schema, new DateTime(firstTimeStamp + ticksAdder), analogValues.ToArray(), digitalValues, sample, false);
                                            sample++;
                                        }
                                    }
                                }
                                break;
                            }
                        case FileType.Binary:
                            {
                                using (FileStream myWriter = new(filePath + ".dat", FileMode.Create, FileAccess.ReadWrite))
                                {
                                    for (int i = 0; i < numberOfSamples; i++)
                                    {
                                        prepareDataforWriting(i);
                                        Writer.WriteNextRecordBinary(myWriter, schema, new DateTime(firstTimeStamp + ticksAdder), analogValues.ToArray(), digitalValues, sample, false);
                                        sample++;
                                    }
                                }
                                break;
                            }
                        case FileType.Binary32:
                            {
                                using (FileStream myWriter = new(filePath + ".dat", FileMode.Create, FileAccess.ReadWrite))
                                {
                                    for (int i = 0; i < numberOfSamples; i++)
                                    {
                                        prepareDataforWriting(i);
                                        Writer.WriteNextRecordBinary32(myWriter, schema, new DateTime(firstTimeStamp + ticksAdder), analogValues.ToArray(), digitalValues, sample, false);
                                        sample++;
                                    }
                                }
                                break;
                            }
                        case FileType.Float32:
                            {
                                using (FileStream myWriter = new(filePath + ".dat", FileMode.Create, FileAccess.ReadWrite))
                                {
                                    for (int i = 0; i < numberOfSamples; i++)
                                    {
                                        prepareDataforWriting(i);
                                        Writer.WriteNextRecordFloat32(myWriter, schema, new DateTime(firstTimeStamp + ticksAdder), analogValues.ToArray(), digitalValues, sample, false);
                                        sample++;
                                    }
                                }
                                break;
                            }
                    }
                }

                // Обновление номера последней выборки
                if (numberOfSamples != schema.TotalSamples)
                {
                    using (FileStream fileStreamWrite = new(filePath + ".cfg", FileMode.Open, FileAccess.ReadWrite))
                    {
                        Writer.UpdateStreamEndSample(fileStreamWrite, numberOfSamples, false);
                    }
                }
            }
        }

        #endregion
    }
}
