using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace _4._18
{
    /// <summary>
    /// JSON 匯入對話框
    /// 提供一個大文本框讓用戶粘貼 AI 生成的 JSON，點確定後匯入到畫布
    /// </summary>
    internal sealed class JsonImportDialog
    {
        private readonly WellheadJsonImporter _importer;
        private readonly Dictionary<string, string> _customDevices;

        public JsonImportDialog(WellheadJsonImporter importer)
            : this(importer, null)
        {
        }

        public JsonImportDialog(WellheadJsonImporter importer, Dictionary<string, string> customDevices)
        {
            _importer = importer ?? throw new ArgumentNullException(nameof(importer));
            _customDevices = customDevices ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// 顯示匯入對話框
        /// </summary>
        /// <param name="owner">父窗口</param>
        /// <returns>匯入成功返回 true</returns>
        public bool ShowDialog(IWin32Window owner)
        {
            const float baselineDpi = 216f;
            int dpi = 216;
            if (owner is Control c && c.DeviceDpi > 0)
                dpi = c.DeviceDpi;
            float scale = dpi / baselineDpi;
            Func<int, int> s = v => (int)Math.Ceiling(v * scale);
            Func<float, float> sf = v => v * scale;

            int pad   = s(20);
            int gap   = s(10);
            int formW = s(560);
            int txtH  = s(340);
            int btnH  = s(46);
            int btnW  = (formW - pad * 2 - gap) / 2;
            int hintH = s(28);

            int copyBtnH = s(40);
            int innerW   = formW - pad * 2;

            int txtTop   = pad;
            int copyTop  = txtTop + txtH + gap;
            int hintTop  = copyTop + copyBtnH + gap;
            int btnTop   = hintTop + hintH + gap;
            int formH    = btnTop + btnH + pad;

            using (Form form = new Form())
            {
                form.Text = LocalizationManager.GetString("ImportJson");
                form.StartPosition = FormStartPosition.CenterParent;
                form.AutoScaleMode = AutoScaleMode.None;
                form.ClientSize = new Size(formW, formH);
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                TextBox jsonBox = new TextBox
                {
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    AcceptsReturn = true,
                    AcceptsTab = true,
                    WordWrap = false,
                    Font = new Font("Consolas", sf(9F)),
                    Left = pad,
                    Top = txtTop,
                    Width = formW - pad * 2,
                    Height = txtH
                };

                Button copySchemaBtn = new Button
                {
                    Text = LocalizationManager.GetString("CopyJsonSchema"),
                    Left = pad,
                    Top = copyTop,
                    Width = innerW,
                    Height = copyBtnH,
                    Font = new Font("Microsoft YaHei UI", sf(9F)),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(45, 120, 180),
                    ForeColor = Color.White,
                    Cursor = Cursors.Hand
                };
                copySchemaBtn.FlatAppearance.BorderSize = 0;
                copySchemaBtn.Click += (s2, ev) =>
                {
                    Clipboard.SetText(GetSchemaText());
                    string origText = copySchemaBtn.Text;
                    copySchemaBtn.Text = LocalizationManager.GetString("Copied");
                    Timer timer = new Timer { Interval = 1500 };
                    timer.Tick += (s3, ev3) =>
                    {
                        copySchemaBtn.Text = origText;
                        timer.Stop();
                        timer.Dispose();
                    };
                    timer.Start();
                };

                // 滾動提示標籤
                string hintText = LocalizationManager.GetString("JsonPasteHint");
                Font hintFont = new Font("Microsoft YaHei UI", sf(9F), FontStyle.Bold);
                Panel hintPanel = new DoubleBufferedPanel
                {
                    Left = pad,
                    Top = hintTop,
                    Width = innerW,
                    Height = hintH,
                    BackColor = form.BackColor
                };

                int scrollOffset = 0;
                int hintTextWidth = 0;
                int loopGap = s(12);
                Timer scrollTimer = null;

                hintPanel.Paint += (s2, pe) =>
                {
                    pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    pe.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    using (Brush brush = new SolidBrush(Color.FromArgb(45, 45, 45)))
                    {
                        float y = (hintPanel.Height - hintFont.Height) / 2f;
                        float x1 = -scrollOffset;
                        float x2 = x1 + hintTextWidth + loopGap;
                        pe.Graphics.DrawString(hintText, hintFont, brush, x1, y);
                        pe.Graphics.DrawString(hintText, hintFont, brush, x2, y);
                    }
                };

                hintPanel.HandleCreated += (s2, ev) =>
                {
                    using (Graphics g = hintPanel.CreateGraphics())
                    {
                        hintTextWidth = (int)Math.Ceiling(g.MeasureString(hintText, hintFont).Width);
                    }
                    if (hintTextWidth > innerW)
                    {
                        scrollTimer = new Timer { Interval = 30 };
                        scrollTimer.Tick += (s3, ev3) =>
                        {
                            scrollOffset += 2;
                            if (scrollOffset > hintTextWidth + loopGap)
                                scrollOffset = 0;
                            hintPanel.Invalidate();
                        };
                        scrollTimer.Start();
                    }
                };

                Button okButton = new Button
                {
                    Text = LocalizationManager.GetString("OK"),
                    Left = pad,
                    Top = btnTop,
                    Width = btnW,
                    Height = btnH,
                    Font = new Font("Microsoft YaHei UI", sf(10F)),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(60, 63, 70),
                    ForeColor = Color.White
                };
                okButton.FlatAppearance.BorderSize = 0;

                Button cancelButton = new Button
                {
                    Text = LocalizationManager.GetString("Cancel"),
                    Left = pad + btnW + gap,
                    Top = btnTop,
                    Width = btnW,
                    Height = btnH,
                    Font = new Font("Microsoft YaHei UI", sf(10F)),
                    FlatStyle = FlatStyle.Flat
                };
                cancelButton.FlatAppearance.BorderColor = Color.FromArgb(180, 184, 190);

                bool imported = false;

                okButton.Click += (s2, ev) =>
                {
                    string text = jsonBox.Text.Trim();
                    if (string.IsNullOrEmpty(text))
                    {
                        MessageBox.Show(
                            LocalizationManager.GetString("JsonPasteHint"),
                            LocalizationManager.GetString("ImportJson"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return;
                    }

                    string error;
                    if (_importer.Import(text, out error))
                    {
                        if (!string.IsNullOrWhiteSpace(error))
                        {
                            ShowCopyableErrorDialog(
                                form,
                                LocalizationManager.GetString("ImportJson"),
                                error,
                                MessageBoxIcon.Warning);
                        }
                        imported = true;
                        form.Close();
                    }
                    else
                    {
                        ShowCopyableErrorDialog(
                            form,
                            LocalizationManager.GetString("ImportJson"),
                            string.Format(LocalizationManager.GetString("JsonImportError"), error),
                            MessageBoxIcon.Warning);
                    }
                };

                cancelButton.Click += (s2, ev) =>
                {
                    form.Close();
                };

                form.AcceptButton = okButton;
                form.CancelButton = cancelButton;
                form.Controls.AddRange(new Control[]
                {
                    jsonBox, copySchemaBtn, hintPanel, okButton, cancelButton
                });

                form.FormClosed += (s2, ev) =>
                {
                    scrollTimer?.Stop();
                    scrollTimer?.Dispose();
                    hintFont.Dispose();
                };

                form.ShowDialog(owner);
                return imported;
            }
        }

        private static void ShowCopyableErrorDialog(IWin32Window owner, string title, string message, MessageBoxIcon icon)
        {
            const float baselineDpi = 216f;
            int dpi = 216;
            if (owner is Control oc && oc.DeviceDpi > 0)
                dpi = oc.DeviceDpi;
            float scale = dpi / baselineDpi;
            Func<int, int> sc = v => (int)Math.Ceiling(v * scale);
            Func<float, float> scf = v => v * scale;

            int pad  = sc(16);
            int gap  = sc(10);
            int hdrH = sc(36);
            int btnH = sc(42);
            int btnW = sc(100);

            using (Form dlg = new Form())
            {
                dlg.Text = title;
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.Sizable;
                dlg.MinimizeBox = false;
                dlg.MaximizeBox = true;
                dlg.ShowInTaskbar = false;
                dlg.BackColor = Color.FromArgb(242, 244, 247);
                dlg.ClientSize = new Size(sc(620), sc(480));
                dlg.MinimumSize = new Size(sc(400), sc(300));
                dlg.AutoScaleMode = AutoScaleMode.None;

                // 頂部標題
                Label header = new Label
                {
                    Text = title,
                    Dock = DockStyle.Top,
                    Height = hdrH,
                    Font = new Font("Microsoft YaHei UI", scf(10F), FontStyle.Bold),
                    ForeColor = icon == MessageBoxIcon.Warning
                        ? Color.FromArgb(180, 120, 0)
                        : Color.FromArgb(165, 35, 35),
                    Padding = new Padding(pad, 0, pad, 0),
                    TextAlign = ContentAlignment.MiddleLeft
                };

                // 底部按鈕面板
                Panel bottomPanel = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = btnH + pad * 2,
                    BackColor = dlg.BackColor
                };

                Button btnCopy = new Button
                {
                    Text = LocalizationManager.GetString("CopyError"),
                    Width = btnW,
                    Height = btnH,
                    Font = new Font("Microsoft YaHei UI", scf(9F)),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(55, 120, 180),
                    ForeColor = Color.White,
                    Cursor = Cursors.Hand,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                btnCopy.FlatAppearance.BorderSize = 0;
                btnCopy.Click += (s2, e2) =>
                {
                    Clipboard.SetText(message ?? string.Empty);
                    string origText = btnCopy.Text;
                    btnCopy.Text = LocalizationManager.GetString("Copied");
                    Timer t = new Timer { Interval = 1500 };
                    t.Tick += (s3, e3) => { btnCopy.Text = origText; t.Stop(); t.Dispose(); };
                    t.Start();
                };

                Button btnOk = new Button
                {
                    Text = LocalizationManager.GetString("OK"),
                    Width = btnW,
                    Height = btnH,
                    Font = new Font("Microsoft YaHei UI", scf(9F)),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(60, 63, 70),
                    ForeColor = Color.White,
                    Cursor = Cursors.Hand,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                btnOk.FlatAppearance.BorderSize = 0;
                btnOk.Click += (s2, e2) => dlg.Close();

                // 按鈕靠右排列
                btnOk.Left = bottomPanel.ClientSize.Width - pad - btnW;
                btnCopy.Left = btnOk.Left - gap - btnW;
                btnOk.Top = pad;
                btnCopy.Top = pad;
                bottomPanel.Controls.Add(btnCopy);
                bottomPanel.Controls.Add(btnOk);

                // 中間可複製文本框（自動填滿剩餘空間）
                TextBox box = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Both,
                    WordWrap = true,
                    Dock = DockStyle.Fill,
                    Font = new Font("Consolas", scf(9.5F)),
                    Text = message,
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };

                // Dock 順序：先 Dock 的佔位優先
                dlg.Controls.Add(box);          // Fill — 最後加，佔剩餘
                dlg.Controls.Add(bottomPanel);  // Bottom
                dlg.Controls.Add(header);        // Top

                dlg.AcceptButton = btnOk;
                dlg.ShowDialog(owner);
            }
        }

        /// <summary>
        /// 生成 JSON schema 說明文字，供用戶複製後發給任意 AI
        /// </summary>
        private string GetSchemaText()
        {
            if (LocalizationManager.CurrentLanguage == Language.Japanese)
            {
                return BuildJapaneseSchemaText();
            }

            if (LocalizationManager.CurrentLanguage == Language.Korean)
            {
                return BuildKoreanSchemaText();
            }

            string baseSchema = @"=== Wellhead Device Stack Drawing - JSON Schema ===

You are generating JSON data for a wellhead device stack drawing tool.
Devices are stacked vertically from TOP to BOTTOM in the order they appear in the array.
The importer will parse your JSON and render each device as a graphical component on the canvas.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CRITICAL RULES (violations cause import failure)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
1) Output ONLY raw JSON. No markdown, no ```json``` code fences, no explanations, no comments.
2) Each JSON object MUST have a ""devices"" array with at least 1 item.
3) Each device MUST have a ""type"" field with a value from the type list below.
4) The ""type"" value must EXACTLY match one of the listed type identifiers (case-insensitive).
   WRONG: ""Annular BOP""  →  RIGHT: ""annular_bop""
   WRONG: ""Ram-BOP""      →  RIGHT: ""ram_bop""
   WRONG: ""闸板防喷器""    →  RIGHT: ""ram_bop""
5) Use standard ASCII double quotes "" only. No smart/curly quotes ("" "" '' '').
6) No trailing commas after the last item in arrays or objects.
7) No parentheses ')' in property values (common typo that breaks JSON).
8) No single-line // or block /* */ comments inside JSON.

