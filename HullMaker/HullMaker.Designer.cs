namespace HullMaker
{
    partial class HullMaker
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
            this.btnLoadHull = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbxExistingHulls = new System.Windows.Forms.ComboBox();
            this.tbxHullName = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nudHullPointsMax = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.drpPartLimits = new Microsoft.VisualBasic.PowerPacks.DataRepeater();
            this.lblPartLimitTitle = new System.Windows.Forms.Label();
            this.btnPartLimitRemove = new System.Windows.Forms.Button();
            this.btnAddPartLimit = new System.Windows.Forms.Button();
            this.nudMaxPartCount = new System.Windows.Forms.NumericUpDown();
            this.cbxPartDetail = new System.Windows.Forms.ComboBox();
            this.cbxPartType = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSaveHull = new System.Windows.Forms.Button();
            this.tbxImage = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudHullPointsMax)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.drpPartLimits.ItemTemplate.SuspendLayout();
            this.drpPartLimits.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxPartCount)).BeginInit();
            this.SuspendLayout();
            // 
            // btnLoadHull
            // 
            this.btnLoadHull.Location = new System.Drawing.Point(355, 4);
            this.btnLoadHull.Name = "btnLoadHull";
            this.btnLoadHull.Size = new System.Drawing.Size(39, 23);
            this.btnLoadHull.TabIndex = 6;
            this.btnLoadHull.Text = "Load";
            this.btnLoadHull.UseVisualStyleBackColor = true;
            this.btnLoadHull.Click += new System.EventHandler(this.btnLoadHull_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Load Existing";
            // 
            // cbxExistingHulls
            // 
            this.cbxExistingHulls.FormattingEnabled = true;
            this.cbxExistingHulls.Location = new System.Drawing.Point(88, 6);
            this.cbxExistingHulls.Name = "cbxExistingHulls";
            this.cbxExistingHulls.Size = new System.Drawing.Size(261, 21);
            this.cbxExistingHulls.TabIndex = 4;
            // 
            // tbxHullName
            // 
            this.tbxHullName.Location = new System.Drawing.Point(88, 33);
            this.tbxHullName.Name = "tbxHullName";
            this.tbxHullName.Size = new System.Drawing.Size(200, 20);
            this.tbxHullName.TabIndex = 19;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(12, 36);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(56, 13);
            this.label22.TabIndex = 18;
            this.label22.Text = "Hull Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Max Hullpoints";
            // 
            // nudHullPointsMax
            // 
            this.nudHullPointsMax.Location = new System.Drawing.Point(88, 59);
            this.nudHullPointsMax.Name = "nudHullPointsMax";
            this.nudHullPointsMax.Size = new System.Drawing.Size(91, 20);
            this.nudHullPointsMax.TabIndex = 21;
            this.nudHullPointsMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.drpPartLimits);
            this.groupBox1.Controls.Add(this.btnAddPartLimit);
            this.groupBox1.Controls.Add(this.nudMaxPartCount);
            this.groupBox1.Controls.Add(this.cbxPartDetail);
            this.groupBox1.Controls.Add(this.cbxPartType);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(15, 115);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(379, 337);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Part Limits";
            // 
            // drpPartLimits
            // 
            // 
            // drpPartLimits.ItemTemplate
            // 
            this.drpPartLimits.ItemTemplate.Controls.Add(this.lblPartLimitTitle);
            this.drpPartLimits.ItemTemplate.Controls.Add(this.btnPartLimitRemove);
            this.drpPartLimits.ItemTemplate.Size = new System.Drawing.Size(354, 23);
            this.drpPartLimits.Location = new System.Drawing.Point(9, 73);
            this.drpPartLimits.Name = "drpPartLimits";
            this.drpPartLimits.Size = new System.Drawing.Size(362, 254);
            this.drpPartLimits.TabIndex = 7;
            this.drpPartLimits.Text = "dataRepeater1";
            this.drpPartLimits.VirtualMode = true;
            // 
            // lblPartLimitTitle
            // 
            this.lblPartLimitTitle.AutoSize = true;
            this.lblPartLimitTitle.Location = new System.Drawing.Point(30, 5);
            this.lblPartLimitTitle.Name = "lblPartLimitTitle";
            this.lblPartLimitTitle.Size = new System.Drawing.Size(41, 13);
            this.lblPartLimitTitle.TabIndex = 3;
            this.lblPartLimitTitle.Text = "label13";
            // 
            // btnPartLimitRemove
            // 
            this.btnPartLimitRemove.Location = new System.Drawing.Point(3, 0);
            this.btnPartLimitRemove.Name = "btnPartLimitRemove";
            this.btnPartLimitRemove.Size = new System.Drawing.Size(21, 23);
            this.btnPartLimitRemove.TabIndex = 2;
            this.btnPartLimitRemove.Text = "X";
            this.btnPartLimitRemove.UseVisualStyleBackColor = true;
            // 
            // btnAddPartLimit
            // 
            this.btnAddPartLimit.Location = new System.Drawing.Point(250, 44);
            this.btnAddPartLimit.Name = "btnAddPartLimit";
            this.btnAddPartLimit.Size = new System.Drawing.Size(121, 23);
            this.btnAddPartLimit.TabIndex = 6;
            this.btnAddPartLimit.Text = "Add";
            this.btnAddPartLimit.UseVisualStyleBackColor = true;
            this.btnAddPartLimit.Click += new System.EventHandler(this.btnAddPartLimit_Click);
            // 
            // nudMaxPartCount
            // 
            this.nudMaxPartCount.Location = new System.Drawing.Point(251, 20);
            this.nudMaxPartCount.Name = "nudMaxPartCount";
            this.nudMaxPartCount.Size = new System.Drawing.Size(120, 20);
            this.nudMaxPartCount.TabIndex = 5;
            this.nudMaxPartCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // cbxPartDetail
            // 
            this.cbxPartDetail.FormattingEnabled = true;
            this.cbxPartDetail.Location = new System.Drawing.Point(65, 46);
            this.cbxPartDetail.Name = "cbxPartDetail";
            this.cbxPartDetail.Size = new System.Drawing.Size(179, 21);
            this.cbxPartDetail.TabIndex = 4;
            // 
            // cbxPartType
            // 
            this.cbxPartType.FormattingEnabled = true;
            this.cbxPartType.Location = new System.Drawing.Point(65, 19);
            this.cbxPartType.Name = "cbxPartType";
            this.cbxPartType.Size = new System.Drawing.Size(121, 21);
            this.cbxPartType.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(192, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Max Count";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Detail";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Part Type";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(274, 458);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(120, 28);
            this.btnClear.TabIndex = 24;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSaveHull
            // 
            this.btnSaveHull.Location = new System.Drawing.Point(12, 458);
            this.btnSaveHull.Name = "btnSaveHull";
            this.btnSaveHull.Size = new System.Drawing.Size(120, 28);
            this.btnSaveHull.TabIndex = 23;
            this.btnSaveHull.Text = "Save";
            this.btnSaveHull.UseVisualStyleBackColor = true;
            this.btnSaveHull.Click += new System.EventHandler(this.btnSaveHull_Click);
            // 
            // tbxImage
            // 
            this.tbxImage.Location = new System.Drawing.Point(88, 85);
            this.tbxImage.Name = "tbxImage";
            this.tbxImage.Size = new System.Drawing.Size(200, 20);
            this.tbxImage.TabIndex = 26;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 88);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 25;
            this.label6.Text = "Image Name";
            // 
            // HullMaker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 495);
            this.Controls.Add(this.tbxImage);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnSaveHull);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.nudHullPointsMax);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbxHullName);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.btnLoadHull);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbxExistingHulls);
            this.Name = "HullMaker";
            this.Text = "HullMaker";
            ((System.ComponentModel.ISupportInitialize)(this.nudHullPointsMax)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.drpPartLimits.ItemTemplate.ResumeLayout(false);
            this.drpPartLimits.ItemTemplate.PerformLayout();
            this.drpPartLimits.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxPartCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLoadHull;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbxExistingHulls;
        private System.Windows.Forms.TextBox tbxHullName;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudHullPointsMax;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnAddPartLimit;
        private System.Windows.Forms.NumericUpDown nudMaxPartCount;
        private System.Windows.Forms.ComboBox cbxPartDetail;
        private System.Windows.Forms.ComboBox cbxPartType;
        private System.Windows.Forms.Label label5;
        private Microsoft.VisualBasic.PowerPacks.DataRepeater drpPartLimits;
        private System.Windows.Forms.Label lblPartLimitTitle;
        private System.Windows.Forms.Button btnPartLimitRemove;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSaveHull;
        private System.Windows.Forms.TextBox tbxImage;
        private System.Windows.Forms.Label label6;
    }
}

