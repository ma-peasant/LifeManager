using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LifeManager.Infrastructure;
using LifeManager.Message;
using LifeManager.Tables;
using LifeManager.Utils;
using System;

namespace LifeManager.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            InitializeDatabaseAsync();
            this.PropertyChanged += MainWindowViewModel_PropertyChanged;
        }

        private void MainWindowViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.IsAll))
            {
                Common.IsAll = this.IsAll;
                WeakReferenceMessenger.Default.Send(new IsAllMessage()
                {
                    IsAll = this.IsAll
                });
            }
        }

        public ExampleDefinition[] Definitions { get; } = new ExampleDefinition[]
        {
            new ExampleDefinition("待办", typeof(ToDoList)),
            new ExampleDefinition("笔记", typeof(Notes)),
            new ExampleDefinition("消费统计", typeof(PayPage))
        };

        [ObservableProperty]
        private bool _isAll = false;

        [ObservableProperty]
        private DateTimeOffset? _selectedDateTime ;


        private async void InitializeDatabaseAsync()
        {
            await DbHelpUtils.InitDbAsync();
        }
    }
}