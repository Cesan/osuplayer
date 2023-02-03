using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Timers;
using OsuPlayer.Base.ViewModels;
using OsuPlayer.Network.API.Service.Endpoints;
using ReactiveUI;
using Splat;

namespace OsuPlayer.Views;

public class StatisticsViewModel : BaseViewModel
{
    private float _mbUsed;
    
    private Timer? _timer;
    private TimeSpan _playerAgeTime;
    
    private string _playerAge = string.Empty;

    private ulong _users;
    private ulong _xpLeft;
    private ulong _xpEarned;
    private ulong _songsPlayed;
    private ulong _translators;
    private ulong _communityLevel;
    private ulong _beatmapsTracked;

    public ulong Users
    {
        get => _users;
        set => this.RaiseAndSetIfChanged(ref _users, value);
    }

    public ulong Translators
    {
        get => _translators;
        set => this.RaiseAndSetIfChanged(ref _translators, value);
    }

    public ulong SongsPlayed
    {
        get => _songsPlayed;
        set => this.RaiseAndSetIfChanged(ref _songsPlayed, value);
    }

    public ulong XpEarned
    {
        get => _xpEarned;
        set => this.RaiseAndSetIfChanged(ref _xpEarned, value);
    }

    public ulong CommunityLevel
    {
        get => _communityLevel;
        set => this.RaiseAndSetIfChanged(ref _communityLevel, value);
    }

    public ulong XpLeft
    {
        get => _xpLeft;
        set => this.RaiseAndSetIfChanged(ref _xpLeft, value);
    }

    public ulong BeatmapsTracked
    {
        get => _beatmapsTracked;
        set => this.RaiseAndSetIfChanged(ref _beatmapsTracked, value);
    }

    public float MbUsed
    {
        get => _mbUsed;
        set => this.RaiseAndSetIfChanged(ref _mbUsed, value);
    }

    public string PlayerAge
    {
        get
        {
            var years = (int) Math.Floor(_playerAgeTime.Days / 365D);
            var days = _playerAgeTime.Days % 365;
            var months = (int) Math.Floor(days / 30D);
            days %= 30;

            return $"{years} years, {months} months, {days} days, {_playerAgeTime.Hours}:{_playerAgeTime.Minutes}:{_playerAgeTime.Seconds} old";
        }
        set => this.RaiseAndSetIfChanged(ref _playerAge, value);
    }

    public StatisticsViewModel()
    {
        Activator = new ViewModelActivator();

        this.WhenActivated(Block);
    }

    private async void Block(CompositeDisposable disposable)
    {
        Disposable.Create(() => { _timer?.Close(); }).DisposeWith(disposable);

        // Done separately so they can fail independently
        var tasks = new[]
        {
            UpdateApiStatistics(), UpdateBeatmapCount(), UpdateStorageAmount()
        };

        await Task.WhenAll(tasks);

        UpdateDate();

        _timer = new Timer(1000);
        _timer.Elapsed += (_, _) => UpdateDate();
        _timer.Start();
    }

    private void UpdateDate()
    {
        _playerAgeTime = DateTime.Now.Subtract(new DateTime(2017, 11, 1));

        this.RaisePropertyChanged(nameof(PlayerAge));
    }

    private async Task UpdateApiStatistics()
    {
        var statistics = await Locator.Current.GetService<NorthFox>().GetApiStatistics();

        Users = statistics.TotalUsers;
        Translators = statistics.TotalTranslators;
        SongsPlayed = statistics.TotalSongsPlayed;
        XpEarned = statistics.TotalCommunityXp;
        CommunityLevel = statistics.CommunityLevel;
        XpLeft = statistics.CommunityXpLeft;
    }

    private async Task UpdateStorageAmount()
    {
        MbUsed = (float) await Locator.Current.GetService<NorthFox>().GetStorageAmount();
    }

    private async Task UpdateBeatmapCount()
    {
        BeatmapsTracked = (await Locator.Current.GetService<NorthFox>().GetApiStatistics()).TotalBeatmaps;
    }
}