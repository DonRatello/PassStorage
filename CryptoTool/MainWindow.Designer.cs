namespace CryptoTool
{
    partial class MainWindow
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
            this.txtOrigin = new System.Windows.Forms.TextBox();
            this.btnHash = new System.Windows.Forms.Button();
            this.txtHash = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtOrigin
            // 
            this.txtOrigin.Location = new System.Drawing.Point(12, 12);
            this.txtOrigin.Name = "txtOrigin";
            this.txtOrigin.Size = new System.Drawing.Size(290, 20);
            this.txtOrigin.TabIndex = 0;
            // 
            // btnHash
            // 
            this.btnHash.Location = new System.Drawing.Point(308, 9);
            this.btnHash.Name = "btnHash";
            this.btnHash.Size = new System.Drawing.Size(75, 23);
            this.btnHash.TabIndex = 1;
            this.btnHash.Text = "Hash";
            this.btnHash.UseVisualStyleBackColor = true;
            this.btnHash.Click += new System.EventHandler(this.btnHash_Click);
            // 
            // txtHash
            // 
            this.txtHash.Location = new System.Drawing.Point(12, 48);
            this.txtHash.Name = "txtHash";
            this.txtHash.Size = new System.Drawing.Size(290, 20);
            this.txtHash.TabIndex = 2;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 114);
            this.Controls.Add(this.txtHash);
            this.Controls.Add(this.btnHash);
            this.Controls.Add(this.txtOrigin);
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CryptoTool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtOrigin;
        private System.Windows.Forms.Button btnHash;
        private System.Windows.Forms.TextBox txtHash;
    }
}

