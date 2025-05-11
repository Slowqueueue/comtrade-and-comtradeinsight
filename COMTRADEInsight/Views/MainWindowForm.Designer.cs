namespace COMTRADEInsight
{
    partial class MainWindowForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindowForm));
            mainScreenBindingSource = new BindingSource(components);
            menuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openFileToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            configToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            signalPlotsPanel = new Panel();
            moveForthButton = new Button();
            buttonsPanel = new Panel();
            firstVertVisorChangeVisibilityButton = new Button();
            secondVertVisorChangeVisibilityButton = new Button();
            cropPlotButton = new Button();
            dynamicPlotButton = new Button();
            scaleYDownButon = new Button();
            scaleYUpButon = new Button();
            scaleXDownButon = new Button();
            scaleXUpButon = new Button();
            resetScaleButton = new Button();
            moveBackButton = new Button();
            vectorDiagramPanel = new Panel();
            spectralDiagramPanel = new Panel();
            visorsDataPanel = new Panel();
            currentSampleLabel = new Label();
            valueHorVisorsDeltaLabel = new Label();
            valueSecondHorVIsorLabel = new Label();
            valueFirstHorVisorLabel = new Label();
            timeVertVisorsDeltaLabel = new Label();
            timeSecondVertVisorLabel = new Label();
            timeFirstVertVisorLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)mainScreenBindingSource).BeginInit();
            menuStrip.SuspendLayout();
            buttonsPanel.SuspendLayout();
            visorsDataPanel.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Padding = new Padding(0, 2, 0, 2);
            menuStrip.Size = new Size(1904, 24);
            menuStrip.TabIndex = 3;
            menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openFileToolStripMenuItem, saveToolStripMenuItem, configToolStripMenuItem, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(48, 20);
            fileToolStripMenuItem.Text = "Файл";
            // 
            // openFileToolStripMenuItem
            // 
            openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            openFileToolStripMenuItem.Size = new Size(163, 22);
            openFileToolStripMenuItem.Text = "Открыть...";
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new Size(163, 22);
            saveToolStripMenuItem.Text = "Сохранить как...";
            // 
            // configToolStripMenuItem
            // 
            configToolStripMenuItem.Enabled = false;
            configToolStripMenuItem.Name = "configToolStripMenuItem";
            configToolStripMenuItem.Size = new Size(163, 22);
            configToolStripMenuItem.Text = "Конфигурация";
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(163, 22);
            exitToolStripMenuItem.Text = "Выход";
            // 
            // signalPlotsPanel
            // 
            signalPlotsPanel.AutoScroll = true;
            signalPlotsPanel.BackColor = SystemColors.GradientActiveCaption;
            signalPlotsPanel.BorderStyle = BorderStyle.FixedSingle;
            signalPlotsPanel.Location = new Point(4, 119);
            signalPlotsPanel.Margin = new Padding(0);
            signalPlotsPanel.Name = "signalPlotsPanel";
            signalPlotsPanel.Size = new Size(1605, 653);
            signalPlotsPanel.TabIndex = 4;
            // 
            // moveForthButton
            // 
            moveForthButton.Cursor = Cursors.Hand;
            moveForthButton.Enabled = false;
            moveForthButton.Location = new Point(3, 3);
            moveForthButton.Name = "moveForthButton";
            moveForthButton.Size = new Size(108, 81);
            moveForthButton.TabIndex = 5;
            moveForthButton.Text = "Вперед";
            moveForthButton.UseVisualStyleBackColor = true;
            // 
            // buttonsPanel
            // 
            buttonsPanel.BorderStyle = BorderStyle.FixedSingle;
            buttonsPanel.Controls.Add(firstVertVisorChangeVisibilityButton);
            buttonsPanel.Controls.Add(secondVertVisorChangeVisibilityButton);
            buttonsPanel.Controls.Add(cropPlotButton);
            buttonsPanel.Controls.Add(dynamicPlotButton);
            buttonsPanel.Controls.Add(scaleYDownButon);
            buttonsPanel.Controls.Add(scaleYUpButon);
            buttonsPanel.Controls.Add(scaleXDownButon);
            buttonsPanel.Controls.Add(scaleXUpButon);
            buttonsPanel.Controls.Add(resetScaleButton);
            buttonsPanel.Controls.Add(moveBackButton);
            buttonsPanel.Controls.Add(moveForthButton);
            buttonsPanel.Location = new Point(4, 27);
            buttonsPanel.Name = "buttonsPanel";
            buttonsPanel.Size = new Size(1256, 89);
            buttonsPanel.TabIndex = 6;
            // 
            // firstVertVisorChangeVisibilityButton
            // 
            firstVertVisorChangeVisibilityButton.Cursor = Cursors.Hand;
            firstVertVisorChangeVisibilityButton.Enabled = false;
            firstVertVisorChangeVisibilityButton.Location = new Point(1029, 3);
            firstVertVisorChangeVisibilityButton.Name = "firstVertVisorChangeVisibilityButton";
            firstVertVisorChangeVisibilityButton.Size = new Size(108, 81);
            firstVertVisorChangeVisibilityButton.TabIndex = 16;
            firstVertVisorChangeVisibilityButton.Text = "Вертикальный визир 1";
            firstVertVisorChangeVisibilityButton.UseVisualStyleBackColor = true;
            // 
            // secondVertVisorChangeVisibilityButton
            // 
            secondVertVisorChangeVisibilityButton.Cursor = Cursors.Hand;
            secondVertVisorChangeVisibilityButton.Enabled = false;
            secondVertVisorChangeVisibilityButton.Location = new Point(1143, 3);
            secondVertVisorChangeVisibilityButton.Name = "secondVertVisorChangeVisibilityButton";
            secondVertVisorChangeVisibilityButton.Size = new Size(108, 81);
            secondVertVisorChangeVisibilityButton.TabIndex = 15;
            secondVertVisorChangeVisibilityButton.Text = "Вертикальный визир 2";
            secondVertVisorChangeVisibilityButton.UseVisualStyleBackColor = true;
            // 
            // cropPlotButton
            // 
            cropPlotButton.Cursor = Cursors.Hand;
            cropPlotButton.Enabled = false;
            cropPlotButton.Location = new Point(915, 3);
            cropPlotButton.Name = "cropPlotButton";
            cropPlotButton.Size = new Size(108, 81);
            cropPlotButton.TabIndex = 13;
            cropPlotButton.Text = "Обрезать";
            cropPlotButton.UseVisualStyleBackColor = true;
            // 
            // dynamicPlotButton
            // 
            dynamicPlotButton.Cursor = Cursors.Hand;
            dynamicPlotButton.Enabled = false;
            dynamicPlotButton.Location = new Point(801, 3);
            dynamicPlotButton.Name = "dynamicPlotButton";
            dynamicPlotButton.Size = new Size(108, 81);
            dynamicPlotButton.TabIndex = 12;
            dynamicPlotButton.Text = "Динамическая отрисовка";
            dynamicPlotButton.UseVisualStyleBackColor = true;
            // 
            // scaleYDownButon
            // 
            scaleYDownButon.Cursor = Cursors.Hand;
            scaleYDownButon.Enabled = false;
            scaleYDownButon.Location = new Point(687, 3);
            scaleYDownButon.Name = "scaleYDownButon";
            scaleYDownButon.Size = new Size(108, 81);
            scaleYDownButon.TabIndex = 11;
            scaleYDownButon.Text = "Уменьшить масштаб по Y";
            scaleYDownButon.UseVisualStyleBackColor = true;
            // 
            // scaleYUpButon
            // 
            scaleYUpButon.Cursor = Cursors.Hand;
            scaleYUpButon.Enabled = false;
            scaleYUpButon.Location = new Point(573, 3);
            scaleYUpButon.Name = "scaleYUpButon";
            scaleYUpButon.Size = new Size(108, 81);
            scaleYUpButon.TabIndex = 10;
            scaleYUpButon.Text = "Увеличить масштаб по Y";
            scaleYUpButon.UseVisualStyleBackColor = true;
            // 
            // scaleXDownButon
            // 
            scaleXDownButon.Cursor = Cursors.Hand;
            scaleXDownButon.Enabled = false;
            scaleXDownButon.Location = new Point(459, 3);
            scaleXDownButon.Name = "scaleXDownButon";
            scaleXDownButon.Size = new Size(108, 81);
            scaleXDownButon.TabIndex = 9;
            scaleXDownButon.Text = "Уменьшить масштаб по X";
            scaleXDownButon.UseVisualStyleBackColor = true;
            // 
            // scaleXUpButon
            // 
            scaleXUpButon.Cursor = Cursors.Hand;
            scaleXUpButon.Enabled = false;
            scaleXUpButon.Location = new Point(345, 3);
            scaleXUpButon.Name = "scaleXUpButon";
            scaleXUpButon.Size = new Size(108, 81);
            scaleXUpButon.TabIndex = 8;
            scaleXUpButon.Text = "Увеличить масштаб по X";
            scaleXUpButon.UseVisualStyleBackColor = true;
            // 
            // resetScaleButton
            // 
            resetScaleButton.Cursor = Cursors.Hand;
            resetScaleButton.Enabled = false;
            resetScaleButton.Location = new Point(231, 3);
            resetScaleButton.Name = "resetScaleButton";
            resetScaleButton.Size = new Size(108, 81);
            resetScaleButton.TabIndex = 7;
            resetScaleButton.Text = "Сбросить масштаб";
            resetScaleButton.UseVisualStyleBackColor = true;
            // 
            // moveBackButton
            // 
            moveBackButton.Cursor = Cursors.Hand;
            moveBackButton.Enabled = false;
            moveBackButton.Location = new Point(117, 3);
            moveBackButton.Name = "moveBackButton";
            moveBackButton.Size = new Size(108, 81);
            moveBackButton.TabIndex = 6;
            moveBackButton.Text = "Назад";
            moveBackButton.UseVisualStyleBackColor = true;
            // 
            // vectorDiagramPanel
            // 
            vectorDiagramPanel.BackColor = SystemColors.ControlLightLight;
            vectorDiagramPanel.BorderStyle = BorderStyle.FixedSingle;
            vectorDiagramPanel.Location = new Point(1612, 119);
            vectorDiagramPanel.Name = "vectorDiagramPanel";
            vectorDiagramPanel.Size = new Size(304, 890);
            vectorDiagramPanel.TabIndex = 7;
            // 
            // spectralDiagramPanel
            // 
            spectralDiagramPanel.BackColor = SystemColors.ControlLightLight;
            spectralDiagramPanel.BorderStyle = BorderStyle.FixedSingle;
            spectralDiagramPanel.Location = new Point(4, 775);
            spectralDiagramPanel.Name = "spectralDiagramPanel";
            spectralDiagramPanel.Size = new Size(1605, 234);
            spectralDiagramPanel.TabIndex = 8;
            // 
            // visorsDataPanel
            // 
            visorsDataPanel.BackColor = SystemColors.GradientActiveCaption;
            visorsDataPanel.BorderStyle = BorderStyle.FixedSingle;
            visorsDataPanel.Controls.Add(currentSampleLabel);
            visorsDataPanel.Controls.Add(valueHorVisorsDeltaLabel);
            visorsDataPanel.Controls.Add(valueSecondHorVIsorLabel);
            visorsDataPanel.Controls.Add(valueFirstHorVisorLabel);
            visorsDataPanel.Controls.Add(timeVertVisorsDeltaLabel);
            visorsDataPanel.Controls.Add(timeSecondVertVisorLabel);
            visorsDataPanel.Controls.Add(timeFirstVertVisorLabel);
            visorsDataPanel.Dock = DockStyle.Bottom;
            visorsDataPanel.Location = new Point(0, 1015);
            visorsDataPanel.Name = "visorsDataPanel";
            visorsDataPanel.Size = new Size(1904, 26);
            visorsDataPanel.TabIndex = 9;
            visorsDataPanel.TabStop = true;
            // 
            // currentSampleLabel
            // 
            currentSampleLabel.AutoSize = true;
            currentSampleLabel.Location = new Point(1207, 3);
            currentSampleLabel.Name = "currentSampleLabel";
            currentSampleLabel.Size = new Size(100, 15);
            currentSampleLabel.TabIndex = 6;
            currentSampleLabel.Text = "Номер выборки:";
            // 
            // valueHorVisorsDeltaLabel
            // 
            valueHorVisorsDeltaLabel.AutoSize = true;
            valueHorVisorsDeltaLabel.Location = new Point(907, 3);
            valueHorVisorsDeltaLabel.Name = "valueHorVisorsDeltaLabel";
            valueHorVisorsDeltaLabel.Size = new Size(128, 15);
            valueHorVisorsDeltaLabel.TabIndex = 5;
            valueHorVisorsDeltaLabel.Text = "Разница гор. визиров:";
            // 
            // valueSecondHorVIsorLabel
            // 
            valueSecondHorVIsorLabel.AutoSize = true;
            valueSecondHorVIsorLabel.Location = new Point(757, 3);
            valueSecondHorVIsorLabel.Name = "valueSecondHorVIsorLabel";
            valueSecondHorVIsorLabel.Size = new Size(77, 15);
            valueSecondHorVIsorLabel.TabIndex = 4;
            valueSecondHorVIsorLabel.Text = "Гор. визир 2:";
            // 
            // valueFirstHorVisorLabel
            // 
            valueFirstHorVisorLabel.AutoSize = true;
            valueFirstHorVisorLabel.Location = new Point(607, 3);
            valueFirstHorVisorLabel.Name = "valueFirstHorVisorLabel";
            valueFirstHorVisorLabel.Size = new Size(77, 15);
            valueFirstHorVisorLabel.TabIndex = 3;
            valueFirstHorVisorLabel.Text = "Гор. визир 1:";
            // 
            // timeVertVisorsDeltaLabel
            // 
            timeVertVisorsDeltaLabel.AutoSize = true;
            timeVertVisorsDeltaLabel.Location = new Point(307, 3);
            timeVertVisorsDeltaLabel.Name = "timeVertVisorsDeltaLabel";
            timeVertVisorsDeltaLabel.Size = new Size(133, 15);
            timeVertVisorsDeltaLabel.TabIndex = 2;
            timeVertVisorsDeltaLabel.Text = "Разница верт. визиров:";
            // 
            // timeSecondVertVisorLabel
            // 
            timeSecondVertVisorLabel.AutoSize = true;
            timeSecondVertVisorLabel.Location = new Point(157, 3);
            timeSecondVertVisorLabel.Name = "timeSecondVertVisorLabel";
            timeSecondVertVisorLabel.Size = new Size(82, 15);
            timeSecondVertVisorLabel.TabIndex = 1;
            timeSecondVertVisorLabel.Text = "Верт. визир 2:";
            // 
            // timeFirstVertVisorLabel
            // 
            timeFirstVertVisorLabel.AutoSize = true;
            timeFirstVertVisorLabel.Location = new Point(7, 3);
            timeFirstVertVisorLabel.Name = "timeFirstVertVisorLabel";
            timeFirstVertVisorLabel.Size = new Size(82, 15);
            timeFirstVertVisorLabel.TabIndex = 0;
            timeFirstVertVisorLabel.Text = "Верт. визир 1:";
            // 
            // MainWindowForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            AutoScrollMargin = new Size(0, 100);
            ClientSize = new Size(1904, 1041);
            Controls.Add(visorsDataPanel);
            Controls.Add(spectralDiagramPanel);
            Controls.Add(vectorDiagramPanel);
            Controls.Add(buttonsPanel);
            Controls.Add(signalPlotsPanel);
            Controls.Add(menuStrip);
            DoubleBuffered = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip;
            Name = "MainWindowForm";
            Text = "COMTRADE Insight";
            FormClosing += MainForm_FormClosing_1;
            ((System.ComponentModel.ISupportInitialize)mainScreenBindingSource).EndInit();
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            buttonsPanel.ResumeLayout(false);
            visorsDataPanel.ResumeLayout(false);
            visorsDataPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private BindingSource mainScreenBindingSource;
        private MenuStrip menuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openFileToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem configToolStripMenuItem;
        private Panel signalPlotsPanel;
        private Button moveForthButton;
        private Panel buttonsPanel;
        private Button moveBackButton;
        private Panel vectorDiagramPanel;
        private Panel spectralDiagramPanel;
        private Button resetScaleButton;
        private Button scaleXUpButon;
        private Button scaleYDownButon;
        private Button scaleYUpButon;
        private Button scaleXDownButon;
        private Panel visorsDataPanel;
        private Button dynamicPlotButton;
        private Label timeFirstVertVisorLabel;
        private Label timeSecondVertVisorLabel;
        private Label valueHorVisorsDeltaLabel;
        private Label valueSecondHorVIsorLabel;
        private Label valueFirstHorVisorLabel;
        private Label timeVertVisorsDeltaLabel;
        private Button cropPlotButton;
        private Label currentSampleLabel;
        private Button secondVertVisorChangeVisibilityButton;
        private Button firstVertVisorChangeVisibilityButton;
    }
}
