namespace VulScan
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblMessage = new System.Windows.Forms.Label();
            this.lblSoftwareList = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.BtnSearch = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 60);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(853, 23);
            this.progressBar.TabIndex = 0;
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(12, 102);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(60, 13);
            this.lblMessage.TabIndex = 1;
            this.lblMessage.Text = "lblMessage";
            // 
            // lblSoftwareList
            // 
            this.lblSoftwareList.AutoSize = true;
            this.lblSoftwareList.Location = new System.Drawing.Point(12, 44);
            this.lblSoftwareList.Name = "lblSoftwareList";
            this.lblSoftwareList.Size = new System.Drawing.Size(75, 13);
            this.lblSoftwareList.TabIndex = 2;
            this.lblSoftwareList.Text = "lblSoftwareList";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(401, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "VulScan";
            // 
            // BtnSearch
            // 
            this.BtnSearch.Location = new System.Drawing.Point(790, 97);
            this.BtnSearch.Name = "BtnSearch";
            this.BtnSearch.Size = new System.Drawing.Size(75, 23);
            this.BtnSearch.TabIndex = 4;
            this.BtnSearch.Text = "Search";
            this.BtnSearch.UseVisualStyleBackColor = true;
            this.BtnSearch.Click += new System.EventHandler(this.BtnSearch_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(877, 124);
            this.Controls.Add(this.BtnSearch);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblSoftwareList);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.progressBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "VulScan";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label lblSoftwareList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtnSearch;
    }
}