## OUTPUT FORMAT
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
For a SINGLE drawing, output one JSON object:
{
  ""name"": ""..."",
  ""devices"": [...]
}

For MULTIPLE drawings, output consecutive JSON objects (NOT wrapped in an array):
{""name"": ""A"", ""devices"": [...]}

{""name"": ""B"", ""devices"": [...]}

Do NOT wrap multiple drawings in a top-level array [] unless explicitly requested.

## FIELD REFERENCE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
### Top-level object:
  ""name""    : string (optional) - Drawing/schema name, e.g. ""二开 12-1/4"" BOP Stack""
  ""devices"" : array  (REQUIRED) - Non-empty array of device objects

### Each device object:
  ""type""    : string (REQUIRED) - Device type identifier. MUST be one of the types listed below.
  ""label""   : string (optional) - Text displayed centered ON the device image.
                                    Use \\n (literal backslash-n in JSON string) for line breaks.
                                    Example: ""346.1mm（13-5/8\""""）万能防喷器\\n34.5MPa（5000 psi）""
  ""flange""  : string (optional) - Flange specification displayed to the RIGHT of the device.
                                    Use \\n for line breaks.
                                    Example: ""13-5/8\"" 5M BX160\\n螺栓：1-5/8\""""×16""

### Important: \\n in label/flange
  In JSON strings, a line break is represented as the two-character escape \\n.
  This is a literal backslash followed by 'n' inside the JSON string.
  The tool will render it as an actual line break on the canvas.

