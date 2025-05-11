using COMTRADEInsight.ViewModels;

namespace COMTRADEInsight
{
    /// <summary>
    /// Представление главного окна.
    /// </summary>
    public partial class MainWindowForm : Form
    {
        #region [ Members ]

        // Поля
        private readonly MainWindowViewModel viewModel = new(); // ViewModel главного окна
        private readonly System.Windows.Forms.Timer repeatingTimer = new(); // Таймер повторения нажатий кнопок при их зажатии
        private readonly System.Windows.Forms.Timer dynamicTimer = new(); // Таймер для эмуляции динамической отрисовки
        private bool isMoveButtonPressed = false; // Флаг, указывающий, нажата ли одна из кнопок перемещения
        private Button? currentlyPressedMoveButton = null; // Указатель на кнопку перемещения

        #endregion

        #region [ Constructors ]

        // Конструктор
        public MainWindowForm()
        {
            InitializeComponent();

            // Настройка окна приложения для мониторов разных разрешений
            if (Screen.PrimaryScreen!.Bounds.Width > 1920 || Screen.PrimaryScreen.Bounds.Height > 1080)
            {
                ClientSize = new Size(1940, 1080);
                MaximumSize = new Size(1940, 1080);
                MinimumSize = new Size(1940, 1080);
                WindowState = FormWindowState.Normal;
                MaximizeBox = false;
            }
            else
            {
                ClientSize = new Size(1613, 800);
                WindowState = FormWindowState.Maximized;
            }

            // Включение двойной буферизации
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            UpdateStyles();

            // Привязка ViewModel
            mainScreenBindingSource.DataSource = viewModel;

            // Настройка таймеров
            repeatingTimer.Interval = 60; // Интервал повторения в миллисекундах
            repeatingTimer.Tick += RepeatingTimer_Tick!;

            dynamicTimer.Interval = 100; // Интервал повторения в миллисекундах
            dynamicTimer.Tick += DynamicTimer_Tick!;

            // Обработчики событий
            openFileToolStripMenuItem.Click += (sender, e) =>
            {
                try
                {
                    viewModel.OpenFileCommand.Execute(this);

                    // Включение и изменение элементов управления
                    configToolStripMenuItem.Enabled = true;
                    saveToolStripMenuItem.Enabled = true;
                    moveForthButton.Enabled = true;
                    resetScaleButton.Enabled = true;
                    scaleXDownButon.Enabled = true;
                    scaleYUpButon.Enabled = true;
                    dynamicPlotButton.Enabled = true;

                    firstVertVisorChangeVisibilityButton.Enabled = true;
                    firstVertVisorChangeVisibilityButton.BackColor = SystemColors.GradientActiveCaption;
                    firstVertVisorChangeVisibilityButton.FlatStyle = FlatStyle.Flat;

                    secondVertVisorChangeVisibilityButton.Enabled = true;
                    secondVertVisorChangeVisibilityButton.BackColor = SystemColors.Control;
                    secondVertVisorChangeVisibilityButton.FlatStyle = FlatStyle.System;
                }
                catch (Exception ex)
                {
                    if (ex.Message != "Отмена")
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            saveToolStripMenuItem.Click += (sender, e) =>
            {
                try
                {
                    viewModel.ShowSaveCommand.Execute(null);
                }
                catch
                {
                    MessageBox.Show("Сохранение файла невозможно.", "Ошибка",   MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            configToolStripMenuItem.Click += (sender, e) =>
            {
                try
                {
                    viewModel.ShowConfigCommand.Execute(null);
                }
                catch
                {
                    MessageBox.Show("Открытие конфигурации невозможно.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            exitToolStripMenuItem.Click += (sender, e) =>
            {
                try
                {
                    viewModel.CloseProgramCommand.Execute(null);
                }
                catch
                {
                    MessageBox.Show("Закрытие программы невозможно.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            moveForthButton.Click += (sender, e) =>
            {
                try
                {
                    viewModel.MoveForthCommand.Execute(this);

                    // Перевод кнопки "Вертикальный визир 1" в режим нажатой
                    firstVertVisorChangeVisibilityButton.BackColor = SystemColors.GradientActiveCaption;
                    firstVertVisorChangeVisibilityButton.FlatStyle = FlatStyle.Flat;
                }
                catch
                {
                    MessageBox.Show("Перемещение вперед невозможно.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            moveForthButton.MouseUp += (sender, e) =>
            {
                isMoveButtonPressed = false;
                repeatingTimer.Stop();
            };

            moveForthButton.MouseDown += (sender, e) =>
            {
                isMoveButtonPressed = true;
                currentlyPressedMoveButton = moveForthButton;
                repeatingTimer.Start();
            };

            moveForthButton.MouseLeave += (sender, e) =>
            {
                if (isMoveButtonPressed)
                {
                    isMoveButtonPressed = false;
                    repeatingTimer.Stop();
                }
            };

            moveBackButton.Click += (sender, e) =>
            {
                try
                {
                    viewModel.MoveBackCommand.Execute(this);

                    // Перевод кнопки "Вертикальный визир 1" в режим нажатой
                    firstVertVisorChangeVisibilityButton.BackColor = SystemColors.GradientActiveCaption;
                    firstVertVisorChangeVisibilityButton.FlatStyle = FlatStyle.Flat;
                }
                catch
                {
                    MessageBox.Show("Перемещение назад невозможно.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            moveBackButton.MouseUp += (sender, e) =>
            {
                isMoveButtonPressed = false;
                repeatingTimer.Stop();
            };

            moveBackButton.MouseDown += (sender, e) =>
            {
                isMoveButtonPressed = true;
                currentlyPressedMoveButton = moveBackButton;
                repeatingTimer.Start();
            };

            moveBackButton.MouseLeave += (sender, e) =>
            {
                if (isMoveButtonPressed)
                {
                    isMoveButtonPressed = false;
                    repeatingTimer.Stop();
                }
            };

            resetScaleButton.Click += (sender, e) =>
            {
                try
                {
                    viewModel.ResetScaleCommand.Execute(signalPlotsPanel);
                    scaleXUpButon.Enabled = false;
                    scaleYDownButon.Enabled = false;
                }
                catch
                {
                    MessageBox.Show("Сброс масштаба невозможен.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            scaleXUpButon.Click += (sender, e) =>
            {
                try
                {
                    viewModel.ScaleXUpCommand.Execute(null);
                }
                catch (Exception ex)
                {
                    if (ex.Message == "MaxScale")
                        scaleXUpButon.Enabled = false;
                }
            };

            scaleXDownButon.Click += (sender, e) =>
            {
                try
                {
                    viewModel.ScaleXDownCommand.Execute(null);
                    scaleXUpButon.Enabled = true;
                }
                catch
                {
                    MessageBox.Show("Уменьшение масштаба по X невозможно.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            scaleYUpButon.Click += (sender, e) =>
            {
                try
                {
                    viewModel.ScaleYUpCommand.Execute(signalPlotsPanel);
                    scaleYDownButon.Enabled = true;
                }
                catch
                {
                    MessageBox.Show("Увеличение масштаба по Y невозможно.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            scaleYDownButon.Click += (sender, e) =>
            {
                try
                {
                    viewModel.ScaleYDownCommand.Execute(signalPlotsPanel);
                }
                catch (Exception ex)
                {
                    if (ex.Message == "MinScale")
                        scaleYDownButon.Enabled = false;
                }
            };

            dynamicPlotButton.Click += (sender, e) =>
            {
                if (dynamicTimer.Enabled)
                    dynamicTimer.Stop();
                else
                    dynamicTimer.Start();

                // Смена состояния кнопки
                UpdateButtonState(sender!);
            };

            cropPlotButton.Click += (sender, e) =>
            {
                try
                {
                    viewModel.CropPlotCommand.Execute(null);
                }
                catch
                {
                    MessageBox.Show("Обрезка графика невозможна.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            firstVertVisorChangeVisibilityButton.Click += (sender, e) =>
            {
                try
                {
                    viewModel.СhangeVisibilityOfFirstVertVisorCommand.Execute(null);

                    // Смена состояния кнопки
                    UpdateButtonState(sender!);
                }
                catch
                {
                    MessageBox.Show("Обрезка графика невозможна.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            secondVertVisorChangeVisibilityButton.Click += (sender, e) =>
            {
                try
                {
                    viewModel.СhangeVisibilityOfSecondVertVisorCommand.Execute(null);

                    // Смена состояния кнопки
                    UpdateButtonState(sender!);
                }
                catch
                {
                    MessageBox.Show("Обрезка графика невозможна.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }

        #endregion

        #region [ Static ]

        // Статические методы
        private static void UpdateButtonState(object sender)
        {
            if (sender is Button buttonToUpdate)
            {
                if (buttonToUpdate.FlatStyle == FlatStyle.Standard || buttonToUpdate.FlatStyle == FlatStyle.System)
                {
                    buttonToUpdate.BackColor = SystemColors.GradientActiveCaption;
                    buttonToUpdate.FlatStyle = FlatStyle.Flat;
                }
                else
                {
                    buttonToUpdate.BackColor = SystemColors.Control;
                    buttonToUpdate.FlatStyle = FlatStyle.System;
                }
            }
        }

        #endregion

        #region [ Other ]

        // Прочие обработчики событий
        private void RepeatingTimer_Tick(object sender, EventArgs e)
        {
            if (isMoveButtonPressed)
            {
                currentlyPressedMoveButton!.PerformClick();
            }
        }

        private void DynamicTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                viewModel.EmulateDynamicCommand.Execute(signalPlotsPanel);
                moveForthButton.Enabled = true;
            }
            catch
            {
                MessageBox.Show("Включение динамической отрисовки невозможно.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void MainForm_FormClosing_1(object sender, EventArgs e)
        {
            try
            {
                viewModel.CloseProgramCommand.Execute(null);
            }
            catch
            {
                MessageBox.Show("Закрытие программы невозможно.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Right)
            {
                moveForthButton.PerformClick();
                return true;
            }
            else if (keyData == Keys.Left)
            {
                moveBackButton.PerformClick();
                return true;
            }
            else if (keyData == (Keys.Shift | Keys.Right))
            {
                try
                {
                    viewModel.MoveSecondForthCommand.Execute(this);
                    secondVertVisorChangeVisibilityButton.BackColor = SystemColors.GradientActiveCaption;
                    secondVertVisorChangeVisibilityButton.FlatStyle = FlatStyle.Flat;
                }
                catch
                {
                }
                return true;
            }
            else if (keyData == (Keys.Shift | Keys.Left))
            {
                try
                {
                    viewModel.MoveSecondBackCommand.Execute(this);
                    secondVertVisorChangeVisibilityButton.BackColor = SystemColors.GradientActiveCaption;
                    secondVertVisorChangeVisibilityButton.FlatStyle = FlatStyle.Flat;
                }
                catch
                {
                }
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion
    }
}
