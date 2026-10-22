using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLocalization
{
    [CreateAssetMenu(fileName = "LocalizationTable", menuName = "Localization/Text Table")]
    public class LocalizationTableSO : ScriptableObject
    {
        [Serializable]
        public class Translation
        {
            [Tooltip("Language code, e.g. en, ru")]
            public string language;
            [TextArea] public string text;
        }

        [Serializable]
        public class Entry
        {
            public string key;
            public List<Translation> translations = new List<Translation>();
        }

        public List<Entry> entries = new List<Entry>();

        public string Get(string key) => Get(key, Loc.Language);

        // Text in the requested language; otherwise in the default language; otherwise any; otherwise the key itself
        public string Get(string key, string language)
        {
            var entry = entries.Find(e => e != null && e.key == key);
            if (entry == null || entry.translations == null || entry.translations.Count == 0)
                return key;

            var translation = entry.translations.Find(t => t != null && t.language == language)
                              ?? entry.translations.Find(t => t != null && t.language == Loc.DefaultLanguage)
                              ?? entry.translations[0];
            return string.IsNullOrEmpty(translation.text) ? key : translation.text;
        }

#if UNITY_EDITOR
        [ContextMenu("Add order rejection texts (en, ru)")]
        private void AddOrderRejectTexts()
        {
            UnityEditor.Undo.RecordObject(this, "Add order rejection texts");

            AddIfMissing(Managers.OrderRejectReasonKeys.NeedsPlate, "You need a plate", "Нужна тарелка");
            AddIfMissing(Managers.OrderRejectReasonKeys.EmptyPlate, "The plate is empty", "Тарелка пустая");
            AddIfMissing(Managers.OrderRejectReasonKeys.NotFinished, "The dish isn't ready", "Блюдо не готово");
            AddIfMissing(Managers.OrderRejectReasonKeys.NoSuchOrder, "No such order", "Такого заказа нет");
            AddIfMissing(Managers.OrderRejectReasonKeys.LevelTooLow, "Ingredient level is too low", "Не тот уровень продуктов");

            UnityEditor.EditorUtility.SetDirty(this);
        }

        private void AddIfMissing(string key, string en, string ru)
        {
            if (entries.Exists(e => e != null && e.key == key)) return;

            entries.Add(new Entry
            {
                key = key,
                translations = new List<Translation>
                {
                    new Translation { language = "en", text = en },
                    new Translation { language = "ru", text = ru }
                }
            });
        }
#endif
    }
}
