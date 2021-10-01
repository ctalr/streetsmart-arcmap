namespace StreetSmartArcMap.Forms
{
    partial class StreetSmartConfigurationForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StreetSmartConfigurationForm));
      this.plButtons = new System.Windows.Forms.Panel();
      this.btnOk = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnApply = new System.Windows.Forms.Button();
      this.tcSettings = new System.Windows.Forms.TabControl();
      this.tbSettings = new System.Windows.Forms.TabPage();
      this.grLocalization = new System.Windows.Forms.GroupBox();
      this.lblCulture = new System.Windows.Forms.Label();
      this.cbCulture = new System.Windows.Forms.ComboBox();
      this.grLogin = new System.Windows.Forms.GroupBox();
      this.btnLogin = new System.Windows.Forms.Button();
      this.lblLogin = new System.Windows.Forms.Label();
      this.lblUsername = new System.Windows.Forms.Label();
      this.txtUsername = new System.Windows.Forms.TextBox();
      this.txtPassword = new System.Windows.Forms.TextBox();
      this.lblPassword = new System.Windows.Forms.Label();
      this.grGeneral = new System.Windows.Forms.GroupBox();
      this.lblOverlayDrawDistance = new System.Windows.Forms.Label();
      this.nudOverlayDrawDistance = new System.Windows.Forms.NumericUpDown();
      this.grCoordinateSystems = new System.Windows.Forms.GroupBox();
      this.label1 = new System.Windows.Forms.Label();
      this.cbRecordingsSRS = new System.Windows.Forms.ComboBox();
      this.lblCycloramaSRS = new System.Windows.Forms.Label();
      this.cbCycloramaSRS = new System.Windows.Forms.ComboBox();
      this.tbConfiguration = new System.Windows.Forms.TabPage();
      this.grConfiguration = new System.Windows.Forms.GroupBox();
      this.lblConfiguration = new System.Windows.Forms.Label();
      this.chUseDefaultStreetSmartLocation = new System.Windows.Forms.CheckBox();
      this.txtAPIStreetSmartLocation = new System.Windows.Forms.TextBox();
      this.grProxyServer = new System.Windows.Forms.GroupBox();
      this.lblProxyDomain = new System.Windows.Forms.Label();
      this.txtProxyDomain = new System.Windows.Forms.TextBox();
      this.lblProxyPassword = new System.Windows.Forms.Label();
      this.txtProxyPassword = new System.Windows.Forms.TextBox();
      this.lblProxyUserName = new System.Windows.Forms.Label();
      this.txtProxyUsername = new System.Windows.Forms.TextBox();
      this.ckUseDefaultProxyCredentials = new System.Windows.Forms.CheckBox();
      this.ckBypassProxyOnLocal = new System.Windows.Forms.CheckBox();
      this.lblProxyPort = new System.Windows.Forms.Label();
      this.txtProxyPort = new System.Windows.Forms.TextBox();
      this.lblProxyAddress = new System.Windows.Forms.Label();
      this.ckUseProxyServer = new System.Windows.Forms.CheckBox();
      this.txtProxyAddress = new System.Windows.Forms.TextBox();
      this.grAPI = new System.Windows.Forms.GroupBox();
      this.lblAPI = new System.Windows.Forms.Label();
      this.chUseDefaultConfigurationUrl = new System.Windows.Forms.CheckBox();
      this.txtAPIConfigurationUrl = new System.Windows.Forms.TextBox();
      this.tbAbout = new System.Windows.Forms.TabPage();
      this.rtbAbout = new System.Windows.Forms.RichTextBox();
      this.tbAgreement = new System.Windows.Forms.TabPage();
      this.txtAgreement = new System.Windows.Forms.TextBox();
      this.plButtons.SuspendLayout();
      this.tcSettings.SuspendLayout();
      this.tbSettings.SuspendLayout();
      this.grLocalization.SuspendLayout();
      this.grLogin.SuspendLayout();
      this.grGeneral.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.nudOverlayDrawDistance)).BeginInit();
      this.grCoordinateSystems.SuspendLayout();
      this.tbConfiguration.SuspendLayout();
      this.grConfiguration.SuspendLayout();
      this.grProxyServer.SuspendLayout();
      this.grAPI.SuspendLayout();
      this.tbAbout.SuspendLayout();
      this.tbAgreement.SuspendLayout();
      this.SuspendLayout();
      // 
      // plButtons
      // 
      this.plButtons.Controls.Add(this.btnOk);
      this.plButtons.Controls.Add(this.btnCancel);
      this.plButtons.Controls.Add(this.btnApply);
      resources.ApplyResources(this.plButtons, "plButtons");
      this.plButtons.Name = "plButtons";
      // 
      // btnOk
      // 
      resources.ApplyResources(this.btnOk, "btnOk");
      this.btnOk.Name = "btnOk";
      this.btnOk.UseVisualStyleBackColor = true;
      this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
      // 
      // btnCancel
      // 
      resources.ApplyResources(this.btnCancel, "btnCancel");
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // btnApply
      // 
      resources.ApplyResources(this.btnApply, "btnApply");
      this.btnApply.Name = "btnApply";
      this.btnApply.UseVisualStyleBackColor = true;
      this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
      // 
      // tcSettings
      // 
      resources.ApplyResources(this.tcSettings, "tcSettings");
      this.tcSettings.Controls.Add(this.tbSettings);
      this.tcSettings.Controls.Add(this.tbConfiguration);
      this.tcSettings.Controls.Add(this.tbAbout);
      this.tcSettings.Controls.Add(this.tbAgreement);
      this.tcSettings.Multiline = true;
      this.tcSettings.Name = "tcSettings";
      this.tcSettings.SelectedIndex = 0;
      // 
      // tbSettings
      // 
      this.tbSettings.Controls.Add(this.grLocalization);
      this.tbSettings.Controls.Add(this.grLogin);
      this.tbSettings.Controls.Add(this.grGeneral);
      this.tbSettings.Controls.Add(this.grCoordinateSystems);
      resources.ApplyResources(this.tbSettings, "tbSettings");
      this.tbSettings.Name = "tbSettings";
      this.tbSettings.UseVisualStyleBackColor = true;
      // 
      // grLocalization
      // 
      this.grLocalization.Controls.Add(this.lblCulture);
      this.grLocalization.Controls.Add(this.cbCulture);
      resources.ApplyResources(this.grLocalization, "grLocalization");
      this.grLocalization.Name = "grLocalization";
      this.grLocalization.TabStop = false;
      // 
      // lblCulture
      // 
      resources.ApplyResources(this.lblCulture, "lblCulture");
      this.lblCulture.Name = "lblCulture";
      // 
      // cbCulture
      // 
      resources.ApplyResources(this.cbCulture, "cbCulture");
      this.cbCulture.DisplayMember = "DisplayName";
      this.cbCulture.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbCulture.FormattingEnabled = true;
      this.cbCulture.Name = "cbCulture";
      this.cbCulture.Sorted = true;
      this.cbCulture.ValueMember = "Name";
      // 
      // grLogin
      // 
      this.grLogin.Controls.Add(this.btnLogin);
      this.grLogin.Controls.Add(this.lblLogin);
      this.grLogin.Controls.Add(this.lblUsername);
      this.grLogin.Controls.Add(this.txtUsername);
      this.grLogin.Controls.Add(this.txtPassword);
      this.grLogin.Controls.Add(this.lblPassword);
      resources.ApplyResources(this.grLogin, "grLogin");
      this.grLogin.Name = "grLogin";
      this.grLogin.TabStop = false;
      // 
      // btnLogin
      // 
      resources.ApplyResources(this.btnLogin, "btnLogin");
      this.btnLogin.Name = "btnLogin";
      this.btnLogin.UseVisualStyleBackColor = true;
      this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
      // 
      // lblLogin
      // 
      resources.ApplyResources(this.lblLogin, "lblLogin");
      this.lblLogin.Name = "lblLogin";
      // 
      // lblUsername
      // 
      resources.ApplyResources(this.lblUsername, "lblUsername");
      this.lblUsername.Name = "lblUsername";
      // 
      // txtUsername
      // 
      resources.ApplyResources(this.txtUsername, "txtUsername");
      this.txtUsername.Name = "txtUsername";
      this.txtUsername.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtUsername_KeyUp);
      // 
      // txtPassword
      // 
      resources.ApplyResources(this.txtPassword, "txtPassword");
      this.txtPassword.Name = "txtPassword";
      this.txtPassword.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPassword_KeyUp);
      // 
      // lblPassword
      // 
      resources.ApplyResources(this.lblPassword, "lblPassword");
      this.lblPassword.Name = "lblPassword";
      // 
      // grGeneral
      // 
      this.grGeneral.Controls.Add(this.lblOverlayDrawDistance);
      this.grGeneral.Controls.Add(this.nudOverlayDrawDistance);
      resources.ApplyResources(this.grGeneral, "grGeneral");
      this.grGeneral.Name = "grGeneral";
      this.grGeneral.TabStop = false;
      // 
      // lblOverlayDrawDistance
      // 
      resources.ApplyResources(this.lblOverlayDrawDistance, "lblOverlayDrawDistance");
      this.lblOverlayDrawDistance.Name = "lblOverlayDrawDistance";
      // 
      // nudOverlayDrawDistance
      // 
      resources.ApplyResources(this.nudOverlayDrawDistance, "nudOverlayDrawDistance");
      this.nudOverlayDrawDistance.Name = "nudOverlayDrawDistance";
      this.nudOverlayDrawDistance.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
      // 
      // grCoordinateSystems
      // 
      this.grCoordinateSystems.Controls.Add(this.label1);
      this.grCoordinateSystems.Controls.Add(this.cbRecordingsSRS);
      this.grCoordinateSystems.Controls.Add(this.lblCycloramaSRS);
      this.grCoordinateSystems.Controls.Add(this.cbCycloramaSRS);
      resources.ApplyResources(this.grCoordinateSystems, "grCoordinateSystems");
      this.grCoordinateSystems.Name = "grCoordinateSystems";
      this.grCoordinateSystems.TabStop = false;
      // 
      // label1
      // 
      resources.ApplyResources(this.label1, "label1");
      this.label1.Name = "label1";
      // 
      // cbRecordingsSRS
      // 
      resources.ApplyResources(this.cbRecordingsSRS, "cbRecordingsSRS");
      this.cbRecordingsSRS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbRecordingsSRS.FormattingEnabled = true;
      this.cbRecordingsSRS.Name = "cbRecordingsSRS";
      this.cbRecordingsSRS.Sorted = true;
      // 
      // lblCycloramaSRS
      // 
      resources.ApplyResources(this.lblCycloramaSRS, "lblCycloramaSRS");
      this.lblCycloramaSRS.Name = "lblCycloramaSRS";
      // 
      // cbCycloramaSRS
      // 
      resources.ApplyResources(this.cbCycloramaSRS, "cbCycloramaSRS");
      this.cbCycloramaSRS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbCycloramaSRS.FormattingEnabled = true;
      this.cbCycloramaSRS.Name = "cbCycloramaSRS";
      this.cbCycloramaSRS.Sorted = true;
      this.cbCycloramaSRS.SelectedValueChanged += new System.EventHandler(this.cbCycloramaSRS_SelectedValueChanged);
      // 
      // tbConfiguration
      // 
      this.tbConfiguration.Controls.Add(this.grConfiguration);
      this.tbConfiguration.Controls.Add(this.grProxyServer);
      this.tbConfiguration.Controls.Add(this.grAPI);
      resources.ApplyResources(this.tbConfiguration, "tbConfiguration");
      this.tbConfiguration.Name = "tbConfiguration";
      this.tbConfiguration.UseVisualStyleBackColor = true;
      // 
      // grConfiguration
      // 
      this.grConfiguration.Controls.Add(this.lblConfiguration);
      this.grConfiguration.Controls.Add(this.chUseDefaultStreetSmartLocation);
      this.grConfiguration.Controls.Add(this.txtAPIStreetSmartLocation);
      resources.ApplyResources(this.grConfiguration, "grConfiguration");
      this.grConfiguration.Name = "grConfiguration";
      this.grConfiguration.TabStop = false;
      // 
      // lblConfiguration
      // 
      resources.ApplyResources(this.lblConfiguration, "lblConfiguration");
      this.lblConfiguration.Name = "lblConfiguration";
      // 
      // chUseDefaultStreetSmartLocation
      // 
      resources.ApplyResources(this.chUseDefaultStreetSmartLocation, "chUseDefaultStreetSmartLocation");
      this.chUseDefaultStreetSmartLocation.Name = "chUseDefaultStreetSmartLocation";
      this.chUseDefaultStreetSmartLocation.UseVisualStyleBackColor = true;
      this.chUseDefaultStreetSmartLocation.CheckedChanged += new System.EventHandler(this.chUseDefaultStreetSmartLocation_CheckedChanged);
      // 
      // txtAPIStreetSmartLocation
      // 
      resources.ApplyResources(this.txtAPIStreetSmartLocation, "txtAPIStreetSmartLocation");
      this.txtAPIStreetSmartLocation.Name = "txtAPIStreetSmartLocation";
      // 
      // grProxyServer
      // 
      this.grProxyServer.Controls.Add(this.lblProxyDomain);
      this.grProxyServer.Controls.Add(this.txtProxyDomain);
      this.grProxyServer.Controls.Add(this.lblProxyPassword);
      this.grProxyServer.Controls.Add(this.txtProxyPassword);
      this.grProxyServer.Controls.Add(this.lblProxyUserName);
      this.grProxyServer.Controls.Add(this.txtProxyUsername);
      this.grProxyServer.Controls.Add(this.ckUseDefaultProxyCredentials);
      this.grProxyServer.Controls.Add(this.ckBypassProxyOnLocal);
      this.grProxyServer.Controls.Add(this.lblProxyPort);
      this.grProxyServer.Controls.Add(this.txtProxyPort);
      this.grProxyServer.Controls.Add(this.lblProxyAddress);
      this.grProxyServer.Controls.Add(this.ckUseProxyServer);
      this.grProxyServer.Controls.Add(this.txtProxyAddress);
      resources.ApplyResources(this.grProxyServer, "grProxyServer");
      this.grProxyServer.Name = "grProxyServer";
      this.grProxyServer.TabStop = false;
      // 
      // lblProxyDomain
      // 
      resources.ApplyResources(this.lblProxyDomain, "lblProxyDomain");
      this.lblProxyDomain.Name = "lblProxyDomain";
      // 
      // txtProxyDomain
      // 
      resources.ApplyResources(this.txtProxyDomain, "txtProxyDomain");
      this.txtProxyDomain.Name = "txtProxyDomain";
      // 
      // lblProxyPassword
      // 
      resources.ApplyResources(this.lblProxyPassword, "lblProxyPassword");
      this.lblProxyPassword.Name = "lblProxyPassword";
      // 
      // txtProxyPassword
      // 
      resources.ApplyResources(this.txtProxyPassword, "txtProxyPassword");
      this.txtProxyPassword.Name = "txtProxyPassword";
      // 
      // lblProxyUserName
      // 
      resources.ApplyResources(this.lblProxyUserName, "lblProxyUserName");
      this.lblProxyUserName.Name = "lblProxyUserName";
      // 
      // txtProxyUsername
      // 
      resources.ApplyResources(this.txtProxyUsername, "txtProxyUsername");
      this.txtProxyUsername.Name = "txtProxyUsername";
      // 
      // ckUseDefaultProxyCredentials
      // 
      resources.ApplyResources(this.ckUseDefaultProxyCredentials, "ckUseDefaultProxyCredentials");
      this.ckUseDefaultProxyCredentials.Name = "ckUseDefaultProxyCredentials";
      this.ckUseDefaultProxyCredentials.UseVisualStyleBackColor = true;
      // 
      // ckBypassProxyOnLocal
      // 
      resources.ApplyResources(this.ckBypassProxyOnLocal, "ckBypassProxyOnLocal");
      this.ckBypassProxyOnLocal.Name = "ckBypassProxyOnLocal";
      this.ckBypassProxyOnLocal.UseVisualStyleBackColor = true;
      // 
      // lblProxyPort
      // 
      resources.ApplyResources(this.lblProxyPort, "lblProxyPort");
      this.lblProxyPort.Name = "lblProxyPort";
      // 
      // txtProxyPort
      // 
      resources.ApplyResources(this.txtProxyPort, "txtProxyPort");
      this.txtProxyPort.Name = "txtProxyPort";
      // 
      // lblProxyAddress
      // 
      resources.ApplyResources(this.lblProxyAddress, "lblProxyAddress");
      this.lblProxyAddress.Name = "lblProxyAddress";
      // 
      // ckUseProxyServer
      // 
      resources.ApplyResources(this.ckUseProxyServer, "ckUseProxyServer");
      this.ckUseProxyServer.Name = "ckUseProxyServer";
      this.ckUseProxyServer.UseVisualStyleBackColor = true;
      // 
      // txtProxyAddress
      // 
      resources.ApplyResources(this.txtProxyAddress, "txtProxyAddress");
      this.txtProxyAddress.Name = "txtProxyAddress";
      // 
      // grAPI
      // 
      this.grAPI.Controls.Add(this.lblAPI);
      this.grAPI.Controls.Add(this.chUseDefaultConfigurationUrl);
      this.grAPI.Controls.Add(this.txtAPIConfigurationUrl);
      resources.ApplyResources(this.grAPI, "grAPI");
      this.grAPI.Name = "grAPI";
      this.grAPI.TabStop = false;
      // 
      // lblAPI
      // 
      resources.ApplyResources(this.lblAPI, "lblAPI");
      this.lblAPI.Name = "lblAPI";
      // 
      // chUseDefaultConfigurationUrl
      // 
      resources.ApplyResources(this.chUseDefaultConfigurationUrl, "chUseDefaultConfigurationUrl");
      this.chUseDefaultConfigurationUrl.Name = "chUseDefaultConfigurationUrl";
      this.chUseDefaultConfigurationUrl.UseVisualStyleBackColor = true;
      this.chUseDefaultConfigurationUrl.CheckedChanged += new System.EventHandler(this.chUseDefaultConfigurationUrl_CheckedChanged);
      // 
      // txtAPIConfigurationUrl
      // 
      resources.ApplyResources(this.txtAPIConfigurationUrl, "txtAPIConfigurationUrl");
      this.txtAPIConfigurationUrl.Name = "txtAPIConfigurationUrl";
      // 
      // tbAbout
      // 
      this.tbAbout.Controls.Add(this.rtbAbout);
      resources.ApplyResources(this.tbAbout, "tbAbout");
      this.tbAbout.Name = "tbAbout";
      this.tbAbout.UseVisualStyleBackColor = true;
      // 
      // rtbAbout
      // 
      this.rtbAbout.BackColor = System.Drawing.SystemColors.Window;
      this.rtbAbout.BorderStyle = System.Windows.Forms.BorderStyle.None;
      resources.ApplyResources(this.rtbAbout, "rtbAbout");
      this.rtbAbout.Name = "rtbAbout";
      this.rtbAbout.ReadOnly = true;
      this.rtbAbout.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbAbout_LinkClicked);
      // 
      // tbAgreement
      // 
      this.tbAgreement.Controls.Add(this.txtAgreement);
      resources.ApplyResources(this.tbAgreement, "tbAgreement");
      this.tbAgreement.Name = "tbAgreement";
      this.tbAgreement.UseVisualStyleBackColor = true;
      // 
      // txtAgreement
      // 
      resources.ApplyResources(this.txtAgreement, "txtAgreement");
      this.txtAgreement.Name = "txtAgreement";
      this.txtAgreement.ReadOnly = true;
      // 
      // StreetSmartConfigurationForm
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      resources.ApplyResources(this, "$this");
      this.Controls.Add(this.tcSettings);
      this.Controls.Add(this.plButtons);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.HelpButton = true;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "StreetSmartConfigurationForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.StreetSmartConfigurationForm_FormClosed);
      this.plButtons.ResumeLayout(false);
      this.tcSettings.ResumeLayout(false);
      this.tbSettings.ResumeLayout(false);
      this.grLocalization.ResumeLayout(false);
      this.grLocalization.PerformLayout();
      this.grLogin.ResumeLayout(false);
      this.grLogin.PerformLayout();
      this.grGeneral.ResumeLayout(false);
      this.grGeneral.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.nudOverlayDrawDistance)).EndInit();
      this.grCoordinateSystems.ResumeLayout(false);
      this.grCoordinateSystems.PerformLayout();
      this.tbConfiguration.ResumeLayout(false);
      this.grConfiguration.ResumeLayout(false);
      this.grConfiguration.PerformLayout();
      this.grProxyServer.ResumeLayout(false);
      this.grProxyServer.PerformLayout();
      this.grAPI.ResumeLayout(false);
      this.grAPI.PerformLayout();
      this.tbAbout.ResumeLayout(false);
      this.tbAgreement.ResumeLayout(false);
      this.tbAgreement.PerformLayout();
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel plButtons;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.TabControl tcSettings;
        private System.Windows.Forms.TabPage tbSettings;
        private System.Windows.Forms.GroupBox grCoordinateSystems;
        private System.Windows.Forms.Label lblCycloramaSRS;
        private System.Windows.Forms.ComboBox cbCycloramaSRS;
        private System.Windows.Forms.TabPage tbConfiguration;
        private System.Windows.Forms.GroupBox grProxyServer;
        private System.Windows.Forms.Label lblProxyDomain;
        private System.Windows.Forms.TextBox txtProxyDomain;
        private System.Windows.Forms.Label lblProxyPassword;
        private System.Windows.Forms.TextBox txtProxyPassword;
        private System.Windows.Forms.Label lblProxyUserName;
        private System.Windows.Forms.TextBox txtProxyUsername;
        private System.Windows.Forms.CheckBox ckUseDefaultProxyCredentials;
        private System.Windows.Forms.CheckBox ckBypassProxyOnLocal;
        private System.Windows.Forms.Label lblProxyPort;
        private System.Windows.Forms.TextBox txtProxyPort;
        private System.Windows.Forms.Label lblProxyAddress;
        private System.Windows.Forms.CheckBox ckUseProxyServer;
        private System.Windows.Forms.TextBox txtProxyAddress;
        private System.Windows.Forms.GroupBox grAPI;
        private System.Windows.Forms.Label lblAPI;
        private System.Windows.Forms.CheckBox chUseDefaultConfigurationUrl;
        private System.Windows.Forms.TextBox txtAPIConfigurationUrl;
        private System.Windows.Forms.TabPage tbAbout;
        private System.Windows.Forms.RichTextBox rtbAbout;
        private System.Windows.Forms.TabPage tbAgreement;
        private System.Windows.Forms.TextBox txtAgreement;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbRecordingsSRS;
        private System.Windows.Forms.GroupBox grGeneral;
        private System.Windows.Forms.Label lblOverlayDrawDistance;
        private System.Windows.Forms.NumericUpDown nudOverlayDrawDistance;
        private System.Windows.Forms.GroupBox grLogin;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label lblLogin;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.GroupBox grConfiguration;
        private System.Windows.Forms.Label lblConfiguration;
        private System.Windows.Forms.CheckBox chUseDefaultStreetSmartLocation;
        private System.Windows.Forms.TextBox txtAPIStreetSmartLocation;
        private System.Windows.Forms.GroupBox grLocalization;
        private System.Windows.Forms.Label lblCulture;
        private System.Windows.Forms.ComboBox cbCulture;
    private System.Windows.Forms.TextBox txtUsername;
  }
}