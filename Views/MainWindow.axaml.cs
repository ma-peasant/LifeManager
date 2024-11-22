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
            this.datePickerEnd.SelectedDate  =  this.datePicker.SelectedDate = new DateTimeOffset(DateTime.Now) ;
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
            //不能超过今天

            string format = "yyyy-MM-dd";
            DateTime now = DateTime.Now.Date;   //要取到日期， 不然带时间会导致日期相等的报错
            string selectedDateTime = ((DateTimeOffset)e.NewDate).ToString("yyyy-MM-dd");

            if (DateTime.TryParseExact(selectedDateTime, format, null, System.Globalization.DateTimeStyles.None, out DateTime startTime))
            {
                if (startTime <= now)
                {
                    Common.SelectedDateTime = selectedDateTime;
                }
                else
                {
                    this.datePicker.SelectedDate = new DateTimeOffset(now);
                }
            }

            WeakReferenceMessenger.Default.Send(new IsAllMessage()
            {
                IsAll = false
            });


        }
        private void DatePickerEnd_SelectedDateChanged(object? sender, Avalonia.Controls.DatePickerSelectedValueChangedEventArgs e)
        {
            //不能低于今天
            string format = "yyyy-MM-dd";
            DateTime now = DateTime.Now.Date;   //要取到日期， 不然带时间会导致日期相等的报错
            string selectedDateTimeEnd = ((DateTimeOffset)e.NewDate).ToString("yyyy-MM-dd");

            if (DateTime.TryParseExact(selectedDateTimeEnd, format, null, System.Globalization.DateTimeStyles.None, out DateTime endTime) )
            {
                if (now <= endTime)
                {
                    Common.SelectedDateTimeEnd = selectedDateTimeEnd;

                }
                else {
                    this.datePickerEnd.SelectedDate = new DateTimeOffset(now);
                }
            }

            WeakReferenceMessenger.Default.Send(new IsAllMessage()
            {
                IsAll = false
            });
        }
    }
}