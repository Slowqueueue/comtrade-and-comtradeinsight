namespace COMTRADEInsight.Views
{
    partial class ConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            labelStationName = new Label();
            labelDeviceID = new Label();
            labelVersion = new Label();
            labelTotalChannels = new Label();
            labelTotalAnalogChannels = new Label();
            labelTotalDigitalChannels = new Label();
            labelNominalFrequency = new Label();
            labelTotalSampleRates = new Label();
            labelTotalSamples = new Label();
            labelSampleRates = new Label();
            labelStartTime = new Label();
            labelTriggerTime = new Label();
            labelTimeCode = new Label();
            labelLocalCode = new Label();
            labelFileType = new Label();
            labelTimeFactor = new Label();
            labelTimeQuality = new Label();
            labelLeapSecond = new Label();
            OkButton = new Button();
            SuspendLayout();
            // 
            // labelStationName
            // 
            labelStationName.AutoSize = true;
            labelStationName.Font = new Font("Segoe UI", 9F);
            labelStationName.Location = new Point(125, 10);
            labelStationName.Name = "labelStationName";
            labelStationName.Size = new Size(128, 15);
            labelStationName.TabIndex = 0;
            labelStationName.Text = "Название станции:      ";
            // 
            // labelDeviceID
            // 
            labelDeviceID.AutoSize = true;
            labelDeviceID.Font = new Font("Segoe UI", 9F);
            labelDeviceID.Location = new Point(61, 40);
            labelDeviceID.Name = "labelDeviceID";
            labelDeviceID.Size = new Size(192, 15);
            labelDeviceID.TabIndex = 1;
            labelDeviceID.Text = "Идентификатор регистратора:      ";
            // 
            // labelVersion
            // 
            labelVersion.AutoSize = true;
            labelVersion.Font = new Font("Segoe UI", 9F);
            labelVersion.Location = new Point(68, 70);
            labelVersion.Name = "labelVersion";
            labelVersion.Size = new Size(185, 15);
            labelVersion.TabIndex = 2;
            labelVersion.Text = "Версия формата COMTRADE:      ";
            // 
            // labelTotalChannels
            // 
            labelTotalChannels.AutoSize = true;
            labelTotalChannels.Font = new Font("Segoe UI", 9F);
            labelTotalChannels.Location = new Point(71, 100);
            labelTotalChannels.Name = "labelTotalChannels";
            labelTotalChannels.Size = new Size(182, 15);
            labelTotalChannels.TabIndex = 3;
            labelTotalChannels.Text = "Общее количество каналов:      ";
            // 
            // labelTotalAnalogChannels
            // 
            labelTotalAnalogChannels.AutoSize = true;
            labelTotalAnalogChannels.Font = new Font("Segoe UI", 9F);
            labelTotalAnalogChannels.Location = new Point(43, 130);
            labelTotalAnalogChannels.Name = "labelTotalAnalogChannels";
            labelTotalAnalogChannels.Size = new Size(210, 15);
            labelTotalAnalogChannels.TabIndex = 4;
            labelTotalAnalogChannels.Text = "Количество аналоговых каналов:      ";
            // 
            // labelTotalDigitalChannels
            // 
            labelTotalDigitalChannels.AutoSize = true;
            labelTotalDigitalChannels.Font = new Font("Segoe UI", 9F);
            labelTotalDigitalChannels.Location = new Point(44, 160);
            labelTotalDigitalChannels.Name = "labelTotalDigitalChannels";
            labelTotalDigitalChannels.Size = new Size(209, 15);
            labelTotalDigitalChannels.TabIndex = 5;
            labelTotalDigitalChannels.Text = "Количество дискретных каналов:      ";
            // 
            // labelNominalFrequency
            // 
            labelNominalFrequency.AutoSize = true;
            labelNominalFrequency.Font = new Font("Segoe UI", 9F);
            labelNominalFrequency.Location = new Point(136, 190);
            labelNominalFrequency.Name = "labelNominalFrequency";
            labelNominalFrequency.Size = new Size(117, 15);
            labelNominalFrequency.TabIndex = 6;
            labelNominalFrequency.Text = "Частота сети, Гц:      ";
            // 
            // labelTotalSampleRates
            // 
            labelTotalSampleRates.AutoSize = true;
            labelTotalSampleRates.Font = new Font("Segoe UI", 9F);
            labelTotalSampleRates.Location = new Point(36, 220);
            labelTotalSampleRates.Name = "labelTotalSampleRates";
            labelTotalSampleRates.Size = new Size(217, 15);
            labelTotalSampleRates.TabIndex = 7;
            labelTotalSampleRates.Text = "Количество частот дискретизации:      ";
            // 
            // labelTotalSamples
            // 
            labelTotalSamples.AutoSize = true;
            labelTotalSamples.Font = new Font("Segoe UI", 9F);
            labelTotalSamples.Location = new Point(67, 280);
            labelTotalSamples.Name = "labelTotalSamples";
            labelTotalSamples.Size = new Size(186, 15);
            labelTotalSamples.TabIndex = 8;
            labelTotalSamples.Text = "Общее количество выборок:      ";
            // 
            // labelSampleRates
            // 
            labelSampleRates.AutoSize = true;
            labelSampleRates.Font = new Font("Segoe UI", 9F);
            labelSampleRates.Location = new Point(75, 250);
            labelSampleRates.Name = "labelSampleRates";
            labelSampleRates.Size = new Size(178, 15);
            labelSampleRates.TabIndex = 9;
            labelSampleRates.Text = "Частоты дискретизации, Гц:      ";
            // 
            // labelStartTime
            // 
            labelStartTime.AutoSize = true;
            labelStartTime.Font = new Font("Segoe UI", 9F);
            labelStartTime.Location = new Point(107, 310);
            labelStartTime.Name = "labelStartTime";
            labelStartTime.Size = new Size(146, 15);
            labelStartTime.TabIndex = 10;
            labelStartTime.Text = "Время начала записи:      ";
            // 
            // labelTriggerTime
            // 
            labelTriggerTime.AutoSize = true;
            labelTriggerTime.Font = new Font("Segoe UI", 9F);
            labelTriggerTime.Location = new Point(93, 340);
            labelTriggerTime.Name = "labelTriggerTime";
            labelTriggerTime.Size = new Size(160, 15);
            labelTriggerTime.TabIndex = 11;
            labelTriggerTime.Text = "Время момента запуска:      ";
            // 
            // labelTimeCode
            // 
            labelTimeCode.AutoSize = true;
            labelTimeCode.Font = new Font("Segoe UI", 9F);
            labelTimeCode.Location = new Point(50, 370);
            labelTimeCode.Name = "labelTimeCode";
            labelTimeCode.Size = new Size(203, 15);
            labelTimeCode.TabIndex = 12;
            labelTimeCode.Text = "Смещение по UTC для выборок:      ";
            // 
            // labelLocalCode
            // 
            labelLocalCode.AutoSize = true;
            labelLocalCode.Font = new Font("Segoe UI", 9F);
            labelLocalCode.Location = new Point(25, 400);
            labelLocalCode.Name = "labelLocalCode";
            labelLocalCode.Size = new Size(228, 15);
            labelLocalCode.TabIndex = 13;
            labelLocalCode.Text = "Смещение по UTC для регистратора:      ";
            // 
            // labelFileType
            // 
            labelFileType.AutoSize = true;
            labelFileType.Font = new Font("Segoe UI", 9F);
            labelFileType.Location = new Point(160, 460);
            labelFileType.Name = "labelFileType";
            labelFileType.Size = new Size(93, 15);
            labelFileType.TabIndex = 14;
            labelFileType.Text = "Тип файлов:      ";
            // 
            // labelTimeFactor
            // 
            labelTimeFactor.AutoSize = true;
            labelTimeFactor.Font = new Font("Segoe UI", 9F);
            labelTimeFactor.Location = new Point(30, 430);
            labelTimeFactor.Name = "labelTimeFactor";
            labelTimeFactor.Size = new Size(223, 15);
            labelTimeFactor.TabIndex = 15;
            labelTimeFactor.Text = "Коэффициент умножения времени:      ";
            // 
            // labelTimeQuality
            // 
            labelTimeQuality.AutoSize = true;
            labelTimeQuality.Font = new Font("Segoe UI", 9F);
            labelTimeQuality.Location = new Point(12, 490);
            labelTimeQuality.Name = "labelTimeQuality";
            labelTimeQuality.Size = new Size(241, 15);
            labelTimeQuality.TabIndex = 16;
            labelTimeQuality.Text = "Индикатор качества временных меток:      ";
            // 
            // labelLeapSecond
            // 
            labelLeapSecond.AutoSize = true;
            labelLeapSecond.Font = new Font("Segoe UI", 9F);
            labelLeapSecond.Location = new Point(20, 520);
            labelLeapSecond.Name = "labelLeapSecond";
            labelLeapSecond.Size = new Size(233, 15);
            labelLeapSecond.TabIndex = 17;
            labelLeapSecond.Text = "Индикатор дополнительной секунды:      ";
            // 
            // OkButton
            // 
            OkButton.Location = new Point(505, 519);
            OkButton.Name = "OkButton";
            OkButton.Size = new Size(108, 28);
            OkButton.TabIndex = 18;
            OkButton.Text = "ОК";
            OkButton.UseVisualStyleBackColor = true;
            OkButton.Click += OkButton_Click;
            // 
            // ConfigForm
            // 
            AcceptButton = OkButton;
            AutoScaleMode = AutoScaleMode.None;
            CancelButton = OkButton;
            ClientSize = new Size(628, 562);
            Controls.Add(OkButton);
            Controls.Add(labelLeapSecond);
            Controls.Add(labelTimeQuality);
            Controls.Add(labelTimeFactor);
            Controls.Add(labelFileType);
            Controls.Add(labelLocalCode);
            Controls.Add(labelTimeCode);
            Controls.Add(labelTriggerTime);
            Controls.Add(labelStartTime);
            Controls.Add(labelSampleRates);
            Controls.Add(labelTotalSamples);
            Controls.Add(labelTotalSampleRates);
            Controls.Add(labelNominalFrequency);
            Controls.Add(labelTotalDigitalChannels);
            Controls.Add(labelTotalAnalogChannels);
            Controls.Add(labelTotalChannels);
            Controls.Add(labelVersion);
            Controls.Add(labelDeviceID);
            Controls.Add(labelStationName);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ConfigForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Конфигурация";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelStationName;
        private Label labelDeviceID;
        private Label labelVersion;
        private Label labelTotalChannels;
        private Label labelTotalAnalogChannels;
        private Label labelTotalDigitalChannels;
        private Label labelNominalFrequency;
        private Label labelTotalSampleRates;
        private Label labelTotalSamples;
        private Label labelSampleRates;
        private Label labelStartTime;
        private Label labelTriggerTime;
        private Label labelTimeCode;
        private Label labelLocalCode;
        private Label labelFileType;
        private Label labelTimeFactor;
        private Label labelTimeQuality;
        private Label labelLeapSecond;
        private Button OkButton;
    }
}