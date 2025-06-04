using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Diagnostics;


namespace TellBook
{
    public partial class MainWindow : Window
    {
        public string filePath;
        public MainWindow()
        {
            InitializeComponent();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path = Path.Combine(path, "TellBook");
            Directory.CreateDirectory(path);
            filePath = Path.Combine(path, "save.json");
        }

        private void EnterData(object sender, RoutedEventArgs e)
        {
            var contact = new Contact
            {
                Name = tbName.Text,
                Phone = tbPhone.Text,
                Description = tbDescription.Text
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath, Encoding.UTF8);
                    List<Contact> Contacts = JsonSerializer.Deserialize<List<Contact>>(json);

                    Contacts.Add(contact);

                    json = JsonSerializer.Serialize(Contacts, options);
                    File.WriteAllText(filePath, json, Encoding.UTF8);
                }
                else
                {
                    string json = JsonSerializer.Serialize(new List<Contact> { contact }, options);
                    File.WriteAllText(filePath, json, Encoding.UTF8);
                }

                MessageBox.Show("Успешно записан!");
                tbName.Text = string.Empty;
                tbPhone.Text = string.Empty;
                tbDescription.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка" + ex.Message);
                return;
            }

        }

        private void tbPhoneNumberValidation(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1) &&
                e.Text != "+")
            {
                e.Handled = true; 
            }

            TextBox textBox = sender as TextBox;
            if (e.Text == "8" && textBox.Text.Length == 0)
            {
                e.Handled = true;
                textBox.Text = "+7";
                textBox.CaretIndex = 2;
            }
        }
        private void ConvertToPDF_OnClick(object sender, MouseButtonEventArgs e)
        {
            string jsonFilePath = filePath;
            string pdfFilePath = Path.Combine(Path.GetDirectoryName(filePath), "save.pdf");
            try
            {
                Converter.ConvertToPDF(jsonFilePath, pdfFilePath);
            }
            catch (IOException ex) 
            {
                MessageBox.Show($"Ошибка с файлом\n {ex.Message}",
                   "Ошибка данных",
                   MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неизвестная ошибка\n {ex.Message}",
                   "Ошибка",
                   MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show("Файл успешно создан!");
        }

        private void ConvertToExel_OnClick(object sender, MouseButtonEventArgs e)
        {
            string jsonFilePath = filePath;
            string pdfFilePath = Path.Combine(Path.GetDirectoryName(filePath), "save.xlsx");
            try 
            { 
                Converter.ConvertToExel(jsonFilePath, pdfFilePath);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Ошибка с файлом\n {ex.Message}",
                   "Ошибка данных",
                   MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неизвестная ошибка\n {ex.Message}",
                   "Ошибка",
                   MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show("Файл успешно создан!");
        }

        private void OpenDirectory_OnClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var directorypath = Path.GetDirectoryName(filePath);
                Process.Start(directorypath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неизвестная ошибка\n {ex.Message}",
                   "Ошибка",
                   MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
    }

}
