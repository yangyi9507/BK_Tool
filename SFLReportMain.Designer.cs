
namespace BK_Tool
{
    partial class SFLReportMain
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
            this.lblResolve = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.txtAnaly = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblResolve
            // 
            this.lblResolve.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblResolve.AutoSize = true;
            this.lblResolve.Location = new System.Drawing.Point(381, 102);
            this.lblResolve.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblResolve.Name = "lblResolve";
            this.lblResolve.Size = new System.Drawing.Size(98, 18);
            this.lblResolve.TabIndex = 34;
            this.lblResolve.Text = "分解解析：";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(49, 14);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(141, 62);
            this.btnStart.TabIndex = 32;
            this.btnStart.Text = "启动服务";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(46, 102);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 18);
            this.label3.TabIndex = 31;
            this.label3.Text = "日志：";
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(61, 147);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(301, 520);
            this.txtLog.TabIndex = 32;
            // 
            // txtAnaly
            // 
            this.txtAnaly.Location = new System.Drawing.Point(384, 149);
            this.txtAnaly.Multiline = true;
            this.txtAnaly.Name = "txtAnaly";
            this.txtAnaly.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtAnaly.Size = new System.Drawing.Size(749, 520);
            this.txtAnaly.TabIndex = 35;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(198, 14);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(141, 62);
            this.button1.TabIndex = 36;
            this.button1.Text = "测试服务";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // SFLReportMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1307, 681);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtAnaly);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.lblResolve);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.label3);
            this.Name = "SFLReportMain";
            this.Text = "SFLReportMain";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblResolve;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.TextBox txtAnaly;
        private System.Windows.Forms.Button button1;
    }
}