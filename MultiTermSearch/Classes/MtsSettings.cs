﻿namespace MultiTermSearch.Classes;

internal class MtsSettings
{
    public string Path { get; set; } = string.Empty;
    public bool IncludeSubDir { get; set; } = false;
    public bool Options_MatchWholeWord { get; set; } = false;
    public bool Options_IgnoreCase { get; set; } = false;
    public bool Options_ExcludeLargeDirectories { get; set; } = false;

    public bool Filters_LineContainsAll { get; set; } = false;
    public bool Filters_FileContainsAll { get; set; } = false;

    public SearchInputs.ESearchTargets Target { get; set; } = SearchInputs.ESearchTargets.Both;
    public string[] IncludeTypes { get; set; } = { ".txt" };
    public string[] SavedFileTypes { get; set; } = { ".txt", ".cs", ".vb", ".ts", ".js", ".yaml", ".html", ".cshtml", ".htmx", ".sln", ".csproj", ".vbproj", ".sql" };

    public int MaxThreadCount { get; set; } = 1;

    public void MergeInputs(SearchInputs inputs)
    {
        Path = inputs.Path;
        IncludeSubDir = inputs.IncludeSubDir;
        Options_IgnoreCase = inputs.Options_IgnoreCase;
        Options_MatchWholeWord = inputs.Options_MatchWholeWord;
        Options_ExcludeLargeDirectories = inputs.Options_ExcludeLargeDirectories;
        Filters_FileContainsAll = inputs.Filters_FileContainsAll;
        Filters_LineContainsAll = inputs.Filters_LineContainsAll;
        Target = inputs.Target;
        IncludeTypes = inputs.IncludeTypes;
        MaxThreadCount = inputs.SearcherThreadCount;
    }
}
