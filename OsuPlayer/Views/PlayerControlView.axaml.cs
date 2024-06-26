using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Data.OsuPlayer.StorageModels;
using OsuPlayer.IO.Storage.Blacklist;
using OsuPlayer.IO.Storage.Playlists;
using OsuPlayer.Windows;
using ReactiveUI;

namespace OsuPlayer.Views;

public partial class PlayerControlView : ReactiveControl<PlayerControlViewModel>
{
    private FluentAppWindow? _mainWindow;

    public PlayerControlView()
    {
        InitializeComponent();

        this.WhenActivated(_ =>
        {
            if (this.GetVisualRoot() is FluentAppWindow mainWindow)
                _mainWindow = mainWindow;

            SongProgressSlider.AddHandler(PointerPressedEvent, SongProgressSlider_OnPointerPressed, RoutingStrategies.Tunnel);

            SongProgressSlider.AddHandler(PointerReleasedEvent, SongProgressSlider_OnPointerReleased, RoutingStrategies.Tunnel);

            Repeat.AddHandler(PointerReleasedEvent, Repeat_OnPointerReleased, RoutingStrategies.Tunnel);

            ViewModel.RaisePropertyChanged(nameof(ViewModel.IsAPlaylistSelected));
            ViewModel.RaisePropertyChanged(nameof(ViewModel.IsCurrentSongInPlaylist));
            ViewModel.RaisePropertyChanged(nameof(ViewModel.IsCurrentSongOnBlacklist));
        });
    }

    private void Repeat_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton == MouseButton.Right)
            ViewModel.RaisePropertyChanged(nameof(ViewModel.Playlists));
    }

    private void SongProgressSlider_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        ViewModel.Player.Play();
    }

    private void SongProgressSlider_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        ViewModel.Player.Pause();
    }

    internal async void Blacklist_OnClick(object? sender, RoutedEventArgs e)
    {
        await using (var blacklist = new Blacklist())
        {
            await blacklist.ReadAsync();

            var currentHash = ViewModel.CurrentSong.Value?.Hash;

            if (blacklist.Contains(ViewModel.CurrentSong.Value))
            {
                blacklist.Container.Songs.Remove(currentHash);
            }
            else
            {
                blacklist.Container.Songs.Add(currentHash);

                if (ViewModel.Player.BlacklistSkip.Value)
                    await ViewModel.Player.NextSong(PlayDirection.Forward);
            }
        }

        ViewModel.Player.TriggerBlacklistChanged(new PropertyChangedEventArgs("Black"));
        ViewModel.RaisePropertyChanged(nameof(ViewModel.IsCurrentSongOnBlacklist));
    }

    internal async void Favorite_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.Player.CurrentSong.Value != null)
        {
            await PlaylistManager.ToggleSongOfCurrentPlaylist(ViewModel.Player.SelectedPlaylist.Value, ViewModel.Player.CurrentSong.Value);
            ViewModel.Player.TriggerPlaylistChanged(new PropertyChangedEventArgs("Fav"));
        }

        ViewModel.RaisePropertyChanged(nameof(ViewModel.IsCurrentSongInPlaylist));
    }

    private void SongControl(object? sender, RoutedEventArgs e)
    {
        switch ((sender as Control)?.Name)
        {
            case "Repeat":
                ViewModel.Player.RepeatMode.Value = ViewModel.Player.RepeatMode.Value.Next();
                break;
            case "Previous":
                ViewModel.Player.NextSong(PlayDirection.Backwards);
                break;
            case "PlayPause":
                ViewModel.Player.PlayPause();
                break;
            case "Next":
                ViewModel.Player.NextSong(PlayDirection.Forward);
                break;
            case "Shuffle":
                ViewModel.IsShuffle = !ViewModel.IsShuffle;
                break;
        }
    }

    private void Volume_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.Player.ToggleMute();
    }

    private void PlaybackSpeed_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel!.PlaybackSpeed = 0;
    }

    private void RepeatContextMenu_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        ViewModel.Player.SelectedPlaylist.Value = (Playlist) (sender as ContextMenu)?.SelectedItem;
    }

    private void CurrentSongLabel_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_mainWindow?.ViewModel == default) return;

        var player = ViewModel.Player;

        if (player.RepeatMode.Value != RepeatMode.Playlist)
        {
            switch (_mainWindow.ViewModel.MainView)
            {
                case SearchViewModel:
                    _mainWindow.ViewModel.SearchView.SelectedSong = player.CurrentSong.Value;
                    _mainWindow.ViewModel.MainView = _mainWindow.ViewModel.SearchView;
                    break;
                default:
                    _mainWindow.ViewModel.HomeView.SelectedSong = player.CurrentSong.Value;
                    _mainWindow.ViewModel.MainView = _mainWindow.ViewModel.HomeView;
                    break;
            }
        }
        else
        {
            _mainWindow.ViewModel.PlaylistView.SelectedPlaylist = player.SelectedPlaylist.Value;
            _mainWindow.ViewModel.PlaylistView.SelectedSong = player.CurrentSong.Value;
            _mainWindow.ViewModel.MainView = _mainWindow.ViewModel.PlaylistView;
        }
    }

    private void Volume_OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (ViewModel == default) return;

        switch (e.Delta.Y)
        {
            // When we scroll up and exceed the maximum volume, we set it to the max possible volume
            case > 0 when ViewModel.Player.Volume.Value + 10 > ViewModel.MaximumVolume:
                ViewModel.Player.Volume.Value = ViewModel.MaximumVolume;

                return;
            // When we scroll up we add 10 (10% as the max volume should always be 100)
            case > 0:
                ViewModel.Player.Volume.Value += 10;
                break;
            // When we scroll up and exceed the maximum volume, we set it to 0, otherwise the slider would set it to max volume if we have a negative number
            case < 0 when ViewModel.Player.Volume.Value - 10 < 0:
                ViewModel.Player.Volume.Value = 0;

                return;
            // When we scroll up we subtract 10 (10% as the max volume should always be 100)
            case < 0:
                ViewModel.Player.Volume.Value -= 10;
                break;
        }
    }
}