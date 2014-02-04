namespace PathfindingTest
{
    partial class Form1
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
            this.grpField = new System.Windows.Forms.GroupBox();
            this.lblPathTaken = new System.Windows.Forms.Label();
            this.btnOneStep = new System.Windows.Forms.Button();
            this.btnNewMap = new System.Windows.Forms.Button();
            this.btnResetShips = new System.Windows.Forms.Button();
            this.btnAllSteps = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // grpField
            // 
            this.grpField.Location = new System.Drawing.Point(12, 12);
            this.grpField.Name = "grpField";
            this.grpField.Size = new System.Drawing.Size(650, 650);
            this.grpField.TabIndex = 0;
            this.grpField.TabStop = false;
            this.grpField.Text = "Starfield";
            // 
            // lblPathTaken
            // 
            this.lblPathTaken.AutoSize = true;
            this.lblPathTaken.Location = new System.Drawing.Point(665, 96);
            this.lblPathTaken.Name = "lblPathTaken";
            this.lblPathTaken.Size = new System.Drawing.Size(63, 13);
            this.lblPathTaken.TabIndex = 0;
            this.lblPathTaken.Text = "Path Taken";
            // 
            // btnOneStep
            // 
            this.btnOneStep.Location = new System.Drawing.Point(668, 41);
            this.btnOneStep.Name = "btnOneStep";
            this.btnOneStep.Size = new System.Drawing.Size(75, 23);
            this.btnOneStep.TabIndex = 1;
            this.btnOneStep.Text = "One Step";
            this.btnOneStep.UseVisualStyleBackColor = true;
            this.btnOneStep.Click += new System.EventHandler(this.btnAgain_Click);
            // 
            // btnNewMap
            // 
            this.btnNewMap.Location = new System.Drawing.Point(668, 12);
            this.btnNewMap.Name = "btnNewMap";
            this.btnNewMap.Size = new System.Drawing.Size(75, 23);
            this.btnNewMap.TabIndex = 2;
            this.btnNewMap.Text = "New Map";
            this.btnNewMap.UseVisualStyleBackColor = true;
            this.btnNewMap.Click += new System.EventHandler(this.btnNewMap_Click);
            // 
            // btnResetShips
            // 
            this.btnResetShips.Location = new System.Drawing.Point(758, 12);
            this.btnResetShips.Name = "btnResetShips";
            this.btnResetShips.Size = new System.Drawing.Size(75, 23);
            this.btnResetShips.TabIndex = 3;
            this.btnResetShips.Text = "ResetShips";
            this.btnResetShips.UseVisualStyleBackColor = true;
            this.btnResetShips.Click += new System.EventHandler(this.btnResetShips_Click);
            // 
            // btnAllSteps
            // 
            this.btnAllSteps.Location = new System.Drawing.Point(668, 70);
            this.btnAllSteps.Name = "btnAllSteps";
            this.btnAllSteps.Size = new System.Drawing.Size(75, 23);
            this.btnAllSteps.TabIndex = 5;
            this.btnAllSteps.Text = "All Steps";
            this.btnAllSteps.UseVisualStyleBackColor = true;
            this.btnAllSteps.Click += new System.EventHandler(this.btnAllSteps_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(856, 682);
            this.Controls.Add(this.btnAllSteps);
            this.Controls.Add(this.btnResetShips);
            this.Controls.Add(this.btnNewMap);
            this.Controls.Add(this.btnOneStep);
            this.Controls.Add(this.lblPathTaken);
            this.Controls.Add(this.grpField);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpField;
        private System.Windows.Forms.Label lblPathTaken;
        private System.Windows.Forms.Button btnOneStep;
        private System.Windows.Forms.Button btnNewMap;
        private System.Windows.Forms.Button btnResetShips;
        private System.Windows.Forms.Button btnAllSteps;
    }
}

