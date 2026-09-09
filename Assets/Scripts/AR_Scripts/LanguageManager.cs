using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;
public class LanguageManager : MonoBehaviour
{
    public TMP_Dropdown languageDropdown; // Assign in Inspector

    void Start()
    {
        // Populate dropdown options
        languageDropdown.options.Clear();
        languageDropdown.options.Add(new TMP_Dropdown.OptionData("English"));
        languageDropdown.options.Add(new TMP_Dropdown.OptionData("Hindi"));

        // Set default language (English)
        SetLanguage("en");
        languageDropdown.value = 0;

        // Add listener for dropdown value change
        languageDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    void OnDropdownValueChanged(int index)
    {
        switch (index)
        {
            case 0:
                SetLanguage("en");
                break;
            case 1:
                SetLanguage("hi-IN");
                break;
        }
    }

    void SetLanguage(string languageCode)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales.Find(
            locale => locale.Identifier.Code == languageCode
        );
    }
}
