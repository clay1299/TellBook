using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace TellBook{
    public class MainViewModel
    {
        public ICommand CloseWindowCommand { get; }
        public ICommand Window_DragMoveCommand { get; }
        public ICommand WindowOpen_Book_Command { get; }
        public ICommand WindowOpen_Main_Command { get; }

        public MainViewModel()
        {
            CloseWindowCommand = new RelayCommand(_ => CloseWindow());
            Window_DragMoveCommand = new RelayCommand(Window_DragMove);
            WindowOpen_Book_Command = new RelayCommand(_ => WindowOpen_Book());
            WindowOpen_Main_Command = new RelayCommand(_ => WindowOpen_Main());
        }

        private void CloseWindow()
        {
            Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w.IsActive)?
            .Close();
        }
        private void Window_DragMove(object parameter)
        {
            if (parameter is MouseButtonEventArgs e && e.LeftButton == MouseButtonState.Pressed)
            {
                var window = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
                window?.DragMove();
            }
        }
        private void WindowOpen_Book()
        {
            var currentWindow = Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w.IsActive);

            ViewWindow viewWindow = new ViewWindow();
            viewWindow.Show();

            currentWindow.Close();
        }
        private void WindowOpen_Main()
        {
            var currentWindow = Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w.IsActive);

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            currentWindow.Close();
        }
    }
}