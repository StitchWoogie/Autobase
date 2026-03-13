using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using System.ComponentModel;

namespace LauncherMain.Icon555;

public class Icon3D : ContentControl
{
    //public static readonly StyledProperty<IBrush?> BorderBrushProperty =
    //    AvaloniaProperty.Register<Icon3D, IBrush?>(nameof(BorderBrush));

    public static readonly StyledProperty<string?> STitleProperty =
    AvaloniaProperty.Register<Icon3D, string?>(nameof(STitle));

    //public IBrush? BorderBrush
    //{
    //    get => (IBrush?)GetValue(BorderBrushProperty);
    //    set => SetValue(BorderBrushProperty, value);
    //}

    
    public string? STitle
    {
        get => GetValue(STitleProperty);
        set
        {
            SetValue(STitleProperty, value);
        }
    }

   
}