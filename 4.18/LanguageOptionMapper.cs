using System.Collections.Generic;

namespace _4._18
{
    internal static class LanguageOptionMapper
    {
        private static readonly (Language Language, string DisplayName)[] LanguageOptions =
        {
            (Language.English, "English"),
            (Language.SimplifiedChinese, "简体中文"),
            (Language.TraditionalChinese, "繁體中文"),
            (Language.Spanish, "Español"),
            (Language.French, "Français"),
            (Language.Portuguese, "Português"),
            (Language.Russian, "Русский"),
            (Language.Persian, "فارسی"),
            (Language.Norwegian, "Norsk"),
            (Language.Japanese, "日本語"),
            (Language.Korean, "한국어"),
            (Language.Arabic, "العربية")
        };

        public static IEnumerable<string> GetDisplayNames()
        {
            foreach (var option in LanguageOptions)
            {
                yield return option.DisplayName;
            }
        }

        public static int GetIndex(Language language)
        {
            for (int i = 0; i < LanguageOptions.Length; i++)
            {
                if (LanguageOptions[i].Language == language)
                {
                    return i;
                }
            }
            return 0;
        }

        public static Language GetLanguageByIndex(int index)
        {
            if (index < 0 || index >= LanguageOptions.Length)
            {
                return Language.English;
            }
            return LanguageOptions[index].Language;
        }
    }
}
