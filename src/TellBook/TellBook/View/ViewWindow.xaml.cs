using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;

namespace TellBook
{
    public partial class ViewWindow : Window
    {
        private string filePath;
        private ObservableCollection<Contact> originalData;
        private ObservableCollection<Contact> filteredData;
        public ViewWindow()
        {
            InitializeComponent();
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            filePath = window.filePath;

            ObservableCollection<Contact> contacts = ReadJson(filePath);
            ContactList.ItemsSource = contacts;

            originalData = new ObservableCollection<Contact>(contacts);
            filteredData = new ObservableCollection<Contact>(originalData);
            ContactList.ItemsSource = filteredData;
        }

        private static ObservableCollection<Contact> ReadJson(string filePath)
        {
            try
            {
                string jsonContent = File.ReadAllText(filePath, Encoding.UTF8);
                return JsonConvert.DeserializeObject<ObservableCollection<Contact>>(jsonContent); 
            }
            catch(System.Exception ex){
                MessageBox.Show("Ошибка в чтении файла " + ex.Message);
                return new ObservableCollection<Contact>();
            }
        }

        private void DeledListRow(object sender, RoutedEventArgs e)
        {
            if (ContactList.SelectedItem == null)
            {
                MessageBox.Show("Выберите контакт для удаления");
                return;
            }

            ObservableCollection<Contact> contacts = (ObservableCollection<Contact>)ContactList.ItemsSource;

            contacts.Remove((Contact)ContactList.SelectedItem);

            SaveContactsToJson(contacts, filePath);

            ContactList.ItemsSource = null;
            ContactList.ItemsSource = contacts; 
        }

        private void SaveContactsToJson(ObservableCollection<Contact> contacts, string filePath)
        {
            try
            {
                string json = JsonConvert.SerializeObject(contacts, Formatting.Indented);
                File.WriteAllText(filePath, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении файла: " + ex.Message);
            }
        }
        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = tbSearch.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                filteredData.Clear();
                foreach (var item in originalData)
                    filteredData.Add(item);
            }
            else
            {
                var filteredItems = originalData.Where(item =>
                    item.Name.ToLower().Contains(searchText) ||
                    item.Phone.ToLower().Contains(searchText) ||
                    item.Description.ToLower().Contains(searchText)).ToList();

                filteredData.Clear();
                foreach (var item in filteredItems)
                    filteredData.Add(item);
            }
        }
    }


}
