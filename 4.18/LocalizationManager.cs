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
            ["HelpContentQuick"] = new Dictionary<Language, string>
            {
                { Language.English, "Quick Start\\r\\n\\r\\n1) Select a device on the left and click on the canvas to place it.\\r\\n2) Select a label and click the canvas to place text.\\r\\n3) Use right-click menu for align, capture, and save templates." },
                { Language.Spanish, "Inicio rápido\\r\\n\\r\\n1) Seleccione un dispositivo a la izquierda y haga clic en el lienzo para colocarlo.\\r\\n2) Seleccione una etiqueta y haga clic en el lienzo para colocar el texto.\\r\\n3) Use el menú con clic derecho para alinear, capturar y guardar plantillas." },
                { Language.French, "Démarrage rapide\\r\\n\\r\\n1) Sélectionnez un appareil à gauche et cliquez sur la toile pour le placer.\\r\\n2) Sélectionnez une étiquette et cliquez sur la toile pour placer le texte.\\r\\n3) Utilisez le menu du clic droit pour aligner, capturer et enregistrer les modèles." },
                { Language.Portuguese, "Início rápido\\r\\n\\r\\n1) Selecione um dispositivo à esquerda e clique na tela para colocá-lo.\\r\\n2) Selecione um rótulo e clique na tela para colocar o texto.\\r\\n3) Use o menu do botão direito para alinhar, capturar e salvar modelos." },
                { Language.Russian, "Быстрый старт\\r\\n\\r\\n1) Выберите устройство слева и щёлкните по холсту, чтобы разместить его.\\r\\n2) Выберите метку и щёлкните по холсту, чтобы разместить текст.\\r\\n3) Используйте меню правой кнопки для выравнивания, снимков и сохранения шаблонов." },
                { Language.Persian, "شروع سریع\\r\\n\\r\\n1) از سمت چپ یک دستگاه انتخاب کنید و روی بوم کلیک کنید تا قرار بگیرد.\\r\\n2) یک برچسب انتخاب کنید و روی بوم کلیک کنید تا متن قرار گیرد.\\r\\n3) از منوی راست‌کلیک برای تراز، گرفتن تصویر و ذخیره قالب‌ها استفاده کنید." },
                { Language.Norwegian, "Hurtigstart\\r\\n\\r\\n1) Velg en enhet til venstre og klikk på lerretet for å plassere den.\\r\\n2) Velg en etikett og klikk på lerretet for å plassere tekst.\\r\\n3) Bruk høyreklikkmenyen for justering, skjermbilder og lagring av maler." },
                { Language.TraditionalChinese, "【快速開始】\\r\\n\\r\\n請在左側選擇裝置後在畫布點擊放置，選擇標籤後點擊畫布放置文字，右鍵菜單可保存模板與截圖。" },
                { Language.SimplifiedChinese, "【快速开始】\\r\\n\\r\\n在左侧选择装置后在画布点击放置，选择标签后点击画布放置文字，右键菜单可保存模板与截图。" },
                { Language.Arabic, "بدء سريع\\r\\n\\r\\nحدد جهازاً ثم انقر اللوحة لوضعه، حدد تسمية لوضع نص. زر الفأرة الأيمن يوفر الحفظ واللقطات." }
            },
            ["HelpContentDeviceSelect"] = new Dictionary<Language, string>
            {
                { Language.English, "Select & Place\\r\\n\\r\\nClick a device name, move to the canvas, and left-click to place it." },
                { Language.Spanish, "Seleccionar y colocar\\r\\n\\r\\nHaga clic en el nombre de un dispositivo, mueva el cursor al lienzo y haga clic izquierdo para colocarlo." },
                { Language.French, "Sélectionner et placer\\r\\n\\r\\nCliquez sur le nom d’un appareil, déplacez-vous vers la toile et cliquez avec le bouton gauche pour le placer." },
                { Language.Portuguese, "Selecionar e posicionar\\r\\n\\r\\nClique no nome de um dispositivo, vá até a tela e clique com o botão esquerdo para colocá-lo." },
                { Language.Russian, "Выбор и размещение\\r\\n\\r\\nЩёлкните по названию устройства, наведите на холст и щёлкните левой кнопкой, чтобы разместить." },
                { Language.Persian, "انتخاب و قرار دادن\\r\\n\\r\\nروی نام دستگاه کلیک کنید، به بوم بروید و با کلیک چپ آن را قرار دهید." },
                { Language.Norwegian, "Velg og plasser\\r\\n\\r\\nKlikk på enhetsnavnet, gå til lerretet og venstreklikk for å plassere." },
                { Language.TraditionalChinese, "【選擇與放置裝置】\\r\\n\\r\\n點擊裝置名稱後，移到畫布左鍵放置。" },
                { Language.SimplifiedChinese, "【选择与放置装置】\\r\\n\\r\\n点击装置名称后，移到画布左键放置。" },
                { Language.Arabic, "تحديد ووضع\\r\\n\\r\\nاختر الاسم ثم انقر اللوحة لوضعه." }
            },
            ["HelpContentDevicePreview"] = new Dictionary<Language, string>
            {
                { Language.English, "Preview\\r\\n\\r\\nHover a device to see its preview image." },
                { Language.Spanish, "Vista previa\\r\\n\\r\\nPase el cursor sobre un dispositivo para ver su imagen de vista previa." },
                { Language.French, "Aperçu\\r\\n\\r\\nSurvolez un appareil pour voir son image d’aperçu." },
                { Language.Portuguese, "Pré-visualização\\r\\n\\r\\nPasse o mouse sobre um dispositivo para ver a imagem de pré-visualização." },
                { Language.Russian, "Предпросмотр\\r\\n\\r\\nНаведите на устройство, чтобы увидеть изображение предварительного просмотра." },
                { Language.Persian, "پیش‌نمایش\\r\\n\\r\\nنشانگر را روی دستگاه ببرید تا تصویر پیش‌نمایش را ببینید." },
                { Language.Norwegian, "Forhåndsvisning\\r\\n\\r\\nHold pekeren over en enhet for å se forhåndsvisningen." },
                { Language.TraditionalChinese, "【裝置預覽】\\r\\n\\r\\n懸停裝置名稱可顯示預覽圖。" },
                { Language.SimplifiedChinese, "【装置预览】\\r\\n\\r\\n悬停装置名称可显示预览图。" },
                { Language.Arabic, "معاينة\\r\\n\\r\\nمرر المؤشر لرؤية المعاينة." }
            },
            ["HelpContentDeviceCustom"] = new Dictionary<Language, string>
            {
                { Language.English, "Custom Devices\\r\\n\\r\\nRight-click the device list and choose Add Custom Device." },
                { Language.Spanish, "Dispositivos personalizados\\r\\n\\r\\nHaga clic derecho en la lista de dispositivos y elija Agregar dispositivo personalizado." },
                { Language.French, "Appareils personnalisés\\r\\n\\r\\nCliquez avec le bouton droit sur la liste des appareils et choisissez « Ajouter un appareil personnalisé »." },
                { Language.Portuguese, "Dispositivos personalizados\\r\\n\\r\\nClique com o botão direito na lista de dispositivos e escolha \"Adicionar dispositivo personalizado\"." },
                { Language.Russian, "Пользовательские устройства\\r\\n\\r\\nЩёлкните правой кнопкой по списку устройств и выберите «Добавить пользовательское устройство»." },
                { Language.Persian, "دستگاه‌های سفارشی\\r\\n\\r\\nروی فهرست دستگاه‌ها راست‌کلیک کنید و «افزودن دستگاه سفارشی» را انتخاب کنید." },
                { Language.Norwegian, "Egendefinerte enheter\\r\\n\\r\\nHøyreklikk enhetslisten og velg «Legg til egendefinert enhet»." },
                { Language.TraditionalChinese, "【添加自定義裝置】\\r\\n\\r\\n在裝置列表右鍵選擇「添加自繪裝置」。" },
                { Language.SimplifiedChinese, "【添加自定义装置】\\r\\n\\r\\n在装置列表右键选择“添加自绘装置”。" },
                { Language.Arabic, "أجهزة مخصصة\\r\\n\\r\\nانقر بزر الفأرة الأيمن واختر إضافة جهاز مخصص." }
            },
            ["HelpContentDeviceColor"] = new Dictionary<Language, string>
            {
                { Language.English, "Colored / Uncolored\\r\\n\\r\\nToggle the checkbox to switch styles for new devices." },
                { Language.Spanish, "A color / sin color\\r\\n\\r\\nActive o desactive la casilla para cambiar el estilo de los dispositivos nuevos." },
                { Language.French, "En couleur / sans couleur\\r\\n\\r\\nCochez/décochez la case pour changer le style des nouveaux appareils." },
                { Language.Portuguese, "Colorido / sem cor\\r\\n\\r\\nMarque/desmarque a opção para mudar o estilo dos novos dispositivos." },
                { Language.Russian, "Цветные / без цвета\\r\\n\\r\\nПереключите флажок, чтобы менять стиль новых устройств." },
                { Language.Persian, "رنگی / بدون رنگ\\r\\n\\r\\nگزینه را فعال/غیرفعال کنید تا سبک دستگاه‌های جدید تغییر کند." },
                { Language.Norwegian, "Farget / ufarget\\r\\n\\r\\nSlå av/på avmerkingen for å endre stil på nye enheter." },
                { Language.TraditionalChinese, "【上色/未上色切換】\\r\\n\\r\\n勾選切換新放置裝置的樣式。" },
                { Language.SimplifiedChinese, "【上色/未上色切换】\\r\\n\\r\\n勾选切换新放置装置的样式。" },
                { Language.Arabic, "ملون / غير ملون\\r\\n\\r\\nبدّل خيار النمط للأجهزة الجديدة." }
            },
            ["HelpContentTagCreate"] = new Dictionary<Language, string>
            {
                { Language.English, "Create & Edit\\r\\n\\r\\nRight-click in the label tree to add, rename, or delete labels." },
                { Language.Spanish, "Crear y editar\\r\\n\\r\\nHaga clic derecho en el árbol de etiquetas para añadir, renombrar o eliminar etiquetas." },
                { Language.French, "Créer et modifier\\r\\n\\r\\nCliquez avec le bouton droit dans l’arborescence des étiquettes pour ajouter, renommer ou supprimer." },
                { Language.Portuguese, "Criar e editar\\r\\n\\r\\nClique com o botão direito na árvore de rótulos para adicionar, renomear ou excluir." },
                { Language.Russian, "Создать и редактировать\\r\\n\\r\\nЩёлкните правой кнопкой в дереве меток, чтобы добавить, переименовать или удалить." },
                { Language.Persian, "ایجاد و ویرایش\\r\\n\\r\\nدر درخت برچسب‌ها راست‌کلیک کنید تا افزودن، تغییرنام یا حذف انجام شود." },
                { Language.Norwegian, "Opprett og rediger\\r\\n\\r\\nHøyreklikk i etikett-treet for å legge til, gi nytt navn eller slette." },
                { Language.TraditionalChinese, "【創建與編輯標籤】\\r\\n\\r\\n在標籤樹右鍵可新增/重命名/刪除。" },
                { Language.SimplifiedChinese, "【创建与编辑标签】\\r\\n\\r\\n在标签树右键可新增/重命名/删除。" },
                { Language.Arabic, "إنشاء وتحرير\\r\\n\\r\\nانقر بزر الفأرة الأيمن لإضافة أو إعادة تسمية أو حذف." }
            },
            ["HelpContentTagPlace"] = new Dictionary<Language, string>
            {
                { Language.English, "Place Text\\r\\n\\r\\nSelect a label, then click the canvas to place text." },
                { Language.Spanish, "Colocar texto\\r\\n\\r\\nSeleccione una etiqueta y luego haga clic en el lienzo para colocar el texto." },
                { Language.French, "Placer le texte\\r\\n\\r\\nSélectionnez une étiquette, puis cliquez sur la toile pour placer le texte." },
                { Language.Portuguese, "Posicionar texto\\r\\n\\r\\nSelecione um rótulo e depois clique na tela para colocar o texto." },
                { Language.Russian, "Разместить текст\\r\\n\\r\\nВыберите метку, затем щёлкните по холсту, чтобы разместить текст." },
                { Language.Persian, "قراردادن متن\\r\\n\\r\\nیک برچسب را انتخاب کنید، سپس روی بوم کلیک کنید تا متن قرار گیرد." },
                { Language.Norwegian, "Plasser tekst\\r\\n\\r\\nVelg en etikett, og klikk deretter på lerretet for å plassere tekst." },
                { Language.TraditionalChinese, "【放置文字標籤】\\r\\n\\r\\n選中標籤後在畫布點擊放置文字。" },
                { Language.SimplifiedChinese, "【放置文字标签】\\r\\n\\r\\n选中标签后在画布点击放置文字。" },
                { Language.Arabic, "وضع النص\\r\\n\\r\\nحدد تسمية ثم انقر اللوحة." }
            },
            ["HelpContentTagSearch"] = new Dictionary<Language, string>
            {
                { Language.English, "Search Labels\\r\\n\\r\\nType in the search box to filter labels." },
                { Language.Spanish, "Buscar etiquetas\\r\\n\\r\\nEscriba en el cuadro de búsqueda para filtrar etiquetas." },
                { Language.French, "Rechercher des étiquettes\\r\\n\\r\\nSaisissez dans la zone de recherche pour filtrer les étiquettes." },
                { Language.Portuguese, "Pesquisar rótulos\\r\\n\\r\\nDigite na caixa de pesquisa para filtrar rótulos." },
                { Language.Russian, "Поиск меток\\r\\n\\r\\nВведите в поле поиска, чтобы отфильтровать метки." },
                { Language.Persian, "جستجوی برچسب‌ها\\r\\n\\r\\nدر کادر جستجو تایپ کنید تا برچسب‌ها فیلتر شوند." },
                { Language.Norwegian, "Søk i etiketter\\r\\n\\r\\nSkriv i søkefeltet for å filtrere etiketter." },
                { Language.TraditionalChinese, "【搜索標籤】\\r\\n\\r\\n在搜索框輸入可過濾標籤。" },
                { Language.SimplifiedChinese, "【搜索标签】\\r\\n\\r\\n在搜索框输入可过滤标签。" },
                { Language.Arabic, "بحث التسميات\\r\\n\\r\\nاكتب لتصفية التسميات." }
            },
            ["HelpContentCanvasMove"] = new Dictionary<Language, string>
            {
                { Language.English, "Move & Zoom\\r\\n\\r\\nDrag to move. Use mouse wheel to scale items." },
                { Language.Spanish, "Mover y acercar\\r\\n\\r\\nArrastre para mover. Use la rueda del ratón para escalar los elementos." },
                { Language.French, "Déplacer et zoomer\\r\\n\\r\\nFaites glisser pour déplacer. Utilisez la molette pour mettre à l’échelle les éléments." },
                { Language.Portuguese, "Mover e ampliar\\r\\n\\r\\nArraste para mover. Use a roda do mouse para escalar os itens." },
                { Language.Russian, "Перемещение и масштаб\\r\\n\\r\\nПеретаскивайте для перемещения. Используйте колесо мыши для масштабирования элементов." },
                { Language.Persian, "جابجایی و بزرگ‌نمایی\\r\\n\\r\\nبرای جابجایی بکشید. از چرخ ماوس برای تغییر مقیاس استفاده کنید." },
                { Language.Norwegian, "Flytt og zoom\\r\\n\\r\\nDra for å flytte. Bruk musehjulet for å skalere elementer." },
                { Language.TraditionalChinese, "【移動與縮放】\\r\\n\\r\\n拖動可移動，滾輪可縮放。" },
                { Language.SimplifiedChinese, "【移动与缩放】\\r\\n\\r\\n拖动可移动，滚轮可缩放。" },
                { Language.Arabic, "تحريك وتكبير\\r\\n\\r\\nاسحب للتحريك، عجلة للف缩." }
            },
            ["HelpContentCanvasDelete"] = new Dictionary<Language, string>
            {
                { Language.English, "Delete Items\\r\\n\\r\\nRight-click an item and choose Delete." },
                { Language.Spanish, "Eliminar elementos\\r\\n\\r\\nHaga clic derecho en un elemento y elija Eliminar." },
                { Language.French, "Supprimer des éléments\\r\\n\\r\\nCliquez avec le bouton droit sur un élément et choisissez Supprimer." },
                { Language.Portuguese, "Excluir itens\\r\\n\\r\\nClique com o botão direito em um item e escolha Excluir." },
                { Language.Russian, "Удалить элементы\\r\\n\\r\\nЩёлкните правой кнопкой по элементу и выберите «Удалить»." },
                { Language.Persian, "حذف عناصر\\r\\n\\r\\nروی یک مورد راست‌کلیک کنید و حذف را انتخاب کنید." },
                { Language.Norwegian, "Slett elementer\\r\\n\\r\\nHøyreklikk på et element og velg Slett." },
                { Language.TraditionalChinese, "【刪除控件】\\r\\n\\r\\n右鍵控件選擇刪除。" },
                { Language.SimplifiedChinese, "【删除控件】\\r\\n\\r\\n右键控件选择删除。" },
                { Language.Arabic, "حذف العناصر\\r\\n\\r\\nانقر بزر الفأرة الأيمن واختر حذف." }
            },
            ["HelpContentCanvasAlign"] = new Dictionary<Language, string>
            {
                { Language.English, "Auto Align\\r\\n\\r\\nRight-click the canvas and choose Auto Align." },
                { Language.Spanish, "Alinear automáticamente\\r\\n\\r\\nHaga clic derecho en el lienzo y elija Alinear automáticamente." },
                { Language.French, "Alignement automatique\\r\\n\\r\\nCliquez avec le bouton droit sur la toile et choisissez Alignement automatique." },
                { Language.Portuguese, "Alinhar automaticamente\\r\\n\\r\\nClique com o botão direito na tela e escolha Alinhar automaticamente." },
                { Language.Russian, "Автовыравнивание\\r\\n\\r\\nЩёлкните правой кнопкой по холсту и выберите «Автовыравнивание»." },
                { Language.Persian, "تراز خودکار\\r\\n\\r\\nروی بوم راست‌کلیک کنید و تراز خودکار را انتخاب کنید." },
                { Language.Norwegian, "Automatisk justering\\r\\n\\r\\nHøyreklikk på lerretet og velg Automatisk justering." },
                { Language.TraditionalChinese, "【自動對齊】\\r\\n\\r\\n右鍵畫布選擇自動對齊。" },
                { Language.SimplifiedChinese, "【自动对齐】\\r\\n\\r\\n右键画布选择自动对齐。" },
                { Language.Arabic, "محاذاة تلقائية\\r\\n\\r\\nانقر بزر الفأرة الأيمن واختر محاذاة." }
            },
            ["HelpContentCanvasClear"] = new Dictionary<Language, string>
            {
                { Language.English, "Clear Canvas\\r\\n\\r\\nRight-click and choose Clear Canvas." },
                { Language.Spanish, "Limpiar lienzo\\r\\n\\r\\nHaga clic derecho y elija Limpiar lienzo." },
                { Language.French, "Effacer la toile\\r\\n\\r\\nCliquez avec le bouton droit et choisissez Effacer la toile." },
                { Language.Portuguese, "Limpar tela\\r\\n\\r\\nClique com o botão direito e escolha Limpar tela." },
                { Language.Russian, "Очистить холст\\r\\n\\r\\nЩёлкните правой кнопкой и выберите «Очистить холст»." },
                { Language.Persian, "پاک کردن بوم\\r\\n\\r\\nراست‌کلیک کنید و پاک کردن بوم را انتخاب کنید." },
                { Language.Norwegian, "Tøm lerret\\r\\n\\r\\nHøyreklikk og velg Tøm lerret." },
                { Language.TraditionalChinese, "【清空畫布】\\r\\n\\r\\n右鍵畫布選擇清空畫布。" },
                { Language.SimplifiedChinese, "【清空画布】\\r\\n\\r\\n右键画布选择清空画布。" },
                { Language.Arabic, "مسح اللوحة\\r\\n\\r\\nانقر بزر الفأرة الأيمن واختر المسح." }
            },
            ["HelpContentTemplateSave"] = new Dictionary<Language, string>
            {
                { Language.English, "Save Template\\r\\n\\r\\nRight-click canvas and choose Add to Library." },
                { Language.Spanish, "Guardar plantilla\\r\\n\\r\\nHaga clic derecho en el lienzo y elija Añadir a la biblioteca." },
                { Language.French, "Enregistrer le modèle\\r\\n\\r\\nCliquez avec le bouton droit sur la toile et choisissez « Ajouter à la bibliothèque »." },
                { Language.Portuguese, "Salvar modelo\\r\\n\\r\\nClique com o botão direito na tela e escolha \"Adicionar à biblioteca\"." },
                { Language.Russian, "Сохранить шаблон\\r\\n\\r\\nЩёлкните правой кнопкой по холсту и выберите «Добавить в библиотеку»." },
                { Language.Persian, "ذخیره قالب\\r\\n\\r\\nروی بوم راست‌کلیک کنید و «افزودن به کتابخانه» را انتخاب کنید." },
                { Language.Norwegian, "Lagre mal\\r\\n\\r\\nHøyreklikk på lerretet og velg «Legg til i biblioteket»." },
                { Language.TraditionalChinese, "【保存模板】\\r\\n\\r\\n右鍵畫布選擇添加樣例到庫。" },
                { Language.SimplifiedChinese, "【保存模板】\\r\\n\\r\\n右键画布选择添加样例到库。" },
                { Language.Arabic, "حفظ القالب\\r\\n\\r\\nانقر بزر الفأرة الأيمن واختر إضافة إلى المكتبة." }
            },
            ["HelpContentTemplateLoad"] = new Dictionary<Language, string>
            {
                { Language.English, "Load Template\\r\\n\\r\\nClick a template node to load it." },
                { Language.Spanish, "Cargar plantilla\\r\\n\\r\\nHaga clic en un nodo de plantilla para cargarlo." },
                { Language.French, "Charger le modèle\\r\\n\\r\\nCliquez sur un nœud de modèle pour le charger." },
                { Language.Portuguese, "Carregar modelo\\r\\n\\r\\nClique em um nó de modelo para carregá-lo." },
                { Language.Russian, "Загрузить шаблон\\r\\n\\r\\nЩёлкните по узлу шаблона, чтобы загрузить его." },
                { Language.Persian, "بارگذاری قالب\\r\\n\\r\\nبرای بارگذاری روی گره قالب کلیک کنید." },
                { Language.Norwegian, "Last mal\\r\\n\\r\\nKlikk på en mal-node for å laste den." },
                { Language.TraditionalChinese, "【載入模板】\\r\\n\\r\\n單擊模板節點載入。" },
                { Language.SimplifiedChinese, "【加载模板】\\r\\n\\r\\n单击模板节点加载。" },
                { Language.Arabic, "تحميل القالب\\r\\n\\r\\nانقر عقدة القالب للتحميل." }
            },
            ["HelpContentTemplateFolder"] = new Dictionary<Language, string>
            {
                { Language.English, "Manage Folders\\r\\n\\r\\nRight-click templates to add folders or rename." },
                { Language.Spanish, "Administrar carpetas\\r\\n\\r\\nHaga clic derecho en las plantillas para agregar carpetas o renombrar." },
                { Language.French, "Gérer les dossiers\\r\\n\\r\\nCliquez avec le bouton droit sur les modèles pour ajouter des dossiers ou renommer." },
                { Language.Portuguese, "Gerenciar pastas\\r\\n\\r\\nClique com o botão direito nas plantilhas para adicionar pastas ou renomear." },
                { Language.Russian, "Управление папками\\r\\n\\r\\nЩёлкните правой кнопкой по шаблонам, чтобы добавить папки или переименовать." },
                { Language.Persian, "مدیریت پوشه‌ها\\r\\n\\r\\nروی قالب‌ها راست‌کلیک کنید تا پوشه اضافه کنید یا تغییرنام دهید." },
                { Language.Norwegian, "Administrer mapper\\r\\n\\r\\nHøyreklikk på maler for å legge til mapper eller gi nytt navn." },
                { Language.TraditionalChinese, "【管理資料夾】\\r\\n\\r\\n右鍵可新增資料夾或重命名。" },
                { Language.SimplifiedChinese, "【管理文件夹】\\r\\n\\r\\n右键可新增文件夹或重命名。" },
                { Language.Arabic, "إدارة المجلدات\\r\\n\\r\\nانقر بزر الفأرة الأيمن لإضافة مجلد أو إعادة تسمية." }
            },
            ["HelpContentTemplateSearch"] = new Dictionary<Language, string>
            {
                { Language.English, "Search Templates\\r\\n\\r\\nUse the search box above the template tree." },
                { Language.Spanish, "Buscar plantillas\\r\\n\\r\\nUse el cuadro de búsqueda sobre el árbol de plantillas." },
                { Language.French, "Rechercher des modèles\\r\\n\\r\\nUtilisez la zone de recherche au-dessus de l’arborescence des modèles." },
                { Language.Portuguese, "Pesquisar modelos\\r\\n\\r\\nUse a caixa de pesquisa acima da árvore de modelos." },
                { Language.Russian, "Поиск шаблонов\\r\\n\\r\\nИспользуйте поле поиска над деревом шаблонов." },
                { Language.Persian, "جستجوی قالب‌ها\\r\\n\\r\\nاز کادر جستجو بالای درخت قالب‌ها استفاده کنید." },
                { Language.Norwegian, "Søk i maler\\r\\n\\r\\nBruk søkefeltet over mal-treet." },
                { Language.TraditionalChinese, "【搜索模板】\\r\\n\\r\\n使用模板搜索框過濾。" },
                { Language.SimplifiedChinese, "【搜索模板】\\r\\n\\r\\n使用模板搜索框过滤。" },
                { Language.Arabic, "بحث القوالب\\r\\n\\r\\nاستخدم مربع البحث أعلى القوالب." }
            },
            ["HelpContentCaptureManual"] = new Dictionary<Language, string>
            {
                { Language.English, "Manual Capture\\r\\n\\r\\nRight-click canvas and choose Screenshot." },
                { Language.Spanish, "Captura manual\\r\\n\\r\\nHaga clic derecho en el lienzo y elija Captura de pantalla." },
                { Language.French, "Capture manuelle\\r\\n\\r\\nCliquez avec le bouton droit sur la toile et choisissez Capture d’écran." },
                { Language.Portuguese, "Captura manual\\r\\n\\r\\nClique com o botão direito na tela e escolha Captura de tela." },
                { Language.Russian, "Ручной снимок\\r\\n\\r\\nЩёлкните правой кнопкой по холсту и выберите «Снимок экрана»." },
                { Language.Persian, "گرفتن دستی\\r\\n\\r\\nروی بوم راست‌کلیک کنید و «گرفتن اسکرین‌شات» را انتخاب کنید." },
                { Language.Norwegian, "Manuell fangst\\r\\n\\r\\nHøyreklikk på lerretet og velg Skjermbilde." },
                { Language.TraditionalChinese, "【手動截圖】\\r\\n\\r\\n右鍵畫布選擇截圖後拖拽。" },
                { Language.SimplifiedChinese, "【手动截图】\\r\\n\\r\\n右键画布选择截图后拖拽。" },
                { Language.Arabic, "لقطة يدوية\\r\\n\\r\\nانقر بزر الفأرة الأيمن واختر لقطة." }
            },
            ["HelpContentCaptureAuto"] = new Dictionary<Language, string>
            {
                { Language.English, "Auto Capture\\r\\n\\r\\nRight-click canvas and choose Auto Screenshot." },
                { Language.Spanish, "Captura automática\\r\\n\\r\\nHaga clic derecho en el lienzo y elija Captura automática." },
                { Language.French, "Capture automatique\\r\\n\\r\\nCliquez avec le bouton droit sur la toile et choisissez Capture automatique." },
                { Language.Portuguese, "Captura automática\\r\\n\\r\\nClique com o botão direito na tela e escolha Captura automática." },
                { Language.Russian, "Автоснимок\\r\\n\\r\\nЩёлкните правой кнопкой по холсту и выберите «Автоснимок»." },
                { Language.Persian, "گرفتن خودکار\\r\\n\\r\\nروی بوم راست‌کلیک کنید و «اسکرین‌شات خودکار» را انتخاب کنید." },
                { Language.Norwegian, "Automatisk fangst\\r\\n\\r\\nHøyreklikk på lerretet og velg Automatisk skjermbilde." },
                { Language.TraditionalChinese, "【自動截圖】\\r\\n\\r\\n右鍵畫布選擇自動截圖。" },
                { Language.SimplifiedChinese, "【自动截图】\\r\\n\\r\\n右键画布选择自动截图。" },
                { Language.Arabic, "لقطة تلقائية\\r\\n\\r\\nانقر بزر الفأرة الأيمن واختر لقطة تلقائية." }
            },
            ["HelpContentCaptureLong"] = new Dictionary<Language, string>
            {
                { Language.English, "Long Screenshot\\r\\n\\r\\nDrag to capture, keep mouse pressed, scroll to extend." },
                { Language.Spanish, "Captura larga\\r\\n\\r\\nArrastre para capturar, mantenga el botón presionado y desplácese para ampliar." },
                { Language.French, "Capture longue\\r\\n\\r\\nFaites glisser pour capturer, maintenez le bouton enfoncé et faites défiler pour étendre." },
                { Language.Portuguese, "Captura longa\\r\\n\\r\\nArraste para capturar, mantenha o botão pressionado e role para ampliar." },
                { Language.Russian, "Длинный скриншот\\r\\n\\r\\nПеретащите для захвата, удерживайте кнопку и прокручивайте для расширения." },
                { Language.Persian, "اسکرین‌شات بلند\\r\\n\\r\\nبرای گرفتن بکشید، دکمه را نگه دارید و برای گسترش اسکرول کنید." },
                { Language.Norwegian, "Langt skjermbilde\\r\\n\\r\\nDra for å fange, hold knappen nede og rull for å utvide." },
                { Language.TraditionalChinese, "【長圖截取】\\r\\n\\r\\n拖拽選區，保持按住並滾動滾輪擴展。" },
                { Language.SimplifiedChinese, "【长图截取】\\r\\n\\r\\n拖拽选区，保持按住并滚动滚轮扩展。" },
                { Language.Arabic, "لقطة طويلة\\r\\n\\r\\nاسحب وحدد ثم استمر بالتمرير للتوسيع." }
            },
            ["HelpContentSample"] = new Dictionary<Language, string>
            {
                { Language.English, "Samples\\r\\n\\r\\nUse Open Device Sample to view example images." },
                { Language.Spanish, "Muestras\\r\\n\\r\\nUse Abrir ejemplo de dispositivo para ver imágenes de ejemplo." },
                { Language.French, "Exemples\\r\\n\\r\\nUtilisez « Ouvrir un exemple d’appareil » pour voir des images d’exemple." },
                { Language.Portuguese, "Amostras\\r\\n\\r\\nUse \"Abrir amostra do dispositivo\" para ver imagens de exemplo." },
                { Language.Russian, "Примеры\\r\\n\\r\\nИспользуйте «Открыть пример устройства», чтобы посмотреть примеры." },
                { Language.Persian, "نمونه‌ها\\r\\n\\r\\nاز «باز کردن نمونه دستگاه» برای دیدن تصاویر نمونه استفاده کنید." },
                { Language.Norwegian, "Eksempler\\r\\n\\r\\nBruk «Åpne enhetsprøve» for å se eksempelbilder." },
                { Language.TraditionalChinese, "【裝置樣例】\\r\\n\\r\\n右鍵選擇打開裝置樣例查看示例。" },
                { Language.SimplifiedChinese, "【装置样例】\\r\\n\\r\\n右键选择打开装置样例查看示例。" },
                { Language.Arabic, "عينات\\r\\n\\r\\nافتح عينات الجهاز لعرض الأمثلة." }
            },
            ["HelpContentData"] = new Dictionary<Language, string>
            {
                { Language.English, "Data Files\\r\\n\\r\\nData is stored in the app folder (templates, tags, pictures)." },
                { Language.Spanish, "Archivos de datos\\r\\n\\r\\nLos datos se guardan en la carpeta de la aplicación (plantillas, etiquetas, imágenes)." },
                { Language.French, "Fichiers de données\\r\\n\\r\\nLes données sont stockées dans le dossier de l’application (modèles, étiquettes, images)." },
                { Language.Portuguese, "Arquivos de dados\\r\\n\\r\\nOs dados são armazenados na pasta do aplicativo (modelos, rótulos, imagens)." },
                { Language.Russian, "Файлы данных\\r\\n\\r\\nДанные хранятся в папке приложения (шаблоны, метки, изображения)." },
                { Language.Persian, "فایل‌های داده\\r\\n\\r\\nداده‌ها در پوشه برنامه ذخیره می‌شوند (قالب‌ها، برچسب‌ها، تصاویر)." },
                { Language.Norwegian, "Datafiler\\r\\n\\r\\nData lagres i program-mappen (maler, etiketter, bilder)." },
                { Language.TraditionalChinese, "【數據說明】\\r\\n\\r\\n數據保存在程序目錄（模板、標籤、圖片）。" },
                { Language.SimplifiedChinese, "【数据说明】\\r\\n\\r\\n数据保存在程序目录（模板、标签、图片）。" },
                { Language.Arabic, "ملفات البيانات\\r\\n\\r\\nيتم حفظ البيانات في مجلد البرنامج." }
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
