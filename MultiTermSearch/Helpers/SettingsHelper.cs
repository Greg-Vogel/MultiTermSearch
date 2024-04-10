using MultiTermSearch.Classes;
using MultiTermSearch.Properties;

namespace MultiTermSearch.Helpers;

internal static class SettingsHelper
{
    public static MtsSettings CurrentSettings { get; private set; } = null!;

    public static void LoadSettings()
    {
        // Load last used settings and defaults from the user settings
        string settingString = Settings.Default.LastSettings;
        if (string.IsNullOrWhiteSpace(settingString))
        {
            CurrentSettings = new MtsSettings();
            return;
        }

        // deserialize setting string into its class for easier use
        try
        {
            var settings = System.Text.Json.JsonSerializer.Deserialize<MtsSettings>(settingString);
            CurrentSettings = settings ?? new MtsSettings();
            return;
        }
        catch
        {
            CurrentSettings = new MtsSettings();
            return;
        }
    }

    public static void SaveSettings(SearchInputs inputs)
    {
        CurrentSettings.MergeInputs(inputs);
        Settings.Default.LastSettings = System.Text.Json.JsonSerializer.Serialize(CurrentSettings);
        Settings.Default.Save();
    }
}
