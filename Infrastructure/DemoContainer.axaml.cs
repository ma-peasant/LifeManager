using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

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