using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LifeManager.Message;
using LifeManager.Tables;
using LifeManager.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace LifeManager.ViewModels
{
    public partial class ToDoListViewModel : ViewModelBase
    {
        public ToDoListViewModel()
        {
            // We can use this to add some items for the designer. 
            // You can also use a DesignTime-ViewModel
            if (Design.IsDesignMode)
            {
              
            }

            LoadData();
            WeakReferenceMessenger.Default.Register<IsAllMessage>(this, (r, m) =>
            {
                LoadData();
            });
        }

        private async void LoadData()
        {
            //从数据库读取数据并加载
            ToDoItems.Clear();
            IEnumerable<ToDoItem> records = await DbHelpUtils.QueryRecordAsync<ToDoItem>();

            if (!Common.IsAll)
            {
                records = records.Where(x => x.CreateDate == Common.SelectedDateTime);
            }
            foreach (var record in records)
            {
                record.PropertyChanged += Item_PropertyChanged;
                ToDoItems.Add(record);
            }


        }


        /// <summary>
        /// Gets a collection of <see cref="ToDoItem"/> which allows adding and removing items
        /// </summary>
        public ObservableCollection<ToDoItem> ToDoItems { get; } = new ObservableCollection<ToDoItem>();


        // -- Adding new Items --

        /// <summary>
        /// This command is used to add a new Item to the List
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanAddItem))]
        private async Task AddItem()
        {
            // Add a new item to the list
            ToDoItem item = new ToDoItem() { Content = NewItemContent,CreateDate = Common.SelectedDateTime };
            item.PropertyChanged += Item_PropertyChanged;
            ToDoItems.Add(item);
            // reset the NewItemContent
            NewItemContent = null;

            await DbHelpUtils.AddRecordAsync<ToDoItem>(item);
        }

        private async void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ToDoItem.IsChecked))
            {
                await DbHelpUtils.UpdateRecordAsync<ToDoItem>((ToDoItem)sender);
            }
        }

        /// <summary>
        /// Gets or set the content for new Items to add. If this string is not empty, the AddItemCommand will be enabled automatically
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddItemCommand))] // This attribute will invalidate the command each time this property changes
        private string? _newItemContent;

        /// <summary>
        /// Returns if a new Item can be added. We require to have the NewItem some Text
        /// </summary>
        private bool CanAddItem() => !string.IsNullOrWhiteSpace(NewItemContent);

        // -- Removing Items --

        /// <summary>
        /// Removes the given Item from the list
        /// </summary>
        /// <param name="item">the item to remove</param>
        [RelayCommand]
        private async Task RemoveItem(ToDoItem item)
        {
            // Remove the given item from the list
            ToDoItems.Remove(item);
            item.PropertyChanged -= Item_PropertyChanged;
            await DbHelpUtils.DeleteRecordAsync<ToDoItem>(item);
        }
    }
}
