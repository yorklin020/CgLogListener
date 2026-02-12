namespace CgLogListener
{
    public class TipNotifyOptions
    {
        public string Text { get; set; }
        public string RegexPattern { get; set; }
        public bool Enabled { get; set; }
        public bool PlaySound { get; set; }
        public bool SendMail { get; set; }
        public bool IsRegex { get; set; }
        public bool CustomNotify { get; set; }

        public TipNotifyOptions()
        {
            Enabled = false;
            PlaySound = true;
            SendMail = false;
            IsRegex = false;
            CustomNotify = false;
        }

        public TipNotifyOptions(bool enabled, bool playSound, bool sendMail, bool isRegex = false, bool customNotify = false)
        {
            Enabled = enabled;
            PlaySound = playSound;
            SendMail = sendMail;
            IsRegex = isRegex;
            CustomNotify = customNotify;
        }

        /// <summary>
        /// 從 INI 字串解析
        /// 新格式: "Text|RegexPattern|enabled,playSound,sendMail,isRegex,customNotify" 如 "通知文字|regex.*pattern|1,1,0,0,1"
        /// 舊格式: "enabled,playSound,sendMail,isRegex,customNotify" 如 "1,1,0,0,1" (相容)
        /// 注意: RegexPattern 可能包含 | (regex OR)，所以用 LastIndexOf 從右邊切分
        /// </summary>
        public static TipNotifyOptions Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new TipNotifyOptions();
            }

            // 新格式: Text|RegexPattern|flags
            // RegexPattern 可能包含 | (如 "(你|您)")，所以從右邊找最後一個 | 來切 flags
            var lastPipeIndex = value.LastIndexOf('|');
            if (lastPipeIndex > 0)
            {
                var flagsPart = value.Substring(lastPipeIndex + 1);
                var flags = flagsPart.Split(',');

                if (flags.Length >= 3 && IsValidFlags(flags))
                {
                    var rest = value.Substring(0, lastPipeIndex);
                    var firstPipeIndex = rest.IndexOf('|');

                    string text, regexPattern;
                    if (firstPipeIndex >= 0)
                    {
                        text = rest.Substring(0, firstPipeIndex);
                        regexPattern = rest.Substring(firstPipeIndex + 1);
                    }
                    else
                    {
                        text = rest;
                        regexPattern = "";
                    }

                    return new TipNotifyOptions
                    {
                        Text = text,
                        RegexPattern = regexPattern,
                        Enabled = flags.Length > 0 && flags[0] == "1",
                        PlaySound = flags.Length > 1 && flags[1] == "1",
                        SendMail = flags.Length > 2 && flags[2] == "1",
                        IsRegex = flags.Length > 3 && flags[3] == "1",
                        CustomNotify = flags.Length > 4 && flags[4] == "1"
                    };
                }
            }

            // 舊格式相容
            var parts = value.Split(',');
            return new TipNotifyOptions
            {
                Enabled = parts.Length > 0 && parts[0] == "1",
                PlaySound = parts.Length > 1 && parts[1] == "1",
                SendMail = parts.Length > 2 && parts[2] == "1",
                IsRegex = parts.Length > 3 && parts[3] == "1",
                CustomNotify = parts.Length > 4 && parts[4] == "1"
            };
        }

        private static bool IsValidFlags(string[] flags)
        {
            for (int i = 0; i < flags.Length; i++)
            {
                if (flags[i] != "0" && flags[i] != "1")
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 轉成 INI 字串
        /// 如果有 Text 或 RegexPattern，使用新格式: "Text|RegexPattern|enabled,playSound,sendMail,isRegex,customNotify"
        /// 否則使用舊格式: "enabled,playSound,sendMail,isRegex,customNotify"
        /// </summary>
        public override string ToString()
        {
            var flags = $"{(Enabled ? "1" : "0")},{(PlaySound ? "1" : "0")},{(SendMail ? "1" : "0")},{(IsRegex ? "1" : "0")},{(CustomNotify ? "1" : "0")}";

            if (!string.IsNullOrEmpty(Text) || !string.IsNullOrEmpty(RegexPattern))
            {
                return $"{Text ?? ""}|{RegexPattern ?? ""}|{flags}";
            }

            return flags;
        }
    }
}
