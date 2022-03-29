namespace OsuPlayer.IO.Storage.LazerModels.Beatmaps;

public interface IHasRealmFiles
{
    IList<RealmNamedFileUsage> Files { get; }

    string Hash { get; set; }
}