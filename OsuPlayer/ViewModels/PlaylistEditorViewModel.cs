using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using OsuPlayer.IO.DbReader;
using OsuPlayer.IO.Playlists;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class PlaylistEditorViewModel : BaseViewModel, IActivatableViewModel
{
    private Playlist _currentSelectedPlaylist;
    private ObservableCollection<string> _playlist;
    private ObservableCollection<Playlist> _playlists;
    
    private List<MapEntry> _selectedSonglistItems;
    private List<MapEntry> _selectedPlaylistItems;

    public Playlist CurrentSelectedPlaylist
    {
        get => _currentSelectedPlaylist;
        set
        {
            _currentSelectedPlaylist = value;
            this.RaisePropertyChanged();
        }
    }

    public ObservableCollection<Playlist> Playlists
    {
        get => _playlists;
        set => this.RaiseAndSetIfChanged(ref _playlists, value);
    }

    public PlaylistEditorViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables =>
        {
            Disposable.Create(() => { }).DisposeWith(disposables);

            if (Playlists.Count > 0 && CurrentSelectedPlaylist == default)
            {
                CurrentSelectedPlaylist = Playlists[0];
            }
        });

        SelectedPlaylistItems = new();
        SelectedSonglistItems = new();
        
        Playlist = new();
    }

    public ViewModelActivator Activator { get; }

    public List<MapEntry> Songlist => Core.Instance.Player.SongSource!;

    public ObservableCollection<string> Playlist
    {
        get => _playlist;
        set => this.RaiseAndSetIfChanged(ref _playlist, value);
    }

    public List<MapEntry> SelectedSonglistItems
    {
        get => _selectedSonglistItems;
        set => this.RaiseAndSetIfChanged(ref _selectedSonglistItems, value);
    }

    public List<MapEntry> SelectedPlaylistItems
    {
        get => _selectedPlaylistItems;
        set => this.RaiseAndSetIfChanged(ref _selectedPlaylistItems, value);
    }
}