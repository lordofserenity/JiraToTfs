﻿/*================================================================================================================================
Copyright (c) 2015 Ian Montgomery

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files 
(the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, 
publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:
    
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
================================================================================================================================*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using JiraToTfs.Presenter;
using TicketImporter;
using TicketImporter.Interface;

namespace JiraToTfs.View
{
    public partial class JiraToTfsView : Form, IJiraToTfsView
    {
        public JiraToTfsView()
        {
            InitializeComponent();
            presenter = new JiraToTfsPresenter(this);
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            presenter.ClosingUp();
            Close();
        }

        private void selectProjectBtn_Click(object sender, EventArgs e)
        {
            presenter.SelectTfsProject();
        }

        private void ImportBtn_Click(object sender, EventArgs e)
        {
            presenter.StartImport();
        }

        private void jiraServer_TextChanged(object sender, EventArgs e)
        {
            presenter.SettingsHaveChanged();
        }

        private void jiraUserName_TextChanged(object sender, EventArgs e)
        {
            presenter.SettingsHaveChanged();
        }

        private void jiraPassword_TextChanged(object sender, EventArgs e)
        {
            presenter.SettingsHaveChanged();
        }

        private void jiraProject_TextChanged(object sender, EventArgs e)
        {
            presenter.SettingsHaveChanged();
        }

        private void tfsProjectName_TextChanged(object sender, EventArgs e)
        {
            presenter.SettingsHaveChanged();
        }

        private void JiraToTfsView_Shown(object sender, EventArgs e)
        {
            presenter.Initialise();
        }

        private void jiraMappingsBtn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            presenter.OnShowAdvancedSettings();
        }

        private void tfsTeams_TextChanged(object sender, EventArgs e)
        {
            selectedTeam = tfsTeams.Text;
        }

        private void areaPaths_TextChanged(object sender, EventArgs e)
        {
            selectedAreaPath = areaPaths.Text;
        }

        private void viewReport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            presenter.OnShowReport();
        }

        #region private class members

        private readonly JiraToTfsPresenter presenter;
        private string selectedTeam = "";
        private string selectedAreaPath = "";

        #endregion

        #region IJiraToTfsView Interface

        public string TfsProject
        {
            set { tfsProject.Text = value; }
            get { return tfsProject.Text; }
        }

        public string JiraServer
        {
            set { jiraServer.Text = value; }
            get { return jiraServer.Text; }
        }

        public string JiraUserName
        {
            set { jiraUserName.Text = value; }
            get { return jiraUserName.Text; }
        }

        public string JiraPassword
        {
            set { jiraPassword.Text = value; }
            get { return jiraPassword.Text; }
        }

        public string JiraProject
        {
            set { jiraProject.Text = value; }
            get { return jiraProject.Text; }
        }

        public List<string> TfsTeams
        {
            set { tfsTeams.DataSource = value; }
        }

        public string SelectedTfsTeam
        {
            get { return selectedTeam; }
            set { tfsTeams.SelectedItem = value; }
        }

        public List<string> AreaPaths
        {
            set { areaPaths.DataSource = value; }
        }

        public string SelectedAreaPath
        {
            get { return selectedAreaPath; }
            set { areaPaths.SelectedItem = value; }
        }

        public void ClearMessages()
        {
            progressMessage.Text = "";
            detailedProgress.Text = "";
        }

        public void WarnUser(string problem)
        {
            progressMessage.ForeColor = Color.Red;
            progressMessage.Text = problem;
        }

        public void InformUser(string info)
        {
            progressMessage.ForeColor = Color.DarkBlue;
            progressMessage.Text = info;
        }

        public void InformUserOfDetailedProgress(string info)
        {
            detailedProgress.Text = info;
        }

        public void ShowFailedTickets(List<IFailedTicket> failedTickets)
        {
            var view = new TicektsNotImportedView(failedTickets);
            view.OnShowCustomFieldMappings += () => { presenter.OnShowCustomFieldMappings(); };
            view.ShowDialog(this);
        }

        private void enableView(bool enable)
        {
            CloseBtn.Enabled = ImportBtn.Enabled = enable;
            jiraServer.Enabled = jiraUserName.Enabled = jiraProject.Enabled = jiraPassword.Enabled = enable;
            selectProjectBtn.Enabled = enable;
            jiraMappingsBtn.Enabled = enable;
            tfsTeams.Enabled = enable;
            areaPaths.Enabled = enable;
        }

        public void ImportStarted()
        {
            pleaseWaitSpinner.Visible = true;
            enableView(false);
            viewReport.Visible = false;
        }

        public void ImportFinished()
        {
            pleaseWaitSpinner.Visible = false;
            enableView(true);
            detailedProgress.Text = "";
            viewReport.Visible = true;
        }

        public void ShowAdvancedSettings(AdvancedSettings advancedSettings)
        {
            var view = new AdvancedSettingsView(advancedSettings);
            view.ShowDialog(this);
        }

        public bool IncludeAttachments
        {
            set { includeAttachments.Checked = value; }
            get { return includeAttachments.Checked; }
        }

        public void WaitStart()
        {
            enableView(false);
            pleaseWaitSpinner.Visible = true;
            progressMessage.Text = "Please wait, loading defaults ..";
        }

        public void WaitEnd()
        {
            enableView(true);
            pleaseWaitSpinner.Visible = false;
        }

        public void ShowReport(string path)
        {
            Process.Start(path);
        }

        #endregion
    }
}