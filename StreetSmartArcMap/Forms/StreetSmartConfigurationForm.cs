/*
 * Integration in ArcMap for StreetSmart
 * Copyright (c) 2019, CycloMedia, All rights reserved.
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3.0 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library.
 */

using IntegrationArcMap.Utilities;
using StreetSmartArcMap.Client;
using StreetSmartArcMap.Logic;
using StreetSmartArcMap.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

namespace StreetSmartArcMap.Forms
{
    public partial class StreetSmartConfigurationForm : Form
    {
        private static StreetSmartConfigurationForm _StreetSmartConfigurationForm;

        private Configuration.Configuration Config => Configuration.Configuration.Instance;

        private bool _mssgBoxShow;

        private SpatialReference _previousSelectedSRS;

        public StreetSmartConfigurationForm()
        {
            InitializeComponent();

            reloadSettings();

            LoadCulture();
            LoadResources();
        }

        private void LoadResources()
        {
            lblLogin.Text = string.Empty;

            FormStyling.SetStyling(this);

            SetAbout();
            SetAgreement();

            var selectedSrs = (SpatialReference)cbCycloramaSRS.SelectedItem;
            string units = selectedSrs?.Units ?? string.Empty;

            ComponentResourceManager resources = new ComponentResourceManager(typeof(StreetSmartConfigurationForm));
            resources.ApplyResources(lblOverlayDrawDistance, "lblOverlayDrawDistance");
            lblOverlayDrawDistance.Text = string.Format(lblOverlayDrawDistance.Text, units);
        }

        private void reloadSettings()
        {
            LoadLoginData();
            LoadSpatialReferenceData();
            LoadGeneralSettings();
            LoadAPISettings();
        }

