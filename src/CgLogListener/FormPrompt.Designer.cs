namespace CgLogListener
{
    partial class FormPrompt
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOk = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtExp = new System.Windows.Forms.TextBox();
            this.chkEnabled = new System.Windows.Forms.CheckBox();
            this.chkPlaySound = new System.Windows.Forms.CheckBox();
            this.chkSendMail = new System.Windows.Forms.CheckBox();
            this.chkIsRegex = new System.Windows.Forms.CheckBox();
            this.chkCustomNotify = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(299, 2);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(57, 26);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "設定";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "關鍵字";
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(67, 7);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(210, 21);
            this.txtValue.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(299, 31);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(57, 26);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "排除 (以逗號分隔)";
            // 
            // txtExp
            // 
            this.txtExp.Location = new System.Drawing.Point(112, 34);
            this.txtExp.Name = "txtExp";
            this.txtExp.Size = new System.Drawing.Size(165, 21);
            this.txtExp.TabIndex = 2;
            //
            // chkEnabled
            //
            this.chkEnabled.AutoSize = true;
            this.chkEnabled.Checked = true;
            this.chkEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEnabled.Location = new System.Drawing.Point(14, 61);
            this.chkEnabled.Name = "chkEnabled";
            this.chkEnabled.Size = new System.Drawing.Size(52, 19);
            this.chkEnabled.TabIndex = 5;
            this.chkEnabled.Text = "啟用";
            this.chkEnabled.UseVisualStyleBackColor = true;
            //
            // chkPlaySound
            //
            this.chkPlaySound.AutoSize = true;
            this.chkPlaySound.Checked = true;
            this.chkPlaySound.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPlaySound.Location = new System.Drawing.Point(72, 61);
            this.chkPlaySound.Name = "chkPlaySound";
            this.chkPlaySound.Size = new System.Drawing.Size(52, 19);
            this.chkPlaySound.TabIndex = 6;
            this.chkPlaySound.Text = "音效";
            this.chkPlaySound.UseVisualStyleBackColor = true;
            //
            // chkSendMail
            //
            this.chkSendMail.AutoSize = true;
            this.chkSendMail.Location = new System.Drawing.Point(130, 61);
            this.chkSendMail.Name = "chkSendMail";
            this.chkSendMail.Size = new System.Drawing.Size(52, 19);
            this.chkSendMail.TabIndex = 7;
            this.chkSendMail.Text = "寄信";
            this.chkSendMail.UseVisualStyleBackColor = true;
            //
            // chkIsRegex
            //
            this.chkIsRegex.AutoSize = true;
            this.chkIsRegex.Location = new System.Drawing.Point(188, 61);
            this.chkIsRegex.Name = "chkIsRegex";
            this.chkIsRegex.Size = new System.Drawing.Size(58, 19);
            this.chkIsRegex.TabIndex = 8;
            this.chkIsRegex.Text = "Regex";
            this.chkIsRegex.UseVisualStyleBackColor = true;
            //
            // chkCustomNotify
            //
            this.chkCustomNotify.AutoSize = true;
            this.chkCustomNotify.Location = new System.Drawing.Point(252, 61);
            this.chkCustomNotify.Name = "chkCustomNotify";
            this.chkCustomNotify.Size = new System.Drawing.Size(34, 19);
            this.chkCustomNotify.TabIndex = 9;
            this.chkCustomNotify.Text = "C";
            this.chkCustomNotify.UseVisualStyleBackColor = true;
            //
            // FormPrompt
            //
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(368, 90);
            this.ControlBox = false;
            this.Controls.Add(this.chkCustomNotify);
            this.Controls.Add(this.chkIsRegex);
            this.Controls.Add(this.chkSendMail);
            this.Controls.Add(this.chkPlaySound);
            this.Controls.Add(this.chkEnabled);
            this.Controls.Add(this.txtExp);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtValue);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Font = new System.Drawing.Font("微軟正黑體", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPrompt";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "新增/編輯自訂關鍵字";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtExp;
        private System.Windows.Forms.CheckBox chkEnabled;
        private System.Windows.Forms.CheckBox chkPlaySound;
        private System.Windows.Forms.CheckBox chkSendMail;
        private System.Windows.Forms.CheckBox chkIsRegex;
        private System.Windows.Forms.CheckBox chkCustomNotify;
    }
}