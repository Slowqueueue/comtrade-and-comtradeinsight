using COMTRADE;
using COMTRADEInsight.Models;
using System.ComponentModel;
using System.Windows.Input;
using static COMTRADEInsight.Models.SaveModel;

namespace COMTRADEInsight.ViewModels
{
    /// <summary>
    /// Модель Представления окна сохранения файла.
    /// </summary>
    public class SaveViewModel : INotifyPropertyChanged
    {
        #region [ Members ]

        // Поля
        private readonly ICommand _openFolderDialogCommand; // Команда открытия окна выбора папки для сохранения
        private readonly ICommand _saveFileCommand; // Команда сохранения файла

        private string filePath = ""; // Путь к папке для сохранения файла
        private string fileName = ""; // Имя сохраняемого файла
        private string selectedOptionFormat = "CFF"; // Выбранный формат файла
        private string selectedOptionType = "ASCII"; // Выбранный тип файла
        private bool dontWriteDatFile; // Флаг, указывающий, стоит ли пропускать информационный файл при сохранении

        #endregion

        #region [ Constructors ]

        // Конструктор
        public SaveViewModel()
        {
            _openFolderDialogCommand = new SimpleCommand(OpenFolderDialog);
            _saveFileCommand = new ObjectConditionCommand(SaveFile, CanSaveFile);
        }

        #endregion

        #region [ Properties ]

        // Свойства
        /// <summary>
        /// Получает или устанавливает настройки сохранения файла.
        /// </summary>
        public SaveSettings Settings { get; set; } = new();

        /// <summary>
        /// Получает или устанавливает путь к папке для сохранения файла.
        /// </summary>
        public string FilePath
        {
            get { return filePath; }
            set
            {
                if (filePath != value)
                {
                    filePath = value;
                    OnPropertyChanged(nameof(FilePath));
                }
            }
        }

        /// <summary>
        /// Получает или устанавливает имя сохраняемого файла.
        /// </summary>
        public string FileName
        {
            get { return fileName; }
            set
            {
                if (fileName != value)
                {
                    fileName = value;
                    OnPropertyChanged(nameof(FileName));
                }
            }
        }

        /// <summary>
        /// Получает или устанавливает выбранный формат файла.
        /// </summary>
        public string SelectedOptionFormat
        {
            get { return selectedOptionFormat; }
            set
            {
                if (selectedOptionFormat != value)
                {
                    selectedOptionFormat = value;
                    OnPropertyChanged(nameof(SelectedOptionFormat));
                }
            }
        }

        /// <summary>
        /// Получает или устанавливает выбранный тип файла.
        /// </summary>
        public string SelectedOptionType
        {
            get { return selectedOptionType; }
            set
            {
                if (selectedOptionType != value)
                {
                    selectedOptionType = value;
                    OnPropertyChanged(nameof(SelectedOptionType));
                }
            }
        }

        /// <summary>
        /// Получает или устанавливает флаг, указывающий, стоит ли пропускать информационный файл при сохранении.
        /// </summary>
        public bool DontWriteDatFile
        {
            get { return dontWriteDatFile; }
            set
            {
                if (dontWriteDatFile != value)
                {
                    dontWriteDatFile = value;
                    OnPropertyChanged(nameof(dontWriteDatFile));
                }
            }
        }

        /// <summary>
        /// Получает команду открытия окна выбора папки для сохранения.
        /// </summary>
        public ICommand OpenFolderDialogCommand
        {
            get { return _openFolderDialogCommand; }
        }

        /// <summary>
        /// Получает команду сохранения файла.
        /// </summary>
        /// <remarks>Принимает кортеж из трех элементов типов <see cref="Schema"/>, <see cref="DateTime"/> и <see cref="List{List{Double}"/>, который является списком списков <see cref="Double"/> как объект.</remarks> 
        public ICommand SaveFileCommand
        {
            get { return _saveFileCommand; }
        }

        #endregion

        #region [ Methods ]

        // Методы

        //Открытие окна выбора папки для сохранения
        private void OpenFolderDialog()
        {
            // Настройка, открытие и обработка диалогового окна
            FolderBrowserDialog folderDialog = new() 
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            DialogResult result = folderDialog.ShowDialog();

            if (result == DialogResult.OK)
                FilePath = folderDialog.SelectedPath;
        }

        // Проверка на возможность сохранения файла
        private bool CanSaveFile()
        {
            return !string.IsNullOrWhiteSpace(FileName) && !string.IsNullOrWhiteSpace(FilePath);
        }

        // Сохранение файла
        private void SaveFile(object? obj)
        {
            // Заполнение настроек сохранения
            Settings.FilePath = FilePath;
            Settings.FileName = FileName;

            switch (SelectedOptionFormat)
            {
                case "CFF":
                    Settings.Format = FileFormat.CFF;
                    break;
                case "CFG":
                    Settings.Format = FileFormat.CFG;
                    break;
            }

            switch (SelectedOptionType)
            {
                case "ASCII":
                    Settings.Type = FileType.Ascii;
                    break;
                case "BINARY":
                    Settings.Type = FileType.Binary;
                    break;
                case "BINARY32":
                    Settings.Type = FileType.Binary32;
                    break;
                case "FLOAT32":
                    Settings.Type = FileType.Float32;
                    break;
            }

            Settings.DontWriteDatFile = DontWriteDatFile;

            // Сохранение
            if (obj is Tuple<Schema, DateTime, List<List<double>>> schemaTimeStampData)
                WriteComtradeFiles(schemaTimeStampData, Settings);
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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
