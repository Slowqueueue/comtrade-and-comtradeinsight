namespace COMTRADEInsight.Views
{
    partial class SaveForm
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SaveForm));
            folderBrowserDialog = new FolderBrowserDialog();
            selectFolderButton = new Button();
            folderRichTextBox = new RichTextBox();
            saveAsBindingSource = new BindingSource(components);
            fileNameTextBox = new RichTextBox();
            cffRadioButton = new RadioButton();
            cfgRadioButton = new RadioButton();
            dontWriteDatFileCheckBox = new CheckBox();
            asciiRadioButton = new RadioButton();
            binaryRadioButton = new RadioButton();
            binary32RadioButton = new RadioButton();
            float32RadioButton = new RadioButton();
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            label1 = new Label();
            label2 = new Label();
            cancelButton = new Button();
            saveButton = new Button();
            ((System.ComponentModel.ISupportInitialize)saveAsBindingSource).BeginInit();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // selectFolderButton
            // 
            selectFolderButton.Location = new Point(379, 23);
            selectFolderButton.Name = "selectFolderButton";
            selectFolderButton.Size = new Size(28, 25);
            selectFolderButton.TabIndex = 0;
            selectFolderButton.Text = "...";
            selectFolderButton.UseVisualStyleBackColor = true;
            // 
            // folderRichTextBox
            // 
            folderRichTextBox.DetectUrls = false;
            folderRichTextBox.Location = new Point(12, 24);
            folderRichTextBox.Name = "folderRichTextBox";
            folderRichTextBox.ReadOnly = true;
            folderRichTextBox.Size = new Size(361, 23);
            folderRichTextBox.TabIndex = 1;
            folderRichTextBox.Text = "";
            // 
            // fileNameTextBox
            // 
            fileNameTextBox.DetectUrls = false;
            fileNameTextBox.Location = new Point(12, 68);
            fileNameTextBox.Name = "fileNameTextBox";
            fileNameTextBox.Size = new Size(394, 23);
            fileNameTextBox.TabIndex = 2;
            fileNameTextBox.Text = "";
            // 
            // cffRadioButton
            // 
            cffRadioButton.AutoSize = true;
            cffRadioButton.Checked = true;
            cffRadioButton.Location = new Point(6, 20);
            cffRadioButton.Name = "cffRadioButton";
            cffRadioButton.Size = new Size(45, 19);
            cffRadioButton.TabIndex = 3;
            cffRadioButton.TabStop = true;
            cffRadioButton.Text = "CFF";
            cffRadioButton.UseVisualStyleBackColor = true;
            // 
            // cfgRadioButton
            // 
            cfgRadioButton.AutoSize = true;
            cfgRadioButton.Location = new Point(6, 45);
            cfgRadioButton.Name = "cfgRadioButton";
            cfgRadioButton.Size = new Size(47, 19);
            cfgRadioButton.TabIndex = 4;
            cfgRadioButton.Text = "CFG";
            cfgRadioButton.UseVisualStyleBackColor = true;
            cfgRadioButton.CheckedChanged += CfgRadioButton_CheckedChanged;
            // 
            // dontWriteDatFileCheckBox
            // 
            dontWriteDatFileCheckBox.AutoSize = true;
            dontWriteDatFileCheckBox.Enabled = false;
            dontWriteDatFileCheckBox.Location = new Point(71, 46);
            dontWriteDatFileCheckBox.Name = "dontWriteDatFileCheckBox";
            dontWriteDatFileCheckBox.Size = new Size(123, 19);
            dontWriteDatFileCheckBox.TabIndex = 5;
            dontWriteDatFileCheckBox.Text = "Не сохранять .dat";
            dontWriteDatFileCheckBox.UseVisualStyleBackColor = true;
            // 
            // asciiRadioButton
            // 
            asciiRadioButton.AutoSize = true;
            asciiRadioButton.Checked = true;
            asciiRadioButton.Location = new Point(6, 20);
            asciiRadioButton.Name = "asciiRadioButton";
            asciiRadioButton.Size = new Size(53, 19);
            asciiRadioButton.TabIndex = 6;
            asciiRadioButton.TabStop = true;
            asciiRadioButton.Text = "ASCII";
            asciiRadioButton.UseVisualStyleBackColor = true;
            // 
            // binaryRadioButton
            // 
            binaryRadioButton.AutoSize = true;
            binaryRadioButton.Location = new Point(6, 45);
            binaryRadioButton.Name = "binaryRadioButton";
            binaryRadioButton.Size = new Size(66, 19);
            binaryRadioButton.TabIndex = 7;
            binaryRadioButton.Text = "BINARY";
            binaryRadioButton.UseVisualStyleBackColor = true;
            // 
            // binary32RadioButton
            // 
            binary32RadioButton.AutoSize = true;
            binary32RadioButton.Location = new Point(6, 70);
            binary32RadioButton.Name = "binary32RadioButton";
            binary32RadioButton.Size = new Size(78, 19);
            binary32RadioButton.TabIndex = 8;
            binary32RadioButton.Text = "BINARY32";
            binary32RadioButton.UseVisualStyleBackColor = true;
            // 
            // float32RadioButton
            // 
            float32RadioButton.AutoSize = true;
            float32RadioButton.Location = new Point(6, 95);
            float32RadioButton.Name = "float32RadioButton";
            float32RadioButton.Size = new Size(71, 19);
            float32RadioButton.TabIndex = 9;
            float32RadioButton.Text = "FLOAT32";
            float32RadioButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(cffRadioButton);
            groupBox1.Controls.Add(cfgRadioButton);
            groupBox1.Controls.Add(dontWriteDatFileCheckBox);
            groupBox1.Location = new Point(12, 97);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(200, 70);
            groupBox1.TabIndex = 10;
            groupBox1.TabStop = false;
            groupBox1.Text = "Формат файла";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(asciiRadioButton);
            groupBox2.Controls.Add(binaryRadioButton);
            groupBox2.Controls.Add(float32RadioButton);
            groupBox2.Controls.Add(binary32RadioButton);
            groupBox2.Location = new Point(218, 97);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(188, 121);
            groupBox2.TabIndex = 11;
            groupBox2.TabStop = false;
            groupBox2.Text = "Тип файла";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 6);
            label1.Name = "label1";
            label1.Size = new Size(134, 15);
            label1.TabIndex = 12;
            label1.Text = "Папка для сохранения:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 50);
            label2.Name = "label2";
            label2.Size = new Size(72, 15);
            label2.TabIndex = 13;
            label2.Text = "Имя файла:";
            // 
            // cancelButton
            // 
            cancelButton.Location = new Point(11, 173);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(97, 45);
            cancelButton.TabIndex = 14;
            cancelButton.Text = "Отмена";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // saveButton
            // 
            saveButton.Location = new Point(115, 173);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(98, 45);
            saveButton.TabIndex = 15;
            saveButton.Text = "Сохранить";
            saveButton.UseVisualStyleBackColor = true;
            // 
            // SaveForm
            // 
            AcceptButton = saveButton;
            AutoScaleMode = AutoScaleMode.None;
            CancelButton = cancelButton;
            ClientSize = new Size(418, 229);
            Controls.Add(saveButton);
            Controls.Add(cancelButton);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(fileNameTextBox);
            Controls.Add(folderRichTextBox);
            Controls.Add(selectFolderButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SaveForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Сохранение";
            ((System.ComponentModel.ISupportInitialize)saveAsBindingSource).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FolderBrowserDialog folderBrowserDialog;
        private Button selectFolderButton;
        private RichTextBox folderRichTextBox;
        private BindingSource saveAsBindingSource;
        private RichTextBox fileNameTextBox;
        private RadioButton cffRadioButton;
        private RadioButton cfgRadioButton;
        private CheckBox dontWriteDatFileCheckBox;
        private RadioButton asciiRadioButton;
        private RadioButton binaryRadioButton;
        private RadioButton binary32RadioButton;
        private RadioButton float32RadioButton;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Label label1;
        private Label label2;
        private Button cancelButton;
        private Button saveButton;
    }
}