using System;
using System.Windows.Forms;

namespace _4._18
{
    internal static class CanvasContextMenuFactory
    {
        public static ContextMenuStrip Create(Control owner, Action deleteAction, bool includeImportJson)
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();

            ToolStripMenuItem deleteItem = new ToolStripMenuItem(LocalizationManager.GetString("Delete"), null, (s, e) =>
            {
                deleteAction?.Invoke();
            });
            contextMenu.Items.Add(deleteItem);

            Form1 formRef = owner.FindForm() as Form1;
            ToolStripMenuItem autoAlignItem = new ToolStripMenuItem(
                formRef?.GetAutoAlignMenuText() ?? LocalizationManager.GetString("AutoAlign"), null, (s, e) =>
            {
                (owner.FindForm() as Form1)?.AutoAlignControls();
            });
            contextMenu.Items.Add(autoAlignItem);

            ToolStripMenuItem autoCaptureItem = new ToolStripMenuItem(LocalizationManager.GetString("AutoCapture"), null, (s, e) =>
            {
                (owner.FindForm() as Form1)?.StartAutoCapture();
            });
            contextMenu.Items.Add(autoCaptureItem);
            contextMenu.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem captureItem = new ToolStripMenuItem(LocalizationManager.GetString("Capture"), null, (s, e) =>
            {
                (owner.FindForm() as Form1)?.StartCapture();
            });
            contextMenu.Items.Add(captureItem);

            ToolStripMenuItem clearItem = new ToolStripMenuItem(LocalizationManager.GetString("ClearCanvas"), null, (s, e) =>
            {
                owner.Parent?.Controls.Clear();
                owner.Parent?.Refresh();
            });
            contextMenu.Items.Add(clearItem);

            ToolStripMenuItem openSampleItem = new ToolStripMenuItem(LocalizationManager.GetString("OpenSample"), null, (s, e) =>
            {
                (owner.FindForm() as Form1)?.OpenDeviceSample();
            });
            contextMenu.Items.Add(openSampleItem);

            ToolStripMenuItem saveSampleItem = new ToolStripMenuItem(LocalizationManager.GetString("AddSampleToLibrary"), null, (s, e) =>
            {
                (owner.FindForm() as Form1)?.SaveSampleToLibrary();
            });
            contextMenu.Items.Add(saveSampleItem);

            if (includeImportJson)
            {
                contextMenu.Items.Add(new ToolStripSeparator());
                ToolStripMenuItem importJsonItem = new ToolStripMenuItem(LocalizationManager.GetString("ImportJson"), null, (s, e) =>
                {
                    (owner.FindForm() as Form1)?.ImportJsonToCanvas();
                });
                contextMenu.Items.Add(importJsonItem);
            }

            MenuStyleHelper.Apply(contextMenu);
            return contextMenu;
        }
    }
}
