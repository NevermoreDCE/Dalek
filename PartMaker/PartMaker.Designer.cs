namespace PartMaker
{
    partial class PartMaker
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
            this.tcPartTypes = new System.Windows.Forms.TabControl();
            this.tbpWeapon = new System.Windows.Forms.TabPage();
            this.nudReload = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.nudCritMultiplier = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.nudWeaponDamage = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.tbpDefense = new System.Windows.Forms.TabPage();
            this.tbxPenetrateVerb = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbxDownAdjective = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.nudDR = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.tbpAction = new System.Windows.Forms.TabPage();
            this.tbxActionDescr = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cbxExistingParts = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnLoadPart = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbxPartName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.nudHitpoints = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.nudPointCost = new System.Windows.Forms.NumericUpDown();
            this.btnSavePart = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.cbxActions = new System.Windows.Forms.ComboBox();
            this.nudActionValue = new System.Windows.Forms.NumericUpDown();
            this.btnActionAdd = new System.Windows.Forms.Button();
            this.drpActionList = new Microsoft.VisualBasic.PowerPacks.DataRepeater();
            this.btnActionRemove = new System.Windows.Forms.Button();
            this.lblActionTitle = new System.Windows.Forms.Label();
            this.tcPartTypes.SuspendLayout();
            this.tbpWeapon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudReload)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCritMultiplier)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWeaponDamage)).BeginInit();
            this.tbpDefense.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDR)).BeginInit();
            this.tbpAction.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHitpoints)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPointCost)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudActionValue)).BeginInit();
            this.drpActionList.ItemTemplate.SuspendLayout();
            this.drpActionList.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcPartTypes
            // 
            this.tcPartTypes.Controls.Add(this.tbpWeapon);
            this.tcPartTypes.Controls.Add(this.tbpDefense);
            this.tcPartTypes.Controls.Add(this.tbpAction);
            this.tcPartTypes.Location = new System.Drawing.Point(0, 83);
            this.tcPartTypes.Name = "tcPartTypes";
            this.tcPartTypes.SelectedIndex = 0;
            this.tcPartTypes.Size = new System.Drawing.Size(285, 113);
            this.tcPartTypes.TabIndex = 0;
            // 
            // tbpWeapon
            // 
            this.tbpWeapon.Controls.Add(this.nudReload);
            this.tbpWeapon.Controls.Add(this.label7);
            this.tbpWeapon.Controls.Add(this.nudCritMultiplier);
            this.tbpWeapon.Controls.Add(this.label6);
            this.tbpWeapon.Controls.Add(this.nudWeaponDamage);
            this.tbpWeapon.Controls.Add(this.label5);
            this.tbpWeapon.Location = new System.Drawing.Point(4, 22);
            this.tbpWeapon.Name = "tbpWeapon";
            this.tbpWeapon.Padding = new System.Windows.Forms.Padding(3);
            this.tbpWeapon.Size = new System.Drawing.Size(277, 87);
            this.tbpWeapon.TabIndex = 0;
            this.tbpWeapon.Text = "Weapon Part";
            this.tbpWeapon.UseVisualStyleBackColor = true;
            // 
            // nudReload
            // 
            this.nudReload.Location = new System.Drawing.Point(148, 58);
            this.nudReload.Name = "nudReload";
            this.nudReload.Size = new System.Drawing.Size(120, 20);
            this.nudReload.TabIndex = 5;
            this.nudReload.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 60);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Reload (Turns)";
            // 
            // nudCritMultiplier
            // 
            this.nudCritMultiplier.Location = new System.Drawing.Point(148, 32);
            this.nudCritMultiplier.Name = "nudCritMultiplier";
            this.nudCritMultiplier.Size = new System.Drawing.Size(120, 20);
            this.nudCritMultiplier.TabIndex = 3;
            this.nudCritMultiplier.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 34);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Crit Multiplier";
            // 
            // nudWeaponDamage
            // 
            this.nudWeaponDamage.Location = new System.Drawing.Point(148, 6);
            this.nudWeaponDamage.Name = "nudWeaponDamage";
            this.nudWeaponDamage.Size = new System.Drawing.Size(120, 20);
            this.nudWeaponDamage.TabIndex = 1;
            this.nudWeaponDamage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Weapon Damage";
            // 
            // tbpDefense
            // 
            this.tbpDefense.Controls.Add(this.tbxPenetrateVerb);
            this.tbpDefense.Controls.Add(this.label10);
            this.tbpDefense.Controls.Add(this.tbxDownAdjective);
            this.tbpDefense.Controls.Add(this.label9);
            this.tbpDefense.Controls.Add(this.nudDR);
            this.tbpDefense.Controls.Add(this.label8);
            this.tbpDefense.Location = new System.Drawing.Point(4, 22);
            this.tbpDefense.Name = "tbpDefense";
            this.tbpDefense.Padding = new System.Windows.Forms.Padding(3);
            this.tbpDefense.Size = new System.Drawing.Size(277, 87);
            this.tbpDefense.TabIndex = 1;
            this.tbpDefense.Text = "Defense Part";
            this.tbpDefense.UseVisualStyleBackColor = true;
            // 
            // tbxPenetrateVerb
            // 
            this.tbxPenetrateVerb.Location = new System.Drawing.Point(124, 57);
            this.tbxPenetrateVerb.Name = "tbxPenetrateVerb";
            this.tbxPenetrateVerb.Size = new System.Drawing.Size(147, 20);
            this.tbxPenetrateVerb.TabIndex = 7;
            this.tbxPenetrateVerb.Text = "Penetrating";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(11, 60);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(111, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "Verb for \"Penetrating\"";
            // 
            // tbxDownAdjective
            // 
            this.tbxDownAdjective.Location = new System.Drawing.Point(124, 31);
            this.tbxDownAdjective.Name = "tbxDownAdjective";
            this.tbxDownAdjective.Size = new System.Drawing.Size(147, 20);
            this.tbxDownAdjective.TabIndex = 5;
            this.tbxDownAdjective.Text = "Down";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(11, 34);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(107, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Adjective for \"Down\"";
            // 
            // nudDR
            // 
            this.nudDR.Location = new System.Drawing.Point(151, 6);
            this.nudDR.Name = "nudDR";
            this.nudDR.Size = new System.Drawing.Size(120, 20);
            this.nudDR.TabIndex = 3;
            this.nudDR.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 8);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(99, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Damage Reduction";
            // 
            // tbpAction
            // 
            this.tbpAction.Controls.Add(this.tbxActionDescr);
            this.tbpAction.Controls.Add(this.label11);
            this.tbpAction.Location = new System.Drawing.Point(4, 22);
            this.tbpAction.Name = "tbpAction";
            this.tbpAction.Size = new System.Drawing.Size(277, 87);
            this.tbpAction.TabIndex = 2;
            this.tbpAction.Text = "Action Part";
            this.tbpAction.UseVisualStyleBackColor = true;
            // 
            // tbxActionDescr
            // 
            this.tbxActionDescr.Location = new System.Drawing.Point(121, 3);
            this.tbxActionDescr.Name = "tbxActionDescr";
            this.tbxActionDescr.Size = new System.Drawing.Size(147, 20);
            this.tbxActionDescr.TabIndex = 7;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(8, 6);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(93, 13);
            this.label11.TabIndex = 6;
            this.label11.Text = "Action Description";
            // 
            // cbxExistingParts
            // 
            this.cbxExistingParts.FormattingEnabled = true;
            this.cbxExistingParts.Location = new System.Drawing.Point(77, 4);
            this.cbxExistingParts.Name = "cbxExistingParts";
            this.cbxExistingParts.Size = new System.Drawing.Size(159, 21);
            this.cbxExistingParts.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Load Existing";
            // 
            // btnLoadPart
            // 
            this.btnLoadPart.Location = new System.Drawing.Point(242, 4);
            this.btnLoadPart.Name = "btnLoadPart";
            this.btnLoadPart.Size = new System.Drawing.Size(39, 23);
            this.btnLoadPart.TabIndex = 3;
            this.btnLoadPart.Text = "Load";
            this.btnLoadPart.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Part Name";
            // 
            // tbxPartName
            // 
            this.tbxPartName.Location = new System.Drawing.Point(64, 31);
            this.tbxPartName.Name = "tbxPartName";
            this.tbxPartName.Size = new System.Drawing.Size(208, 20);
            this.tbxPartName.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Part HPs";
            // 
            // nudHitpoints
            // 
            this.nudHitpoints.Location = new System.Drawing.Point(56, 57);
            this.nudHitpoints.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudHitpoints.Name = "nudHitpoints";
            this.nudHitpoints.Size = new System.Drawing.Size(62, 20);
            this.nudHitpoints.TabIndex = 7;
            this.nudHitpoints.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudHitpoints.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(124, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Point Cost";
            // 
            // nudPointCost
            // 
            this.nudPointCost.Location = new System.Drawing.Point(185, 57);
            this.nudPointCost.Name = "nudPointCost";
            this.nudPointCost.Size = new System.Drawing.Size(87, 20);
            this.nudPointCost.TabIndex = 9;
            this.nudPointCost.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnSavePart
            // 
            this.btnSavePart.Location = new System.Drawing.Point(12, 371);
            this.btnSavePart.Name = "btnSavePart";
            this.btnSavePart.Size = new System.Drawing.Size(120, 28);
            this.btnSavePart.TabIndex = 10;
            this.btnSavePart.Text = "Save";
            this.btnSavePart.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(152, 371);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(120, 28);
            this.btnClear.TabIndex = 11;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.drpActionList);
            this.groupBox1.Controls.Add(this.btnActionAdd);
            this.groupBox1.Controls.Add(this.nudActionValue);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.cbxActions);
            this.groupBox1.Location = new System.Drawing.Point(4, 202);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(277, 163);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Actions";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(9, 24);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(37, 13);
            this.label12.TabIndex = 4;
            this.label12.Text = "Action";
            // 
            // cbxActions
            // 
            this.cbxActions.FormattingEnabled = true;
            this.cbxActions.Location = new System.Drawing.Point(52, 21);
            this.cbxActions.Name = "cbxActions";
            this.cbxActions.Size = new System.Drawing.Size(123, 21);
            this.cbxActions.TabIndex = 3;
            // 
            // nudActionValue
            // 
            this.nudActionValue.Location = new System.Drawing.Point(181, 21);
            this.nudActionValue.Name = "nudActionValue";
            this.nudActionValue.Size = new System.Drawing.Size(42, 20);
            this.nudActionValue.TabIndex = 5;
            this.nudActionValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnActionAdd
            // 
            this.btnActionAdd.Location = new System.Drawing.Point(229, 19);
            this.btnActionAdd.Name = "btnActionAdd";
            this.btnActionAdd.Size = new System.Drawing.Size(39, 23);
            this.btnActionAdd.TabIndex = 6;
            this.btnActionAdd.Text = "Add";
            this.btnActionAdd.UseVisualStyleBackColor = true;
            // 
            // drpActionList
            // 
            // 
            // drpActionList.ItemTemplate
            // 
            this.drpActionList.ItemTemplate.Controls.Add(this.lblActionTitle);
            this.drpActionList.ItemTemplate.Controls.Add(this.btnActionRemove);
            this.drpActionList.ItemTemplate.Size = new System.Drawing.Size(248, 30);
            this.drpActionList.Location = new System.Drawing.Point(12, 48);
            this.drpActionList.Name = "drpActionList";
            this.drpActionList.Size = new System.Drawing.Size(256, 109);
            this.drpActionList.TabIndex = 7;
            this.drpActionList.Text = "dataRepeater1";
            // 
            // btnActionRemove
            // 
            this.btnActionRemove.Location = new System.Drawing.Point(3, 3);
            this.btnActionRemove.Name = "btnActionRemove";
            this.btnActionRemove.Size = new System.Drawing.Size(21, 23);
            this.btnActionRemove.TabIndex = 0;
            this.btnActionRemove.Text = "X";
            this.btnActionRemove.UseVisualStyleBackColor = true;
            // 
            // lblActionTitle
            // 
            this.lblActionTitle.AutoSize = true;
            this.lblActionTitle.Location = new System.Drawing.Point(30, 8);
            this.lblActionTitle.Name = "lblActionTitle";
            this.lblActionTitle.Size = new System.Drawing.Size(41, 13);
            this.lblActionTitle.TabIndex = 1;
            this.lblActionTitle.Text = "label13";
            // 
            // PartMaker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 411);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnSavePart);
            this.Controls.Add(this.nudPointCost);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.nudHitpoints);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbxPartName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnLoadPart);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbxExistingParts);
            this.Controls.Add(this.tcPartTypes);
            this.Name = "PartMaker";
            this.Text = "PartMaker";
            this.tcPartTypes.ResumeLayout(false);
            this.tbpWeapon.ResumeLayout(false);
            this.tbpWeapon.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudReload)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCritMultiplier)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWeaponDamage)).EndInit();
            this.tbpDefense.ResumeLayout(false);
            this.tbpDefense.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDR)).EndInit();
            this.tbpAction.ResumeLayout(false);
            this.tbpAction.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHitpoints)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPointCost)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudActionValue)).EndInit();
            this.drpActionList.ItemTemplate.ResumeLayout(false);
            this.drpActionList.ItemTemplate.PerformLayout();
            this.drpActionList.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tcPartTypes;
        private System.Windows.Forms.TabPage tbpWeapon;
        private System.Windows.Forms.TabPage tbpDefense;
        private System.Windows.Forms.TabPage tbpAction;
        private System.Windows.Forms.ComboBox cbxExistingParts;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLoadPart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbxPartName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudHitpoints;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudPointCost;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudCritMultiplier;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudWeaponDamage;
        private System.Windows.Forms.NumericUpDown nudReload;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown nudDR;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbxPenetrateVerb;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbxDownAdjective;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbxActionDescr;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnSavePart;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown nudActionValue;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cbxActions;
        private Microsoft.VisualBasic.PowerPacks.DataRepeater drpActionList;
        private System.Windows.Forms.Label lblActionTitle;
        private System.Windows.Forms.Button btnActionRemove;
        private System.Windows.Forms.Button btnActionAdd;
    }
}

