using COMTRADE;

namespace COMTRADEInsight.Views
{
    /// <summary>
    /// Представление окна конфигурации файла.
    /// </summary>
    public partial class ConfigForm : Form
    {
        #region [ Constructors ]

        // Конструктор
        public ConfigForm(Schema mySchema)
        {   
            InitializeComponent();

            labelStationName.Text = labelStationName.Text + " " + mySchema.StationName;
            labelDeviceID.Text = labelDeviceID.Text + " " + mySchema.DeviceID;
            labelVersion.Text = labelVersion.Text + " " + mySchema.Version;
            labelTotalChannels.Text = labelTotalChannels.Text + " " + mySchema.TotalChannels;
            labelTotalAnalogChannels.Text = labelTotalAnalogChannels.Text + " " + mySchema.TotalAnalogChannels;
            labelTotalDigitalChannels.Text = labelTotalDigitalChannels.Text + " " + mySchema.TotalDigitalChannels;
            labelNominalFrequency.Text = labelNominalFrequency.Text + " " + mySchema.NominalFrequency;
            labelTotalSampleRates.Text = labelTotalSampleRates.Text + " " + mySchema.TotalSampleRates;
            labelSampleRates.Text = labelSampleRates.Text + " " + mySchema.SampleRates![0].Rate;

            for (int i = 1; i < mySchema.TotalSampleRates; i++)
                labelSampleRates.Text = labelSampleRates.Text + ", " + mySchema.SampleRates[i].Rate;

            labelTotalSamples.Text = labelTotalSamples.Text + " " + mySchema.TotalSamples;
            labelStartTime.Text = labelStartTime.Text + " " + mySchema.StartTime;
            labelTriggerTime.Text = labelTriggerTime.Text + " " + mySchema.TriggerTime;
            labelTimeCode.Text = labelTimeCode.Text + " " + mySchema.TimeCode;
            labelLocalCode.Text = labelLocalCode.Text + " " + mySchema.LocalCode;
            labelTimeFactor.Text = labelTimeFactor.Text + " " + mySchema.TimeFactor;
            labelFileType.Text = labelFileType.Text + " " + mySchema.FileType;
            labelTimeQuality.Text = labelTimeQuality.Text + " " + mySchema.TimeQualityIndicatorCode;
            labelLeapSecond.Text = labelLeapSecond.Text + " " + mySchema.LeapSecondIndicator;
        }

        #endregion

        #region [ Other ]

        // Прочие обработчики событий
        private void OkButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion
    }
}
