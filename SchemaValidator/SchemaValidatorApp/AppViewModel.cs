using Microsoft.Win32;
using SchemaValidatorApp.Core;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SchemaValidatorApp
{
    public class AppViewModel : INotifyPropertyChanged
    {
        private readonly SchemaValidator.Validator _schemaValidator;
        public AppViewModel()
        {
            SelectSchemaFilePathCommand = new Command(() => !IsBusy,  SelectSchemaFile);
            SelectInputFilePathCommand = new Command(() => !IsBusy, SelectInputFile);
            SelectOutputFilePathCommand = new Command(() => !IsBusy, SelectOutputPath);
            ValidateCommand = new AsyncCommand(() => !IsBusy, Validate);
            _schemaValidator = new SchemaValidator.Validator();
        }

        private async Task Validate()
        {
            IsBusy = true;

            await _schemaValidator.ValidateAsync(InputFilePath, SchemaFilePath, OutputFilePath);

            IsBusy = false;
        }

        private string outputFilePath;
        public string OutputFilePath
        {
            get => outputFilePath;
            set
            {
                Set(ref outputFilePath, value, nameof(OutputFilePath));
                OnPropertyChanged(nameof(OutputFilePathText));
            }
        }

        public string OutputFilePathText => $"Output json: {outputFilePath}";

        private void SelectOutputPath()
        {
            var openFolderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            openFolderDialog.RootFolder = Environment.SpecialFolder.MyDocuments;
            if (openFolderDialog.ShowDialog() == true)
            {
                OutputFilePath = openFolderDialog.SelectedPath;
            }
        }

        private string schemaFilePath;
        public string SchemaFilePath
        {
            get => schemaFilePath;
            set 
            { 
                Set(ref schemaFilePath, value, nameof(SchemaFilePath)); 
                OnPropertyChanged(nameof(SchemaFilePathText));
            }
        }

        public string SchemaFilePathText => $"Schema json: {schemaFilePath}";

        private void SelectSchemaFile()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Filter = "Json file |schema.json;";
            if (openFileDialog.ShowDialog() == true)
            {
                SchemaFilePath = openFileDialog.FileName;
            }
        }

        private string inputFilePath;
        public string InputFilePath
        {
            get => inputFilePath;
            set
            {
                Set(ref inputFilePath, value, nameof(InputFilePath));
                OnPropertyChanged(nameof(InputFilePathText));
            }
        }

        public string InputFilePathText => $"Input json: {inputFilePath}";

        private void SelectInputFile()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Filter = "Json file |input.json;";
            if (openFileDialog.ShowDialog() == true)
            {
                InputFilePath = openFileDialog.FileName;
            }
        }

        private bool isBusy = false;
        public bool IsBusy
        {
            get => isBusy;
            set 
            { 
                Set(ref isBusy, value, nameof(IsBusy));
                SelectSchemaFilePathCommand.RaiseCanExecuteChanged();
                SelectInputFilePathCommand.RaiseCanExecuteChanged();
                SelectOutputFilePathCommand.RaiseCanExecuteChanged();
                ValidateCommand.RaiseCanExecuteChanged();
            }
        }

        public Command SelectInputFilePathCommand { get; private set; }

        public Command SelectSchemaFilePathCommand { get; private set; }

        public Command SelectOutputFilePathCommand { get; private set; }

        public AsyncCommand ValidateCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool Set<T>(ref T field, T newValue = default(T), [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            field = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
