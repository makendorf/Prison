namespace PrisonClient
{
    partial class Main
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ShopListBox = new System.Windows.Forms.ListBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageServices = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.ServicesListBox = new System.Windows.Forms.ListBox();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ServiceStartButton = new System.Windows.Forms.Button();
            this.ServiceStopButton = new System.Windows.Forms.Button();
            this.ServiceRestartButton = new System.Windows.Forms.Button();
            this.ServiceAutoStartCheckBox = new MetroFramework.Controls.MetroToggle();
            this.ServiceCheckIntervalTextBox = new MetroFramework.Controls.MetroTextBox();
            this.ServiceStatusLabel = new MetroFramework.Controls.MetroLabel();
            this.ServiceAutoStartLabel = new MetroFramework.Controls.MetroLabel();
            this.ServiceCheckIntervalLabel = new MetroFramework.Controls.MetroLabel();
            this.ServiceNameLabel = new MetroFramework.Controls.MetroLabel();
            this.LogListBox = new System.Windows.Forms.ListBox();
            this.tabPagePutty = new System.Windows.Forms.TabPage();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageServices.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(20, 60);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.ShopListBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1127, 607);
            this.splitContainer1.SplitterDistance = 261;
            this.splitContainer1.TabIndex = 0;
            // 
            // ShopListBox
            // 
            this.ShopListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ShopListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ShopListBox.FormattingEnabled = true;
            this.ShopListBox.ItemHeight = 24;
            this.ShopListBox.Location = new System.Drawing.Point(0, 0);
            this.ShopListBox.Name = "ShopListBox";
            this.ShopListBox.Size = new System.Drawing.Size(261, 607);
            this.ShopListBox.TabIndex = 0;
            this.ShopListBox.SelectedIndexChanged += new System.EventHandler(this.ShopListBox_SelectedIndexChanged);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.LogListBox);
            this.splitContainer2.Size = new System.Drawing.Size(862, 607);
            this.splitContainer2.SplitterDistance = 472;
            this.splitContainer2.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageServices);
            this.tabControl1.Controls.Add(this.tabPagePutty);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(862, 472);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPageServices
            // 
            this.tabPageServices.Controls.Add(this.splitContainer3);
            this.tabPageServices.Location = new System.Drawing.Point(4, 22);
            this.tabPageServices.Name = "tabPageServices";
            this.tabPageServices.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageServices.Size = new System.Drawing.Size(854, 446);
            this.tabPageServices.TabIndex = 0;
            this.tabPageServices.Text = "Службы";
            this.tabPageServices.UseVisualStyleBackColor = true;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(3, 3);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.ServicesListBox);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.splitContainer4);
            this.splitContainer3.Size = new System.Drawing.Size(848, 440);
            this.splitContainer3.SplitterDistance = 282;
            this.splitContainer3.TabIndex = 0;
            // 
            // ServicesListBox
            // 
            this.ServicesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ServicesListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ServicesListBox.FormattingEnabled = true;
            this.ServicesListBox.ItemHeight = 24;
            this.ServicesListBox.Location = new System.Drawing.Point(0, 0);
            this.ServicesListBox.Name = "ServicesListBox";
            this.ServicesListBox.Size = new System.Drawing.Size(282, 440);
            this.ServicesListBox.TabIndex = 0;
            this.ServicesListBox.SelectedIndexChanged += new System.EventHandler(this.ServicesListBox_SelectedIndexChanged);
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer4.Panel2.Controls.Add(this.ServiceAutoStartCheckBox);
            this.splitContainer4.Panel2.Controls.Add(this.ServiceCheckIntervalTextBox);
            this.splitContainer4.Panel2.Controls.Add(this.ServiceStatusLabel);
            this.splitContainer4.Panel2.Controls.Add(this.ServiceAutoStartLabel);
            this.splitContainer4.Panel2.Controls.Add(this.ServiceCheckIntervalLabel);
            this.splitContainer4.Panel2.Controls.Add(this.ServiceNameLabel);
            this.splitContainer4.Size = new System.Drawing.Size(562, 440);
            this.splitContainer4.SplitterDistance = 187;
            this.splitContainer4.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.ServiceStartButton, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ServiceStopButton, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.ServiceRestartButton, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 157F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(187, 440);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // ServiceStartButton
            // 
            this.ServiceStartButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ServiceStartButton.Location = new System.Drawing.Point(3, 3);
            this.ServiceStartButton.Name = "ServiceStartButton";
            this.ServiceStartButton.Size = new System.Drawing.Size(181, 135);
            this.ServiceStartButton.TabIndex = 0;
            this.ServiceStartButton.Text = "ЗАПУСК";
            this.ServiceStartButton.UseVisualStyleBackColor = true;
            this.ServiceStartButton.Click += new System.EventHandler(this.ServiceStartButton_Click);
            // 
            // ServiceStopButton
            // 
            this.ServiceStopButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ServiceStopButton.Location = new System.Drawing.Point(3, 144);
            this.ServiceStopButton.Name = "ServiceStopButton";
            this.ServiceStopButton.Size = new System.Drawing.Size(181, 135);
            this.ServiceStopButton.TabIndex = 1;
            this.ServiceStopButton.Text = "ОСТАНОВКА";
            this.ServiceStopButton.UseVisualStyleBackColor = true;
            this.ServiceStopButton.Click += new System.EventHandler(this.ServiceStopButton_Click);
            // 
            // ServiceRestartButton
            // 
            this.ServiceRestartButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ServiceRestartButton.Location = new System.Drawing.Point(3, 285);
            this.ServiceRestartButton.Name = "ServiceRestartButton";
            this.ServiceRestartButton.Size = new System.Drawing.Size(181, 152);
            this.ServiceRestartButton.TabIndex = 2;
            this.ServiceRestartButton.Text = "ПЕРЕЗАПУСК";
            this.ServiceRestartButton.UseVisualStyleBackColor = true;
            this.ServiceRestartButton.Click += new System.EventHandler(this.ServiceRestartButton_Click);
            // 
            // ServiceAutoStartCheckBox
            // 
            this.ServiceAutoStartCheckBox.AutoSize = true;
            this.ServiceAutoStartCheckBox.Location = new System.Drawing.Point(154, 84);
            this.ServiceAutoStartCheckBox.Name = "ServiceAutoStartCheckBox";
            this.ServiceAutoStartCheckBox.Size = new System.Drawing.Size(80, 17);
            this.ServiceAutoStartCheckBox.TabIndex = 1;
            this.ServiceAutoStartCheckBox.Text = "Off";
            this.ServiceAutoStartCheckBox.UseSelectable = true;
            this.ServiceAutoStartCheckBox.CheckedChanged += new System.EventHandler(this.ServiceAutoStartComboBox_CheckedChanged);
            // 
            // ServiceCheckIntervalTextBox
            // 
            // 
            // 
            // 
            this.ServiceCheckIntervalTextBox.CustomButton.Image = null;
            this.ServiceCheckIntervalTextBox.CustomButton.Location = new System.Drawing.Point(97, 1);
            this.ServiceCheckIntervalTextBox.CustomButton.Name = "";
            this.ServiceCheckIntervalTextBox.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.ServiceCheckIntervalTextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.ServiceCheckIntervalTextBox.CustomButton.TabIndex = 1;
            this.ServiceCheckIntervalTextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.ServiceCheckIntervalTextBox.CustomButton.UseSelectable = true;
            this.ServiceCheckIntervalTextBox.CustomButton.Visible = false;
            this.ServiceCheckIntervalTextBox.Lines = new string[0];
            this.ServiceCheckIntervalTextBox.Location = new System.Drawing.Point(154, 115);
            this.ServiceCheckIntervalTextBox.MaxLength = 32767;
            this.ServiceCheckIntervalTextBox.Name = "ServiceCheckIntervalTextBox";
            this.ServiceCheckIntervalTextBox.PasswordChar = '\0';
            this.ServiceCheckIntervalTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.ServiceCheckIntervalTextBox.SelectedText = "";
            this.ServiceCheckIntervalTextBox.SelectionLength = 0;
            this.ServiceCheckIntervalTextBox.SelectionStart = 0;
            this.ServiceCheckIntervalTextBox.ShortcutsEnabled = true;
            this.ServiceCheckIntervalTextBox.Size = new System.Drawing.Size(119, 23);
            this.ServiceCheckIntervalTextBox.TabIndex = 5;
            this.ServiceCheckIntervalTextBox.UseSelectable = true;
            this.ServiceCheckIntervalTextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.ServiceCheckIntervalTextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.ServiceCheckIntervalTextBox.TextChanged += new System.EventHandler(this.ServiceCheckIntervalTextBox_TextChanged);
            // 
            // ServiceStatusLabel
            // 
            this.ServiceStatusLabel.AutoSize = true;
            this.ServiceStatusLabel.Location = new System.Drawing.Point(49, 50);
            this.ServiceStatusLabel.Name = "ServiceStatusLabel";
            this.ServiceStatusLabel.Size = new System.Drawing.Size(99, 19);
            this.ServiceStatusLabel.TabIndex = 3;
            this.ServiceStatusLabel.Text = "Статус службы:";
            // 
            // ServiceAutoStartLabel
            // 
            this.ServiceAutoStartLabel.AutoSize = true;
            this.ServiceAutoStartLabel.Location = new System.Drawing.Point(23, 84);
            this.ServiceAutoStartLabel.Name = "ServiceAutoStartLabel";
            this.ServiceAutoStartLabel.Size = new System.Drawing.Size(123, 19);
            this.ServiceAutoStartLabel.TabIndex = 2;
            this.ServiceAutoStartLabel.Text = "Проверять службу:";
            // 
            // ServiceCheckIntervalLabel
            // 
            this.ServiceCheckIntervalLabel.AutoSize = true;
            this.ServiceCheckIntervalLabel.Location = new System.Drawing.Point(13, 119);
            this.ServiceCheckIntervalLabel.Name = "ServiceCheckIntervalLabel";
            this.ServiceCheckIntervalLabel.Size = new System.Drawing.Size(135, 19);
            this.ServiceCheckIntervalLabel.TabIndex = 1;
            this.ServiceCheckIntervalLabel.Text = "Интервал проверки:";
            // 
            // ServiceNameLabel
            // 
            this.ServiceNameLabel.AutoSize = true;
            this.ServiceNameLabel.Location = new System.Drawing.Point(61, 20);
            this.ServiceNameLabel.Name = "ServiceNameLabel";
            this.ServiceNameLabel.Size = new System.Drawing.Size(87, 19);
            this.ServiceNameLabel.TabIndex = 0;
            this.ServiceNameLabel.Text = "Имя службы:";
            // 
            // LogListBox
            // 
            this.LogListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogListBox.FormattingEnabled = true;
            this.LogListBox.Location = new System.Drawing.Point(0, 0);
            this.LogListBox.Name = "LogListBox";
            this.LogListBox.Size = new System.Drawing.Size(862, 131);
            this.LogListBox.TabIndex = 0;
            // 
            // tabPagePutty
            // 
            this.tabPagePutty.Location = new System.Drawing.Point(4, 22);
            this.tabPagePutty.Name = "tabPagePutty";
            this.tabPagePutty.Size = new System.Drawing.Size(854, 446);
            this.tabPagePutty.TabIndex = 1;
            this.tabPagePutty.Text = "Кассы";
            this.tabPagePutty.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1167, 687);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Main";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Main_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageServices.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            this.splitContainer4.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox ShopListBox;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListBox LogListBox;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageServices;
        private System.Windows.Forms.ListBox ServicesListBox;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button ServiceStartButton;
        private System.Windows.Forms.Button ServiceStopButton;
        private System.Windows.Forms.Button ServiceRestartButton;
        private MetroFramework.Controls.MetroLabel ServiceNameLabel;
        private MetroFramework.Controls.MetroLabel ServiceStatusLabel;
        private MetroFramework.Controls.MetroLabel ServiceAutoStartLabel;
        private MetroFramework.Controls.MetroLabel ServiceCheckIntervalLabel;
        private MetroFramework.Controls.MetroTextBox ServiceCheckIntervalTextBox;
        public MetroFramework.Controls.MetroToggle ServiceAutoStartCheckBox;
        private System.Windows.Forms.TabPage tabPagePutty;
    }
}

