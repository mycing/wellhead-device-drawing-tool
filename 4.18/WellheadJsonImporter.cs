using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Svg;

namespace _4._18
{
    /// <summary>
    /// 井口裝置 JSON 匯入器
    /// 負責 JSON 解析、設備類型映射、自動佈局、生成面板控件
    /// </summary>
    public class WellheadJsonImporter
    {
        private readonly Panel _targetPanel;
        private readonly List<Panel> _dynamicPanels;
        private readonly Font _textFont;
        private readonly Dictionary<string, string> _customDevices;
        private readonly string _customDevicePath;

        private const int TOP_MARGIN = 150;
        private const int TEXT_RIGHT_OFFSET = 30;

        /// <summary>
        /// 最近一次成功匯入的 schema（供外部讀取設備信息）
        /// </summary>
        public WellheadJsonSchema LastImportedSchema { get; private set; }
        public List<WellheadJsonSchema> LastImportedSchemas { get; private set; } = new List<WellheadJsonSchema>();
        public string LastImportReport { get; private set; }

        /// <summary>
        /// 設備類型字串 → 內置索引的映射（大小寫不敏感）
        /// </summary>
        private static readonly Dictionary<string, int> DeviceTypeToIndex =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "rotary_table",            0 },
            { "bell_nipple",             1 },
            { "packing_gland",           2 },
            { "mpd_rotating_head",       3 },
            { "temporary_wellhead",      4 },
            { "annular_bop",             5 },
            { "ram_bop",                 6 },
            { "double_ram_bop",          7 },
            { "drilling_spool",          8 },
            { "tubing_spool",            9 },
            { "casing_spool",           10 },
            { "reducer_flange",         11 },
            { "adapter_flange",         12 },
            { "riser",                  13 },
            { "casing_head",            14 },
            { "wellhead_platform",      15 },
            { "diverter",               16 },
            { "single_casing",          17 },
            { "double_casing",          18 },
            { "triple_casing",          19 },
            { "single_bore_double_well",20 },
            { "marine_conductor",       21 },
            { "choke_kill_manifold",    22 }
        };

        private static readonly string[] SchemaNameAliases = new[]
        {
            "name", "well_name", "wellname", "well", "title",
            "井名", "名称", "名稱", "nombre", "nom", "nome", "имя", "نام", "navn", "الاسم"
        };

        private static readonly string[] SchemaDevicesAliases = new[]
        {
            "devices", "device", "items", "equipment", "equipments", "device_list", "entries", "list",
            "components", "component", "stack", "parts", "equipment_list", "equipment_stack",
            "装置", "裝置", "设备", "設備", "装置列表", "設備列表", "组件", "組件",
            "dispositivos", "appareils", "equipamentos", "устройства", "دستگاه‌ها", "enheter", "الأجهزة"
        };

        private static readonly string[] DeviceTypeAliases = new[]
        {
            "type", "device_type", "devicetype", "kind", "model", "category",
            "装置类型", "裝置類型", "类型", "類型", "tipo", "тип", "نوع", "type_de_dispositif"
        };

        private static readonly string[] DeviceLabelAliases = new[]
        {
            "label", "text", "name", "title", "desc", "description", "note",
            "标签", "標籤", "说明", "說明", "descripcion", "descrição", "описание", "توضیح", "beskrivelse", "وصف"
        };

        private static readonly string[] DeviceFlangeAliases = new[]
        {
            "flange", "flange_info", "flangeinfo", "connection", "bolt", "bolts", "spec", "specs",
            "法兰", "法蘭", "法兰信息", "法蘭信息", "连接", "連接", "brida", "bride", "flange_data", "фланец", "فلنج", "flens", "شفة"
        };

        private static readonly Dictionary<string, string> DeviceTypeAliasToCanonical = BuildDeviceTypeAliasMap();

        public WellheadJsonImporter(Panel targetPanel, List<Panel> dynamicPanels, Font textFont)
            : this(targetPanel, dynamicPanels, textFont, null, null)
        {
        }

        public WellheadJsonImporter(Panel targetPanel, List<Panel> dynamicPanels, Font textFont,
            Dictionary<string, string> customDevices, string customDevicePath)
        {
            _targetPanel = targetPanel ?? throw new ArgumentNullException(nameof(targetPanel));
            _dynamicPanels = dynamicPanels ?? throw new ArgumentNullException(nameof(dynamicPanels));
            _textFont = textFont ?? new Font("Microsoft YaHei UI", 13F, FontStyle.Bold);
            _customDevices = customDevices ?? new Dictionary<string, string>();
            _customDevicePath = customDevicePath ?? "";
        }

        /// <summary>
        /// 取得所有支持的設備類型名稱（用於提示/文檔）
        /// </summary>
        public static IEnumerable<string> SupportedTypes => DeviceTypeToIndex.Keys;

        /// <summary>
        /// 匯入 JSON 文字到畫布
        /// </summary>
        /// <param name="jsonText">用戶粘貼的 JSON 文字</param>
        /// <param name="error">失敗時的錯誤信息</param>
        /// <returns>成功返回 true</returns>
        public bool Import(string jsonText, out string error)
        {
            error = null;
            LastImportReport = null;

            List<WellheadJsonSchema> schemas;
            string parseError;
            if (!TryParseSchemas(jsonText, out schemas, out parseError))
            {
                if (!TryParseSchemasBestEffort(jsonText, out schemas, out parseError))
                {
                    error = parseError;
                    LastImportReport = parseError;
                    return false;
                }
            }

            if (schemas == null || schemas.Count == 0)
            {
                error = LocalizationManager.GetString("JsonNoDevices");
                LastImportReport = error;
                return false;
            }

            var imported = new List<WellheadJsonSchema>();
            var failed = new List<string>();
            for (int i = 0; i < schemas.Count; i++)
            {
                string oneError;
                if (!ImportOneSchema(schemas[i], out oneError))
                {
                    string schemaName = GetSchemaDisplayName(schemas[i], i + 1);
                    string schemaLine = GetLineHint(schemas[i]?.SourceLine, schemas[i]?.SourceLinePosition);
                    failed.Add(LocalizationManager.GetString("JsonSchemaImportFailed", i + 1, schemaName, schemaLine, oneError));
                    continue;
                }
                imported.Add(schemas[i]);
            }

            if (!string.IsNullOrWhiteSpace(parseError))
            {
                failed.Insert(0, parseError);
            }

            LastImportedSchemas = imported;
            LastImportedSchema = imported.Count > 0 ? imported[imported.Count - 1] : null;

            if (failed.Count > 0)
            {
                error = string.Join(Environment.NewLine, failed);
                LastImportReport = error;
            }

            return imported.Count > 0;
        }

        private static bool TryParseSchemas(string jsonText, out List<WellheadJsonSchema> schemas, out string error)
        {
            schemas = new List<WellheadJsonSchema>();
            error = null;

            if (string.IsNullOrWhiteSpace(jsonText))
            {
                error = LocalizationManager.GetString("JsonNoDevices");
                return false;
            }

            // 先嘗試「原始文本」解析，避免對本來合法的 JSON 做過度修復造成破壞。
            string rawText = PrepareRawJsonInput(jsonText);
            if (TryParseSchemasCore(rawText, out schemas, out error))
            {
                return true;
            }

            string text = NormalizeJsonInput(jsonText);

            if (TryParseSchemasCore(text, out schemas, out error))
            {
                return true;
            }

            // 第二輪更激進修復：補常見缺失冒號、等號鍵值、更多符號變體。
            string aggressive = NormalizeJsonInputAggressively(text);
            if (TryParseSchemasCore(aggressive, out schemas, out error))
            {
                return true;
            }

            if (schemas.Count == 0)
            {
                error = LocalizationManager.GetString("JsonNoDevices");
                return false;
            }

            return true;
        }

        private static string PrepareRawJsonInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            string text = input.Trim().Trim('\uFEFF');
            text = StripAllCodeFences(text);

            var sb = new StringBuilder(text.Length);
            foreach (char ch in text)
            {
                if (ch == '\u200B' || ch == '\u200C' || ch == '\u200D' || ch == '\u2060')
                {
                    continue;
                }
                if (char.IsControl(ch) && ch != '\r' && ch != '\n' && ch != '\t')
                {
                    continue;
                }
                sb.Append(ch);
            }
            return sb.ToString().Trim();
        }

        private static bool TryParseSchemasCore(string text, out List<WellheadJsonSchema> schemas, out string error)
        {
            schemas = new List<WellheadJsonSchema>();
            error = null;
            try
            {
                using (var sr = new StringReader(text))
                using (var reader = new JsonTextReader(sr))
                {
                    reader.SupportMultipleContent = true;

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.Comment)
                        {
                            continue;
                        }

                        var token = JToken.ReadFrom(reader);
                        if (token == null)
                        {
                            continue;
                        }

                        if (token.Type == JTokenType.Object)
                        {
                            if (!TryParseSchemaFromToken(token, out var schema, out error))
                            {
                                return false;
                            }
                            if (schema != null) schemas.Add(schema);
                        }
                        else if (token.Type == JTokenType.Array)
                        {
                            int itemIndex = 0;
                            foreach (var item in token.Children())
                            {
                                itemIndex++;
                                if (item.Type != JTokenType.Object)
                                {
                                    error = LocalizationManager.GetString("JsonTopArrayItemNotObject", itemIndex, item.Type.ToString());
                                    return false;
                                }
                                if (!TryParseSchemaFromToken(item, out var schema, out error))
                                {
                                    return false;
                                }
                                if (schema != null) schemas.Add(schema);
                            }
                        }
                        else
                        {
                            error = LocalizationManager.GetString("JsonTopLevelMustObjectOrArray");
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (JsonReaderException ex)
            {
                error = BuildJsonReaderError(ex, text);
                return false;
            }
            catch (JsonException ex)
            {
                error = ex.Message;
                return false;
            }
        }

        private static bool TryParseSchemasBestEffort(string jsonText, out List<WellheadJsonSchema> schemas, out string error)
        {
            schemas = new List<WellheadJsonSchema>();
            var issues = new List<string>();

            string normalized = NormalizeJsonInput(jsonText);
            string aggressive = NormalizeJsonInputAggressively(normalized);
            var blocks = ExtractTopLevelBlocks(aggressive);

            if (blocks.Count == 0)
            {
                error = LocalizationManager.GetString("JsonNoTopLevelBlocks");
                return false;
            }

            for (int i = 0; i < blocks.Count; i++)
            {
                var block = blocks[i];
                List<WellheadJsonSchema> oneSchemas;
                string oneErr;
                string blockText = RepairCommonBrokenJson(block.Content);
                if (TryParseSchemasCore(blockText, out oneSchemas, out oneErr))
                {
                    foreach (var schema in oneSchemas)
                    {
                        ApplyLineOffset(schema, block.StartLine - 1);
                        schemas.Add(schema);
                    }
                }
                else
                {
                    issues.Add(LocalizationManager.GetString("JsonBlockErrorAtLine", i + 1, block.StartLine, oneErr));
                }
            }

            if (schemas.Count == 0)
            {
                error = issues.Count > 0 ? string.Join(Environment.NewLine, issues) : LocalizationManager.GetString("JsonNoDevices");
                return false;
            }

            error = issues.Count > 0
                ? LocalizationManager.GetString("JsonSkippedInvalidBlocks") + Environment.NewLine + string.Join(Environment.NewLine, issues)
                : null;
            return true;
        }

        private static string NormalizeJsonInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            string text = input.Trim().Trim('\uFEFF');

            // 支持多個 markdown code fence 包裹的 JSON。
            // AI 經常爲每個 JSON 塊各加一對 ```json ... ```，需要全部提取。
            text = StripAllCodeFences(text);

            // 常见富文本/输入法符号归一化。
            text = text
                .Replace('：', ':')
                .Replace('，', ',')
                .Replace('（', '(')
                .Replace('）', ')');

            // 去掉零宽字符和不可见控制字符（保留 \r\n\t）。
            var sb = new StringBuilder(text.Length);
            foreach (char ch in text)
            {
                if (ch == '\u200B' || ch == '\u200C' || ch == '\u200D' || ch == '\u2060')
                {
                    continue;
                }
                if (char.IsControl(ch) && ch != '\r' && ch != '\n' && ch != '\t')
                {
                    continue;
                }
                sb.Append(ch);
            }
            text = sb.ToString();

            // 去注释 + 统一引号（支持单引号、中文引号）为标准 JSON 双引号。
            text = NormalizeQuotesAndComments(text);

            // 容忍未加引号的 key（如 {name: "..."}）。
            text = Regex.Replace(
                text,
                @"(?<=\{|,)\s*([A-Za-z_\p{L}][A-Za-z0-9_\-\p{L}\p{N}]*)\s*:",
                m => $" \"{m.Groups[1].Value}\":");

            // 容忍尾随逗号。
            text = Regex.Replace(text, ",\\s*([}\\]])", "$1");

            return text.Trim();
        }

        private static string NormalizeJsonInputAggressively(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            // 更多中英文符號變體歸一化
            text = text
                .Replace('\uFF1A', ':') // 全角冒号
                .Replace('\uFE55', ':') // 小型冒号
                .Replace('\u2236', ':') // 比例符号
                .Replace('\uFF0C', ',') // 全角逗号
                .Replace('\uFE50', ',') // 小型逗号
                .Replace('\uFF5B', '{') // 全角左花括号
                .Replace('\uFF5D', '}') // 全角右花括号
                .Replace('\uFF3B', '[') // 全角左方括号
                .Replace('\uFF3D', ']') // 全角右方括号
                .Replace('\uFF02', '"') // 全角双引号
                .Replace('\u201C', '"') // 左双引号（再次确保）
                .Replace('\u201D', '"') // 右双引号（再次确保）
                .Replace('\u2018', '\'') // 左单引号
                .Replace('\u2019', '\''); // 右单引号

            // key = value / key => value
            text = Regex.Replace(text, @"(?<=\{|,)\s*(""[^""]+""|'[^']+'|[A-Za-z_\p{L}][A-Za-z0-9_\-\p{L}\p{N}]*)\s*(=>|=)\s*", m => $" {ToJsonQuotedKey(m.Groups[1].Value)}:");

            // "key" "value" 這類缺失冒號
            text = Regex.Replace(text, @"(?<=\{|,)\s*(""[^""]+""|'[^']+')\s+(?=(""|\{|\[|-?\d|true|false|null))", m => $" {ToJsonQuotedKey(m.Groups[1].Value)}:");

            // 單引號 key 補成雙引號 key
            text = Regex.Replace(text, @"(?<=\{|,)\s*'([^']+)'\s*:", m => $" \"{m.Groups[1].Value}\":");

            // 容忍值後誤輸入的 ')'：例如 "type":"single_bore_double_well"), 
            text = Regex.Replace(text, "\"\\s*\\)\\s*(?=[,\\]}])", "\"");
            text = Regex.Replace(text, "\",\\s*\\)", "\",");
            text = Regex.Replace(text, "\\)\\s*(?=,|\\]|\\})", "");

            // 再清一次尾逗號
            text = Regex.Replace(text, ",\\s*([}\\]])", "$1");
            return text.Trim();
        }

        private static string ToJsonQuotedKey(string rawKey)
        {
            if (string.IsNullOrWhiteSpace(rawKey))
            {
                return "\"\"";
            }

            string key = rawKey.Trim();
            if (key.StartsWith("\"") && key.EndsWith("\"") && key.Length >= 2)
            {
                return key;
            }
            if (key.StartsWith("'") && key.EndsWith("'") && key.Length >= 2)
            {
                key = key.Substring(1, key.Length - 2).Replace("\"", "\\\"");
                return $"\"{key}\"";
            }
            key = key.Replace("\"", "\\\"");
            return $"\"{key}\"";
        }

        private static void ApplyLineOffset(WellheadJsonSchema schema, int offset)
        {
            if (schema == null || offset == 0)
            {
                return;
            }

            if (schema.SourceLine > 0)
            {
                schema.SourceLine += offset;
            }

            if (schema.Devices == null)
            {
                return;
            }

            foreach (var d in schema.Devices)
            {
                if (d != null && d.SourceLine > 0)
                {
                    d.SourceLine += offset;
                }
            }
        }

        private static List<JsonBlock> ExtractTopLevelBlocks(string text)
        {
            var blocks = new List<JsonBlock>();
            if (string.IsNullOrWhiteSpace(text))
            {
                return blocks;
            }

            bool inString = false;
            bool escaping = false;
            char quote = '\0';
            int curly = 0;
            int square = 0;
            int start = -1;
            int startLine = 1;
            int line = 1;

            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];
                if (ch == '\n')
                {
                    line++;
                }

                if (inString)
                {
                    if (escaping)
                    {
                        escaping = false;
                        continue;
                    }
                    if (ch == '\\')
                    {
                        escaping = true;
                        continue;
                    }
                    if (ch == quote)
                    {
                        inString = false;
                        quote = '\0';
                    }
                    continue;
                }

                if (ch == '"' || ch == '\'')
                {
                    inString = true;
                    quote = ch;
                    continue;
                }

                if (start < 0)
                {
                    if (ch == '{' || ch == '[')
                    {
                        start = i;
                        startLine = line;
                        if (ch == '{') curly = 1;
                        else square = 1;
                    }
                    continue;
                }

                if (ch == '{') curly++;
                else if (ch == '}') curly--;
                else if (ch == '[') square++;
                else if (ch == ']') square--;

                if (curly == 0 && square == 0)
                {
                    string content = text.Substring(start, i - start + 1).Trim();
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        blocks.Add(new JsonBlock { Content = content, StartLine = startLine });
                    }
                    start = -1;
                }
            }

            if (start >= 0)
            {
                string tail = text.Substring(start).Trim();
                if (!string.IsNullOrWhiteSpace(tail))
                {
                    blocks.Add(new JsonBlock { Content = tail, StartLine = startLine });
                }
            }

            return blocks;
        }

        private static string RepairCommonBrokenJson(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            // 去掉字符串外的多余 ')'
            var sb = new StringBuilder(text.Length);
            bool inString = false;
            bool escaping = false;
            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];
                if (inString)
                {
                    sb.Append(ch);
                    if (escaping) { escaping = false; continue; }
                    if (ch == '\\') { escaping = true; continue; }
                    if (ch == '"') inString = false;
                    continue;
                }
                if (ch == '"')
                {
                    inString = true;
                    sb.Append(ch);
                    continue;
                }
                if (ch == ')')
                {
                    continue;
                }
                sb.Append(ch);
            }

            string fixedText = sb.ToString();
            fixedText = Regex.Replace(fixedText, ",\\s*([}\\]])", "$1");

            // 自動補齊缺失的 ] / }（按真實嵌套順序反向補齊）
            var stack = new List<char>(128);
            inString = false;
            escaping = false;
            for (int i = 0; i < fixedText.Length; i++)
            {
                char ch = fixedText[i];
                if (inString)
                {
                    if (escaping) { escaping = false; continue; }
                    if (ch == '\\') { escaping = true; continue; }
                    if (ch == '"') inString = false;
                    continue;
                }
                if (ch == '"') { inString = true; continue; }
                if (ch == '{' || ch == '[')
                {
                    stack.Add(ch);
                }
                else if (ch == '}')
                {
                    if (stack.Count > 0 && stack[stack.Count - 1] == '{')
                        stack.RemoveAt(stack.Count - 1);
                }
                else if (ch == ']')
                {
                    if (stack.Count > 0 && stack[stack.Count - 1] == '[')
                        stack.RemoveAt(stack.Count - 1);
                }
            }

            for (int i = stack.Count - 1; i >= 0; i--)
            {
                fixedText += stack[i] == '[' ? "]" : "}";
            }
            return fixedText;
        }

        private static string NormalizeQuotesAndComments(string text)
        {
            var output = new StringBuilder(text.Length);
            bool inString = false;
            bool escaping = false;
            char delimiter = '\0';

            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];
                char next = i + 1 < text.Length ? text[i + 1] : '\0';

                if (!inString)
                {
                    // 去掉 // 行注释
                    if (ch == '/' && next == '/')
                    {
                        i += 2;
                        while (i < text.Length && text[i] != '\n') i++;
                        if (i < text.Length) output.Append('\n');
                        continue;
                    }
                    // 去掉 /* */ 块注释
                    if (ch == '/' && next == '*')
                    {
                        i += 2;
                        while (i + 1 < text.Length && !(text[i] == '*' && text[i + 1] == '/')) i++;
                        i++; // 跳过 '/'
                        continue;
                    }

                    if (IsAnyQuote(ch))
                    {
                        inString = true;
                        delimiter = ch;
                        output.Append('"');
                        continue;
                    }

                    output.Append(ch);
                    continue;
                }

                // inString
                if (escaping)
                {
                    output.Append(ch);
                    escaping = false;
                    continue;
                }

                if (ch == '\\')
                {
                    output.Append(ch);
                    escaping = true;
                    continue;
                }

                // 容忍字符串内部未转义的双引号（如: 1-7/8"×20）。
                // 只有当引号后（忽略空白）是 , } ] : 或文本结束时，才将其视为真正的字符串结束符。
                if (delimiter == '"' && ch == '"')
                {
                    char nextSignificant = GetNextNonWhitespaceChar(text, i + 1);
                    bool isClosingQuote = nextSignificant == '\0'
                        || nextSignificant == ','
                        || nextSignificant == '}'
                        || nextSignificant == ']'
                        || nextSignificant == ':';
                    if (!isClosingQuote)
                    {
                        output.Append("\\\"");
                        continue;
                    }
                }

                if (IsMatchingQuote(ch, delimiter))
                {
                    inString = false;
                    delimiter = '\0';
                    output.Append('"');
                    continue;
                }

                // 单引号字符串里如果出现双引号，需要转义成合法 JSON。
                if (delimiter == '\'' && ch == '"')
                {
                    output.Append("\\\"");
                    continue;
                }

                output.Append(ch);
            }

            return output.ToString();
        }

        private static bool IsAnyQuote(char ch)
        {
            return ch == '"' || ch == '\'' || ch == '“' || ch == '”' || ch == '‘' || ch == '’';
        }

        private static bool IsMatchingQuote(char ch, char delimiter)
        {
            if (delimiter == '"') return ch == '"';
            if (delimiter == '\'') return ch == '\'';
            if (delimiter == '“' || delimiter == '”') return ch == '“' || ch == '”';
            if (delimiter == '‘' || delimiter == '’') return ch == '‘' || ch == '’';
            return ch == delimiter;
        }

        /// <summary>
        /// 剝離所有 markdown 代碼塊標記，提取其中的內容。
        /// 支持 AI 輸出的多個 ```json ... ``` 塊，以及塊之間夾帶的解釋文字。
        /// </summary>
        private static string StripAllCodeFences(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || !text.Contains("```"))
                return text;

            var sb = new StringBuilder(text.Length);
            int i = 0;
            bool foundAnyFence = false;

            while (i < text.Length)
            {
                int fenceStart = text.IndexOf("```", i, StringComparison.Ordinal);
                if (fenceStart < 0)
                {
                    // 沒有更多的 fence
                    if (foundAnyFence)
                    {
                        // fence 之後的尾部文字（AI 的解釋），跳過非 JSON 行
                        // 但如果包含 { 或 [ 則保留（可能是裸 JSON）
                        string tail = text.Substring(i);
                        if (tail.IndexOfAny(new[] { '{', '[' }) >= 0)
                            sb.Append(tail);
                    }
                    else
                    {
                        // 沒有任何 fence，原樣返回
                        return text;
                    }
                    break;
                }

                foundAnyFence = true;

                // 跳過開 fence 行（```json / ```JSON / ``` 等）
                int lineEnd = text.IndexOf('\n', fenceStart);
                if (lineEnd < 0) break; // fence 是最後一行，無內容

                int contentStart = lineEnd + 1;

                // 找關閉 fence
                int closeFence = text.IndexOf("```", contentStart, StringComparison.Ordinal);
                if (closeFence < 0)
                {
                    // 沒有關閉 fence，取剩餘全部
                    sb.Append(text.Substring(contentStart));
                    break;
                }

                // 提取 fence 之間的內容
                sb.Append(text.Substring(contentStart, closeFence - contentStart));
                sb.Append('\n'); // 塊之間加換行分隔

                // 跳過關閉 fence 行
                int closeLineEnd = text.IndexOf('\n', closeFence);
                i = closeLineEnd >= 0 ? closeLineEnd + 1 : text.Length;
            }

            string result = sb.ToString().Trim();
            return string.IsNullOrWhiteSpace(result) ? text : result;
        }

        private static char GetNextNonWhitespaceChar(string text, int startIndex)
        {
            if (string.IsNullOrEmpty(text) || startIndex >= text.Length)
            {
                return '\0';
            }

            for (int i = startIndex; i < text.Length; i++)
            {
                if (!char.IsWhiteSpace(text[i]))
                {
                    return text[i];
                }
            }
            return '\0';
        }

        private sealed class JsonBlock
        {
            public string Content { get; set; }
            public int StartLine { get; set; }
        }

        private bool ImportOneSchema(WellheadJsonSchema schema, out string error)
        {
            error = null;

            if (schema == null || schema.Devices == null || schema.Devices.Count == 0)
            {
                error = LocalizationManager.GetString("JsonSchemaDevicesEmpty");
                return false;
            }

            var resolvedDevices = new List<ResolvedDevice>();
            for (int i = 0; i < schema.Devices.Count; i++)
            {
                var device = schema.Devices[i];
                string deviceLine = GetLineHint(device.SourceLine, device.SourceLinePosition);
                if (string.IsNullOrWhiteSpace(device.Type))
                {
                    var details = new List<string>
                    {
                        string.Format(LocalizationManager.GetString("JsonInvalidType"), i + 1, "(empty)"),
                        LocalizationManager.GetString("JsonTypeRequired", i + 1),
                        LocalizationManager.GetString("JsonTypeFieldExample"),
                        LocalizationManager.GetString("JsonDevicePathHint", i, deviceLine)
                    };
                    error = string.Join(Environment.NewLine, details);
                    return false;
                }

                ResolvedDevice resolved;
                if (!TryResolveDevice(device.Type, out resolved))
                {
                    // 構建詳細的錯誤信息，包含最接近的建議
                    string closest = FindClosestDeviceType(device.Type);
                    string hint = LocalizationManager.GetString("JsonUnknownDeviceType", device.Type);
                    if (!string.IsNullOrEmpty(closest))
                        hint += " " + LocalizationManager.GetString("JsonDidYouMean", closest);
                    hint += Environment.NewLine + LocalizationManager.GetString("JsonExpectedBuiltInTypes");
                    if (_customDevices != null && _customDevices.Count > 0)
                        hint += Environment.NewLine + LocalizationManager.GetString("JsonOrCustomDevices") + " " + string.Join(", ",
                            _customDevices.Keys.Select(k => Path.GetFileNameWithoutExtension(k)).Take(5));
                    hint += Environment.NewLine + LocalizationManager.GetString("JsonSeeSchemaTypeList");
                    error = string.Format(LocalizationManager.GetString("JsonInvalidType"), i + 1, device.Type)
                        + Environment.NewLine + hint + Environment.NewLine + LocalizationManager.GetString("JsonDevicePathHint", i, deviceLine);
                    return false;
                }
                resolvedDevices.Add(resolved);
            }

            _targetPanel.Controls.Clear();
            _dynamicPanels.Clear();

            int currentY = TOP_MARGIN;
            int centerX = _targetPanel.Width / 2;
            int maxBottom = currentY;

            for (int i = 0; i < schema.Devices.Count; i++)
            {
                try
                {
                    var device = schema.Devices[i];
                    string deviceLine = GetLineHint(device.SourceLine, device.SourceLinePosition);
                    var rd = resolvedDevices[i];

                    Size deviceSize;
                    int x;

                    if (!rd.IsCustom)
                    {
                        // ── 內置裝置：SVG 渲染 ──
                        byte[] svgData = GetSvgDataByIndex(rd.BuiltInIndex);
                        if (svgData == null || svgData.Length == 0)
                        {
                            error = LocalizationManager.GetString("JsonDeviceNoDrawableResource", i + 1, device.Type, i, deviceLine);
                            return false;
                        }

                        deviceSize = GetSvgSize(svgData);
                        if (deviceSize.IsEmpty)
                        {
                            error = LocalizationManager.GetString("JsonDeviceReadSvgSizeFailed", i + 1, device.Type, i, deviceLine);
                            return false;
                        }

                        x = centerX - (deviceSize.Width / 2);
                        var svgPanel = new SvgDrawPicturePanel(new Point(x, currentY), svgData, _targetPanel, null, _dynamicPanels)
                        {
                            Size = deviceSize
                        };
                        _targetPanel.Controls.Add(svgPanel);
                        _dynamicPanels.Add(svgPanel);
                    }
                    else
                    {
                        // ── 自定義裝置：位圖渲染 ──
                        string filePath = _customDevices.ContainsKey(rd.CustomFileName)
                            ? _customDevices[rd.CustomFileName]
                            : Path.Combine(_customDevicePath, rd.CustomFileName);

                        if (!File.Exists(filePath))
                        {
                            error = LocalizationManager.GetString("JsonCustomImageNotFound", i + 1, device.Type, i, deviceLine, rd.CustomFileName);
                            return false;
                        }

                        Image img;
                        using (var bmp = new Bitmap(filePath))
                            img = new Bitmap(bmp);

                        deviceSize = img.Size;
                        x = centerX - (deviceSize.Width / 2);
                        var imagePanel = new ImagePicturePanel(new Point(x, currentY), img, _targetPanel, null, _dynamicPanels);
                        _targetPanel.Controls.Add(imagePanel);
                        _dynamicPanels.Add(imagePanel);
                    }

                    // label 和 flange 渲染（內置和自定義通用）
                    if (!string.IsNullOrWhiteSpace(device.Label))
                    {
                        var labelPanel = new DrawstringPanel(Point.Empty, device.Label, _textFont);
                        int labelX = x + (deviceSize.Width / 2) - (labelPanel.Width / 2);
                        int labelY = currentY + (deviceSize.Height / 2) - (labelPanel.Height / 2);
                        labelPanel.Location = new Point(labelX, labelY);
                        _targetPanel.Controls.Add(labelPanel);
                        labelPanel.BringToFront();
                        _dynamicPanels.Add(labelPanel);
                    }

                    if (!string.IsNullOrWhiteSpace(device.Flange))
                    {
                        var flangePanel = new DrawstringPanel(Point.Empty, device.Flange, _textFont);
                        int flangeX = x + deviceSize.Width + TEXT_RIGHT_OFFSET;
                        int flangeY = currentY + (deviceSize.Height / 2) - (flangePanel.Height / 2);
                        if (flangeX + flangePanel.Width > _targetPanel.Width - 10)
                        {
                            int leftX = x - TEXT_RIGHT_OFFSET - flangePanel.Width;
                            flangeX = Math.Max(10, leftX);
                        }
                        flangePanel.Location = new Point(flangeX, flangeY);
                        _targetPanel.Controls.Add(flangePanel);
                        flangePanel.BringToFront();
                        _dynamicPanels.Add(flangePanel);
                    }

                    currentY += deviceSize.Height;
                    maxBottom = Math.Max(maxBottom, currentY + TOP_MARGIN);
                }
                catch (Exception ex)
                {
                    var device = schema.Devices[i];
                    string deviceLine = device == null ? string.Empty : GetLineHint(device.SourceLine, device.SourceLinePosition);
                    error = LocalizationManager.GetString("JsonDeviceRenderFailed", i + 1, device?.Type ?? LocalizationManager.GetString("JsonUnknown"), i, deviceLine, ex.Message);
                    return false;
                }
            }

            // 导入后根据内容自动拉高画布，避免底部内容被截断
            if (maxBottom > _targetPanel.Height)
            {
                _targetPanel.Height = maxBottom;
            }

            _targetPanel.Refresh();
            return true;
        }

        private static bool TryParseSchemaFromToken(JToken token, out WellheadJsonSchema schema, out string error)
        {
            schema = null;
            error = null;

            if (token == null || token.Type != JTokenType.Object)
            {
                error = LocalizationManager.GetString("JsonEachBlockMustObject");
                return false;
            }

            var obj = (JObject)token;
            var parsed = new WellheadJsonSchema
            {
                Name = ReadFirstString(obj, SchemaNameAliases),
                Devices = new List<WellheadDeviceEntry>()
            };

            var devicesToken = ReadFirstToken(obj, SchemaDevicesAliases);
            if (devicesToken == null)
            {
                // 兼容：根对象本身就是单个 device。
                if (TryParseDeviceEntry(obj, out var oneDevice))
                {
                    parsed.Devices.Add(oneDevice);
                    schema = parsed;
                    return true;
                }

                // 列出對象中實際存在的 key，幫助用戶定位問題
                var existingKeys = string.Join(", ", obj.Properties().Select(p => "\"" + p.Name + "\"").Take(8));
                error = LocalizationManager.GetString("JsonNoDevices")
                    + Environment.NewLine + LocalizationManager.GetString("JsonDevicesFieldNotFound")
                    + Environment.NewLine + LocalizationManager.GetString("JsonObjectKeysFound", string.IsNullOrEmpty(existingKeys) ? LocalizationManager.GetString("JsonNone") : existingKeys)
                    + Environment.NewLine + LocalizationManager.GetString("JsonExpectedFormat");
                return false;
            }

            if (devicesToken.Type == JTokenType.Array)
            {
                int itemIndex = 0;
                foreach (var item in devicesToken.Children())
                {
                    itemIndex++;
                    if (!TryParseDeviceEntry(item, out var entry))
                    {
                        var line = item as IJsonLineInfo;
                        error = LocalizationManager.GetString("JsonInvalidDeviceItemAtIndex", itemIndex - 1, item.Type.ToString(), GetLineHint(line?.LineNumber, line?.LinePosition));
                        return false;
                    }
                    parsed.Devices.Add(entry);
                }
            }
            else if (devicesToken.Type == JTokenType.Object || devicesToken.Type == JTokenType.String)
            {
                if (!TryParseDeviceEntry(devicesToken, out var entry))
                {
                    var line = devicesToken as IJsonLineInfo;
                    error = LocalizationManager.GetString("JsonInvalidDeviceItem", devicesToken.Type.ToString(), GetLineHint(line?.LineNumber, line?.LinePosition));
                    return false;
                }
                parsed.Devices.Add(entry);
            }
            else
            {
                error = LocalizationManager.GetString("JsonNoDevices");
                return false;
            }

            if (parsed.Devices.Count == 0)
            {
                error = LocalizationManager.GetString("JsonNoDevices");
                return false;
            }

            var schemaLine = token as IJsonLineInfo;
            parsed.SourceLine = schemaLine != null && schemaLine.HasLineInfo() ? schemaLine.LineNumber : 0;
            parsed.SourceLinePosition = schemaLine != null && schemaLine.HasLineInfo() ? schemaLine.LinePosition : 0;
            schema = parsed;
            return true;
        }

        private static string GetSchemaDisplayName(WellheadJsonSchema schema, int fallbackIndex)
        {
            if (schema == null)
            {
                return LocalizationManager.GetString("JsonUnnamedSchema", fallbackIndex);
            }
            if (!string.IsNullOrWhiteSpace(schema.Name))
            {
                return schema.Name;
            }
            return LocalizationManager.GetString("JsonUnnamedSchema", fallbackIndex);
        }

        private static string GetLineHint(int? line, int? pos)
        {
            if (!line.HasValue || line.Value <= 0)
            {
                return string.Empty;
            }
            int p = pos.GetValueOrDefault(0);
            return LocalizationManager.GetString("JsonLinePosHint", line.Value, p);
        }

        private static string BuildJsonReaderError(JsonReaderException ex, string content)
        {
            if (ex == null)
            {
                return LocalizationManager.GetString("JsonUnknownReaderError");
            }

            int line = ex.LineNumber;
            int pos = ex.LinePosition;
            string near = string.Empty;
            string contextLines = string.Empty;

            if (!string.IsNullOrEmpty(content) && line > 0)
            {
                string[] lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                if (line - 1 < lines.Length)
                {
                    string ln = lines[line - 1];
                    int idx = Math.Max(0, Math.Min(ln.Length - 1, Math.Max(pos - 1, 0)));
                    int start = Math.Max(0, idx - 30);
                    int len = Math.Min(80, ln.Length - start);
                    if (len > 0)
                    {
                        near = ln.Substring(start, len);
                    }

                    // 顯示錯誤行附近的上下文（前後各1行）
                    var ctx = new List<string>();
                    for (int li = Math.Max(0, line - 2); li <= Math.Min(lines.Length - 1, line); li++)
                    {
                        string prefix = (li == line - 1) ? ">>> " : "    ";
                        ctx.Add($"{prefix}L{li + 1}: {lines[li]}");
                    }
                    contextLines = string.Join(Environment.NewLine, ctx);
                }
            }

            var sb = new StringBuilder();
            sb.AppendLine(LocalizationManager.GetString("JsonSyntaxErrorAtLinePos", line, pos));
            if (!string.IsNullOrWhiteSpace(near))
                sb.AppendLine(LocalizationManager.GetString("JsonNearContext", near));
            if (!string.IsNullOrWhiteSpace(contextLines))
            {
                sb.AppendLine(LocalizationManager.GetString("JsonContextHeader"));
                sb.AppendLine(contextLines);
            }
            sb.AppendLine();
            sb.AppendLine(LocalizationManager.GetString("JsonCommonFixes"));
            sb.AppendLine(LocalizationManager.GetString("JsonFixMissingComma"));
            sb.AppendLine(LocalizationManager.GetString("JsonFixTrailingComma"));
            sb.AppendLine(LocalizationManager.GetString("JsonFixDoubleQuotes"));
            sb.AppendLine(LocalizationManager.GetString("JsonFixBalancedBrackets"));
            sb.AppendLine();
            sb.Append(LocalizationManager.GetString("JsonOriginalError", ex.Message));
            return sb.ToString();
        }

        private static bool TryParseDeviceEntry(JToken token, out WellheadDeviceEntry entry)
        {
            entry = null;
            if (token == null)
            {
                return false;
            }

            if (token.Type == JTokenType.String)
            {
                var line = token as IJsonLineInfo;
                entry = new WellheadDeviceEntry
                {
                    Type = token.ToString(),
                    SourceLine = line != null && line.HasLineInfo() ? line.LineNumber : 0,
                    SourceLinePosition = line != null && line.HasLineInfo() ? line.LinePosition : 0
                };
                return true;
            }

            if (token.Type != JTokenType.Object)
            {
                return false;
            }

            var obj = (JObject)token;
            string type = ReadFirstString(obj, DeviceTypeAliases);
            if (string.IsNullOrWhiteSpace(type))
            {
                // 容錯：允許用 label/name 直接寫設備名。
                type = ReadFirstString(obj, DeviceLabelAliases);
            }

            // 清理 type 值：去除前後空白
            if (!string.IsNullOrEmpty(type))
            {
                type = type.Trim();
            }

            entry = new WellheadDeviceEntry
            {
                Type = type,
                Label = ReadFirstString(obj, DeviceLabelAliases),
                Flange = ReadFirstString(obj, DeviceFlangeAliases)
            };
            var objLine = token as IJsonLineInfo;
            entry.SourceLine = objLine != null && objLine.HasLineInfo() ? objLine.LineNumber : 0;
            entry.SourceLinePosition = objLine != null && objLine.HasLineInfo() ? objLine.LinePosition : 0;
            return true;
        }

        private static string ReadFirstString(JObject obj, IEnumerable<string> aliases)
        {
            var token = ReadFirstToken(obj, aliases);
            return token == null ? null : token.ToString();
        }

        private static JToken ReadFirstToken(JObject obj, IEnumerable<string> aliases)
        {
            if (obj == null)
            {
                return null;
            }

            var lookup = obj.Properties()
                .GroupBy(p => NormalizeKey(p.Name))
                .ToDictionary(g => g.Key, g => g.First().Value, StringComparer.Ordinal);

            foreach (var alias in aliases)
            {
                if (lookup.TryGetValue(NormalizeKey(alias), out var token))
                {
                    return token;
                }
            }
            return null;
        }

        /// <summary>
        /// 解析後的裝置信息，區分內置 vs 自定義
        /// </summary>
        private class ResolvedDevice
        {
            public int BuiltInIndex = -1;
            public string CustomFileName;
            public bool IsCustom => BuiltInIndex < 0;
        }

        /// <summary>
        /// 嘗試解析裝置類型（先查內置，再查自定義裝置文件名）
        /// </summary>
        private bool TryResolveDevice(string rawType, out ResolvedDevice resolved)
        {
            resolved = new ResolvedDevice();

            int index;
            if (TryResolveDeviceType(rawType, out index))
            {
                resolved.BuiltInIndex = index;
                return true;
            }

            if (_customDevices != null && _customDevices.Count > 0)
            {
                // 也嘗試去括號版本
                string stripped = StripParenthetical(rawType);
                string normalizedRaw = NormalizeKey(rawType);
                string normalizedStripped = NormalizeKey(stripped);

                foreach (var kvp in _customDevices)
                {
                    string nameNoExt = Path.GetFileNameWithoutExtension(kvp.Key);
                    string normalizedName = NormalizeKey(nameNoExt);

                    if (string.Equals(nameNoExt, rawType, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(kvp.Key, rawType, StringComparison.OrdinalIgnoreCase) ||
                        normalizedName == normalizedRaw ||
                        string.Equals(nameNoExt, stripped, StringComparison.OrdinalIgnoreCase) ||
                        normalizedName == normalizedStripped)
                    {
                        resolved.CustomFileName = kvp.Key;
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool TryResolveDeviceType(string rawType, out int index)
        {
            index = -1;
            if (string.IsNullOrWhiteSpace(rawType))
            {
                return false;
            }

            // 1. 直接匹配（大小寫不敏感）
            if (DeviceTypeToIndex.TryGetValue(rawType, out index))
                return true;

            // 2. NormalizeKey 後查別名表
            string canonical;
            var normalized = NormalizeKey(rawType);
            if (DeviceTypeAliasToCanonical.TryGetValue(normalized, out canonical))
            {
                return DeviceTypeToIndex.TryGetValue(canonical, out index);
            }

            // 3. 去除括號內容再嘗試，如 "annular_bop (环形防喷器)" → "annular_bop"
            string stripped = StripParenthetical(rawType);
            if (!string.Equals(stripped, rawType, StringComparison.Ordinal))
            {
                if (DeviceTypeToIndex.TryGetValue(stripped, out index))
                    return true;
                var strippedNorm = NormalizeKey(stripped);
                if (DeviceTypeAliasToCanonical.TryGetValue(strippedNorm, out canonical))
                    return DeviceTypeToIndex.TryGetValue(canonical, out index);
            }

            // 4. 把空格/連字符替換為下劃線再嘗試，如 "ram-bop" "ram bop" → "ram_bop"
            string underscored = rawType.Trim().Replace(' ', '_').Replace('-', '_');
            if (!string.Equals(underscored, rawType, StringComparison.OrdinalIgnoreCase))
            {
                if (DeviceTypeToIndex.TryGetValue(underscored, out index))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 去除字串中括號及其內容，如 "annular_bop (环形)" → "annular_bop"
        /// </summary>
        private static string StripParenthetical(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;
            // 去除 (...) （...） 等
            string result = Regex.Replace(text, @"\s*[\(（][^)）]*[\)）]", "").Trim();
            return string.IsNullOrWhiteSpace(result) ? text.Trim() : result;
        }

        /// <summary>
        /// 找到與輸入最相似的設備類型名稱（用於錯誤提示）
        /// </summary>
        private string FindClosestDeviceType(string rawType)
        {
            if (string.IsNullOrWhiteSpace(rawType))
                return null;

            string normalizedInput = NormalizeKey(rawType);
            if (string.IsNullOrEmpty(normalizedInput))
                return null;

            string bestMatch = null;
            int bestDist = int.MaxValue;

            foreach (var key in DeviceTypeToIndex.Keys)
            {
                int dist = LevenshteinDistance(normalizedInput, NormalizeKey(key));
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestMatch = key;
                }
            }

            // 也查別名表的中文名
            foreach (var kvp in DeviceTypeAliasToCanonical)
            {
                int dist = LevenshteinDistance(normalizedInput, kvp.Key);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestMatch = kvp.Value; // 返回規範名
                }
            }

            // 只在距離足夠小時才建議（閾值：輸入長度的 50% 或 5，取較小值）
            int threshold = Math.Min(Math.Max(normalizedInput.Length / 2, 2), 5);
            return bestDist <= threshold ? bestMatch : null;
        }

        private static int LevenshteinDistance(string a, string b)
        {
            if (string.IsNullOrEmpty(a)) return b?.Length ?? 0;
            if (string.IsNullOrEmpty(b)) return a.Length;

            int la = a.Length, lb = b.Length;
            var dp = new int[la + 1, lb + 1];
            for (int i = 0; i <= la; i++) dp[i, 0] = i;
            for (int j = 0; j <= lb; j++) dp[0, j] = j;

            for (int i = 1; i <= la; i++)
            {
                for (int j = 1; j <= lb; j++)
                {
                    int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                    dp[i, j] = Math.Min(
                        Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                        dp[i - 1, j - 1] + cost);
                }
            }
            return dp[la, lb];
        }

        private static string NormalizeKey(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            var chars = text.Where(char.IsLetterOrDigit).ToArray();
            return new string(chars).ToLowerInvariant();
        }

        private static Dictionary<string, string> BuildDeviceTypeAliasMap()
        {
            var map = new Dictionary<string, string>(StringComparer.Ordinal);

            void Add(string canonical, params string[] aliases)
            {
                var all = new List<string> { canonical };
                all.AddRange(aliases);
                foreach (var alias in all)
                {
                    var key = NormalizeKey(alias);
                    if (!string.IsNullOrEmpty(key) && !map.ContainsKey(key))
                    {
                        map[key] = canonical;
                    }
                }
            }

            Add("rotary_table", "rotary table", "转盘面", "轉盤面", "转盘", "轉盤");
            Add("bell_nipple", "bell nipple", "喇叭口");
            Add("packing_gland", "packing gland", "packing gland riser sub", "密封盘根升高短节", "密封盤根升高短節", "密封盘根", "密封盤根");
            Add("mpd_rotating_head", "mpd rotating head", "rotating control head", "精细控压旋转控制头", "精細控壓旋轉控制頭", "旋转控制头", "旋轉控制頭", "控压头", "控壓頭");
            Add("temporary_wellhead", "temporary wellhead", "临时井口头", "臨時井口頭", "临时井口", "臨時井口");
            Add("annular_bop", "annular bop", "bop annular", "环形防喷器", "環形防噴器", "万能防喷器", "萬能防噴器", "环形万能防喷器", "環形萬能防噴器");
            Add("ram_bop", "ram bop", "闸板防喷器", "閘板防噴器", "单闸板防喷器", "單閘板防噴器");
            Add("double_ram_bop", "double ram bop", "双闸板防喷器", "雙閘板防噴器");
            Add("drilling_spool", "drilling spool", "钻井四通", "鑽井四通");
            Add("tubing_spool", "tubing spool", "油管四通");
            Add("casing_spool", "casing spool", "套管四通");
            Add("reducer_flange", "reducer flange", "变径法兰", "變徑法蘭");
            Add("adapter_flange", "adapter flange", "变压法兰", "變壓法蘭", "转换法兰", "轉換法蘭");
            Add("riser", "升高立管", "立管");
            Add("casing_head", "casing head", "套管头", "套管頭", "井口头", "井口頭");
            Add("wellhead_platform", "wellhead platform", "井口平台", "平台");
            Add("diverter", "分流器");
            Add("single_casing", "single casing", "单层套管", "單層套管", "套管");
            Add("double_casing", "double casing", "双层套管", "雙層套管", "复合套管", "複合套管");
            Add("triple_casing", "triple casing", "三层套管", "三層套管");
            Add("single_bore_double_well", "single bore double well", "单筒双井", "單筒雙井", "基盘", "基盤");
            Add("marine_conductor", "marine conductor", "隔水导管", "隔水導管", "导管", "導管");
            Add("choke_kill_manifold", "choke and kill manifold", "choke kill manifold", "节流压井管汇", "節流壓井管匯", "节流管汇", "節流管匯", "压井管汇", "壓井管匯");

            return map;
        }

        /// <summary>
        /// 根據內置設備索引獲取 SVG 字節數據
        /// </summary>
        private static byte[] GetSvgDataByIndex(int index)
        {
            return BuiltInDeviceCatalog.GetSvgData(index);
        }

        /// <summary>
        /// 從 SVG 字節數據中解析原始尺寸
        /// </summary>
        private static Size GetSvgSize(byte[] svgData)
        {
            if (svgData == null || svgData.Length == 0)
                return Size.Empty;
            try
            {
                using (var stream = new MemoryStream(svgData))
                {
                    var doc = SvgDocument.Open<SvgDocument>(stream);
                    if (doc != null && doc.Width > 0 && doc.Height > 0)
                        return new Size((int)doc.Width, (int)doc.Height);
                }
            }
            catch { }
            return Size.Empty;
        }
    }
}