## AVAILABLE DEVICE TYPES — 23 built-in types
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Use EXACTLY these identifiers as the ""type"" value (case-insensitive):

  Type Identifier             Chinese Name              English Name
  ─────────────────────────   ─────────────────────────  ──────────────────────────
  rotary_table                转盘面                     Rotary Table
  bell_nipple                 喇叭口                     Bell Nipple
  packing_gland               密封盘根升高短节           Packing Gland Riser Sub
  mpd_rotating_head           精细控压旋转控制头         MPD Rotating Control Head
  temporary_wellhead          临时井口头                 Temporary Wellhead
  annular_bop                 环形(万能)防喷器           Annular BOP
  ram_bop                     闸板防喷器                 Ram BOP
  double_ram_bop              双闸板防喷器               Double Ram BOP
  drilling_spool              钻井四通                   Drilling Spool
  tubing_spool                油管四通                   Tubing Spool
  casing_spool                套管四通                   Casing Spool
  reducer_flange              变径法兰                   Reducer Flange
  adapter_flange              变压法兰                   Adapter Flange
  riser                       升高立管                   Riser
  casing_head                 套管头                     Casing Head
  wellhead_platform           井口平台                   Wellhead Platform
  diverter                    分流器                     Diverter
  single_casing               单层套管                   Single Casing
  double_casing               双层套管(复合套管)         Double Casing
  triple_casing               三层套管                   Triple Casing
  single_bore_double_well     单筒双井(基盘)             Single Bore Double Well
  marine_conductor            隔水导管                   Marine Conductor
  choke_kill_manifold         节流压井管汇               Choke & Kill Manifold

