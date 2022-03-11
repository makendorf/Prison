namespace PL
{
    partial class PuttyForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.StartSessionButton = new MetroFramework.Controls.MetroButton();
            this.ShopBoxTabControl = new System.Windows.Forms.TabControl();
            this.IPLabel = new MetroFramework.Controls.MetroLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.ShopBoxTabControl);
            this.splitContainer1.Size = new System.Drawing.Size(800, 450);
            this.splitContainer1.SplitterDistance = 126;
            this.splitContainer1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.StartSessionButton, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.IPLabel, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 77.37226F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22.62774F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 303F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(126, 450);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // StartSessionButton
            // 
            this.StartSessionButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.StartSessionButton.Location = new System.Drawing.Point(17, 34);
            this.StartSessionButton.Name = "StartSessionButton";
            this.StartSessionButton.Size = new System.Drawing.Size(91, 23);
            this.StartSessionButton.TabIndex = 0;
            this.StartSessionButton.Text = "Начать сессию";
            this.StartSessionButton.UseSelectable = true;
            this.StartSessionButton.Click += new System.EventHandler(this.StartSessionButton1_Click);
            // 
            // ShopBoxTabControl
            // 
            this.ShopBoxTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ShopBoxTabControl.Location = new System.Drawing.Point(0, 0);
            this.ShopBoxTabControl.Name = "ShopBoxTabControl";
            this.ShopBoxTabControl.SelectedIndex = 0;
            this.ShopBoxTabControl.Size = new System.Drawing.Size(670, 450);
            this.ShopBoxTabControl.TabIndex = 0;
            this.ShopBoxTabControl.SelectedIndexChanged += new System.EventHandler(this.ShopBoxTabControl_SelectedIndexChanged);
            // 
            // IPLabel
            // 
            this.IPLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.IPLabel.AutoSize = true;
            this.IPLabel.Location = new System.Drawing.Point(22, 91);
            this.IPLabel.Name = "IPLabel";
            this.IPLabel.Size = new System.Drawing.Size(81, 19);
            this.IPLabel.TabIndex = 1;
            this.IPLabel.Text = "metroLabel1";
            // 
            // PuttyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Name = "PuttyForm";
            this.Text = "PuttyForm";
            this.Load += new System.EventHandler(this.PuttyForm_Load);
            this.Resize += new System.EventHandler(this.PuttyForm_Resize);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        public System.Windows.Forms.TabControl ShopBoxTabControl;
        private MetroFramework.Controls.MetroButton StartSessionButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private MetroFramework.Controls.MetroLabel IPLabel;
    }
}