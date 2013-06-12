using System;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using RRLib;

namespace ResourceRing
{
	/// <summary>
	/// Summary description for ServerProxy.
	/// </summary>
	public class ServerProxy
	{
		private ulong _sessionID;
		public ulong sessionID
		{
			get { return _sessionID; }
			set { _sessionID = value; }
		}

		public ServerProxy()
		{
		}

		public void syncUserInfo(User user)
		{
			byte[] message = new byte[Constants.WRITEBUFFSIZE];
			MemoryStream stream;

			try 
			{
				//Serialize data in memory so you can send them as a continuous stream
				BinaryFormatter serializer = new BinaryFormatter();
				stream = new MemoryStream(message);
				serializer.Serialize(stream, Constants.MessageTypes.MSG_SYNCUSRINFO);
				serializer.Serialize(stream, sessionID);
				serializer.Serialize(stream, user);
				NetLib.communicate(Constants.SERVER, message, false);
			}
			catch
			{
			}
		}

		public Constants.LoginStatus logon(User user)
		{
			Constants.LoginStatus retval = Constants.LoginStatus.STATUS_SERVERNOTREACHED;
			byte[] message = new byte[Constants.WRITEBUFFSIZE];
			byte[] reply;
			MemoryStream stream = new MemoryStream(message);

			try
			{
				//Serialize data in memory so you can send them as a continuous stream
				BinaryFormatter serializer = new BinaryFormatter();
				
				NetLib.insertEntropyHeader(serializer, stream);

				serializer.Serialize(stream, Constants.MessageTypes.MSG_LOGIN);
				serializer.Serialize(stream, user.ringsInfo[0].ring.ringID);
				serializer.Serialize(stream, user.ringsInfo[0].userName);
				serializer.Serialize(stream, user.ringsInfo[0].password);
				serializer.Serialize(stream, user.node.syncCommunicationPoint);
				reply = NetLib.communicate(Constants.SERVER2,message, true);
				stream.Close();
				stream = new MemoryStream(reply);
				NetLib.bypassEntropyHeader(serializer, stream);
				Constants.MessageTypes replyMsg = (Constants.MessageTypes)serializer.Deserialize(stream);

				switch(replyMsg)
				{
					case Constants.MessageTypes.MSG_OK:
						ulong sessionID = (ulong)serializer.Deserialize(stream);
						uint numRings = (uint)serializer.Deserialize(stream);
						uint ringID;
						Ring ring;
						for(uint ringCounter = 0; ringCounter < numRings; ringCounter++)
						{
							LordInfo lordInfo = (LordInfo)serializer.Deserialize(stream);
							ring = RingInfo.findRingByID(user.ringsInfo, lordInfo.ringID);
							ring.lords = lordInfo.lords;
						}
						
						user.loggedIn = true;
						retval = Constants.LoginStatus.STATUS_LOGGEDIN;
						break;
					case Constants.MessageTypes.MSG_NOTAMEMBER:
						retval = Constants.LoginStatus.STATUS_NOTAMEMBER;
						break;
					case Constants.MessageTypes.MSG_ALREADYSIGNEDIN:
						retval = Constants.LoginStatus.STATUS_ALREADYSIGNEDIN;
						break;
					default:
						break;
				}
			}
			catch (Exception e)
			{
				
				int x = 2;
			}

			return retval;
		}

		public void logoff(User user)
		{
			byte[] message = new byte[Constants.WRITEBUFFSIZE];
			MemoryStream stream = null;

			if(!user.loggedIn)
				return;
			try 
			{
				//Serialize data in memory so you can send them as a continuous stream
				BinaryFormatter serializer = new BinaryFormatter();
				stream = new MemoryStream(message);
				serializer.Serialize(stream, Constants.MessageTypes.MSG_LOGOFF);
				serializer.Serialize(stream, user.publicUserInfo);
				serializer.Serialize(stream, user.privateUserInfo);
				serializer.Serialize(stream, user.node);
				NetLib.communicate(Constants.SERVER,message, false);
				stream.Close();
			}
			catch
			{
				return;
			}
			finally
			{
				user.loggedIn = false;
			}
		}

