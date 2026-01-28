# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build Commands

```bash
# Build entire solution
msbuild src/CgLogListener.sln

# Build specific project
msbuild src/CgLogListener/CgLogListener.csproj

# Build Release version
msbuild src/CgLogListener.sln /p:Configuration=Release

# Restore NuGet packages (if needed)
nuget restore src/CgLogListener.sln
```

## Architecture

This is a Windows Forms application (.NET Framework 4.6) that monitors game log files and sends notifications when keywords are detected.

### Project Structure

```
src/
├── CgLogListener/          # Main WinForms application
│   ├── FormMain.cs         # Main UI, event handling, notification triggers
│   ├── FormMain.Designer.cs # UI layout (auto-generated, can be manually edited)
│   ├── CgLogHandler.cs     # FileSystemWatcher for log files (BIG5 encoding)
│   ├── CgLogListenerControls.cs # Custom controls (CgLogListenerCheckBox, CgLogListenerListBox)
│   ├── Settings.cs         # Singleton, reads/writes settings.ini
│   ├── TipNotifyOptions.cs # Data class for keyword settings (enabled, playSound, sendMail)
│   ├── NotifyResult.cs     # Return type for INotifyMessage.Notify()
│   ├── INotifyMessage.cs   # Interface for notification controls
│   └── MailHelper.cs       # SMTP email sending, reads mail.ini
├── DiscordNotifier/        # External notifier (Console App)
├── TelegramNotifier/       # External notifier (Console App)
└── LineNotifier/           # External notifier (Console App)
```

### Core Interfaces

```csharp
// INotifyMessage.cs
public interface INotifyMessage
{
    NotifyResult Notify(string message);
}

// NotifyResult.cs
public class NotifyResult
{
    public bool IsMatch { get; set; }
    public bool PlaySound { get; set; }
    public bool SendMail { get; set; }
}

// TipNotifyOptions.cs - stored in settings.ini as "enabled,playSound,sendMail" (e.g., "1,1,0")
public class TipNotifyOptions
{
    public bool Enabled { get; set; }
    public bool PlaySound { get; set; }
    public bool SendMail { get; set; }
}
```

### Notification Flow

```
Log file changes → CgLogHandler.OnNewLog event
    ↓
FormMain.Watcher_OnNewLog(log)
    ↓
1. Check 吃料理通知 (cooking reminder with timer)
    ↓
2. Loop through panel1.Controls.OfType<INotifyMessage>()
    - CgLogListenerCheckBox (standard keywords with regex)
    - CgLogListenerListBox (custom keywords with contains)
    ↓
3. If result.IsMatch:
    - Show BalloonTip notification
    - Play sound.wav (if result.PlaySound)
    - Send email via MailHelper (if result.SendMail)
    - Call external notifiers (if CustomNotify enabled)
```

### UI Components

| Control | Type | Function |
|---------|------|----------|
| cgLogListenerCheckBox1-6 | CgLogListenerCheckBox | Standard keyword checkboxes with RegexPattern |
| Dynamic 🔊/✉ checkboxes | CheckBox | Per-keyword sound/mail toggles (created in SetupStandardTips) |
| cgLogListenerListBox | CgLogListenerListBox | Custom keywords list |
| btnAddCus / btnDelCus | Button | Add/remove custom keywords |
| cgLogListenerTrackBar | TrackBar | Sound volume (0-10) |
| checkBox1 | CheckBox | Custom Notify (external notifier) |
| chkCookingReminder | CheckBox | Cooking reminder toggle |
| txtCookingInterval | TextBox | Cooking reminder interval (seconds) |
| timerCooking | Timer | Cooking reminder timer |

### Standard Keywords (RegexPattern)

| NameInSetting | Pattern | Description |
|---------------|---------|-------------|
| Health | `在工作時不小心受傷了。` | 採集受傷通知 |
| ItemFull | `物品欄沒有空位。` | 道具滿通知 |
| MP0 | `魔力不足。` | 魔力不足通知 |
| PlayerJoin | `加入了(你\|您)的隊伍。` | 被加入隊伍通知 |
| Sell | `您順利賣掉了一個.*，(收入\|獲得).*魔幣！` | 擺攤售出通知 |
| ReMaze | `你感覺到一股不可思議的力量，而『.*』好像快(要?)消失了。` | 迷宮重組通知 |

### Cooking Reminder (吃料理通知)

- **Trigger**: Detects `恢復了\d+魔力` in log
- **Behavior**: Resets timer on each detection, alerts when timer expires
- **Flow**:
  ```
  [✓] 吃料理通知 [180] 秒
      ↓
  Log: "恢復了123魔力" detected
      ↓
  Timer reset, starts counting down
      ↓
  180 seconds later → BalloonTip + sound.wav
  ```

### Configuration Files

| File | Format | Purpose |
|------|--------|---------|
| `settings.ini` | INI | Main settings |
| `mail.ini` | INI | SMTP credentials (host, port, username, password, from, to) |

**settings.ini structure:**
```ini
[base]
CgLogPath=C:\Game\CrossGate
SoundVol=5
CustomNotify=0
CustomNotifier=

[standard tips]
Health=1,1,0          # enabled=1, playSound=1, sendMail=0
ItemFull=1,1,1        # enabled=1, playSound=1, sendMail=1

[custom tips]
關鍵字=1,1,0
關鍵字|排除詞=1,1,0   # keyword with exclusion
```

**mail.ini structure:**
```ini
[smtp]
host=smtp.gmail.com
port=587
enableSsl=1
username=
password=
from=your-email@gmail.com
to=recipient@example.com
```

### Adding a New Standard Keyword

1. Add `CgLogListenerCheckBox` in `FormMain.Designer.cs`
2. Set `NameInSetting` and `RegexPattern` properties
3. Add to `standardCheckBoxes` array in `FormMain.cs` `SetupStandardTips()`

### Adding a New External Notifier

1. Create new Console App project targeting .NET Framework 4.6
2. Add `ini-parser` NuGet package
3. Read config from `{NotifierName}.ini` in same directory as exe
4. Accept message as `args[0]`
5. User configures path in `settings.ini` under `CustomNotifier`
