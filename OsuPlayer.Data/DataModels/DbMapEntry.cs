﻿using System.Diagnostics;
using OsuPlayer.Data.DataModels.Interfaces;

namespace OsuPlayer.Data.DataModels;

/// <summary>
/// a full beatmap entry with optionally used data
/// <remarks>only created on a <see cref="IMapEntryBase.ReadFullEntry" /> call</remarks>
/// </summary>
public class DbMapEntry : DbMapEntryBase, IMapEntry
{
    public string AudioFileName { get; init; } = string.Empty;
    public string FolderName { get; init; } = string.Empty;
    public string FolderPath { get; init; } = string.Empty;
    public string FullPath { get; init; } = string.Empty;

    public async Task<string?> FindBackground()
    {
        var eventCount = 0;

        if (string.IsNullOrWhiteSpace(FolderPath))
            return null;

        string[] files;

        try
        {
            files = Directory.GetFiles(FolderPath, "*.osu");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);

            return null;
        }

        if (files.Length == 0)
            return null;
        if (files[0].Length > 260)
            return null;

        var content = (await File.ReadAllLinesAsync(files[0])).ToArray();

        foreach (var s in content)
        {
            if (s.Equals("[Events]")) break;

            eventCount++;
        }

        var background = string.Empty;

        if (content.Length == 0)
            return null;

        for (var e = 1; e < 6; e++)
            if (content[eventCount + e].ToLower().Contains(".jpg") ||
                content[eventCount + e].ToLower().Contains(".png"))
            {
                background = content[eventCount + e];
                break;
            }

        if (string.IsNullOrWhiteSpace(background))
            return null;

        var fileName = background.Split(',')[2].Replace("\"", string.Empty);
        var path = Path.Combine(FolderPath, fileName);

        return File.Exists(path) ? path : null;
    }
}