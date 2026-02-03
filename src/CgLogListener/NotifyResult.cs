namespace CgLogListener
{
    public class NotifyResult
    {
        public bool IsMatch { get; set; }
        public bool PlaySound { get; set; }
        public bool SendMail { get; set; }
        public bool CustomNotify { get; set; }

        public static NotifyResult NoMatch => new NotifyResult { IsMatch = false };

        public static NotifyResult Match(bool playSound, bool sendMail, bool customNotify)
        {
            return new NotifyResult
            {
                IsMatch = true,
                PlaySound = playSound,
                SendMail = sendMail,
                CustomNotify = customNotify
            };
        }
    }
}
