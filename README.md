CgLogListener
====
設計給魔力寶貝使用的輔助程式 

主要為監視Log資料夾, 並依據內建(或自訂)的關鍵字提醒警示  

![demo](https://i.imgur.com/28FDH9D.png)  

![demo](https://i.imgur.com/xT5ZAwe.png)



目錄下可替換wav

York
20260128
1. 增加 smtp mailsettings (暫時只使用 gmail 進行測試)。
2. 新增物品損壞通知。
3. 設定應用來源，方便信件過濾。
4. 新增吃料偵測，秒數循環提醒。

20260203
1. 預設選項可由 setting.ini 中自由修改命名、regex 規則。
2. 吃料偵測改成計時器功能，可自定義秒數、提示訊息、regex 規則，若須恢復吃料判斷，案預設即可。
3. custom notify 路徑設定改成跟選擇魔力資料夾相同，不用再手動打。
4. custom notify 是否啟用通知，改成跟 sound, mail 一樣，可自行勾選是否啟用。
5. 自訂關鍵字功能，可自行增加、修改，並增加 sound mail, notify, regex 彈性設定。

![demo](https://i.meee.com.tw/QBmHzKn.png)