		public bool signup(User user)
		{
			return true;
		}
	}
}
this.browserAddressBar = new System.Windows.Forms.TextBox();
			this.browserAddressLabel = new System.Windows.Forms.Label();
			this.browserGoButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.browserToolBar = new System.Windows.Forms.ToolBar();
			this.browserToolBarHomeButton = new System.Windows.Forms.ToolBarButton();
			this.browserToolBarBackButton = new System.Windows.Forms.ToolBarButton();
			this.browserToolBarForwardButton = new System.Windows.Forms.ToolBarButton();
			this.browserToolBarStopButton = new System.Windows.Forms.ToolBarButton();
			this.browserToolBarReloadButton = new System.Windows.Forms.ToolBarButton();
			this.browserStatusBar = new System.Windows.Forms.StatusBar();
			this.helpMenuItem = new System.Windows.Forms.MenuItem();
			this.aboutMenuItem = new System.Windows.Forms.MenuItem();
			this.sectionsTab.SuspendLayout();
			this.loginPage.SuspendLayout();
			this.resourcesPage.SuspendLayout();
			this.searchPage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.queryHitsGrid)).BeginInit();
			this.ringsPage.SuspendLayout();
			this.ringsPanel.SuspendLayout();
			this.settingsPage.SuspendLayout();
			this.browsePage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.webBrowser)).BeginInit();
			this.SuspendLayout();
			// 
			// sectionsTab
			// 
			this.sectionsTab.Controls.AddRange(new System.Windows.Forms.Control[] {
																					  this.loginPage,
																					  this.resourcesPage,
																					  this.ringsPage,
																					  this.settingsPage,
																					  this.searchPage,
																					  this.browsePage});
			this.sectionsTab.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sectionsTab.Name = "sectionsTab";
			this.sectionsTab.SelectedIndex = 0;
			this.sectionsTab.Size = new System.Drawing.Size(1024, 774);
			this.sectionsTab.TabIndex = 0;
			this.sectionsTab.SelectedIndexChanged += new System.EventHandler(this.Search_SelectedIndexChanged);

