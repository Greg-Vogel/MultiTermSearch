using MultiTermSearch.Classes;
using MultiTermSearch.Helpers;

namespace MultiTermSearch.Logic;

internal class FileMetaDataSearchLogic
{
    /// <summary>
    /// Checks if we should exclude the current file being searched based on the directory it is in
    ///    We know some folders are massive and pointless to search for our needs
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="inputs"></param>
    /// <returns></returns>
    public static bool ExcludeDirectory(string filePath, SearchInputs inputs)
    {
        if (!inputs.Options_ExcludeLargeDirectories)
            return false;

        if (filePath.Contains(@"\.git\") || filePath.Contains(@"\node_modules\"))
            return true;

        return false;
    }

    /// <summary>
    /// Checks if a file should be included based on the defined Include File Types in the SearchInputs
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="inputs"></param>
    /// <returns></returns>
    public static bool IsValidFileType(string filePath, SearchInputs inputs)
    {
        // If the file type list included the wildcard, then we need to check all file types for the search vals
        if (inputs.ScanAllFileTypes)
            return true;

        // We do not have the wild card, so we need to see if any of the Include File Types defined are the filetype at the end of the filepath
        return inputs.IncludeTypes.Any(filePath.EndsWith);
    }

    /// <summary>
    /// Scans the files name to see if it meets any of the compiled Regex searches.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    public static FileResult? ScanFileName(string filePath, CancellationToken cancelToken)
    {
        string fileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);

        LineResult? lineResult = null;
        foreach (var regex in RegexHelper.CompiledRegex)
        {
            if (cancelToken.IsCancellationRequested)
                return null;

            // get the matches in this fileName for this search term
            var matches = regex.Matches(fileName);

            // if we didnt find any matches for this search term, move on
            if (!matches.Any())
                continue;

            // if this is the first matching term for this fileName, create a new result obj for it
            if (lineResult == null)
                lineResult = new LineResult() { LineNumber = 0, Line = fileName };

            // Add the matches for this term to the line
            lineResult.AddTermResult(new TermResult()
            {
                Term = regex.SearchTerm,
                IndexOfMatches = matches.Select(m => m.Index).ToArray()
            });
        }

        if (lineResult != null)
        {
            FileResult? result = new FileResult(filePath);
            result.AddLineResult(lineResult);
            return result;
        }

        return null;
    }
}
