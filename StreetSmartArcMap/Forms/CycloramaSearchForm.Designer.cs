namespace StreetSmartArcMap.Forms
{
    partial class CycloramaSearchForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CycloramaSearchForm));
      this.btnClose = new System.Windows.Forms.Button();
      this.btnFind = new System.Windows.Forms.Button();
      this.txtImageId = new System.Windows.Forms.TextBox();
      this.lvResults = new System.Windows.Forms.ListView();
      this.chImageId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.chRecordedAt = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.backgroundImageSearch = new System.ComponentModel.BackgroundWorker();
      this.SuspendLayout();
      // 
      // btnClose
      // 
      resources.ApplyResources(this.btnClose, "btnClose");
      this.btnClose.Name = "btnClose";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnFind
      // 
      resources.ApplyResources(this.btnFind, "btnFind");
      this.btnFind.Name = "btnFind";
      this.btnFind.UseVisualStyleBackColor = true;
      this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
      // 
      // txtImageId
      // 
      resources.ApplyResources(this.txtImageId, "txtImageId");
      this.txtImageId.Name = "txtImageId";
      this.txtImageId.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtImageId_KeyDown);
      // 
      // lvResults
      // 
      resources.ApplyResources(this.lvResults, "lvResults");
      this.lvResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chImageId,
            this.chRecordedAt});
      this.lvResults.FullRowSelect = true;
      this.lvResults.HideSelection = false;
      this.lvResults.MultiSelect = false;
      this.lvResults.Name = "lvResults";
      this.lvResults.Scrollable = false;
      this.lvResults.UseCompatibleStateImageBehavior = false;
      this.lvResults.View = System.Windows.Forms.View.Details;
      this.lvResults.DoubleClick += new System.EventHandler(this.lvResults_DoubleClick);
      // 
      // chImageId
      // 
      resources.ApplyResources(this.chImageId, "chImageId");
      // 
      // chRecordedAt
      // 
      resources.ApplyResources(this.chRecordedAt, "chRecordedAt");
      // 
      // backgroundImageSearch
      // 
      this.backgroundImageSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundImageSearch_DoWork);
      this.backgroundImageSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundImageSearch_RunWorkerCompleted);
      // 
      // CycloramaSearchForm
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      resources.ApplyResources(this, "$this");
      this.Controls.Add(this.btnFind);
      this.Controls.Add(this.lvResults);
      this.Controls.Add(this.txtImageId);
      this.Controls.Add(this.btnClose);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.HelpButton = true;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "CycloramaSearchForm";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CycloramaSearchForm_FormClosing);
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CycloramaSearchForm_FormClosed);
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txtImageId;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.ListView lvResults;
        private System.Windows.Forms.ColumnHeader chImageId;
        private System.Windows.Forms.ColumnHeader chRecordedAt;
        private System.ComponentModel.BackgroundWorker backgroundImageSearch;
    }
}