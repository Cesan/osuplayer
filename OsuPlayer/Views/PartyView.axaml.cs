﻿using Avalonia.Controls;
using Avalonia.Interactivity;
using Material.Icons;
using Material.Icons.Avalonia;
using Nein.Extensions;

namespace OsuPlayer.Views;

public partial class PartyView : UserControl
{
    public PartyView()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        GeneralExtensions.OpenUrl("https://github.com/osu-player/osuplayer/issues/70");
    }
}