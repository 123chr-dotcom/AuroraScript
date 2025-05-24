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
        private static readonly object _lock = new object();
        
        // 默认值定义
        private const string DefaultFontName = "Microsoft Sans Serif";
        private const float DefaultFontSize = 9f;
        private static readonly Color DefaultTextColor = Color.Black;
        private const HorizontalAlignment DefaultAlignment = HorizontalAlignment.Left;

        private class FontInfo
        {
            public string Name { get; set; } = DefaultFontName;
            public float Size { get; set; } = DefaultFontSize;
            public FontStyle Style { get; set; } = FontStyle.Regular;
        }

        private class SettingsModel
        {
            public FontInfo Font { get; set; }
            public int TextColorArgb { get; set; } = DefaultTextColor.ToArgb();
            public HorizontalAlignment Alignment { get; set; } = DefaultAlignment;

            public SettingsModel()
            {
                Font = new FontInfo
                {
                    Name = DefaultFontName,
                    Size = DefaultFontSize,
                    Style = FontStyle.Regular
                };
            }
        }

        private static Font GetSafeFont(string fontName, float fontSize, FontStyle style)
        {
            try
            {
                using (var testFont = new Font(fontName, fontSize, style))
                {
                    return new Font(testFont.Name, fontSize, style);
                }
            }
            catch
            {
                return new Font(DefaultFontName, DefaultFontSize, FontStyle.Regular);
            }
        }

        private static JsonSerializerOptions GetJsonOptions()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return options;
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
            lock (_lock)
            {
                try
                {
                    if (File.Exists(SettingsPath))
                    {
                        var json = File.ReadAllText(SettingsPath);
                        Console.WriteLine($"加载设置: {json}");
                        var settings = JsonSerializer.Deserialize<SettingsModel>(json) ?? new SettingsModel();
                        
                        // 应用字体设置
                        var font = GetSafeFont(
                            settings.Font.Name, 
                            settings.Font.Size, 
                            settings.Font.Style);
                        textBox.Font = font;
                        Console.WriteLine($"应用字体: {font.Name} {font.Size} {font.Style}");

                        // 应用其他设置
                        textBox.ForeColor = Color.FromArgb(settings.TextColorArgb);
                        textBox.SelectAll();
                        textBox.SelectionAlignment = settings.Alignment;
                        textBox.DeselectAll();
                    }
                    else
                    {
                        ApplyDefaultSettings(textBox);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"加载设置出错: {ex}");
                    ApplyDefaultSettings(textBox);
                    MessageBox.Show($"加载设置时出错，已恢复默认设置", "错误", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static void ApplyDefaultSettings(RichTextBox textBox)
        {
            textBox.Font = new Font(DefaultFontName, DefaultFontSize);
            textBox.ForeColor = DefaultTextColor;
            textBox.SelectAll();
            textBox.SelectionAlignment = DefaultAlignment;
            textBox.DeselectAll();
            Console.WriteLine("应用默认设置");
        }

        private static void SaveSettings(RichTextBox textBox)
        {
            lock (_lock)
            {
                try
                {
                    var currentFont = textBox.SelectionFont ?? textBox.Font;
                    var settings = new SettingsModel
                    {
                        Font = new FontInfo
                        {
                            Name = currentFont.Name,
                            Size = currentFont.Size,
                            Style = currentFont.Style
                        },
                        TextColorArgb = textBox.ForeColor.ToArgb(),
                        Alignment = textBox.SelectionAlignment
                    };

                    Directory.CreateDirectory("data");
                    var json = JsonSerializer.Serialize(settings, GetJsonOptions());
                    File.WriteAllText(SettingsPath, json);
                    Console.WriteLine($"设置已保存: {json}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"保存设置出错: {ex}");
                    MessageBox.Show($"保存设置时出错", "错误", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
