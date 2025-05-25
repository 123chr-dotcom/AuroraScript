using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace TextEditor
{
    public static class TextEditing
    {
        public static void CopyText(RichTextBox textBox)
        {
            if (textBox == null) return;
            try
            {
                if (textBox.SelectionLength > 0)
                    Clipboard.SetText(textBox.SelectedText);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"复制文本时出错: {ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void PasteText(RichTextBox textBox)
        {
            if (textBox == null) return;
            try
            {
                if (Clipboard.ContainsText())
                    textBox.Paste();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"粘贴文本时出错: {ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void CutText(RichTextBox textBox)
        {
            if (textBox == null) return;
            try
            {
                if (textBox.SelectionLength > 0)
                {
                    Clipboard.SetText(textBox.SelectedText);
                    textBox.SelectedText = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"剪切文本时出错: {ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void SelectAllText(RichTextBox textBox)
        {
            if (textBox == null) return;
            textBox.SelectAll();
        }
    }
}