        private async void btnOk_Click(object sender, EventArgs e)
        {
            Save();

            LoadResources();

            await StreetSmartApiWrapper.Instance.RestartStreetSmartAPI(Config);

            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void btnApply_Click(object sender, EventArgs e)
        {
            Save();

            LoadResources();

            await StreetSmartApiWrapper.Instance.RestartStreetSmartAPI(Config);
        }

        public static void CheckOpenCredentials()
        {
            bool credentials = Client.Login.Instance.Check();

            if (!credentials)
            {
                OpenForm();
            }
        }

        private void LoadAPISettings()
        {
            chUseDefaultConfigurationUrl.Checked = Config.UseDefaultBaseUrl;
            if (!chUseDefaultConfigurationUrl.Checked)
            {
                txtAPIConfigurationUrl.Text = Config.BaseUrl;
                txtAPIConfigurationUrl.Enabled = true;
            }
            else
            {
                txtAPIConfigurationUrl.Text = "";
                txtAPIConfigurationUrl.Enabled = false;
            }

            chUseDefaultStreetSmartLocation.Checked = Config.UseDefaultStreetSmartLocation;
            if (!chUseDefaultStreetSmartLocation.Checked)
            {
                txtAPIStreetSmartLocation.Text = Config.StreetSmartLocation;
                txtAPIStreetSmartLocation.Enabled = true;
            }
            else
            {
                txtAPIStreetSmartLocation.Text = "";
                txtAPIStreetSmartLocation.Enabled = false;
            }




            ckUseProxyServer.Checked = Config.UseProxyServer;
            ckUseDefaultProxyCredentials.Checked = Config.ProxyUseDefaultCredentials;
            txtProxyAddress.Text = Config.ProxyAddress;
            txtProxyDomain.Text = Config.ProxyDomain;
            txtProxyPort.Text = Config.ProxyPort.ToString();
            txtProxyUsername.Text = Config.ProxyUsername;
            txtProxyPassword.Text = Config.ProxyPassword;
            ckBypassProxyOnLocal.Checked = Config.BypassProxyOnLocal;
        }

        private void LoadGeneralSettings()
        {
            var selectedSRS = (SpatialReference) cbCycloramaSRS.SelectedItem;

            if (selectedSRS != null)
            {
                switch (selectedSRS.Units)
                {
                    case "ft":
                        nudOverlayDrawDistance.Maximum = (decimal) Math.Round((double) nudOverlayDrawDistance.Maximum * 3.280839895, 0);
                        nudOverlayDrawDistance.Value =
                            (decimal) Math.Round(Config.OverlayDrawDistanceInMeters * 3.280839895, 0);
                        break;
                    case "m":
                        nudOverlayDrawDistance.Value = Config.OverlayDrawDistanceInMeters;
                        break;
                    default: break;
                }
            }
        }

        private void LoadCulture()
        {
            var items = new CultureInfo[]
            {
                new CultureInfo(Configuration.Configuration.DefaultCulture),
                new CultureInfo("fr"),
                new CultureInfo("de")
            };
            cbCulture.Items.AddRange(items);

            var current = new CultureInfo(Config.Culture);
            if (cbCulture.Items.Contains(current))
                cbCulture.SelectedItem = current;
        }

        private void LoadLoginData()
        {
            txtUsername.Text = Config.ApiUsername;
            txtPassword.Text = Config.ApiPassword;
        }

        private void LoadSpatialReferenceData()
        {
            //Clear existing items
            cbCycloramaSRS.Items.Clear();
            cbRecordingsSRS.Items.Clear();

            var selectedCMSRS = default(SpatialReference);
            var selectedRCSRS = default(SpatialReference);

            var boundary = ArcUtils.ActiveView?.Extent;

            SpatialReferences spatialReferences = SpatialReferences.Instance;

            foreach (var spatialReference in spatialReferences)
            {
                if (spatialReference.KnownInArcMap)
                {
                    cbCycloramaSRS.Items.Add(spatialReference);
                    cbRecordingsSRS.Items.Add(spatialReference);

                    if (spatialReference.SRSName == Config.ApiSRS)
                    {
                        selectedCMSRS = spatialReference;
                    }
                    if (spatialReference.SRSName == Config.DefaultRecordingSrs)
                    {
                        selectedRCSRS = spatialReference;
                    }

                }
            }
            if (selectedCMSRS != null)
            {
                cbCycloramaSRS.SelectedItem = selectedCMSRS;
            }
            if (selectedRCSRS != null)
            {
                cbRecordingsSRS.SelectedItem = selectedRCSRS;
            }
        }

        public static bool IsActive()
        {
            return (_StreetSmartConfigurationForm != null);
        }

        public static void OpenCloseSwitch()
        {
            if (_StreetSmartConfigurationForm == null)
            {
                OpenForm();
            }
            else
            {
                CloseForm();
            }
        }

        public static void OpenForm()
        {
            if (_StreetSmartConfigurationForm == null)
            {
                _StreetSmartConfigurationForm = new StreetSmartConfigurationForm();
                var application = ArcMap.Application;
                int hWnd = application.hWnd;
                IWin32Window parent = new WindowWrapper(hWnd);
                _StreetSmartConfigurationForm.Show(parent);
            }
            else
            {
                _StreetSmartConfigurationForm.reloadSettings();
            }
        }

        private void ShowAgreement()
        {
            tcSettings.SelectTab(tbAgreement);
        }

        public static void CloseForm()
        {
            _StreetSmartConfigurationForm?.Close();

            _StreetSmartConfigurationForm = null;
        }

        private void Save()
        {
            bool change = false;
            string message = null;

            if (!VerifyConfiguration(out message) && MessageBox.Show($"{message} {Properties.Resources.ConfigurationAsk}", Properties.Resources.Title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
            {
                return;
            }

            change = Config.ApiUsername != txtUsername.Text || Config.ApiPassword != txtPassword.Text;

            Config.ApiUsername = txtUsername.Text;
            Config.ApiPassword = txtPassword.Text;

            var selectedSRS = (SpatialReference)cbCycloramaSRS.SelectedItem;
            Config.ApiSRS = selectedSRS?.SRSName ?? Config.ApiSRS;

            var selectedRecordingSRS = (SpatialReference)cbRecordingsSRS.SelectedItem;
            change = selectedRecordingSRS?.SRSName != Config.DefaultRecordingSrs || change;
            Config.DefaultRecordingSrs = selectedRecordingSRS?.SRSName ?? Config.DefaultRecordingSrs;

            var overlayDrawDistance = (int) nudOverlayDrawDistance.Value;

            if ((selectedSRS?.Units ?? string.Empty) == "ft")
            {
                overlayDrawDistance = (int) Math.Round(overlayDrawDistance / 3.280839895, 0);
            }

            if (overlayDrawDistance > -1 && overlayDrawDistance < 101)
            {
                Config.OverlayDrawDistanceInMeters = overlayDrawDistance;
                StreetSmartApiWrapper.Instance.SetOverlayDrawDistance(overlayDrawDistance, ArcMap.Document.FocusMap.MapUnits);
            }

            var selectedCulture = (CultureInfo)cbCulture.SelectedItem;
            Config.Culture = selectedCulture?.Name ?? Config.Culture;

            change = Config.UseDefaultBaseUrl != chUseDefaultConfigurationUrl.Checked ||
                     Config.BaseUrl != txtAPIConfigurationUrl.Text || change;
            Config.UseDefaultBaseUrl = chUseDefaultConfigurationUrl.Checked;
            Config.BaseUrl = txtAPIConfigurationUrl.Text;

            Config.UseDefaultStreetSmartLocation = chUseDefaultStreetSmartLocation.Checked;
            Config.StreetSmartLocation = txtAPIStreetSmartLocation.Text;

            change = Config.UseProxyServer != ckUseProxyServer.Checked || change;
            Config.UseProxyServer = ckUseProxyServer.Checked;
            change = Config.ProxyUseDefaultCredentials != ckUseDefaultProxyCredentials.Checked || change;
            Config.ProxyUseDefaultCredentials = ckUseDefaultProxyCredentials.Checked;

            change = Config.ProxyAddress != txtProxyAddress.Text || change;
            Config.ProxyAddress = txtProxyAddress.Text;
            change = Config.ProxyDomain != txtProxyDomain.Text || change;
            Config.ProxyDomain = txtProxyDomain.Text;
            var port = 0;

            change = Config.ProxyPort != (int.TryParse(txtProxyPort.Text, out port) ? port : 0) || change;
            Config.ProxyPort = int.TryParse(txtProxyPort.Text, out port) ? port : 0;

            change = Config.ProxyUsername != txtProxyUsername.Text || change;
            Config.ProxyUsername = txtProxyUsername.Text;
            change = Config.ProxyPassword != txtProxyPassword.Text || change;
            Config.ProxyPassword = txtProxyPassword.Text;
            change = Config.BypassProxyOnLocal != ckBypassProxyOnLocal.Checked || change;
            Config.BypassProxyOnLocal = ckBypassProxyOnLocal.Checked;

            Config.Save();

            FormStyling.SetStyling(this);

            if (change)
            {
                MessageBox.Show(Properties.Resources.RestartArcMap, Properties.Resources.Title,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtUsername_KeyUp(object sender, KeyEventArgs e)
        {
            lblLogin.Text = string.Empty;

            if (((e.KeyCode == Keys.Enter) && (!string.IsNullOrEmpty(txtUsername.Text))) && (!_mssgBoxShow))
            {
                txtPassword.Focus();
            }
            else
            {
                _mssgBoxShow = false;
            }

            btnApply.Enabled = true;
        }

        private void txtPassword_KeyUp(object sender, KeyEventArgs e)
        {
            lblLogin.Text = string.Empty;

            if (((e.KeyCode == Keys.Enter) && (!string.IsNullOrEmpty(txtPassword.Text))) && (!_mssgBoxShow))
            {
                Login();
            }
            else
            {
                _mssgBoxShow = false;
            }

            btnApply.Enabled = true;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            lblLogin.Text = string.Empty;

            Login();
        }

        private void Login()
        {
            Client.Login.Instance.SetLoginCredentials(txtUsername.Text, txtPassword.Text);

            if (Client.Login.Instance.Check())
                lblLogin.Text = Properties.Resources.LoginSuccessfully;
            else
                lblLogin.Text = Properties.Resources.LoginFailed;
        }

        private void StreetSmartConfigurationForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _StreetSmartConfigurationForm = null;
        }

        private void SetAbout()
        {
            // Assembly info
            var assembly = Assembly.GetExecutingAssembly();
            var info = FileVersionInfo.GetVersionInfo(assembly.Location);

            // Street Smart API info
            var type = typeof(StreetSmart.WinForms.StreetSmartGUI);
            var apiAssembly = type.Assembly;
            var apiInfo = FileVersionInfo.GetVersionInfo(apiAssembly.Location);

            string[] text =
            {
                $"{info.ProductName} {assembly.GetName().Version.ToString(3)}",
                $"{apiInfo.ProductName} {apiAssembly.GetName().Version.ToString(3)}",
                info.LegalCopyright,
                "https://www.cyclomedia.com"
            };

            rtbAbout.Text = string.Join(Environment.NewLine, text);
        }

        private void rtbAbout_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        private void SetAgreement()
        {
            txtAgreement.Text = Properties.Resources.Agreement;
        }

        public bool VerifyConfiguration(out string message)
        {
            message = null;

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
                message = Properties.Resources.InvalidApiUsername;
            else if (string.IsNullOrWhiteSpace(txtPassword.Text))
                message = Properties.Resources.InvalidApiPassword;
            else if (cbCycloramaSRS.SelectedItem == null)
                message = Properties.Resources.InvalidApiSRS;
            else
                return true;

            return false;
        }

        private void chUseDefaultConfigurationUrl_CheckedChanged(object sender, EventArgs e)
        {
            if (chUseDefaultConfigurationUrl.Checked)
            {
                txtAPIConfigurationUrl.Text = "";
                txtAPIConfigurationUrl.Enabled = false;
            }
            else
            {
                txtAPIConfigurationUrl.Enabled = true;
            }
        }

        private void chUseDefaultStreetSmartLocation_CheckedChanged(object sender, EventArgs e)
        {
            if (chUseDefaultStreetSmartLocation.Checked)
            {
                txtAPIStreetSmartLocation.Text = "";
                txtAPIStreetSmartLocation.Enabled = false;
            }
            else
            {
                txtAPIStreetSmartLocation.Enabled = true;
            }
        }

        private void cbCycloramaSRS_SelectedValueChanged(object sender, EventArgs e)
        {
            setOverlayDrawDistanceValues();
        }

        private void setOverlayDrawDistanceValues()
        {
            var selectedSRS = (SpatialReference) cbCycloramaSRS.SelectedItem;

            if (_previousSelectedSRS == null || selectedSRS.Units != _previousSelectedSRS.Units)
            {
                switch (selectedSRS.Units)
                {
                    case "ft":
                        nudOverlayDrawDistance.Maximum = (decimal) Math.Round((double) nudOverlayDrawDistance.Maximum * 3.280839895, 0);
                        nudOverlayDrawDistance.Value =
                            (decimal) Math.Round((double) nudOverlayDrawDistance.Value * 3.280839895, 0);
                        break;
                    case "m":
                        if (_previousSelectedSRS != null)
                        {
                            nudOverlayDrawDistance.Value =
                                (decimal) Math.Round((double) nudOverlayDrawDistance.Value / 3.280839895, 0);
                            nudOverlayDrawDistance.Maximum =
                                (decimal) Math.Round((double) nudOverlayDrawDistance.Maximum / 3.280839895, 0);
                        }

                        break;
                    default: break;
                }

                _previousSelectedSRS = selectedSRS;
            }

            LoadResources();
        }
    }
}
