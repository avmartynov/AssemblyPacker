namespace Twidlle.AssemblyPacker.WinFormsApp.Forms
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this._packButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this._inputDirectoryTextBox = new System.Windows.Forms.TextBox();
            this._bindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this._inputDirectoryBrowseButton = new System.Windows.Forms.Button();
            this._outputDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._outputDirectoryBrowseButton = new System.Windows.Forms.Button();
            this._inputDirectoryOpenButton = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this._outputDirectoryOpenButton = new System.Windows.Forms.Button();
            this._packedAssemblyNameTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this._generatePackScriptCheckBox = new System.Windows.Forms.CheckBox();
            this._inputFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this._outputFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this._bindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // _packButton
            // 
            this._packButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._packButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this._packButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._packButton.Location = new System.Drawing.Point(288, 23);
            this._packButton.Name = "_packButton";
            this._packButton.Size = new System.Drawing.Size(62, 23);
            this._packButton.TabIndex = 0;
            this._packButton.Text = "Pack";
            this.toolTip.SetToolTip(this._packButton, "Упаковать приложение");
            this._packButton.UseVisualStyleBackColor = true;
            this._packButton.Click += new System.EventHandler(this.PackButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(289, 158);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(61, 23);
            this.closeButton.TabIndex = 13;
            this.closeButton.Text = "Exit";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // _inputDirectoryTextBox
            // 
            this._inputDirectoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._inputDirectoryTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this._bindingSource, "InputDirectory", true));
            this._inputDirectoryTextBox.Location = new System.Drawing.Point(10, 74);
            this._inputDirectoryTextBox.Name = "_inputDirectoryTextBox";
            this._inputDirectoryTextBox.Size = new System.Drawing.Size(266, 20);
            this._inputDirectoryTextBox.TabIndex = 2;
            // 
            // _bindingSource
            // 
            this._bindingSource.DataSource = typeof(Twidlle.AssemblyPacker.WinFormsApp.Forms.MainFormModel);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Input directory:";
            // 
            // _inputDirectoryBrowseButton
            // 
            this._inputDirectoryBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._inputDirectoryBrowseButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._inputDirectoryBrowseButton.Location = new System.Drawing.Point(288, 73);
            this._inputDirectoryBrowseButton.Name = "_inputDirectoryBrowseButton";
            this._inputDirectoryBrowseButton.Size = new System.Drawing.Size(24, 22);
            this._inputDirectoryBrowseButton.TabIndex = 3;
            this._inputDirectoryBrowseButton.Text = "...";
            this.toolTip.SetToolTip(this._inputDirectoryBrowseButton, "Задать каталог исходного приложения");
            this._inputDirectoryBrowseButton.UseVisualStyleBackColor = true;
            this._inputDirectoryBrowseButton.Click += new System.EventHandler(this.InputDirectoryBrowseButton_Click);
            // 
            // _outputDirectoryTextBox
            // 
            this._outputDirectoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._outputDirectoryTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this._bindingSource, "OutputDirectory", true));
            this._outputDirectoryTextBox.Location = new System.Drawing.Point(10, 123);
            this._outputDirectoryTextBox.Name = "_outputDirectoryTextBox";
            this._outputDirectoryTextBox.Size = new System.Drawing.Size(266, 20);
            this._outputDirectoryTextBox.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 107);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Output directory:";
            // 
            // _outputDirectoryBrowseButton
            // 
            this._outputDirectoryBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._outputDirectoryBrowseButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._outputDirectoryBrowseButton.Location = new System.Drawing.Point(288, 122);
            this._outputDirectoryBrowseButton.Name = "_outputDirectoryBrowseButton";
            this._outputDirectoryBrowseButton.Size = new System.Drawing.Size(24, 22);
            this._outputDirectoryBrowseButton.TabIndex = 8;
            this._outputDirectoryBrowseButton.Text = "...";
            this.toolTip.SetToolTip(this._outputDirectoryBrowseButton, "Задать каталог упакованного приложения");
            this._outputDirectoryBrowseButton.UseVisualStyleBackColor = true;
            this._outputDirectoryBrowseButton.Click += new System.EventHandler(this.OutputDirectoryBrowseButton_Click);
            // 
            // _inputDirectoryOpenButton
            // 
            this._inputDirectoryOpenButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._inputDirectoryOpenButton.ImageIndex = 0;
            this._inputDirectoryOpenButton.ImageList = this.imageList1;
            this._inputDirectoryOpenButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._inputDirectoryOpenButton.Location = new System.Drawing.Point(324, 73);
            this._inputDirectoryOpenButton.Name = "_inputDirectoryOpenButton";
            this._inputDirectoryOpenButton.Size = new System.Drawing.Size(26, 22);
            this._inputDirectoryOpenButton.TabIndex = 4;
            this.toolTip.SetToolTip(this._inputDirectoryOpenButton, "Открыть каталог исходного приложения");
            this._inputDirectoryOpenButton.UseVisualStyleBackColor = true;
            this._inputDirectoryOpenButton.Click += new System.EventHandler(this.InputDirectoryOpenButton_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "folder.ico");
            // 
            // _outputDirectoryOpenButton
            // 
            this._outputDirectoryOpenButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._outputDirectoryOpenButton.ImageIndex = 0;
            this._outputDirectoryOpenButton.ImageList = this.imageList1;
            this._outputDirectoryOpenButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._outputDirectoryOpenButton.Location = new System.Drawing.Point(324, 122);
            this._outputDirectoryOpenButton.Name = "_outputDirectoryOpenButton";
            this._outputDirectoryOpenButton.Size = new System.Drawing.Size(26, 23);
            this._outputDirectoryOpenButton.TabIndex = 9;
            this.toolTip.SetToolTip(this._outputDirectoryOpenButton, "Открыть каталог упакованного приложения");
            this._outputDirectoryOpenButton.UseVisualStyleBackColor = true;
            this._outputDirectoryOpenButton.Click += new System.EventHandler(this.OutputDirectoryOpenButton_Click);
            // 
            // _packedAssemblyNameTextBox
            // 
            this._packedAssemblyNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._packedAssemblyNameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this._bindingSource, "PackedAssemblyName", true));
            this._packedAssemblyNameTextBox.Location = new System.Drawing.Point(10, 25);
            this._packedAssemblyNameTextBox.Name = "_packedAssemblyNameTextBox";
            this._packedAssemblyNameTextBox.Size = new System.Drawing.Size(266, 20);
            this._packedAssemblyNameTextBox.TabIndex = 12;
            this.toolTip.SetToolTip(this._packedAssemblyNameTextBox, "Name of created assembly, that will contain all assembly within input directory.");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(122, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Packed assembly name:";
            // 
            // _generatePackScriptCheckBox
            // 
            this._generatePackScriptCheckBox.AutoSize = true;
            this._generatePackScriptCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this._bindingSource, "Scripting", true));
            this._generatePackScriptCheckBox.Location = new System.Drawing.Point(10, 158);
            this._generatePackScriptCheckBox.Name = "_generatePackScriptCheckBox";
            this._generatePackScriptCheckBox.Size = new System.Drawing.Size(139, 17);
            this._generatePackScriptCheckBox.TabIndex = 16;
            this._generatePackScriptCheckBox.Text = "Generate packing script";
            this._generatePackScriptCheckBox.UseVisualStyleBackColor = true;
            // 
            // _inputFolderBrowserDialog
            // 
            this._inputFolderBrowserDialog.Description = "Choose source application directory.";
            this._inputFolderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this._inputFolderBrowserDialog.ShowNewFolderButton = false;
            // 
            // _outputFolderBrowserDialog
            // 
            this._outputFolderBrowserDialog.Description = "Choose destination directory.";
            this._outputFolderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(361, 194);
            this.Controls.Add(this._generatePackScriptCheckBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._packedAssemblyNameTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._outputDirectoryTextBox);
            this.Controls.Add(this._outputDirectoryBrowseButton);
            this.Controls.Add(this._inputDirectoryBrowseButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._inputDirectoryTextBox);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this._outputDirectoryOpenButton);
            this.Controls.Add(this._inputDirectoryOpenButton);
            this.Controls.Add(this._packButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(999, 233);
            this.MinimumSize = new System.Drawing.Size(377, 233);
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = ".Net Application Packer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this._bindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _packButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.TextBox _inputDirectoryTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _inputDirectoryBrowseButton;
        private System.Windows.Forms.TextBox _outputDirectoryTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button _outputDirectoryBrowseButton;
        private System.Windows.Forms.Button _inputDirectoryOpenButton;
        private System.Windows.Forms.Button _outputDirectoryOpenButton;
        private System.Windows.Forms.TextBox _packedAssemblyNameTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.FolderBrowserDialog _inputFolderBrowserDialog;
        private System.Windows.Forms.FolderBrowserDialog _outputFolderBrowserDialog;
        private System.Windows.Forms.CheckBox _generatePackScriptCheckBox;
        private System.Windows.Forms.BindingSource _bindingSource;
    }
}

