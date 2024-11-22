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
using System.Threading.Tasks;

namespace LifeManager.ViewModels
{
    public partial class PayViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _payMoney;

        [ObservableProperty]
        private string? _incomeMoney;

        [ObservableProperty]
        private string? _allMoney;
        public PayViewModel()
        {
            // We can use this to add some items for the designer.
            // You can also use a DesignTime-ViewModel
            if (Design.IsDesignMode)
            {
            }
            Pays.CollectionChanged += Pays_CollectionChanged;
            LoadData();
            WeakReferenceMessenger.Default.Register<IsAllMessage>(this, (r, m) =>
            {
                LoadData();
            });
        }

        private void Pays_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CalculateAmounts();
        }

        private async void LoadData()
        {
            //从数据库读取数据并加载
            Pays.Clear();
            IEnumerable<PayItem> records = await DbHelpUtils.QueryRecordAsync<PayItem>();
            if (!Common.IsAll)
            {
                string format = "yyyy-MM-dd"; // 假设你的日期格式是 "yyyy-MM-dd"
                if (DateTime.TryParseExact(Common.SelectedDateTime, format, null, System.Globalization.DateTimeStyles.None, out DateTime selectedDateTime) &&
                    DateTime.TryParseExact(Common.SelectedDateTimeEnd, format, null, System.Globalization.DateTimeStyles.None, out DateTime selectedDateTimeEnd))
                {
                    records = records.Where(x =>
                        DateTime.TryParseExact(x.Date, format, null, System.Globalization.DateTimeStyles.None, out DateTime createDate) &&
                        createDate >= selectedDateTime &&
                        createDate <= selectedDateTimeEnd);
                }
                //records = records.Where(x => x.Date == Common.SelectedDateTime);
            }
            foreach (var record in records)
            {
                record.PropertyChanged += Item_PropertyChanged;
                Pays.Add(record);
            }
        }

        public ObservableCollection<PayItem> Pays { get; } = new ObservableCollection<PayItem>();

        // 定义一个委托，用于通知 View 执行 UI 操作
        public Action<int>? BeginEditAction { get; set; }

        [RelayCommand]
        private async Task AddData()
        {
            var newItem = new PayItem
            {
                Date = DateTime.Now.ToString("yyyy-MM-dd")  // 默认日期可以设置为当前时间
            };
            newItem.PropertyChanged += Item_PropertyChanged;
            // 添加新行到集合中
            Pays.Add(newItem);
            var lastRowIndex = Pays.Count - 1;
            BeginEditAction?.Invoke(Pays.Count - 1); // 通知 View 开始编辑新行
            await DbHelpUtils.AddRecordAsync<PayItem>(newItem);
            // 找到最后一行，并让用户开始编辑第一个单元格
        }

        private async void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PayItem.Amount) || e.PropertyName==nameof(PayItem.Type))
            {
                CalculateAmounts();
            }
            await DbHelpUtils.UpdateRecordAsync<PayItem>((PayItem)sender);
        }

        public void CalculateAmounts()
        {
            double sumTrue = Pays.Where(r => r.Type).Sum(m => m.Amount);
            double sumfalse = Pays.Where(r => !r.Type).Sum(m => m.Amount);
            PayMoney = sumfalse.ToString();
            IncomeMoney = sumTrue.ToString();
            AllMoney = (sumTrue - sumfalse).ToString();
        }

        [RelayCommand]
        private async Task DeleteItem(PayItem item)
        {
            // Remove the given item from the list
            Pays.Remove(item);
            item.PropertyChanged -= Item_PropertyChanged;
            await DbHelpUtils.DeleteRecordAsync<PayItem>(item);
        }
    }
}