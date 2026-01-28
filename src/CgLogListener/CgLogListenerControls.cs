using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CgLogListener
{
    public class CgLogListenerCheckBox : CheckBox, INotifyMessage
    {
        public string NameInSetting { get; set; }
        public string RegexPattern { get; set; }

        public NotifyResult Notify(string message)
        {
            var settings = Settings.GetInstance();

            if (settings.StandardTips.TryGetValue(NameInSetting, out TipNotifyOptions options) &&
                options.Enabled &&
                Regex.IsMatch(message, RegexPattern))
            {
                return NotifyResult.Match(options.PlaySound, options.SendMail);
            }

            return NotifyResult.NoMatch;
        }
    }

    public class CgLogListenerTrackBar : TrackBar
    {
        public string NameInSetting { get; set; }
    }

    public class CgLogListenerListBox : ListBox, INotifyMessage
    {
        public NotifyIcon NotifyIcon { get; set; }

        public NotifyResult Notify(string message)
        {
            var settings = Settings.GetInstance();
            foreach (var kv in settings.CustomizeTips)
            {
                var keyword = kv.Key;
                var options = kv.Value;

                if (!options.Enabled) continue;

                var split = keyword.Split('|');

                if (message.Contains(split[0]))
                {
                    if (split.Length > 1)
                    {
                        var exps = split[1].Split(',');
                        if (!exps.Any(x => message.Contains(x)))
                        {
                            return NotifyResult.Match(options.PlaySound, options.SendMail);
                        }
                    }
                    else
                    {
                        return NotifyResult.Match(options.PlaySound, options.SendMail);
                    }
                }
            }

            return NotifyResult.NoMatch;
        }
    }
}
