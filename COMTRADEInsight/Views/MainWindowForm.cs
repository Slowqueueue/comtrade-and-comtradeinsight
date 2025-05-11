using COMTRADEInsight.ViewModels;

namespace COMTRADEInsight
{
    /// <summary>
    /// ������������� �������� ����.
    /// </summary>
    public partial class MainWindowForm : Form
    {
        #region [ Members ]

        // ����
        private readonly MainWindowViewModel viewModel = new(); // ViewModel �������� ����
        private readonly System.Windows.Forms.Timer repeatingTimer = new(); // ������ ���������� ������� ������ ��� �� �������
        private readonly System.Windows.Forms.Timer dynamicTimer = new(); // ������ ��� �������� ������������ ���������
        private bool isMoveButtonPressed = false; // ����, �����������, ������ �� ���� �� ������ �����������
        private Button? currentlyPressedMoveButton = null; // ��������� �� ������ �����������

        #endregion

        #region [ Constructors ]

        // �����������
        public MainWindowForm()
        {
            InitializeComponent();

            // ��������� ���� ���������� ��� ��������� ������ ����������
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

            // ��������� ������� �����������
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            UpdateStyles();

            // �������� ViewModel
            mainScreenBindingSource.DataSource = viewModel;

            // ��������� ��������
            repeatingTimer.Interval = 60; // �������� ���������� � �������������
            repeatingTimer.Tick += RepeatingTimer_Tick!;

            dynamicTimer.Interval = 100; // �������� ���������� � �������������
            dynamicTimer.Tick += DynamicTimer_Tick!;

            // ����������� �������
            openFileToolStripMenuItem.Click += (sender, e) =>
            {
                try
                {
                    viewModel.OpenFileCommand.Execute(this);

                    // ��������� � ��������� ��������� ����������
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
                    if (ex.Message != "������")
                        MessageBox.Show(ex.Message, "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show("���������� ����� ����������.", "������",   MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show("�������� ������������ ����������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show("�������� ��������� ����������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            moveForthButton.Click += (sender, e) =>
            {
                try
                {
                    viewModel.MoveForthCommand.Execute(this);

                    // ������� ������ "������������ ����� 1" � ����� �������
                    firstVertVisorChangeVisibilityButton.BackColor = SystemColors.GradientActiveCaption;
                    firstVertVisorChangeVisibilityButton.FlatStyle = FlatStyle.Flat;
                }
                catch
                {
                    MessageBox.Show("����������� ������ ����������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    // ������� ������ "������������ ����� 1" � ����� �������
                    firstVertVisorChangeVisibilityButton.BackColor = SystemColors.GradientActiveCaption;
                    firstVertVisorChangeVisibilityButton.FlatStyle = FlatStyle.Flat;
                }
                catch
                {
                    MessageBox.Show("����������� ����� ����������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show("����� �������� ����������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show("���������� �������� �� X ����������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show("���������� �������� �� Y ����������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                // ����� ��������� ������
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
                    MessageBox.Show("������� ������� ����������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            firstVertVisorChangeVisibilityButton.Click += (sender, e) =>
            {
                try
                {
                    viewModel.�hangeVisibilityOfFirstVertVisorCommand.Execute(null);

                    // ����� ��������� ������
                    UpdateButtonState(sender!);
                }
                catch
                {
                    MessageBox.Show("������� ������� ����������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            secondVertVisorChangeVisibilityButton.Click += (sender, e) =>
            {
                try
                {
                    viewModel.�hangeVisibilityOfSecondVertVisorCommand.Execute(null);

                    // ����� ��������� ������
                    UpdateButtonState(sender!);
                }
                catch
                {
                    MessageBox.Show("������� ������� ����������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }

        #endregion

        #region [ Static ]

        // ����������� ������
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

        // ������ ����������� �������
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
                MessageBox.Show("��������� ������������ ��������� ����������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("�������� ��������� ����������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
