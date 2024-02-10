using BmwWheelsScanner.Models;
using Microsoft.Maui.Controls;
using Prism.Commands;

namespace BmwWheelsScanner.CustomControls;

public partial class WheelTileView : ContentView
{
    public WheelTileView()
    {
        InitializeComponent();
    }

    public string WheelPicture
    {
        get => (string)GetValue(WheelPictureProperty);
        set => SetValue(WheelPictureProperty, value);
    }

    public string WheelStyleName
    {
        get => (string)GetValue(WheelStyleNameProperty);
        set => SetValue(WheelStyleNameProperty, value);
    }

    public static readonly BindableProperty WheelPictureProperty = BindableProperty.Create(nameof(WheelPicture), typeof(string),
    typeof(WheelTileView), string.Empty, BindingMode.TwoWay);

    public static readonly BindableProperty WheelStyleNameProperty = BindableProperty.Create(nameof(WheelStyleName), typeof(string),
    typeof(WheelTileView), string.Empty, BindingMode.TwoWay);

    public static readonly BindableProperty WheelTappedCommandProperty = BindableProperty.Create(nameof(WheelTappedCommand), typeof(DelegateCommand<Wheel>), typeof(WheelTileView), null);
    public DelegateCommand<Wheel> WheelTappedCommand
    {
        get => (DelegateCommand<Wheel>)GetValue(WheelTappedCommandProperty);
        set => SetValue(WheelTappedCommandProperty, value);
    }
}
