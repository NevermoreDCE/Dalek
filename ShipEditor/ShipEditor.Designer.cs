namespace ShipEditor
{
    partial class ShipEditor
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
            this.ofdOpen = new System.Windows.Forms.OpenFileDialog();
            this.sfdSave = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.tbxShipName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nudHitpoints = new System.Windows.Forms.NumericUpDown();
            this.ItemTemplate = new Microsoft.VisualBasic.PowerPacks.DataRepeaterItem();
            this.btnDeletePart = new System.Windows.Forms.Button();
            this.lblPartName = new System.Windows.Forms.Label();
            this.drpPartList = new Microsoft.VisualBasic.PowerPacks.DataRepeater();
            this.cbxPartList = new System.Windows.Forms.ComboBox();
            this.btnAddPart = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHitpoints)).BeginInit();
            this.drpPartList.ItemTemplate.SuspendLayout();
            this.drpPartList.SuspendLayout();
            this.SuspendLayout();
            // 
            // ofdOpen
            // 
            this.ofdOpen.DefaultExt = "ship";
            this.ofdOpen.Filter = "ShipSim Files|*.ship";
            this.ofdOpen.FileOk += new System.ComponentModel.CancelEventHandler(this.ofdOpen_FileOk);
            // 
            // sfdSave
            // 
            this.sfdSave.DefaultExt = "ship";
            this.sfdSave.Filter = "ShipSim Files|*.ship";
            this.sfdSave.FileOk += new System.ComponentModel.CancelEventHandler(this.sfdSave_FileOk);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(653, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.newToolStripMenuItem.Text = "&New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(152, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Ship Name";
            // 
            // tbxShipName
            // 
            this.tbxShipName.Location = new System.Drawing.Point(103, 34);
            this.tbxShipName.Name = "tbxShipName";
            this.tbxShipName.Size = new System.Drawing.Size(232, 20);
            this.tbxShipName.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Max Hitpoints";
            // 
            // nudHitpoints
            // 
            this.nudHitpoints.Location = new System.Drawing.Point(103, 60);
            this.nudHitpoints.Name = "nudHitpoints";
            this.nudHitpoints.Size = new System.Drawing.Size(55, 20);
            this.nudHitpoints.TabIndex = 4;
            this.nudHitpoints.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudHitpoints.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // ItemTemplate
            // 
            this.ItemTemplate.Size = new System.Drawing.Size(232, 29);
            // 
            // btnDeletePart
            // 
            this.btnDeletePart.Location = new System.Drawing.Point(3, 3);
            this.btnDeletePart.Name = "btnDeletePart";
            this.btnDeletePart.Size = new System.Drawing.Size(22, 23);
            this.btnDeletePart.TabIndex = 0;
            this.btnDeletePart.Text = "X";
            this.btnDeletePart.UseVisualStyleBackColor = true;
            this.btnDeletePart.Click += new System.EventHandler(this.btnDeletePart_Click);
            // 
            // lblPartName
            // 
            this.lblPartName.AutoSize = true;
            this.lblPartName.Location = new System.Drawing.Point(31, 8);
            this.lblPartName.Name = "lblPartName";
            this.lblPartName.Size = new System.Drawing.Size(35, 13);
            this.lblPartName.TabIndex = 1;
            this.lblPartName.Text = "label3";
            // 
            // drpPartList
            // 
            // 
            // drpPartList.ItemTemplate
            // 
            this.drpPartList.ItemTemplate.Controls.Add(this.lblPartName);
            this.drpPartList.ItemTemplate.Controls.Add(this.btnDeletePart);
            this.drpPartList.ItemTemplate.Size = new System.Drawing.Size(394, 29);
            this.drpPartList.Location = new System.Drawing.Point(15, 132);
            this.drpPartList.Name = "drpPartList";
            this.drpPartList.Size = new System.Drawing.Size(402, 379);
            this.drpPartList.TabIndex = 5;
            this.drpPartList.Text = "dataRepeater1";
            this.drpPartList.VirtualMode = true;
            // 
            // cbxPartList
            // 
            this.cbxPartList.FormattingEnabled = true;
            this.cbxPartList.Location = new System.Drawing.Point(19, 95);
            this.cbxPartList.Name = "cbxPartList";
            this.cbxPartList.Size = new System.Drawing.Size(316, 21);
            this.cbxPartList.TabIndex = 6;
            // 
            // btnAddPart
            // 
            this.btnAddPart.Location = new System.Drawing.Point(342, 93);
            this.btnAddPart.Name = "btnAddPart";
            this.btnAddPart.Size = new System.Drawing.Size(75, 23);
            this.btnAddPart.TabIndex = 7;
            this.btnAddPart.Text = "Add Part";
            this.btnAddPart.UseVisualStyleBackColor = true;
            this.btnAddPart.Click += new System.EventHandler(this.btnAddPart_Click);
            // 
            // ShipEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(653, 546);
            this.Controls.Add(this.btnAddPart);
            this.Controls.Add(this.cbxPartList);
            this.Controls.Add(this.drpPartList);
            this.Controls.Add(this.nudHitpoints);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbxShipName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ShipEditor";
            this.Text = "ShipEditor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHitpoints)).EndInit();
            this.drpPartList.ItemTemplate.ResumeLayout(false);
            this.drpPartList.ItemTemplate.PerformLayout();
            this.drpPartList.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog ofdOpen;
        private System.Windows.Forms.SaveFileDialog sfdSave;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbxShipName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudHitpoints;
        private Microsoft.VisualBasic.PowerPacks.DataRepeaterItem ItemTemplate;
        private System.Windows.Forms.Button btnDeletePart;
        private System.Windows.Forms.Label lblPartName;
        private Microsoft.VisualBasic.PowerPacks.DataRepeater drpPartList;
        private System.Windows.Forms.ComboBox cbxPartList;
        private System.Windows.Forms.Button btnAddPart;
    }
}

