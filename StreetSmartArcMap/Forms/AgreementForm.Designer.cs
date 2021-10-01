namespace StreetSmartArcMap.Forms
{
    partial class AgreementForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AgreementForm));
      this.txtAgreement = new System.Windows.Forms.TextBox();
      this.ckAgreement = new System.Windows.Forms.CheckBox();
      this.btnClose = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // txtAgreement
      // 
      resources.ApplyResources(this.txtAgreement, "txtAgreement");
      this.txtAgreement.Name = "txtAgreement";
      this.txtAgreement.ReadOnly = true;
      // 
      // ckAgreement
      // 
      resources.ApplyResources(this.ckAgreement, "ckAgreement");
      this.ckAgreement.Name = "ckAgreement";
      this.ckAgreement.UseVisualStyleBackColor = true;
      // 
      // btnClose
      // 
      resources.ApplyResources(this.btnClose, "btnClose");
      this.btnClose.Name = "btnClose";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // AgreementForm
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      resources.ApplyResources(this, "$this");
      this.ControlBox = false;
      this.Controls.Add(this.btnClose);
      this.Controls.Add(this.ckAgreement);
      this.Controls.Add(this.txtAgreement);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AgreementForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Load += new System.EventHandler(this.AgreementForm_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtAgreement;
        private System.Windows.Forms.CheckBox ckAgreement;
        private System.Windows.Forms.Button btnClose;
    }
}