NOTE: The same device type can appear multiple times in one stack
      (e.g., two ram_bop in a row, or multiple single_casing for different casing sizes).

## COMMON AI MISTAKES — AVOID THESE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
✗ Using Chinese device names as type:  ""type"": ""闸板防喷器""
  → Use the English identifier:       ""type"": ""ram_bop""

✗ Using spaces instead of underscores: ""type"": ""ram bop""
  → Use underscores:                   ""type"": ""ram_bop""

✗ Using hyphens instead of underscores: ""type"": ""ram-bop""
  → Use underscores:                    ""type"": ""ram_bop""

✗ Adding extra description in type:    ""type"": ""annular_bop (环形防喷器)""
  → Type must be the identifier only:  ""type"": ""annular_bop""

✗ Wrapping JSON in markdown code fence: ```json { ... } ```
  → Output raw JSON only:              { ... }

✗ Adding trailing comma:               ""devices"": [ {...}, {...}, ]
  → Remove trailing comma:             ""devices"": [ {...}, {...} ]

✗ Using actual newlines in label strings instead of \\n:
  ""label"": ""Line1
  Line2""
  → Use \\n escape: ""label"": ""Line1\\nLine2""

✗ Forgetting to escape double quotes inside strings:
  ""label"": ""13-5/8"" casing""
  → Escape with backslash: ""label"": ""13-5/8\"" casing""

