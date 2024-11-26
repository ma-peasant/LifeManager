using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using System;
using System.Linq;

namespace LifeManager.Infrastructure;

public partial class DemoContainer : UserControl
{
    public DemoContainer(ExampleDefinition exampleDefinition)
    {
        InitializeComponent();
        TitleElement.Text = exampleDefinition.Name;
        if (exampleDefinition.Control != null) {
            var demo = Activator.CreateInstance(exampleDefinition.Control);
            ContentControl.Content = demo;
        }

     

}

}