using Avalonia.Controls;
using LifeManager.Infrastructure;
using System.Text;
using System;
using LifeManager.ViewModels;
using System.ComponentModel;
using LifeManager.Utils;
using CommunityToolkit.Mvvm.Messaging;
using LifeManager.Message;

namespace LifeManager.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
          
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.DataContext = new MainWindowViewModel();
        }

        private void MainWindow_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.DemoItemsControl.SelectedIndex = 0;
            this.datePicker.SelectedDate = new DateTimeOffset(DateTime.Now) ;
        }

        private void ListBox_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
        {
            if (!(DemoItemsControl.SelectedItem is ExampleDefinition exampleDefinition))
            {
                return;
            }
            var container = new DemoContainer(exampleDefinition);
            DemoContentControl.Content = container;
        }

        private void DatePicker_SelectedDateChanged(object? sender, Avalonia.Controls.DatePickerSelectedValueChangedEventArgs e)
        {
            Common.SelectedDateTime = ((DateTimeOffset)e.NewDate).ToString("yyyy-MM-dd");
            WeakReferenceMessenger.Default.Send(new IsAllMessage()
            {
                IsAll = false
            });
        }
    }
}