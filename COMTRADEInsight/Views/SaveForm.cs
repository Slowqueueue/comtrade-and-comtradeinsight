using COMTRADE;
using COMTRADEInsight.ViewModels;

namespace COMTRADEInsight.Views
{
    /// <summary>
    /// Представление окна сохранения файла.
    /// </summary>
    public partial class SaveForm : Form
    {
        #region [ Members ]

        // Поля
        private readonly SaveViewModel viewModel = new(); // ViewModel окна сохранения

        #endregion

        #region [ Constructors ]

        // Конструктор
        public SaveForm(Tuple<Schema, DateTime, List<List<double>>> schemaTimeStampData)
        {
            InitializeComponent();

            // Привязка ViewModel
            saveAsBindingSource.DataSource = viewModel; 

            // Привязка текстовых полей папки для сохранения и имени файла
            folderRichTextBox.DataBindings.Add("Text", saveAsBindingSource, nameof(viewModel.FilePath), false, DataSourceUpdateMode.OnPropertyChanged);
            fileNameTextBox.DataBindings.Add("Text", saveAsBindingSource, nameof(viewModel.FileName), false, DataSourceUpdateMode.OnValidation);

            // Предварительная запись имени файла
            viewModel.FileName = Path.GetFileNameWithoutExtension(schemaTimeStampData.Item1.FileName!);

            // Привязка радиокнопок формата файла
            BindRadioButton(cffRadioButton, nameof(viewModel.SelectedOptionFormat));
            BindRadioButton(cfgRadioButton, nameof(viewModel.SelectedOptionFormat));

            // Привязка радиокнопок типа файла
            BindRadioButton(asciiRadioButton, nameof(viewModel.SelectedOptionType));
            BindRadioButton(binaryRadioButton, nameof(viewModel.SelectedOptionType));
            BindRadioButton(binary32RadioButton, nameof(viewModel.SelectedOptionType));
            BindRadioButton(float32RadioButton, nameof(viewModel.SelectedOptionType));

            // Привязка флажка "Не сохранять .dat"
            dontWriteDatFileCheckBox.DataBindings.Add("Checked", saveAsBindingSource, nameof(viewModel.DontWriteDatFile), false, DataSourceUpdateMode.OnPropertyChanged);

            // Обработчики событий
            selectFolderButton.Click += (sender, e) =>
            {
                try
                {
                    viewModel.OpenFolderDialogCommand.Execute(null);
                }
                catch 
                {
                    MessageBox.Show("Открытие диалогового окна невозможно.");
                }
            };

            saveButton.Click += (sender, e) =>
            {
                try
                {
                    viewModel.SaveFileCommand.Execute(schemaTimeStampData);
                }
                catch
                {
                    MessageBox.Show("Сохранение невозможно.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            cancelButton.Click += (sender, e) =>
            {
                Close();
            };
        }

        #endregion

        #region [ Methods ]

        // Методы
        private void BindRadioButton(RadioButton radioButton, string viewModelPropertyName)
        {
            radioButton.DataBindings.Add("Checked", saveAsBindingSource, viewModelPropertyName, false, DataSourceUpdateMode.OnPropertyChanged);

            Binding binding = radioButton.DataBindings["Checked"]!;

            // Преобразование из ViewModel в радиокнопку
            binding.Format += (sender, e) =>
            {
                // Если значение во ViewModel соответствует тексту этой радиокнопки, то радиокнопка нажата
                e.Value = (string)saveAsBindingSource.DataSource!.GetType().GetProperty(viewModelPropertyName)!.GetValue(saveAsBindingSource.DataSource)! == radioButton.Text;
            };

            // Преобразование из радиокнопки в ViewModel
            binding.Parse += (sender, e) =>
            {
                // Если радиокнопка нажата, то записываем ее текст в ViewModel
                if ((bool)e.Value!)
                {
                    e.Value = radioButton.Text;
                }
                else
                {
                    string currentlySelected = (string)saveAsBindingSource.DataSource!.GetType().GetProperty(viewModelPropertyName)!.GetValue(saveAsBindingSource.DataSource)!;
                    e.Value = currentlySelected;
                }
            };

        }

        #endregion

        #region [ Other ]

        // Прочие обработчики событий
        private void CfgRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Checked)
                dontWriteDatFileCheckBox.Enabled = true;
            else
                dontWriteDatFileCheckBox.Enabled = false;
        }

        #endregion
    }
}
