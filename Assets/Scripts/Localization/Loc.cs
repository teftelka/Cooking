using System;
using UnityEngine;

namespace GameLocalization
{
    // Current game language. Texts are looked up by key in a LocalizationTableSO.
    // If the project later moves to the Unity Localization package, only the lookup changes, not the UI.
    public static class Loc
    {
        public const string DefaultLanguage = "en";

        private static string language;

        public static event Action OnLanguageChanged;

        public static string Language
        {
            get
            {
                if (string.IsNullOrEmpty(language))
                    language = DetectSystemLanguage();
                return language;
            }
        }

        public static void SetLanguage(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode) || languageCode == language) return;
            language = languageCode;
            OnLanguageChanged?.Invoke();
        }

        private static string DetectSystemLanguage()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Russian: return "ru";
                default: return DefaultLanguage;
            }
        }
    }
}
