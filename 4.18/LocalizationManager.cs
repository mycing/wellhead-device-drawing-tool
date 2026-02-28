using System;
using System.Collections.Generic;
using System.IO;

namespace _4._18
{
    /// <summary>
    /// 語言枚舉
    /// </summary>
    public enum Language
    {
        English,
        SimplifiedChinese,
        TraditionalChinese,
        Spanish,
        French,
        Portuguese,
        Russian,
        Persian,
        Norwegian,
        Japanese,
        Korean,
        Arabic
    }

    /// <summary>
    /// 本地化管理器 - 管理多語言字符串
    /// </summary>
    public static class LocalizationManager
    {
        private static Language _currentLanguage = Language.English;
        private static readonly string SettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "language.conf");

        /// <summary>
        /// 當前語言是否為 RTL（從右到左）
        /// </summary>
        public static bool IsRtl => _currentLanguage == Language.Arabic || _currentLanguage == Language.Persian;

        // ===== 內置裝置名稱（按 index 順序，共 23 個） =====
        private static readonly string[][] DeviceNames = new string[][]
        {
            //             English,                  简体中文,            繁體中文,            Spanish,                          French,                               Portuguese,                        Russian,                               Persian,                     Norwegian,                     Arabic
            new[] { "Rotary Table",               "转盘面",            "轉盤面",            "Mesa rotatoria",                 "Table rotative",                   "Mesa rotativa",                  "Роторный стол",                   "میز دوّار",                "Rotasjonsbord",            "طاولة دوارة" },             // 0
            new[] { "Bell Nipple",                "喇叭口",            "喇叭口",            "Campana",                        "Col de cloche",                    "Campânula",                      "Колокол",                        "ناقوس",                    "Klokke",                   "حلمة الجرس" },              // 1
            new[] { "Packing Gland Riser Sub",    "密封盘根升高短节",  "密封盤根升高短節",   "Sub de elevación con prensaestopas", "Raccord de rehausse à presse-étoupe", "Sub de elevação com prensa-gaxeta", "Удлинительная муфта с сальником", "زیرِ بالابر با گلند پکینگ", "Stigerørforlenger med pakkboks", "وصلة رافعة حشو محكم" },     // 2
            new[] { "MPD Rotating Control Head",  "精细控压旋转控制头", "精細控壓旋轉控制頭", "Cabeza de control rotatorio MPD", "Tête de contrôle rotative MPD",    "Cabeça de controle rotativa MPD","Вращающаяся головка управления MPD","هد کنترل دورانی MPD",      "MPD roterende kontrollhode","رأس التحكم الدوار" },        // 3
            new[] { "Temporary Wellhead",         "临时井口头",        "臨時井口頭",         "Cabeza de pozo temporal",         "Tête de puits temporaire",          "Cabeça de poço temporária",      "Временная устьевая головка",      "سرچاه موقت",               "Midlertidig brønnhode",     "رأس البئر المؤقت" },        // 4
            new[] { "Annular BOP",                "环形防喷器",        "環形防噴器",         "BOP anular",                      "BOP annulaire",                     "BOP anular",                     "Кольцевой превентор",             "BOP حلقوی",                "Ringformet BOP",            "مانع الانفجار الحلقي" },     // 5
            new[] { "Ram BOP",                    "闸板防喷器",        "閘板防噴器",         "BOP de arietes",                  "BOP à vérins",                      "BOP de gavetas",                 "Плашечный превентор",             "BOP رم",                   "Ram-BOP",                  "مانع الانفجار الكبشي" },     // 6
            new[] { "Double Ram BOP",             "双闸板防喷器",      "雙閘板防噴器",       "BOP de doble ariete",             "BOP à double vérin",                "BOP de dupla gaveta",            "Двухплашечный превентор",         "BOP دو رم",                "Dobbel ram-BOP",            "مانع انفجار كبشي مزدوج" },   // 7
            new[] { "Drilling Spool",             "钻井四通",          "鑽井四通",           "Cruceta de perforación",          "Té de forage",                      "Cruzeta de perfuração",          "Буровой крестовик",               "چهارراهی حفاری",           "Bore-spool",               "بكرة الحفر" },              // 8
            new[] { "Tubing Spool",               "油管四通",          "油管四通",           "Cruceta de tubing",               "Té de tubing",                      "Cruzeta de produção",            "НКТ-крестовик",                   "چهارراهی لوله مغزی",       "Tubing-spool",             "بكرة الأنابيب" },            // 9
            new[] { "Casing Spool",               "套管四通",          "套管四通",           "Cruceta de revestimiento",        "Té de tubage",                      "Cruzeta de revestimento",        "Обсадной крестовик",              "چهارراهی لوله جداری",       "Casing-spool",             "بكرة التغليف" },             // 10
            new[] { "Reducer Flange",             "变径法兰",          "變徑法蘭",           "Brida reductora",                 "Bride réductrice",                  "Flange redutor",                 "Переходной фланец",               "فلنج کاهنده",              "Reduksjonsflens",          "شفة تخفيض" },               // 11
            new[] { "Adapter Flange",             "变压法兰",          "變壓法蘭",           "Brida adaptadora",                "Bride d’adaptation",                "Flange adaptador",               "Адаптерный фланец",               "فلنج تطبیق",               "Adapterflens",             "شفة تكييف" },               // 12
            new[] { "Riser",                      "升高立管",          "升高立管",           "Riser",                           "Riser",                              "Riser",                          "Райзер",                         "رایزر",                    "Riser",                    "الناهض" },                  // 13
            new[] { "Casing Head",                "套管头",            "套管頭",             "Cabeza de revestimiento",         "Tête de tubage",                     "Cabeça de revestimento",         "Обсадная головка",               "سرجداری",                  "Casing-head",              "رأس التغليف" },             // 14
            new[] { "Wellhead Platform",          "井口平台",          "井口平台",           "Plataforma de cabeza de pozo",    "Plateforme de tête de puits",       "Plataforma de cabeça de poço",   "Платформа устья",                "سکوی سرچاه",               "Brønnhodeplattform",       "منصة رأس البئر" },          // 15
            new[] { "Diverter",                   "分流器",            "分流器",             "Desviador",                       "Déviateur",                          "Desviador",                      "Дивертор",                       "دیورتر",                   "Diverter",                 "محوّل" },                   // 16
            new[] { "Single Casing",              "单层套管",          "單層套管",           "Revestimiento simple",            "Tubage simple",                      "Revestimento simples",           "Одноколонная обсадка",           "جداری تک‌لایه",            "Enkelt casing",            "تغليف أحادي" },             // 17
            new[] { "Double Casing",              "双层套管",          "雙層套管",           "Revestimiento doble",             "Tubage double",                      "Revestimento duplo",             "Двухколонная обсадка",           "جداری دولایه",             "Dobbelt casing",           "تغليف مزدوج" },             // 18
            new[] { "Triple Casing",              "三层套管",          "三層套管",           "Revestimiento triple",            "Tubage triple",                      "Revestimento triplo",            "Трёхколонная обсадка",           "جداری سه‌لایه",            "Trippel casing",           "تغليف ثلاثي" },             // 19
            new[] { "Single Bore Double Well",    "单筒双井",          "單筒雙井",           "Pozo doble de un solo conducto",  "Double puits à alésage unique",       "Poço duplo de furo único",       "Двухскважинная одноствольная",   "دوچاه تک‌مجرایی",           "Dobbel brønn, enkelt boreløp","بئر مزدوج أحادي الفتحة" },   // 20
            new[] { "Marine Conductor",           "隔水导管",          "隔水導管",           "Conductor marino",                "Conduite marine",                    "Condutor marinho",               "Морской кондуктор",              "هادی دریایی",              "Sjø-conductor",            "موصل بحري" },               // 21
            new[] { "Choke & Kill Manifold",      "节流压井管汇",      "節流壓井管匯",       "Colector de estrangulación y kill","Collecteur choke & kill",            "Manifold de choke e kill",       "Коллектор глушения и дросселирования","منیفولد چوک و کیل",        "Choke- og kill-manifold", "مشعب الخنق والقتل" },        // 22
        };

        private static readonly string[] DeviceNamesJapanese =
        {
            "回転テーブル",
            "ベルニップル",
            "パッキンググランドライザーサブ",
            "MPD回転制御ヘッド",
            "仮設ウェルヘッド",
            "アニュラBOP",
            "ラムBOP",
            "ダブルラムBOP",
            "掘削スプール",
            "チュービングスプール",
            "ケーシングスプール",
            "異径フランジ",
            "アダプタフランジ",
            "ライザー",
            "ケーシングヘッド",
            "ウェルヘッドプラットフォーム",
            "ダイバータ",
            "単層ケーシング",
            "二層ケーシング",
            "三層ケーシング",
            "単孔二井",
            "マリンコンダクタ",
            "チョーク・キルマニホールド"
        };

        private static readonly string[] DeviceNamesKorean =
        {
            "로터리 테이블",
            "벨 니플",
            "패킹 글랜드 라이저 서브",
            "MPD 회전 제어 헤드",
            "임시 웰헤드",
            "환형 BOP",
            "램 BOP",
            "더블 램 BOP",
            "드릴링 스풀",
            "튜빙 스풀",
            "케이싱 스풀",
            "리듀서 플랜지",
            "어댑터 플랜지",
            "라이저",
            "케이싱 헤드",
            "웰헤드 플랫폼",
            "다이버터",
            "단층 케이싱",
            "이중 케이싱",
            "삼중 케이싱",
            "단일 보어 이중정",
            "해양 컨덕터",
            "초크 앤 킬 매니폴드"
        };

