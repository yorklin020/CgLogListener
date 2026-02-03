using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CgLogListener
{
    public sealed class Settings
    {
        static Settings instance;
        const string settingsFileName = "settings.ini";
        const string settingsBaseSection = "base";
        const string settingsStandardTipsSection = "standard tips";
        const string settingsCustomTipsSection = "custom tips";
        const string custmizeFileName = "custmize.dat";

        public string AppName { get; private set; }
        public string CustomNotifier { get; private set; }
        public int SoundVol { get; private set; }
        public string CgLogPath { get; private set; }
        public Dictionary<string, TipNotifyOptions> StandardTips { get; private set; } = new Dictionary<string, TipNotifyOptions>();
        public Dictionary<string, TipNotifyOptions> CustomizeTips { get; private set; } = new Dictionary<string, TipNotifyOptions>();

        public static Settings GetInstance()
        {
            if (instance == null)
            {
                instance = new Settings();
                instance.Load();
            }

            return instance;
        }

        public void Load()
        {
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), settingsFileName)))
            {
                GenConfigFile();
            }

            LoadSettings();
        }

        private void LoadSettings()
        {
            var fileIniDataParser = new FileIniDataParser();
            var iniData = fileIniDataParser.ReadFile(settingsFileName);

            var baseData = iniData[settingsBaseSection];
            AppName = baseData[nameof(AppName)] ?? "CgLogListener";
            if (string.IsNullOrEmpty(AppName)) AppName = "CgLogListener";
            CgLogPath = baseData[nameof(CgLogPath)];
            SoundVol = int.Parse(baseData[nameof(SoundVol)] ?? "5");
            CustomNotifier = baseData[nameof(CustomNotifier)];

            // 載入標準關鍵字設定
            var standardTipData = iniData[settingsStandardTipsSection];
            foreach (var kd in standardTipData)
            {
                StandardTips[kd.KeyName] = TipNotifyOptions.Parse(kd.Value);
            }

            // 載入自訂關鍵字設定
            var customTipData = iniData[settingsCustomTipsSection];
            foreach (var kd in customTipData)
            {
                CustomizeTips[kd.KeyName] = TipNotifyOptions.Parse(kd.Value);
            }

            // 相容舊版：讀取舊的 custmize.dat
            if (File.Exists(custmizeFileName))
            {
                foreach (var s in File.ReadAllLines(custmizeFileName))
                {
                    if (!string.IsNullOrEmpty(s) && !CustomizeTips.ContainsKey(s))
                    {
                        CustomizeTips[s] = new TipNotifyOptions(true, true, false);
                    }
                }
                // 遷移後刪除舊檔
                try { File.Delete(custmizeFileName); } catch { }
            }
        }

        private void GenConfigFile()
        {
            var iniData = new IniData();
            var baseSection = iniData[settingsBaseSection];
            baseSection[nameof(AppName)] = "CgLogListener";
            baseSection[nameof(CgLogPath)] = string.Empty;
            baseSection[nameof(SoundVol)] = "5";

            var fileIniDataParser = new FileIniDataParser();
            fileIniDataParser.WriteFile(settingsFileName, iniData);
        }

        private void UpdateConfig()
        {
            var fileIniDataParser = new FileIniDataParser();
            var iniData = new IniData();

            var baseSection = iniData[settingsBaseSection];
            baseSection[nameof(AppName)] = AppName;
            baseSection[nameof(CgLogPath)] = CgLogPath;
            baseSection[nameof(SoundVol)] = SoundVol.ToString();
            baseSection[nameof(CustomNotifier)] = CustomNotifier;

            var standardTipData = iniData[settingsStandardTipsSection];
            foreach (var kv in StandardTips)
            {
                standardTipData[kv.Key] = kv.Value.ToString();
            }

            var customTipData = iniData[settingsCustomTipsSection];
            foreach (var kv in CustomizeTips)
            {
                customTipData[kv.Key] = kv.Value.ToString();
            }

            fileIniDataParser.WriteFile(settingsFileName, iniData);
        }

        internal void SetCgLogPath(string cgLogPath)
        {
            CgLogPath = cgLogPath;
            UpdateConfig();
        }

        internal void SetStandardTip(string nameInSetting, TipNotifyOptions options)
        {
            StandardTips[nameInSetting] = options;
            UpdateConfig();
        }

        internal void SetStandardTipEnabled(string nameInSetting, bool enabled)
        {
            if (!StandardTips.ContainsKey(nameInSetting))
            {
                StandardTips[nameInSetting] = new TipNotifyOptions();
            }
            StandardTips[nameInSetting].Enabled = enabled;
            UpdateConfig();
        }

        internal void SetStandardTipPlaySound(string nameInSetting, bool playSound)
        {
            if (!StandardTips.ContainsKey(nameInSetting))
            {
                StandardTips[nameInSetting] = new TipNotifyOptions();
            }
            StandardTips[nameInSetting].PlaySound = playSound;
            UpdateConfig();
        }

        internal void SetStandardTipSendMail(string nameInSetting, bool sendMail)
        {
            if (!StandardTips.ContainsKey(nameInSetting))
            {
                StandardTips[nameInSetting] = new TipNotifyOptions();
            }
            StandardTips[nameInSetting].SendMail = sendMail;
            UpdateConfig();
        }

        internal void SetSoundVol(int value)
        {
            SoundVol = value;
            UpdateConfig();
        }

        internal void AddCustomizeTip(string keyword, TipNotifyOptions options)
        {
            CustomizeTips[keyword] = options;
            UpdateConfig();
        }

        internal void RemoveCustomizeTip(string keyword)
        {
            CustomizeTips.Remove(keyword);
            UpdateConfig();
        }

        internal void SetCustomizeTipEnabled(string keyword, bool enabled)
        {
            if (CustomizeTips.ContainsKey(keyword))
            {
                CustomizeTips[keyword].Enabled = enabled;
                UpdateConfig();
            }
        }

        internal void SetCustomizeTipPlaySound(string keyword, bool playSound)
        {
            if (CustomizeTips.ContainsKey(keyword))
            {
                CustomizeTips[keyword].PlaySound = playSound;
                UpdateConfig();
            }
        }

        internal void SetCustomizeTipSendMail(string keyword, bool sendMail)
        {
            if (CustomizeTips.ContainsKey(keyword))
            {
                CustomizeTips[keyword].SendMail = sendMail;
                UpdateConfig();
            }
        }

        internal void SetCustomNotifier(string value)
        {
            CustomNotifier = value;
            UpdateConfig();
        }

        internal void SetStandardTipCustomNotify(string nameInSetting, bool customNotify)
        {
            if (!StandardTips.ContainsKey(nameInSetting))
            {
                StandardTips[nameInSetting] = new TipNotifyOptions();
            }
            StandardTips[nameInSetting].CustomNotify = customNotify;
            UpdateConfig();
        }

        internal void SetCustomizeTipCustomNotify(string keyword, bool customNotify)
        {
            if (CustomizeTips.ContainsKey(keyword))
            {
                CustomizeTips[keyword].CustomNotify = customNotify;
                UpdateConfig();
            }
        }

        internal void SetAppName(string value)
        {
            AppName = string.IsNullOrEmpty(value) ? "CgLogListener" : value;
            UpdateConfig();
        }
    }
}
