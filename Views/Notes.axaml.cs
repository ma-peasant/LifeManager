using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;
using LifeManager.Models;
using LifeManager.Utils;
using LifeManager.ViewModels;
using System.Linq;

namespace LifeManager;

public partial class Notes : HeaderedContentControl
{
    public Notes()
    {
        InitializeComponent();
        this.DataContext = new NotesViewModel();
        this.Loaded += Notes_Loaded;
    }

    private void Notes_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Lb_Notes.SelectedIndex = 0;
    }

    private void ListBox_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        if (!(Lb_Notes.SelectedItem is NoteBase noteBase))
        {
            return;
        }
        ((NotesViewModel)this.DataContext).SelectNote = noteBase;
        ((NotesViewModel)this.DataContext).Content = FileUtils.ReadTxtFile(noteBase.Path); ;
    }

    //修改名字后要重新读取文件信息

    private void TextBox_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            var viewModel = (NoteBase)((Control)sender).DataContext;
            viewModel.RenameCommand((sender as TextBox).Text.Trim()); // 触发重命名
            viewModel.IsEnabled = false; // 退出编辑模式
        }
    }

    private void TextBox_LostFocus_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
         var viewModel = (NoteBase)((Control)sender).DataContext;
        viewModel.RenameCommand((sender as TextBox).Text.Trim()); // 触发重命名
        viewModel.IsEnabled = false; // 退出编辑模式
    }

    private void Border_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        var viewModel = (NoteBase)((Control)sender).DataContext;
        viewModel.IsEnabled = true; // 启用编辑模式
                                    // 查找子控件 TextBox 并设置焦点
        var border = (Border)sender;
        var textBox = border.GetVisualDescendants().OfType<TextBox>().FirstOrDefault();
        if (textBox != null)
        {
            textBox.Focus(); // 将焦点设置到 TextBox
            textBox.SelectionStart = textBox.Text.Length; // 可选：将光标放在文本末尾
        }
    }
}