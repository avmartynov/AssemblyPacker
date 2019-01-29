namespace Twidlle.Infrastructure.WinForms
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this._pictureBox = new System.Windows.Forms.PictureBox();
            this._copyrightLabel = new System.Windows.Forms.Label();
            this._versionLabel = new System.Windows.Forms.Label();
            this._productLabel = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // _pictureBox
            // 
            this._pictureBox.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this._pictureBox, "_pictureBox");
            this._pictureBox.Name = "_pictureBox";
            this._pictureBox.TabStop = false;
            // 
            // _copyrightLabel
            // 
            resources.ApplyResources(this._copyrightLabel, "_copyrightLabel");
            this._copyrightLabel.Name = "_copyrightLabel";
            // 
            // _versionLabel
            // 
            resources.ApplyResources(this._versionLabel, "_versionLabel");
            this._versionLabel.Name = "_versionLabel";
            // 
            // _productLabel
            // 
            resources.ApplyResources(this._productLabel, "_productLabel");
            this._productLabel.Name = "_productLabel";
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // AboutForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonOK;
            this.Controls.Add(this._pictureBox);
            this.Controls.Add(this._copyrightLabel);
            this.Controls.Add(this._versionLabel);
            this.Controls.Add(this._productLabel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Load += new System.EventHandler(this.AboutForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AboutForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox _pictureBox;
        private System.Windows.Forms.Label _copyrightLabel;
        private System.Windows.Forms.Label _versionLabel;
        private System.Windows.Forms.Label _productLabel;
        private System.Windows.Forms.Button buttonOK;
    }
}