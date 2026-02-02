using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using System.Windows.Media;

namespace CgLogListener
{
    public partial class FormMain : Form
    {
        private Settings settings;
        private CgLogHandler watcher;
        private readonly MediaPlayer mp = new MediaPlayer();
        private readonly Dictionary<string, CheckBox> soundCheckBoxes = new Dictionary<string, CheckBox>();
        private readonly Dictionary<string, CheckBox> mailCheckBoxes = new Dictionary<string, CheckBox>();

        public FormMain()
        {
            InitializeComponent();

            // fix IME bug
            ImeMode = ImeMode.OnHalf;
            Icon = Resource.icon;
            notifyIcon.Icon = Resource.icon;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            settings = Settings.GetInstance();

            // 設定通知標題
            txtAppName.Text = settings.AppName;
            UpdateAppTitle();

            if (string.IsNullOrEmpty(settings.CgLogPath))
            {
                string cgLogPath = settings.CgLogPath;

                if (!SelectLogPath(out cgLogPath))
                {
                    this.Close();
                    return;
                }

                settings.SetCgLogPath(cgLogPath);
            }

            if (!Directory.Exists(settings.CgLogPath) || !CgLogHandler.ValidationPath(settings.CgLogPath))
            {
                settings.SetCgLogPath(string.Empty);
                MessageBox.Show(this, "設定檔路徑錯誤, 請重新啟動", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            BindWatcher();

            // set playsound vol
            cgLogListenerTrackBar.Value = settings.SoundVol;

            // set line notify
            checkBox1.Checked = settings.CustomNotify;

            // 設定標準關鍵字及其音效/郵件選項
            SetupStandardTips();

            // 設定自訂關鍵字
            SetupCustomTips();

            cgLogListenerTrackBar.ValueChanged += CgLogListenerTrackBar_ValueChanged;
            checkBox1.CheckedChanged += CheckBox1_CheckedChanged;
        }

        private void SetupStandardTips()
        {
            var standardCheckBoxes = new[]
            {
                cgLogListenerCheckBox1,
                cgLogListenerCheckBox2,
                cgLogListenerCheckBox3,
                cgLogListenerCheckBox4,
                cgLogListenerCheckBox5,
                cgLogListenerCheckBox6,
                cgLogListenerCheckBox7
            };

            // 固定位置讓 🔊/✉ checkbox 對齊
            const int soundCheckBoxX = 155;
            const int mailCheckBoxX = 210;

            foreach (var chk in standardCheckBoxes)
            {
                var nameInSetting = chk.NameInSetting;

                // 取得或建立設定
                if (!settings.StandardTips.TryGetValue(nameInSetting, out TipNotifyOptions options))
                {
                    options = new TipNotifyOptions();
                    settings.SetStandardTip(nameInSetting, options);
                }

                // 設定主 checkbox
                chk.Checked = options.Enabled;
                chk.CheckedChanged += (s, ev) =>
                {
                    var cb = (CgLogListenerCheckBox)s;
                    settings.SetStandardTipEnabled(cb.NameInSetting, cb.Checked);
                };

                // 動態建立音效 checkbox
                var soundChk = new CheckBox
                {
                    Text = "🔊",
                    AutoSize = true,
                    Location = new Point(soundCheckBoxX, chk.Top),
                    Checked = options.PlaySound,
                    Font = new Font("Segoe UI Emoji", 8)
                };
                soundChk.CheckedChanged += (s, ev) =>
                {
                    settings.SetStandardTipPlaySound(nameInSetting, ((CheckBox)s).Checked);
                };
                panel1.Controls.Add(soundChk);
                soundCheckBoxes[nameInSetting] = soundChk;

                // 動態建立郵件 checkbox
                var mailChk = new CheckBox
                {
                    Text = "✉",
                    AutoSize = true,
                    Location = new Point(mailCheckBoxX, chk.Top),
                    Checked = options.SendMail,
                    Font = new Font("Segoe UI Emoji", 8)
                };
                mailChk.CheckedChanged += (s, ev) =>
                {
                    var isChecked = ((CheckBox)s).Checked;
                    if (isChecked && !MailHelper.IsConfigured())
                    {
                        MailHelper.GenerateDefaultConfig();
                        MessageBox.Show(this, "請先設定 mail.ini 檔案中的 SMTP 資訊", "郵件設定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    settings.SetStandardTipSendMail(nameInSetting, isChecked);
                };
                panel1.Controls.Add(mailChk);
                mailCheckBoxes[nameInSetting] = mailChk;
            }
        }

        private void SetupCustomTips()
        {
            foreach (var kv in settings.CustomizeTips)
            {
                if (!string.IsNullOrEmpty(kv.Key))
                {
                    cgLogListenerListBox.Items.Add(kv.Key);
                }
            }
        }

        private void CgLogListenerTrackBar_ValueChanged(object sender, EventArgs e)
        {
            var bar = (CgLogListenerTrackBar)sender;
            settings.SetSoundVol(bar.Value);
        }

        private void BtnSelectLogPath_Click(object sender, EventArgs e)
        {
            if (SelectLogPath(out _))
            {
                watcher.Dispose();
                BindWatcher();
            }
        }

        bool SelectLogPath(out string path)
        {
            path = null;
            var dialog = new FolderBrowserDialog()
            {
                ShowNewFolderButton = false,
                Description = @"請選擇魔力寶貝的目錄 (e.g. D:\CrossGate\)"
            };

            while (true)
            {
                var result = dialog.ShowDialog(this);

                if (result == DialogResult.Cancel)
                {
                    return false;
                }

                if (result == DialogResult.OK)
                {
                    if (!CgLogHandler.ValidationPath(dialog.SelectedPath))
                    {
                        MessageBox.Show(this, "請選擇魔力寶貝的目錄", "錯誤的路徑", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }

                    path = dialog.SelectedPath;
                    return true;
                }
            }
        }

        void BindWatcher()
        {
            txtCgLogPath.Text = settings.CgLogPath;
            watcher = new CgLogHandler(settings.CgLogPath);
            watcher.OnNewLog += Watcher_OnNewLog;
        }

        private const string DefaultCookingPattern = @"恢復了\d+魔力";
        private const string DefaultCookingMessage = "時間到了，吃料理~";

        void Watcher_OnNewLog(string log)
        {
            // 吃料理監聽：使用自定義 Regex pattern 偵測
            var cookingPattern = string.IsNullOrWhiteSpace(txtCookingPattern.Text)
                ? DefaultCookingPattern
                : txtCookingPattern.Text;

            bool cookingMatch = false;
            try
            {
                cookingMatch = chkCookingReminder.Checked && Regex.IsMatch(log, cookingPattern);
            }
            catch { }

            if (cookingMatch)
            {
                Invoke((Action)delegate
                {
                    // 重置計時器
                    timerCooking.Stop();
                    if (int.TryParse(txtCookingInterval.Text, out int seconds) && seconds > 0)
                    {
                        timerCooking.Interval = seconds * 1000;
                        timerCooking.Start();
                    }
                });
            }

            foreach (var n in panel1.Controls.OfType<INotifyMessage>())
            {
                var result = n.Notify(log);
                if (result.IsMatch)
                {
                    notifyIcon.ShowBalloonTip(1, notifyIcon.BalloonTipTitle, log, ToolTipIcon.None);

                    // 根據該關鍵字的設定決定是否播放音效
                    const string soundName = "sound.wav";
                    if (result.PlaySound && File.Exists(soundName))
                    {
                        Invoke((Action)delegate
                        {
                            mp.Stop();
                            mp.Open(new Uri(new FileInfo(soundName).FullName));
                            mp.Volume = settings.SoundVol / 10d;
                            mp.Play();
                        });
                    }

                    // 根據該關鍵字的設定決定是否發送郵件
                    if (result.SendMail)
                    {
                        try
                        {
                            MailHelper.SendMail("魔力Log監視通知", log);
                        }
                        catch { }
                    }

                    // Custom Notifier (全域設定)
                    if (settings.CustomNotify)
                    {
                        foreach (var notifier in settings.CustomNotifier.Split(','))
                        {
                            try
                            {
                                ProcessStartInfo p = new ProcessStartInfo(notifier, $"\"[{settings.AppName}] {log}\"")
                                {
                                    WindowStyle = ProcessWindowStyle.Hidden,
                                    CreateNoWindow = true
                                };
                                Process.Start(p);
                            }
                            catch { }
                        }
                    }

                    break;
                }
            }
        }

        private void BtnAddCus_Click(object sender, EventArgs e)
        {
            if (FormPrompt.ShowDialog(this, out string value, out TipNotifyOptions options) != DialogResult.OK ||
                string.IsNullOrEmpty(value))
            {
                return;
            }

            settings.AddCustomizeTip(value, options);
            cgLogListenerListBox.Items.Add(value);
        }

        private void BtnDelCus_Click(object sender, EventArgs e)
        {
            if (cgLogListenerListBox.SelectedIndex < 0)
            {
                return;
            }

            var selectItem = (string)cgLogListenerListBox.SelectedItem;
            settings.RemoveCustomizeTip(selectItem);
            cgLogListenerListBox.Items.Remove(selectItem);
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                FormCustomNotifierPrompt.ShowDialog(this, out string value);
                settings.SetCustomNotify(true);
                settings.SetCustomNotifier(value);
            }
            else
            {
                settings.SetCustomNotify(false);
                settings.SetCustomNotifier(string.Empty);
            }
        }

        private void TxtAppName_Leave(object sender, EventArgs e)
        {
            // 不再自動儲存，由按鈕觸發
        }

        private void BtnSaveAppName_Click(object sender, EventArgs e)
        {
            var newAppName = txtAppName.Text.Trim();
            if (string.IsNullOrEmpty(newAppName))
            {
                newAppName = "CgLogListener";
                txtAppName.Text = newAppName;
            }
            settings.SetAppName(newAppName);
            UpdateAppTitle();
        }

        private void UpdateAppTitle()
        {
            var appTitle = $"[{settings.AppName}] 魔力Log監視";
            notifyIcon.BalloonTipTitle = appTitle;
            notifyIcon.Text = appTitle;
            this.Text = appTitle;
        }

        private void ChkCookingReminder_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCookingReminder.Checked)
            {
                if (!int.TryParse(txtCookingInterval.Text, out int seconds) || seconds <= 0)
                {
                    MessageBox.Show("請輸入有效的秒數", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkCookingReminder.Checked = false;
                    return;
                }

                // 驗證 Regex pattern 是否有效
                var pattern = string.IsNullOrWhiteSpace(txtCookingPattern.Text)
                    ? DefaultCookingPattern
                    : txtCookingPattern.Text;
                try
                {
                    Regex.IsMatch("test", pattern);
                }
                catch
                {
                    MessageBox.Show("Regex 格式錯誤", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkCookingReminder.Checked = false;
                    return;
                }
            }
            else
            {
                timerCooking.Stop();
            }
        }

        private void BtnCookingDefault_Click(object sender, EventArgs e)
        {
            txtCookingPattern.Text = DefaultCookingPattern;
            txtCookingMessage.Text = DefaultCookingMessage;
        }

        private void TimerCooking_Tick(object sender, EventArgs e)
        {
            var message = string.IsNullOrWhiteSpace(txtCookingMessage.Text)
                ? DefaultCookingMessage
                : txtCookingMessage.Text;

            notifyIcon.ShowBalloonTip(3000, $"[{settings.AppName}] 吃料理通知", message, ToolTipIcon.Info);

            // 播放音效
            const string soundName = "sound.wav";
            if (File.Exists(soundName))
            {
                Invoke((Action)delegate
                {
                    mp.Stop();
                    mp.Open(new Uri(new FileInfo(soundName).FullName));
                    mp.Volume = settings.SoundVol / 10d;
                    mp.Play();
                });
            }
        }

        #region notifyIcon, window minsize and exit ...

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Visible = true;
                this.WindowState = FormWindowState.Normal;
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ToolOpen_Click(object sender, EventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
        }

        private void ToolMinsize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void ToolExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormMain_MinimumSizeChanged(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 確保 AppName 在關閉前儲存
            var newAppName = txtAppName.Text.Trim();
            if (!string.IsNullOrEmpty(newAppName) && newAppName != settings.AppName)
            {
                settings.SetAppName(newAppName);
            }

            watcher?.Dispose();
            notifyIcon?.Dispose();
        }

        #endregion

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/WindOfNet/CgLogListener");
        }
    }
}