## SELF-CHECK BEFORE OUTPUT
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Before outputting, verify:
  [ ] Every JSON object has a ""devices"" array with length > 0
  [ ] Every device has a non-empty ""type"" matching the type list above
  [ ] All brackets {} and [] are properly balanced and matched
  [ ] No trailing commas before } or ]
  [ ] No stray parentheses ')' near property values
  [ ] All strings use standard double quotes ""
  [ ] Line breaks in label/flange use \\n, not actual newlines
  [ ] Double quotes inside strings are escaped as \""
  [ ] Output is pure JSON with no surrounding text or markdown

## COMMON BORE SIZES
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  13-5/8"" (346.1mm)  — most common
  18-3/4"" (476.25mm) — high-pressure / large bore
  11""     (279.4mm)  — intermediate
  21-1/4"" (539.75mm) — surface casing
  20-3/4"" (527.05mm) — conductor

## LABEL FORMAT GUIDELINES
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Device labels typically: bore_size + device_chinese_name + pressure_rating
  Full format:    ""346.1mm（13-5/8\""""）万能防喷器\n34.5MPa（5000 psi）""
  Medium format:  ""346.1mm（13-5/8\""""）闸板防喷器\n68.9MPa（10000 psi）""
  Simple format:  ""转盘面"", ""变压法兰"", ""变径法兰"", ""井口平台""
  Casing labels:  ""13-3/8\""""套管"", ""9-5/8\""""套管"", ""7\""""尾管""
  Conductor:      ""20\""""隔水导管"", ""24\""""隔水导管"", ""36\""""隔水导管""
  Composite:      ""20\""+13-3/8\""""套管""

## FLANGE SPECIFICATION FORMAT
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Format: ""[bore] [pressure_class] [ring_gasket]\n螺栓：[bolt_size]×[count]""

Common specifications:
  13-5/8\"" 5M BX160     螺栓：1-5/8\""""×16
  13-5/8\"" 10M BX159    螺栓：1-7/8\""""×20
  13-5/8\"" 15M BX159    螺栓：2-1/4\""""×20
  13-5/8\"" 3M R57       螺栓：1-3/8\""""×20
  11\"" 10M BX158         螺栓：1-3/4\""""×16
  11\"" 5M R54            螺栓：1-7/8\""""×12
  11\"" 3M R53            螺栓：1-3/8\""""×16
  21-1/4\"" 2M R73        螺栓：1-5/8\""""×24
  21-1/4\"" 5M BX165      螺栓：2\""""×24
  18-3/4\"" 15M BX164     螺栓：3\""""×20
  18-3/4\"" 10M BX164     螺栓：2-1/4\""""×24
  20-3/4\"" 3M R74        螺栓：2\""""×20

When top/bottom flanges differ, list both with \\n separating:
  ""13-5/8\"" 5M BX160\n螺栓：1-5/8\""""×16\n13-5/8\"" 10M BX159\n螺栓：1-7/8\""""×20""

## PRESSURE CLASSES
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  2M =  2000 psi = 13.8 MPa
  3M =  3000 psi = 20.7 MPa
  5M =  5000 psi = 34.5 MPa
  10M = 10000 psi = 68.9 MPa
  15M = 15000 psi = 103.4 MPa

## TYPICAL DEVICE STACK ORDER (top → bottom)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
1st Spud (一开):
  rotary_table → annular_bop/diverter → marine_conductor

2nd Spud (二开 12.25""):
  rotary_table → annular_bop → adapter_flange → ram_bop →
  drilling_spool → casing_head → wellhead_platform →
  single_casing(13-3/8"") → marine_conductor

3rd Spud (三开 8.5""):
  rotary_table → annular_bop → adapter_flange → ram_bop →
  drilling_spool → adapter_flange → casing_spool →
  reducer_flange → casing_head → wellhead_platform →
  single_casing(13-3/8"") → single_casing(9-5/8"") → marine_conductor

Testing (测试/试油):
  Similar to 3rd spud but may add tubing_spool above drilling_spool

## MINIMAL VALID EXAMPLE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
{""devices"":[{""type"":""rotary_table""},{""type"":""annular_bop""},{""type"":""casing_head""}]}

## EXAMPLE 1: Standard 2nd Spud (二开 12.25"", 10000 psi)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
{
  ""name"": ""92 Series - 2nd Spud 12.25"",
  ""devices"": [
    { ""type"": ""rotary_table"", ""label"": ""转盘面"" },
    { ""type"": ""annular_bop"", ""label"": ""346.1mm（13-5/8\""）万能防喷器\n34.5MPa（5000 psi）"", ""flange"": ""13-5/8\"" 5M BX160\n螺栓：1-5/8\""×16\n13-5/8\"" 10M BX159\n螺栓：1-7/8\""×20"" },
    { ""type"": ""adapter_flange"", ""label"": ""346.1mm（13-5/8\""）变压法兰"", ""flange"": ""13-5/8\"" 10M BX159\n螺栓：1-7/8\""×20"" },
    { ""type"": ""ram_bop"", ""label"": ""346.1mm（13-5/8\""）闸板防喷器\n68.9MPa（10000 psi）"", ""flange"": ""13-5/8\"" 10M BX159\n螺栓：1-7/8\""×20"" },
    { ""type"": ""drilling_spool"", ""label"": ""346.1mm（13-5/8\""）钻井四通\n68.9MPa（10000 psi）"", ""flange"": ""13-5/8\"" 10M BX159\n螺栓：1-7/8\""×20"" },
    { ""type"": ""casing_head"", ""label"": ""346.1mm（13-5/8\""）套管头（3000 psi）"", ""flange"": ""13-5/8\"" 5M BX160\n螺栓：1-5/8\""×16\n21-1/4\"" 2M R73\n螺栓：1-5/8\""×24"" },
    { ""type"": ""wellhead_platform"", ""label"": ""井口平台"" },
    { ""type"": ""single_casing"", ""label"": ""13-3/8\""套管"" }
  ]
}

## EXAMPLE 2: 3rd Spud (三开 8.5"") with casing spool
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
{
  ""name"": ""92 Series - 3rd Spud 8.5"",
  ""devices"": [
    { ""type"": ""rotary_table"", ""label"": ""转盘面"" },
    { ""type"": ""annular_bop"", ""label"": ""346.1mm（13-5/8\""）万能防喷器\n34.5MPa（5000 psi）"", ""flange"": ""13-5/8\"" 5M BX160\n螺栓：1-5/8\""×16\n13-5/8\"" 10M BX159\n螺栓：1-7/8\""×20"" },
    { ""type"": ""adapter_flange"", ""label"": ""346.1mm（13-5/8\""）变压法兰"", ""flange"": ""13-5/8\"" 10M BX159\n螺栓：1-7/8\""×20"" },
    { ""type"": ""ram_bop"", ""label"": ""346.1mm（13-5/8\""）闸板防喷器\n68.9MPa（10000 psi）"", ""flange"": ""13-5/8\"" 10M BX159\n螺栓：1-7/8\""×20"" },
    { ""type"": ""drilling_spool"", ""label"": ""346.1mm（13-5/8\""）钻井四通\n68.9MPa（10000 psi）"", ""flange"": ""13-5/8\"" 10M BX159\n螺栓：1-7/8\""×20\n11\"" 10M BX158\n螺栓：1-3/4\""×16"" },
    { ""type"": ""adapter_flange"", ""label"": ""346.1mm（13-5/8\""）变压法兰"", ""flange"": ""11\"" 10M BX158\n螺栓：1-3/4\""×16\n13-5/8\"" 5M BX160\n螺栓：1-5/8\""×16"" },
    { ""type"": ""casing_spool"", ""label"": ""套管四通"", ""flange"": ""11\"" 3M BX158\n螺栓：1-3/4\""×16\n13-5/8\"" 3M BX160"" },
    { ""type"": ""casing_head"", ""label"": ""346.1mm（13-5/8\""）套管头（3000 psi）"", ""flange"": ""13-5/8\"" 5M BX160\n螺栓：1-5/8\""×16\n21-1/4\"" 2M R73\n螺栓：1-5/8\""×24"" },
    { ""type"": ""wellhead_platform"", ""label"": ""井口平台"" },
    { ""type"": ""single_casing"", ""label"": ""13-3/8\""套管"" },
    { ""type"": ""single_casing"", ""label"": ""9-5/8\""套管"" }
  ]
}

## EXAMPLE 3: 18-3/4"" Large Bore (15000 psi) 2nd Spud
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
{
  ""name"": ""K6 Composite Casing - 2nd Spud"",
  ""devices"": [
    { ""type"": ""rotary_table"", ""label"": ""转盘面"" },
    { ""type"": ""annular_bop"", ""label"": ""476.25mm（18-3/4\""）万能防喷器\n103.4MPa（15000 psi）"", ""flange"": ""18-3/4\"" 10M BX164\n螺栓：2-1/4\""×24"" },
    { ""type"": ""ram_bop"", ""label"": ""476.25mm（18-3/4\""）闸板防喷器\n103.4MPa（15000 psi）"", ""flange"": ""18-3/4\"" 15M BX164\n螺栓：3\""×20"" },
    { ""type"": ""ram_bop"", ""label"": ""476.25mm（18-3/4\""）闸板防喷器\n103.4MPa（15000 psi）"", ""flange"": ""18-3/4\"" 15M BX164\n螺栓：3\""×20"" },
    { ""type"": ""adapter_flange"", ""label"": ""476.25mm（18-3/4\""）变压法兰"", ""flange"": ""18-3/4\"" 15M BX164\n螺栓：3\""×20\n13-5/8\"" 10M BX159\n螺栓：1-7/8\""×20\n13-5/8\"" 5M BX160\n螺栓：1-5/8\""×16"" },
    { ""type"": ""casing_head"", ""label"": ""套管头"", ""flange"": ""13-5/8\"" 5M BX160\n螺栓：1-5/8\""×16\n21-1/4\"" 5M R73\n螺栓：1-5/8\""×24"" },
    { ""type"": ""wellhead_platform"", ""label"": ""井口平台"" },
    { ""type"": ""double_casing"", ""label"": ""20\""+13-3/8\""套管"" }
  ]
}";

            // 動態追加自定義裝置信息
            if (_customDevices != null && _customDevices.Count > 0)
            {
                var sb = new System.Text.StringBuilder(baseSchema);
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("## CUSTOM DEVICE TYPES (user-defined, " + _customDevices.Count + " total)");
                sb.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                sb.AppendLine("In addition to the 23 built-in types above, the following custom device images are available.");
                sb.AppendLine("Use the filename (WITHOUT extension) as the \"type\" value.");
                sb.AppendLine("Custom devices are rendered as bitmap images instead of SVG.");
                sb.AppendLine();
                sb.AppendLine("  Type Identifier (use this)    Source File");
                sb.AppendLine("  ─────────────────────────────  ──────────────────────────");
                foreach (var key in _customDevices.Keys)
                {
                    string nameNoExt = System.IO.Path.GetFileNameWithoutExtension(key);
                    sb.AppendLine("  " + nameNoExt.PadRight(30) + "  " + key);
                }
                sb.AppendLine();
                sb.AppendLine("Custom devices support the same \"label\" and \"flange\" fields as built-in devices.");
                sb.AppendLine();
                string firstKey = null;
                foreach (var key in _customDevices.Keys) { firstKey = key; break; }
                if (firstKey != null)
                {
                    string example = System.IO.Path.GetFileNameWithoutExtension(firstKey);
                    sb.AppendLine("Example using custom device:");
                    sb.AppendLine("  { \"type\": \"" + example + "\", \"label\": \"Custom Device\", \"flange\": \"Spec Info\" }");
                }
                return sb.ToString();
            }

            return baseSchema;
        }

        private string BuildJapaneseSchemaText()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== 井口装置スタック作図 - JSON規則 ===");
            sb.AppendLine();
            sb.AppendLine("重要:");
            sb.AppendLine("1) 純粋なJSONのみを出力（Markdown/```json/説明文は不可）");
            sb.AppendLine("2) 各オブジェクトに devices 配列が必要");
            sb.AppendLine("3) 各 devices 項目に type が必要");
            sb.AppendLine("4) type は下記 Type Identifier を使用");
            sb.AppendLine("5) label/flange の改行は \\n を使用");
            sb.AppendLine();
            sb.AppendLine("単一図面:");
            sb.AppendLine("{\"name\":\"...\",\"devices\":[...]}");
            sb.AppendLine();
            sb.AppendLine("複数図面（配列で包まない）:");
            sb.AppendLine("{\"name\":\"A\",\"devices\":[...]}");
            sb.AppendLine("{\"name\":\"B\",\"devices\":[...]}");
            sb.AppendLine();
            sb.AppendLine("利用可能 Type（23）:");
            AppendTypeTable(sb);
            sb.AppendLine();
            sb.AppendLine("最小例:");
            sb.AppendLine("{\"devices\":[{\"type\":\"rotary_table\"},{\"type\":\"annular_bop\"},{\"type\":\"casing_head\"}]}");
            AppendCustomDeviceSchemaSection(sb, "カスタム装置タイプ", "ファイル名（拡張子なし）を type として使用します。");
            return sb.ToString();
        }

        private string BuildKoreanSchemaText()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== 웰헤드 장치 스택 도면 - JSON 규격 ===");
            sb.AppendLine();
            sb.AppendLine("중요:");
            sb.AppendLine("1) 순수 JSON만 출력(Markdown/```json/설명문 금지)");
            sb.AppendLine("2) 각 객체에 devices 배열 필요");
            sb.AppendLine("3) 각 devices 항목에 type 필수");
            sb.AppendLine("4) type 은 아래 Type Identifier 사용");
            sb.AppendLine("5) label/flange 줄바꿈은 \\n 사용");
            sb.AppendLine();
            sb.AppendLine("단일 도면:");
            sb.AppendLine("{\"name\":\"...\",\"devices\":[...]}");
            sb.AppendLine();
            sb.AppendLine("다중 도면(배열로 감싸지 않음):");
            sb.AppendLine("{\"name\":\"A\",\"devices\":[...]}");
            sb.AppendLine("{\"name\":\"B\",\"devices\":[...]}");
            sb.AppendLine();
            sb.AppendLine("사용 가능한 Type (23):");
            AppendTypeTable(sb);
            sb.AppendLine();
            sb.AppendLine("최소 예시:");
            sb.AppendLine("{\"devices\":[{\"type\":\"rotary_table\"},{\"type\":\"annular_bop\"},{\"type\":\"casing_head\"}]}");
            AppendCustomDeviceSchemaSection(sb, "사용자 장치 타입", "파일명(확장자 제외)을 type 값으로 사용하세요.");
            return sb.ToString();
        }

        private void AppendTypeTable(System.Text.StringBuilder sb)
        {
            string[] typeIds =
            {
                "rotary_table","bell_nipple","packing_gland","mpd_rotating_head","temporary_wellhead",
                "annular_bop","ram_bop","double_ram_bop","drilling_spool","tubing_spool","casing_spool",
                "reducer_flange","adapter_flange","riser","casing_head","wellhead_platform","diverter",
                "single_casing","double_casing","triple_casing","single_bore_double_well","marine_conductor",
                "choke_kill_manifold"
            };

            for (int i = 0; i < typeIds.Length; i++)
            {
                sb.AppendLine($"- {typeIds[i],-26}  {LocalizationManager.GetDeviceName(i)}");
            }
        }

        private void AppendCustomDeviceSchemaSection(System.Text.StringBuilder sb, string title, string guide)
        {
            if (_customDevices == null || _customDevices.Count == 0)
            {
                return;
            }

            sb.AppendLine();
            sb.AppendLine($"{title} ({_customDevices.Count}):");
            sb.AppendLine(guide);
            foreach (var key in _customDevices.Keys)
            {
                string nameNoExt = System.IO.Path.GetFileNameWithoutExtension(key);
                sb.AppendLine($"- {nameNoExt}  ({key})");
            }
        }
    }

    /// <summary>
    /// 啟用雙緩衝的 Panel，用於滾動文字無閃爍
    /// </summary>
    internal sealed class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.UserPaint, true);
        }
    }
}
