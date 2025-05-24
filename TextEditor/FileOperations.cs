using System;
using System.IO;
using System.Windows.Forms;

namespace TextEditor
{
    public static class FileOperations
    {
        public static void OpenFile(RichTextBox textBox)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "文本文件 (*.txt)|*.txt|富文本文件 (*.rtf)|*.rtf|所有文件 (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (Path.GetExtension(openFileDialog.FileName).ToLower() == ".rtf")
                            textBox.LoadFile(openFileDialog.FileName);
                        else
                            textBox.Text = File.ReadAllText(openFileDialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"打开文件时出错: {ex.Message}", "错误", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public static void SaveFile(RichTextBox textBox)
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "文本文件 (*.txt)|*.txt|富文本文件 (*.rtf)|*.rtf|所有文件 (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (Path.GetExtension(saveFileDialog.FileName).ToLower() == ".rtf")
                            textBox.SaveFile(saveFileDialog.FileName);
                        else
                            File.WriteAllText(saveFileDialog.FileName, textBox.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"保存文件时出错: {ex.Message}", "错误", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
