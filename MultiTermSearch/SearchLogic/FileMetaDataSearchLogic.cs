using MultiTermSearch.Classes;
using MultiTermSearch.Helpers;

namespace MultiTermSearch.SearchLogic;

internal static class FileMetaDataSearchLogic
{
    private readonly static string[] _binaryFileTypes = [".exe", ".dll", ".pdb", ".png", ".jpeg", ".jpg", ".bmp", ".ico", ".zip", ".7z", ".vsidx", ".xls", ".xlsx", ".doc", ".docx", ".dbml", ".ttf", ".eot", ".woff", ".svg"];

    /// <summary>
    /// Checks if we should exclude the current file being searched based on the directory it is in
    ///    We know some folders are massive and pointless to search for our needs
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static bool ExcludeDirectory(ref string filePath)
    {
        if (filePath.Contains(@"\.git\") || filePath.Contains(@"\node_modules\") || filePath.Contains(@"\packages\"))
            return true;

        return false;
    }

    /// <summary>
    /// Checks if a file should be included based on the defined Include File Types in the SearchInputs
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="inputs"></param>
    /// <returns></returns>
    public static bool IsValidFileType(ref SearchInputs inputs, ref string filePath)
    {
        // If the file type list included the wildcard, then we need to check all file types for the search vals
        if (inputs.ScanAllFileTypes)
            return true;

        // We do not have the wild card, so we need to see if any of the Include File Types defined are the filetype at the end of the filepath
        return inputs.IncludeTypes.Any(filePath.EndsWith);
    }


    /// <summary>
    /// Checks if the file is a binary like file type that does not contain human readable text
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
    public static bool SkipBinaryFile(ref SearchInputs inputs, ref string filePath)
    {
        if (!inputs.Options_ExcludeBinaries)
            return false;

        foreach (var fileType in _binaryFileTypes)
        {
            if (filePath.EndsWith(fileType, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Scans the files name to see if it meets any of the compiled Regex searches.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    public static void ScanFileName(ref FileResult result, ref SearchInputs inputs, CancellationToken cancelToken)
    {
        var fullPath = inputs.Path.AsSpan();
        var fileName = fullPath.Slice(fullPath.LastIndexOf('\\') + 1);

        LineResult? fileNameLineResult = null;
        foreach (var regex in RegexHelper.CompiledRegex)
        {
            if (cancelToken.IsCancellationRequested)
                return;

            // get the matches in this fileName for this search term
            var matches = regex.EnumerateMatches(fileName);
            List<int> matchIndexes = new List<int>();
            foreach (var match in matches)
            {
                matchIndexes.Add(match.Index);
            }
            
            // if we got any match indexes, add them to the result
            if (matchIndexes.Any())
            {
                if (fileNameLineResult is null)
                    fileNameLineResult = new LineResult();
                fileNameLineResult.AddTermResults(regex.SearchTerm, matchIndexes);
            }
        }

        // If we did not get a matching file name... just stop here
        if (fileNameLineResult == null)
            return;

        // if the user request a line to have all terms in it... make sure the found terms match the searched terms
        //   if not... clear out anything we found so far and return
        if (inputs.Filters_LineContainsAll && fileNameLineResult.FoundTerms.Count != inputs.SearchTerms.Count())
        {
            fileNameLineResult = null;
            return;
        }

        // If we got here... then we have a file name that meets our search criteria
        //    Add the filename as if it were a line result in the file so the display has something to count/show
        result.AddLineResult(fileNameLineResult);
    }
}
