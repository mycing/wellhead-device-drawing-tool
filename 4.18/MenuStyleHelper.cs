using System.Drawing;
using System.Windows.Forms;

internal static class MenuStyleHelper
{
    public static void Apply(ContextMenuStrip menu)
    {
        if (menu == null)
        {
            return;
        }

        menu.BackColor = Color.FromArgb(230, 232, 235);
        menu.ForeColor = Color.FromArgb(35, 35, 35);
        menu.ShowImageMargin = false;
        menu.Renderer = new ToolStripProfessionalRenderer(new IndustrialColorTable());
    }

    private sealed class IndustrialColorTable : ProfessionalColorTable
    {
        public override Color ToolStripDropDownBackground => Color.FromArgb(230, 232, 235);
        public override Color ImageMarginGradientBegin => Color.FromArgb(230, 232, 235);
        public override Color ImageMarginGradientMiddle => Color.FromArgb(230, 232, 235);
        public override Color ImageMarginGradientEnd => Color.FromArgb(230, 232, 235);
        public override Color MenuBorder => Color.FromArgb(200, 204, 210);
        public override Color MenuItemBorder => Color.FromArgb(190, 195, 202);
        public override Color MenuItemSelected => Color.FromArgb(210, 215, 220);
        public override Color SeparatorDark => Color.FromArgb(200, 204, 210);
        public override Color SeparatorLight => Color.FromArgb(230, 232, 235);
    }
}