this.loginPage.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.rememberUserNameAndPassword,
																					this.loginButton,
																					this.loginPasswordBox,
																					this.loginBox,
																					this.loginPasswordLabel,
																					this.userNameLoginLabel});
			this.loginPage.Location = new System.Drawing.Point(4, 22);
			this.loginPage.Name = "loginPage";
			this.loginPage.Size = new System.Drawing.Size(1016, 748);
			this.loginPage.TabIndex = 2;
			this.loginPage.Text = "Login";
			// 
			// rememberUserNameAndPassword
			// 
			this.rememberUserNameAndPassword.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.rememberUserNameAndPassword.Location = new System.Drawing.Point(208, 256);
			this.rememberUserNameAndPassword.Name = "rememberUserNameAndPassword";
			this.rememberUserNameAndPassword.Size = new System.Drawing.Size(320, 24);
			this.rememberUserNameAndPassword.TabIndex = 5;
			this.rememberUserNameAndPassword.Text = "Remember my user name and password on this machine";
			// 
			// loginButton
			// 
			this.loginButton.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.loginButton.Location = new System.Drawing.Point(376, 288);
			this.loginButton.Name = "loginButton";
			this.loginButton.TabIndex = 4;
			this.loginButton.Text = "Login";
			// 
			// loginPasswordBox
			// 
			this.loginPasswordBox.Location = new System.Drawing.Point(336, 208);
			this.loginPasswordBox.Name = "loginPasswordBox";
			this.loginPasswordBox.PasswordChar = '*';
			this.loginPasswordBox.Size = new System.Drawing.Size(176, 20);
			this.loginPasswordBox.TabIndex = 3;
			this.loginPasswordBox.Text = "";
			// 
			// loginBox
			// 
			this.loginBox.Location = new System.Drawing.Point(336, 160);
			this.loginBox.Name = "loginBox";
			this.loginBox.Size = new System.Drawing.Size(176, 20);
			this.loginBox.TabIndex = 2;
			this.loginBox.Text = "";
			// 
			// loginPasswordLabel
			// 
			this.loginPasswordLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.loginPasswordLabel.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.loginPasswordLabel.Location = new System.Drawing.Point(200, 208);
			this.loginPasswordLabel.Name = "loginPasswordLabel";
			this.loginPasswordLabel.Size = new System.Drawing.Size(112, 48);
			this.loginPasswordLabel.TabIndex = 1;
			this.loginPasswordLabel.Text = "Password";
			// 
			// userNameLoginLabel
			// 
			this.userNameLoginLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.userNameLoginLabel.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.userNameLoginLabel.Location = new System.Drawing.Point(200, 160);
			this.userNameLoginLabel.Name = "userNameLoginLabel";
			this.userNameLoginLabel.Size = new System.Drawing.Size(136, 48);
			this.userNameLoginLabel.TabIndex = 0;
			this.userNameLoginLabel.Text = "User Name";
			// 
			// resourcesPage
			// 
			this.resourcesPage.Controls.AddRange(new System.Windows.Forms.Control[] {
																						this.numCatalogedLabel,
																						this.progressCount,
																						this.catalogProgressLabel,
																						this.catalogProgressBar,
																						this.resourceInfo,
																						this.resourceTree});
			this.resourcesPage.Location = new System.Drawing.Point(4, 22);
			this.resourcesPage.Name = "resourcesPage";
			this.resourcesPage.Size = new System.Drawing.Size(1016, 748);
			this.resourcesPage.TabIndex = 4;
			this.resourcesPage.Text = "Resources";
			// 
			// numCatalogedLabel
			// 
			this.numCatalogedLabel.Location = new System.Drawing.Point(112, 616);
			this.numCatalogedLabel.Name = "numCatalogedLabel";
			this.numCatalogedLabel.TabIndex = 5;
			this.numCatalogedLabel.Text = "Files Catalogged";
			// 
			// progressCount
			// 
			this.progressCount.Location = new System.Drawing.Point(24, 616);
			this.progressCount.Name = "progressCount";
			this.progressCount.Size = new System.Drawing.Size(64, 32);
			this.progressCount.TabIndex = 4;
			// 
			// catalogProgressLabel
			// 
			this.catalogProgressLabel.Location = new System.Drawing.Point(24, 584);
			this.catalogProgressLabel.Name = "catalogProgressLabel";
			this.catalogProgressLabel.Size = new System.Drawing.Size(64, 23);
			this.catalogProgressLabel.TabIndex = 3;
			this.catalogProgressLabel.Text = "Cataloging";
			// 
			// catalogProgressBar
			// 
			this.catalogProgressBar.Location = new System.Drawing.Point(88, 576);
			this.catalogProgressBar.Name = "catalogProgressBar";
			this.catalogProgressBar.Size = new System.Drawing.Size(368, 23);
			this.catalogProgressBar.TabIndex = 2;
			// 
			// resourceInfo
			// 
			this.resourceInfo.Location = new System.Drawing.Point(472, 8);
			this.resourceInfo.Name = "resourceInfo";
			this.resourceInfo.Size = new System.Drawing.Size(528, 552);
			this.resourceInfo.TabIndex = 1;
			this.resourceInfo.Text = "";
			// 
			// resourceTree
			// 
			this.resourceTree.ImageIndex = -1;
			this.resourceTree.Location = new System.Drawing.Point(8, 8);
			this.resourceTree.Name = "resourceTree";
			this.resourceTree.SelectedImageIndex = -1;
			this.resourceTree.Size = new System.Drawing.Size(448, 552);
			this.resourceTree.TabIndex = 0;
			// 
			// searchPage
			// 
			this.searchPage.Controls.AddRange(new System.Windows.Forms.Control[] {
																					 this.binaryCheckBox,
																					 this.commPointLabel,
																					 this.graphicsCheckBox,
																					 this.documentCheckBox,
																					 this.videoCheckBox,
																					 this.audioCheckBox,
																					 this.musicCheckBox,
																					 this.queryHitsGrid,
																					 this.searchButton,
																					 this.searchBox});
			this.searchPage.Location = new System.Drawing.Point(4, 22);
			this.searchPage.Name = "searchPage";
			this.searchPage.Size = new System.Drawing.Size(1016, 748);
			this.searchPage.TabIndex = 0;
			this.searchPage.Text = "Search";
			// 
			// binaryCheckBox
			// 
			this.binaryCheckBox.Location = new System.Drawing.Point(8, 240);
			this.binaryCheckBox.Name = "binaryCheckBox";
			this.binaryCheckBox.TabIndex = 9;
			this.binaryCheckBox.Text = "Binary";
			this.binaryCheckBox.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// commPointLabel
			// 
			this.commPointLabel.Location = new System.Drawing.Point(8, 272);
			this.commPointLabel.Name = "commPointLabel";
			this.commPointLabel.Size = new System.Drawing.Size(184, 256);
			this.commPointLabel.TabIndex = 8;
			this.commPointLabel.Text = "no comm point";
			// 
			// graphicsCheckBox
			// 
			this.graphicsCheckBox.Location = new System.Drawing.Point(8, 176);
			this.graphicsCheckBox.Name = "graphicsCheckBox";
			this.graphicsCheckBox.TabIndex = 7;
			this.graphicsCheckBox.Text = "Graphics";
			// 
			// documentCheckBox
			// 
			this.documentCheckBox.Location = new System.Drawing.Point(8, 208);
			this.documentCheckBox.Name = "documentCheckBox";
			this.documentCheckBox.TabIndex = 6;
			this.documentCheckBox.Text = "Document";
			// 
			// videoCheckBox
			// 
			this.videoCheckBox.Location = new System.Drawing.Point(8, 144);
			this.videoCheckBox.Name = "videoCheckBox";
			this.videoCheckBox.TabIndex = 5;
			this.videoCheckBox.Text = "Video";
			// 
			// audioCheckBox
			// 
			this.audioCheckBox.Location = new System.Drawing.Point(8, 112);
			this.audioCheckBox.Name = "audioCheckBox";
			this.audioCheckBox.TabIndex = 4;
			this.audioCheckBox.Text = "Audio";
			// 
			// musicCheckBox
			// 
			this.musicCheckBox.Enabled = false;
			this.musicCheckBox.Location = new System.Drawing.Point(8, 80);
			this.musicCheckBox.Name = "musicCheckBox";
			this.musicCheckBox.TabIndex = 3;
			this.musicCheckBox.Text = "Music";
			// 
			// queryHitsGrid
			// 
			this.queryHitsGrid.AllowSorting = ((bool)(configurationAppSettings.GetValue("queryHitsGrid.AllowSorting", typeof(bool))));
			this.queryHitsGrid.ColumnHeadersVisible = ((bool)(configurationAppSettings.GetValue("queryHitsGrid.ColumnHeadersVisible", typeof(bool))));
			this.queryHitsGrid.DataMember = "";
			this.queryHitsGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.queryHitsGrid.Location = new System.Drawing.Point(200, 8);
			this.queryHitsGrid.Name = "queryHitsGrid";
			this.queryHitsGrid.PreferredColumnWidth = ((int)(configurationAppSettings.GetValue("queryHitsGrid.PreferredColumnWidth", typeof(int))));
			this.queryHitsGrid.PreferredRowHeight = ((int)(configurationAppSettings.GetValue("queryHitsGrid.PreferredRowHeight", typeof(int))));
			this.queryHitsGrid.ReadOnly = ((bool)(configurationAppSettings.GetValue("queryHitsGrid.ReadOnly", typeof(bool))));
			this.queryHitsGrid.RowHeadersVisible = ((bool)(configurationAppSettings.GetValue("queryHitsGrid.RowHeadersVisible", typeof(bool))));
			this.queryHitsGrid.RowHeaderWidth = ((int)(configurationAppSettings.GetValue("queryHitsGrid.RowHeaderWidth", typeof(int))));
			this.queryHitsGrid.Size = new System.Drawing.Size(800, 720);
			this.queryHitsGrid.TabIndex = 2;
			this.queryHitsGrid.Navigate += new System.Windows.Forms.NavigateEventHandler(this.queryHitsGrid_Navigate);
			// 
			// searchButton
			// 
			this.searchButton.Location = new System.Drawing.Point(40, 40);
			this.searchButton.Name = "searchButton";
			this.searchButton.Size = new System.Drawing.Size(96, 24);
			this.searchButton.TabIndex = 1;
			this.searchButton.Text = "Search";
			// 
			// searchBox
			// 
			this.searchBox.Location = new System.Drawing.Point(0, 8);
			this.searchBox.Name = "searchBox";
			this.searchBox.Size = new System.Drawing.Size(184, 20);
			this.searchBox.TabIndex = 0;
			this.searchBox.Text = "";
			// 
			// ringsPage
			// 
			this.ringsPage.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.ringNeighborsLabelText,
																					this.ringNeighborsLabel,
																					this.ringPasswordLabelText,
																					this.ringPasswordLabel,
																					this.ringUserNameLabelText,
																					this.ringUserNameLabel,
																					this.extensionLabelText,
																					this.extensionLabel,
																					this.ringNameLabelText,
																					this.ringNameLabel,
																					this.ringsPanel});
			this.ringsPage.Location = new System.Drawing.Point(4, 22);
			this.ringsPage.Name = "ringsPage";
			this.ringsPage.Size = new System.Drawing.Size(1016, 748);
			this.ringsPage.TabIndex = 3;
			this.ringsPage.Text = "Rings";
			// 
			// ringNeighborsLabelText
			// 
			this.ringNeighborsLabelText.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.ringNeighborsLabelText.Location = new System.Drawing.Point(304, 232);
			this.ringNeighborsLabelText.Name = "ringNeighborsLabelText";
			this.ringNeighborsLabelText.Size = new System.Drawing.Size(688, 23);
			this.ringNeighborsLabelText.TabIndex = 10;
			// 
			// ringNeighborsLabel
			// 
			this.ringNeighborsLabel.Location = new System.Drawing.Point(248, 232);
			this.ringNeighborsLabel.Name = "ringNeighborsLabel";
			this.ringNeighborsLabel.Size = new System.Drawing.Size(56, 23);
			this.ringNeighborsLabel.TabIndex = 9;
			this.ringNeighborsLabel.Text = "Neighbors:";
			// 
			// ringPasswordLabelText
			// 
			this.ringPasswordLabelText.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.ringPasswordLabelText.Location = new System.Drawing.Point(304, 200);
			this.ringPasswordLabelText.Name = "ringPasswordLabelText";
			this.ringPasswordLabelText.Size = new System.Drawing.Size(688, 23);
			this.ringPasswordLabelText.TabIndex = 8;
			// 
			// ringPasswordLabel
			// 
			this.ringPasswordLabel.Location = new System.Drawing.Point(248, 200);
			this.ringPasswordLabel.Name = "ringPasswordLabel";
			this.ringPasswordLabel.Size = new System.Drawing.Size(64, 23);
			this.ringPasswordLabel.TabIndex = 7;
			this.ringPasswordLabel.Text = "Password:";
			// 
			// ringUserNameLabelText
			// 
			this.ringUserNameLabelText.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.ringUserNameLabelText.Location = new System.Drawing.Point(312, 168);
			this.ringUserNameLabelText.Name = "ringUserNameLabelText";
			this.ringUserNameLabelText.Size = new System.Drawing.Size(680, 23);
			this.ringUserNameLabelText.TabIndex = 6;
			// 
			// ringUserNameLabel
			// 
			this.ringUserNameLabel.Location = new System.Drawing.Point(248, 168);
			this.ringUserNameLabel.Name = "ringUserNameLabel";
			this.ringUserNameLabel.Size = new System.Drawing.Size(64, 23);
			this.ringUserNameLabel.TabIndex = 5;
			this.ringUserNameLabel.Text = "User Name:";
			// 
			// extensionLabelText
			// 
			this.extensionLabelText.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.extensionLabelText.Location = new System.Drawing.Point(368, 48);
			this.extensionLabelText.Name = "extensionLabelText";
			this.extensionLabelText.Size = new System.Drawing.Size(624, 112);
			this.extensionLabelText.TabIndex = 4;
			// 
			// extensionLabel
			// 
			this.extensionLabel.Location = new System.Drawing.Point(240, 48);
			this.extensionLabel.Name = "extensionLabel";
			this.extensionLabel.Size = new System.Drawing.Size(144, 24);
			this.extensionLabel.TabIndex = 3;
			this.extensionLabel.Text = "Common File Extensions:";
			// 
			// ringNameLabelText
			// 
			this.ringNameLabelText.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.ringNameLabelText.Location = new System.Drawing.Point(304, 16);
			this.ringNameLabelText.Name = "ringNameLabelText";
			this.ringNameLabelText.Size = new System.Drawing.Size(688, 23);
			this.ringNameLabelText.TabIndex = 2;
			// 
			// ringNameLabel
			// 
			this.ringNameLabel.Location = new System.Drawing.Point(240, 16);
			this.ringNameLabel.Name = "ringNameLabel";
			this.ringNameLabel.Size = new System.Drawing.Size(64, 23);
			this.ringNameLabel.TabIndex = 1;
			this.ringNameLabel.Text = "Ring Name:";
			// 
			// ringsPanel
			// 
			this.ringsPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																					 this.ringsTree});
			this.ringsPanel.Name = "ringsPanel";
			this.ringsPanel.Size = new System.Drawing.Size(216, 576);
			this.ringsPanel.TabIndex = 0;
			// 
			// ringsTree
			// 
			this.ringsTree.ImageIndex = -1;
			this.ringsTree.Location = new System.Drawing.Point(8, 0);
			this.ringsTree.Name = "ringsTree";
			this.ringsTree.SelectedImageIndex = -1;
			this.ringsTree.Size = new System.Drawing.Size(200, 568);
			this.ringsTree.TabIndex = 0;
			// 
			// settingsPage
			// 
			this.settingsPage.Controls.AddRange(new System.Windows.Forms.Control[] {
																					   this.settingsStatusLabel,
																					   this.passwordBox2,
																					   this.passwordLabel2,
																					   this.zipCodeBox,
																					   this.zipCodeLabel,
																					   this.countryBox,
																					   this.countryLabel,
																					   this.stateBox,
																					   this.stateLabel,
																					   this.cityBox,
																					   this.cityLabel,
																					   this.streetAddrBox,
																					   this.streetAddrLabel,
																					   this.submitButton,
																					   this.suffixBox,
																					   this.suffixLabel,
																					   this.lastNameBox,
																					   this.LastNameLabel,
																					   this.middleNameBox,
																					   this.middleNameLabel,
																					   this.firstNameBox,
																					   this.firstNameLabel,
																					   this.prefixBox,
																					   this.prefixLabel,
																					   this.personalInfoDisclaimer,
																					   this.passwordBox,
																					   this.passwordLabel,
																					   this.userNameLabel,
																					   this.userNameBox});
			this.settingsPage.Location = new System.Drawing.Point(4, 22);
			this.settingsPage.Name = "settingsPage";
			this.settingsPage.Size = new System.Drawing.Size(1016, 748);
			this.settingsPage.TabIndex = 1;
			this.settingsPage.Text = "Settings";
			// 
			// settingsStatusLabel
			// 
			this.settingsStatusLabel.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(192)), ((System.Byte)(0)), ((System.Byte)(0)));
			this.settingsStatusLabel.Location = new System.Drawing.Point(56, 640);
			this.settingsStatusLabel.Name = "settingsStatusLabel";
			this.settingsStatusLabel.Size = new System.Drawing.Size(208, 23);
			this.settingsStatusLabel.TabIndex = 28;
			// 
			// passwordBox2
			// 
			this.passwordBox2.Location = new System.Drawing.Point(96, 136);
			this.passwordBox2.Name = "passwordBox2";
			this.passwordBox2.PasswordChar = '*';
			this.passwordBox2.Size = new System.Drawing.Size(224, 20);
			this.passwordBox2.TabIndex = 2;
			this.passwordBox2.Text = "";
			// 
			// passwordLabel2
			// 
			this.passwordLabel2.Location = new System.Drawing.Point(16, 136);
			this.passwordLabel2.Name = "passwordLabel2";
			this.passwordLabel2.Size = new System.Drawing.Size(64, 32);
			this.passwordLabel2.TabIndex = 26;
			this.passwordLabel2.Text = "Password(again)";
			// 
			// zipCodeBox
			// 
			this.zipCodeBox.Location = new System.Drawing.Point(96, 496);
			this.zipCodeBox.Name = "zipCodeBox";
			this.zipCodeBox.Size = new System.Drawing.Size(72, 20);
			this.zipCodeBox.TabIndex = 11;
			this.zipCodeBox.Text = "";
			// 
			// zipCodeLabel
			// 
			this.zipCodeLabel.Location = new System.Drawing.Point(16, 496);
			this.zipCodeLabel.Name = "zipCodeLabel";
			this.zipCodeLabel.TabIndex = 23;
			this.zipCodeLabel.Text = "Zip Code";
			// 
			// countryBox
			// 
			this.countryBox.Items.AddRange(new object[] {
															"Bosnia & Hertzegovina",
															"France",
															"Germany",
															"Iran",
															"Iraq",
															"United States of America"});
			this.countryBox.Location = new System.Drawing.Point(96, 536);
			this.countryBox.Name = "countryBox";
			this.countryBox.Size = new System.Drawing.Size(168, 21);
			this.countryBox.TabIndex = 12;
			// 
			// countryLabel
			// 
			this.countryLabel.Location = new System.Drawing.Point(16, 536);
			this.countryLabel.Name = "countryLabel";
			this.countryLabel.TabIndex = 24;
			this.countryLabel.Text = "Country";
			// 
			// stateBox
			// 
			this.stateBox.Items.AddRange(new object[] {
														  "California",
														  "Nevada"});
			this.stateBox.Location = new System.Drawing.Point(96, 456);
			this.stateBox.Name = "stateBox";
			this.stateBox.Size = new System.Drawing.Size(168, 21);
			this.stateBox.TabIndex = 10;
			// 
			// stateLabel
			// 
			this.stateLabel.Location = new System.Drawing.Point(16, 456);
			this.stateLabel.Name = "stateLabel";
			this.stateLabel.TabIndex = 22;
			this.stateLabel.Text = "State/Province";
			// 
			// cityBox
			// 
			this.cityBox.Location = new System.Drawing.Point(96, 416);
			this.cityBox.Name = "cityBox";
			this.cityBox.Size = new System.Drawing.Size(224, 20);
			this.cityBox.TabIndex = 9;
			this.cityBox.Text = "";
			// 
			// cityLabel
			// 
			this.cityLabel.Location = new System.Drawing.Point(16, 416);
			this.cityLabel.Name = "cityLabel";
			this.cityLabel.TabIndex = 21;
			this.cityLabel.Text = "City";
			// 
			// streetAddrBox
			// 
			this.streetAddrBox.Location = new System.Drawing.Point(96, 376);
			this.streetAddrBox.Name = "streetAddrBox";
			this.streetAddrBox.Size = new System.Drawing.Size(224, 20);
			this.streetAddrBox.TabIndex = 8;
			this.streetAddrBox.Text = "";
			// 
			// streetAddrLabel
			// 
			this.streetAddrLabel.Location = new System.Drawing.Point(16, 376);
			this.streetAddrLabel.Name = "streetAddrLabel";
			this.streetAddrLabel.TabIndex = 20;
			this.streetAddrLabel.Text = "Street Address";
			// 
			// submitButton
			// 
			this.submitButton.Location = new System.Drawing.Point(120, 592);
			this.submitButton.Name = "submitButton";
			this.submitButton.TabIndex = 13;
			this.submitButton.Text = "Submit";
			// 
			// suffixBox
			// 
			this.suffixBox.Items.AddRange(new object[] {
														   "Jr.",
														   "Sr.",
														   "I",
														   "II",
														   "III",
														   "IV",
														   "V",
														   "VI",
														   "VII",
														   "VIII",
														   "IX",
														   "X"});
			this.suffixBox.Location = new System.Drawing.Point(96, 336);
			this.suffixBox.Name = "suffixBox";
			this.suffixBox.Size = new System.Drawing.Size(80, 21);
			this.suffixBox.TabIndex = 7;
			// 
			// suffixLabel
			// 
			this.suffixLabel.Location = new System.Drawing.Point(16, 336);
			this.suffixLabel.Name = "suffixLabel";
			this.suffixLabel.Size = new System.Drawing.Size(48, 23);
			this.suffixLabel.TabIndex = 19;
			this.suffixLabel.Text = "Suffix";
			// 
			// lastNameBox
			// 
			this.lastNameBox.Location = new System.Drawing.Point(96, 296);
			this.lastNameBox.Name = "lastNameBox";
			this.lastNameBox.Size = new System.Drawing.Size(224, 20);
			this.lastNameBox.TabIndex = 6;
			this.lastNameBox.Text = "";
			// 
			// LastNameLabel
			// 
			this.LastNameLabel.Location = new System.Drawing.Point(16, 296);
			this.LastNameLabel.Name = "LastNameLabel";
			this.LastNameLabel.Size = new System.Drawing.Size(72, 23);
			this.LastNameLabel.TabIndex = 18;
			this.LastNameLabel.Text = "Last Name";
			// 
			// middleNameBox
			// 
			this.middleNameBox.Location = new System.Drawing.Point(96, 256);
			this.middleNameBox.Name = "middleNameBox";
			this.middleNameBox.Size = new System.Drawing.Size(224, 20);
			this.middleNameBox.TabIndex = 5;
			this.middleNameBox.Text = "";
			// 
			// middleNameLabel
			// 
			this.middleNameLabel.Location = new System.Drawing.Point(16, 256);
			this.middleNameLabel.Name = "middleNameLabel";
			this.middleNameLabel.Size = new System.Drawing.Size(72, 23);
			this.middleNameLabel.TabIndex = 17;
			this.middleNameLabel.Text = "Middle Name";
			// 
			// firstNameBox
			// 
			this.firstNameBox.Location = new System.Drawing.Point(96, 216);
			this.firstNameBox.Name = "firstNameBox";
			this.firstNameBox.Size = new System.Drawing.Size(224, 20);
			this.firstNameBox.TabIndex = 4;
			this.firstNameBox.Text = "";
			// 
			// firstNameLabel
			// 
			this.firstNameLabel.Location = new System.Drawing.Point(16, 216);
			this.firstNameLabel.Name = "firstNameLabel";
			this.firstNameLabel.Size = new System.Drawing.Size(72, 23);
			this.firstNameLabel.TabIndex = 16;
			this.firstNameLabel.Text = "First Name";
			// 
			// prefixBox
			// 
			this.prefixBox.Items.AddRange(new object[] {
														   "Mr.",
														   "Ms.",
														   "Mrs.",
														   "Sr.",
														   "Dr."});
			this.prefixBox.Location = new System.Drawing.Point(96, 176);
			this.prefixBox.Name = "prefixBox";
			this.prefixBox.Size = new System.Drawing.Size(80, 21);
			this.prefixBox.TabIndex = 3;
			// 
			// prefixLabel
			// 
			this.prefixLabel.Location = new System.Drawing.Point(16, 176);
			this.prefixLabel.Name = "prefixLabel";
			this.prefixLabel.Size = new System.Drawing.Size(48, 23);
			this.prefixLabel.TabIndex = 15;
			this.prefixLabel.Text = "Prefix";
			// 
			// personalInfoDisclaimer
			// 
			this.personalInfoDisclaimer.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.personalInfoDisclaimer.Location = new System.Drawing.Point(16, 16);
			this.personalInfoDisclaimer.Name = "personalInfoDisclaimer";
			this.personalInfoDisclaimer.Size = new System.Drawing.Size(448, 24);
			this.personalInfoDisclaimer.TabIndex = 25;
			this.personalInfoDisclaimer.Text = "Personal Information: (Fields marked with (*) will be visible to other ResourceRi" +
				"ng users";
			// 
			// passwordBox
			// 
			this.passwordBox.Location = new System.Drawing.Point(96, 96);
			this.passwordBox.Name = "passwordBox";
			this.passwordBox.PasswordChar = '*';
			this.passwordBox.Size = new System.Drawing.Size(224, 20);
			this.passwordBox.TabIndex = 1;
			this.passwordBox.Text = "";
			// 
			// passwordLabel
			// 
			this.passwordLabel.Location = new System.Drawing.Point(16, 96);
			this.passwordLabel.Name = "passwordLabel";
			this.passwordLabel.Size = new System.Drawing.Size(64, 23);
			this.passwordLabel.TabIndex = 14;
			this.passwordLabel.Text = "Password";
			// 
			// userNameLabel
			// 
			this.userNameLabel.Location = new System.Drawing.Point(16, 56);
			this.userNameLabel.Name = "userNameLabel";
			this.userNameLabel.Size = new System.Drawing.Size(72, 16);
			this.userNameLabel.TabIndex = 27;
			this.userNameLabel.Text = "* User Name";
			// 
			// userNameBox
			// 
			this.userNameBox.Location = new System.Drawing.Point(96, 56);
			this.userNameBox.Name = "userNameBox";
			this.userNameBox.Size = new System.Drawing.Size(224, 20);
			this.userNameBox.TabIndex = 0;
			this.userNameBox.Text = "";
			// 
			// browsePage
			// 
			this.browsePage.Controls.AddRange(new System.Windows.Forms.Control[] {
																					 this.browserStatusBar,
																					 this.browserToolBar,
																					 this.browserGoButton,
																					 this.browserAddressLabel,
																					 this.browserAddressBar,
																					 this.webBrowser,
																					 this.groupBox1});
			this.browsePage.Location = new System.Drawing.Point(4, 22);
			this.browsePage.Name = "browsePage";
			this.browsePage.Size = new System.Drawing.Size(1016, 748);
			this.browsePage.TabIndex = 5;
			this.browsePage.Text = "Browse";
			// 
			// webBrowser
			// 
			this.webBrowser.ContainingControl = this;
			this.webBrowser.Enabled = true;
			this.webBrowser.Location = new System.Drawing.Point(0, 72);
			this.webBrowser.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("webBrowser.OcxState")));
			this.webBrowser.Size = new System.Drawing.Size(1016, 656);
			this.webBrowser.TabIndex = 0;
			this.webBrowser.Enter += new System.EventHandler(this.webBrowser_Enter);
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.fileMenuItem,
																					 this.helpMenuItem});
			// 
			// fileMenuItem
			// 
			this.fileMenuItem.Index = 0;
			this.fileMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.exitMenuItem});
			this.fileMenuItem.Text = "File";
			// 
			// exitMenuItem
			// 
			this.exitMenuItem.Index = 0;
			this.exitMenuItem.Text = "Exit";
			// 
			// browserAddressBar
			// 
			this.browserAddressBar.Location = new System.Drawing.Point(64, 48);
			this.browserAddressBar.Name = "browserAddressBar";
			this.browserAddressBar.Size = new System.Drawing.Size(888, 20);
			this.browserAddressBar.TabIndex = 5;
			this.browserAddressBar.Text = "";
			// 
			// browserAddressLabel
			// 
			this.browserAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.browserAddressLabel.Location = new System.Drawing.Point(8, 48);
			this.browserAddressLabel.Name = "browserAddressLabel";
			this.browserAddressLabel.Size = new System.Drawing.Size(48, 16);
			this.browserAddressLabel.TabIndex = 6;
			this.browserAddressLabel.Text = "Address";
			// 
			// browserGoButton
			// 
			this.browserGoButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.browserGoButton.Location = new System.Drawing.Point(960, 48);
			this.browserGoButton.Name = "browserGoButton";
			this.browserGoButton.Size = new System.Drawing.Size(40, 16);
			this.browserGoButton.TabIndex = 7;
			this.browserGoButton.Text = "Go";
			// 
			// groupBox1
			// 
			this.groupBox1.Location = new System.Drawing.Point(0, 40);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(1008, 32);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			// 
			// browserToolBar
			// 
			this.browserToolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																							  this.browserToolBarHomeButton,
																							  this.browserToolBarBackButton,
																							  this.browserToolBarForwardButton,
																							  this.browserToolBarStopButton,
																							  this.browserToolBarReloadButton});
			this.browserToolBar.DropDownArrows = true;
			this.browserToolBar.Name = "browserToolBar";
			this.browserToolBar.ShowToolTips = true;
			this.browserToolBar.Size = new System.Drawing.Size(1016, 39);
			this.browserToolBar.TabIndex = 10;
			this.browserToolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.browserToolBar_ButtonClick);
			// 
			// browserToolBarHomeButton
			// 
			this.browserToolBarHomeButton.Text = "Home";
			// 
			// browserToolBarBackButton
			// 
			this.browserToolBarBackButton.Text = "Back";
			// 
			// browserToolBarForwardButton
			// 
			this.browserToolBarForwardButton.Text = "Forward";
			// 
			// browserToolBarStopButton
			// 
			this.browserToolBarStopButton.Text = "Stop";
			// 
			// browserToolBarReloadButton
			// 
			this.browserToolBarReloadButton.Text = "Reload";
			// 
			// browserStatusBar
			// 
			this.browserStatusBar.Location = new System.Drawing.Point(0, 724);
			this.browserStatusBar.Name = "browserStatusBar";
			this.browserStatusBar.Size = new System.Drawing.Size(1016, 24);
			this.browserStatusBar.TabIndex = 12;
			// 
			// helpMenuItem
			// 
			this.helpMenuItem.Index = 1;
			this.helpMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.aboutMenuItem});
			this.helpMenuItem.Text = "Help";
			// 
			// aboutMenuItem
			// 
			this.aboutMenuItem.Index = 0;
			this.aboutMenuItem.Text = "About ResourceRing";
			// 
			// ClientUserInterface
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.Desktop;
			this.ClientSize = new System.Drawing.Size(1024, 774);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.sectionsTab});
			this.Menu = this.mainMenu;
			this.Name = "ClientUserInterface";
			this.Text = "ResourceRing";
			this.sectionsTab.ResumeLayout(false);
			this.loginPage.ResumeLayout(false);
			this.resourcesPage.ResumeLayout(false);
			this.searchPage.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.queryHitsGrid)).EndInit();
			this.ringsPage.ResumeLayout(false);
			this.ringsPanel.ResumeLayout(false);
			this.settingsPage.ResumeLayout(false);
			this.browsePage.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.webBrowser)).EndInit();
			this.ResumeLayout(false);
