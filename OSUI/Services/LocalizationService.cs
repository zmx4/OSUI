using System.Globalization;
using System.Windows;
using HandyControl.Tools;

namespace OSUI.Services;

public sealed class LocalizationService
{
    private const string DictionaryPrefix = "pack://application:,,,/OSUI;component/Resources/Localization/Strings.";
    private const string DictionarySuffix = ".xaml";

    public const string ChineseLanguageCode = "zh-CN";
    public const string EnglishLanguageCode = "en-US";

    private static readonly Dictionary<string, string> SupportedLanguageCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        [ChineseLanguageCode] = ChineseLanguageCode,
        [EnglishLanguageCode] = EnglishLanguageCode
    };

    public static LocalizationService Instance { get; } = new();

    public string CurrentLanguageCode { get; private set; } = ChineseLanguageCode;

    private LocalizationService()
    {
    }

    public IReadOnlyList<string> GetSupportedLanguageCodes()
    {
        return [ChineseLanguageCode, EnglishLanguageCode];
    }

    public string NormalizeLanguageCode(string? languageCode)
    {
        return languageCode is not null && SupportedLanguageCodes.TryGetValue(languageCode, out var normalized)
            ? normalized
            : ChineseLanguageCode;
    }

    public void ApplyLanguage(string? languageCode)
    {
        var normalizedCode = NormalizeLanguageCode(languageCode);
        if (Application.Current is null)
        {
            CurrentLanguageCode = normalizedCode;
            return;
        }

        var resourceDictionary = CreateLanguageDictionary(normalizedCode);
        var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
        var existingDictionary = mergedDictionaries.FirstOrDefault(IsLanguageDictionary);

        if (existingDictionary is not null)
        {
            mergedDictionaries.Remove(existingDictionary);
        }

        mergedDictionaries.Add(resourceDictionary);

        CurrentLanguageCode = normalizedCode;
        ConfigHelper.Instance.SetLang(normalizedCode.ToLowerInvariant());

        var culture = CultureInfo.GetCultureInfo(normalizedCode);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }

    public string GetString(string key)
    {
        if (Application.Current is null)
        {
            return key;
        }

        var value = Application.Current.TryFindResource(key);
        return value as string ?? key;
    }

    public string Format(string key, params object[] args)
    {
        return string.Format(CultureInfo.CurrentCulture, GetString(key), args);
    }

    private static bool IsLanguageDictionary(ResourceDictionary dictionary)
    {
        var source = dictionary.Source?.OriginalString;
        return source is not null
               && source.StartsWith(DictionaryPrefix, StringComparison.OrdinalIgnoreCase)
               && source.EndsWith(DictionarySuffix, StringComparison.OrdinalIgnoreCase);
    }

    private static ResourceDictionary CreateLanguageDictionary(string languageCode)
    {
        return new ResourceDictionary
        {
            Source = new Uri($"{DictionaryPrefix}{languageCode}{DictionarySuffix}", UriKind.Absolute)
        };
    }
}
