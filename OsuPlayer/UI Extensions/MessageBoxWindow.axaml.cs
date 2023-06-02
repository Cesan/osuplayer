﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace OsuPlayer.UI_Extensions;

public partial class MessageBoxWindow : ReactiveWindow<MessageBoxViewModel>
{
    public MessageBoxWindow()
    {
        InitializeComponent();

        ViewModel = new MessageBoxViewModel();

        var config = new Config();
        TransparencyLevelHint = (WindowTransparencyLevel) config.Container.BackgroundMode;

#if DEBUG
        this.AttachDevTools();
#endif
    }

    public MessageBoxWindow(string text, string? title)
    {
        InitializeComponent();

        ViewModel = new MessageBoxViewModel(this, text, title);

        var config = new Config();
        TransparencyLevelHint = (WindowTransparencyLevel) config.Container.BackgroundMode;
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        this.WhenActivated(_ => { });

        AvaloniaXamlLoader.Load(this);
    }
}