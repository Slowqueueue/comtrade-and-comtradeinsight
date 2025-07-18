using COMTRADE;
using COMTRADE.Helpers;
using COMTRADEInsight.Models;
using COMTRADEInsight.Views;
using NAudio.Wave;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WinForms;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Label = System.Windows.Forms.Label;

namespace COMTRADEInsight.ViewModels
{
    /// <summary>
    /// Модель Представления главного окна.
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region [ Members ]

        // Поля
        private readonly MainWindowModel model = new(); //Model главного окна

        private readonly ICommand openFileCommand; // Команда открытия файла
        private readonly ICommand showSaveCommand; // Команда открытия окна сохранения файла
        private readonly ICommand showConfigCommand; // Команда открытия окна конфигурации файла
        private readonly ICommand closeProgramCommand; // Команда закрытия программы
        private readonly ICommand moveForthCommand; // Команда перемещения первого вертикального визира вперед
        private readonly ICommand moveBackCommand; // Команда перемещения первого вертикального визира назад
        private readonly ICommand moveSecondForthCommand; // Команда перемещения второго вертикального визира вперед
        private readonly ICommand moveSecondBackCommand; // Команда перемещения второго вертикального визира назад
        private readonly ICommand resetScaleCommand; // Команда сброса настроек масштабирования графиков
        private readonly ICommand scaleXUpCommand; // Команда увеличения масштаба графиков по X
        private readonly ICommand scaleXDownCommand; // Команда уменьшения масштаба графиков по X
        private readonly ICommand scaleYUpCommand; // Команда увеличения масштаба графиков по Y
        private readonly ICommand scaleYDownCommand; // Команда уменьшения масштаба графиков по Y
        private readonly ICommand emulateDynamicCommand; // Команда эмуляции динамической отрисовки
        private readonly ICommand cropPlotCommand; // Команда обрезки графиков
        private readonly ICommand changeVisibilityOfFirstVertVisorCommand; // Команда смены видимости первого вертикального визира
        private readonly ICommand changeVisibilityOfSecondVertVisorCommand; // Команда смены видимости второго вертикального визира
        private readonly ICommand playFileCommand;

        private bool isParserInitialized = false; // Флаг, указывающий, инициализирован ли синтаксический анализатор файлов формата COMTRADE
        private int numberOfPlots = 0; // Количество графиков
        private int firstVisorSample = 0; // Позиция первого вертикального визира
        private int secondVisorSample = -1; // Позиция второго вертикального визира (изначально не установлен)
        private int analogChannelsCount = 0; // Количество аналоговых каналов
        private long numberOfSamples = 0; // Количество выборок
        private int numberOfSamplesPerPeriod = 0; // Количество выборок за период
        private bool wasVectorDiagramScaled = false; // Флаг, показывающий, была ли масштабирована векторная диаграмма
        private const int AnalogSignalPlotDefaultHeight = 100; // Высота графика аналогового сигнала по умолчанию
        private const int DigitalSignalPlotDefaultHeight = 50; // Высота графика дискретного сигнала по умолчанию
        private const double SpectralScalingFactor = 1.41421356237; // Коэффициент масштабирования амплитуд спектра (корень из двух)
        private int AnalogSignalPlotHeight = AnalogSignalPlotDefaultHeight; // Высота графика аналогового сигнала
        private int XScale = 1; // Масштаб графиков по X
        private int YScale = 1; // Масштаб графиков по Y
        private DateTime firstTimeStamp = DateTime.MinValue; // Первая временная метка (используется для сохранения файлов)
        private Parser? parser; // Синтаксический анализатор файлов формата COMTRADE

        private readonly List<string> channelsNames = new(); // Названия каналов
        private readonly List<List<double>> channelsData = new(); // Значения каналов
        private readonly List<double> firstHarmonicAmplitude = new(); // Амплитуды первых гармоник аналоговых каналов
        private readonly List<double> firstHarmonicPhase = new(); // Фазовые углы первых гармоник  аналоговых каналов
        private readonly List<double> RootMeanSquare = new(); // Среднеквадратичная величина аналоговых каналов
        private readonly List<double[]> amplitudesForSpectralDiagram = new(); // Амплитуды всех гармоник аналоговых каналов
        private readonly List<string> analogChannelsUnits = new(); // Единицы измерения аналоговых каналов
        private readonly List<double> timeStamps = new(); // Временные метки
        private readonly List<FormsPlot> signalPlots = new(); // Графики сигналов
        private readonly List<SignalXY> signals = new(); // Сигналы с графиков
        private readonly List<Arrow> vectors = new(); // Вектора векторной диаграммы аналоговых сигналов
        private readonly List<BarPlot> barPlots = new(); // Наборы столбцов спектральных диаграмм аналоговых сигналов
        private readonly List<VerticalLine> firstVertVisors = new(); // Первые вертикальные визиры
        private readonly List<VerticalLine> secondVertVisors = new(); // Вторые вертикальные визиры

        private HorizontalLine[]? firstHorVisors; // Первые горизонтальные визиры
        private HorizontalLine[]? secondHorVisors; // Вторые горизонтальные визиры

        #endregion

        #region [ Constructors ]

        // Конструктор
        public MainWindowViewModel()
        {
            openFileCommand = new ObjectCommand(OpenFile);
            showSaveCommand = new SimpleCommand(ShowSave);
            showConfigCommand = new SimpleCommand(ShowConfig);
            closeProgramCommand = new SimpleCommand(CloseProgram);
            moveForthCommand = new ObjectConditionCommand(MoveForth, CanMoveForth);
            moveBackCommand = new ObjectConditionCommand(MoveBack, CanMoveBack);
            moveSecondForthCommand = new ObjectConditionCommand(MoveSecondVisorForth, CanMoveSecondForth);
            moveSecondBackCommand = new ObjectConditionCommand(MoveSecondVisorBack, CanMoveSecondBack);
            resetScaleCommand = new ObjectCommand(ResetScale);
            scaleXUpCommand = new ConditionCommand(ScaleXUp, CanScaleXUp);
            scaleXDownCommand = new SimpleCommand(ScaleXDown);
            scaleYUpCommand = new ObjectCommand(ScaleYUp);
            scaleYDownCommand = new ObjectConditionCommand(ScaleYDown, CanScaleYDown);
            emulateDynamicCommand = new ObjectCommand(EmulateDynamic);
            cropPlotCommand = new SimpleCommand(CropPlot);
            changeVisibilityOfFirstVertVisorCommand = new SimpleCommand(FirstVertVisorChangeVisibility);
            changeVisibilityOfSecondVertVisorCommand = new SimpleCommand(SecondVertVisorChangeVisibility);
            playFileCommand = new SimpleCommand(PlayFile);
        }

        #endregion

        #region [ Properties ]

        // Свойства
        /// <summary>
        /// Получает название программы.
        /// </summary>
        public string ApplicationTitle
        {
            get { return model.ApplicationTitle; }
        }

        /// <summary>
        /// Получает команду открытия файла.
        /// </summary>
        /// <remarks>Принимает <see cref="Notifications"/> как объект.</remarks> 
        public ICommand OpenFileCommand
        {
            get { return openFileCommand; }
        }

        /// <summary>
        /// Получает команду открытия окна сохранения файла.
        /// </summary>
        public ICommand ShowSaveCommand
        {
            get { return showSaveCommand; }
        }

        /// <summary>
        /// Получает команду открытия окна конфигурации файла.
        /// </summary>
        public ICommand ShowConfigCommand
        {
            get { return showConfigCommand; }
        }

        /// <summary>
        /// Получает команду закрытия программы.
        /// </summary>

        public ICommand CloseProgramCommand
        {
            get { return closeProgramCommand; }
        }

        /// <summary>
        /// Получает команду перемещения первого вертикального визира вперед.
        /// </summary>
        /// <remarks>Принимает <see cref="Notifications"/> как объект.</remarks> 
        public ICommand MoveForthCommand
        {
            get { return moveForthCommand; }
        }

        /// <summary>
        /// Получает команду перемещения первого вертикального визира назад.
        /// </summary>
        /// <remarks>Принимает <see cref="Notifications"/> как объект.</remarks> 
        public ICommand MoveBackCommand
        {
            get { return moveBackCommand; }
        }

        /// <summary>
        /// Получает команду перемещения второго вертикального визира вперед.
        /// </summary>
        /// <remarks>Принимает <see cref="Notifications"/> как объект.</remarks> 
        public ICommand MoveSecondForthCommand
        {
            get { return moveSecondForthCommand; }
        }

        /// <summary>
        /// Получает команду перемещения второго вертикального визира назад.
        /// </summary>
        /// <remarks>Принимает <see cref="Notifications"/> как объект.</remarks> 
        public ICommand MoveSecondBackCommand
        {
            get { return moveSecondBackCommand; }
        }

        /// <summary>
        /// Получает команду сброса настроек масштабирования графиков.
        /// </summary>
        /// <remarks>Принимает signalPlotsPanel как объект.</remarks> 
        public ICommand ResetScaleCommand
        {
            get { return resetScaleCommand; }
        }

        /// <summary>
        /// Получает команду увеличения масштаба графиков по X.
        /// </summary>
        public ICommand ScaleXUpCommand
        {
            get { return scaleXUpCommand; }
        }

        /// <summary>
        /// Получает команду уменьшения масштаба графиков по X.
        /// </summary>
        public ICommand ScaleXDownCommand
        {
            get { return scaleXDownCommand; }
        }

        /// <summary>
        /// Получает команду увеличения масштаба графиков по Y.
        /// </summary>
        /// <remarks>Принимает signalPlotsPanel как объект.</remarks> 
        public ICommand ScaleYUpCommand
        {
            get { return scaleYUpCommand; }
        }

        /// <summary>
        /// Получает команду уменьшения масштаба графиков по Y.
        /// </summary>
        /// <remarks>Принимает signalPlotsPanel как объект.</remarks> 
        public ICommand ScaleYDownCommand
        {
            get { return scaleYDownCommand; }
        }

        /// <summary>
        /// Получает команду эмуляции динамической отрисовки.
        /// </summary>
        /// <remarks>Принимает signalPlotsPanel как объект.</remarks> 
        public ICommand EmulateDynamicCommand
        {
            get { return emulateDynamicCommand; }
        }

        /// <summary>
        /// Получает команду обрезки графиков.
        /// </summary>
        public ICommand CropPlotCommand
        {
            get { return cropPlotCommand; }
        }

        /// <summary>
        /// Получает команду смены видимости первого вертикального визира.
        /// </summary>
        public ICommand СhangeVisibilityOfFirstVertVisorCommand
        {
            get { return changeVisibilityOfFirstVertVisorCommand; }
        }

        /// <summary>
        /// Получает команду смены видимости второго вертикального визира.
        /// </summary>
        public ICommand СhangeVisibilityOfSecondVertVisorCommand
        {
            get { return changeVisibilityOfSecondVertVisorCommand; }
        }

        public ICommand PlayFileCommand
        {
            get { return playFileCommand; }
        }

        #endregion

        #region [ Methods ]

        // Методы

