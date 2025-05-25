using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TextEditor
{
    public class MainForm : Form
    {
        private MenuStrip menuStrip = new MenuStrip();
        private TabControl tabControl = new TabControl();
        private StatusStrip statusStrip = new StatusStrip();
        private WebBrowser welcomeBrowser = new WebBrowser();
        private int tabCount = 0;

        public MainForm()
        {
            InitializeComponents();
        }

        private void CreateNewTab(bool showWelcome = false)
        {
            var tabPage = new TabPage($"文档 {++tabCount}");
            var textBox = new RichTextBox { Dock = DockStyle.Fill };
            
            // 添加上下文菜单
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("复制", null, (s,e) => TextEditing.CopyText(textBox)),
                new ToolStripMenuItem("粘贴", null, (s,e) => TextEditing.PasteText(textBox)),
                new ToolStripMenuItem("剪切", null, (s,e) => TextEditing.CutText(textBox))
            });
            textBox.ContextMenuStrip = contextMenu;

            tabPage.Controls.Add(textBox);
            tabControl!.TabPages.Add(tabPage);
            tabControl.SelectedTab = tabPage;
            
            // 显示逻辑
            if (showWelcome && welcomeBrowser != null)
            {
                welcomeBrowser.Visible = true;
                tabControl.Visible = false;
            }
            else
            {
                welcomeBrowser?.Hide();
                tabControl.Visible = true;
            }
        }

        private void CloseCurrentTab()
        {
            if (tabControl != null && tabControl.TabCount > 0 && tabControl.SelectedTab != null)
            {
                tabControl.TabPages.Remove(tabControl.SelectedTab);
                tabCount--;
                
                if (tabControl.TabCount == 0)
                {
                    CreateNewTab(true); // 最后一个标签页关闭时创建新标签页并显示欢迎界面
                }
            }
        }

        private RichTextBox GetCurrentTextBox()
        {
            if (tabControl?.SelectedTab?.Controls.Count > 0)
            {
                return tabControl.SelectedTab.Controls[0] as RichTextBox;
            }
            return null;
        }

        private void InitializeComponents()
        {
            this.Text = "AuroraScript";
            this.Width = 800;
            this.Height = 600;

            // 初始化TabControl
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Visible = false;
            this.Controls.Add(tabControl);

            InitializeMenuStrip();
            InitializeWelcomeBrowser();
            InitializeStatusStrip();
            this.MainMenuStrip = menuStrip;

            // 初始标签页显示欢迎界面
            CreateNewTab(true);
        }

        private void OpenFileInCurrentTab()
        {
            var textBox = GetCurrentTextBox();
            if (textBox != null && welcomeBrowser != null)
            {
                bool fileOpened = FileOperations.TryOpenFile(textBox);
                if (fileOpened)
                {
                    welcomeBrowser.Visible = false;
                    tabControl!.Visible = true;
                }
            }
        }

        private void InitializeWelcomeBrowser()
        {
            welcomeBrowser = new WebBrowser();
            welcomeBrowser.Dock = DockStyle.Fill;
            welcomeBrowser.AllowNavigation = false;
            welcomeBrowser.WebBrowserShortcutsEnabled = false;
            welcomeBrowser.ScriptErrorsSuppressed = true;
            welcomeBrowser.IsWebBrowserContextMenuEnabled = false;

            // 设置欢迎界面内容
            try
            {
                string htmlPath = Path.Combine(Application.StartupPath, "welcome.html");
                if (File.Exists(htmlPath))
                {
                    welcomeBrowser.Navigate(htmlPath);
                }
                else
                {
                    welcomeBrowser.DocumentText = @"<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body { 
            font-family: 'Microsoft YaHei', sans-serif;
            text-align: center; 
            padding: 40px;
            background-color: #f5f5f5;
        }
        h1 { 
            color: #2c3e50; 
            font-size: 24px;
            margin-bottom: 30px;
        }
    </style>
</head>
<body>
    <h1>🌟 欢迎使用 AuroraScript 文本编辑器 🌟</h1>
    <p>请按任意键开始编辑</p>
</body>
</html>";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"初始化欢迎界面时出错: {ex}");
                welcomeBrowser.DocumentText = @"<h1 style='text-align:center;font-family:Microsoft YaHei;color:red'>
                    欢迎界面加载失败，请按任意键继续
                </h1>";
            }

            welcomeBrowser.Visible = true;
            welcomeBrowser.BringToFront();
            this.Controls.Add(welcomeBrowser);

            // 首次编辑时切换到文本编辑模式
            welcomeBrowser.PreviewKeyDown += (s, e) => {
                if (e.KeyCode != Keys.Tab && e.KeyCode != Keys.Escape)
                {
                    SwitchToEditMode();
                }
            };
        }

        private void InitializeMenuStrip()
        {
            menuStrip = new MenuStrip();

            // 文件菜单
            var fileMenu = new ToolStripMenuItem("文件(&F)");
            var newTabItem = new ToolStripMenuItem("新建标签页(&N)");
            var openItem = new ToolStripMenuItem("打开(&O)");
            var saveItem = new ToolStripMenuItem("保存(&S)");
            var saveAsItem = new ToolStripMenuItem("另存为(&A)");
            var closeTabItem = new ToolStripMenuItem("关闭标签页(&W)");
            var exitItem = new ToolStripMenuItem("退出(&X)");
            
            newTabItem.ShortcutKeys = Keys.Control | Keys.T;
            openItem.ShortcutKeys = Keys.Control | Keys.O;
            saveItem.ShortcutKeys = Keys.Control | Keys.S;
            saveAsItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
            closeTabItem.ShortcutKeys = Keys.Control | Keys.W;
            
            newTabItem.Click += (s, e) => CreateNewTab();
            openItem.Click += (s, e) => OpenFileInCurrentTab();
            closeTabItem.Click += (s, e) => CloseCurrentTab();
            exitItem.Click += (s, e) => Application.Exit();
            
            fileMenu.DropDownItems.AddRange(new ToolStripItem[] { 
                newTabItem, openItem, saveItem, saveAsItem, 
                new ToolStripSeparator(), closeTabItem, exitItem 
            });

            // 编辑菜单
            var editMenu = new ToolStripMenuItem("编辑(&E)");
            var copyItem = new ToolStripMenuItem("复制(&C)");
            var pasteItem = new ToolStripMenuItem("粘贴(&V)");
            var cutItem = new ToolStripMenuItem("剪切(&X)");
            var selectAllItem = new ToolStripMenuItem("全选(&A)");
            
            copyItem.ShortcutKeys = Keys.Control | Keys.C;
            pasteItem.ShortcutKeys = Keys.Control | Keys.V;
            cutItem.ShortcutKeys = Keys.Control | Keys.X;
            selectAllItem.ShortcutKeys = Keys.Control | Keys.A;
            
            copyItem.Click += (s, e) => TextEditing.CopyText(GetCurrentTextBox());
            pasteItem.Click += (s, e) => TextEditing.PasteText(GetCurrentTextBox());
            cutItem.Click += (s, e) => TextEditing.CutText(GetCurrentTextBox());
            selectAllItem.Click += (s, e) => TextEditing.SelectAllText(GetCurrentTextBox());
            
            editMenu.DropDownItems.AddRange(new ToolStripItem[] { copyItem, pasteItem, cutItem, selectAllItem });

            // 格式菜单
            var formatMenu = new ToolStripMenuItem("格式(&O)");
            var fontItem = new ToolStripMenuItem("字体(&F)");
            var colorItem = new ToolStripMenuItem("颜色(&C)");
            
            fontItem.Click += (s, e) => Settings.SetFont(GetCurrentTextBox());
            colorItem.Click += (s, e) => Settings.SetColor(GetCurrentTextBox());
            
            formatMenu.DropDownItems.AddRange(new ToolStripItem[] { fontItem, colorItem });

            menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, editMenu, formatMenu });
            this.Controls.Add(menuStrip);
        }

        private void InitializeStatusStrip()
        {
            statusStrip = new StatusStrip();
            var statusLabel = new ToolStripStatusLabel("就绪");
            statusStrip.Items.Add(statusLabel);
            statusStrip.Dock = DockStyle.Bottom;
            this.Controls.Add(statusStrip);
        }

        private void SwitchToEditMode()
        {
            if (welcomeBrowser != null && tabControl != null)
            {
                welcomeBrowser.Visible = false;
                tabControl.Visible = true;
                GetCurrentTextBox()?.Focus();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var textBox = GetCurrentTextBox();
            if (textBox != null)
            {
                Settings.LoadSettings(textBox);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var textBox = GetCurrentTextBox();
            if (keyData == (Keys.Control | Keys.O) && textBox != null && welcomeBrowser != null)
            {
                OpenFileInCurrentTab();
                return true;
            }
            else if (keyData == (Keys.Control | Keys.S) && textBox != null)
            {
                FileOperations.TrySaveFile(textBox);
                return true;
            }
            else if (keyData == (Keys.Control | Keys.Shift | Keys.S) && textBox != null)
            {
                FileOperations.TrySaveFileAs(textBox);
                return true;
            }
            else if (keyData == (Keys.Control | Keys.T))
            {
                CreateNewTab();
                return true;
            }
            else if (keyData == (Keys.Control | Keys.W))
            {
                CloseCurrentTab();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
