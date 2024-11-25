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
                dataGrid.ScrollIntoView(rowIndex, null); // ���������һ��
                dataGrid.SelectedIndex = rowIndex;       // ѡ�����һ��
                dataGrid.BeginEdit();                        // ��ʼ�༭
            });
        };
        this.Loaded += PayPage_Loaded;
    }

    private void PayPage_Loaded(object? sender, RoutedEventArgs e)
    {
        this.dataGrid.Focus();  //��Ҫ��ȡ����ſ���ʹ�ÿ�ݼ�
    }

    private void OnTextBoxKeyDown(object? sender, KeyEventArgs e)
    {
        // ��������İ��������֡��˸�ɾ����С���㡢�����
        if (!(e.Key >= Key.D0 && e.Key <= Key.D9) &&   // ���ּ� 0-9
            !(e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) &&  // С���� 0-9
            e.Key != Key.Back && e.Key != Key.Delete && // �˸��ɾ��
            e.Key != Key.Decimal && e.Key != Key.OemPeriod && // С����
            e.Key != Key.Left && e.Key != Key.Right) // ���ҷ����
        {
            e.Handled = true; // ��ֹ������������
        }
    }

    private void OnTextBoxLostFocus(object? sender, RoutedEventArgs e)
    {
        var textBox = sender as TextBox;
        if (textBox != null && !double.TryParse(textBox.Text, out _))
        {
            textBox.Text = "0"; // ������벻�Ϸ�������Ϊ0
        }
    }
}