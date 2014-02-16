using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ElevatorSimulator.PassengerArrivals;

namespace ElevatorSimulator.View
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        public string LoadDistributionFilePath { get; private set; }
        public string LoadXMLSpecFilePath { get; private set; }
        public string NewDistributionSavePath { get; private set; }
        public PassengerDistributionSource SourceAction { get; private set; }
        public string LogFilePath { get; private set; }

        public MainMenu()
        {
            InitializeComponent();
            this.NewPanel.Visibility = System.Windows.Visibility.Collapsed;
            this.LoadPanel.Visibility = System.Windows.Visibility.Collapsed;
            RunSimButton.IsEnabled = false;
            CreateDistroButtton.IsEnabled = false;
            SaveNewDistroButton.IsEnabled = false;
            SourceAction = PassengerDistributionSource.Error;
            LogFilePath = string.Empty;
        }

        private void NewRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.NewPanel.Visibility = System.Windows.Visibility.Visible;
            this.LoadPanel.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void LoadRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.NewPanel.Visibility = System.Windows.Visibility.Collapsed;
            this.LoadPanel.Visibility = System.Windows.Visibility.Visible;
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog() { Multiselect = false, Title = "Select Distribution", Filter = "XML files | *.xml" };
            var result = dlg.ShowDialog();

            if (result == true)
            {
                LoadDistributionFilePath = dlg.FileName;
                LoadFilePathTextBlock.Text = dlg.FileName;
                RunSimButton.IsEnabled = true;
            }
        }

        private void SaveNewDistroButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog() { Title = "Create New Distribution", ValidateNames = true, AddExtension = true, DefaultExt = "xml", Filter = "XML files | *.xml" };
            var result = dlg.ShowDialog();

            if (result == true)
            {
                NewDistributionSavePath = dlg.FileName;
                NewFilePathTextBox.Text = dlg.FileName;
                CreateDistroButtton.IsEnabled = true;
            }
        }

        private void SelectXMLSpecButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog() { Multiselect = false, Title = "Select XML Spec", Filter = "XML files | *.xml" };
            var result = dlg.ShowDialog();

            if (result == true)
            {
                LoadXMLSpecFilePath = dlg.FileName;
                XMLSpecFilePathTextBlock.Text = dlg.FileName;
                SaveNewDistroButton.IsEnabled = true;
            }
        }

        private void CreateDistroButtton_Click(object sender, RoutedEventArgs e)
        {
            SourceAction = PassengerDistributionSource.New;
            this.Close();
        }

        private void RunSimButton_Click(object sender, RoutedEventArgs e)
        {
            SourceAction = PassengerDistributionSource.Load;
            this.Close();
        }

        private void SetLogFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog() { Title = "Set Log File", ValidateNames = true, AddExtension = true, DefaultExt = "txt", Filter = "text files | *.txt" };
            var result = dlg.ShowDialog();

            if (result == true)
            {
                LogFilePath = dlg.FileName;
                LogFilePathTextBlock.Text = dlg.FileName;
            }
        }


    }
}
