using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CgLogListener
{
    public partial class FormPrompt : Form
    {
        FormPrompt()
        {
            InitializeComponent();
            this.ImeMode = ImeMode.OnHalf;
        }

        public static DialogResult ShowDialog(IWin32Window owner, out string value)
        {
            var result = ShowDialog(owner, out value, out _);
            return result;
        }

        public static DialogResult ShowDialog(IWin32Window owner, out string value, out TipNotifyOptions options)
        {
            FormPrompt f = new FormPrompt();
            DialogResult result = f.ShowDialog(owner);
            value = f.txtValue.Text;
            if (!string.IsNullOrWhiteSpace(f.txtExp.Text))
            {
                value += $"|{f.txtExp.Text}";
            }

            options = new TipNotifyOptions(
                f.chkEnabled.Checked,
                f.chkPlaySound.Checked,
                f.chkSendMail.Checked,
                f.chkIsRegex.Checked,
                f.chkCustomNotify.Checked
            );

            return result;
        }

        /// <summary>
        /// 編輯模式：預填已有的關鍵字和選項
        /// </summary>
        public static DialogResult ShowDialogForEdit(IWin32Window owner, string existingKeyword, TipNotifyOptions existingOptions, out string value, out TipNotifyOptions options)
        {
            FormPrompt f = new FormPrompt();
            f.Text = "編輯自訂關鍵字";

            // 解析 keyword 和排除詞
            var split = existingKeyword.Split('|');
            f.txtValue.Text = split[0];
            if (split.Length > 1)
            {
                f.txtExp.Text = split[1];
            }

            // 預填選項
            f.chkEnabled.Checked = existingOptions.Enabled;
            f.chkPlaySound.Checked = existingOptions.PlaySound;
            f.chkSendMail.Checked = existingOptions.SendMail;
            f.chkIsRegex.Checked = existingOptions.IsRegex;
            f.chkCustomNotify.Checked = existingOptions.CustomNotify;

            DialogResult result = f.ShowDialog(owner);
            value = f.txtValue.Text;
            if (!string.IsNullOrWhiteSpace(f.txtExp.Text))
            {
                value += $"|{f.txtExp.Text}";
            }

            options = new TipNotifyOptions(
                f.chkEnabled.Checked,
                f.chkPlaySound.Checked,
                f.chkSendMail.Checked,
                f.chkIsRegex.Checked,
                f.chkCustomNotify.Checked
            );

            return result;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }
    }
}
