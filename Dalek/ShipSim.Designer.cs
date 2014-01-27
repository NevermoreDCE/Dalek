namespace Dalek
{
    partial class ShipSim
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
            this.btnResetShips = new System.Windows.Forms.Button();
            this.btnFight = new System.Windows.Forms.Button();
            this.gbxShip1 = new System.Windows.Forms.GroupBox();
            this.tlpShip1 = new System.Windows.Forms.TableLayoutPanel();
            this.gbxShip2 = new System.Windows.Forms.GroupBox();
            this.tlpShip2 = new System.Windows.Forms.TableLayoutPanel();
            this.lbxResults = new System.Windows.Forms.ListBox();
            this.btnSelectShip2 = new System.Windows.Forms.Button();
            this.ofdShip1 = new System.Windows.Forms.OpenFileDialog();
            this.ofdShip2 = new System.Windows.Forms.OpenFileDialog();
            this.btnSelectShip1 = new System.Windows.Forms.Button();
            this.btnToTheDeath = new System.Windows.Forms.Button();
            this.btnTestShip2 = new System.Windows.Forms.Button();
            this.gbxShip1.SuspendLayout();
            this.gbxShip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnResetShips
            // 
            this.btnResetShips.Enabled = false;
            this.btnResetShips.Location = new System.Drawing.Point(12, 87);
            this.btnResetShips.Name = "btnResetShips";
            this.btnResetShips.Size = new System.Drawing.Size(177, 55);
            this.btnResetShips.TabIndex = 1;
            this.btnResetShips.Text = "Reset Ships";
            this.btnResetShips.UseVisualStyleBackColor = true;
            this.btnResetShips.Click += new System.EventHandler(this.btnResetShips_Click);
            // 
            // btnFight
            // 
            this.btnFight.Enabled = false;
            this.btnFight.Location = new System.Drawing.Point(12, 148);
            this.btnFight.Name = "btnFight";
            this.btnFight.Size = new System.Drawing.Size(177, 55);
            this.btnFight.TabIndex = 2;
            this.btnFight.Text = "Fight One Round";
            this.btnFight.UseVisualStyleBackColor = true;
            this.btnFight.Click += new System.EventHandler(this.btnFight_Click);
            // 
            // gbxShip1
            // 
            this.gbxShip1.Controls.Add(this.tlpShip1);
            this.gbxShip1.Location = new System.Drawing.Point(195, 15);
            this.gbxShip1.Name = "gbxShip1";
            this.gbxShip1.Size = new System.Drawing.Size(340, 359);
            this.gbxShip1.TabIndex = 3;
            this.gbxShip1.TabStop = false;
            this.gbxShip1.Text = "Ship 1";
            // 
            // tlpShip1
            // 
            this.tlpShip1.ColumnCount = 1;
            this.tlpShip1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpShip1.Location = new System.Drawing.Point(6, 19);
            this.tlpShip1.Name = "tlpShip1";
            this.tlpShip1.RowCount = 2;
            this.tlpShip1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpShip1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpShip1.Size = new System.Drawing.Size(323, 334);
            this.tlpShip1.TabIndex = 2;
            // 
            // gbxShip2
            // 
            this.gbxShip2.Controls.Add(this.tlpShip2);
            this.gbxShip2.Location = new System.Drawing.Point(541, 15);
            this.gbxShip2.Name = "gbxShip2";
            this.gbxShip2.Size = new System.Drawing.Size(340, 359);
            this.gbxShip2.TabIndex = 4;
            this.gbxShip2.TabStop = false;
            this.gbxShip2.Text = "Ship 2";
            // 
            // tlpShip2
            // 
            this.tlpShip2.ColumnCount = 1;
            this.tlpShip2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlpShip2.Location = new System.Drawing.Point(12, 19);
            this.tlpShip2.Name = "tlpShip2";
            this.tlpShip2.RowCount = 2;
            this.tlpShip2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpShip2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpShip2.Size = new System.Drawing.Size(317, 334);
            this.tlpShip2.TabIndex = 3;
            // 
            // lbxResults
            // 
            this.lbxResults.FormattingEnabled = true;
            this.lbxResults.Location = new System.Drawing.Point(12, 380);
            this.lbxResults.Name = "lbxResults";
            this.lbxResults.ScrollAlwaysVisible = true;
            this.lbxResults.Size = new System.Drawing.Size(869, 394);
            this.lbxResults.TabIndex = 5;
            // 
            // btnSelectShip2
            // 
            this.btnSelectShip2.Location = new System.Drawing.Point(12, 51);
            this.btnSelectShip2.Name = "btnSelectShip2";
            this.btnSelectShip2.Size = new System.Drawing.Size(177, 30);
            this.btnSelectShip2.TabIndex = 6;
            this.btnSelectShip2.Text = "Select Ship 2";
            this.btnSelectShip2.UseVisualStyleBackColor = true;
            this.btnSelectShip2.Click += new System.EventHandler(this.btnSelectShip2_Click);
            // 
            // ofdShip1
            // 
            this.ofdShip1.DefaultExt = "ship";
            this.ofdShip1.Filter = "ShipSim Files|*.ship";
            this.ofdShip1.FileOk += new System.ComponentModel.CancelEventHandler(this.ofdShip1_FileOk);
            // 
            // ofdShip2
            // 
            this.ofdShip2.DefaultExt = "ship";
            this.ofdShip2.Filter = "ShipSim Files|*.ship";
            this.ofdShip2.FileOk += new System.ComponentModel.CancelEventHandler(this.ofdShip2_FileOk);
            // 
            // btnSelectShip1
            // 
            this.btnSelectShip1.Location = new System.Drawing.Point(12, 15);
            this.btnSelectShip1.Name = "btnSelectShip1";
            this.btnSelectShip1.Size = new System.Drawing.Size(177, 30);
            this.btnSelectShip1.TabIndex = 7;
            this.btnSelectShip1.Text = "Select Ship 1";
            this.btnSelectShip1.UseVisualStyleBackColor = true;
            this.btnSelectShip1.Click += new System.EventHandler(this.btnSelectShip1_Click);
            // 
            // btnToTheDeath
            // 
            this.btnToTheDeath.Enabled = false;
            this.btnToTheDeath.Location = new System.Drawing.Point(12, 209);
            this.btnToTheDeath.Name = "btnToTheDeath";
            this.btnToTheDeath.Size = new System.Drawing.Size(177, 55);
            this.btnToTheDeath.TabIndex = 8;
            this.btnToTheDeath.Text = "Fight TO THE DEATH!";
            this.btnToTheDeath.UseVisualStyleBackColor = true;
            this.btnToTheDeath.Click += new System.EventHandler(this.btnToTheDeath_Click);
            // 
            // btnTestShip2
            // 
            this.btnTestShip2.Location = new System.Drawing.Point(12, 338);
            this.btnTestShip2.Name = "btnTestShip2";
            this.btnTestShip2.Size = new System.Drawing.Size(177, 30);
            this.btnTestShip2.TabIndex = 9;
            this.btnTestShip2.Text = "Test Ship 2";
            this.btnTestShip2.UseVisualStyleBackColor = true;
            this.btnTestShip2.Click += new System.EventHandler(this.btnTestShip2_Click);
            // 
            // ShipSim
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(891, 789);
            this.Controls.Add(this.btnTestShip2);
            this.Controls.Add(this.btnToTheDeath);
            this.Controls.Add(this.btnSelectShip1);
            this.Controls.Add(this.btnSelectShip2);
            this.Controls.Add(this.lbxResults);
            this.Controls.Add(this.gbxShip2);
            this.Controls.Add(this.gbxShip1);
            this.Controls.Add(this.btnFight);
            this.Controls.Add(this.btnResetShips);
            this.Name = "ShipSim";
            this.Text = "ShipSim";
            this.gbxShip1.ResumeLayout(false);
            this.gbxShip2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnResetShips;
        private System.Windows.Forms.Button btnFight;
        private System.Windows.Forms.GroupBox gbxShip1;
        private System.Windows.Forms.GroupBox gbxShip2;
        private System.Windows.Forms.ListBox lbxResults;
        private System.Windows.Forms.TableLayoutPanel tlpShip1;
        private System.Windows.Forms.TableLayoutPanel tlpShip2;
        private System.Windows.Forms.Button btnSelectShip2;
        private System.Windows.Forms.OpenFileDialog ofdShip1;
        private System.Windows.Forms.OpenFileDialog ofdShip2;
        private System.Windows.Forms.Button btnSelectShip1;
        private System.Windows.Forms.Button btnToTheDeath;
        private System.Windows.Forms.Button btnTestShip2;
    }
}

