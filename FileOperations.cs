using System;
using System.IO;
using System.Windows.Forms;

namespace TextEditor
{
    public static class FileOperations
    {
        private static string _currentFilePath = string.Empty;

        public static void OpenFile(RichTextBox textBox)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "文本文件 (*.txt)|*.txt|富文本文件 (*.rtf)|*.rtf|所有文件 (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // 保存当前文本
                        var currentText = textBox.Text;
                        var currentRtf = textBox.Rtf;
                        
                        _currentFilePath = openFileDialog.FileName;
                        
                        if (Path.GetExtension(_currentFilePath).ToLower() == ".rtf")
                            textBox.LoadFile(_currentFilePath);
                        else
                            textBox.Text = File.ReadAllText(_currentFilePath);
                        
                        // 应用当前设置
                        Settings.LoadSettings(textBox);
                        
                        // 恢复RTF格式内容
                        if (Path.GetExtension(_currentFilePath).ToLower() == ".rtf")
                        {
                            textBox.Rtf = currentRtf;
                        }
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
            if (string.IsNullOrEmpty(_currentFilePath) || _currentFilePath == string.Empty)
            {
                SaveFileAs(textBox);
                return;
            }

            try
            {
                if (Path.GetExtension(_currentFilePath).ToLower() == ".rtf")
                    textBox.SaveFile(_currentFilePath);
                else
                    File.WriteAllText(_currentFilePath, textBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存文件时出错: {ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void SaveFileAs(RichTextBox textBox)
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

        public static bool TryOpenFile(RichTextBox textBox)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "文本文件 (*.txt)|*.txt|富文本文件 (*.rtf)|*.rtf|所有文件 (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _currentFilePath = openFileDialog.FileName;
                        
                        if (Path.GetExtension(_currentFilePath).ToLower() == ".rtf")
                            textBox.LoadFile(_currentFilePath);
                        else
                            textBox.Text = File.ReadAllText(_currentFilePath);
                        
                        Settings.LoadSettings(textBox);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public static bool TrySaveFile(RichTextBox textBox)
        {
            if (string.IsNullOrEmpty(_currentFilePath) || _currentFilePath == string.Empty)
                return TrySaveFileAs(textBox);

            try
            {
                if (Path.GetExtension(_currentFilePath).ToLower() == ".rtf")
                    textBox.SaveFile(_currentFilePath);
                else
                    File.WriteAllText(_currentFilePath, textBox.Text);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TrySaveFileAs(RichTextBox textBox)
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "文本文件 (*.txt)|*.txt|富文本文件 (*.rtf)|*.rtf|所有文件 (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _currentFilePath = saveFileDialog.FileName;
                        if (Path.GetExtension(_currentFilePath).ToLower() == ".rtf")
                            textBox.SaveFile(_currentFilePath);
                        else
                            File.WriteAllText(_currentFilePath, textBox.Text);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            return false;
        }
    }
}
