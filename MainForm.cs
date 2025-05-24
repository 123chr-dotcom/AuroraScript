using System;
using System.Windows.Forms;

namespace TextEditor
{
    public class MainForm : Form
    {
        private MenuStrip menuStrip = null!;
        private RichTextBox textBox = null!;
        private StatusStrip statusStrip = null!;

        public MainForm()
        {
            InitializeComponents();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Settings.LoadSettings(textBox);
        }

        private void InitializeComponents()
        {
            // 主窗体设置
            this.Text = "AuroraScript";
            this.Width = 800;
            this.Height = 600;

            // 菜单栏
            menuStrip = new MenuStrip();
            var fileMenu = new ToolStripMenuItem("文件(&F)");
            var openItem = new ToolStripMenuItem("打开(&O)");
            var saveItem = new ToolStripMenuItem("保存(&S)");
            var exitItem = new ToolStripMenuItem("退出(&X)");
            
            openItem.Click += (s, e) => FileOperations.OpenFile(textBox);
            saveItem.Click += (s, e) => FileOperations.SaveFile(textBox);
            exitItem.Click += (s, e) => Application.Exit();
            
            fileMenu.DropDownItems.AddRange(new ToolStripItem[] { openItem, saveItem, exitItem });
            var editMenu = new ToolStripMenuItem("编辑(&E)");
            var copyItem = new ToolStripMenuItem("复制(&C)");
            var pasteItem = new ToolStripMenuItem("粘贴(&V)");
            var cutItem = new ToolStripMenuItem("剪切(&X)");
            var selectAllItem = new ToolStripMenuItem("全选(&A)");
            
            copyItem.ShortcutKeys = Keys.Control | Keys.C;
            pasteItem.ShortcutKeys = Keys.Control | Keys.V;
            cutItem.ShortcutKeys = Keys.Control | Keys.X;
            selectAllItem.ShortcutKeys = Keys.Control | Keys.A;
            
            copyItem.Click += (s, e) => TextEditing.CopyText(textBox);
            pasteItem.Click += (s, e) => TextEditing.PasteText(textBox);
            cutItem.Click += (s, e) => TextEditing.CutText(textBox);
            selectAllItem.Click += (s, e) => TextEditing.SelectAllText(textBox);
            
            editMenu.DropDownItems.AddRange(new ToolStripItem[] { 
                copyItem, pasteItem, cutItem, selectAllItem 
            });
            var formatMenu = new ToolStripMenuItem("格式(&O)");
            var fontItem = new ToolStripMenuItem("字体(&F)");
            var colorItem = new ToolStripMenuItem("颜色(&C)");
            var alignMenu = new ToolStripMenuItem("对齐方式(&A)");
            var leftAlignItem = new ToolStripMenuItem("左对齐(&L)");
            var centerAlignItem = new ToolStripMenuItem("居中对齐(&M)");
            var rightAlignItem = new ToolStripMenuItem("右对齐(&R)");
            
            fontItem.Click += (s, e) => Settings.SetFont(textBox);
            colorItem.Click += (s, e) => Settings.SetColor(textBox);
            leftAlignItem.Click += (s, e) => Settings.SetAlignment(textBox, HorizontalAlignment.Left);
            centerAlignItem.Click += (s, e) => Settings.SetAlignment(textBox, HorizontalAlignment.Center);
            rightAlignItem.Click += (s, e) => Settings.SetAlignment(textBox, HorizontalAlignment.Right);
            
            alignMenu.DropDownItems.AddRange(new ToolStripItem[] {
                leftAlignItem, centerAlignItem, rightAlignItem
            });
            
            formatMenu.DropDownItems.AddRange(new ToolStripItem[] {
                fontItem, colorItem, alignMenu
            });
            
            menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, editMenu, formatMenu });
            this.Controls.Add(menuStrip);

            // 文本编辑区
            textBox = new RichTextBox();
            textBox.Dock = DockStyle.Fill;
            this.Controls.Add(textBox);

            // 状态栏
            statusStrip = new StatusStrip();
            var statusLabel = new ToolStripStatusLabel("就绪");
            statusStrip.Items.Add(statusLabel);
            statusStrip.Dock = DockStyle.Bottom;
            this.Controls.Add(statusStrip);

            // 设置控件层级
            this.MainMenuStrip = menuStrip;
        }
    }
}