        private static readonly Dictionary<string, Dictionary<Language, string>> Strings = new Dictionary<string, Dictionary<Language, string>>
        {
            // ===== 主窗體 =====
            ["FormTitle"] = new Dictionary<Language, string>
            {
                { Language.English, "Wellhead Device Drawing Tool" },
                { Language.SimplifiedChinese, "井口装置绘图工具" },
                { Language.TraditionalChinese, "井口裝置繪圖工具" },
                { Language.Spanish, "Herramienta de dibujo de equipos de cabeza de pozo" },
                { Language.French, "Outil de dessin d’équipements de tête de puits" },
                { Language.Portuguese, "Ferramenta de desenho de equipamentos de cabeça de poço" },
                { Language.Russian, "Инструмент для чертежа устьевого оборудования" },
                { Language.Persian, "ابزار ترسیم تجهیزات سرچاه" },
                { Language.Norwegian, "Tegneverktøy for brønnhodeutstyr" },
                { Language.Arabic, "أداة رسم معدات رأس البئر" }
            },
            ["DeviceSelection"] = new Dictionary<Language, string>
            {
                { Language.English, "Device Selection" },
                { Language.SimplifiedChinese, "井口装置选择" },
                { Language.TraditionalChinese, "井口裝置選擇" },
                { Language.Spanish, "Selección de equipos" },
                { Language.French, "Sélection des équipements" },
                { Language.Portuguese, "Seleção de equipamentos" },
                { Language.Russian, "Выбор оборудования" },
                { Language.Persian, "انتخاب تجهیزات" },
                { Language.Norwegian, "Valg av utstyr" },
                { Language.Arabic, "اختيار المعدات" }
            },
            ["TagManagement"] = new Dictionary<Language, string>
            {
                { Language.English, "Label Manager" },
                { Language.SimplifiedChinese, "标签管理" },
                { Language.TraditionalChinese, "標籤管理" },
                { Language.Spanish, "Gestor de etiquetas" },
                { Language.French, "Gestionnaire d’étiquettes" },
                { Language.Portuguese, "Gerenciador de etiquetas" },
                { Language.Russian, "Менеджер меток" },
                { Language.Persian, "مدیریت برچسب‌ها" },
                { Language.Norwegian, "Etikettbehandling" },
                { Language.Arabic, "إدارة التسميات" }
            },
            ["BOPConfig"] = new Dictionary<Language, string>
            {
                { Language.English, "BOP Configuration" },
                { Language.SimplifiedChinese, "防喷器配置" },
                { Language.TraditionalChinese, "防噴器配置" },
                { Language.Spanish, "Configuración del BOP" },
                { Language.French, "Configuration du BOP" },
                { Language.Portuguese, "Configuração do BOP" },
                { Language.Russian, "Конфигурация ПВО" },
                { Language.Persian, "پیکربندی BOP" },
                { Language.Norwegian, "BOP-konfigurasjon" },
                { Language.Arabic, "تكوين مانع الانفجار" }
            },
            ["SavedTemplates"] = new Dictionary<Language, string>
            {
                { Language.English, "Saved Templates" },
                { Language.SimplifiedChinese, "已保存模板" },
                { Language.TraditionalChinese, "已保存模板" },
                { Language.Spanish, "Plantillas guardadas" },
                { Language.French, "Modèles enregistrés" },
                { Language.Portuguese, "Modelos salvos" },
                { Language.Russian, "Сохранённые шаблоны" },
                { Language.Persian, "قالب‌های ذخیره‌شده" },
                { Language.Norwegian, "Lagrede maler" },
                { Language.Arabic, "القوالب المحفوظة" }
            },
            ["UseUncoloredDevice"] = new Dictionary<Language, string>
            {
                { Language.English, "Use Uncolored Devices" },
                { Language.SimplifiedChinese, "使用未上色装置" },
                { Language.TraditionalChinese, "使用未上色裝置" },
                { Language.Spanish, "Usar dispositivos sin color" },
                { Language.French, "Utiliser des appareils non colorés" },
                { Language.Portuguese, "Usar dispositivos sem cor" },
                { Language.Russian, "Использовать неокрашенные устройства" },
                { Language.Persian, "استفاده از دستگاه‌های بدون رنگ" },
                { Language.Norwegian, "Bruk ufarvede enheter" },
                { Language.Arabic, "استخدام أجهزة غير ملونة" }
            },
            ["UseUncoloredDeviceLabel"] = new Dictionary<Language, string>
            {
                { Language.English, "Device Style:" },
                { Language.SimplifiedChinese, "装置样式：" },
                { Language.TraditionalChinese, "裝置樣式：" },
                { Language.Spanish, "Estilo del dispositivo:" },
                { Language.French, "Style de l’appareil :" },
                { Language.Portuguese, "Estilo do dispositivo:" },
                { Language.Russian, "Стиль устройства:" },
                { Language.Persian, "سبک دستگاه:" },
                { Language.Norwegian, "Enhetsstil:" },
                { Language.Arabic, "نمط الجهاز:" }
            },
            ["UseColoredDevice"] = new Dictionary<Language, string>
            {
                { Language.English, "Colored" },
                { Language.SimplifiedChinese, "上色" },
                { Language.TraditionalChinese, "上色" },
                { Language.Spanish, "A color" },
                { Language.French, "En couleur" },
                { Language.Portuguese, "Colorido" },
                { Language.Russian, "Цветные" },
                { Language.Persian, "رنگی" },
                { Language.Norwegian, "Farget" },
                { Language.Arabic, "ملون" }
            },
            ["Help"] = new Dictionary<Language, string>
            {
                { Language.English, "Help" },
                { Language.SimplifiedChinese, "帮助" },
                { Language.TraditionalChinese, "幫助" },
                { Language.Spanish, "Ayuda" },
                { Language.French, "Aide" },
                { Language.Portuguese, "Ajuda" },
                { Language.Russian, "Справка" },
                { Language.Persian, "راهنما" },
                { Language.Norwegian, "Hjelp" },
                { Language.Arabic, "مساعدة" }
            },
            ["Settings"] = new Dictionary<Language, string>
            {
                { Language.English, "Settings" },
                { Language.SimplifiedChinese, "设置" },
                { Language.TraditionalChinese, "設置" },
                { Language.Spanish, "Configuración" },
                { Language.French, "Paramètres" },
                { Language.Portuguese, "Configurações" },
                { Language.Russian, "Настройки" },
                { Language.Persian, "تنظیمات" },
                { Language.Norwegian, "Innstillinger" },
                { Language.Arabic, "إعدادات" }
            },

            // ===== Panel2 右鍵菜單 =====
            ["Delete"] = new Dictionary<Language, string>
            {
                { Language.English, "Delete" },
                { Language.SimplifiedChinese, "删除" },
                { Language.TraditionalChinese, "刪除" },
                { Language.Spanish, "Eliminar" },
                { Language.French, "Supprimer" },
                { Language.Portuguese, "Excluir" },
                { Language.Russian, "Удалить" },
                { Language.Persian, "حذف" },
                { Language.Norwegian, "Slett" },
                { Language.Arabic, "حذف" }
            },
            ["AutoCapture"] = new Dictionary<Language, string>
            {
                { Language.English, "Auto Screenshot" },
                { Language.SimplifiedChinese, "自动截图" },
                { Language.TraditionalChinese, "自動截圖" },
                { Language.Spanish, "Captura automática" },
                { Language.French, "Capture automatique" },
                { Language.Portuguese, "Captura automática" },
                { Language.Russian, "Автоснимок" },
                { Language.Persian, "اسکرین‌شات خودکار" },
                { Language.Norwegian, "Automatisk skjermbilde" },
                { Language.Arabic, "لقطة شاشة تلقائية" }
            },
            ["Capture"] = new Dictionary<Language, string>
            {
                { Language.English, "Screenshot" },
                { Language.SimplifiedChinese, "截图" },
                { Language.TraditionalChinese, "截圖" },
                { Language.Spanish, "Captura de pantalla" },
                { Language.French, "Capture d’écran" },
                { Language.Portuguese, "Captura de tela" },
                { Language.Russian, "Скриншот" },
                { Language.Persian, "اسکرین‌شات" },
                { Language.Norwegian, "Skjermbilde" },
                { Language.Arabic, "لقطة شاشة" }
            },
            ["ClearCanvas"] = new Dictionary<Language, string>
            {
                { Language.English, "Clear Canvas" },
                { Language.SimplifiedChinese, "清空画布" },
                { Language.TraditionalChinese, "清空畫布" },
                { Language.Spanish, "Limpiar lienzo" },
                { Language.French, "Effacer la toile" },
                { Language.Portuguese, "Limpar tela" },
                { Language.Russian, "Очистить холст" },
                { Language.Persian, "پاک کردن بوم" },
                { Language.Norwegian, "Tøm lerretet" },
                { Language.Arabic, "مسح اللوحة" }
            },
            ["OpenSample"] = new Dictionary<Language, string>
            {
                { Language.English, "Open Device Sample" },
                { Language.SimplifiedChinese, "打开装置样例" },
                { Language.TraditionalChinese, "打開裝置樣例" },
                { Language.Spanish, "Abrir muestra del dispositivo" },
                { Language.French, "Ouvrir l’exemple d’équipement" },
                { Language.Portuguese, "Abrir amostra do dispositivo" },
                { Language.Russian, "Открыть образец устройства" },
                { Language.Persian, "باز کردن نمونه دستگاه" },
                { Language.Norwegian, "Åpne enhetseksempel" },
                { Language.Arabic, "فتح عينة الجهاز" }
            },
            ["AutoAlign"] = new Dictionary<Language, string>
            {
                { Language.English, "Auto Align" },
                { Language.SimplifiedChinese, "自动对齐" },
                { Language.TraditionalChinese, "自動對齊" },
                { Language.Spanish, "Alinear automáticamente" },
                { Language.French, "Aligner automatiquement" },
                { Language.Portuguese, "Alinhar automaticamente" },
                { Language.Russian, "Автовыравнивание" },
                { Language.Persian, "تراز خودکار" },
                { Language.Norwegian, "Autojuster" },
                { Language.Arabic, "محاذاة تلقائية" }
            },
            ["AutoAlignCenter"] = new Dictionary<Language, string>
            {
                { Language.English, "Auto Align (Center)" },
                { Language.SimplifiedChinese, "自动对齐（居中）" },
                { Language.TraditionalChinese, "自動對齊（居中顯示）" },
                { Language.Spanish, "Alinear automáticamente (centrado)" },
                { Language.French, "Aligner automatiquement (centré)" },
                { Language.Portuguese, "Alinhar automaticamente (centralizado)" },
                { Language.Russian, "Автовыравнивание (по центру)" },
                { Language.Persian, "تراز خودکار (وسط)" },
                { Language.Norwegian, "Autojuster (sentrert)" },
                { Language.Arabic, "محاذاة تلقائية (مركز)" }
            },
            ["AutoAlignRight"] = new Dictionary<Language, string>
            {
                { Language.English, "Auto Align (Right)" },
                { Language.SimplifiedChinese, "自动对齐（右侧）" },
                { Language.TraditionalChinese, "自動對齊（右側顯示）" },
                { Language.Spanish, "Alinear automáticamente (derecha)" },
                { Language.French, "Aligner automatiquement (droite)" },
                { Language.Portuguese, "Alinhar automaticamente (direita)" },
                { Language.Russian, "Автовыравнивание (справа)" },
                { Language.Persian, "تراز خودکار (راست)" },
                { Language.Norwegian, "Autojuster (høyre)" },
                { Language.Arabic, "محاذاة تلقائية (يمين)" }
            },
            ["AddSampleToLibrary"] = new Dictionary<Language, string>
            {
                { Language.English, "Add to Library" },
                { Language.SimplifiedChinese, "添加样例到库" },
                { Language.TraditionalChinese, "添加樣例到庫" },
                { Language.Spanish, "Añadir a la biblioteca" },
                { Language.French, "Ajouter à la bibliothèque" },
                { Language.Portuguese, "Adicionar à biblioteca" },
                { Language.Russian, "Добавить в библиотеку" },
                { Language.Persian, "افزودن به کتابخانه" },
                { Language.Norwegian, "Legg til i biblioteket" },
                { Language.Arabic, "إضافة إلى المكتبة" }
            },
            ["SaveCurrentTemplate"] = new Dictionary<Language, string>
            {
                { Language.English, "Save to Current Template" },
                { Language.SimplifiedChinese, "保存到当前模板" },
                { Language.TraditionalChinese, "保存到目前模板" },
                { Language.Spanish, "Guardar en la plantilla actual" },
                { Language.French, "Enregistrer dans le modèle actuel" },
                { Language.Portuguese, "Salvar no modelo atual" },
                { Language.Russian, "Сохранить в текущий шаблон" },
                { Language.Persian, "ذخیره در قالب فعلی" },
                { Language.Norwegian, "Lagre til gjeldende mal" },
                { Language.Arabic, "حفظ في القالب الحالي" }
            },

            // ===== JSON 匯入 =====
            ["ImportJson"] = new Dictionary<Language, string>
            {
                { Language.English, "Import JSON" },
                { Language.SimplifiedChinese, "导入JSON" },
                { Language.TraditionalChinese, "匯入JSON" },
                { Language.Spanish, "Importar JSON" },
                { Language.French, "Importer JSON" },
                { Language.Portuguese, "Importar JSON" },
                { Language.Russian, "Импорт JSON" },
                { Language.Persian, "وارد کردن JSON" },
                { Language.Norwegian, "Importer JSON" },
                { Language.Arabic, "استيراد JSON" }
            },
            ["JsonImportError"] = new Dictionary<Language, string>
            {
                { Language.English, "JSON import failed:\n{0}" },
                { Language.SimplifiedChinese, "JSON导入失败：\n{0}" },
                { Language.TraditionalChinese, "JSON匯入失敗：\n{0}" },
                { Language.Spanish, "Error al importar JSON:\n{0}" },
                { Language.French, "Échec de l'importation JSON:\n{0}" },
                { Language.Portuguese, "Falha ao importar JSON:\n{0}" },
                { Language.Russian, "Ошибка импорта JSON:\n{0}" },
                { Language.Persian, "خطا در وارد کردن JSON:\n{0}" },
                { Language.Norwegian, "JSON-import feilet:\n{0}" },
                { Language.Arabic, "فشل استيراد JSON:\n{0}" }
            },
            ["JsonNoDevices"] = new Dictionary<Language, string>
            {
                { Language.English, "No devices found in JSON" },
                { Language.SimplifiedChinese, "JSON中未找到设备" },
                { Language.TraditionalChinese, "JSON中未找到裝置" },
                { Language.Spanish, "No se encontraron dispositivos en el JSON" },
                { Language.French, "Aucun appareil trouvé dans le JSON" },
                { Language.Portuguese, "Nenhum dispositivo encontrado no JSON" },
                { Language.Russian, "Устройства не найдены в JSON" },
                { Language.Persian, "دستگاهی در JSON یافت نشد" },
                { Language.Norwegian, "Ingen enheter funnet i JSON" },
                { Language.Arabic, "لم يتم العثور على أجهزة في JSON" }
            },
            ["JsonInvalidType"] = new Dictionary<Language, string>
            {
                { Language.English, "Device #{0}: unknown type \"{1}\"" },
                { Language.SimplifiedChinese, "设备#{0}：未知类型\"{1}\"" },
                { Language.TraditionalChinese, "裝置#{0}：未知類型\"{1}\"" },
                { Language.Spanish, "Dispositivo #{0}: tipo desconocido \"{1}\"" },
                { Language.French, "Appareil #{0}: type inconnu \"{1}\"" },
                { Language.Portuguese, "Dispositivo #{0}: tipo desconhecido \"{1}\"" },
                { Language.Russian, "Устройство #{0}: неизвестный тип \"{1}\"" },
                { Language.Persian, "دستگاه #{0}: نوع ناشناخته \"{1}\"" },
                { Language.Norwegian, "Enhet #{0}: ukjent type \"{1}\"" },
                { Language.Arabic, "الجهاز #{0}: نوع غير معروف \"{1}\"" }
            },
            ["JsonTopArrayItemNotObject"] = new Dictionary<Language, string>
            {
                { Language.English, "Top-level array item #{0} is not an object (actual: {1})." },
                { Language.SimplifiedChinese, "顶层数组第#{0}项不是对象（实际类型：{1}）。" },
                { Language.TraditionalChinese, "頂層陣列第#{0}項不是物件（實際類型：{1}）。" },
                { Language.Spanish, "Top-level array item #{0} is not an object (actual: {1})." },
                { Language.French, "Top-level array item #{0} is not an object (actual: {1})." },
                { Language.Portuguese, "Top-level array item #{0} is not an object (actual: {1})." },
                { Language.Russian, "Top-level array item #{0} is not an object (actual: {1})." },
                { Language.Persian, "Top-level array item #{0} is not an object (actual: {1})." },
                { Language.Norwegian, "Top-level array item #{0} is not an object (actual: {1})." },
                { Language.Arabic, "Top-level array item #{0} is not an object (actual: {1})." }
            },
            ["JsonTopLevelMustObjectOrArray"] = new Dictionary<Language, string>
            {
                { Language.English, "Top-level JSON must be object or array." },
                { Language.SimplifiedChinese, "顶层JSON必须是对象或数组。" },
                { Language.TraditionalChinese, "頂層JSON必須是物件或陣列。" },
                { Language.Spanish, "Top-level JSON must be object or array." },
                { Language.French, "Top-level JSON must be object or array." },
                { Language.Portuguese, "Top-level JSON must be object or array." },
                { Language.Russian, "Top-level JSON must be object or array." },
                { Language.Persian, "Top-level JSON must be object or array." },
                { Language.Norwegian, "Top-level JSON must be object or array." },
                { Language.Arabic, "Top-level JSON must be object or array." }
            },
            ["JsonNoTopLevelBlocks"] = new Dictionary<Language, string>
            {
                { Language.English, "No top-level JSON object/array blocks found." },
                { Language.SimplifiedChinese, "未找到顶层JSON对象/数组块。" },
                { Language.TraditionalChinese, "未找到頂層JSON物件/陣列塊。" },
                { Language.Spanish, "No top-level JSON object/array blocks found." },
                { Language.French, "No top-level JSON object/array blocks found." },
                { Language.Portuguese, "No top-level JSON object/array blocks found." },
                { Language.Russian, "No top-level JSON object/array blocks found." },
                { Language.Persian, "No top-level JSON object/array blocks found." },
                { Language.Norwegian, "No top-level JSON object/array blocks found." },
                { Language.Arabic, "No top-level JSON object/array blocks found." }
            },
            ["JsonBlockErrorAtLine"] = new Dictionary<Language, string>
            {
                { Language.English, "JSON #{0} (line {1}): {2}" },
                { Language.SimplifiedChinese, "JSON #{0}（第{1}行）：{2}" },
                { Language.TraditionalChinese, "JSON #{0}（第{1}行）：{2}" },
                { Language.Spanish, "JSON #{0} (line {1}): {2}" },
                { Language.French, "JSON #{0} (line {1}): {2}" },
                { Language.Portuguese, "JSON #{0} (line {1}): {2}" },
                { Language.Russian, "JSON #{0} (line {1}): {2}" },
                { Language.Persian, "JSON #{0} (line {1}): {2}" },
                { Language.Norwegian, "JSON #{0} (line {1}): {2}" },
                { Language.Arabic, "JSON #{0} (line {1}): {2}" }
            },
            ["JsonSchemaImportFailed"] = new Dictionary<Language, string>
            {
                { Language.English, "Schema #{0} ({1}{2}): {3}" },
                { Language.SimplifiedChinese, "Schema #{0}（{1}{2}）：{3}" },
                { Language.TraditionalChinese, "Schema #{0}（{1}{2}）：{3}" },
                { Language.Spanish, "Schema #{0} ({1}{2}): {3}" },
                { Language.French, "Schema #{0} ({1}{2}): {3}" },
                { Language.Portuguese, "Schema #{0} ({1}{2}): {3}" },
                { Language.Russian, "Schema #{0} ({1}{2}): {3}" },
                { Language.Persian, "Schema #{0} ({1}{2}): {3}" },
                { Language.Norwegian, "Schema #{0} ({1}{2}): {3}" },
                { Language.Arabic, "Schema #{0} ({1}{2}): {3}" }
            },
            ["JsonSkippedInvalidBlocks"] = new Dictionary<Language, string>
            {
                { Language.English, "Skipped invalid JSON blocks:" },
                { Language.SimplifiedChinese, "已跳过无效JSON块：" },
                { Language.TraditionalChinese, "已跳過無效JSON塊：" },
                { Language.Spanish, "Skipped invalid JSON blocks:" },
                { Language.French, "Skipped invalid JSON blocks:" },
                { Language.Portuguese, "Skipped invalid JSON blocks:" },
                { Language.Russian, "Skipped invalid JSON blocks:" },
                { Language.Persian, "Skipped invalid JSON blocks:" },
                { Language.Norwegian, "Skipped invalid JSON blocks:" },
                { Language.Arabic, "Skipped invalid JSON blocks:" }
            },
            ["JsonSchemaDevicesEmpty"] = new Dictionary<Language, string>
            {
                { Language.English, "No devices found in JSON (schema.devices is null or empty)." },
                { Language.SimplifiedChinese, "JSON中未找到设备（schema.devices为空）。" },
                { Language.TraditionalChinese, "JSON中未找到裝置（schema.devices為空）。" },
                { Language.Spanish, "No devices found in JSON (schema.devices is null or empty)." },
                { Language.French, "No devices found in JSON (schema.devices is null or empty)." },
                { Language.Portuguese, "No devices found in JSON (schema.devices is null or empty)." },
                { Language.Russian, "No devices found in JSON (schema.devices is null or empty)." },
                { Language.Persian, "No devices found in JSON (schema.devices is null or empty)." },
                { Language.Norwegian, "No devices found in JSON (schema.devices is null or empty)." },
                { Language.Arabic, "No devices found in JSON (schema.devices is null or empty)." }
            },
            ["JsonTypeRequired"] = new Dictionary<Language, string>
            {
                { Language.English, "Device #{0} is missing the required \"type\" field." },
                { Language.SimplifiedChinese, "设备#{0}缺少必填字段\"type\"。" },
                { Language.TraditionalChinese, "裝置#{0}缺少必填欄位\"type\"。" },
                { Language.Spanish, "Device #{0} is missing the required \"type\" field." },
                { Language.French, "Device #{0} is missing the required \"type\" field." },
                { Language.Portuguese, "Device #{0} is missing the required \"type\" field." },
                { Language.Russian, "Device #{0} is missing the required \"type\" field." },
                { Language.Persian, "Device #{0} is missing the required \"type\" field." },
                { Language.Norwegian, "Device #{0} is missing the required \"type\" field." },
                { Language.Arabic, "Device #{0} is missing the required \"type\" field." }
            },
            ["JsonTypeFieldExample"] = new Dictionary<Language, string>
            {
                { Language.English, "Each device object must have: {\"type\": \"device_type_here\", ...}" },
                { Language.SimplifiedChinese, "每个设备对象都必须包含：{\"type\": \"device_type_here\", ...}" },
                { Language.TraditionalChinese, "每個裝置物件都必須包含：{\"type\": \"device_type_here\", ...}" },
                { Language.Spanish, "Each device object must have: {\"type\": \"device_type_here\", ...}" },
                { Language.French, "Each device object must have: {\"type\": \"device_type_here\", ...}" },
                { Language.Portuguese, "Each device object must have: {\"type\": \"device_type_here\", ...}" },
                { Language.Russian, "Each device object must have: {\"type\": \"device_type_here\", ...}" },
                { Language.Persian, "Each device object must have: {\"type\": \"device_type_here\", ...}" },
                { Language.Norwegian, "Each device object must have: {\"type\": \"device_type_here\", ...}" },
                { Language.Arabic, "Each device object must have: {\"type\": \"device_type_here\", ...}" }
            },
            ["JsonDevicePathHint"] = new Dictionary<Language, string>
            {
                { Language.English, "(devices[{0}]{1})" },
                { Language.SimplifiedChinese, "（devices[{0}]{1}）" },
                { Language.TraditionalChinese, "（devices[{0}]{1}）" },
                { Language.Spanish, "(devices[{0}]{1})" },
                { Language.French, "(devices[{0}]{1})" },
                { Language.Portuguese, "(devices[{0}]{1})" },
                { Language.Russian, "(devices[{0}]{1})" },
                { Language.Persian, "(devices[{0}]{1})" },
                { Language.Norwegian, "(devices[{0}]{1})" },
                { Language.Arabic, "(devices[{0}]{1})" }
            },
            ["JsonUnknownDeviceType"] = new Dictionary<Language, string>
            {
                { Language.English, "Unknown device type: \"{0}\"." },
                { Language.SimplifiedChinese, "未知设备类型：\"{0}\"。" },
                { Language.TraditionalChinese, "未知裝置類型：\"{0}\"。" },
                { Language.Spanish, "Unknown device type: \"{0}\"." },
                { Language.French, "Unknown device type: \"{0}\"." },
                { Language.Portuguese, "Unknown device type: \"{0}\"." },
                { Language.Russian, "Unknown device type: \"{0}\"." },
                { Language.Persian, "Unknown device type: \"{0}\"." },
                { Language.Norwegian, "Unknown device type: \"{0}\"." },
                { Language.Arabic, "Unknown device type: \"{0}\"." }
            },
            ["JsonDidYouMean"] = new Dictionary<Language, string>
            {
                { Language.English, "Did you mean: \"{0}\"?" },
                { Language.SimplifiedChinese, "你是否想写：\"{0}\"？" },
                { Language.TraditionalChinese, "你是否想寫：\"{0}\"？" },
                { Language.Spanish, "Did you mean: \"{0}\"?" },
                { Language.French, "Did you mean: \"{0}\"?" },
                { Language.Portuguese, "Did you mean: \"{0}\"?" },
                { Language.Russian, "Did you mean: \"{0}\"?" },
                { Language.Persian, "Did you mean: \"{0}\"?" },
                { Language.Norwegian, "Did you mean: \"{0}\"?" },
                { Language.Arabic, "Did you mean: \"{0}\"?" }
            },
            ["JsonExpectedBuiltInTypes"] = new Dictionary<Language, string>
            {
                { Language.English, "Expected one of the 23 built-in types (e.g. annular_bop, ram_bop, rotary_table, casing_head)." },
                { Language.SimplifiedChinese, "应为23种内建设备类型之一（如 annular_bop、ram_bop、rotary_table、casing_head）。" },
                { Language.TraditionalChinese, "應為23種內建裝置類型之一（如 annular_bop、ram_bop、rotary_table、casing_head）。" },
                { Language.Spanish, "Expected one of the 23 built-in types (e.g. annular_bop, ram_bop, rotary_table, casing_head)." },
                { Language.French, "Expected one of the 23 built-in types (e.g. annular_bop, ram_bop, rotary_table, casing_head)." },
                { Language.Portuguese, "Expected one of the 23 built-in types (e.g. annular_bop, ram_bop, rotary_table, casing_head)." },
                { Language.Russian, "Expected one of the 23 built-in types (e.g. annular_bop, ram_bop, rotary_table, casing_head)." },
                { Language.Persian, "Expected one of the 23 built-in types (e.g. annular_bop, ram_bop, rotary_table, casing_head)." },
                { Language.Norwegian, "Expected one of the 23 built-in types (e.g. annular_bop, ram_bop, rotary_table, casing_head)." },
                { Language.Arabic, "Expected one of the 23 built-in types (e.g. annular_bop, ram_bop, rotary_table, casing_head)." }
            },
            ["JsonOrCustomDevices"] = new Dictionary<Language, string>
            {
                { Language.English, "Or one of the custom devices:" },
                { Language.SimplifiedChinese, "或使用以下自定义设备之一：" },
                { Language.TraditionalChinese, "或使用以下自訂裝置之一：" },
                { Language.Spanish, "Or one of the custom devices:" },
                { Language.French, "Or one of the custom devices:" },
                { Language.Portuguese, "Or one of the custom devices:" },
                { Language.Russian, "Or one of the custom devices:" },
                { Language.Persian, "Or one of the custom devices:" },
                { Language.Norwegian, "Or one of the custom devices:" },
                { Language.Arabic, "Or one of the custom devices:" }
            },
            ["JsonSeeSchemaTypeList"] = new Dictionary<Language, string>
            {
                { Language.English, "See the full type list in the JSON schema (copy via the schema button)." },
                { Language.SimplifiedChinese, "请在JSON规范中查看完整type列表（可通过上方按钮复制）。" },
                { Language.TraditionalChinese, "請在JSON規範中查看完整type列表（可透過上方按鈕複製）。" },
                { Language.Spanish, "See the full type list in the JSON schema (copy via the schema button)." },
                { Language.French, "See the full type list in the JSON schema (copy via the schema button)." },
                { Language.Portuguese, "See the full type list in the JSON schema (copy via the schema button)." },
                { Language.Russian, "See the full type list in the JSON schema (copy via the schema button)." },
                { Language.Persian, "See the full type list in the JSON schema (copy via the schema button)." },
                { Language.Norwegian, "See the full type list in the JSON schema (copy via the schema button)." },
                { Language.Arabic, "See the full type list in the JSON schema (copy via the schema button)." }
            },
            ["JsonDeviceNoDrawableResource"] = new Dictionary<Language, string>
            {
                { Language.English, "Device #{0} ({1}, devices[{2}]{3}): no drawable resource found." },
                { Language.SimplifiedChinese, "设备#{0}（{1}，devices[{2}]{3}）：未找到可绘制资源。" },
                { Language.TraditionalChinese, "裝置#{0}（{1}，devices[{2}]{3}）：未找到可繪製資源。" },
                { Language.Spanish, "Device #{0} ({1}, devices[{2}]{3}): no drawable resource found." },
                { Language.French, "Device #{0} ({1}, devices[{2}]{3}): no drawable resource found." },
                { Language.Portuguese, "Device #{0} ({1}, devices[{2}]{3}): no drawable resource found." },
                { Language.Russian, "Device #{0} ({1}, devices[{2}]{3}): no drawable resource found." },
                { Language.Persian, "Device #{0} ({1}, devices[{2}]{3}): no drawable resource found." },
                { Language.Norwegian, "Device #{0} ({1}, devices[{2}]{3}): no drawable resource found." },
                { Language.Arabic, "Device #{0} ({1}, devices[{2}]{3}): no drawable resource found." }
            },
            ["JsonDeviceReadSvgSizeFailed"] = new Dictionary<Language, string>
            {
                { Language.English, "Device #{0} ({1}, devices[{2}]{3}): failed to read SVG size." },
                { Language.SimplifiedChinese, "设备#{0}（{1}，devices[{2}]{3}）：读取SVG尺寸失败。" },
                { Language.TraditionalChinese, "裝置#{0}（{1}，devices[{2}]{3}）：讀取SVG尺寸失敗。" },
                { Language.Spanish, "Device #{0} ({1}, devices[{2}]{3}): failed to read SVG size." },
                { Language.French, "Device #{0} ({1}, devices[{2}]{3}): failed to read SVG size." },
                { Language.Portuguese, "Device #{0} ({1}, devices[{2}]{3}): failed to read SVG size." },
                { Language.Russian, "Device #{0} ({1}, devices[{2}]{3}): failed to read SVG size." },
                { Language.Persian, "Device #{0} ({1}, devices[{2}]{3}): failed to read SVG size." },
                { Language.Norwegian, "Device #{0} ({1}, devices[{2}]{3}): failed to read SVG size." },
                { Language.Arabic, "Device #{0} ({1}, devices[{2}]{3}): failed to read SVG size." }
            },
            ["JsonCustomImageNotFound"] = new Dictionary<Language, string>
            {
                { Language.English, "Device #{0} ({1}, devices[{2}]{3}): custom image not found: {4}" },
                { Language.SimplifiedChinese, "设备#{0}（{1}，devices[{2}]{3}）：未找到自定义图片：{4}" },
                { Language.TraditionalChinese, "裝置#{0}（{1}，devices[{2}]{3}）：未找到自訂圖片：{4}" },
                { Language.Spanish, "Device #{0} ({1}, devices[{2}]{3}): custom image not found: {4}" },
                { Language.French, "Device #{0} ({1}, devices[{2}]{3}): custom image not found: {4}" },
                { Language.Portuguese, "Device #{0} ({1}, devices[{2}]{3}): custom image not found: {4}" },
                { Language.Russian, "Device #{0} ({1}, devices[{2}]{3}): custom image not found: {4}" },
                { Language.Persian, "Device #{0} ({1}, devices[{2}]{3}): custom image not found: {4}" },
                { Language.Norwegian, "Device #{0} ({1}, devices[{2}]{3}): custom image not found: {4}" },
                { Language.Arabic, "Device #{0} ({1}, devices[{2}]{3}): custom image not found: {4}" }
            },
            ["JsonDeviceRenderFailed"] = new Dictionary<Language, string>
            {
                { Language.English, "Device #{0} ({1}, devices[{2}]{3}): render failed - {4}" },
                { Language.SimplifiedChinese, "设备#{0}（{1}，devices[{2}]{3}）：渲染失败 - {4}" },
                { Language.TraditionalChinese, "裝置#{0}（{1}，devices[{2}]{3}）：渲染失敗 - {4}" },
                { Language.Spanish, "Device #{0} ({1}, devices[{2}]{3}): render failed - {4}" },
                { Language.French, "Device #{0} ({1}, devices[{2}]{3}): render failed - {4}" },
                { Language.Portuguese, "Device #{0} ({1}, devices[{2}]{3}): render failed - {4}" },
                { Language.Russian, "Device #{0} ({1}, devices[{2}]{3}): render failed - {4}" },
                { Language.Persian, "Device #{0} ({1}, devices[{2}]{3}): render failed - {4}" },
                { Language.Norwegian, "Device #{0} ({1}, devices[{2}]{3}): render failed - {4}" },
                { Language.Arabic, "Device #{0} ({1}, devices[{2}]{3}): render failed - {4}" }
            },
            ["JsonEachBlockMustObject"] = new Dictionary<Language, string>
            {
                { Language.English, "Each JSON block must be an object." },
                { Language.SimplifiedChinese, "每个JSON块都必须是对象。" },
                { Language.TraditionalChinese, "每個JSON塊都必須是物件。" },
                { Language.Spanish, "Each JSON block must be an object." },
                { Language.French, "Each JSON block must be an object." },
                { Language.Portuguese, "Each JSON block must be an object." },
                { Language.Russian, "Each JSON block must be an object." },
                { Language.Persian, "Each JSON block must be an object." },
                { Language.Norwegian, "Each JSON block must be an object." },
                { Language.Arabic, "Each JSON block must be an object." }
            },
            ["JsonDevicesFieldNotFound"] = new Dictionary<Language, string>
            {
                { Language.English, "Could not find a \"devices\" field (also tried: device, items, equipment, components, stack, etc.)" },
                { Language.SimplifiedChinese, "未找到\"devices\"字段（也尝试了：device、items、equipment、components、stack等）。" },
                { Language.TraditionalChinese, "未找到\"devices\"欄位（也嘗試了：device、items、equipment、components、stack等）。" },
                { Language.Spanish, "Could not find a \"devices\" field (also tried: device, items, equipment, components, stack, etc.)" },
                { Language.French, "Could not find a \"devices\" field (also tried: device, items, equipment, components, stack, etc.)" },
                { Language.Portuguese, "Could not find a \"devices\" field (also tried: device, items, equipment, components, stack, etc.)" },
                { Language.Russian, "Could not find a \"devices\" field (also tried: device, items, equipment, components, stack, etc.)" },
                { Language.Persian, "Could not find a \"devices\" field (also tried: device, items, equipment, components, stack, etc.)" },
                { Language.Norwegian, "Could not find a \"devices\" field (also tried: device, items, equipment, components, stack, etc.)" },
                { Language.Arabic, "Could not find a \"devices\" field (also tried: device, items, equipment, components, stack, etc.)" }
            },
            ["JsonObjectKeysFound"] = new Dictionary<Language, string>
            {
                { Language.English, "Keys found in this object: {0}" },
                { Language.SimplifiedChinese, "该对象中找到的键：{0}" },
                { Language.TraditionalChinese, "該物件中找到的鍵：{0}" },
                { Language.Spanish, "Keys found in this object: {0}" },
                { Language.French, "Keys found in this object: {0}" },
                { Language.Portuguese, "Keys found in this object: {0}" },
                { Language.Russian, "Keys found in this object: {0}" },
                { Language.Persian, "Keys found in this object: {0}" },
                { Language.Norwegian, "Keys found in this object: {0}" },
                { Language.Arabic, "Keys found in this object: {0}" }
            },
            ["JsonExpectedFormat"] = new Dictionary<Language, string>
            {
                { Language.English, "Expected format: { \"devices\": [ {\"type\": \"...\", ...}, ... ] }" },
                { Language.SimplifiedChinese, "期望格式：{ \"devices\": [ {\"type\": \"...\", ...}, ... ] }" },
                { Language.TraditionalChinese, "期望格式：{ \"devices\": [ {\"type\": \"...\", ...}, ... ] }" },
                { Language.Spanish, "Expected format: { \"devices\": [ {\"type\": \"...\", ...}, ... ] }" },
                { Language.French, "Expected format: { \"devices\": [ {\"type\": \"...\", ...}, ... ] }" },
                { Language.Portuguese, "Expected format: { \"devices\": [ {\"type\": \"...\", ...}, ... ] }" },
                { Language.Russian, "Expected format: { \"devices\": [ {\"type\": \"...\", ...}, ... ] }" },
                { Language.Persian, "Expected format: { \"devices\": [ {\"type\": \"...\", ...}, ... ] }" },
                { Language.Norwegian, "Expected format: { \"devices\": [ {\"type\": \"...\", ...}, ... ] }" },
                { Language.Arabic, "Expected format: { \"devices\": [ {\"type\": \"...\", ...}, ... ] }" }
            },
            ["JsonInvalidDeviceItemAtIndex"] = new Dictionary<Language, string>
            {
                { Language.English, "Invalid device item at devices[{0}] (token type: {1}{2})." },
                { Language.SimplifiedChinese, "devices[{0}] 处的设备项无效（token类型：{1}{2}）。" },
                { Language.TraditionalChinese, "devices[{0}] 處的裝置項無效（token類型：{1}{2}）。" },
                { Language.Spanish, "Invalid device item at devices[{0}] (token type: {1}{2})." },
                { Language.French, "Invalid device item at devices[{0}] (token type: {1}{2})." },
                { Language.Portuguese, "Invalid device item at devices[{0}] (token type: {1}{2})." },
                { Language.Russian, "Invalid device item at devices[{0}] (token type: {1}{2})." },
                { Language.Persian, "Invalid device item at devices[{0}] (token type: {1}{2})." },
                { Language.Norwegian, "Invalid device item at devices[{0}] (token type: {1}{2})." },
                { Language.Arabic, "Invalid device item at devices[{0}] (token type: {1}{2})." }
            },
            ["JsonInvalidDeviceItem"] = new Dictionary<Language, string>
            {
                { Language.English, "Invalid device item in JSON (token type: {0}{1})." },
                { Language.SimplifiedChinese, "JSON中的设备项无效（token类型：{0}{1}）。" },
                { Language.TraditionalChinese, "JSON中的裝置項無效（token類型：{0}{1}）。" },
                { Language.Spanish, "Invalid device item in JSON (token type: {0}{1})." },
                { Language.French, "Invalid device item in JSON (token type: {0}{1})." },
                { Language.Portuguese, "Invalid device item in JSON (token type: {0}{1})." },
                { Language.Russian, "Invalid device item in JSON (token type: {0}{1})." },
                { Language.Persian, "Invalid device item in JSON (token type: {0}{1})." },
                { Language.Norwegian, "Invalid device item in JSON (token type: {0}{1})." },
                { Language.Arabic, "Invalid device item in JSON (token type: {0}{1})." }
            },
            ["JsonUnnamedSchema"] = new Dictionary<Language, string>
            {
                { Language.English, "unnamed-{0}" },
                { Language.SimplifiedChinese, "未命名-{0}" },
                { Language.TraditionalChinese, "未命名-{0}" },
                { Language.Spanish, "unnamed-{0}" },
                { Language.French, "unnamed-{0}" },
                { Language.Portuguese, "unnamed-{0}" },
                { Language.Russian, "unnamed-{0}" },
                { Language.Persian, "unnamed-{0}" },
                { Language.Norwegian, "unnamed-{0}" },
                { Language.Arabic, "unnamed-{0}" }
            },
            ["JsonLinePosHint"] = new Dictionary<Language, string>
            {
                { Language.English, ", line {0}, pos {1}" },
                { Language.SimplifiedChinese, "，第{0}行，第{1}列" },
                { Language.TraditionalChinese, "，第{0}行，第{1}列" },
                { Language.Spanish, ", line {0}, pos {1}" },
                { Language.French, ", line {0}, pos {1}" },
                { Language.Portuguese, ", line {0}, pos {1}" },
                { Language.Russian, ", line {0}, pos {1}" },
                { Language.Persian, ", line {0}, pos {1}" },
                { Language.Norwegian, ", line {0}, pos {1}" },
                { Language.Arabic, ", line {0}, pos {1}" }
            },
            ["JsonUnknownReaderError"] = new Dictionary<Language, string>
            {
                { Language.English, "Unknown JSON reader error." },
                { Language.SimplifiedChinese, "未知JSON读取错误。" },
                { Language.TraditionalChinese, "未知JSON讀取錯誤。" },
                { Language.Spanish, "Unknown JSON reader error." },
                { Language.French, "Unknown JSON reader error." },
                { Language.Portuguese, "Unknown JSON reader error." },
                { Language.Russian, "Unknown JSON reader error." },
                { Language.Persian, "Unknown JSON reader error." },
                { Language.Norwegian, "Unknown JSON reader error." },
                { Language.Arabic, "Unknown JSON reader error." }
            },
            ["JsonSyntaxErrorAtLinePos"] = new Dictionary<Language, string>
            {
                { Language.English, "JSON syntax error at line {0}, position {1}:" },
                { Language.SimplifiedChinese, "JSON语法错误：第{0}行，第{1}列：" },
                { Language.TraditionalChinese, "JSON語法錯誤：第{0}行，第{1}列：" },
                { Language.Spanish, "JSON syntax error at line {0}, position {1}:" },
                { Language.French, "JSON syntax error at line {0}, position {1}:" },
                { Language.Portuguese, "JSON syntax error at line {0}, position {1}:" },
                { Language.Russian, "JSON syntax error at line {0}, position {1}:" },
                { Language.Persian, "JSON syntax error at line {0}, position {1}:" },
                { Language.Norwegian, "JSON syntax error at line {0}, position {1}:" },
                { Language.Arabic, "JSON syntax error at line {0}, position {1}:" }
            },
            ["JsonNearContext"] = new Dictionary<Language, string>
            {
                { Language.English, "  Near: ...{0}..." },
                { Language.SimplifiedChinese, "  附近：...{0}..." },
                { Language.TraditionalChinese, "  附近：...{0}..." },
                { Language.Spanish, "  Near: ...{0}..." },
                { Language.French, "  Near: ...{0}..." },
                { Language.Portuguese, "  Near: ...{0}..." },
                { Language.Russian, "  Near: ...{0}..." },
                { Language.Persian, "  Near: ...{0}..." },
                { Language.Norwegian, "  Near: ...{0}..." },
                { Language.Arabic, "  Near: ...{0}..." }
            },
            ["JsonContextHeader"] = new Dictionary<Language, string>
            {
                { Language.English, "  Context:" },
                { Language.SimplifiedChinese, "  上下文：" },
                { Language.TraditionalChinese, "  上下文：" },
                { Language.Spanish, "  Context:" },
                { Language.French, "  Context:" },
                { Language.Portuguese, "  Context:" },
                { Language.Russian, "  Context:" },
                { Language.Persian, "  Context:" },
                { Language.Norwegian, "  Context:" },
                { Language.Arabic, "  Context:" }
            },
            ["JsonCommonFixes"] = new Dictionary<Language, string>
            {
                { Language.English, "Common fixes:" },
                { Language.SimplifiedChinese, "常见修复建议：" },
                { Language.TraditionalChinese, "常見修復建議：" },
                { Language.Spanish, "Common fixes:" },
                { Language.French, "Common fixes:" },
                { Language.Portuguese, "Common fixes:" },
                { Language.Russian, "Common fixes:" },
                { Language.Persian, "Common fixes:" },
                { Language.Norwegian, "Common fixes:" },
                { Language.Arabic, "Common fixes:" }
            },
            ["JsonFixMissingComma"] = new Dictionary<Language, string>
            {
                { Language.English, "  - Check for missing commas between array items or object properties" },
                { Language.SimplifiedChinese, "  - 检查数组项或对象属性之间是否缺少逗号" },
                { Language.TraditionalChinese, "  - 檢查陣列項或物件屬性之間是否缺少逗號" },
                { Language.Spanish, "  - Check for missing commas between array items or object properties" },
                { Language.French, "  - Check for missing commas between array items or object properties" },
                { Language.Portuguese, "  - Check for missing commas between array items or object properties" },
                { Language.Russian, "  - Check for missing commas between array items or object properties" },
                { Language.Persian, "  - Check for missing commas between array items or object properties" },
                { Language.Norwegian, "  - Check for missing commas between array items or object properties" },
                { Language.Arabic, "  - Check for missing commas between array items or object properties" }
            },
            ["JsonFixTrailingComma"] = new Dictionary<Language, string>
            {
                { Language.English, "  - Check for trailing commas before } or ]" },
                { Language.SimplifiedChinese, "  - 检查是否在 } 或 ] 之前有多余逗号" },
                { Language.TraditionalChinese, "  - 檢查是否在 } 或 ] 之前有多餘逗號" },
                { Language.Spanish, "  - Check for trailing commas before } or ]" },
                { Language.French, "  - Check for trailing commas before } or ]" },
                { Language.Portuguese, "  - Check for trailing commas before } or ]" },
                { Language.Russian, "  - Check for trailing commas before } or ]" },
                { Language.Persian, "  - Check for trailing commas before } or ]" },
                { Language.Norwegian, "  - Check for trailing commas before } or ]" },
                { Language.Arabic, "  - Check for trailing commas before } or ]" }
            },
            ["JsonFixDoubleQuotes"] = new Dictionary<Language, string>
            {
                { Language.English, "  - Check all strings use standard double quotes \"" },
                { Language.SimplifiedChinese, "  - 检查所有字符串是否使用标准双引号 \"" },
                { Language.TraditionalChinese, "  - 檢查所有字串是否使用標準雙引號 \"" },
                { Language.Spanish, "  - Check all strings use standard double quotes \"" },
                { Language.French, "  - Check all strings use standard double quotes \"" },
                { Language.Portuguese, "  - Check all strings use standard double quotes \"" },
                { Language.Russian, "  - Check all strings use standard double quotes \"" },
                { Language.Persian, "  - Check all strings use standard double quotes \"" },
                { Language.Norwegian, "  - Check all strings use standard double quotes \"" },
                { Language.Arabic, "  - Check all strings use standard double quotes \"" }
            },
            ["JsonFixBalancedBrackets"] = new Dictionary<Language, string>
            {
                { Language.English, "  - Check all brackets {} and [] are balanced" },
                { Language.SimplifiedChinese, "  - 检查所有括号 {} 和 [] 是否成对" },
                { Language.TraditionalChinese, "  - 檢查所有括號 {} 和 [] 是否成對" },
                { Language.Spanish, "  - Check all brackets {} and [] are balanced" },
                { Language.French, "  - Check all brackets {} and [] are balanced" },
                { Language.Portuguese, "  - Check all brackets {} and [] are balanced" },
                { Language.Russian, "  - Check all brackets {} and [] are balanced" },
                { Language.Persian, "  - Check all brackets {} and [] are balanced" },
                { Language.Norwegian, "  - Check all brackets {} and [] are balanced" },
                { Language.Arabic, "  - Check all brackets {} and [] are balanced" }
            },
            ["JsonOriginalError"] = new Dictionary<Language, string>
            {
                { Language.English, "Original error: {0}" },
                { Language.SimplifiedChinese, "原始错误：{0}" },
                { Language.TraditionalChinese, "原始錯誤：{0}" },
                { Language.Spanish, "Original error: {0}" },
                { Language.French, "Original error: {0}" },
                { Language.Portuguese, "Original error: {0}" },
                { Language.Russian, "Original error: {0}" },
                { Language.Persian, "Original error: {0}" },
                { Language.Norwegian, "Original error: {0}" },
                { Language.Arabic, "Original error: {0}" }
            },
            ["JsonUnknown"] = new Dictionary<Language, string>
            {
                { Language.English, "unknown" },
                { Language.SimplifiedChinese, "未知" },
                { Language.TraditionalChinese, "未知" },
                { Language.Spanish, "unknown" },
                { Language.French, "unknown" },
                { Language.Portuguese, "unknown" },
                { Language.Russian, "unknown" },
                { Language.Persian, "unknown" },
                { Language.Norwegian, "unknown" },
                { Language.Arabic, "unknown" }
            },
            ["JsonNone"] = new Dictionary<Language, string>
            {
                { Language.English, "(none)" },
                { Language.SimplifiedChinese, "（无）" },
                { Language.TraditionalChinese, "（無）" },
                { Language.Spanish, "(none)" },
                { Language.French, "(none)" },
                { Language.Portuguese, "(none)" },
                { Language.Russian, "(none)" },
                { Language.Persian, "(none)" },
                { Language.Norwegian, "(none)" },
                { Language.Arabic, "(none)" }
            },
            ["JsonPasteHint"] = new Dictionary<Language, string>
            {
                { Language.English, "Send Excel/image + JSON schema (copy above) to any AI to generate JSON, then paste here." },
                { Language.SimplifiedChinese, "将Excel或图片加上JSON规则（点上方按钮复制）发给任意AI，让AI生成JSON后粘贴到此处。" },
                { Language.TraditionalChinese, "將Excel或圖片加上JSON規則（點上方按鈕複製）發給任意AI，讓AI生成JSON後貼上到此處。" },
                { Language.Spanish, "Envíe Excel/imagen + esquema JSON (copiar arriba) a cualquier IA para generar JSON, luego pegue aquí." },
                { Language.French, "Envoyez Excel/image + schéma JSON (copier ci-dessus) à n'importe quelle IA, puis collez le JSON ici." },
                { Language.Portuguese, "Envie Excel/imagem + esquema JSON (copiar acima) para qualquer IA, depois cole o JSON aqui." },
                { Language.Russian, "Отправьте Excel/изображение + схему JSON (скопируйте выше) любому ИИ, затем вставьте JSON сюда." },
                { Language.Persian, "Excel/تصویر + طرح JSON (کپی بالا) را به هر هوش مصنوعی بفرستید، سپس JSON را اینجا بچسبانید." },
                { Language.Norwegian, "Send Excel/bilde + JSON-skjema (kopier ovenfor) til en AI, lim deretter inn JSON her." },
                { Language.Arabic, "أرسل Excel/صورة + مخطط JSON (انسخ أعلاه) إلى أي ذكاء اصطناعي، ثم الصق JSON هنا." }
            },
            ["CopyJsonSchema"] = new Dictionary<Language, string>
            {
                { Language.English, "Copy JSON Schema to Clipboard" },
                { Language.SimplifiedChinese, "复制JSON规范到剪贴板" },
                { Language.TraditionalChinese, "複製JSON規範到剪貼板" },
                { Language.Spanish, "Copiar esquema JSON al portapapeles" },
                { Language.French, "Copier le schéma JSON dans le presse-papiers" },
                { Language.Portuguese, "Copiar esquema JSON para a área de transferência" },
                { Language.Russian, "Скопировать схему JSON в буфер обмена" },
                { Language.Persian, "کپی طرح JSON به کلیپ بورد" },
                { Language.Norwegian, "Kopier JSON-skjema til utklippstavlen" },
                { Language.Arabic, "نسخ مخطط JSON إلى الحافظة" }
            },
            ["Copied"] = new Dictionary<Language, string>
            {
                { Language.English, "Copied!" },
                { Language.SimplifiedChinese, "已复制！" },
                { Language.TraditionalChinese, "已複製！" },
                { Language.Spanish, "¡Copiado!" },
                { Language.French, "Copié !" },
                { Language.Portuguese, "Copiado!" },
                { Language.Russian, "Скопировано!" },
                { Language.Persian, "!کپی شد" },
                { Language.Norwegian, "Kopiert!" },
                { Language.Arabic, "!تم النسخ" }
            },
            ["CopyError"] = new Dictionary<Language, string>
            {
                { Language.English, "Copy Error" },
                { Language.SimplifiedChinese, "复制错误信息" },
                { Language.TraditionalChinese, "複製錯誤信息" },
                { Language.Spanish, "Copiar error" },
                { Language.French, "Copier l'erreur" },
                { Language.Portuguese, "Copiar erro" },
                { Language.Russian, "Копировать ошибку" },
                { Language.Persian, "کپی خطا" },
                { Language.Norwegian, "Kopier feil" },
                { Language.Arabic, "نسخ الخطأ" }
            },

            // ===== 複製粘貼 =====
            ["CopyNode"] = new Dictionary<Language, string>
            {
                { Language.English, "Copy" },
                { Language.SimplifiedChinese, "复制" },
                { Language.TraditionalChinese, "複製" },
                { Language.Spanish, "Copiar" },
                { Language.French, "Copier" },
                { Language.Portuguese, "Copiar" },
                { Language.Russian, "Копировать" },
                { Language.Persian, "کپی" },
                { Language.Norwegian, "Kopier" },
                { Language.Arabic, "نسخ" }
            },
            ["PasteAsChild"] = new Dictionary<Language, string>
            {
                { Language.English, "Paste as Child" },
                { Language.SimplifiedChinese, "粘贴为子节点" },
                { Language.TraditionalChinese, "貼上為子節點" },
                { Language.Spanish, "Pegar como hijo" },
                { Language.French, "Coller en tant qu'enfant" },
                { Language.Portuguese, "Colar como filho" },
                { Language.Russian, "Вставить как дочерний" },
                { Language.Persian, "چسباندن به عنوان زیرمجموعه" },
                { Language.Norwegian, "Lim inn som underordnet" },
                { Language.Arabic, "لصق كعنصر فرعي" }
            },

            // ===== ListBox1 右鍵菜單 =====
            ["AddCustomDevice"] = new Dictionary<Language, string>
            {
                { Language.English, "Add Custom Device" },
                { Language.SimplifiedChinese, "添加自绘装置" },
                { Language.TraditionalChinese, "添加自繪裝置" },
                { Language.Spanish, "Agregar dispositivo personalizado" },
                { Language.French, "Ajouter un appareil personnalisé" },
                { Language.Portuguese, "Adicionar dispositivo personalizado" },
                { Language.Russian, "Добавить пользовательское устройство" },
                { Language.Persian, "افزودن دستگاه سفارشی" },
                { Language.Norwegian, "Legg til egendefinert enhet" },
                { Language.Arabic, "إضافة جهاز مخصص" }
            },
            ["DeleteCurrentDevice"] = new Dictionary<Language, string>
            {
                { Language.English, "Delete Current Device" },
                { Language.SimplifiedChinese, "删除当前装置" },
                { Language.TraditionalChinese, "刪除當前裝置" },
                { Language.Spanish, "Eliminar dispositivo actual" },
                { Language.French, "Supprimer l’appareil actuel" },
                { Language.Portuguese, "Excluir dispositivo atual" },
                { Language.Russian, "Удалить текущее устройство" },
                { Language.Persian, "حذف دستگاه فعلی" },
                { Language.Norwegian, "Slett gjeldende enhet" },
                { Language.Arabic, "حذف الجهاز الحالي" }
            },

            // ===== 模板樹右鍵菜單 =====
            ["Rename"] = new Dictionary<Language, string>
            {
                { Language.English, "Rename" },
                { Language.SimplifiedChinese, "重命名" },
                { Language.TraditionalChinese, "重命名" },
                { Language.Spanish, "Renombrar" },
                { Language.French, "Renommer" },
                { Language.Portuguese, "Renomear" },
                { Language.Russian, "Переименовать" },
                { Language.Persian, "تغییر نام" },
                { Language.Norwegian, "Gi nytt navn" },
                { Language.Arabic, "إعادة تسمية" }
            },
            ["AddFolder"] = new Dictionary<Language, string>
            {
                { Language.English, "New Folder" },
                { Language.SimplifiedChinese, "新增文件夹" },
                { Language.TraditionalChinese, "新增資料夾" },
                { Language.Spanish, "Nueva carpeta" },
                { Language.French, "Nouveau dossier" },
                { Language.Portuguese, "Nova pasta" },
                { Language.Russian, "Новая папка" },
                { Language.Persian, "پوشه جدید" },
                { Language.Norwegian, "Ny mappe" },
                { Language.Arabic, "مجلد جديد" }
            },

            // ===== 對話框 =====
            ["SaveTemplateTitle"] = new Dictionary<Language, string>
            {
                { Language.English, "Save Template to Library" },
                { Language.SimplifiedChinese, "保存模板到库" },
                { Language.TraditionalChinese, "保存模板到庫" },
                { Language.Spanish, "Guardar plantilla en la biblioteca" },
                { Language.French, "Enregistrer le modèle dans la bibliothèque" },
                { Language.Portuguese, "Salvar modelo na biblioteca" },
                { Language.Russian, "Сохранить шаблон в библиотеку" },
                { Language.Persian, "ذخیره قالب در کتابخانه" },
                { Language.Norwegian, "Lagre mal i biblioteket" },
                { Language.Arabic, "حفظ القالب في المكتبة" }
            },
            ["SelectTargetFolder"] = new Dictionary<Language, string>
            {
                { Language.English, "Select target folder (root if none selected):" },
                { Language.SimplifiedChinese, "选择目标文件夹（不选择则保存为根节点）：" },
                { Language.TraditionalChinese, "選擇目標資料夾（不選擇則保存為根節點）：" },
                { Language.Spanish, "Seleccione la carpeta destino (raíz si no se selecciona):" },
                { Language.French, "Sélectionnez le dossier cible (racine si aucun n’est sélectionné) :" },
                { Language.Portuguese, "Selecione a pasta de destino (raiz se nenhuma for selecionada):" },
                { Language.Russian, "Выберите целевую папку (если не выбрано — корень):" },
                { Language.Persian, "پوشه مقصد را انتخاب کنید (در صورت عدم انتخاب، ریشه):" },
                { Language.Norwegian, "Velg målmappe (rot hvis ingen valgt):" },
                { Language.Arabic, "اختر المجلد الهدف (الجذر إذا لم يتم التحديد):" }
            },
            ["TemplateName"] = new Dictionary<Language, string>
            {
                { Language.English, "Template Name:" },
                { Language.SimplifiedChinese, "模板名称：" },
                { Language.TraditionalChinese, "模板名稱：" },
                { Language.Spanish, "Nombre de la plantilla:" },
                { Language.French, "Nom du modèle :" },
                { Language.Portuguese, "Nome do modelo:" },
                { Language.Russian, "Имя шаблона:" },
                { Language.Persian, "نام قالب:" },
                { Language.Norwegian, "Navn på mal:" },
                { Language.Arabic, "اسم القالب:" }
            },
            ["Save"] = new Dictionary<Language, string>
            {
                { Language.English, "Save" },
                { Language.SimplifiedChinese, "保存" },
                { Language.TraditionalChinese, "保存" },
                { Language.Spanish, "Guardar" },
                { Language.French, "Enregistrer" },
                { Language.Portuguese, "Salvar" },
                { Language.Russian, "Сохранить" },
                { Language.Persian, "ذخیره" },
                { Language.Norwegian, "Lagre" },
                { Language.Arabic, "حفظ" }
            },
            ["Cancel"] = new Dictionary<Language, string>
            {
                { Language.English, "Cancel" },
                { Language.SimplifiedChinese, "取消" },
                { Language.TraditionalChinese, "取消" },
                { Language.Spanish, "Cancelar" },
                { Language.French, "Annuler" },
                { Language.Portuguese, "Cancelar" },
                { Language.Russian, "Отмена" },
                { Language.Persian, "لغو" },
                { Language.Norwegian, "Avbryt" },
                { Language.Arabic, "إلغاء" }
            },
            ["TemplateNameEmpty"] = new Dictionary<Language, string>
            {
                { Language.English, "Template name cannot be empty." },
                { Language.SimplifiedChinese, "模板名称不能为空。" },
                { Language.TraditionalChinese, "模板名稱不能為空。" },
                { Language.Spanish, "El nombre de la plantilla no puede estar vacío." },
                { Language.French, "Le nom du modèle ne peut pas être vide." },
                { Language.Portuguese, "O nome do modelo não pode estar vazio." },
                { Language.Russian, "Имя шаблона не может быть пустым." },
                { Language.Persian, "نام قالب نمی‌تواند خالی باشد." },
                { Language.Norwegian, "Malnavn kan ikke være tomt." },
                { Language.Arabic, "لا يمكن أن يكون اسم القالب فارغاً." }
            },
            ["Error"] = new Dictionary<Language, string>
            {
                { Language.English, "Error" },
                { Language.SimplifiedChinese, "错误" },
                { Language.TraditionalChinese, "錯誤" },
                { Language.Spanish, "Error" },
                { Language.French, "Erreur" },
                { Language.Portuguese, "Erro" },
                { Language.Russian, "Ошибка" },
                { Language.Persian, "خطا" },
                { Language.Norwegian, "Feil" },
                { Language.Arabic, "خطأ" }
            },
            ["SaveToFolder"] = new Dictionary<Language, string>
            {
                { Language.English, "Save to \"{0}\"" },
                { Language.SimplifiedChinese, "保存到「{0}」" },
                { Language.TraditionalChinese, "保存到「{0}」" },
                { Language.Spanish, "Guardar en \"{0}\"" },
                { Language.French, "Enregistrer dans \"{0}\"" },
                { Language.Portuguese, "Salvar em \"{0}\"" },
                { Language.Russian, "Сохранить в \"{0}\"" },
                { Language.Persian, "ذخیره در «{0}»" },
                { Language.Norwegian, "Lagre i \"{0}\"" },
                { Language.Arabic, "حفظ في \"{0}\"" }
            },
            ["ConfirmDelete"] = new Dictionary<Language, string>
            {
                { Language.English, "Confirm Delete" },
                { Language.SimplifiedChinese, "确认删除" },
                { Language.TraditionalChinese, "確認刪除" },
                { Language.Spanish, "Confirmar eliminación" },
                { Language.French, "Confirmer la suppression" },
                { Language.Portuguese, "Confirmar exclusão" },
                { Language.Russian, "Подтвердите удаление" },
                { Language.Persian, "تأیید حذف" },
                { Language.Norwegian, "Bekreft sletting" },
                { Language.Arabic, "تأكيد الحذف" }
            },
            ["ConfirmDeleteFolder"] = new Dictionary<Language, string>
            {
                { Language.English, "Delete folder \"{0}\" and all its contents?" },
                { Language.SimplifiedChinese, "确定要删除文件夹「{0}」及其所有内容吗？" },
                { Language.TraditionalChinese, "確定要刪除資料夾「{0}」及其所有內容嗎？" },
                { Language.Spanish, "¿Eliminar la carpeta \"{0}\" y todo su contenido?" },
                { Language.French, "Supprimer le dossier \"{0}\" et tout son contenu ?" },
                { Language.Portuguese, "Excluir a pasta \"{0}\" e todo o conteúdo?" },
                { Language.Russian, "Удалить папку \"{0}\" и всё её содержимое?" },
                { Language.Persian, "پوشه «{0}» و همه محتوا حذف شود؟" },
                { Language.Norwegian, "Slette mappen \"{0}\" og alt innhold?" },
                { Language.Arabic, "حذف المجلد \"{0}\" وجميع محتوياته؟" }
            },
            ["ConfirmDeleteTemplate"] = new Dictionary<Language, string>
            {
                { Language.English, "Delete template \"{0}\"?" },
                { Language.SimplifiedChinese, "确定要删除模板「{0}」吗？" },
                { Language.TraditionalChinese, "確定要刪除模板「{0}」嗎？" },
                { Language.Spanish, "¿Eliminar la plantilla \"{0}\"?" },
                { Language.French, "Supprimer le modèle \"{0}\" ?" },
                { Language.Portuguese, "Excluir o modelo \"{0}\"?" },
                { Language.Russian, "Удалить шаблон \"{0}\"?" },
                { Language.Persian, "قالب «{0}» حذف شود؟" },
                { Language.Norwegian, "Slette malen \"{0}\"?" },
                { Language.Arabic, "حذف القالب \"{0}\"؟" }
            },
            ["NewFolder"] = new Dictionary<Language, string>
            {
                { Language.English, "New Folder" },
                { Language.SimplifiedChinese, "新文件夹" },
                { Language.TraditionalChinese, "新資料夾" },
                { Language.Spanish, "Nueva carpeta" },
                { Language.French, "Nouveau dossier" },
                { Language.Portuguese, "Nova pasta" },
                { Language.Russian, "Новая папка" },
                { Language.Persian, "پوشه جدید" },
                { Language.Norwegian, "Ny mappe" },
                { Language.Arabic, "مجلد جديد" }
            },
            ["ImageFileFilter"] = new Dictionary<Language, string>
            {
                { Language.English, "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.svg" },
                { Language.SimplifiedChinese, "图像文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.svg" },
                { Language.TraditionalChinese, "圖像文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.svg" },
                { Language.Spanish, "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.svg" },
                { Language.French, "Fichiers image|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.svg" },
                { Language.Portuguese, "Arquivos de imagem|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.svg" },
                { Language.Russian, "Файлы изображений|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.svg" },
                { Language.Persian, "فایل‌های تصویر|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.svg" },
                { Language.Norwegian, "Bildefiler|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.svg" },
                { Language.Arabic, "ملفات الصور|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.svg" }
            },
            ["SelectCustomDevice"] = new Dictionary<Language, string>
            {
                { Language.English, "Select Custom Device" },
                { Language.SimplifiedChinese, "选择自绘装置" },
                { Language.TraditionalChinese, "選擇自繪裝置" },
                { Language.Spanish, "Seleccionar dispositivo personalizado" },
                { Language.French, "Sélectionner un appareil personnalisé" },
                { Language.Portuguese, "Selecionar dispositivo personalizado" },
                { Language.Russian, "Выбрать пользовательское устройство" },
                { Language.Persian, "انتخاب دستگاه سفارشی" },
                { Language.Norwegian, "Velg egendefinert enhet" },
                { Language.Arabic, "اختيار جهاز مخصص" }
            },
            ["ImageNotExist"] = new Dictionary<Language, string>
            {
                { Language.English, "Image file does not exist." },
                { Language.SimplifiedChinese, "图片文件不存在。" },
                { Language.TraditionalChinese, "圖片文件不存在。" },
                { Language.Spanish, "El archivo de imagen no existe." },
                { Language.French, "Le fichier image n’existe pas." },
                { Language.Portuguese, "O arquivo de imagem não existe." },
                { Language.Russian, "Файл изображения не существует." },
                { Language.Persian, "فایل تصویر وجود ندارد." },
                { Language.Norwegian, "Bildefilen finnes ikke." },
                { Language.Arabic, "ملف الصورة غير موجود." }
            },
            ["DeleteFileFailed"] = new Dictionary<Language, string>
            {
                { Language.English, "Failed to delete file: {0}" },
                { Language.SimplifiedChinese, "删除文件失败：{0}" },
                { Language.TraditionalChinese, "刪除文件失敗：{0}" },
                { Language.Spanish, "Error al eliminar el archivo: {0}" },
                { Language.French, "Échec de la suppression du fichier : {0}" },
                { Language.Portuguese, "Falha ao excluir o arquivo: {0}" },
                { Language.Russian, "Не удалось удалить файл: {0}" },
                { Language.Persian, "حذف فایل ناموفق بود: {0}" },
                { Language.Norwegian, "Kunne ikke slette filen: {0}" },
                { Language.Arabic, "فشل حذف الملف: {0}" }
            },
            ["ErrorOccurred"] = new Dictionary<Language, string>
            {
                { Language.English, "Error occurred: {0}" },
                { Language.SimplifiedChinese, "发生错误：{0}" },
                { Language.TraditionalChinese, "發生錯誤：{0}" },
                { Language.Spanish, "Ocurrió un error: {0}" },
                { Language.French, "Une erreur s’est produite : {0}" },
                { Language.Portuguese, "Ocorreu um erro: {0}" },
                { Language.Russian, "Произошла ошибка: {0}" },
                { Language.Persian, "خطا رخ داد: {0}" },
                { Language.Norwegian, "En feil oppstod: {0}" },
                { Language.Arabic, "حدث خطأ: {0}" }
            },

            // ===== 設置對話框 =====
            ["SettingsTitle"] = new Dictionary<Language, string>
            {
                { Language.English, "Settings" },
                { Language.SimplifiedChinese, "设置" },
                { Language.TraditionalChinese, "設置" },
                { Language.Spanish, "Configuración" },
                { Language.French, "Paramètres" },
                { Language.Portuguese, "Configurações" },
                { Language.Russian, "Настройки" },
                { Language.Persian, "تنظیمات" },
                { Language.Norwegian, "Innstillinger" },
                { Language.Arabic, "إعدادات" }
            },
            ["LanguageLabel"] = new Dictionary<Language, string>
            {
                { Language.English, "Language:" },
                { Language.SimplifiedChinese, "语言 / Language：" },
                { Language.TraditionalChinese, "語言 / Language：" },
                { Language.Spanish, "Idioma:" },
                { Language.French, "Langue :" },
                { Language.Portuguese, "Idioma:" },
                { Language.Russian, "Язык:" },
                { Language.Persian, "زبان:" },
                { Language.Norwegian, "Språk:" },
                { Language.Arabic, "اللغة:" }
            },
            ["OK"] = new Dictionary<Language, string>
            {
                { Language.English, "OK" },
                { Language.SimplifiedChinese, "确定" },
                { Language.TraditionalChinese, "確定" },
                { Language.Spanish, "Aceptar" },
                { Language.French, "OK" },
                { Language.Portuguese, "OK" },
                { Language.Russian, "ОК" },
                { Language.Persian, "تأیید" },
                { Language.Norwegian, "OK" },
                { Language.Arabic, "موافق" }
            },

            // ===== HelpDialog =====
            ["HelpTitle"] = new Dictionary<Language, string>
            {
                { Language.English, "User Guide" },
                { Language.SimplifiedChinese, "操作说明" },
                { Language.TraditionalChinese, "操作說明" },
                { Language.Spanish, "Guía del usuario" },
                { Language.French, "Guide de l’utilisateur" },
                { Language.Portuguese, "Guia do usuário" },
                { Language.Russian, "Руководство пользователя" },
                { Language.Persian, "راهنمای کاربر" },
                { Language.Norwegian, "Brukerhåndbok" },
                { Language.Arabic, "دليل المستخدم" }
            },
            ["HelpNavHeader"] = new Dictionary<Language, string>
            {
                { Language.English, "Contents" },
                { Language.SimplifiedChinese, "目录" },
                { Language.TraditionalChinese, "目錄" },
                { Language.Spanish, "Contenido" },
                { Language.French, "Sommaire" },
                { Language.Portuguese, "Conteúdo" },
                { Language.Russian, "Содержание" },
                { Language.Persian, "فهرست" },
                { Language.Norwegian, "Innhold" },
                { Language.Arabic, "المحتويات" }
            },
            ["HelpNavQuick"] = new Dictionary<Language, string>
            {
                { Language.English, "Quick Start" },
                { Language.Spanish, "Inicio rápido" },
                { Language.French, "Démarrage rapide" },
                { Language.Portuguese, "Início rápido" },
                { Language.Russian, "Быстрый старт" },
                { Language.Persian, "شروع سریع" },
                { Language.Norwegian, "Hurtigstart" },
                { Language.TraditionalChinese, "快速開始" },
                { Language.SimplifiedChinese, "快速开始" },
                { Language.Arabic, "بدء سريع" }
            },
            ["HelpNavDevice"] = new Dictionary<Language, string>
            {
                { Language.English, "Devices" },
                { Language.Spanish, "Dispositivos" },
                { Language.French, "Appareils" },
                { Language.Portuguese, "Dispositivos" },
                { Language.Russian, "Устройства" },
                { Language.Persian, "دستگاه‌ها" },
                { Language.Norwegian, "Enheter" },
                { Language.TraditionalChinese, "裝置管理" },
                { Language.SimplifiedChinese, "装置管理" },
                { Language.Arabic, "الأجهزة" }
            },
            ["HelpNavDeviceSelect"] = new Dictionary<Language, string>
            {
                { Language.English, "Select & Place" },
                { Language.Spanish, "Seleccionar y colocar" },
                { Language.French, "Sélectionner et placer" },
                { Language.Portuguese, "Selecionar e posicionar" },
                { Language.Russian, "Выбор и размещение" },
                { Language.Persian, "انتخاب و قرار دادن" },
                { Language.Norwegian, "Velg og plasser" },
                { Language.TraditionalChinese, "選擇與放置裝置" },
                { Language.SimplifiedChinese, "选择与放置装置" },
                { Language.Arabic, "تحديد ووضع" }
            },
            ["HelpNavDevicePreview"] = new Dictionary<Language, string>
            {
                { Language.English, "Preview" },
                { Language.Spanish, "Vista previa" },
                { Language.French, "Aperçu" },
                { Language.Portuguese, "Pré-visualização" },
                { Language.Russian, "Предпросмотр" },
                { Language.Persian, "پیش‌نمایش" },
                { Language.Norwegian, "Forhåndsvisning" },
                { Language.TraditionalChinese, "裝置預覽" },
                { Language.SimplifiedChinese, "装置预览" },
                { Language.Arabic, "معاينة" }
            },
            ["HelpNavDeviceCustom"] = new Dictionary<Language, string>
            {
                { Language.English, "Custom Devices" },
                { Language.Spanish, "Dispositivos personalizados" },
                { Language.French, "Appareils personnalisés" },
                { Language.Portuguese, "Dispositivos personalizados" },
                { Language.Russian, "Пользовательские устройства" },
                { Language.Persian, "دستگاه‌های سفارشی" },
                { Language.Norwegian, "Egendefinerte enheter" },
                { Language.TraditionalChinese, "添加自定義裝置" },
                { Language.SimplifiedChinese, "添加自定义装置" },
                { Language.Arabic, "أجهزة مخصصة" }
            },
            ["HelpNavDeviceColor"] = new Dictionary<Language, string>
            {
                { Language.English, "Colored / Uncolored" },
                { Language.Spanish, "A color / sin color" },
                { Language.French, "En couleur / sans couleur" },
                { Language.Portuguese, "Colorido / sem cor" },
                { Language.Russian, "Цветные / без цвета" },
                { Language.Persian, "رنگی / بدون رنگ" },
                { Language.Norwegian, "Farget / ufarget" },
                { Language.TraditionalChinese, "上色/未上色切換" },
                { Language.SimplifiedChinese, "上色/未上色切换" },
                { Language.Arabic, "ملون / غير ملون" }
            },
            ["HelpNavTags"] = new Dictionary<Language, string>
            {
                { Language.English, "Labels" },
                { Language.Spanish, "Etiquetas" },
                { Language.French, "Étiquettes" },
                { Language.Portuguese, "Rótulos" },
                { Language.Russian, "Метки" },
                { Language.Persian, "برچسب‌ها" },
                { Language.Norwegian, "Etiketter" },
                { Language.TraditionalChinese, "標籤管理" },
                { Language.SimplifiedChinese, "标签管理" },
                { Language.Arabic, "الملصقات" }
            },
            ["HelpNavTagCreate"] = new Dictionary<Language, string>
            {
                { Language.English, "Create & Edit" },
                { Language.Spanish, "Crear y editar" },
                { Language.French, "Créer et modifier" },
                { Language.Portuguese, "Criar e editar" },
                { Language.Russian, "Создать и редактировать" },
                { Language.Persian, "ایجاد و ویرایش" },
                { Language.Norwegian, "Opprett og rediger" },
                { Language.TraditionalChinese, "創建與編輯標籤" },
                { Language.SimplifiedChinese, "创建与编辑标签" },
                { Language.Arabic, "إنشاء وتحرير" }
            },
            ["HelpNavTagPlace"] = new Dictionary<Language, string>
            {
                { Language.English, "Place Text" },
                { Language.Spanish, "Colocar texto" },
                { Language.French, "Placer le texte" },
                { Language.Portuguese, "Posicionar texto" },
                { Language.Russian, "Разместить текст" },
                { Language.Persian, "قراردادن متن" },
                { Language.Norwegian, "Plasser tekst" },
                { Language.TraditionalChinese, "放置文字標籤" },
                { Language.SimplifiedChinese, "放置文字标签" },
                { Language.Arabic, "وضع النص" }
            },
            ["HelpNavTagSearch"] = new Dictionary<Language, string>
            {
                { Language.English, "Search Labels" },
                { Language.Spanish, "Buscar etiquetas" },
                { Language.French, "Rechercher des étiquettes" },
                { Language.Portuguese, "Pesquisar rótulos" },
                { Language.Russian, "Поиск меток" },
                { Language.Persian, "جستجوی برچسب‌ها" },
                { Language.Norwegian, "Søk i etiketter" },
                { Language.TraditionalChinese, "搜索標籤" },
                { Language.SimplifiedChinese, "搜索标签" },
                { Language.Arabic, "بحث التسميات" }
            },
            ["HelpNavCanvas"] = new Dictionary<Language, string>
            {
                { Language.English, "Canvas" },
                { Language.Spanish, "Lienzo" },
                { Language.French, "Toile" },
                { Language.Portuguese, "Tela" },
                { Language.Russian, "Холст" },
                { Language.Persian, "بوم" },
                { Language.Norwegian, "Lerret" },
                { Language.TraditionalChinese, "畫布操作" },
                { Language.SimplifiedChinese, "画布操作" },
                { Language.Arabic, "اللوحة" }
            },
            ["HelpNavCanvasMove"] = new Dictionary<Language, string>
            {
                { Language.English, "Move & Zoom" },
                { Language.Spanish, "Mover y acercar" },
                { Language.French, "Déplacer et zoomer" },
                { Language.Portuguese, "Mover e ampliar" },
                { Language.Russian, "Перемещение и масштаб" },
                { Language.Persian, "جابجایی و بزرگ‌نمایی" },
                { Language.Norwegian, "Flytt og zoom" },
                { Language.TraditionalChinese, "移動與縮放" },
                { Language.SimplifiedChinese, "移动与缩放" },
                { Language.Arabic, "تحريك وتكبير" }
            },
            ["HelpNavCanvasDelete"] = new Dictionary<Language, string>
            {
                { Language.English, "Delete Items" },
                { Language.Spanish, "Eliminar elementos" },
                { Language.French, "Supprimer des éléments" },
                { Language.Portuguese, "Excluir itens" },
                { Language.Russian, "Удалить элементы" },
                { Language.Persian, "حذف عناصر" },
                { Language.Norwegian, "Slett elementer" },
                { Language.TraditionalChinese, "刪除控件" },
                { Language.SimplifiedChinese, "删除控件" },
                { Language.Arabic, "حذف العناصر" }
            },
            ["HelpNavCanvasAlign"] = new Dictionary<Language, string>
            {
                { Language.English, "Auto Align" },
                { Language.Spanish, "Alinear automáticamente" },
                { Language.French, "Alignement automatique" },
                { Language.Portuguese, "Alinhar automaticamente" },
                { Language.Russian, "Автовыравнивание" },
                { Language.Persian, "تراز خودکار" },
                { Language.Norwegian, "Automatisk justering" },
                { Language.TraditionalChinese, "自動對齊" },
                { Language.SimplifiedChinese, "自动对齐" },
                { Language.Arabic, "محاذاة تلقائية" }
            },
            ["HelpNavCanvasClear"] = new Dictionary<Language, string>
            {
                { Language.English, "Clear Canvas" },
                { Language.Spanish, "Limpiar lienzo" },
                { Language.French, "Effacer la toile" },
                { Language.Portuguese, "Limpar tela" },
                { Language.Russian, "Очистить холст" },
                { Language.Persian, "پاک کردن بوم" },
                { Language.Norwegian, "Tøm lerret" },
                { Language.TraditionalChinese, "清空畫布" },
                { Language.SimplifiedChinese, "清空画布" },
                { Language.Arabic, "مسح اللوحة" }
            },
            ["HelpNavTemplate"] = new Dictionary<Language, string>
            {
                { Language.English, "Templates" },
                { Language.Spanish, "Plantillas" },
                { Language.French, "Modèles" },
                { Language.Portuguese, "Modelos" },
                { Language.Russian, "Шаблоны" },
                { Language.Persian, "قالب‌ها" },
                { Language.Norwegian, "Maler" },
                { Language.TraditionalChinese, "模板庫" },
                { Language.SimplifiedChinese, "模板库" },
                { Language.Arabic, "القوالب" }
            },
            ["HelpNavTemplateSave"] = new Dictionary<Language, string>
            {
                { Language.English, "Save Template" },
                { Language.Spanish, "Guardar plantilla" },
                { Language.French, "Enregistrer le modèle" },
                { Language.Portuguese, "Salvar modelo" },
                { Language.Russian, "Сохранить шаблон" },
                { Language.Persian, "ذخیره قالب" },
                { Language.Norwegian, "Lagre mal" },
                { Language.TraditionalChinese, "保存模板" },
                { Language.SimplifiedChinese, "保存模板" },
                { Language.Arabic, "حفظ القالب" }
            },
            ["HelpNavTemplateLoad"] = new Dictionary<Language, string>
            {
                { Language.English, "Load Template" },
                { Language.Spanish, "Cargar plantilla" },
                { Language.French, "Charger le modèle" },
                { Language.Portuguese, "Carregar modelo" },
                { Language.Russian, "Загрузить шаблон" },
                { Language.Persian, "بارگذاری قالب" },
                { Language.Norwegian, "Last mal" },
                { Language.TraditionalChinese, "載入模板" },
                { Language.SimplifiedChinese, "加载模板" },
                { Language.Arabic, "تحميل القالب" }
            },
            ["HelpNavTemplateFolder"] = new Dictionary<Language, string>
            {
                { Language.English, "Manage Folders" },
                { Language.Spanish, "Administrar carpetas" },
                { Language.French, "Gérer les dossiers" },
                { Language.Portuguese, "Gerenciar pastas" },
                { Language.Russian, "Управление папками" },
                { Language.Persian, "مدیریت پوشه‌ها" },
                { Language.Norwegian, "Administrer mapper" },
                { Language.TraditionalChinese, "管理資料夾" },
                { Language.SimplifiedChinese, "管理文件夹" },
                { Language.Arabic, "إدارة المجلدات" }
            },
            ["HelpNavTemplateSearch"] = new Dictionary<Language, string>
            {
                { Language.English, "Search Templates" },
                { Language.Spanish, "Buscar plantillas" },
                { Language.French, "Rechercher des modèles" },
                { Language.Portuguese, "Pesquisar modelos" },
                { Language.Russian, "Поиск шаблонов" },
                { Language.Persian, "جستجوی قالب‌ها" },
                { Language.Norwegian, "Søk i maler" },
                { Language.TraditionalChinese, "搜索模板" },
                { Language.SimplifiedChinese, "搜索模板" },
                { Language.Arabic, "بحث القوالب" }
            },
            ["HelpNavCapture"] = new Dictionary<Language, string>
            {
                { Language.English, "Screenshots" },
                { Language.Spanish, "Capturas de pantalla" },
                { Language.French, "Captures d’écran" },
                { Language.Portuguese, "Capturas de tela" },
                { Language.Russian, "Снимки экрана" },
                { Language.Persian, "اسکرین‌شات‌ها" },
                { Language.Norwegian, "Skjermbilder" },
                { Language.TraditionalChinese, "截圖功能" },
                { Language.SimplifiedChinese, "截图功能" },
                { Language.Arabic, "لقطات الشاشة" }
            },
            ["HelpNavCaptureManual"] = new Dictionary<Language, string>
            {
                { Language.English, "Manual Capture" },
                { Language.Spanish, "Captura manual" },
                { Language.French, "Capture manuelle" },
                { Language.Portuguese, "Captura manual" },
                { Language.Russian, "Ручной снимок" },
                { Language.Persian, "گرفتن دستی" },
                { Language.Norwegian, "Manuell fangst" },
                { Language.TraditionalChinese, "手動截圖" },
                { Language.SimplifiedChinese, "手动截图" },
                { Language.Arabic, "لقطة يدوية" }
            },
            ["HelpNavCaptureAuto"] = new Dictionary<Language, string>
            {
                { Language.English, "Auto Capture" },
                { Language.Spanish, "Captura automática" },
                { Language.French, "Capture automatique" },
                { Language.Portuguese, "Captura automática" },
                { Language.Russian, "Автоснимок" },
                { Language.Persian, "گرفتن خودکار" },
                { Language.Norwegian, "Automatisk fangst" },
                { Language.TraditionalChinese, "自動截圖" },
                { Language.SimplifiedChinese, "自动截图" },
                { Language.Arabic, "لقطة تلقائية" }
            },
            ["HelpNavCaptureLong"] = new Dictionary<Language, string>
            {
                { Language.English, "Long Screenshot" },
                { Language.Spanish, "Captura larga" },
                { Language.French, "Capture longue" },
                { Language.Portuguese, "Captura longa" },
                { Language.Russian, "Длинный скриншот" },
                { Language.Persian, "اسکرین‌شات بلند" },
                { Language.Norwegian, "Langt skjermbilde" },
                { Language.TraditionalChinese, "長圖截取" },
                { Language.SimplifiedChinese, "长图截取" },
                { Language.Arabic, "لقطة طويلة" }
            },
            // ===== JSON 匯入幫助 =====
            ["HelpNavJsonImport"] = new Dictionary<Language, string>
            {
                { Language.English, "JSON Import" },
                { Language.Spanish, "Importar JSON" },
                { Language.French, "Import JSON" },
                { Language.Portuguese, "Importar JSON" },
                { Language.Russian, "Импорт JSON" },
                { Language.Persian, "ورود JSON" },
                { Language.Norwegian, "JSON-import" },
                { Language.TraditionalChinese, "JSON 匯入" },
                { Language.SimplifiedChinese, "JSON 导入" },
                { Language.Arabic, "استيراد JSON" }
            },
            ["HelpNavJsonUsage"] = new Dictionary<Language, string>
            {
                { Language.English, "How to Use" },
                { Language.Spanish, "Cómo usar" },
                { Language.French, "Comment utiliser" },
                { Language.Portuguese, "Como usar" },
                { Language.Russian, "Как использовать" },
                { Language.Persian, "نحوه استفاده" },
                { Language.Norwegian, "Slik bruker du" },
                { Language.TraditionalChinese, "使用方法" },
                { Language.SimplifiedChinese, "使用方法" },
                { Language.Arabic, "كيفية الاستخدام" }
            },
            ["HelpNavJsonSchema"] = new Dictionary<Language, string>
            {
                { Language.English, "Copy Schema Rules" },
                { Language.Spanish, "Copiar reglas" },
                { Language.French, "Copier les règles" },
                { Language.Portuguese, "Copiar regras" },
                { Language.Russian, "Копировать правила" },
                { Language.Persian, "کپی قوانین" },
                { Language.Norwegian, "Kopier regler" },
                { Language.TraditionalChinese, "複製 JSON 規則" },
                { Language.SimplifiedChinese, "复制 JSON 规则" },
                { Language.Arabic, "نسخ القواعد" }
            },
            ["HelpNavSample"] = new Dictionary<Language, string>
            {
                { Language.English, "Samples" },
                { Language.Spanish, "Muestras" },
                { Language.French, "Exemples" },
                { Language.Portuguese, "Amostras" },
                { Language.Russian, "Примеры" },
                { Language.Persian, "نمونه‌ها" },
                { Language.Norwegian, "Eksempler" },
                { Language.TraditionalChinese, "裝置樣例" },
                { Language.SimplifiedChinese, "装置样例" },
                { Language.Arabic, "عينات" }
            },
            ["HelpNavData"] = new Dictionary<Language, string>
            {
                { Language.English, "Data Files" },
                { Language.Spanish, "Archivos de datos" },
                { Language.French, "Fichiers de données" },
                { Language.Portuguese, "Arquivos de dados" },
                { Language.Russian, "Файлы данных" },
                { Language.Persian, "فایل‌های داده" },
                { Language.Norwegian, "Datafiler" },
                { Language.TraditionalChinese, "數據說明" },
                { Language.SimplifiedChinese, "数据说明" },
                { Language.Arabic, "ملفات البيانات" }
            },
            // ===== 幫助內容（合併版） =====
            ["HelpContentQuick"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "【快速開始】\r\n\r\n本軟件用於繪製井口裝置示意圖，以下是基本操作流程：\r\n\r\n第一步：放置裝置\r\n• 在左側「井口裝置選擇」列表中點擊選中一個裝置\r\n• 將鼠標移到右側畫布，點擊左鍵放置裝置\r\n• 可重複點擊放置多個相同裝置\r\n\r\n第二步：添加文字標籤\r\n• 在中間「標籤管理」樹中點擊選中一個標籤\r\n• 將鼠標移到畫布，點擊左鍵放置文字\r\n\r\n第三步：調整位置和大小\r\n• 左鍵拖動：移動裝置或文字的位置\r\n• 滾輪滾動：放大或縮小裝置或文字\r\n• 右鍵菜單「自動對齊」：一鍵整理排列\r\n\r\n第四步：保存或截圖\r\n• 右鍵菜單「添加樣例到庫」：保存為模板\r\n• 右鍵菜單「保存到目前模板」：覆蓋保存已載入的模板\r\n• 右鍵菜單「自動截圖」：一鍵截圖到剪貼簿\r\n• 右鍵菜單「匯入 JSON」：通過 AI 生成的 JSON 快速建圖\r\n\r\n提示：畫布上右鍵點擊空白處或控件均可打開功能菜單。在「設置」中可切換語言與上色/未上色裝置樣式。" },
                { Language.SimplifiedChinese, "【快速开始】\r\n\r\n本软件用于绘制井口装置示意图，以下是基本操作流程：\r\n\r\n第一步：放置装置\r\n• 在左侧「井口装置选择」列表中点击选中一个装置\r\n• 将鼠标移到右侧画布，点击左键放置装置\r\n• 可重复点击放置多个相同装置\r\n\r\n第二步：添加文字标签\r\n• 在中间「标签管理」树中点击选中一个标签\r\n• 将鼠标移到画布，点击左键放置文字\r\n\r\n第三步：调整位置和大小\r\n• 左键拖动：移动装置或文字的位置\r\n• 滚轮滚动：放大或缩小装置或文字\r\n• 右键菜单「自动对齐」：一键整理排列\r\n\r\n第四步：保存或截图\r\n• 右键菜单「添加样例到库」：保存为模板\r\n• 右键菜单「保存到当前模板」：覆盖保存已加载的模板\r\n• 右键菜单「自动截图」：一键截图到剪贴板\r\n• 右键菜单「导入 JSON」：通过 AI 生成的 JSON 快速建图\r\n\r\n提示：画布上右键点击空白处或控件均可打开功能菜单。在「设置」中可切换语言与上色/未上色装置样式。" },
                { Language.English, "[Quick Start]\r\n\r\nThis software is used to draw wellhead device diagrams. Basic workflow:\r\n\r\nStep 1: Place a device\r\n• Click a device name in the left Device List\r\n• Move to the canvas and left-click to place it\r\n• Click multiple times to place copies\r\n\r\nStep 2: Add a text label\r\n• Click a label in the Tag Tree\r\n• Move to the canvas and left-click to place the text\r\n\r\nStep 3: Adjust position and size\r\n• Drag with left button to move\r\n• Scroll wheel to resize\r\n• Right-click menu > Auto Align to arrange automatically\r\n\r\nStep 4: Save or capture\r\n• Right-click > Add to Library to save as template\r\n• Right-click > Save to Current Template to overwrite a loaded template\r\n• Right-click > Auto Screenshot to copy to clipboard\r\n• Right-click > Import JSON to build diagrams from AI-generated JSON\r\n\r\nTip: Right-click on the canvas (blank area or any element) to open the context menu. Open Settings to switch language and colored/uncolored device style." },
                { Language.Spanish, "[Inicio rápido]\r\n\r\nEste software dibuja diagramas de equipos de cabeza de pozo.\r\n\r\n1. Seleccione un dispositivo en la lista izquierda y haga clic en el lienzo para colocarlo\r\n2. Seleccione una etiqueta y haga clic en el lienzo para colocar texto\r\n3. Arrastre para mover, rueda para redimensionar\r\n4. Clic derecho: Alinear, Captura, Añadir a biblioteca, Guardar en plantilla actual, Importar JSON\r\n5. Abra Configuración para cambiar idioma y estilo con/sin color" },
                { Language.French, "[Démarrage rapide]\r\n\r\nCe logiciel dessine des schémas d'équipements de tête de puits.\r\n\r\n1. Sélectionnez un appareil dans la liste de gauche et cliquez sur la toile pour le placer\r\n2. Sélectionnez une étiquette et cliquez sur la toile pour placer le texte\r\n3. Glissez pour déplacer, molette pour redimensionner\r\n4. Clic droit : Aligner, Capturer, Ajouter à la bibliothèque, Enregistrer dans le modèle actuel, Importer JSON\r\n5. Ouvrez Paramètres pour la langue et le style coloré/non coloré" },
                { Language.Portuguese, "[Início rápido]\r\n\r\nEste software desenha diagramas de equipamentos de cabeça de poço.\r\n\r\n1. Selecione um dispositivo na lista esquerda e clique na tela para posicionar\r\n2. Selecione um rótulo e clique na tela para colocar texto\r\n3. Arraste para mover, roda para redimensionar\r\n4. Clique direito: Alinhar, Capturar, Adicionar à biblioteca, Salvar no modelo atual, Importar JSON\r\n5. Abra Configurações para idioma e estilo colorido/sem cor" },
                { Language.Russian, "[Быстрый старт]\r\n\r\nПрограмма для чертежа устьевого оборудования.\r\n\r\n1. Выберите устройство в списке слева и щёлкните по холсту для размещения\r\n2. Выберите метку и щёлкните по холсту для размещения текста\r\n3. Перетаскивайте для перемещения, колёсико для масштабирования\r\n4. ПКМ: Выравнивание, Снимок, Добавить в библиотеку, Сохранить в текущий шаблон, Импорт JSON\r\n5. В Настройках — язык и стиль цветной/без цвета" },
                { Language.Persian, "[شروع سریع]\r\n\r\nاین نرم‌افزار برای ترسیم تجهیزات سرچاه استفاده می‌شود.\r\n\r\n1. یک دستگاه را از فهرست سمت چپ انتخاب کنید و روی بوم کلیک کنید\r\n2. یک برچسب انتخاب کنید و روی بوم کلیک کنید\r\n3. برای جابجایی بکشید، برای تغییر اندازه اسکرول کنید\r\n4. راست‌کلیک: تراز، اسکرین‌شات، افزودن به کتابخانه، ذخیره در قالب فعلی، ورود JSON\r\n5. در تنظیمات زبان و سبک رنگی/بدون رنگ را تغییر دهید" },
                { Language.Norwegian, "[Hurtigstart]\r\n\r\nDenne programvaren tegner brønnhodeutstyrsdiagrammer.\r\n\r\n1. Velg en enhet fra listen til venstre og klikk på lerretet for å plassere\r\n2. Velg en etikett og klikk på lerretet for å plassere tekst\r\n3. Dra for å flytte, rull for å endre størrelse\r\n4. Høyreklikk: Juster, Skjermbilde, Legg til i bibliotek, Lagre til gjeldende mal, Importer JSON\r\n5. Åpne Innstillinger for språk og farget/ufarget enhetsstil" },
                { Language.Arabic, "[بدء سريع]\r\n\r\nهذا البرنامج لرسم مخططات معدات رأس البئر.\r\n\r\n1. اختر جهازاً من القائمة اليسرى وانقر على اللوحة\r\n2. اختر تسمية وانقر على اللوحة\r\n3. اسحب للتحريك، مرر العجلة لتغيير الحجم\r\n4. انقر يمين: محاذاة، لقطة، إضافة للمكتبة، حفظ في القالب الحالي، استيراد JSON\r\n5. افتح الإعدادات للغة ونمط ملون/غير ملون" }
            },
            ["HelpContentDevice"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "【裝置選擇與放置】\r\n\r\n選擇裝置：\r\n• 在左側「井口裝置選擇」列表中單擊選中裝置\r\n• 選中後鼠標會顯示裝置預覽圖\r\n• 移開鼠標後預覽圖自動消失\r\n\r\n放置裝置：\r\n• 將鼠標移到右側畫布區域\r\n• 在想要的位置點擊左鍵即可放置\r\n• 可連續點擊放置多個相同裝置\r\n\r\n取消選擇：\r\n• 在畫布空白處點擊右鍵\r\n• 或點擊其他區域（標籤樹、模板庫等）\r\n\r\n添加自定義裝置：\r\n• 在裝置列表空白處或任意位置點擊右鍵\r\n• 選擇「添加自繪裝置」，選擇圖片文件\r\n• 支持 JPG、PNG、BMP、GIF、SVG 格式\r\n• 刪除自定義裝置：右鍵點擊選擇「刪除當前裝置」\r\n\r\n上色/未上色切換：\r\n• 打開「設置」，在裝置樣式下拉框中選擇\r\n• 彩色版本適合演示彙報，線條版本適合技術文檔\r\n\r\n注意：此設置影響新放置的裝置，已放置的裝置不會改變。\r\n提示：建議自定義裝置使用透明背景的 PNG 或 SVG 格式。" },
                { Language.SimplifiedChinese, "【装置选择与放置】\r\n\r\n选择装置：\r\n• 在左侧「井口装置选择」列表中单击选中装置\r\n• 选中后鼠标会显示装置预览图\r\n• 移开鼠标后预览图自动消失\r\n\r\n放置装置：\r\n• 将鼠标移到右侧画布区域\r\n• 在想要的位置点击左键即可放置\r\n• 可连续点击放置多个相同装置\r\n\r\n取消选择：\r\n• 在画布空白处点击右键\r\n• 或点击其他区域（标签树、模板库等）\r\n\r\n添加自定义装置：\r\n• 在装置列表空白处点击右键\r\n• 选择「添加自绘装置」，选择图片文件\r\n• 支持 JPG、PNG、BMP、GIF、SVG 格式\r\n• 删除：右键点击选择「删除当前装置」\r\n\r\n上色/未上色切换：\r\n• 打开「设置」，在装置样式下拉框中选择\r\n• 彩色版本适合演示汇报，线条版本适合技术文档\r\n\r\n注意：此设置影响新放置的装置，已放置的不会改变。\r\n提示：建议自定义装置使用透明背景的 PNG 或 SVG 格式。" },
                { Language.English, "[Device Selection & Placement]\r\n\r\nSelect a device:\r\n• Click a device name in the left Device List\r\n• A preview image follows your cursor\r\n• The preview disappears when you move away\r\n\r\nPlace a device:\r\n• Move your cursor to the canvas\r\n• Left-click at the desired position to place\r\n• Click multiple times to place copies\r\n\r\nCancel selection:\r\n• Right-click on the canvas\r\n• Or click another area (tag tree, template library, etc.)\r\n\r\nCustom devices:\r\n• Right-click in the Device List > Add Custom Device\r\n• Select an image file (JPG, PNG, BMP, GIF, SVG)\r\n• Delete: right-click the custom device > Delete Current Device\r\n\r\nColored / Uncolored style:\r\n• Open Settings and change Device Style\r\n• Colored: suitable for presentations; Uncolored: suitable for technical documents\r\n\r\nNote: This setting affects newly placed devices only.\r\nTip: PNG or SVG with transparent background is recommended for custom devices." },
                { Language.Spanish, "[Selección y colocación de dispositivos]\r\n\r\nSeleccionar：Haga clic en un dispositivo en la lista izquierda; se muestra una vista previa.\r\nColocar：Mueva el cursor al lienzo y haga clic izquierdo.\r\nCancelar：Clic derecho en el lienzo o haga clic en otra área.\r\n\r\nDispositivos personalizados：\r\n• Clic derecho en la lista > Agregar dispositivo personalizado\r\n• Formatos: JPG, PNG, BMP, GIF, SVG\r\n• Eliminar: clic derecho > Eliminar dispositivo actual\r\n\r\nEstilo con/sin color：Configuración > Estilo del dispositivo.\r\nNota: Solo afecta a los nuevos dispositivos colocados." },
                { Language.French, "[Sélection et placement des appareils]\r\n\r\nSélectionner：Cliquez sur un appareil dans la liste; un aperçu s'affiche.\r\nPlacer：Déplacez le curseur sur la toile et cliquez avec le bouton gauche.\r\nAnnuler：Clic droit sur la toile ou cliquez sur une autre zone.\r\n\r\nAppareils personnalisés：\r\n• Clic droit dans la liste > Ajouter un appareil personnalisé\r\n• Formats : JPG, PNG, BMP, GIF, SVG\r\n• Supprimer : clic droit > Supprimer l'appareil actuel\r\n\r\nStyle coloré/non coloré：Paramètres > Style d'appareil.\r\nNote : N'affecte que les nouveaux éléments placés." },
                { Language.Portuguese, "[Seleção e posicionamento de dispositivos]\r\n\r\nSelecionar：Clique em um dispositivo na lista; uma pré-visualização é exibida.\r\nColocar：Mova o cursor para a tela e clique com o botão esquerdo.\r\nCancelar：Clique direito na tela ou clique em outra área.\r\n\r\nDispositivos personalizados：\r\n• Clique direito na lista > Adicionar dispositivo personalizado\r\n• Formatos: JPG, PNG, BMP, GIF, SVG\r\n• Excluir: clique direito > Excluir dispositivo atual\r\n\r\nEstilo colorido/sem cor：Configurações > Estilo do dispositivo.\r\nNota: Afeta apenas novos dispositivos posicionados." },
                { Language.Russian, "[Выбор и размещение устройств]\r\n\r\nВыбор：Щёлкните по устройству в списке; появится предпросмотр.\r\nРазмещение：Наведите на холст и щёлкните ЛКМ.\r\nОтмена：ПКМ по холсту или щёлкните другую область.\r\n\r\nПользовательские устройства：\r\n• ПКМ в списке > Добавить пользовательское устройство\r\n• Форматы: JPG, PNG, BMP, GIF, SVG\r\n• Удалить: ПКМ > Удалить текущее устройство\r\n\r\nСтиль цветной/без цвета：Настройки > Стиль устройства.\r\nПримечание: Влияет только на новые размещённые элементы." },
                { Language.Persian, "[انتخاب و قراردادن دستگاه]\r\n\r\nانتخاب：روی دستگاه در فهرست کلیک کنید؛ پیش‌نمایش نمایش داده می‌شود.\r\nقراردادن：نشانگر را به بوم ببرید و کلیک چپ کنید.\r\nلغو：راست‌کلیک روی بوم یا ناحیه دیگر.\r\n\r\nدستگاه‌های سفارشی：\r\n• راست‌کلیک > افزودن دستگاه سفارشی\r\n• فرمت‌ها: JPG, PNG, BMP, GIF, SVG\r\n• حذف: راست‌کلیک > حذف دستگاه فعلی\r\n\r\nسبک رنگی/بدون رنگ：تنظیمات > سبک دستگاه.\r\nتوجه: فقط دستگاه‌های جدید تأثیر می‌پذیرند." },
                { Language.Norwegian, "[Valg og plassering av enheter]\r\n\r\nVelg：Klikk på en enhet i listen; forhåndsvisning vises.\r\nPlasser：Flytt markøren til lerretet og venstreklikk.\r\nAvbryt：Høyreklikk på lerretet eller klikk et annet område.\r\n\r\nEgendefinerte enheter：\r\n• Høyreklikk > Legg til egendefinert enhet\r\n• Formater: JPG, PNG, BMP, GIF, SVG\r\n• Slett: høyreklikk > Slett gjeldende enhet\r\n\r\nFarget/ufarget stil：Innstillinger > Enhetsstil.\r\nMerk: Påvirker kun nye plasserte enheter." },
                { Language.Arabic, "[اختيار ووضع الأجهزة]\r\n\r\nاختيار：انقر على جهاز في القائمة؛ تظهر معاينة.\r\nوضع：حرك المؤشر إلى اللوحة وانقر بالزر الأيسر.\r\nإلغاء：انقر بالزر الأيمن أو انقر منطقة أخرى.\r\n\r\nأجهزة مخصصة：\r\n• انقر يمين > إضافة جهاز مخصص\r\n• الصيغ: JPG, PNG, BMP, GIF, SVG\r\n• حذف: انقر يمين > حذف الجهاز الحالي\r\n\r\nنمط ملون/غير ملون：الإعدادات > نمط الجهاز.\r\nملاحظة: يؤثر فقط على العناصر الجديدة." }
            },
            ["HelpContentTags"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "【標籤管理】\r\n\r\n標籤用於在畫布上添加文字說明。\r\n\r\n添加標籤：\r\n• 在標籤樹空白處點擊右鍵，選擇「添加根節點」\r\n• 右鍵點擊已有標籤，選擇「添加子節點」\r\n• 輸入名稱後按 Enter 確認\r\n\r\n重命名/刪除：\r\n• 右鍵點擊標籤，選擇「重命名」或「刪除當前節點」\r\n• 重命名按 Enter 確認，按 Esc 取消\r\n\r\n放置文字：\r\n• 在標籤樹中單擊選中一個標籤\r\n• 將鼠標移到畫布上，在想要的位置點擊左鍵\r\n• 拖動可移動文字位置，滾輪可縮放\r\n• 右鍵點擊文字可選擇「刪除」\r\n\r\n搜索標籤：\r\n• 在標籤樹上方的搜索框中輸入關鍵字\r\n• 即時過濾顯示匹配的標籤，不區分大小寫\r\n• 清空搜索框恢復全部顯示\r\n\r\n提示：標籤支持多層級結構，方便分類管理。懸停在標籤上會顯示完整名稱提示框。" },
                { Language.SimplifiedChinese, "【标签管理】\r\n\r\n标签用于在画布上添加文字说明。\r\n\r\n添加标签：\r\n• 在标签树空白处点击右键，选择「添加根节点」\r\n• 右键点击已有标签，选择「添加子节点」\r\n• 输入名称后按 Enter 确认\r\n\r\n重命名/删除：\r\n• 右键点击标签，选择「重命名」或「删除当前节点」\r\n• 重命名按 Enter 确认，按 Esc 取消\r\n\r\n放置文字：\r\n• 在标签树中单击选中一个标签\r\n• 将鼠标移到画布上，在想要的位置点击左键\r\n• 拖动可移动文字位置，滚轮可缩放\r\n• 右键点击文字可选择「删除」\r\n\r\n搜索标签：\r\n• 在标签树上方的搜索框中输入关键字\r\n• 即时过滤显示匹配的标签，不区分大小写\r\n• 清空搜索框恢复全部显示\r\n\r\n提示：标签支持多层级结构，方便分类管理。" },
                { Language.English, "[Tag Management]\r\n\r\nTags are used to add text labels on the canvas.\r\n\r\nAdd tags:\r\n• Right-click in the Tag Tree blank area > Add Root Node\r\n• Right-click an existing node > Add Child Node\r\n• Type the name and press Enter to confirm\r\n\r\nRename / Delete:\r\n• Right-click a tag > Rename or Delete Current Node\r\n• Press Enter to confirm rename, Esc to cancel\r\n\r\nPlace text:\r\n• Click a tag in the Tag Tree to select it\r\n• Move to the canvas and left-click to place the text\r\n• Drag to move, scroll wheel to resize\r\n• Right-click text > Delete to remove\r\n\r\nSearch:\r\n• Type in the search box above the Tag Tree\r\n• Labels are filtered in real time, case-insensitive\r\n• Clear the search box to restore all labels\r\n\r\nTip: Tags support multiple levels for organized management." },
                { Language.Spanish, "[Gestión de etiquetas]\r\n\r\nAgregar：Clic derecho en el árbol > Agregar nodo raíz / Agregar nodo hijo. Enter para confirmar.\r\nRenombrar/Eliminar：Clic derecho > Renombrar o Eliminar nodo actual.\r\n\r\nColocar texto：Seleccione una etiqueta, luego clic izquierdo en el lienzo.\r\n• Arrastre para mover, rueda para redimensionar, clic derecho > Eliminar.\r\n\r\nBúsqueda：Escriba en el cuadro de búsqueda para filtrar (no distingue mayúsculas)." },
                { Language.French, "[Gestion des étiquettes]\r\n\r\nAjouter：Clic droit dans l'arbre > Ajouter un nœud racine / Ajouter un nœud enfant. Entrée pour confirmer.\r\nRenommer/Supprimer：Clic droit > Renommer ou Supprimer le nœud actuel.\r\n\r\nPlacer du texte：Sélectionnez une étiquette, puis cliquez sur la toile.\r\n• Glissez pour déplacer, molette pour redimensionner, clic droit > Supprimer.\r\n\r\nRecherche：Tapez dans la zone de recherche pour filtrer (insensible à la casse)." },
                { Language.Portuguese, "[Gerenciamento de rótulos]\r\n\r\nAdicionar：Clique direito na árvore > Adicionar nó raiz / nó filho. Enter para confirmar.\r\nRenomear/Excluir：Clique direito > Renomear ou Excluir nó atual.\r\n\r\nColocar texto：Selecione um rótulo e clique na tela.\r\n• Arraste para mover, roda para redimensionar, clique direito > Excluir.\r\n\r\nPesquisa：Digite na caixa para filtrar (não diferencia maiúsculas)." },
                { Language.Russian, "[Управление метками]\r\n\r\nДобавить：ПКМ в дереве > Добавить корневой/дочерний узел. Enter для подтверждения.\r\nПереименовать/Удалить：ПКМ > Переименовать или Удалить текущий узел.\r\n\r\nРазместить текст：Выберите метку, затем щёлкните ЛКМ по холсту.\r\n• Перетаскивание для перемещения, колёсико для масштабирования, ПКМ > Удалить.\r\n\r\nПоиск：Введите в поле поиска для фильтрации (без учёта регистра)." },
                { Language.Persian, "[مدیریت برچسب‌ها]\r\n\r\nافزودن：راست‌کلیک در درخت > افزودن گره ریشه / گره فرزند. Enter برای تأیید.\r\nتغییرنام/حذف：راست‌کلیک > تغییرنام یا حذف گره فعلی.\r\n\r\nقراردادن متن：یک برچسب انتخاب کنید، سپس روی بوم کلیک کنید.\r\n• بکشید برای جابجایی، اسکرول برای تغییر اندازه، راست‌کلیک > حذف.\r\n\r\nجستجو：در کادر جستجو تایپ کنید (بدون حساسیت به حروف)." },
                { Language.Norwegian, "[Etikettbehandling]\r\n\r\nLegg til：Høyreklikk i treet > Legg til rotnode / barnenode. Enter for å bekrefte.\r\nGi nytt navn/Slett：Høyreklikk > Gi nytt navn eller Slett gjeldende node.\r\n\r\nPlasser tekst：Velg en etikett, deretter venstreklikk på lerretet.\r\n• Dra for å flytte, rull for å endre størrelse, høyreklikk > Slett.\r\n\r\nSøk：Skriv i søkefeltet for å filtrere (ikke skille mellom store/små bokstaver)." },
                { Language.Arabic, "[إدارة التسميات]\r\n\r\nإضافة：انقر يمين في الشجرة > إضافة عقدة جذر / فرعية. Enter للتأكيد.\r\nإعادة تسمية/حذف：انقر يمين > إعادة تسمية أو حذف العقدة الحالية.\r\n\r\nوضع النص：حدد تسمية ثم انقر على اللوحة.\r\n• اسحب للتحريك، مرر العجلة لتغيير الحجم، انقر يمين > حذف.\r\n\r\nبحث：اكتب في مربع البحث للتصفية (غير حساس لحالة الأحرف)." }
            },
            ["HelpContentCanvas"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "【畫布操作】\r\n\r\n移動控件：\r\n• 將鼠標移到裝置或文字上\r\n• 按住左鍵拖動到新位置\r\n• 鬆開左鍵完成移動\r\n\r\n縮放控件：\r\n• 將鼠標移到裝置或文字上\r\n• 向上滾動滾輪：放大\r\n• 向下滾動滾輪：縮小\r\n• 縮放時保持原始比例不變形\r\n\r\n滾動畫布：\r\n• 在畫布空白處滾動滾輪可上下滾動\r\n• 使用縱向與橫向滾動條查看大圖\r\n• 畫布尺寸會依目前內容自動調整\r\n\r\n刪除控件：\r\n• 右鍵點擊要刪除的裝置或文字\r\n• 在彈出菜單中選擇「刪除」\r\n\r\n自動對齊：\r\n• 右鍵點擊畫布或任意控件\r\n• 選擇「自動對齊（居中顯示）」或「自動對齊（右側顯示）」\r\n• 連續點擊可在兩種模式間切換\r\n\r\n清空畫布：\r\n• 右鍵選擇「清空畫布」移除所有內容\r\n\r\n提示：鼠標在控件上時，滾輪用於縮放；在空白處時，滾輪用於滾動畫布。\r\n注意：刪除和清空操作無法撤銷，請謹慎使用。" },
                { Language.SimplifiedChinese, "【画布操作】\r\n\r\n移动控件：\r\n• 将鼠标移到装置或文字上\r\n• 按住左键拖动到新位置\r\n\r\n缩放控件：\r\n• 将鼠标移到装置或文字上\r\n• 向上滚动滚轮：放大；向下：缩小\r\n• 缩放时保持原始比例\r\n\r\n滚动画布：\r\n• 在画布空白处滚动滚轮\r\n• 使用纵向与横向滚动条查看大图\r\n• 画布尺寸会按内容自动调整\r\n\r\n删除控件：\r\n• 右键点击控件选择「删除」\r\n\r\n自动对齐：\r\n• 右键画布选择「自动对齐（居中显示）」或「自动对齐（右侧显示）」\r\n• 连续点击可在两种模式间切换\r\n\r\n清空画布：\r\n• 右键选择「清空画布」移除所有内容\r\n\r\n提示：鼠标在控件上时滚轮用于缩放，在空白处时用于滚动画布。\r\n注意：删除和清空操作无法撤销。" },
                { Language.English, "[Canvas Operations]\r\n\r\nMove elements:\r\n• Hover over a device or text\r\n• Hold left button and drag to new position\r\n\r\nResize elements:\r\n• Hover over a device or text\r\n• Scroll up to enlarge, scroll down to shrink\r\n• Aspect ratio is preserved\r\n\r\nScroll the canvas:\r\n• Scroll the wheel on the blank canvas area\r\n• Use vertical and horizontal scrollbars for large drawings\r\n• Canvas size auto-fits current content\r\n\r\nDelete:\r\n• Right-click an element > Delete\r\n\r\nAuto Align:\r\n• Right-click > Auto Align (Center) or Auto Align (Right)\r\n• Click repeatedly to alternate between modes\r\n\r\nClear Canvas:\r\n• Right-click > Clear Canvas to remove all elements\r\n\r\nTip: Wheel over an element = resize; wheel over blank = scroll canvas.\r\nNote: Delete and Clear cannot be undone." },
                { Language.Spanish, "[Operaciones del lienzo]\r\n\r\nMover：Arrastre con el botón izquierdo.\r\nRedimensionar：Rueda del mouse sobre el elemento.\r\nDesplazar：Rueda en el área en blanco o barras de desplazamiento.\r\n\r\nEliminar：Clic derecho > Eliminar.\r\nAlinear：Clic derecho > Alinear automáticamente (Centro/Derecha). Alternar con clics repetidos.\r\nLimpiar：Clic derecho > Limpiar lienzo.\r\n\r\nNota: Eliminar y limpiar no se pueden deshacer." },
                { Language.French, "[Opérations sur la toile]\r\n\r\nDéplacer：Glissez avec le bouton gauche.\r\nRedimensionner：Molette sur l'élément.\r\nDéfiler：Molette sur la zone vide ou barres de défilement.\r\n\r\nSupprimer：Clic droit > Supprimer.\r\nAligner：Clic droit > Alignement auto (Centre/Droite). Alternez en cliquant.\r\nEffacer：Clic droit > Effacer la toile.\r\n\r\nNote : Supprimer et Effacer sont irréversibles." },
                { Language.Portuguese, "[Operações da tela]\r\n\r\nMover：Arraste com o botão esquerdo.\r\nRedimensionar：Roda do mouse sobre o elemento.\r\nRolar：Roda na área em branco ou barras de rolagem.\r\n\r\nExcluir：Clique direito > Excluir.\r\nAlinhar：Clique direito > Alinhar automaticamente (Centro/Direita). Alterne com cliques.\r\nLimpar：Clique direito > Limpar tela.\r\n\r\nNota: Excluir e Limpar não podem ser desfeitos." },
                { Language.Russian, "[Операции с холстом]\r\n\r\nПеремещение：Перетаскивайте ЛКМ.\r\nМасштабирование：Колёсико на элементе.\r\nПрокрутка：Колёсико на пустой области или полосы прокрутки.\r\n\r\nУдаление：ПКМ > Удалить.\r\nВыравнивание：ПКМ > Автовыравнивание (По центру/Справа). Чередуйте кликами.\r\nОчистка：ПКМ > Очистить холст.\r\n\r\nПримечание: Удаление и очистка необратимы." },
                { Language.Persian, "[عملیات بوم]\r\n\r\nجابجایی：با کلیک چپ بکشید.\r\nتغییر اندازه：اسکرول روی عنصر.\r\nپیمایش：اسکرول در ناحیه خالی یا نوارهای اسکرول.\r\n\r\nحذف：راست‌کلیک > حذف.\r\nتراز：راست‌کلیک > تراز خودکار (مرکز/راست). با کلیک‌های متوالی تناوب کنید.\r\nپاک کردن：راست‌کلیک > پاک کردن بوم.\r\n\r\nتوجه: حذف و پاک کردن قابل بازگشت نیست." },
                { Language.Norwegian, "[Lerretsoperasjoner]\r\n\r\nFlytt：Dra med venstre museknapp.\r\nEndre størrelse：Rull på elementet.\r\nBla：Rull på tomt område eller bruk rullefelt.\r\n\r\nSlett：Høyreklikk > Slett.\r\nJuster：Høyreklikk > Autojustering (Senter/Høyre). Veksle med gjentatte klikk.\r\nTøm：Høyreklikk > Tøm lerret.\r\n\r\nMerk: Slett og Tøm kan ikke angres." },
                { Language.Arabic, "[عمليات اللوحة]\r\n\r\nتحريك：اسحب بالزر الأيسر.\r\nتغيير الحجم：مرر العجلة على العنصر.\r\nتمرير：مرر العجلة في المنطقة الفارغة أو استخدم أشرطة التمرير.\r\n\r\nحذف：انقر يمين > حذف.\r\nمحاذاة：انقر يمين > محاذاة تلقائية (وسط/يمين). تبادل بالنقرات.\r\nمسح：انقر يمين > مسح اللوحة.\r\n\r\nملاحظة: الحذف والمسح لا يمكن التراجع عنهما." }
            },
            ["HelpContentTemplate"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "【模板庫】\r\n\r\n可以將當前畫布內容保存為模板，以便日後重複使用。\r\n\r\n保存為新模板：\r\n• 右鍵點擊畫布，選擇「添加樣例到庫」\r\n• 在彈出對話框中輸入模板名稱\r\n• 可選擇保存到指定資料夾\r\n• 保存內容包括所有裝置和文字的類型、位置、大小\r\n\r\n保存到已有模板：\r\n• 先從模板庫載入一個模板，修改後\r\n• 右鍵選擇「保存到目前模板」覆蓋保存\r\n\r\n載入模板：\r\n• 在模板庫樹中單擊模板節點即可載入\r\n• 載入模板會清空當前畫布內容\r\n• 資料夾節點無法載入，只有模板節點可以\r\n\r\n管理資料夾：\r\n• 右鍵可新增資料夾、重命名、刪除\r\n• 刪除資料夾會連同內容一起刪除\r\n\r\n搜索模板：\r\n• 在模板庫上方的搜索框中輸入關鍵字\r\n• 同時搜索資料夾名和模板名，不區分大小寫\r\n• 清空搜索框恢復全部顯示\r\n\r\n提示：模板會自動保存到本地文件，關閉程序後不會丟失。建議按項目或類型分類存放。" },
                { Language.SimplifiedChinese, "【模板库】\r\n\r\n可以将当前画布内容保存为模板，以便日后重复使用。\r\n\r\n保存为新模板：\r\n• 右键点击画布，选择「添加样例到库」\r\n• 输入模板名称，可选择保存到指定文件夹\r\n• 保存内容包括所有装置和文字的类型、位置、大小\r\n\r\n保存到已有模板：\r\n• 先从模板库加载一个模板，修改后\r\n• 右键选择「保存到当前模板」覆盖保存\r\n\r\n加载模板：\r\n• 在模板库树中单击模板节点即可加载\r\n• 加载模板会清空当前画布内容\r\n\r\n管理文件夹：\r\n• 右键可新增文件夹、重命名、删除\r\n• 删除文件夹会连同内容一起删除\r\n\r\n搜索模板：\r\n• 在模板库上方的搜索框中输入关键字\r\n• 同时搜索文件夹名和模板名，不区分大小写\r\n\r\n提示：模板自动保存到本地，关闭程序后不会丢失。" },
                { Language.English, "[Template Library]\r\n\r\nSave your canvas as a template for future reuse.\r\n\r\nSave as new template:\r\n• Right-click the canvas > Add to Library\r\n• Enter a template name, optionally select a folder\r\n• Saves all device types, positions, and sizes\r\n\r\nSave to existing template:\r\n• Load a template from the library, make edits\r\n• Right-click > Save to Current Template to overwrite\r\n\r\nLoad a template:\r\n• Click a template node in the Template Library tree\r\n• Loading replaces the current canvas content\r\n• Only template nodes can be loaded (not folders)\r\n\r\nManage folders:\r\n• Right-click to add folders, rename, or delete\r\n• Deleting a folder also deletes its contents\r\n\r\nSearch:\r\n• Type in the search box above the Template Library\r\n• Searches both folder and template names, case-insensitive\r\n\r\nTip: Templates are stored locally and persist after closing." },
                { Language.Spanish, "[Biblioteca de plantillas]\r\n\r\nGuardar nuevo：Clic derecho > Añadir a biblioteca. Ingrese un nombre.\r\nGuardar existente：Cargue una plantilla, edítela, clic derecho > Guardar en plantilla actual.\r\n\r\nCargar：Clic en un nodo de plantilla en el árbol.\r\nNota: Cargar reemplaza el contenido actual del lienzo.\r\n\r\nCarpetas：Clic derecho para agregar/renombrar/eliminar carpetas.\r\nBúsqueda：Escriba en el cuadro de búsqueda para filtrar.\r\n\r\nLas plantillas se guardan localmente y persisten tras cerrar." },
                { Language.French, "[Bibliothèque de modèles]\r\n\r\nNouveau：Clic droit > Ajouter à la bibliothèque. Saisissez un nom.\r\nExistant：Chargez un modèle, modifiez-le, clic droit > Enregistrer dans le modèle actuel.\r\n\r\nCharger：Cliquez sur un nœud de modèle dans l'arbre.\r\nNote : Le chargement remplace le contenu actuel de la toile.\r\n\r\nDossiers：Clic droit pour ajouter/renommer/supprimer.\r\nRecherche：Tapez dans la zone de recherche pour filtrer.\r\n\r\nLes modèles sont stockés localement et persistent après fermeture." },
                { Language.Portuguese, "[Biblioteca de modelos]\r\n\r\nNovo：Clique direito > Adicionar à biblioteca. Digite um nome.\r\nExistente：Carregue um modelo, edite, clique direito > Salvar no modelo atual.\r\n\r\nCarregar：Clique em um nó de modelo na árvore.\r\nNota: Carregar substitui o conteúdo atual da tela.\r\n\r\nPastas：Clique direito para adicionar/renomear/excluir.\r\nPesquisa：Digite na caixa para filtrar.\r\n\r\nModelos são armazenados localmente e persistem após fechar." },
                { Language.Russian, "[Библиотека шаблонов]\r\n\r\nНовый：ПКМ > Добавить в библиотеку. Введите имя.\r\nСуществующий：Загрузите шаблон, отредактируйте, ПКМ > Сохранить в текущий шаблон.\r\n\r\nЗагрузить：Щёлкните по узлу шаблона в дереве.\r\nПримечание: Загрузка заменяет текущее содержимое холста.\r\n\r\nПапки：ПКМ для добавления/переименования/удаления.\r\nПоиск：Введите в поле поиска для фильтрации.\r\n\r\nШаблоны хранятся локально и сохраняются после закрытия." },
                { Language.Persian, "[کتابخانه قالب‌ها]\r\n\r\nجدید：راست‌کلیک > افزودن به کتابخانه. نام وارد کنید.\r\nموجود：یک قالب بارگذاری کنید، ویرایش کنید، راست‌کلیک > ذخیره در قالب فعلی.\r\n\r\nبارگذاری：روی گره قالب در درخت کلیک کنید.\r\nتوجه: بارگذاری محتوای فعلی بوم را جایگزین می‌کند.\r\n\r\nپوشه‌ها：راست‌کلیک برای افزودن/تغییرنام/حذف.\r\nجستجو：در کادر جستجو تایپ کنید.\r\n\r\nقالب‌ها به صورت محلی ذخیره می‌شوند و پس از بسته شدن باقی می‌مانند." },
                { Language.Norwegian, "[Malbibliotek]\r\n\r\nNy：Høyreklikk > Legg til i biblioteket. Skriv inn et navn.\r\nEksisterende：Last en mal, rediger, høyreklikk > Lagre til gjeldende mal.\r\n\r\nLast：Klikk på en mal-node i treet.\r\nMerk: Lasting erstatter gjeldende lerretsinnhold.\r\n\r\nMapper：Høyreklikk for å legge til/gi nytt navn/slette.\r\nSøk：Skriv i søkefeltet for å filtrere.\r\n\r\nMaler lagres lokalt og beholdes etter lukking." },
                { Language.Arabic, "[مكتبة القوالب]\r\n\r\nجديد：انقر يمين > إضافة إلى المكتبة. أدخل اسماً.\r\nموجود：حمّل قالباً، عدّله، انقر يمين > حفظ في القالب الحالي.\r\n\r\nتحميل：انقر على عقدة القالب في الشجرة.\r\nملاحظة: التحميل يستبدل محتوى اللوحة الحالي.\r\n\r\nمجلدات：انقر يمين لإضافة/إعادة تسمية/حذف.\r\nبحث：اكتب في مربع البحث للتصفية.\r\n\r\nالقوالب تُخزن محلياً وتبقى بعد الإغلاق." }
            },
            ["HelpContentCapture"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "【截圖功能】\r\n\r\n手動截圖：\r\n1. 右鍵點擊畫布，選擇「截圖」\r\n2. 鼠標變為十字形，進入截圖模式\r\n3. 按住左鍵，從左上角拖動到右下角\r\n4. 鬆開左鍵完成截圖，自動複製到剪貼簿\r\n• 在截圖模式下點擊右鍵可取消\r\n\r\n自動截圖：\r\n1. 右鍵點擊畫布，選擇「自動截圖」\r\n2. 程序自動計算所有控件的範圍\r\n3. 截取包含所有內容的最小區域，自動複製到剪貼簿\r\n• 畫布為空時會提示無法截圖\r\n\r\n長圖截取：\r\n1. 進入截圖模式後從左上角開始拖動\r\n2. 拖到畫布底部時，保持左鍵按住不放\r\n3. 滾動滾輪向下滾動畫布\r\n4. 到達目標位置後鬆開左鍵\r\n5. 程序自動拼接成完整長圖\r\n\r\n使用截圖：\r\n• 在 Word、PPT、微信等軟件中按 Ctrl+V 粘貼\r\n• 或在畫圖軟件中粘貼後保存為圖片文件" },
                { Language.SimplifiedChinese, "【截图功能】\r\n\r\n手动截图：\r\n1. 右键点击画布，选择「截图」\r\n2. 鼠标变为十字形，进入截图模式\r\n3. 按住左键，从左上角拖动到右下角\r\n4. 松开左键完成截图，自动复制到剪贴板\r\n• 截图模式下点击右键可取消\r\n\r\n自动截图：\r\n1. 右键点击画布，选择「自动截图」\r\n2. 程序自动计算所有控件的范围\r\n3. 截取最小包含区域，自动复制到剪贴板\r\n• 画布为空时会提示无法截图\r\n\r\n长图截取：\r\n1. 进入截图模式后从左上角开始拖动\r\n2. 拖到画布底部时保持左键按住\r\n3. 滚动滚轮向下滚动画布\r\n4. 到达目标位置后松开左键，自动拼接\r\n\r\n使用截图：\r\n• 在 Word、PPT 等软件中按 Ctrl+V 粘贴" },
                { Language.English, "[Screenshot & Capture]\r\n\r\nManual screenshot:\r\n1. Right-click canvas > Screenshot\r\n2. Cursor changes to crosshair (capture mode)\r\n3. Hold left button and drag from top-left to bottom-right\r\n4. Release to capture; image is copied to clipboard\r\n• Right-click to cancel capture mode\r\n\r\nAuto screenshot:\r\n1. Right-click canvas > Auto Screenshot\r\n2. The app detects all elements and captures the minimum bounding area\r\n3. Image is copied to clipboard automatically\r\n• Shows a warning if the canvas is empty\r\n\r\nLong screenshot:\r\n1. Enter capture mode and start dragging from top-left\r\n2. When reaching the bottom, keep left button held\r\n3. Scroll down with the wheel to extend the canvas\r\n4. Release to finish; images are stitched automatically\r\n\r\nUsage:\r\n• Press Ctrl+V in Word, PPT, etc. to paste the screenshot" },
                { Language.Spanish, "[Captura de pantalla]\r\n\r\nManual：Clic derecho > Captura. Arrastre un rectángulo. Clic derecho para cancelar.\r\nAutomática：Clic derecho > Captura automática. Captura el área mínima que contiene todos los elementos.\r\nLarga：En modo captura, arrastre hasta el borde inferior, mantenga el botón y desplácese con la rueda.\r\n\r\nUso：Ctrl+V para pegar en Word, PPT, etc." },
                { Language.French, "[Capture d'écran]\r\n\r\nManuelle：Clic droit > Capture. Dessinez un rectangle. Clic droit pour annuler.\r\nAutomatique：Clic droit > Capture auto. Capture la zone minimale contenant tous les éléments.\r\nLongue：En mode capture, glissez vers le bas, maintenez le bouton et faites défiler.\r\n\r\nUtilisation：Ctrl+V pour coller dans Word, PPT, etc." },
                { Language.Portuguese, "[Captura de tela]\r\n\r\nManual：Clique direito > Captura. Arraste um retângulo. Clique direito para cancelar.\r\nAutomática：Clique direito > Captura automática. Captura a área mínima com todos os elementos.\r\nLonga：No modo captura, arraste até o fundo, segure o botão e role.\r\n\r\nUso：Ctrl+V para colar no Word, PPT, etc." },
                { Language.Russian, "[Снимок экрана]\r\n\r\nВручную：ПКМ > Снимок. Выделите прямоугольник. ПКМ для отмены.\r\nАвтоматически：ПКМ > Автоснимок. Захватывает минимальную область со всеми элементами.\r\nДлинный：В режиме захвата тяните вниз, удерживая кнопку, и прокручивайте колёсиком.\r\n\r\nИспользование：Ctrl+V для вставки в Word, PPT и т.д." },
                { Language.Persian, "[اسکرین‌شات]\r\n\r\nدستی：راست‌کلیک > اسکرین‌شات. مستطیل بکشید. راست‌کلیک برای لغو.\r\nخودکار：راست‌کلیک > اسکرین‌شات خودکار. ناحیه حداقل شامل همه عناصر را می‌گیرد.\r\nبلند：در حالت گرفتن، به پایین بکشید، دکمه را نگه دارید و اسکرول کنید.\r\n\r\nاستفاده：Ctrl+V برای چسباندن در Word, PPT و غیره." },
                { Language.Norwegian, "[Skjermbilde]\r\n\r\nManuell：Høyreklikk > Skjermbilde. Dra et rektangel. Høyreklikk for å avbryte.\r\nAutomatisk：Høyreklikk > Automatisk skjermbilde. Fanger det minste området med alle elementer.\r\nLangt：I fangstmodus, dra til bunnen, hold knappen og rull.\r\n\r\nBruk：Ctrl+V for å lime inn i Word, PPT, osv." },
                { Language.Arabic, "[لقطة الشاشة]\r\n\r\nيدوية：انقر يمين > لقطة. ارسم مستطيلاً. انقر يمين للإلغاء.\r\nتلقائية：انقر يمين > لقطة تلقائية. تلتقط أصغر منطقة تحتوي كل العناصر.\r\nطويلة：في وضع الالتقاط، اسحب للأسفل، أبقِ الزر مضغوطاً ومرر العجلة.\r\n\r\nالاستخدام：Ctrl+V للصق في Word, PPT, إلخ." }
            },
            ["HelpContentJsonImport"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "【JSON 匯入】\r\n\r\nJSON 匯入功能可讓您通過粘貼 AI 工具（ChatGPT、Claude 等）生成的 JSON 文字，快速創建井口裝置示意圖。\r\n\r\n使用流程：\r\n1. 在畫布上右鍵選擇「匯入 JSON」\r\n2. 在彈出對話框中點擊「複製 JSON 規則」\r\n3. 將規則粘貼到任意 AI 對話中，描述您需要的井口配置\r\n4. 複製 AI 生成的 JSON，粘貼到文本框中\r\n5. 點擊確定匯入 — 裝置自動顯示在畫布上\r\n\r\n支持的格式：\r\n• 單個 JSON 物件（含 \"devices\" 陣列）\r\n• 多個 JSON 物件連續粘貼（每個成為獨立繪圖）\r\n• JSON 陣列格式（每個元素是一個繪圖）\r\n\r\n規則內容：\r\n• 全部 23 種內置裝置類型及其標識符\r\n• JSON 格式規則和常見錯誤提醒\r\n• 標籤和法蘭規格的格式指南\r\n• 各開次的典型裝置堆疊順序和完整示例\r\n• 如果您添加了自定義裝置，它們也會列出\r\n\r\n錯誤處理：\r\n• JSON 格式錯誤時，彈窗顯示具體行號和原因\r\n• 裝置類型無法識別時，提示最接近的正確名稱\r\n• 錯誤彈窗支持複製文字，方便發送給 AI 修正\r\n\r\n自定義裝置：\r\n• 已添加的自定義裝置圖片會出現在規則中\r\n• 使用文件名（不含擴展名）作為 type 值\r\n• 自定義裝置在畫布上以位圖方式渲染\r\n\r\n提示：匯入後，裝置標籤會自動添加到標籤樹中。規則設計兼容各種 AI 工具，如果出錯可複製錯誤訊息發送給 AI 修正。" },
                { Language.SimplifiedChinese, "【JSON 导入】\r\n\r\nJSON 导入功能可让您通过粘贴 AI 工具（ChatGPT、Claude 等）生成的 JSON 文字，快速创建井口装置示意图。\r\n\r\n使用流程：\r\n1. 在画布上右键选择「导入 JSON」\r\n2. 在弹出对话框中点击「复制 JSON 规则」\r\n3. 将规则粘贴到任意 AI 对话中，描述您需要的井口配置\r\n4. 复制 AI 生成的 JSON，粘贴到文本框中\r\n5. 点击确定导入 — 装置自动显示在画布上\r\n\r\n支持的格式：\r\n• 单个 JSON 对象（含 \"devices\" 数组）\r\n• 多个 JSON 对象连续粘贴（每个成为独立绘图）\r\n• JSON 数组格式（每个元素是一个绘图）\r\n\r\n规则内容：\r\n• 全部 23 种内置装置类型及其标识符\r\n• JSON 格式规则和常见错误提醒\r\n• 标签和法兰规格的格式指南\r\n• 各开次的典型装置堆叠顺序和完整示例\r\n• 如果您添加了自定义装置，它们也会列出\r\n\r\n错误处理：\r\n• JSON 格式错误时，弹窗显示具体行号和原因\r\n• 装置类型无法识别时，提示最接近的正确名称\r\n• 错误弹窗支持复制文字，方便发送给 AI 修正\r\n\r\n自定义装置：\r\n• 已添加的自定义装置图片会出现在规则中\r\n• 使用文件名（不含扩展名）作为 type 值\r\n• 自定义装置在画布上以位图方式渲染\r\n\r\n提示：导入后，装置标签会自动添加到标签树中。规则设计兼容各种 AI 工具，如果出错可复制错误信息发送给 AI 修正。" },
                { Language.English, "[JSON Import]\r\n\r\nJSON Import lets you create wellhead device stacks by pasting JSON text generated by AI tools (ChatGPT, Claude, etc.).\r\n\r\nWorkflow:\r\n1. Right-click the canvas > Import JSON\r\n2. Click \"Copy JSON Schema Rules\" to copy the schema to clipboard\r\n3. Paste the schema into any AI chat, describe the wellhead configuration\r\n4. Copy the AI-generated JSON, paste into the text box\r\n5. Click OK — devices appear on the canvas automatically\r\n\r\nSupported formats:\r\n• Single JSON object with a \"devices\" array\r\n• Multiple JSON objects pasted together (each becomes a drawing)\r\n• JSON array of objects\r\n\r\nSchema contents:\r\n• All 23 built-in device types with identifiers\r\n• JSON format rules and common mistakes\r\n• Label and flange formatting guidelines\r\n• Typical stacking orders and complete examples\r\n• Custom devices are also listed if you have added any\r\n\r\nError handling:\r\n• Invalid JSON shows exact line number and cause\r\n• Unrecognized device type suggests closest match\r\n• Error dialog has a Copy button for easy troubleshooting\r\n\r\nCustom devices:\r\n• Custom device images appear in the schema\r\n• Use filename (without extension) as the type value\r\n• Custom devices render as bitmap images on canvas\r\n\r\nTip: After import, device labels are automatically added to the tag tree." },
                { Language.Spanish, "[Importar JSON]\r\n\r\nImportar JSON permite crear pilas de dispositivos pegando JSON generado por herramientas de IA.\r\n\r\nFlujo：\r\n1. Clic derecho > Importar JSON\r\n2. Copie las reglas del esquema con el botón\r\n3. Pegue las reglas en su IA, describa la configuración\r\n4. Copie el JSON generado y péguelo\r\n5. Clic en Aceptar para importar\r\n\r\nFormatos：JSON único, múltiples objetos, o arreglo JSON.\r\n\r\nErrores：Muestra línea/causa exactas; tipos no reconocidos sugieren coincidencias cercanas. Botón Copiar para enviar errores al AI.\r\n\r\nDispositivos personalizados：Los nombres de archivo (sin extensión) se usan como tipo." },
                { Language.French, "[Import JSON]\r\n\r\nL'import JSON permet de créer des empilements en collant du JSON généré par des outils IA.\r\n\r\nFlux：\r\n1. Clic droit > Importer JSON\r\n2. Copiez les règles du schéma avec le bouton\r\n3. Collez les règles dans votre IA, décrivez la configuration\r\n4. Copiez le JSON généré et collez-le\r\n5. Cliquez OK pour importer\r\n\r\nFormats：JSON unique, plusieurs objets, ou tableau JSON.\r\n\r\nErreurs：Affiche la ligne/cause exactes ; types inconnus suggèrent des correspondances proches. Bouton Copier pour envoyer les erreurs à l'IA.\r\n\r\nAppareils personnalisés：Les noms de fichier (sans extension) sont utilisés comme type." },
                { Language.Portuguese, "[Importar JSON]\r\n\r\nA importação JSON permite criar empilhamentos colando JSON gerado por ferramentas de IA.\r\n\r\nFluxo：\r\n1. Clique direito > Importar JSON\r\n2. Copie as regras do esquema com o botão\r\n3. Cole as regras na sua IA, descreva a configuração\r\n4. Copie o JSON gerado e cole\r\n5. Clique OK para importar\r\n\r\nFormatos：JSON único, múltiplos objetos, ou array JSON.\r\n\r\nErros：Mostra linha/causa exatas; tipos não reconhecidos sugerem correspondências. Botão Copiar para enviar erros à IA.\r\n\r\nDispositivos personalizados：Nomes de arquivo (sem extensão) são usados como tipo." },
                { Language.Russian, "[Импорт JSON]\r\n\r\nИмпорт JSON позволяет создавать стеки устройств, вставляя JSON из ИИ-инструментов.\r\n\r\nПроцесс：\r\n1. ПКМ > Импорт JSON\r\n2. Скопируйте правила схемы кнопкой\r\n3. Вставьте правила в ИИ-чат, опишите конфигурацию\r\n4. Скопируйте сгенерированный JSON и вставьте\r\n5. Нажмите ОК для импорта\r\n\r\nФорматы：Один JSON, несколько объектов или JSON-массив.\r\n\r\nОшибки：Показывают точную строку/причину; нераспознанные типы предлагают ближайшие совпадения. Кнопка «Копировать» для отправки ошибок ИИ.\r\n\r\nПользовательские устройства：Имена файлов (без расширения) используются как тип." },
                { Language.Persian, "[ورود JSON]\r\n\r\nورود JSON امکان ایجاد پشته تجهیزات با چسباندن JSON از ابزارهای هوش مصنوعی را فراهم می‌کند.\r\n\r\nگردش کار：\r\n1. راست‌کلیک > ورود JSON\r\n2. قوانین طرح را با دکمه کپی کنید\r\n3. قوانین را در هوش مصنوعی بچسبانید و پیکربندی را توضیح دهید\r\n4. JSON تولید شده را کپی و بچسبانید\r\n5. روی تأیید کلیک کنید\r\n\r\nفرمت‌ها：JSON تک، چند شیء، یا آرایه JSON.\r\n\r\nخطاها：خط/علت دقیق نمایش داده می‌شود؛ انواع ناشناخته نزدیک‌ترین تطابق را پیشنهاد می‌دهند. دکمه کپی برای ارسال خطاها.\r\n\r\nدستگاه‌های سفارشی：نام فایل (بدون پسوند) به‌عنوان نوع استفاده می‌شود." },
                { Language.Norwegian, "[JSON-import]\r\n\r\nJSON-import lar deg lage enhetsstakker ved å lime inn JSON fra AI-verktøy.\r\n\r\nArbeidsflyt：\r\n1. Høyreklikk > Importer JSON\r\n2. Kopier skjemaregler med knappen\r\n3. Lim reglene inn i AI-chatten, beskriv konfigurasjonen\r\n4. Kopier den genererte JSON-en og lim inn\r\n5. Klikk OK for å importere\r\n\r\nFormater：Enkelt JSON, flere objekter, eller JSON-array.\r\n\r\nFeil：Viser nøyaktig linje/årsak; ukjente typer foreslår nærmeste treff. Kopier-knapp for å sende feil til AI.\r\n\r\nEgendefinerte enheter：Filnavn (uten filtype) brukes som type." },
                { Language.Arabic, "[استيراد JSON]\r\n\r\nيتيح استيراد JSON إنشاء مكدسات بلصق JSON من أدوات الذكاء الاصطناعي.\r\n\r\nسير العمل：\r\n1. انقر يمين > استيراد JSON\r\n2. انسخ قواعد المخطط بالزر\r\n3. الصق القواعد في الذكاء الاصطناعي وصف التكوين\r\n4. انسخ JSON الناتج والصقه\r\n5. انقر موافق للاستيراد\r\n\r\nالتنسيقات：JSON واحد، عدة كائنات، أو مصفوفة JSON.\r\n\r\nالأخطاء：تعرض السطر/السبب بالضبط؛ الأنواع غير المعروفة تقترح أقرب تطابق. زر نسخ لإرسال الأخطاء.\r\n\r\nأجهزة مخصصة：اسم الملف (بدون الامتداد) يُستخدم كنوع." }
            },
            ["HelpContentData"] = new Dictionary<Language, string>
            {
                { Language.TraditionalChinese, "【數據與樣例】\r\n\r\n數據文件：\r\n• template_library.bin — 模板庫數據\r\n• tagtree_items.bin — 標籤樹數據\r\n• pictures 資料夾 — 自定義裝置圖片\r\n• language.conf — 語言設置\r\n\r\n自動保存：\r\n• 標籤和模板的修改會自動保存\r\n• 關閉程序時也會自動保存\r\n• 無需手動保存操作\r\n\r\n裝置樣例：\r\n• 右鍵點擊畫布，選擇「打開裝置樣例」\r\n• 會自動打開系統畫圖程序顯示樣例圖\r\n• 可作為繪製井口裝置示意圖的參考\r\n\r\n備份建議：\r\n• 定期備份程序目錄下的數據文件\r\n• 重裝系統前請備份整個程序資料夾\r\n\r\n注意事項：\r\n• 請勿手動刪除或修改數據文件\r\n• 數據文件損壞可能導致內容丟失\r\n• 遷移到其他電腦時請複製整個程序資料夾" },
                { Language.SimplifiedChinese, "【数据与样例】\r\n\r\n数据文件：\r\n• template_library.bin — 模板库数据\r\n• tagtree_items.bin — 标签树数据\r\n• pictures 文件夹 — 自定义装置图片\r\n• language.conf — 语言设置\r\n\r\n自动保存：\r\n• 标签和模板的修改会自动保存\r\n• 关闭程序时也会自动保存\r\n• 无需手动保存操作\r\n\r\n装置样例：\r\n• 右键点击画布，选择「打开装置样例」\r\n• 会自动打开系统画图程序显示样例图\r\n• 可作为绘制井口装置示意图的参考\r\n\r\n备份建议：\r\n• 定期备份程序目录下的数据文件\r\n• 迁移到其他电脑时请复制整个程序文件夹\r\n\r\n注意事项：\r\n• 请勿手动删除或修改数据文件\r\n• 数据文件损坏可能导致内容丢失" },
                { Language.English, "[Data & Samples]\r\n\r\nData files:\r\n• template_library.bin — template library\r\n• tagtree_items.bin — tag tree data\r\n• pictures folder — custom device images\r\n• language.conf — language setting\r\n\r\nAuto-save:\r\n• Changes to labels and templates save automatically\r\n• No manual save required\r\n\r\nDevice samples:\r\n• Right-click canvas > Open Device Sample\r\n• Opens system image viewer with example diagrams\r\n• Use as reference while drawing\r\n\r\nBackup:\r\n• Back up the app folder regularly\r\n• Copy the entire app folder when moving to a new PC\r\n\r\nNotes:\r\n• Do not manually edit or delete data files\r\n• Corrupted data files may cause content loss" },
                { Language.Spanish, "[Datos y muestras]\r\n\r\nArchivos de datos：template_library.bin, tagtree_items.bin, carpeta pictures, language.conf.\r\nGuardado automático：Los cambios se guardan automáticamente.\r\nMuestras：Clic derecho > Abrir muestra de dispositivo para ver ejemplos.\r\n\r\nCopia de seguridad：Respalde la carpeta del programa regularmente.\r\nNota：No edite ni elimine manualmente los archivos de datos." },
                { Language.French, "[Données et exemples]\r\n\r\nFichiers de données：template_library.bin, tagtree_items.bin, dossier pictures, language.conf.\r\nSauvegarde auto：Les modifications sont sauvegardées automatiquement.\r\nExemples：Clic droit > Ouvrir un exemple d'appareil pour voir des schémas.\r\n\r\nSauvegarde：Sauvegardez régulièrement le dossier du programme.\r\nNote：Ne modifiez ni supprimez manuellement les fichiers de données." },
                { Language.Portuguese, "[Dados e amostras]\r\n\r\nArquivos de dados：template_library.bin, tagtree_items.bin, pasta pictures, language.conf.\r\nSalvamento auto：As alterações são salvas automaticamente.\r\nAmostras：Clique direito > Abrir amostra do dispositivo para ver exemplos.\r\n\r\nBackup：Faça backup da pasta do programa regularmente.\r\nNota：Não edite nem exclua manualmente os arquivos de dados." },
                { Language.Russian, "[Данные и примеры]\r\n\r\nФайлы данных：template_library.bin, tagtree_items.bin, папка pictures, language.conf.\r\nАвтосохранение：Изменения сохраняются автоматически.\r\nПримеры：ПКМ > Открыть пример устройства для просмотра схем.\r\n\r\nРезервная копия：Регулярно копируйте папку программы.\r\nПримечание：Не редактируйте и не удаляйте файлы данных вручную." },
                { Language.Persian, "[داده‌ها و نمونه‌ها]\r\n\r\nفایل‌های داده：template_library.bin, tagtree_items.bin, پوشه pictures, language.conf.\r\nذخیره خودکار：تغییرات به‌صورت خودکار ذخیره می‌شوند.\r\nنمونه‌ها：راست‌کلیک > باز کردن نمونه دستگاه.\r\n\r\nپشتیبان‌گیری：مرتباً از پوشه برنامه پشتیبان بگیرید.\r\nتوجه：فایل‌های داده را به‌صورت دستی ویرایش یا حذف نکنید." },
                { Language.Norwegian, "[Data og eksempler]\r\n\r\nDatafiler：template_library.bin, tagtree_items.bin, pictures-mappe, language.conf.\r\nAutolagring：Endringer lagres automatisk.\r\nEksempler：Høyreklikk > Åpne enhetsprøve for å se diagrammer.\r\n\r\nSikkerhetskopiering：Sikkerhetskopier programmappen jevnlig.\r\nMerk：Ikke rediger eller slett datafiler manuelt." },
                { Language.Arabic, "[البيانات والعينات]\r\n\r\nملفات البيانات：template_library.bin, tagtree_items.bin, مجلد pictures, language.conf.\r\nحفظ تلقائي：يتم حفظ التغييرات تلقائياً.\r\nعينات：انقر يمين > فتح عينة الجهاز لعرض الأمثلة.\r\n\r\nنسخ احتياطي：انسخ مجلد البرنامج بانتظام.\r\nملاحظة：لا تعدّل أو تحذف ملفات البيانات يدوياً." }
            },
            ["UserFolderError"] = new Dictionary<Language, string>
            {
                { Language.English, "User folder is not accessible." },
                { Language.SimplifiedChinese, "用户文件夹无法访问" },
                { Language.TraditionalChinese, "用戶文件夾無法訪問" },
                { Language.Spanish, "La carpeta de usuario no es accesible." },
                { Language.French, "Le dossier utilisateur n'est pas accessible." },
                { Language.Portuguese, "A pasta do usuário não está acessível." },
                { Language.Russian, "Папка пользователя недоступна." },
                { Language.Persian, "پوشه کاربر قابل دسترسی نیست." },
                { Language.Norwegian, "Brukermappen er ikke tilgjengelig." },
                { Language.Arabic, "لا يمكن الوصول إلى مجلد المستخدم" }
            },
            ["ImageProcessError"] = new Dictionary<Language, string>
            {
                { Language.English, "Cannot process image: {0}\nError: {1}" },
                { Language.SimplifiedChinese, "无法处理图片: {0}\n错误: {1}" },
                { Language.TraditionalChinese, "無法處理圖片：{0}\n錯誤：{1}" },
                { Language.Spanish, "No se puede procesar la imagen: {0}\nError: {1}" },
                { Language.French, "Impossible de traiter l'image : {0}\nErreur : {1}" },
                { Language.Portuguese, "Não foi possível processar a imagem: {0}\nErro: {1}" },
                { Language.Russian, "Не удалось обработать изображение: {0}\nОшибка: {1}" },
                { Language.Persian, "پردازش تصویر ممکن نیست: {0}\nخطا: {1}" },
                { Language.Norwegian, "Kan ikke behandle bildet: {0}\nFeil: {1}" },
                { Language.Arabic, "تعذّر معالجة الصورة: {0}\nخطأ: {1}" }
            },

            // ===== AutoCapture =====
            ["CanvasEmpty"] = new Dictionary<Language, string>
            {
                { Language.English, "There is nothing on the canvas to capture." },
                { Language.SimplifiedChinese, "画布上没有任何内容可以截取。" },
                { Language.TraditionalChinese, "畫布上沒有任何內容可以截取。" },
                { Language.Spanish, "No hay nada en el lienzo para capturar." },
                { Language.French, "Il n'y a rien sur le canevas à capturer." },
                { Language.Portuguese, "Não há nada na tela para capturar." },
                { Language.Russian, "На холсте нет содержимого для захвата." },
                { Language.Persian, "محتوایی روی بوم برای ثبت وجود ندارد." },
                { Language.Norwegian, "Det er ingenting på lerretet å fange." },
                { Language.Arabic, "لا يوجد شيء على اللوحة لالتقاطه." }
            },
            ["Hint"] = new Dictionary<Language, string>
            {
                { Language.English, "Hint" },
                { Language.SimplifiedChinese, "提示" },
                { Language.TraditionalChinese, "提示" },
                { Language.Spanish, "Aviso" },
                { Language.French, "Info" },
                { Language.Portuguese, "Dica" },
                { Language.Russian, "Подсказка" },
                { Language.Persian, "راهنما" },
                { Language.Norwegian, "Tips" },
                { Language.Arabic, "تلميح" }
            },
            ["CannotCalculateBounds"] = new Dictionary<Language, string>
            {
                { Language.English, "Cannot calculate content bounds." },
                { Language.SimplifiedChinese, "无法计算内容边界。" },
                { Language.TraditionalChinese, "無法計算內容邊界。" },
                { Language.Spanish, "No se pueden calcular los límites del contenido." },
                { Language.French, "Impossible de calculer les limites du contenu." },
                { Language.Portuguese, "Não é possível calcular os limites do conteúdo." },
                { Language.Russian, "Не удалось вычислить границы содержимого." },
                { Language.Persian, "محاسبه محدوده محتوا ممکن نیست." },
                { Language.Norwegian, "Kan ikke beregne innholdsgrenser." },
                { Language.Arabic, "تعذّر حساب حدود المحتوى." }
            },
            ["CaptureSuccess"] = new Dictionary<Language, string>
            {
                { Language.English, "Screenshot copied to clipboard." },
                { Language.SimplifiedChinese, "截图已复制到剪贴板。" },
                { Language.TraditionalChinese, "截圖已複製到剪貼簿。" },
                { Language.Spanish, "Captura copiada al portapapeles." },
                { Language.French, "Capture copiée dans le presse-papiers." },
                { Language.Portuguese, "Captura copiada para a área de transferência." },
                { Language.Russian, "Снимок скопирован в буфер обмена." },
                { Language.Persian, "تصویر صفحه به حافظه کپی شد." },
                { Language.Norwegian, "Skjermbilde kopiert til utklippstavlen." },
                { Language.Arabic, "تم نسخ لقطة الشاشة إلى الحافظة." }
            },
            ["Success"] = new Dictionary<Language, string>
            {
                { Language.English, "Success" },
                { Language.SimplifiedChinese, "成功" },
                { Language.TraditionalChinese, "成功" },
                { Language.Spanish, "Éxito" },
                { Language.French, "Succès" },
                { Language.Portuguese, "Sucesso" },
                { Language.Russian, "Успех" },
                { Language.Persian, "موفق" },
                { Language.Norwegian, "Vellykket" },
                { Language.Arabic, "نجاح" }
            },
            ["CaptureFailed"] = new Dictionary<Language, string>
            {
                { Language.English, "Screenshot failed: {0}" },
                { Language.SimplifiedChinese, "截图失败：{0}" },
                { Language.TraditionalChinese, "截圖失敗：{0}" },
                { Language.Spanish, "Error de captura: {0}" },
                { Language.French, "Échec de la capture : {0}" },
                { Language.Portuguese, "Falha na captura: {0}" },
                { Language.Russian, "Ошибка снимка: {0}" },
                { Language.Persian, "خطا در ثبت تصویر: {0}" },
                { Language.Norwegian, "Skjermbilde mislyktes: {0}" },
                { Language.Arabic, "فشل التقاط الشاشة: {0}" }
            },

            // ===== TagTreeUserControl =====
            ["AddRootNode"] = new Dictionary<Language, string>
            {
                { Language.English, "Add Root Node" },
                { Language.SimplifiedChinese, "添加根节点" },
                { Language.TraditionalChinese, "添加根節點" },
                { Language.Spanish, "Añadir nodo raíz" },
                { Language.French, "Ajouter un nœud racine" },
                { Language.Portuguese, "Adicionar nó raiz" },
                { Language.Russian, "Добавить корневой узел" },
                { Language.Persian, "افزودن گره ریشه" },
                { Language.Norwegian, "Legg til rotnode" },
                { Language.Arabic, "إضافة عقدة جذرية" }
            },
            ["AddChildNode"] = new Dictionary<Language, string>
            {
                { Language.English, "Add Child Node" },
                { Language.SimplifiedChinese, "添加子节点" },
                { Language.TraditionalChinese, "添加子節點" },
                { Language.Spanish, "Añadir nodo hijo" },
                { Language.French, "Ajouter un nœud enfant" },
                { Language.Portuguese, "Adicionar nó filho" },
                { Language.Russian, "Добавить дочерний узел" },
                { Language.Persian, "افزودن گره فرزند" },
                { Language.Norwegian, "Legg til undernode" },
                { Language.Arabic, "إضافة عقدة فرعية" }
            },
            ["DeleteCurrentNode"] = new Dictionary<Language, string>
            {
                { Language.English, "Delete Current Node" },
                { Language.SimplifiedChinese, "删除当前节点" },
                { Language.TraditionalChinese, "刪除當前節點" },
                { Language.Spanish, "Eliminar nodo actual" },
                { Language.French, "Supprimer le nœud actuel" },
                { Language.Portuguese, "Excluir nó atual" },
                { Language.Russian, "Удалить текущий узел" },
                { Language.Persian, "حذف گره فعلی" },
                { Language.Norwegian, "Slett gjeldende node" },
                { Language.Arabic, "حذف العقدة الحالية" }
            },
            ["NewRootNode"] = new Dictionary<Language, string>
            {
                { Language.English, "New Root Node" },
                { Language.SimplifiedChinese, "新根节点" },
                { Language.TraditionalChinese, "新根節點" },
                { Language.Spanish, "Nuevo nodo raíz" },
                { Language.French, "Nouveau nœud racine" },
                { Language.Portuguese, "Novo nó raiz" },
                { Language.Russian, "Новый корневой узел" },
                { Language.Persian, "گره ریشه جدید" },
                { Language.Norwegian, "Ny rotnode" },
                { Language.Arabic, "عقدة جذرية جديدة" }
            },
            ["NewChildNode"] = new Dictionary<Language, string>
            {
                { Language.English, "New Child Node" },
                { Language.SimplifiedChinese, "新子节点" },
                { Language.TraditionalChinese, "新子節點" },
                { Language.Spanish, "Nuevo nodo hijo" },
                { Language.French, "Nouveau nœud enfant" },
                { Language.Portuguese, "Novo nó filho" },
                { Language.Russian, "Новый дочерний узел" },
                { Language.Persian, "گره فرزند جدید" },
                { Language.Norwegian, "Ny undernode" },
                { Language.Arabic, "عقدة فرعية جديدة" }
            },
        };

        static LocalizationManager()
        {
            EnsureLanguageEntriesForJapaneseAndKorean();
            ApplyJapaneseKoreanOverrides();
        }

        private static void EnsureLanguageEntriesForJapaneseAndKorean()
        {
            foreach (var kv in Strings)
            {
                Dictionary<Language, string> langDict = kv.Value;
                if (!langDict.ContainsKey(Language.Japanese) && langDict.TryGetValue(Language.English, out string en))
                {
                    langDict[Language.Japanese] = en;
                }
                if (!langDict.ContainsKey(Language.Korean) && langDict.TryGetValue(Language.English, out string en2))
                {
                    langDict[Language.Korean] = en2;
                }
            }
        }

        private static void ApplyJapaneseKoreanOverrides()
        {
            // 主界面與菜單
            SetJaKo("FormTitle", "井口装置作図ツール", "웰헤드 장치 도면 도구");
            SetJaKo("DeviceSelection", "装置選択", "장치 선택");
            SetJaKo("TagManagement", "ラベル管理", "라벨 관리");
            SetJaKo("BOPConfig", "BOP構成", "BOP 구성");
            SetJaKo("SavedTemplates", "保存済みテンプレート", "저장된 템플릿");
            SetJaKo("Help", "ヘルプ", "도움말");
            SetJaKo("Settings", "設定", "설정");
            SetJaKo("SettingsTitle", "設定", "설정");
            SetJaKo("LanguageLabel", "言語 / Language:", "언어 / Language:");
            SetJaKo("UseUncoloredDeviceLabel", "装置スタイル:", "장치 스타일:");
            SetJaKo("UseColoredDevice", "着色", "컬러");
            SetJaKo("UseUncoloredDevice", "無着色", "무채색");
            SetJaKo("Delete", "削除", "삭제");
            SetJaKo("AutoCapture", "自動キャプチャ", "자동 캡처");
            SetJaKo("Capture", "キャプチャ", "캡처");
            SetJaKo("ClearCanvas", "キャンバスをクリア", "캔버스 지우기");
            SetJaKo("OpenSample", "装置サンプルを開く", "장치 샘플 열기");
            SetJaKo("AutoAlign", "自動整列", "자동 정렬");
            SetJaKo("AutoAlignCenter", "自動整列（中央）", "자동 정렬(가운데)");
            SetJaKo("AutoAlignRight", "自動整列（右側）", "자동 정렬(오른쪽)");
            SetJaKo("AddSampleToLibrary", "ライブラリに追加", "라이브러리에 추가");
            SetJaKo("SaveCurrentTemplate", "現在のテンプレートに保存", "현재 템플릿에 저장");
            SetJaKo("ImportJson", "JSONインポート", "JSON 가져오기");
            SetJaKo("CopyNode", "コピー", "복사");
            SetJaKo("PasteAsChild", "子ノードとして貼り付け", "하위 노드로 붙여넣기");
            SetJaKo("AddCustomDevice", "カスタム装置を追加", "사용자 장치 추가");
            SetJaKo("DeleteCurrentDevice", "現在の装置を削除", "현재 장치 삭제");
            SetJaKo("Rename", "名前変更", "이름 바꾸기");
            SetJaKo("AddFolder", "フォルダを追加", "폴더 추가");
            SetJaKo("SaveTemplateTitle", "テンプレートをライブラリに保存", "템플릿 라이브러리에 저장");
            SetJaKo("SelectTargetFolder", "保存先フォルダを選択（未選択ならルート）:", "대상 폴더 선택(미선택 시 루트):");
            SetJaKo("TemplateName", "テンプレート名:", "템플릿 이름:");
            SetJaKo("Save", "保存", "저장");
            SetJaKo("Cancel", "キャンセル", "취소");
            SetJaKo("TemplateNameEmpty", "テンプレート名を入力してください。", "템플릿 이름을 입력하세요.");
            SetJaKo("Error", "エラー", "오류");
            SetJaKo("SaveToFolder", "「{0}」に保存", "\"{0}\"에 저장");
            SetJaKo("ConfirmDelete", "削除の確認", "삭제 확인");
            SetJaKo("ConfirmDeleteFolder", "フォルダ「{0}」と中身を削除しますか？", "폴더 \"{0}\" 및 모든 내용을 삭제하시겠습니까?");
            SetJaKo("ConfirmDeleteTemplate", "テンプレート「{0}」を削除しますか？", "템플릿 \"{0}\"을(를) 삭제하시겠습니까?");
            SetJaKo("NewFolder", "新しいフォルダ", "새 폴더");
            SetJaKo("ImageFileFilter", "画像ファイル|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.svg", "이미지 파일|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.svg");
            SetJaKo("SelectCustomDevice", "カスタム装置を選択", "사용자 장치 선택");
            SetJaKo("ImageNotExist", "画像ファイルが存在しません。", "이미지 파일이 존재하지 않습니다.");
            SetJaKo("DeleteFileFailed", "ファイル削除失敗: {0}", "파일 삭제 실패: {0}");
            SetJaKo("ErrorOccurred", "エラーが発生しました: {0}", "오류가 발생했습니다: {0}");
            SetJaKo("OK", "OK", "확인");

            // Help
            SetJaKo("ImportJson", "JSONインポート", "JSON 가져오기");
            SetJaKo("HelpTitle", "操作説明", "사용 안내");
            SetJaKo("HelpNavHeader", "目次", "목차");
            SetJaKo("HelpNavQuick", "クイックスタート", "빠른 시작");
            SetJaKo("HelpNavDevice", "装置管理", "장치 관리");
            SetJaKo("HelpNavDeviceSelect", "装置の選択と配置", "장치 선택 및 배치");
            SetJaKo("HelpNavDevicePreview", "装置プレビュー", "장치 미리보기");
            SetJaKo("HelpNavDeviceCustom", "カスタム装置追加", "사용자 장치 추가");
            SetJaKo("HelpNavDeviceColor", "色付き/無色切替", "컬러/무채색 전환");
            SetJaKo("HelpNavTags", "タグ管理", "태그 관리");
            SetJaKo("HelpNavTagCreate", "タグ作成", "태그 만들기");
            SetJaKo("HelpNavTagPlace", "タグ配置", "태그 배치");
            SetJaKo("HelpNavTagSearch", "タグ検索", "태그 검색");
            SetJaKo("HelpNavCanvas", "キャンバス操作", "캔버스 조작");
            SetJaKo("HelpNavCanvasMove", "移動と拡縮", "이동 및 크기 조절");
            SetJaKo("HelpNavCanvasDelete", "削除", "삭제");
            SetJaKo("HelpNavCanvasAlign", "自動整列", "자동 정렬");
            SetJaKo("HelpNavCanvasClear", "キャンバスクリア", "캔버스 지우기");
            SetJaKo("HelpNavTemplate", "テンプレートライブラリ", "템플릿 라이브러리");
            SetJaKo("HelpNavTemplateSave", "テンプレート保存", "템플릿 저장");
            SetJaKo("HelpNavTemplateLoad", "テンプレート読込", "템플릿 불러오기");
            SetJaKo("HelpNavTemplateFolder", "フォルダ管理", "폴더 관리");
            SetJaKo("HelpNavTemplateSearch", "テンプレート検索", "템플릿 검색");
            SetJaKo("HelpNavCapture", "キャプチャ", "캡처");
            SetJaKo("HelpNavCaptureManual", "手動キャプチャ", "수동 캡처");
            SetJaKo("HelpNavCaptureAuto", "自動キャプチャ", "자동 캡처");
            SetJaKo("HelpNavCaptureLong", "長図キャプチャ", "장면 캡처");
            SetJaKo("HelpNavJsonImport", "JSONインポート", "JSON 가져오기");
            SetJaKo("HelpNavJsonUsage", "JSON使用法", "JSON 사용법");
            SetJaKo("HelpNavJsonSchema", "JSON仕様", "JSON 규격");
            SetJaKo("HelpNavSample", "サンプル", "샘플");
            SetJaKo("HelpNavData", "データ保存", "데이터 저장");
            // Help 正文（與其他語言相同，提供實際語言內容而非英文回退）
            SetJaKo("HelpContentQuick",
                "【クイックスタート】\r\n\r\n1) 左の装置一覧から装置を選択し、キャンバスを左クリックして配置します。\r\n2) タグツリーからタグを選択し、キャンバスを左クリックして文字を配置します。\r\n3) 左ドラッグで移動、マウスホイールで拡大/縮小します。\r\n4) 右クリックメニューから整列・スクリーンショット・テンプレート保存・JSONインポートを実行できます。\r\n5) 設定で言語と装置スタイル（着色/無着色）を切り替えます。",
                "【빠른 시작】\r\n\r\n1) 왼쪽 장치 목록에서 장치를 선택하고 캔버스를 좌클릭해 배치합니다.\r\n2) 태그 트리에서 태그를 선택하고 캔버스를 좌클릭해 텍스트를 배치합니다.\r\n3) 좌클릭 드래그로 이동, 마우스 휠로 크기 조절합니다.\r\n4) 우클릭 메뉴에서 정렬, 스크린샷, 템플릿 저장, JSON 가져오기를 실행합니다.\r\n5) 설정에서 언어와 장치 스타일(컬러/무채색)을 변경합니다.");
            SetJaKo("HelpContentDevice",
                "【装置の選択と配置】\r\n\r\n・左の装置一覧をクリックするとカーソルにプレビューが表示されます。\r\n・キャンバス上で左クリックすると装置を配置できます。\r\n・同じ装置は連続クリックで複数配置できます。\r\n・右クリックまたは他領域クリックで選択解除できます。\r\n・カスタム装置は右クリックメニュー「カスタム装置を追加」から追加できます（JPG/PNG/BMP/GIF/SVG）。",
                "【장치 선택 및 배치】\r\n\r\n· 왼쪽 장치 목록을 클릭하면 커서에 미리보기가 표시됩니다.\r\n· 캔버스에서 좌클릭하면 장치를 배치할 수 있습니다.\r\n· 같은 장치는 연속 클릭으로 여러 개 배치할 수 있습니다.\r\n· 우클릭 또는 다른 영역 클릭으로 선택 해제할 수 있습니다.\r\n· 사용자 장치는 우클릭 메뉴의 \"사용자 장치 추가\"에서 등록합니다(JPG/PNG/BMP/GIF/SVG).");
            SetJaKo("HelpContentTags",
                "【タグ管理】\r\n\r\n・空白右クリックでルートノード追加、ノード右クリックで子ノード追加ができます。\r\n・右クリックメニューで名前変更/削除/コピー/貼り付けが可能です。\r\n・タグを選択してキャンバスを左クリックすると文字を配置します。\r\n・上部検索ボックスでタグをリアルタイムに絞り込みできます。",
                "【태그 관리】\r\n\r\n· 빈 영역 우클릭으로 루트 노드 추가, 노드 우클릭으로 하위 노드 추가가 가능합니다.\r\n· 우클릭 메뉴에서 이름 변경/삭제/복사/붙여넣기를 사용할 수 있습니다.\r\n· 태그를 선택한 뒤 캔버스를 좌클릭하면 텍스트가 배치됩니다.\r\n· 상단 검색창에서 태그를 실시간 필터링할 수 있습니다.");
            SetJaKo("HelpContentCanvas",
                "【キャンバス操作】\r\n\r\n・左ドラッグ: 要素移動\r\n・マウスホイール: 要素拡大/縮小\r\n・右クリックメニュー: 自動整列、削除、クリア、保存、JSONインポートなど\r\n・内容が大きい場合はスクロールバーで全体を確認できます。",
                "【캔버스 조작】\r\n\r\n· 좌클릭 드래그: 요소 이동\r\n· 마우스 휠: 요소 크기 조절\r\n· 우클릭 메뉴: 자동 정렬, 삭제, 지우기, 저장, JSON 가져오기 등\r\n· 내용이 큰 경우 스크롤바로 전체 영역을 확인할 수 있습니다.");
            SetJaKo("HelpContentTemplate",
                "【テンプレートライブラリ】\r\n\r\n・キャンバス右クリック「ライブラリに追加」で現在図面を保存します。\r\n・テンプレートノードをクリックすると内容を読み込みます。\r\n・ノード右クリックでフォルダ追加、名前変更、削除、コピー/貼り付けが可能です。\r\n・上部検索ボックスでテンプレートを絞り込みできます。",
                "【템플릿 라이브러리】\r\n\r\n· 캔버스 우클릭 \"라이브러리에 추가\"로 현재 도면을 저장합니다.\r\n· 템플릿 노드를 클릭하면 내용을 불러옵니다.\r\n· 노드 우클릭으로 폴더 추가, 이름 변경, 삭제, 복사/붙여넣기를 사용할 수 있습니다.\r\n· 상단 검색창에서 템플릿을 필터링할 수 있습니다.");
            SetJaKo("HelpContentCapture",
                "【スクリーンショット】\r\n\r\n・手動キャプチャ: 右クリック > キャプチャ\r\n・自動キャプチャ: 右クリック > 自動キャプチャ（内容範囲を自動計算）\r\n・キャプチャ結果はクリップボードにコピーされ、Word/PPTへ貼り付け可能です。",
                "【스크린샷】\r\n\r\n· 수동 캡처: 우클릭 > 캡처\r\n· 자동 캡처: 우클릭 > 자동 캡처(내용 범위 자동 계산)\r\n· 결과는 클립보드로 복사되며 Word/PPT 등에 붙여넣을 수 있습니다.");
            SetJaKo("HelpContentJsonImport",
                "【JSONインポート】\r\n\r\n・右クリック > JSONインポートを開き、AI生成JSONを貼り付けます。\r\n・「JSON規則をクリップボードへコピー」で最新規則をAIへ渡せます。\r\n・単一JSON、複数JSON連続貼り付け、JSON配列に対応します。\r\n・失敗時はどのJSON/何行/どの装置で失敗したかを詳細表示します。",
                "【JSON 가져오기】\r\n\r\n· 우클릭 > JSON 가져오기를 열고 AI가 생성한 JSON을 붙여넣습니다.\r\n· \"JSON 규격을 클립보드에 복사\"로 최신 규격을 AI에 전달할 수 있습니다.\r\n· 단일 JSON, 연속 다중 JSON, JSON 배열을 지원합니다.\r\n· 실패 시 어떤 JSON/몇 번째 줄/어떤 장치에서 실패했는지 상세 표시합니다.");
            SetJaKo("HelpContentData",
                "【データ保存】\r\n\r\n・template_library.bin: テンプレートデータ\r\n・tagtree_items.bin: タグツリーデータ\r\n・pictures フォルダ: カスタム装置画像\r\n・language.conf: 言語設定\r\n\r\nタグやテンプレートの変更は自動保存されます。移行時はプログラムフォルダ全体をコピーしてください。",
                "【데이터 저장】\r\n\r\n· template_library.bin: 템플릿 데이터\r\n· tagtree_items.bin: 태그 트리 데이터\r\n· pictures 폴더: 사용자 장치 이미지\r\n· language.conf: 언어 설정\r\n\r\n태그/템플릿 변경은 자동 저장됩니다. 다른 PC로 옮길 때는 프로그램 폴더 전체를 복사하세요.");

            // 常見提示與錯誤
            SetJaKo("UserFolderError", "ユーザーフォルダにアクセスできません", "사용자 폴더에 접근할 수 없습니다");
            SetJaKo("ImageProcessError", "画像処理失敗: {0}\n{1}", "이미지 처리 실패: {0}\n{1}");
            SetJaKo("CanvasEmpty", "キャンバスにキャプチャ対象がありません。", "캔버스에 캡처할 내용이 없습니다.");
            SetJaKo("Hint", "ヒント", "힌트");
            SetJaKo("CannotCalculateBounds", "内容境界を計算できません。", "내용 경계를 계산할 수 없습니다.");
            SetJaKo("CaptureSuccess", "スクリーンショットをクリップボードにコピーしました。", "스크린샷을 클립보드에 복사했습니다.");
            SetJaKo("Success", "成功", "성공");
            SetJaKo("CaptureFailed", "キャプチャ失敗: {0}", "캡처 실패: {0}");

            // TagTree
            SetJaKo("AddRootNode", "ルートノード追加", "루트 노드 추가");
            SetJaKo("AddChildNode", "子ノード追加", "하위 노드 추가");
            SetJaKo("DeleteCurrentNode", "現在のノードを削除", "현재 노드 삭제");
            SetJaKo("NewRootNode", "新しいルートノード", "새 루트 노드");
            SetJaKo("NewChildNode", "新しい子ノード", "새 하위 노드");

            // JSON 解析報錯關鍵
            SetJaKo("JsonImportError", "JSONインポート失敗:\n{0}", "JSON 가져오기 실패:\n{0}");
            SetJaKo("JsonNoDevices", "JSON内に装置が見つかりません", "JSON에서 장치를 찾을 수 없습니다");
            SetJaKo("JsonInvalidType", "装置#{0}: 不明なタイプ \"{1}\"", "장치 #{0}: 알 수 없는 타입 \"{1}\"");
            SetJaKo("JsonTopArrayItemNotObject", "トップレベル配列の項目 #{0} はオブジェクトではありません（実際: {1}）。", "최상위 배열 항목 #{0} 이(가) 객체가 아닙니다(실제: {1}).");
            SetJaKo("JsonTopLevelMustObjectOrArray", "トップレベルJSONはオブジェクトまたは配列である必要があります。", "최상위 JSON은 객체 또는 배열이어야 합니다.");
            SetJaKo("JsonNoTopLevelBlocks", "トップレベルJSONオブジェクト/配列ブロックが見つかりません。", "최상위 JSON 객체/배열 블록을 찾을 수 없습니다.");
            SetJaKo("JsonSchemaImportFailed", "Schema #{0} ({1}{2}): {3}", "스키마 #{0} ({1}{2}): {3}");
            SetJaKo("JsonSkippedInvalidBlocks", "無効なJSONブロックをスキップしました:", "유효하지 않은 JSON 블록을 건너뜀:");
            SetJaKo("JsonSchemaDevicesEmpty", "JSON内に装置が見つかりません（schema.devices が空です）。", "JSON에서 장치를 찾을 수 없습니다(schema.devices가 비어 있음).");
            SetJaKo("JsonTypeRequired", "装置 #{0} に必須フィールド \"type\" がありません。", "장치 #{0} 에 필수 필드 \"type\" 이 없습니다.");
            SetJaKo("JsonTypeFieldExample", "各装置オブジェクトには次が必要です: {\"type\": \"device_type_here\", ...}", "각 장치 객체에는 다음이 필요합니다: {\"type\": \"device_type_here\", ...}");
            SetJaKo("JsonUnknownDeviceType", "不明な装置タイプ: \"{0}\"。", "알 수 없는 장치 타입: \"{0}\".");
            SetJaKo("JsonDidYouMean", "もしかして: \"{0}\" ?", "혹시 \"{0}\" ?");
            SetJaKo("JsonExpectedBuiltInTypes", "23種の内蔵タイプのいずれかを指定してください（例: annular_bop, ram_bop, rotary_table, casing_head）。", "23개 내장 타입 중 하나를 사용하세요(예: annular_bop, ram_bop, rotary_table, casing_head).");
            SetJaKo("JsonOrCustomDevices", "または次のカスタム装置:", "또는 다음 사용자 장치:");
            SetJaKo("JsonSeeSchemaTypeList", "JSON規則（上のボタンでコピー）で完全な type リストを確認してください。", "JSON 규격(상단 버튼 복사)에서 전체 type 목록을 확인하세요.");
            SetJaKo("JsonDeviceNoDrawableResource", "装置 #{0} ({1}, devices[{2}]{3}): 描画可能なリソースが見つかりません。", "장치 #{0} ({1}, devices[{2}]{3}): 그릴 수 있는 리소스를 찾을 수 없습니다.");
            SetJaKo("JsonDeviceReadSvgSizeFailed", "装置 #{0} ({1}, devices[{2}]{3}): SVGサイズの取得に失敗しました。", "장치 #{0} ({1}, devices[{2}]{3}): SVG 크기 읽기 실패.");
            SetJaKo("JsonCustomImageNotFound", "装置 #{0} ({1}, devices[{2}]{3}): カスタム画像が見つかりません: {4}", "장치 #{0} ({1}, devices[{2}]{3}): 사용자 이미지 없음: {4}");
            SetJaKo("JsonDeviceRenderFailed", "装置 #{0} ({1}, devices[{2}]{3}): 描画失敗 - {4}", "장치 #{0} ({1}, devices[{2}]{3}): 렌더링 실패 - {4}");
            SetJaKo("JsonEachBlockMustObject", "各JSONブロックはオブジェクトである必要があります。", "각 JSON 블록은 객체여야 합니다.");
            SetJaKo("JsonDevicesFieldNotFound", "\"devices\" フィールドが見つかりません（device/items/equipment/components/stack 等も試行）。", "\"devices\" 필드를 찾을 수 없습니다(device/items/equipment/components/stack 등도 시도).");
            SetJaKo("JsonObjectKeysFound", "このオブジェクトで見つかったキー: {0}", "이 객체에서 찾은 키: {0}");
            SetJaKo("JsonExpectedFormat", "期待形式: { \"devices\": [ {\"type\": \"...\", ...}, ... ] }", "기대 형식: { \"devices\": [ {\"type\": \"...\", ...}, ... ] }");
            SetJaKo("JsonInvalidDeviceItemAtIndex", "devices[{0}] の装置項目が無効です（token type: {1}{2}）。", "devices[{0}] 장치 항목이 잘못되었습니다(token type: {1}{2}).");
            SetJaKo("JsonInvalidDeviceItem", "JSON内の装置項目が無効です（token type: {0}{1}）。", "JSON 장치 항목이 잘못되었습니다(token type: {0}{1}).");
            SetJaKo("JsonUnnamedSchema", "無名-{0}", "이름없음-{0}");
            SetJaKo("JsonLinePosHint", "、行 {0}、位置 {1}", ", 줄 {0}, 위치 {1}");
            SetJaKo("JsonUnknownReaderError", "不明なJSONリーダーエラー。", "알 수 없는 JSON 리더 오류.");
            SetJaKo("JsonSyntaxErrorAtLinePos", "JSON構文エラー（行 {0}, 位置 {1}）:", "JSON 구문 오류(줄 {0}, 위치 {1}):");
            SetJaKo("JsonNearContext", "  付近: ...{0}...", "  주변: ...{0}...");
            SetJaKo("JsonContextHeader", "  コンテキスト:", "  문맥:");
            SetJaKo("JsonCommonFixes", "一般的な修正:", "일반적인 수정:");
            SetJaKo("JsonFixMissingComma", "  - 配列項目またはオブジェクト属性間のカンマ不足を確認してください", "  - 배열 항목/객체 속성 사이의 쉼표 누락 확인");
            SetJaKo("JsonFixTrailingComma", "  - } または ] の前の余分なカンマを確認してください", "  - } 또는 ] 앞의 후행 쉼표 확인");
            SetJaKo("JsonFixDoubleQuotes", "  - 文字列が標準のダブルクォート \" を使っているか確認してください", "  - 문자열이 표준 큰따옴표 \" 를 사용하는지 확인");
            SetJaKo("JsonFixBalancedBrackets", "  - 括弧 {} と [] が対応しているか確認してください", "  - 괄호 {} 및 [] 의 균형 확인");
            SetJaKo("JsonOriginalError", "元のエラー: {0}", "원본 오류: {0}");
            SetJaKo("JsonUnknown", "不明", "알 수 없음");
            SetJaKo("JsonNone", "(なし)", "(없음)");
            SetJaKo("JsonPasteHint", "Excel/画像にJSON規則（上のボタンでコピー）を添えてAIに渡し、生成JSONをここへ貼り付けてください。", "Excel/이미지와 JSON 규격(위 버튼 복사)을 AI에 전달한 뒤 생성된 JSON을 여기에 붙여넣으세요.");
            SetJaKo("CopyJsonSchema", "JSON規則をクリップボードへコピー", "JSON 규격을 클립보드에 복사");
            SetJaKo("Copied", "コピーしました！", "복사됨!");
            SetJaKo("CopyError", "エラーをコピー", "오류 복사");
        }

        private static void SetJaKo(string key, string ja, string ko)
        {
            if (!Strings.TryGetValue(key, out var langDict))
            {
                return;
            }
            langDict[Language.Japanese] = ja;
            langDict[Language.Korean] = ko;
        }

        private static void SetJaKoFromEnglish(string key)
        {
            if (!Strings.TryGetValue(key, out var langDict))
            {
                return;
            }

            if (langDict.TryGetValue(Language.English, out string en))
            {
                langDict[Language.Japanese] = en;
                langDict[Language.Korean] = en;
            }
        }

        public static Language CurrentLanguage
        {
            get { return _currentLanguage; }
        }

        /// <summary>
        /// 內置裝置數量
        /// </summary>
        public static int BuiltInDeviceCount => DeviceNames.Length;

        /// <summary>
        /// 獲取指定索引的本地化裝置名稱
        /// </summary>
        public static string GetDeviceName(int index)
        {
            if (index < 0 || index >= DeviceNames.Length)
                return string.Empty;

            if (_currentLanguage == Language.Japanese)
            {
                return DeviceNamesJapanese[index];
            }

            if (_currentLanguage == Language.Korean)
            {
                return DeviceNamesKorean[index];
            }

            int langIndex;
            switch (_currentLanguage)
            {
                case Language.English: langIndex = 0; break;
                case Language.SimplifiedChinese: langIndex = 1; break;
                case Language.TraditionalChinese: langIndex = 2; break;
                case Language.Spanish: langIndex = 3; break;
                case Language.French: langIndex = 4; break;
                case Language.Portuguese: langIndex = 5; break;
                case Language.Russian: langIndex = 6; break;
                case Language.Persian: langIndex = 7; break;
                case Language.Norwegian: langIndex = 8; break;
                case Language.Arabic: langIndex = 9; break;
                default: langIndex = 0; break;
            }
            return DeviceNames[index][langIndex];
        }

        /// <summary>
        /// 獲取所有本地化裝置名稱
        /// </summary>
        public static string[] GetAllDeviceNames()
        {
            string[] names = new string[DeviceNames.Length];
            for (int i = 0; i < DeviceNames.Length; i++)
            {
                names[i] = GetDeviceName(i);
            }
            return names;
        }

        /// <summary>
        /// 獲取指定 key 的本地化字符串
        /// </summary>
        public static string GetString(string key)
        {
            if (Strings.TryGetValue(key, out var langDict))
            {
                if (langDict.TryGetValue(_currentLanguage, out var value))
                {
                    return value;
                }
                if (langDict.TryGetValue(Language.English, out var fallback))
                {
                    return fallback;
                }
            }
            return key;
        }

        /// <summary>
        /// 獲取格式化的本地化字符串
        /// </summary>
        public static string GetString(string key, params object[] args)
        {
            return string.Format(GetString(key), args);
        }

        /// <summary>
        /// 設置當前語言
        /// </summary>
        public static void SetLanguage(Language language)
        {
            _currentLanguage = language;
            SaveLanguageSetting();
        }

        /// <summary>
        /// 從配置文件讀取語言設置
        /// </summary>
        public static void LoadLanguageSetting()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string content = File.ReadAllText(SettingsPath).Trim();
                    if (Enum.TryParse(content, out Language lang))
                    {
                        _currentLanguage = lang;
                    }
                }
            }
            catch
            {
                _currentLanguage = Language.English;
            }
        }

        /// <summary>
        /// 保存語言設置到配置文件
        /// </summary>
        private static void SaveLanguageSetting()
        {
            try
            {
                File.WriteAllText(SettingsPath, _currentLanguage.ToString());
            }
            catch
            {
                // 忽略保存失敗
            }
        }
    }
}
