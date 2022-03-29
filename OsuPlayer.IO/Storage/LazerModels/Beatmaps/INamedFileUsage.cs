namespace OsuPlayer.IO.Storage.LazerModels.Beatmaps;

public interface INamedFileUsage
{
    /// <summary>
    /// The underlying file on disk.
    /// </summary>
    IFileInfo File { get; }

    /// <summary>
    /// The filename for this usage.
    /// </summary>
    string Filename { get; }
}