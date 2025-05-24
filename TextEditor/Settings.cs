using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace TextEditor
{
    public static class Settings
    {
        private static readonly string SettingsPath = Path.Combine("data", "settings.json");

        private class SettingsModel
        {
            public string? FontName { get; set; }
            public float FontSize { get; set; }
            public Color TextColor { get; set; }
            public HorizontalAlignment Alignment { get; set; } = HorizontalAlignment.Left;
        }

        public static void SetFont(RichTextBox textBox)
        {
            using var fontDialog = new FontDialog
            {
                Font = textBox.SelectionFont ?? textBox.Font
            };

            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                textBox.Font = fontDialog.Font;
                SaveSettings(textBox);
            }
        }

        public static void SetColor(RichTextBox textBox)
        {
            using var colorDialog = new ColorDialog
            {
                Color = textBox.ForeColor
            };

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                textBox.ForeColor = colorDialog.Color;
                SaveSettings(textBox);
            }
        }

        public static void SetAlignment(RichTextBox textBox, HorizontalAlignment alignment)
        {
            textBox.SelectAll();
            textBox.SelectionAlignment = alignment;
            textBox.DeselectAll();
            SaveSettings(textBox);
        }

        public static void LoadSettings(RichTextBox textBox)
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    var json = File.ReadAllText(SettingsPath);
                    var settings = JsonSerializer.Deserialize<SettingsModel>(json);
                    
                    if (settings?.FontName != null)
                    {
                        textBox.Font = new Font(settings.FontName, settings.FontSize);
                    }
                    
                    textBox.ForeColor = settings?.TextColor ?? textBox.ForeColor;
                    
                    textBox.SelectAll();
                    textBox.SelectionAlignment = settings?.Alignment ?? HorizontalAlignment.Left;
                    textBox.DeselectAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载设置时出错: {ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void SaveSettings(RichTextBox textBox)
        {
            try
            {
                var settings = new SettingsModel
                {
                    FontName = textBox.Font.Name,
                    FontSize = textBox.Font.Size,
                    TextColor = textBox.ForeColor,
                    Alignment = textBox.SelectionAlignment
                };

                Directory.CreateDirectory("data");
                var json = JsonSerializer.Serialize(settings);
                File.WriteAllText(SettingsPath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存设置时出错: {ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