        // Открытие файла
        private void OpenFile(object? obj)
        {
            // Настройка, открытие и обработка диалогового окна
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Осциллограммы COMTRADE (*.cfg, *.cff)|*.cfg;*.cff|Все файлы (*.*)|*.*", // Фильтр файлов
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) // Начальная директория
            };

            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                string fileExtension = Path.GetExtension(filePath).ToLower();

                if ((fileExtension == ".cfg" || fileExtension == ".cff") && InitializeParser(filePath) && obj is MainWindowForm mainform)
                {
                    // Подготовка данных и главного окна
                    mainform.Text = Path.GetFileName(filePath) + " - " + ApplicationTitle;
                    PrepareData();
                    CreateSignalPlots(mainform);
                    CreateVectorDiagram((Panel)mainform.Controls["vectorDiagramPanel"]!);
                    CreateSpectralDiagram((Panel)mainform.Controls["spectralDiagramPanel"]!);
                }
                else
                    throw new Exception("Неверное расширение файла. Допустимые расширения: .cfg, .cff");
            }
            else
                throw new Exception("Отмена");
        }

        // Инициализация синтаксического анализатора файлов формата COMTRADE
        private bool InitializeParser(string filePath)
        {
            using (parser = new Parser())
            {
                try
                {
                    // Установка схемы конфигурационного файла
                    parser.Schema = new Schema(filePath);
                    parser.InferTimeFromSampleRates = false;
                    isParserInitialized = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    isParserInitialized = false;
                    return false;
                }

                return true;
            }
        }

        // Подготовка данных
        private void PrepareData()
        {
            // Очистка списков
            channelsNames.Clear();
            channelsData.Clear();
            firstHarmonicAmplitude.Clear();
            firstHarmonicPhase.Clear();
            RootMeanSquare.Clear();
            amplitudesForSpectralDiagram.Clear();
            analogChannelsUnits.Clear();
            timeStamps.Clear();
            signalPlots.Clear();
            signals.Clear();
            vectors.Clear();
            barPlots.Clear();
            firstVertVisors.Clear();
            secondVertVisors.Clear();

            // Заполнение полей
            numberOfPlots = parser!.Schema!.TotalChannels;
            analogChannelsCount = parser!.Schema!.TotalAnalogChannels;
            numberOfSamples = parser.Schema.TotalSamples;
            numberOfSamplesPerPeriod = (int)Math.Round(parser!.Schema!.SampleRates![0].Rate / parser.Schema.NominalFrequency);
            wasVectorDiagramScaled = false;

            // Добавление каналов
            for (int i = 0; i < numberOfPlots; i++)
                channelsData.Add(new List<double>());

            // Добавление аналоговых каналов
            for (int i = 0; i < analogChannelsCount; i++)
            {
                firstHarmonicAmplitude.Add(new double());
                firstHarmonicPhase.Add(new double());
                RootMeanSquare.Add(new double());
                amplitudesForSpectralDiagram.Add(new double[numberOfSamplesPerPeriod]);
            }

            // Считывание данных и временных меток из синтаксического анализатора
            try
            {
                parser.OpenFiles();
            }
            catch (Exception ex) 
            {
                isParserInitialized = false;
                throw new Exception(ex.Message);
            }

            double fileTimeOffset = 0;
            double timeFinalized;

            TimeOffset offset = parser.Schema!.TimeCode;
            DateTime startTime = new(parser.Schema.StartTime.Value);
            DateTime timeStamp;
            Ticks timeSpan;

            for (int i = 0; i < numberOfSamples; i++)
            {
                parser.ReadNext();

                // Обработка временных параметров
                timeStamp = new DateTime(parser.Timestamp.Ticks - offset.TickOffset);

                timeSpan = (timeStamp - startTime) / (10 * parser.Schema.TimeFactor); // 10, потому что в микросекунде 10 тактов

                timeFinalized = timeSpan; // Временная метка в микросекундах

                if (i == 0)
                {
                    fileTimeOffset = timeFinalized; // Смещение на первую временную метку, чтобы отсчет начинался с 0
                    firstTimeStamp = timeStamp;
                }

                timeStamps.Add((timeFinalized - fileTimeOffset) / 1000); // Преобразование в миллисекунды

                for (int j = 0; j < numberOfPlots; j++)
                    channelsData[j].Add(parser.Values![j]); // Добавление значения для j канала
            }

            // Установка заглушки на случай чтения пустого файла
            if (numberOfSamples == 0)
            {
                numberOfSamples++;
                timeStamps.Add(0);
                firstTimeStamp = parser.Schema.StartTime.Value;

                for (int i = 0; i < numberOfPlots; i++)
                    channelsData[i].Add(0);
            }
        }

        // Подготовка панели с графиками сигналов
        private void CreateSignalPlots(MainWindowForm mainForm)
        {
            // Поиск панели на форме
            Panel signalPlotsPanel = (Panel)mainForm.Controls["signalPlotsPanel"]!;

            // Очистка элементов управления панели
            signalPlotsPanel.Controls.Clear();

            // Добавление элементов управления на форму и их настройка
            for (int i = 0; i < numberOfPlots; i++)
            {
                Label valueLabel = new()
                {
                    AutoSize = true,
                    Name = "valueLabel" + i
                };

                Label maxValueLabel = new()
                {
                    AutoSize = true,
                    Name = "maxValueLabel" + i
                };

                Label minValueLabel = new()
                {
                    AutoSize = true,
                    Name = "minValueLabel" + i
                };

                Label RMSLabel = new()
                {
                    AutoSize = true,
                    Name = "RMSLabel" + i
                };

                Label A1Label = new()
                {
                    AutoSize = true,
                    Name = "A1Label" + i
                };

                FormsPlot signalPlot = new()
                {
                    Name = "signalPlot" + i,
                    BackColor = System.Drawing.Color.White
                };

                Panel separator = new()
                {
                    Name = "separator" + i,
                    BackColor = System.Drawing.Color.Black,
                    Height = 1,
                    Width = 100
                };

                if (i < analogChannelsCount) // Аналоговые каналы
                {
                    channelsNames.Add(parser!.Schema!.AnalogChannels![i].Name!);
                    analogChannelsUnits.Add(parser.Schema.AnalogChannels![i].Units!);

                    valueLabel.Text = $"{channelsNames[i]}: {channelsData[i][0].ToString("F3", CultureInfo.InvariantCulture)} {analogChannelsUnits[i]}";
                    maxValueLabel.Text = $"Max: {channelsData[i].Max().ToString("F3", CultureInfo.InvariantCulture)} {analogChannelsUnits[i]}";
                    minValueLabel.Text = $"Min: {channelsData[i].Min().ToString("F3", CultureInfo.InvariantCulture)} {analogChannelsUnits[i]}";
                    RMSLabel.Text = "RMS: 0 " + analogChannelsUnits[i];
                    A1Label.Text = "A1: 0 " + analogChannelsUnits[i];

                    valueLabel.Location = new Point(0, 3 + i * AnalogSignalPlotDefaultHeight - 1);

                    maxValueLabel.Location = new Point(0, 23 + i * AnalogSignalPlotDefaultHeight - 1);
                    signalPlotsPanel.Controls.Add(maxValueLabel);

                    minValueLabel.Location = new Point(0, 43 + i * AnalogSignalPlotDefaultHeight - 1);
                    signalPlotsPanel.Controls.Add(minValueLabel);

                    RMSLabel.Location = new Point(0, 63 + i * AnalogSignalPlotDefaultHeight - 1);
                    signalPlotsPanel.Controls.Add(RMSLabel);

                    A1Label.Location = new Point(0, 83 + i * AnalogSignalPlotDefaultHeight - 1);
                    signalPlotsPanel.Controls.Add(A1Label);

                    separator.Location = new Point(0, AnalogSignalPlotDefaultHeight + i * AnalogSignalPlotDefaultHeight - 1);
                    signalPlotsPanel.Controls.Add(separator);

                    signalPlot.Size = i == numberOfPlots - 1 ? new Size(signalPlotsPanel.Width - 102, AnalogSignalPlotDefaultHeight + 20) : new Size(signalPlotsPanel.Width - 102, AnalogSignalPlotDefaultHeight);
                    signalPlot.Location = new Point(100, i * AnalogSignalPlotDefaultHeight - 1);
                }
                else // Дискретные каналы
                {
                    channelsNames.Add(parser!.Schema!.DigitalChannels![i - analogChannelsCount].Name!);

                    valueLabel.Text = $"{channelsNames[i]}: {channelsData[i][0]}";

                    signalPlot.Size = i == numberOfPlots - 1 ? new Size(signalPlotsPanel.Width - 102, DigitalSignalPlotDefaultHeight + 20) : new Size(signalPlotsPanel.Width - 102, DigitalSignalPlotDefaultHeight);

                    if (i == analogChannelsCount)
                    {
                        valueLabel.Location = new Point(0, 18 + i * AnalogSignalPlotDefaultHeight - 1);
                        separator.Location = new Point(0, 50 + i * AnalogSignalPlotDefaultHeight - 1);
                        signalPlot.Location = new Point(100, i * AnalogSignalPlotDefaultHeight - 1);
                    }
                    else if (i == numberOfPlots - 1)
                    {
                        valueLabel.Location = new Point(0, 19 + analogChannelsCount * AnalogSignalPlotDefaultHeight + (i - analogChannelsCount) * DigitalSignalPlotDefaultHeight - 1);
                        separator.Location = new Point(0, 52 + analogChannelsCount * AnalogSignalPlotDefaultHeight + (i - analogChannelsCount) * DigitalSignalPlotDefaultHeight - 1);
                        signalPlot.Location = new Point(100, analogChannelsCount * AnalogSignalPlotDefaultHeight + (i - analogChannelsCount) * DigitalSignalPlotDefaultHeight - 1);
                    }
                    else
                    {
                        valueLabel.Location = new Point(0, 18 + analogChannelsCount * AnalogSignalPlotDefaultHeight + (i - analogChannelsCount) * DigitalSignalPlotDefaultHeight - 1);
                        separator.Location = new Point(0, 50 + analogChannelsCount * AnalogSignalPlotDefaultHeight + (i - analogChannelsCount) * DigitalSignalPlotDefaultHeight - 1);
                        signalPlot.Location = new Point(100, analogChannelsCount * AnalogSignalPlotDefaultHeight + (i - analogChannelsCount) * DigitalSignalPlotDefaultHeight - 1);
                    }
                }

                signalPlotsPanel.Controls.Add(valueLabel);
                signalPlotsPanel.Controls.Add(separator);

                // Добавление графиков сигналов
                var signal = signalPlot.Plot.Add.SignalXY(timeStamps, channelsData[i]);

                signal.MarkerStyle.Shape = MarkerShape.FilledCircle;
                signal.MarkerStyle.Size = 4;

                if (i >= analogChannelsCount)
                    signal.ConnectStyle = ConnectStyle.StepHorizontal;
                else
                    signal.Color = model.ColorMap[i % model.ColorMap.Count];

                signals.Add(signal);

                // Настройка графиков
                signalPlot.Plot.Axes.SetLimitsX(timeStamps[0] - 0.1, timeStamps[^1] + 0.1);
                signalPlot.Plot.Grid.YAxisStyle.IsVisible = false;
                signalPlot.Plot.Add.HorizontalLine(0, 0.1F, ScottPlot.Color.Gray(0));
                signalPlot.Plot.Layout.Fixed(new PixelPadding(0, 2, 0, 0));

                if (i % 2 != 0)
                    signalPlot.Plot.DataBackground.Color = Colors.WhiteSmoke;

                if (i == numberOfPlots - 1)
                {
                    signalPlot.Plot.Axes.Bottom.Label.Text = "lastPlot";
                    signalPlot.Plot.Layout.Fixed(new PixelPadding(0, 2, 20, 0)); // У последнего графика другой отступ снизу, чтобы было видно временную ось
                }

                signalPlot.MouseDown += (sender, e) => SignalPlotMouseDown(sender!, e, mainForm);
                signalPlot.MouseMove += (sender, e) => SignalPlotMouseMove(sender!, e, mainForm);
                signalPlot.MouseUp += (sender, e) => SignalPlotMouseUp(sender!, e);

                signalPlot.UserInputProcessor.LeftClickDragPan(false);
                signalPlot.UserInputProcessor.RightClickDragZoom(false);
                signalPlot.UserInputProcessor.DoubleLeftClickBenchmark(false);
                signalPlot.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseWheelZoom>();
                signalPlot.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoomRectangle>();
                signalPlot.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.KeyboardAutoscale>();
                signalPlot.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.KeyboardPanAndZoom>();
                signalPlot.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.SingleClickAutoscale>();

                signalPlot.Menu!.Clear();

                int plotIndex = i;

                signalPlot.Menu.Add("Открыть в новом окне", (plot) => OpenInNewWindow(plot, timeStamps[0] - 0.1, timeStamps[^1] + 0.1));
                signalPlot.Menu.Add("Скопировать в буфер обмена", (plot) => CopyImageToClipboard(plot, true));
                signalPlot.Menu.Add("Сохранить изображение", (plot) => OpenSaveImageDialog(plot, true));

                if (i < analogChannelsCount)
                    signalPlot.Menu.Add("Добавить горизонтальные визиры", (plot) => AddHorizontalVisors(plot, plotIndex, signalPlot));

                // Добавление и настройка визиров
                var firstVertVisor = signalPlot.Plot.Add.VerticalLine(timeStamps[0], 2, Colors.Red);
                var secondVertVisor = signalPlot.Plot.Add.VerticalLine(timeStamps[0], 2, Colors.Blue);

                firstVertVisors.Add(firstVertVisor);
                secondVertVisors.Add(secondVertVisor);

                secondVertVisors[i].IsVisible = false;

                // Добавление графиков сигналов на панель
                signalPlotsPanel.Controls.Add(signalPlot);
                signalPlots.Add(signalPlot);

                // Обновление графиков сигналов
                signalPlot.Refresh();
            }

            // Установка позиций вертикальных визиров
            firstVisorSample = 0;
            secondVisorSample = -1; // Изначально не установлен

            // Создание массивов под горизонтальные визиры
            firstHorVisors = new HorizontalLine[analogChannelsCount];
            secondHorVisors = new HorizontalLine[analogChannelsCount];

            // Настройка панелей
            Panel buttonsPanel = (Panel)mainForm.Controls["buttonsPanel"]!;
            ((Button)buttonsPanel.Controls["cropPlotButton"]!).Enabled = false;
            ((Button)buttonsPanel.Controls["resetScaleButton"]!).PerformClick();

            signalPlotsPanel.BackColor = SystemColors.ControlLightLight;

            if (signalPlotsPanel.HorizontalScroll.Visible)
            {
                for (int i = 0; i < numberOfPlots; i++)
                {
                    if (i < analogChannelsCount)
                        signalPlots[i].Size = i == numberOfPlots - 1 ? new Size(signalPlotsPanel.Width - 119, AnalogSignalPlotDefaultHeight + 20) : new Size(signalPlotsPanel.Width - 119, AnalogSignalPlotDefaultHeight);
                    else
                        signalPlots[i].Size = i == numberOfPlots - 1 ? new Size(signalPlotsPanel.Width - 119, AnalogSignalPlotDefaultHeight / 2 + 20) : new Size(signalPlotsPanel.Width - 119, AnalogSignalPlotDefaultHeight / 2);

                    signalPlots[i].Refresh();
                }
            }
        }

        private bool isDraggingVerticalVisor = false; // Флаг, указывающий, выполняется ли перетаскивание вертикального визира
        private AxisLine? HorizontalVisorBeingDragged = null; // Перетаскиваемый горизонтальный визир
        private int initialMouseX; // Координата X мыши при начале перетаскивания

        // Событие нажатия на кнопку мыши
        private void SignalPlotMouseDown(object sender, MouseEventArgs e, MainWindowForm mainForm)
        {
            if (sender is FormsPlot senderPlot)
            {
                // Поиск горизонтального визира под курсором мыши
                var lineUnderMouse = GetLineUnderMouse(e.X, e.Y, senderPlot);
                if (lineUnderMouse != null && lineUnderMouse is HorizontalLine)
                {
                    HorizontalVisorBeingDragged = lineUnderMouse;

                    // Завершение обработки при нахождении визира
                    return;
                }

                // Перемещение вертикальных визиров к курсору
                if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle)
                {
                    initialMouseX = e.X;

                    double mousePlotXCoord = senderPlot.Plot.GetCoordinates(new Pixel(e.X, e.Y)).X;
                    int closestSample = FindClosest(timeStamps, mousePlotXCoord);
                    Panel buttonsPanel = (Panel)mainForm.Controls["buttonsPanel"]!;

                    // Перемещение первого визира и перевод соответствующей кнопки в режим нажатой
                    if (e.Button == MouseButtons.Left)
                    {
                        MoveSample(mainForm, closestSample);
                        Button firstVertVisorChangeVisibilityButton = (Button)buttonsPanel.Controls["firstVertVisorChangeVisibilityButton"]!;
                        firstVertVisorChangeVisibilityButton.BackColor = SystemColors.GradientActiveCaption;
                        firstVertVisorChangeVisibilityButton.FlatStyle = FlatStyle.Flat;
                    }

                    // Перемещение второго визира и перевод соответствующей кнопки в режим нажатой
                    if (e.Button == MouseButtons.Middle)
                    {
                        MoveSecondVisor(mainForm, closestSample);
                        Button secondVertVisorChangeVisibilityButton = (Button)buttonsPanel.Controls["secondVertVisorChangeVisibilityButton"]!;
                        secondVertVisorChangeVisibilityButton.BackColor = SystemColors.GradientActiveCaption;
                        secondVertVisorChangeVisibilityButton.FlatStyle = FlatStyle.Flat;
                    }

                    isDraggingVerticalVisor = true;
                }
            }
        }

        // Событие движения мыши
        private void SignalPlotMouseMove(object sender, MouseEventArgs e, MainWindowForm mainForm)
        {
            if (sender is FormsPlot senderPlot)
            {
                // Стилизация курсора
                double mousePlotXCoord = senderPlot.Plot.GetCoordinates(new Pixel(e.X, e.Y)).X;
                double gap = 0.1;

                if ((mousePlotXCoord > firstVertVisors[0].Position - gap && mousePlotXCoord < firstVertVisors[0].Position + gap) || (mousePlotXCoord > secondVertVisors[0].Position - gap && mousePlotXCoord < secondVertVisors[0].Position + gap))
                    senderPlot.Cursor = Cursors.SizeWE;
                else
                {
                    if (!isDraggingVerticalVisor)
                        senderPlot.Cursor = Cursors.Default;
                }

                if (HorizontalVisorBeingDragged == null)
                {
                    var lineUnderMouse = GetLineUnderMouse(e.X, e.Y, senderPlot);
                    if (lineUnderMouse != null && lineUnderMouse.IsDraggable && lineUnderMouse is HorizontalLine)
                        senderPlot.Cursor = Cursors.SizeNS;
                }
                else
                {
                    if (HorizontalVisorBeingDragged is HorizontalLine horVisor)
                    {
                        // Перемещение визиров и их стилизация
                        CoordinateRect rect = senderPlot.Plot.GetCoordinateRect(e.X, e.Y, radius: 10);
                        horVisor.Y = rect.VerticalCenter;
                        int i = int.Parse(horVisor.Text);

                        if (firstHorVisors![i].Y == secondHorVisors![i].Y) // При пересечении визиры окрашиваются в зеленый
                        {
                            firstHorVisors[i].Color = Colors.Green;
                            secondHorVisors[i].Color = Colors.Green;
                        }
                        else // Цвета по умолчанию: красный для первого, синий для второго
                        {
                            firstHorVisors[i].Color = Colors.Red;
                            secondHorVisors[i].Color = Colors.Blue;
                        }

                        // Обновление панели с координатами визиров
                        Panel visorsDataPanel = (Panel)mainForm.Controls["visorsDataPanel"]!;

                        ((Label)visorsDataPanel.Controls["valueFirstHorVisorLabel"]!).Text = "Гор. визир 1: " + firstHorVisors[i].Position.ToString("F3", CultureInfo.InvariantCulture) + " " + analogChannelsUnits[i];
                        ((Label)visorsDataPanel.Controls["valueSecondHorVisorLabel"]!).Text = "Гор. визир 1: " + secondHorVisors[i].Position.ToString("F3", CultureInfo.InvariantCulture) + " " + analogChannelsUnits[i];
                        ((Label)visorsDataPanel.Controls["valueHorVisorsDeltaLabel"]!).Text = "Разница гор. визиров: " + Math.Abs((firstHorVisors[i].Position - secondHorVisors[i].Position)).ToString("F3", CultureInfo.InvariantCulture) + " " + analogChannelsUnits[i];
                    }
                    senderPlot.Refresh();
                }

                if (isDraggingVerticalVisor)
                {
                    int deltaX = e.X - initialMouseX; // Вычисление смещения мыши по горизонтали
                    Panel signalPlotsPanel = (Panel)mainForm.Controls["signalPlotsPanel"]!;

                    if (deltaX > (signalPlotsPanel.Width - 100) / numberOfSamples) // Смещение вправо (вперед)
                    {
                        try
                        {
                            if (e.Button == MouseButtons.Left)
                                MoveForthCommand.Execute(mainForm); // Левая кнопка мыши перемещает первый визир
                            if (e.Button == MouseButtons.Middle)
                                MoveSecondForthCommand.Execute(mainForm); // Правая кнопка мыши перемещает второй визир
                        }
                        catch { }
                        initialMouseX = e.X; // Сброс начальной координаты
                    }
                    else if (deltaX < -(signalPlotsPanel.Width - 100) / numberOfSamples)  // Смещение влево (назад)
                    {
                        try
                        {
                            if (e.Button == MouseButtons.Left)
                                MoveBackCommand.Execute(mainForm); // Левая кнопка мыши перемещает первый визир
                            if (e.Button == MouseButtons.Middle)
                                MoveSecondBackCommand.Execute(mainForm); // Правая кнопка мыши перемещает второй визир
                        }
                        catch { }
                        initialMouseX = e.X; // Сброс начальной координаты
                    }
                }
            }
        }

        // Событие отпускания кнопки мыши
        private void SignalPlotMouseUp(object sender, MouseEventArgs e)
        {
            isDraggingVerticalVisor = false;
            HorizontalVisorBeingDragged = null;
        }

        // Подготовка панели с векторной диаграммой
        private void CreateVectorDiagram(Panel vectorDiagramPanel)
        {
            // Очистка элементов управления панели
            vectorDiagramPanel.Controls.Clear();

            // Создание и настройка векторной диаграммы
            FormsPlot vectorDiagram = new()
            {
                Name = "vectorDiagram"
            };

            vectorDiagram.Plot.DataBackground.Color = ScottPlot.Color.FromColor(SystemColors.ControlLightLight);

            vectorDiagram.Size = new Size(vectorDiagramPanel.Width + 30, vectorDiagramPanel.Width + 30);
            vectorDiagram.Location = new Point(-17, 200);

            vectorDiagram.UserInputProcessor.LeftClickDragPan(false);
            vectorDiagram.UserInputProcessor.RightClickDragZoom(false);
            vectorDiagram.UserInputProcessor.DoubleLeftClickBenchmark(false);
            vectorDiagram.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseWheelZoom>();
            vectorDiagram.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoomRectangle>();
            vectorDiagram.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.KeyboardAutoscale>();
            vectorDiagram.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.KeyboardPanAndZoom>();
            vectorDiagram.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.SingleClickAutoscale>();

            vectorDiagram.Menu!.Clear();
            vectorDiagram.Menu.Add("Скопировать в буфер обмена", (plot) => CopyImageToClipboard(plot, false));
            vectorDiagram.Menu.Add("Сохранить изображение", (plot) => OpenSaveImageDialog(plot, false));
            vectorDiagram.Menu.Add("Обновить масштаб", (plot) => UpdateVectorScale(vectorDiagramPanel));

            // Создание и настройка таблицы со значениями первых гармоник
            TableLayoutPanel firstHarmonicTable = new()
            {
                Name = "firstHarmonicTable",

                Size = new Size(vectorDiagramPanel.Width - 50, 25 * analogChannelsCount + 3)
            };

            firstHarmonicTable.Location = new Point((vectorDiagramPanel.Width - firstHarmonicTable.Width) / 2, 700);

            SetDoubleBuffered(firstHarmonicTable, true);

            firstHarmonicTable.ColumnCount = 4;
            firstHarmonicTable.RowCount = analogChannelsCount;

            firstHarmonicTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30f));
            firstHarmonicTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10f));
            firstHarmonicTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30f));
            firstHarmonicTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30f));

            firstHarmonicTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            vectorDiagramPanel.Controls.Add(firstHarmonicTable);

            // Заполнение таблицы
            for (int i = 0; i < analogChannelsCount; i++)
            {
                firstHarmonicTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));

                for (int col = 0; col < firstHarmonicTable.ColumnCount; col++)
                {
                    Label label = new()
                    {
                        Location = new Point(0, 5)
                    };

                    if (col == 0) // Название аналогового канала
                    {
                        label.TextAlign = ContentAlignment.MiddleLeft;
                        label.Text = channelsNames[i].ToString();
                        label.Name = "channelNameLabel" + i;
                    }
                    else if (col == 1) // Цвет графика сигнала
                    {
                        Bitmap bmp = new(16, 16);
                        PictureBox pictureBox = new();

                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            g.Clear(model.ColorMap[i % model.ColorMap.Count].ToSDColor());
                        }

                        pictureBox.Image = bmp;
                        pictureBox.Dock = DockStyle.Fill;
                        pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;

                        firstHarmonicTable.Controls.Add(pictureBox, col, i);
                        continue;
                    }
                    else if (col == 2) // Фазовый угол
                    {
                        label.TextAlign = ContentAlignment.MiddleCenter;
                        label.Name = "phaseLabel" + i;
                    }
                    else if (col == 3) // Амплитуда
                    {
                        label.TextAlign = ContentAlignment.MiddleRight;
                        label.Name = "amplitudeLabel" + i;
                    }

                    firstHarmonicTable.Controls.Add(label, col, i);
                }
            }

            // Определение максимальных значений амплитуд токов и напряжений первых гармоник 
            double maxValue = 0;

            if (firstHarmonicAmplitude.Count > 0)
                maxValue = firstHarmonicAmplitude.Max();

            double maxCurrent = 0, maxVoltage = 0; // Значения по умолчанию
            string maxCurrentUnits = "A", maxVoltageUnits = "V"; // Единицы измерения по умолчанию

            if (maxValue > 0)
            {
                for (int i = 0; i < analogChannelsCount; i++)
                {
                    if (model.CurrentUnits.Contains(analogChannelsUnits[i]))
                    {
                        if (firstHarmonicAmplitude!.Max() > maxCurrent)
                        {
                            maxCurrent = firstHarmonicAmplitude.Max();
                            maxCurrentUnits = analogChannelsUnits[i];
                        }
                    }
                    else if (model.VoltageUnits.Contains(analogChannelsUnits[i]))
                    {
                        if (firstHarmonicAmplitude!.Max() > maxVoltage)
                        {
                            maxVoltage = firstHarmonicAmplitude.Max();
                            maxVoltageUnits = analogChannelsUnits[i];
                        }
                    }
                }
            }

            // Создание и настройка таблицы с максимальными значениями
            TableLayoutPanel maxCurrentAndVoltage = new()
            {
                Size = new Size(120, 43),
                Location = new Point(vectorDiagramPanel.Width - 130, 40),
                BackColor = System.Drawing.Color.FromArgb(255, 254, 220),
                ColumnCount = 2,
                RowCount = 2
            };

            maxCurrentAndVoltage.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 41f));
            maxCurrentAndVoltage.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 59f));

            maxCurrentAndVoltage.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            vectorDiagramPanel.Controls.Add(maxCurrentAndVoltage);

            // Заполнение таблицы
            maxCurrentAndVoltage.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));

            Label maxVoltageLabel = new()
            {
                Text = "U max",
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label maxCurrentLabel = new()
            {
                Text = "I max",
                TextAlign = ContentAlignment.MiddleLeft
            };

            maxCurrentAndVoltage.Controls.Add(maxVoltageLabel, 0, 0);
            maxCurrentAndVoltage.Controls.Add(maxCurrentLabel, 0, 1);

            maxCurrentAndVoltage.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));

            Label maxVoltageValueLabel = new()
            {
                Text = maxVoltage.ToString("F3", CultureInfo.InvariantCulture) + " " + maxVoltageUnits,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label maxCurrentValueLabel = new()
            {
                Text = maxCurrent.ToString("F3", CultureInfo.InvariantCulture) + " " + maxCurrentUnits,
                TextAlign = ContentAlignment.MiddleLeft
            };

            maxCurrentAndVoltage.Controls.Add(maxVoltageValueLabel, 1, 0);
            maxCurrentAndVoltage.Controls.Add(maxCurrentValueLabel, 1, 1);

            // Создание и настройка заголовка панели
            Label vectorDiagramPanelLabel = new()
            {
                Text = "Векторная диаграмма",
                BackColor = SystemColors.GradientActiveCaption,
                Width = vectorDiagramPanel.Width,
                Height = 18,
                Location = new Point(0, 0)
            };

            vectorDiagramPanel.Controls.Add(vectorDiagramPanelLabel);

            // Добавление на диаграмму полярной оси и ее стилизация
            var polarAxis = vectorDiagram.Plot.Add.PolarAxis(maxValue);
            polarAxis.Rotation = ScottPlot.Angle.FromDegrees(90);
            polarAxis.Circles.ForEach(x => x.LinePattern = LinePattern.Dotted);
            polarAxis.Circles.ForEach(x => x.LineWidth = 1);
            polarAxis.Circles.Last().LinePattern = LinePattern.Solid;
            polarAxis.Spokes.ForEach(x => x.LinePattern = LinePattern.Dotted);
            polarAxis.Spokes.ForEach(x => x.LineWidth = 1);
            polarAxis.Spokes.ForEach(x => x.Length = maxValue);
            polarAxis.Spokes.ForEach(x => x.LabelPaddingFraction = -0.09);

            List<int> indicesToClear = [1, 2, 4, 5, 7, 8, 10, 11];

            for (int i = 0; i < polarAxis.Spokes.Count; i++)
                if (indicesToClear.Contains(i))
                    polarAxis.Spokes[i].LabelText = "";

            polarAxis.Spokes[0].LabelText = "0°";
            polarAxis.Spokes[0].LabelStyle.OffsetX = 3;
            polarAxis.Spokes[0].LabelStyle.OffsetY = 9;

            polarAxis.Spokes[3].LabelText = "90°";
            polarAxis.Spokes[3].LabelStyle.OffsetX = 7;

            polarAxis.Spokes[6].LabelText = "180°";
            polarAxis.Spokes[6].LabelStyle.OffsetX = -6;
            polarAxis.Spokes[6].LabelStyle.OffsetY = 15;

            polarAxis.Spokes[9].LabelText = "-90°";
            polarAxis.Spokes[9].LabelStyle.OffsetX = 7;
            polarAxis.Spokes[9].LabelStyle.OffsetY = -2;

            // Добавление на диаграмму векторов и их стилизация 
            Coordinates center = polarAxis.GetCoordinates(0, 0);

            for (int i = 0; i < analogChannelsCount; i++)
            {
                var vector = vectorDiagram.Plot.Add.Arrow(center, center);
                vector.ArrowLineWidth = 0;
                vector.ArrowWidth = 3;
                vector.ArrowheadLength = 20;
                vector.ArrowheadAxisLength = 20;
                vector.ArrowheadWidth = 7;
                vector.ArrowFillColor = model.ColorMap[i % model.ColorMap.Count];

                vector.IsVisible = false;

                vectors.Add(vector);
            }

            // Добавление векторной диаграммы на панель
            vectorDiagramPanel.Controls.Add(vectorDiagram);

            // Обновление векторной диаграммы
            vectorDiagram.Refresh();

            // Настройка панели
            vectorDiagramPanel.AutoScroll = false;

            if (analogChannelsCount > 6)
            {
                vectorDiagramPanel.AutoScroll = true;
                firstHarmonicTable.Size = new Size(vectorDiagramPanel.Width - 50, 25 * analogChannelsCount + 25);
                firstHarmonicTable.Location = new Point((vectorDiagramPanel.Width - firstHarmonicTable.Width) / 2 - 10, 700);
                vectorDiagram.Size = new Size(vectorDiagramPanel.Width + 15, vectorDiagramPanel.Width + 30);
                maxCurrentAndVoltage.Location = new Point(vectorDiagramPanel.Width - 145, 40);
            }
        }

        // Подготовка панели со спектральными диаграммами
        private void CreateSpectralDiagram(Panel spectralDiagramPanel)
        {
            // Очистка элементов управления панели
            spectralDiagramPanel.Controls.Clear();

            // Создание заголовка и настройка панели
            Label spectralDiagramPanelLabel = new()
            {
                Text = "Спектральная диаграмма",
                BackColor = SystemColors.GradientActiveCaption
            };

            spectralDiagramPanel.AutoScroll = false;

            if (analogChannelsCount > 6)
            {
                spectralDiagramPanelLabel.Width = spectralDiagramPanel.Width / 6 * analogChannelsCount - 1;
                spectralDiagramPanel.Height = 234 - 20;
                spectralDiagramPanel.AutoScroll = true;
            }
            else
            {
                spectralDiagramPanelLabel.Width = spectralDiagramPanel.Width - 2;
                spectralDiagramPanel.Height = 234;
            }

            spectralDiagramPanelLabel.Height = 18;
            spectralDiagramPanelLabel.Location = new Point(0, 0);

            spectralDiagramPanel.Controls.Add(spectralDiagramPanelLabel);

            // Создание спектральных диаграмм
            for (int i = 0; i < analogChannelsCount; i++)
            {
                FormsPlot spectralDiagram = new()
                {
                    Name = "spectralDiagram" + i
                };

                if (analogChannelsCount > 6)
                    spectralDiagram.Width = spectralDiagramPanel.Width / 6;
                else
                    spectralDiagram.Width = spectralDiagramPanel.Width / analogChannelsCount;

                spectralDiagram.Height = spectralDiagramPanel.Height;
                spectralDiagram.Location = new Point(i * spectralDiagram.Width - 1, -22);
                spectralDiagram.BackColor = SystemColors.ControlLightLight;

                double nyquistFrequency = parser!.Schema!.SampleRates![0].Rate / 2;
                double nominalFrequency = parser.Schema.NominalFrequency;

                int barsCount = (int)(nyquistFrequency / nominalFrequency) + 1; // + 1, потому что еще постоянная составляющая

                double[] positions = new double[barsCount];
                double[] values = new double[barsCount];

                positions = Enumerable.Range(0, barsCount)
                    .Select(i => i * nominalFrequency)
                    .ToArray();

                // Добавление на диаграммы столбцов и их настройка
                var barPlot = spectralDiagram.Plot.Add.Bars(positions, values);
                barPlot.ValueLabelStyle.FontSize = 12;

                foreach (var bar in barPlot.Bars)
                {
                    bar.Label = bar.Value.ToString();
                    bar.Size = 20;
                    bar.FillColor = model.ColorMap[i % model.ColorMap.Count];
                    bar.LineColor = bar.FillColor.Darken(0.75);
                }

                barPlots.Add(barPlot);

                // Настройка спектральных диаграмм
                spectralDiagram.Plot.Axes.Margins(bottom: 0, top: .2);
                spectralDiagram.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericFixedInterval(nominalFrequency);
                spectralDiagram.Plot.Layout.Fixed(new PixelPadding(0, 0, 40, 40));
                spectralDiagram.Plot.HideGrid();
                spectralDiagram.Plot.Axes.GetXAxes().Last().IsVisible = false;
                spectralDiagram.Plot.Axes.SetLimitsY(0, 120);
                spectralDiagram.Plot.Axes.Bottom.Label.Text = channelsNames[i] + ", %";
                spectralDiagram.Plot.Axes.Bottom.Label.FontSize = 12;
                spectralDiagram.Plot.Axes.Bottom.Label.OffsetY = -3;
                spectralDiagram.Plot.Axes.Bottom.Label.Bold = false;

                spectralDiagram.UserInputProcessor.LeftClickDragPan(false);
                spectralDiagram.UserInputProcessor.RightClickDragZoom(false);
                spectralDiagram.UserInputProcessor.DoubleLeftClickBenchmark(false);
                spectralDiagram.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseWheelZoom>();
                spectralDiagram.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.MouseDragZoomRectangle>();
                spectralDiagram.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.KeyboardAutoscale>();
                spectralDiagram.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.KeyboardPanAndZoom>();
                spectralDiagram.UserInputProcessor.RemoveAll<ScottPlot.Interactivity.UserActionResponses.SingleClickAutoscale>();

                spectralDiagram.Menu!.Clear();
                spectralDiagram.Menu.Add("Скопировать в буфер обмена", (plot) => CopyImageToClipboard(plot, false));
                spectralDiagram.Menu.Add("Сохранить изображение", (plot) => OpenSaveImageDialog(plot, false));
                
                // Добавление спектральных диаграмм на панель
                spectralDiagramPanel.Controls.Add(spectralDiagram);

                // Обновление спектральных диаграмм
                spectralDiagram.Refresh();
            }
        }

        // Открытие окна сохранения файла
        private void ShowSave()
        {
            Tuple<Schema, DateTime, List<List<double>>> schemaTimeStampData = new(parser!.Schema!, firstTimeStamp, channelsData);

            SaveForm saveForm = new(schemaTimeStampData);
            saveForm.ShowDialog();
        }

        // Открытие окна конфигурации файла
        private void ShowConfig()
        {
            ConfigForm configForm = new(parser!.Schema!);
            configForm.ShowDialog();
        }

        // Закрытие программы
        private void CloseProgram()
        {
            if (isParserInitialized)
                parser!.Dispose();

            Application.Exit();
        }

        // Проверка на возможность перемещения первого вертикального визира вперед
        private bool CanMoveForth()
        {
            return isParserInitialized && firstVisorSample != numberOfSamples - 1;
        }

        // Перемещение первого вертикального визира вперед
        private void MoveForth(object? obj)
        {
            if (obj is MainWindowForm mainForm)
                MoveSample(mainForm, firstVisorSample + 1);
        }

        // Проверка на возможность перемещения первого вертикального визира назад
        private bool CanMoveBack()
        {
            return isParserInitialized && firstVisorSample != 0;
        }

        // Перемещение первого вертикального визира назад
        private void MoveBack(object? obj)
        {
            if (obj is MainWindowForm mainForm)
                MoveSample(mainForm, firstVisorSample - 1);
        }

        // Перемещение первого вертикального визира
        private void MoveSample(MainWindowForm mainForm, int targetSample)
        {
            // Обновление панелей с графиками сигналов и координатами визиров
            Panel signalPlotsPanel = (Panel)mainForm.Controls["signalPlotsPanel"]!;
            Panel visorsDataPanel = (Panel)mainForm.Controls["visorsDataPanel"]!;

            UpdateSignalPlots(signalPlotsPanel, visorsDataPanel, targetSample);

            // Обновление панелей с векторной и спектральными диаграммами
            if (firstHarmonicAmplitude.Count > 0 && firstVisorSample >= numberOfSamplesPerPeriod - 1) // До окончания первого периода отрисовка невозможна
            {
                Panel vectorDiagramPanel = (Panel)mainForm.Controls["vectorDiagramPanel"]!;
                Panel spectralDiagramPanel = (Panel)mainForm.Controls["spectralDiagramPanel"]!;

                AnalyzeSpectrum(firstVisorSample);

                if (!wasVectorDiagramScaled)
                {
                    UpdateVectorScale(vectorDiagramPanel);
                    wasVectorDiagramScaled = true;
                }

                UpdateVectorDiagram(vectorDiagramPanel, signalPlotsPanel);
                UpdateSpectralDiagram(spectralDiagramPanel);
            }
            else // В случае выбора точки первого периода диаграммы скрываются, а показатели зануляются
            {
                if (vectors.Count > 0 && vectors[0].IsVisible) // Скрытие происходит лишь в случае предварительного отображения
                {
                    Panel vectorDiagramPanel = (Panel)mainForm.Controls["vectorDiagramPanel"]!;
                    FormsPlot vectorDiagram = (FormsPlot)vectorDiagramPanel.Controls["vectorDiagram"]!;
                    TableLayoutPanel firstHarmonicTable = (TableLayoutPanel)vectorDiagramPanel.Controls["firstHarmonicTable"]!;

                    Panel spectralDiagramPanel = (Panel)mainForm.Controls["spectralDiagramPanel"]!;

                    for (int i = 0; i < analogChannelsCount; i++)
                    {
                        // Скрытие векторной диаграммы
                        vectors[i].IsVisible = false;

                        // Зануление показателей
                        ((Label)firstHarmonicTable.Controls["phaseLabel" + i]!).Text = 0 + "°";
                        ((Label)firstHarmonicTable.Controls["amplitudeLabel" + i]!).Text = 0.ToString("F3", CultureInfo.InvariantCulture) + " " + analogChannelsUnits[i];
                        ((Label)signalPlotsPanel.Controls["A1Label" + i]!).Text = "A1: 0 " + analogChannelsUnits[i];
                        ((Label)signalPlotsPanel.Controls["RMSLabel" + i]!).Text = "RMS: 0 " + analogChannelsUnits[i];

                        // Зануление значений столбцов спектральных диаграмм
                        FormsPlot spectralDiagram = (FormsPlot)spectralDiagramPanel.Controls["spectralDiagram" + i]!;

                        foreach (var bar in barPlots[i].Bars)
                        {
                            bar.Value = 0;
                            bar.Label = bar.Value.ToString();
                        }

                        spectralDiagram.Refresh();
                    }

                    vectorDiagram.Refresh();
                }
            }

            // Обновление панели с кнопками
            Panel buttonsPanel = (Panel)mainForm.Controls["buttonsPanel"]!;

            if (firstVisorSample == numberOfSamples - 1) // Визиром выбрана последняя позиция
            {
                ((Button)buttonsPanel.Controls["moveForthButton"]!).Enabled = false;
                ((Button)buttonsPanel.Controls["moveBackButton"]!).Enabled = true;
            }
            else if (firstVisorSample == 0) // Визиром выбрана первая позиция
            {
                ((Button)buttonsPanel.Controls["moveBackButton"]!).Enabled = false;
                ((Button)buttonsPanel.Controls["moveForthButton"]!).Enabled = true;
            }
            else // Визиром выбрана иная позиция 
            {
                ((Button)buttonsPanel.Controls["moveBackButton"]!).Enabled = true;
                ((Button)buttonsPanel.Controls["moveForthButton"]!).Enabled = true;
            }
        }

        // Обновление панелей с графиками сигналов и координатами визиров
        private void UpdateSignalPlots(Panel signalPlotsPanel, Panel visorsDataPanel, int targetSample)
        {
            // Удаление предыдущего визира
            for (int i = 0; i < numberOfPlots; i++)
                signalPlots[i].Plot.Remove(firstVertVisors[i]);

            firstVertVisors.Clear();

            // Смена позиции визира
            firstVisorSample = targetSample;

            // Установка нового визира и обновление показателей
            for (int i = 0; i < numberOfPlots; i++)
            {
                if (firstVisorSample == secondVisorSample) // При пересечении визиры окрашиваются в зеленый
                {
                    firstVertVisors.Add(signalPlots[i].Plot.Add.VerticalLine(timeStamps[targetSample], 2, Colors.Green));
                    secondVertVisors[i].Color = Colors.Green;
                }
                else // Цвета по умолчанию: красный для первого, синий для второго
                {
                    firstVertVisors.Add(signalPlots[i].Plot.Add.VerticalLine(timeStamps[targetSample], 2, Colors.Red));
                    secondVertVisors[i].Color = Colors.Blue;
                }

                // Обновление показателей мгновенных значений токов и напряжений
                Label valueLabel = (Label)signalPlotsPanel.Controls[$"valueLabel{i}"]!;

                if (i < analogChannelsCount)
                    valueLabel.Text = $"{channelsNames[i]}: {channelsData[i][targetSample].ToString("F3", CultureInfo.InvariantCulture)} {analogChannelsUnits[i]}";
                else
                    valueLabel.Text = $"{channelsNames[i]}: {channelsData[i][targetSample]}";

                // Обновление показателей координат визиров
                ((Label)visorsDataPanel.Controls["timeFirstVertVisorLabel"]!).Text = "Верт. визир 1: " + firstVertVisors[i].Position.ToString("F3", CultureInfo.InvariantCulture) + " мс";
                ((Label)visorsDataPanel.Controls["currentSampleLabel"]!).Text = "Номер выборки: " + (firstVisorSample + 1);

                if (secondVisorSample != -1)
                    ((Label)visorsDataPanel.Controls["timeVertVisorsDeltaLabel"]!).Text = "Разница верт. визиров: " + Math.Abs((firstVertVisors[i].Position - secondVertVisors[i].Position)).ToString("F3", CultureInfo.InvariantCulture) + " мс";
            }

            for (int i = 0; i < numberOfPlots; i++)
                signalPlots[i].Refresh();
        }

        // Обновление панели с векторной диаграммой
        private void UpdateVectorDiagram(Panel vectorDiagramPanel, Panel signalPlotsPanel)
        {
            // Поиск элементов управления на панели для обновления 
            TableLayoutPanel firstHarmonicTable = (TableLayoutPanel)vectorDiagramPanel.Controls["firstHarmonicTable"]!;
            FormsPlot vectorDiagram = (FormsPlot)vectorDiagramPanel.Controls["vectorDiagram"]!;
            PolarAxis polarAxis = (PolarAxis)vectorDiagram.Plot.GetPlottables().First();

            // Обновление элементов управления
            for (int i = 0; i < analogChannelsCount; i++)
            {
                double phaseDeg = firstHarmonicPhase[i];
                double amplitude = firstHarmonicAmplitude[i];
                double RMS = RootMeanSquare[i];

                string amplitudeLine = amplitude.ToString("F3", CultureInfo.InvariantCulture) + " " + analogChannelsUnits[i];

                ((Label)firstHarmonicTable.Controls["phaseLabel" + i]!).Text = phaseDeg.ToString("F0", CultureInfo.InvariantCulture) + "°";
                ((Label)firstHarmonicTable.Controls["amplitudeLabel" + i]!).Text = amplitudeLine;
                ((Label)signalPlotsPanel.Controls["RMSLabel" + i]!).Text = "RMS: " + RMS.ToString("F3", CultureInfo.InvariantCulture) + " " + analogChannelsUnits[i];
                ((Label)signalPlotsPanel.Controls["A1Label" + i]!).Text = "A1: " + amplitudeLine;

                // Обновление координат векторов
                PolarCoordinates point = new(amplitude, ScottPlot.Angle.FromDegrees(phaseDeg));
                vectors[i].Tip = polarAxis.GetCoordinates(point);

                // Включение видимости векторов
                if (vectors[i].IsVisible == false)
                    vectors[i].IsVisible = true;
            }

            vectorDiagram.Refresh();
        }

        // Обновление панели со спектральными диаграммами
        private void UpdateSpectralDiagram(Panel spectralDiagramPanel)
        {
            for (int i = 0; i < analogChannelsCount; i++)
            {
                FormsPlot spectralDiagram = (FormsPlot)spectralDiagramPanel.Controls["spectralDiagram" + i]!;

                double sumAmplitude = amplitudesForSpectralDiagram[i].Take(numberOfSamplesPerPeriod / 2 + 1).Sum();

                foreach (var bar in barPlots[i].Bars)
                {
                    bar.Value = amplitudesForSpectralDiagram[i][(int)(bar.Position / parser!.Schema!.NominalFrequency)] / sumAmplitude * 100;
                    bar.Label = bar.Value.ToString("F1", CultureInfo.InvariantCulture);
                }

                spectralDiagram.Refresh();
            }
        }

        // Выполнение спектрального анализа всего набора данных окнами
        private void AnalyzeSpectrum(int j)
        {
            // Параллельный анализ всех каналов
            Parallel.For(0, analogChannelsCount, i =>
            {
                int startIndex = j - numberOfSamplesPerPeriod + 1; // Левая граница окна
                int endIndex = j; // Правая граница окна

                int length = endIndex - startIndex + 1; // Длина окна в количестве выборок

                double[] data = channelsData[i].ToArray();
                double[] windowData = new double[length];

                Array.Copy(data, startIndex, windowData, 0, length);

                // Расчет среднеквадратичной величины в рамках окна
                double RMS = Math.Sqrt(windowData.Select(x => x * x).Sum() / length);

                // Перевод значений каналов в комплексную форму
                Complex[] ComplexWindowData = new Complex[length];

                for (int m = 0; m < length; m++)
                    ComplexWindowData[m] = new(windowData[m], 0);

                // Выполнение дискретного преобразования Фурье
                Complex[] realSpectrum = DiscreteFourierTransform(ComplexWindowData);

                // Применение коэффициентов масштабирования
                double[] scaledAmplitudes = realSpectrum.Select(x => x.Magnitude / (SpectralScalingFactor * 10)).ToArray();
                scaledAmplitudes[0] = scaledAmplitudes[0] / SpectralScalingFactor; 

                // Определение значения амплитуды и фазового угла первой гармоники
                double maxAmplitude = 0;
                int maxIndex = 0;

                for (int k = 0; k <= scaledAmplitudes.Length / 2; k++) // Рассматриваем только первую половину спектра, так как вторая содержит ту же информацию, что и первая
                {
                    double currentAmplitude = scaledAmplitudes[k];
                    if (currentAmplitude > maxAmplitude)
                    {
                        maxAmplitude = currentAmplitude;
                        maxIndex = k;
                    }
                }

                double amplitude = scaledAmplitudes[maxIndex];
                double phaseRad = realSpectrum[maxIndex].Phase;

                // Конвертирование радиан в градусы и добавление сдвига фазы
                double phaseDeg = phaseRad * 180 / Math.PI;
                phaseDeg += 72;

                // Коррекция угла, чтобы он находился в диапазоне от -180 до 180 градусов
                if (phaseDeg > 180) phaseDeg -= 360;

                firstHarmonicAmplitude[i] = amplitude;
                firstHarmonicPhase[i] = phaseDeg;
                RootMeanSquare[i] = RMS;

                amplitudesForSpectralDiagram[i] = scaledAmplitudes;
            });
        }

        // Проверка на возможность перемещения второго вертикального визира вперед
        private bool CanMoveSecondForth()
        {
            return isParserInitialized && secondVisorSample != numberOfSamples - 1;
        }

        // Перемещение второго вертикального визира вперед
        private void MoveSecondVisorForth(object? obj)
        {
            if (obj is MainWindowForm mainForm)
                MoveSecondVisor(mainForm, secondVisorSample + 1);
        }

        // Проверка на возможность перемещения второго вертикального визира назад
        private bool CanMoveSecondBack()
        {
            return isParserInitialized && secondVisorSample > 0;
        }

        // Перемещение второго вертикального визира назад
        private void MoveSecondVisorBack(object? obj)
        {
            if (obj is MainWindowForm mainForm)
                MoveSecondVisor(mainForm, secondVisorSample - 1);
        }

        // Перемещение второго вертикального визира
        private void MoveSecondVisor(MainWindowForm mainForm, int targetSample)
        {
            // Удаление предыдущего визира
            for (int i = 0; i < numberOfPlots; i++)
                signalPlots[i].Plot.Remove(secondVertVisors[i]); 

            secondVertVisors.Clear();

            // Включение кнопки обрезки графика при первом использовании второго вертикального визира
            if (secondVisorSample == -1)
            {
                Panel buttonsPanel = (Panel)mainForm.Controls["buttonsPanel"]!;
                ((Button)buttonsPanel.Controls["cropPlotButton"]!).Enabled = true;
            }

            // Смена позиции визира
            secondVisorSample = targetSample;

            // Установка нового визира и обновление показателей
            for (int i = 0; i < numberOfPlots; i++)
            {
                if (secondVisorSample == firstVisorSample) // При пересечении визиры окрашиваются в зеленый
                {
                    secondVertVisors.Add(signalPlots[i].Plot.Add.VerticalLine(timeStamps[targetSample], 2, Colors.Green));
                    firstVertVisors[i].Color = Colors.Green;
                }
                else // Цвета по умолчанию: красный для первого, синий для второго
                {
                    secondVertVisors.Add(signalPlots[i].Plot.Add.VerticalLine(timeStamps[targetSample], 2, Colors.Blue));
                    firstVertVisors[i].Color = Colors.Red;
                }

                // Обновление показателей координат визиров
                Panel visorsDataPanel = (Panel)mainForm.Controls["visorsDataPanel"]!;

                ((Label)visorsDataPanel.Controls["timeSecondVertVisorLabel"]!).Text = "Верт. визир 2: " + secondVertVisors[i].Position.ToString("F3", CultureInfo.InvariantCulture) + " мс";
                ((Label)visorsDataPanel.Controls["timeVertVisorsDeltaLabel"]!).Text = "Разница верт. визиров: " + Math.Abs((secondVertVisors[i].Position - firstVertVisors[i].Position)).ToString("F3", CultureInfo.InvariantCulture) + " мс";
            }

            for (int i = 0; i < numberOfPlots; i++)
                signalPlots[i].Refresh();
        }

        // Cброс настроек масштабирования графиков
        private void ResetScale(object? obj)
        {
            // Сброс по оси X
            foreach (var signalPlot in signalPlots)
            {
                signalPlot.Plot.Axes.AutoScale();
                signalPlot.Plot.Axes.SetLimitsX(timeStamps[0] - 0.1, timeStamps[^1] + 0.1);

                signalPlot.Refresh();
            }

            XScale = 1;

            // Сброс по оси Y
            if (YScale != 1 && obj is Panel signalPlotsPanel)
            {
                UpdateYScale(signalPlotsPanel, AnalogSignalPlotDefaultHeight);
                YScale = 1;
            }
        }

        // Проверка на возможность увеличения масштаба по оси X
        private bool CanScaleXUp()
        {
            return XScale < 1;
        }

        // Увеличение масштаба по оси X
        private void ScaleXUp()
        {
            foreach (var signalPlot in signalPlots)
            {
                AxisLimits axisLimits = signalPlot.Plot.Axes.GetLimits();
                double offset = timeStamps[^1] / 5;

                signalPlot.Plot.Axes.SetLimitsX(axisLimits.XRange.Min, axisLimits.XRange.Max - offset);

                signalPlot.Refresh();
            }

            XScale++;

            if (XScale == 1)
                throw new Exception("MaxScale");
        }

        // Уменьшение масштаба по оси X
        private void ScaleXDown()
        {
            foreach (var signalPlot in signalPlots)
            {
                AxisLimits axisLimits = signalPlot.Plot.Axes.GetLimits();
                double offset = timeStamps[^1] / 5;

                signalPlot.Plot.Axes.SetLimitsX(axisLimits.XRange.Min, axisLimits.XRange.Max + offset);

                signalPlot.Refresh();
            }

            XScale--;
        }

        // Увеличение масштаба по оси Y
        private void ScaleYUp(object? obj)
        {
            if (obj is Panel signalPlotsPanel)
            {
                UpdateYScale(signalPlotsPanel, AnalogSignalPlotHeight + 20);
                YScale++;
            }
        }

        // Проверка на возможность уменьшения масштаба по оси Y
        private bool CanScaleYDown()
        {
            return YScale > 1;
        }

        // Уменьшение масштаба по оси Y
        private void ScaleYDown(object? obj)
        {
            if (obj is Panel signalPlotsPanel)
            {
                UpdateYScale(signalPlotsPanel, AnalogSignalPlotHeight - 20);
                YScale--;

                if (YScale == 1)
                    throw new Exception("MinScale");
            }
        }

        // Обновление масштаба по оси Y
        private void UpdateYScale(Panel signalPlotsPanel, int targetAnalogSignalPlotHeight)
        {
            // Изменение размеров графиков и расположения им сопутствующих элементов управления
            AnalogSignalPlotHeight = targetAnalogSignalPlotHeight;

            for (int i = 0; i < numberOfPlots; i++)
            {
                Label valueLabel = (Label)signalPlotsPanel.Controls["valueLabel" + i]!;
                Panel separator = (Panel)signalPlotsPanel.Controls["separator" + i]!;
                FormsPlot signalPlot = (FormsPlot)signalPlotsPanel.Controls["signalPlot" + i]!;

                if (i < analogChannelsCount) // Аналоговые каналы
                {
                    valueLabel.Location = new Point(0, 3 + i * AnalogSignalPlotHeight - 1);

                    Label maxValueLabel = (Label)signalPlotsPanel.Controls["maxValueLabel" + i]!;
                    maxValueLabel.Location = new Point(0, 23 + i * AnalogSignalPlotHeight - 1);

                    Label minValueLabel = (Label)signalPlotsPanel.Controls["minValueLabel" + i]!;
                    minValueLabel.Location = new Point(0, 43 + i * AnalogSignalPlotHeight - 1);

                    Label RMSLabel = (Label)signalPlotsPanel.Controls["RMSLabel" + i]!;
                    RMSLabel.Location = new Point(0, 63 + i * AnalogSignalPlotHeight - 1);

                    Label A1Label = (Label)signalPlotsPanel.Controls["A1Label" + i]!;
                    A1Label.Location = new Point(0, 83 + i * AnalogSignalPlotHeight - 1);

                    separator.Location = new Point(0, AnalogSignalPlotHeight + i * AnalogSignalPlotHeight - 1);

                    signalPlot.Height = i == numberOfPlots - 1 ? AnalogSignalPlotHeight + 20 : AnalogSignalPlotHeight;
                    signalPlot.Location = new Point(100, i * AnalogSignalPlotHeight - 1);
                }
                else // Дискретные каналы
                {
                    if (i == analogChannelsCount)
                    {
                        valueLabel.Location = new Point(0, 18 + i * AnalogSignalPlotHeight - 1);
                        separator.Location = new Point(0, 50 + i * AnalogSignalPlotHeight - 1);
                        signalPlot.Location = new Point(100, i * AnalogSignalPlotHeight - 1);
                    }
                    else if (i == numberOfPlots - 1)
                    {
                        valueLabel.Location = new Point(0, 19 + analogChannelsCount * AnalogSignalPlotHeight + (i - analogChannelsCount) * DigitalSignalPlotDefaultHeight - 1);
                        separator.Location = new Point(0, 52 + analogChannelsCount * AnalogSignalPlotHeight + (i - analogChannelsCount) * DigitalSignalPlotDefaultHeight - 1);
                        signalPlot.Location = new Point(100, analogChannelsCount * AnalogSignalPlotHeight + (i - analogChannelsCount) * DigitalSignalPlotDefaultHeight - 1);
                    }
                    else
                    {
                        valueLabel.Location = new Point(0, 18 + analogChannelsCount * AnalogSignalPlotHeight + (i - analogChannelsCount) * DigitalSignalPlotDefaultHeight - 1);
                        separator.Location = new Point(0, 50 + analogChannelsCount * AnalogSignalPlotHeight + (i - analogChannelsCount) * DigitalSignalPlotDefaultHeight - 1);
                        signalPlot.Location = new Point(100, analogChannelsCount * AnalogSignalPlotHeight + (i - analogChannelsCount) * DigitalSignalPlotDefaultHeight - 1);
                    }
                }
            }

            // Настройка панели
            if (signalPlotsPanel.HorizontalScroll.Visible)
            {
                for (int i = 0; i < numberOfPlots; i++)
                {
                    if (i < analogChannelsCount)
                        signalPlots[i].Size = i == numberOfPlots - 1 ? new Size(signalPlotsPanel.Width - 119, AnalogSignalPlotHeight + 20) : new Size(signalPlotsPanel.Width - 119, AnalogSignalPlotHeight);
                    else
                        signalPlots[i].Size = i == numberOfPlots - 1 ? new Size(signalPlotsPanel.Width - 119, DigitalSignalPlotDefaultHeight + 20) : new Size(signalPlotsPanel.Width - 119, DigitalSignalPlotDefaultHeight);

                    signalPlots[i].Refresh();
                }
            }
            else
            {
                if (!signalPlotsPanel.VerticalScroll.Visible)
                {
                    for (int i = 0; i < numberOfPlots; i++)
                    {
                        if (i < analogChannelsCount)
                            signalPlots[i].Size = i == numberOfPlots - 1 ? new Size(signalPlotsPanel.Width - 102, AnalogSignalPlotHeight + 20) : new Size(signalPlotsPanel.Width - 102, AnalogSignalPlotHeight);
                        else
                            signalPlots[i].Size = i == numberOfPlots - 1 ? new Size(signalPlotsPanel.Width - 102, DigitalSignalPlotDefaultHeight + 20) : new Size(signalPlotsPanel.Width - 102, DigitalSignalPlotDefaultHeight);

                        signalPlots[i].Refresh();
                    }
                }
            }
        }

        // Эмуляция динамической отрисовки
        private void EmulateDynamic(object? obj)
        {
            // Выбор временного интервала между выборками
            double time = 1.0;
            if (timeStamps.Count > 1) time = timeStamps[1];
            // Добавление новой временной метки
            timeStamps.Add(timeStamps[(int)numberOfSamples - 1] + time);

            // Добавление нового набора значений каналов
            for (int i = 0; i < numberOfPlots; i++)
            {
                if (i < analogChannelsCount) // Аналоговые каналы
                {
                    channelsData[i].Add(Math.Sin(numberOfSamples * 0.5 + i * i) * 25);
                    signals[i].MaxRenderIndex = (int)numberOfSamples;
                }
                else // Дискретные каналы
                {
                    channelsData[i].Add(numberOfSamples % 2);
                    signals[i].MaxRenderIndex = (int)numberOfSamples;
                }

                // Обновление показателей максимальных и минимальных значений токов и напряжений
                if (obj is Panel signalPlotsPanel && i < analogChannelsCount) 
                { 
                    if (channelsData[i].Last() == channelsData[i].Max())
                        ((Label)signalPlotsPanel.Controls["maxValueLabel" + i]!).Text = $"Max: {channelsData[i].Last().ToString("F3", CultureInfo.InvariantCulture)} {analogChannelsUnits[i]}";

                    if (channelsData[i].Last() == channelsData[i].Min())
                        ((Label)signalPlotsPanel.Controls["minValueLabel" + i]!).Text = $"Min: {channelsData[i].Last().ToString("F3", CultureInfo.InvariantCulture)} {analogChannelsUnits[i]}";
                }

                // Обновление границ графиков по оси X
                AxisLimits axisLimits = signalPlots[i].Plot.Axes.GetLimits();

                if (timeStamps[^1] > axisLimits.XRange.Max)
                    signalPlots[i].Plot.Axes.SetLimitsX(axisLimits.XRange.Min, axisLimits.XRange.Max + time);

                signalPlots[i].Refresh();
            }

            // Инкрементирование количества выборок
            numberOfSamples++;
        }

        // Обрезка графиков
        private void CropPlot()
        {
            for (int i = 0; i < numberOfPlots; i++)
            {
                if (firstVisorSample <= secondVisorSample)
                    signalPlots[i].Plot.Axes.SetLimitsX(firstVertVisors[i].Position - 0.1, secondVertVisors[i].Position + 0.1);
                else
                    signalPlots[i].Plot.Axes.SetLimitsX(secondVertVisors[i].Position - 0.1, firstVertVisors[i].Position + 0.1);

                signalPlots[i].Refresh();
            }
        }

        // Смена видимости первого вертикального визира
        private void FirstVertVisorChangeVisibility()
        {
            if (firstVertVisors.Count > 0)
            {
                if (firstVertVisors[0].IsVisible)
                {
                    foreach (var visor in firstVertVisors)
                        visor.IsVisible = false;
                }
                else
                {
                    foreach (var visor in firstVertVisors)
                        visor.IsVisible = true;
                }

                foreach (var signalPlot in signalPlots)
                    signalPlot.Refresh();
            }
        }

        // Смена видимости второго вертикального визира
        private void SecondVertVisorChangeVisibility()
        {
            if (secondVertVisors.Count > 0)
            {
                if (secondVertVisors[0].IsVisible)
                {
                    foreach (var visor in secondVertVisors)
                        visor.IsVisible = false;
                }
                else
                {
                    foreach (var visor in secondVertVisors)
                        visor.IsVisible = true;
                }

                foreach (var signalPlot in signalPlots)
                    signalPlot.Refresh();

                if (secondVisorSample == -1) // Изначально второй вертикальный визир не установлен, смена видимости устанавливает визир
                    secondVisorSample = 0;
            }
        }

        private void PlayFile()
        {
            firstVisorSample = 0;
            WaveFormat waveFormat = WaveFormat.CreateIeeeFloatWaveFormat((int)parser!.Schema!.SampleRates![0].Rate, 2);
            IWavePlayer waveOut = new WaveOutEvent();

            var provider = new BufferedWaveProvider(waveFormat);

            provider.BufferLength = channelsData[0].Count * sizeof(float);
            provider.AddSamples(ToByteArray(channelsData[0].ConvertAll(e => (float)e).ToArray()), 0, channelsData[0].Count * sizeof(float));

            waveOut.Init(provider);
            waveOut.Play();

            byte[] ToByteArray(float[] floatArray)
            {
                byte[] byteArray = new byte[floatArray.Length * sizeof(float)];
                Buffer.BlockCopy(floatArray, 0, byteArray, 0, byteArray.Length);
                return byteArray;
            }
        }

        // Добавление горизонтальных визиров
        private void AddHorizontalVisors(Plot plot, int plotIndex, FormsPlot signalPlot)
        {
            // Добавление первого горизонтального визира
            var firstHorVisor = plot.Add.HorizontalLine(plot.Axes.GetLimits().YRange.Max / 2, 2, Colors.Red);
            firstHorVisor.Text = plotIndex.ToString();
            firstHorVisor.IsDraggable = true;
            firstHorVisors![plotIndex] = firstHorVisor;

            // Добавление второго горизонтального визира
            var secondHorVisor = plot.Add.HorizontalLine(plot.Axes.GetLimits().YRange.Max / 4, 2, Colors.Blue);
            secondHorVisor.Text = plotIndex.ToString();
            secondHorVisor.IsDraggable = true;
            secondHorVisors![plotIndex] = secondHorVisor;

            // Обновление меню
            signalPlot.Menu!.Clear();
            signalPlot.Menu.Add("Открыть в новом окне", (plot) => OpenInNewWindow(plot, timeStamps[0] - 0.1, timeStamps[^1] + 0.1));
            signalPlot.Menu.Add("Скопировать в буфер обмена", (plot) => CopyImageToClipboard(plot, true));
            signalPlot.Menu.Add("Сохранить изображение", (plot) => OpenSaveImageDialog(plot, true));
            signalPlot.Menu.Add("Удалить горизонтальные визиры", (plot) => RemoveHorizontalVisors(plot, plotIndex, signalPlot, firstHorVisor, secondHorVisor));

            plot.PlotControl!.Refresh();
        }

        // Удаление горизонтальных визиров
        private void RemoveHorizontalVisors(Plot plot, int plotIndex, FormsPlot signalPlot, HorizontalLine firstHorVisor, HorizontalLine secondHorVisor)
        {
            plot.Remove(firstHorVisor);
            plot.Remove(secondHorVisor);

            // Обновление меню
            signalPlot.Menu!.Clear();
            signalPlot.Menu.Add("Открыть в новом окне", (plot) => OpenInNewWindow(plot, timeStamps[0] - 0.1, timeStamps[^1] + 0.1));
            signalPlot.Menu.Add("Скопировать в буфер обмена", (plot) => CopyImageToClipboard(plot, true));
            signalPlot.Menu.Add("Сохранить изображение", (plot) => OpenSaveImageDialog(plot, true));
            signalPlot.Menu.Add("Добавить горизонтальные визиры", (plot) => AddHorizontalVisors(plot, plotIndex, signalPlot));

            plot.PlotControl!.Refresh();
        }

        // Обновление масшатаба векторной диаграммы
        private void UpdateVectorScale(Panel vectorDiagramPanel)
        {
            vectors.Clear();
            CreateVectorDiagram(vectorDiagramPanel);
        }

        #endregion

        #region [ Static ]

        // Статические методы

        // Установка двойной буферизации
        private static void SetDoubleBuffered(Control control, bool value)
        {
            // Поиск свойства DoubleBuffered через рефлексию
            PropertyInfo pi = control.GetType().GetProperty("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic)!;

            // Установка значения
            pi?.SetValue(control, value, null);
        }

        // Получение линии под курсором мыши
        private static AxisLine? GetLineUnderMouse(float x, float y, FormsPlot senderPlot)
        {
            CoordinateRect rect = senderPlot.Plot.GetCoordinateRect(x, y, radius: 10);

            foreach (AxisLine axLine in senderPlot.Plot.GetPlottables<AxisLine>().Reverse())
                if (axLine.IsUnderMouse(rect))
                    return axLine;

            return null;
        }

        // Получение индекса значения списка ближайшего к заданному
        private static int FindClosest(List<double> list, double targetValue)
        {
            int closestIndex = 0;
            double closestDifference = Math.Abs(list[0] - targetValue);

            for (int i = 1; i < list.Count; i++)
            {
                double currentDifference = Math.Abs(list[i] - targetValue);

                if (currentDifference < closestDifference)
                {
                    closestIndex = i;
                    closestDifference = currentDifference;
                }
            }

            return closestIndex;
        }

        // Выполнение дискретного преобразования Фурье
        internal Complex[] DiscreteFourierTransform(Complex[] input)
        {
            int N = input.Length; // Количество входных значений сигнала, измеренных за период
            double mult = -2 * Math.PI / N; // Часть выражения, чтобы не проводить вычисления на каждой итерации
            Complex[] output = new Complex[N]; // Массив для выходных значений

            for (int k = 0; k < N; k++)
            {
                for (int n = 0; n < N; n++)
                {
                    double radians = n * k * mult; // Аргумент комплексной экспоненты
                    Complex temp = new(Math.Cos(radians), Math.Sin(radians)); // cos(-x) = cos(x), sin(-x) = -sinx
                    temp *= input[n];
                    output[k] += temp * 5;
                }
            }

            return output;
        }

        // Открытие графика в новом окне
        private static void OpenInNewWindow(Plot plot, double leftXLimit, double rightXLimit)
        {
            // Установка отступов, при которых видно оси графика
            plot.Layout.Fixed(new PixelPadding(30, 5, 20, 5));

            // Запуск окна с графиком
            LaunchNewWindow(plot, "Интерактивный график");

            // Установка прежних отступов
            if (plot.Axes.Bottom.Label.Text == "lastPlot")
                plot.Layout.Fixed(new PixelPadding(0, 2, 20, 0));
            else
                plot.Layout.Fixed(new PixelPadding(0, 2, 0, 0));

            // Сброс настроек масштабирования графика по оси X
            plot.Axes.AutoScale();
            plot.Axes.SetLimitsX(leftXLimit, rightXLimit);

            plot.PlotControl!.Refresh();
        }

        // Запуск окна с графиком
        private static void LaunchNewWindow(Plot plot, string title = "", int width = 600, int height = 400, bool blocking = true)
        {
            Plot interactivePlot = plot;
            IPlotControl originalControl = interactivePlot.PlotControl!; // Сохранение состояния графика до манипуляций с ним
            Form form = CreateForm(interactivePlot, title, width, height);
            form.Icon = Resources.Logo;

            form.FormClosed += delegate
            {
                interactivePlot.PlotControl = originalControl; // Восстановление состояния графика при закрытии окна
            };

            if (blocking)
                form.ShowDialog();
            else
                form.Show();
        }

        // Создание формы для интерактивного графика
        private static Form CreateForm(Plot interactivePlot, string title = "", int width = 600, int height = 400)
        {
            FormsPlot signalPlot = new()
            {
                Dock = DockStyle.Fill
            };

            signalPlot.Reset(interactivePlot); // Передача интерактивного графика элементу управления

            signalPlot.Menu!.Clear();
            signalPlot.Menu.Add("Скопировать в буфер обмена", (plot) => CopyImageToClipboard(plot, true));
            signalPlot.Menu.Add("Сохранить изображение", (plot) => OpenSaveImageDialog(plot, true));

            signalPlot.UserInputProcessor.DoubleLeftClickBenchmark(false);

            return new Form
            {
                StartPosition = FormStartPosition.CenterParent,
                Width = width,
                Height = height,
                Text = title,
                Controls = { (Control?)signalPlot }
            };
        }

        // Копирование изображения графика в буфер обмена
        private static void CopyImageToClipboard(Plot plot, bool addPadding)
        {
            if (addPadding) // С отступами
            {
                // Установка отступов, при которых видно оси графика
                plot.Layout.Fixed(new PixelPadding(30, 5, 20, 5));

                // Копирование в буфер обмена
                PixelSize lastRenderSize = plot.RenderManager.LastRender.FigureRect.Size;
                ScottPlot.Image img = plot.GetImage((int)lastRenderSize.Width, (int)lastRenderSize.Height);
                Bitmap bmp = img.GetBitmap();
                Clipboard.SetImage(bmp);

                // Установка прежних отступов
                if (plot.Axes.Bottom.Label.Text == "lastPlot")
                    plot.Layout.Fixed(new PixelPadding(0, 2, 20, 0));
                else
                    plot.Layout.Fixed(new PixelPadding(0, 2, 0, 0));
            }
            else // Без отступов
            {
                // Копирование в буфер обмена
                PixelSize lastRenderSize = plot.RenderManager.LastRender.FigureRect.Size;
                ScottPlot.Image img = plot.GetImage((int)lastRenderSize.Width, (int)lastRenderSize.Height);
                Bitmap bmp = img.GetBitmap();
                Clipboard.SetImage(bmp);
            }
        }

        // Сохранение изображения на диске
        private static void OpenSaveImageDialog(Plot plot, bool addPadding)
        {
            // Настройка, открытие и обработка диалогового окна
            SaveFileDialog dialog = new()
            {
                FileName = "plot.png",
                Filter = "PNG (*.png)|*.png" +
                         "|JPEG (*.jpg, *.jpeg)|*.jpg;*.jpeg" +
                         "|BMP (*.bmp)|*.bmp" +
                         "|WebP (*.webp)|*.webp" +
                         "|SVG (*.svg)|*.svg" +
                         "|Все файлы (*.*)|*.*"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.FileName))
                    return;

                // Определение формата изображения
                ImageFormat format;

                try
                {
                    format = ImageFormats.FromFilename(dialog.FileName);
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("Неподдерживаемый формат изображения", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    if (addPadding) // С отступами
                    {
                        // Установка отступов, при которых видно оси графика
                        plot.Layout.Fixed(new PixelPadding(30, 5, 20, 5));

                        // Сохранение на диске
                        PixelSize lastRenderSize = plot.RenderManager.LastRender.FigureRect.Size;
                        plot.Save(dialog.FileName, (int)lastRenderSize.Width, (int)lastRenderSize.Height, format);

                        // Установка прежних отступов
                        if (plot.Axes.Bottom.Label.Text == "lastPlot")
                            plot.Layout.Fixed(new PixelPadding(0, 2, 20, 0));
                        else
                            plot.Layout.Fixed(new PixelPadding(0, 2, 0, 0));
                    }
                    else // Без отступов
                    {
                        PixelSize lastRenderSize = plot.RenderManager.LastRender.FigureRect.Size;
                        plot.Save(dialog.FileName, (int)lastRenderSize.Width, (int)lastRenderSize.Height, format);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Не удалось сохранить изображение", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        #endregion

        #region [ Helpers ]

        // Вспомогательные классы для реализации команд и члены интерфейса

        // Команда без объекта и без предусловия
        private class SimpleCommand(Action execute) : ICommand
        {
            public event EventHandler? CanExecuteChanged;
            private readonly Action _execute = execute ?? throw new ArgumentNullException(nameof(execute)); // Ссылка на метод, который должен быть выполнен при вызове команды

            public bool CanExecute(object? parameter) // Проверка на возможность выполнения команды
            {
                return true;
            }

            public void Execute(object? parameter) // Выполнение команды 
            {
                _execute();
            }

            public void RaiseCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        // Команда с объектом, но без предусловия
        private class ObjectCommand(Action<object> execute) : ICommand
        {
            public event EventHandler? CanExecuteChanged;
            private readonly Action<object> _execute = execute ?? throw new ArgumentNullException(nameof(execute)); // Ссылка на метод, который должен быть выполнен при вызове команды

            public bool CanExecute(object? parameter) // Проверка на возможность выполнения команды
            {
                return true;
            }

            public void Execute(object? parameter) // Выполнение команды 
            {
                _execute(parameter!);
            }

            public void RaiseCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        // Команда без объекта, но с предусловием
        private class ConditionCommand(Action execute, Func<bool> canExecute) : ICommand
        {
            public event EventHandler? CanExecuteChanged;
            private readonly Action _execute = execute ?? throw new ArgumentNullException(nameof(execute)); // Ссылка на метод, который должен быть выполнен при вызове команды
            private readonly Func<bool> _canExecute = canExecute; // Ссылка на метод, проверяющий возможность выполнения команды

            public bool CanExecute(object? parameter) // Проверка на возможность выполнения команды
            {
                return _canExecute == null || _canExecute();
            }

            public void Execute(object? parameter) // Выполнение команды 
            {
                if (!CanExecute(parameter))
                    throw new ApplicationException();
                _execute();
            }

            public void RaiseCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        // Команда с объектом и с предусловием
        private class ObjectConditionCommand(Action<object> execute, Func<bool> canExecute) : ICommand
        {
            public event EventHandler? CanExecuteChanged;
            private readonly Action<object> _execute = execute ?? throw new ArgumentNullException(nameof(execute)); // Ссылка на метод, который должен быть выполнен при вызове команды
            private readonly Func<bool> _canExecute = canExecute; // Ссылка на метод, проверяющий возможность выполнения команды

            public bool CanExecute(object? parameter) // Проверка на возможность выполнения команды
            {
                return _canExecute == null || _canExecute();
            }

            public void Execute(object? parameter) // Выполнение команды 
            {
                if (!CanExecute(parameter))
                    throw new ApplicationException();
                _execute(parameter!);
            }

            public void RaiseCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        // Член интерфейса INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion
    }
}
