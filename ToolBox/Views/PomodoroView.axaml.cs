using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ToolBox.ViewModels;

namespace ToolBox.Views;

public partial class PomodoroView : UserControl
{
    public PomodoroView()
    {
        InitializeComponent();
        DataContext = new PomodoroViewModel();
    }
}