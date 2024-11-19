using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using LifeManager.ViewModels;

namespace LifeManager;

public partial class ToDoList : HeaderedContentControl
{
    public ToDoList()
    {
        InitializeComponent();
        this.DataContext = new ToDoListViewModel();
    }
}