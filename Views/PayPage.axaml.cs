using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using LifeManager.Tables;
using LifeManager.ViewModels;
using System;
using System.Threading.Tasks;

namespace LifeManager;

public partial class PayPage : HeaderedContentControl
{
    public PayPage()
    {
        InitializeComponent();
        var viewModel = new PayViewModel();
        this.DataContext = viewModel;
        viewModel.BeginEditAction = (rowIndex) =>
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                dataGrid.ScrollIntoView(rowIndex, null); // 滚动到最后一行
                dataGrid.SelectedIndex = rowIndex;       // 选择最后一行
                dataGrid.BeginEdit();                        // 开始编辑
            });
        };
        this.Loaded += PayPage_Loaded;
    }

    private void PayPage_Loaded(object? sender, RoutedEventArgs e)
    {
        this.dataGrid.Focus();  //需要获取焦点才可以使用快捷键
    }

    private void OnTextBoxKeyDown(object? sender, KeyEventArgs e)
    {
        // 允许输入的按键：数字、退格、删除、小数点、方向键
        if (!(e.Key >= Key.D0 && e.Key <= Key.D9) &&   // 数字键 0-9
            !(e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) &&  // 小键盘 0-9
            e.Key != Key.Back && e.Key != Key.Delete && // 退格和删除
            e.Key != Key.Decimal && e.Key != Key.OemPeriod && // 小数点
            e.Key != Key.Left && e.Key != Key.Right) // 左右方向键
        {
            e.Handled = true; // 阻止其他按键输入
        }
    }

    private void OnTextBoxLostFocus(object? sender, RoutedEventArgs e)
    {
        var textBox = sender as TextBox;
        if (textBox != null && !double.TryParse(textBox.Text, out _))
        {
            textBox.Text = "0"; // 如果输入不合法，重置为0
        }
    }
}