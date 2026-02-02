namespace CgLogListener
{
    public class TipNotifyOptions
    {
        public bool Enabled { get; set; }
        public bool PlaySound { get; set; }
        public bool SendMail { get; set; }
        public bool IsRegex { get; set; }

        public TipNotifyOptions()
        {
            Enabled = false;
            PlaySound = true;
            SendMail = false;
            IsRegex = false;
        }

        public TipNotifyOptions(bool enabled, bool playSound, bool sendMail, bool isRegex = false)
        {
            Enabled = enabled;
            PlaySound = playSound;
            SendMail = sendMail;
            IsRegex = isRegex;
        }

        /// <summary>
        /// 從 INI 字串解析 (格式: "enabled,playSound,sendMail,isRegex" 如 "1,1,0,0")
        /// </summary>
        public static TipNotifyOptions Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new TipNotifyOptions();
            }

            var parts = value.Split(',');
            return new TipNotifyOptions
            {
                Enabled = parts.Length > 0 && parts[0] == "1",
                PlaySound = parts.Length > 1 && parts[1] == "1",
                SendMail = parts.Length > 2 && parts[2] == "1",
                IsRegex = parts.Length > 3 && parts[3] == "1"
            };
        }

        /// <summary>
        /// 轉成 INI 字串 (格式: "enabled,playSound,sendMail,isRegex")
        /// </summary>
        public override string ToString()
        {
            return $"{(Enabled ? "1" : "0")},{(PlaySound ? "1" : "0")},{(SendMail ? "1" : "0")},{(IsRegex ? "1" : "0")}";
        }
    }
}
