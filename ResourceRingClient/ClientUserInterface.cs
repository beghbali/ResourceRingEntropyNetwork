using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Timers;
using System.IO;
using System.Diagnostics;
using RRLib;

namespace ResourceRing
{
	/// <summary>
	/// Summary description for ResourceRing.
	/// </summary>
	public class ClientUserInterface : System.Windows.Forms.Form
	{
		
		/// <summary>
		/// Defines the delegates for changing the user interface layer
		/// </summary>
		public class Callbacks
		{
			public delegate void showPostLogin();
			public delegate void showCatalog(RingInfo ringInfo);
			public delegate void showQueryHit(RingInfo ringInfo, Peer hitPeer, ResourceHeader resourceHeader);
		}
	
		// Table of user interface delegates

		public struct callbackTable_
		{
			public Callbacks.showPostLogin showPostLogin;
			public Callbacks.showCatalog showCatalog;
			public Callbacks.showQueryHit showQueryHit;
		};

		public callbackTable_ callbackTable;
		public static ClientUserInterface UI;

		private DataTable queryResultsTable;

		//For global access to the UI form to do callbacks from other threads to update the UI
		public static ClientUserInterface clientUserInterfaceInstance;
		private static Thread clientThread;

		#region UI variables
		private System.Windows.Forms.TextBox searchBox;
		private System.Windows.Forms.Label userNameLabel;
		private System.Windows.Forms.Label passwordLabel;
		private System.Windows.Forms.Label personalInfoDisclaimer;
		private System.Windows.Forms.Label prefixLabel;
		private System.Windows.Forms.ComboBox prefixBox;
		private System.Windows.Forms.Label firstNameLabel;
		private System.Windows.Forms.TextBox firstNameBox;
		private System.Windows.Forms.Label middleNameLabel;
		private System.Windows.Forms.TextBox middleNameBox;
		private System.Windows.Forms.Label LastNameLabel;
		private System.Windows.Forms.TextBox lastNameBox;
		private System.Windows.Forms.Label suffixLabel;
		private System.Windows.Forms.ComboBox suffixBox;
		private System.Windows.Forms.Button submitButton;
		private System.Windows.Forms.Label streetAddrLabel;
		private System.Windows.Forms.TextBox streetAddrBox;
		private System.Windows.Forms.Label cityLabel;
		private System.Windows.Forms.TextBox cityBox;
		private System.Windows.Forms.Label stateLabel;
		private System.Windows.Forms.ComboBox stateBox;
		private System.Windows.Forms.Label countryLabel;
		private System.Windows.Forms.ComboBox countryBox;
		private System.Windows.Forms.TextBox passwordBox;
		private System.Windows.Forms.TextBox userNameBox;
		private System.Windows.Forms.Label zipCodeLabel;
		private System.Windows.Forms.TextBox zipCodeBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		//Business Logic object
		private static Client client;
		private System.Windows.Forms.Label passwordLabel2;
		private System.Windows.Forms.TextBox passwordBox2;
		private System.Windows.Forms.Label settingsStatusLabel;
		private System.Windows.Forms.TabPage settingsPage;
		private System.Windows.Forms.TabPage loginPage;
		private System.Windows.Forms.Label loginPasswordLabel;
		private System.Windows.Forms.TextBox loginBox;
		private System.Windows.Forms.TextBox loginPasswordBox;
		private System.Windows.Forms.Label userNameLoginLabel;
		private System.Windows.Forms.Button loginButton;
		private System.Windows.Forms.CheckBox rememberUserNameAndPassword;
		private System.Windows.Forms.TabControl sectionsTab;
		private System.Windows.Forms.TabPage searchPage;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem fileMenuItem;
		private System.Windows.Forms.MenuItem exitMenuItem;
		private System.Windows.Forms.Button searchButton;
		private System.Windows.Forms.Panel ringsPanel;
		private System.Windows.Forms.TreeView ringsTree;
		private System.Windows.Forms.TreeView resourceTree;
		private System.Windows.Forms.TabPage ringsPage;
		private System.Windows.Forms.TabPage resourcesPage;
		private System.Windows.Forms.RichTextBox resourceInfo;
		private System.Windows.Forms.Label catalogProgressLabel;
		private System.Windows.Forms.ProgressBar catalogProgressBar;
		private System.Windows.Forms.Label ringNameLabel;
		private System.Windows.Forms.Label ringNameLabelText;
		private System.Windows.Forms.Label extensionLabel;
		private System.Windows.Forms.Label ringUserNameLabel;
		private System.Windows.Forms.Label ringPasswordLabel;
		private System.Windows.Forms.Label extensionLabelText;
		private System.Windows.Forms.Label ringUserNameLabelText;
		private System.Windows.Forms.Label ringPasswordLabelText;
		private System.Windows.Forms.Label progressCount;
		private System.Windows.Forms.Label numCatalogedLabel;
		private System.Windows.Forms.CheckBox musicCheckBox;
		private System.Windows.Forms.CheckBox documentCheckBox;
		private System.Windows.Forms.CheckBox graphicsCheckBox;
		private System.Windows.Forms.CheckBox videoCheckBox;
		private System.Windows.Forms.CheckBox audioCheckBox;
		private System.Windows.Forms.Label commPointLabel;
		private System.Windows.Forms.Label ringNeighborsLabel;
		private System.Windows.Forms.Label ringNeighborsLabelText;
		private System.Windows.Forms.DataGrid queryHitsGrid;
		private System.Windows.Forms.CheckBox binaryCheckBox;
		private System.Windows.Forms.TabPage browsePage;
		private AxSHDocVw.AxWebBrowser webBrowser;
		private System.Windows.Forms.TextBox browserAddressBar;
		private System.Windows.Forms.Label browserAddressLabel;
		private System.Windows.Forms.Button browserGoButton;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ToolBar browserToolBar;
		private System.Windows.Forms.ToolBarButton browserToolBarHomeButton;
		private System.Windows.Forms.ToolBarButton browserToolBarBackButton;
		private System.Windows.Forms.ToolBarButton browserToolBarForwardButton;
		private System.Windows.Forms.ToolBarButton browserToolBarStopButton;
		private System.Windows.Forms.ToolBarButton browserToolBarReloadButton;
		private System.Windows.Forms.StatusBar browserStatusBar;
		private System.Windows.Forms.MenuItem helpMenuItem;
		private System.Windows.Forms.MenuItem aboutMenuItem;
		
		#endregion

		public ClientUserInterface()
		{
			callbackTable.showPostLogin = new Callbacks.showPostLogin(showPostLogin);
			callbackTable.showCatalog = new Callbacks.showCatalog(showCatalog);
			callbackTable.showQueryHit = new Callbacks.showQueryHit(showQueryHit);

			InitializeComponent();
			//All tabs except login should be invisible
			this.sectionsTab.Controls.Remove(this.searchPage);
			this.sectionsTab.Controls.Remove(this.settingsPage);
			this.sectionsTab.Controls.Remove(this.ringsPage);
			this.sectionsTab.Controls.Remove(this.resourcesPage);
			this.sectionsTab.Controls.Remove(this.browsePage);

			//Register UI event handlers
			this.submitButton.Click += new System.EventHandler(this.submitSettings);
			this.exitMenuItem.Click += new System.EventHandler(this.exitHandler);
			this.aboutMenuItem.Click+= new System.EventHandler(this.aboutHandler);
			this.loginButton.Click += new System.EventHandler(this.loginHandler);
			this.searchButton.Click += new System.EventHandler(this.searchHandler);
			this.resourceTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.resourceSelectHandler);
			this.ringsTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ringSelectHandler);
			
			//Browser UI event handlers
			this.browserGoButton.Click += new EventHandler(this.browserGoButtonHandler);
			
			//setup UI data grids and tables
			queryResultsTable = new DataTable("queryResultsTable");
			queryResultsTable.Columns.Add(new DataColumn("Nickname", typeof(string)));
			queryResultsTable.Columns.Add(new DataColumn("FileID", typeof(ulong)));
			queryResultsTable.Columns.Add(new DataColumn("Filename", typeof(string)));
			queryResultsTable.Columns.Add(new DataColumn("Size", typeof(uint)));
			queryResultsTable.Columns.Add(new DataColumn("Download", typeof(ProgressBar)));
			
			DataColumn peerColumn = new DataColumn("Peer", typeof(Peer));
			peerColumn.ColumnMapping = MappingType.Hidden;
			queryResultsTable.Columns.Add(peerColumn);

			DataColumn ringInfoColumn = new DataColumn("RingInfo", typeof(RingInfo));
			ringInfoColumn.ColumnMapping = MappingType.Hidden;
			queryResultsTable.Columns.Add(ringInfoColumn);

			DataColumn resourceHeaderColumn = new DataColumn("ResourceHeader", typeof(ResourceHeader));
			resourceHeaderColumn.ColumnMapping = MappingType.Hidden;
			queryResultsTable.Columns.Add(resourceHeaderColumn);

			queryHitsGrid.Paint += new PaintEventHandler(paintQueryHitsDataGrid);

			DataSet queryResults = new DataSet("queryResults");
			
			queryResults.Tables.Add(queryResultsTable);
			this.queryHitsGrid.SetDataBinding(queryResults,"queryResultsTable");
			this.queryHitsGrid.ReadOnly = true;
			//this.queryHitsGrid.Enabled = false;
			this.queryHitsGrid.MouseUp += new System.Windows.Forms.MouseEventHandler(this.searchResultSelectHandler);
			this.queryHitsGrid.DoubleClick += new System.EventHandler(this.searchResultDoubleClickHandler);
			queryResultsTable.DefaultView.AllowNew = false;
			queryResultsTable.DefaultView.AllowEdit = true;
		}

		public bool settingsCorrect()
		{
			string errors = "";

			if(!passwordBox.Text.Equals(passwordBox2.Text))
			{
				errors += "You did not re-type your password correctly\n";
			}
			if(Convert.ToUInt32(zipCodeBox.Text) > Constants.MAXZIPCODE)
			{
				errors += "Zip code is out of range";
			}
			if(errors.CompareTo("") == 0)
				return true;

			MessageBox.Show(errors);
			return false;
		}

		public void submitSettings(object sender, System.EventArgs e)
		{
			if(!settingsCorrect())
				return;

			client.submitSettings(userNameBox.Text, prefixBox.Text, firstNameBox.Text, 
				middleNameBox.Text, lastNameBox.Text, suffixBox.Text, streetAddrBox.Text, cityBox.Text,
				Convert.ToUInt32(zipCodeBox.Text), stateBox.Text, countryBox.Text, 
				System.Text.Encoding.ASCII.GetBytes(passwordBox.Text));
			passwordBox.Clear();
			passwordBox2.Clear();
			settingsStatusLabel.Text = "Settings updated successfully";
			System.Timers.Timer resetSettingsStatusTimer = new System.Timers.Timer(5000);
			resetSettingsStatusTimer.Elapsed += new System.Timers.ElapsedEventHandler(resetSettingsStatus);
			resetSettingsStatusTimer.Start();
		}
		
		private void resetSettingsStatus(object sender, System.Timers.ElapsedEventArgs e)
		{
			settingsStatusLabel.Text = "";
		}

		private void showPostLogin()
		{
			this.sectionsTab.Controls.Add(this.searchPage);
			this.sectionsTab.Controls.Add(this.settingsPage);
			this.sectionsTab.Controls.Add(this.ringsPage);
			this.sectionsTab.Controls.Add(this.resourcesPage);
			this.sectionsTab.Controls.Add(this.browsePage);
			this.loginPage.Dispose();
			this.sectionsTab.Controls.Remove(this.loginPage);
			User member = User.getInstance();
			TreeNode ringNode;
			foreach (RingInfo ringInfo in member.ringsInfo)
			{
				ringNode = ringsTree.Nodes.Add(ringInfo.ring.ringName);
				ringNode.Tag = ringInfo;
			}
		}

		private void showCatalog(RingInfo ringInfo)
		{
			TreeNode ringNode, groupNode, theNode;

			if (ringInfo == null || ringInfo.cataloger == null || ringInfo.cataloger.groups == null)
				return;

			ringNode = resourceTree.Nodes.Add(ringInfo.ring.ringName);

			foreach (ResourceGroup group in ringInfo.cataloger.groups)
			{
				groupNode = ringNode.Nodes.Add(group.groupName);
				if (group is FileGroup)
				{
					IDictionaryEnumerator enumerator = ((FileGroup)group).resourceList.GetEnumerator();
					while(enumerator.MoveNext())
					{
						Resource resource = (Resource)enumerator.Value;
						theNode = groupNode.Nodes.Add(((FileInfo)resource.data).FullName);
						theNode.Tag = resource;
					}
				}
			}

			// REVISIT: This is temp for debugging, remove please. We should have at least one neighbor
			if(User.getInstance().ringsInfo[0].neighbors != null && User.getInstance().ringsInfo[0].neighbors.Count != 0)
			{
				string temp = commPointLabel.Text;
				commPointLabel.Text = commPointLabel.Text + "," + User.getInstance().ringsInfo[0].ring.ringName + ": (" + 
					((Neighbor)User.getInstance().ringsInfo[0].neighbors[0]).peer.node.asyncCommunicationPoint.Port + ", " +
					((Neighbor)User.getInstance().ringsInfo[0].neighbors[0]).peer.node.syncCommunicationPoint.Port + ")\n";
			}
			commPointLabel.Text = commPointLabel.Text + "IE: " + LibUtil.detokenize(
				LibUtil.InformationEntropyToStringArray(User.getInstance().ringsInfo[0].IE), ' ') + "\n";
		}
		
		public void showQueryHit (RingInfo ringInfo, Peer hitPeer, ResourceHeader resourceHeader)
		{
			DataRow dr = queryResultsTable.NewRow();
			
			dr[0] = hitPeer.userInfo.nickname;
			dr[1] = resourceHeader.resourceID;
			dr[2] = resourceHeader.name;
			dr[3] = resourceHeader.size;
			dr[4] = "";
			dr[5] = hitPeer;
			dr[6] = ringInfo;
			dr[7] = resourceHeader;
		
			this.queryResultsTable.Rows.Add(dr);
		}

		public void setupProgressBar(ProgressBar bar, int min, int max)
		{
			bar.Minimum = min;
			bar.Maximum = max;
		}

		public void updateProgressBar(ProgressBar bar)
		{
			updateProgressBar(bar, 1);
		}

		public void updateDownloadProgressBar(object obj1, object obj2)
		{
			ProgressBar bar = obj1 as ProgressBar;
			int amount = (int)((long)obj2);
			bar.Step = amount;
			bar.PerformStep();
			this.queryHitsGrid.Refresh();
		}

		public void updateProgressBar(ProgressBar bar, int amount)
		{
			bar.Step = amount;
			bar.PerformStep();
			progressCount.Text = bar.Value+"/"+bar.Maximum;
		}

		public void setupCatalogUIHandler(int min, int max)
		{
			setupProgressBar(catalogProgressBar, min, max);
		}

		public void updateCatalogUIHandler(int amount)
		{
			updateProgressBar(catalogProgressBar, amount);
		}

		private void paintQueryHitsDataGrid(object grid, PaintEventArgs e)
		{
			if (this.queryResultsTable.Rows.Count <= 0)
				return;

			foreach (DataRow dr in this.queryResultsTable.Rows)
			{
				object obj = dr[4];
				if (obj is string && ((string)obj) == "")
					return;
				e.Graphics.FillRectangle(new SolidBrush(this.queryHitsGrid.BackColor), ((ProgressBar)dr[4]).Bounds);
				int progress = ((ProgressBar)obj).Value;
				PaintProgressCellSolidBorderStyle(((ProgressBar)dr[4]).Bounds, (double)progress/((ProgressBar)obj).Maximum*100, e.Graphics);
			}
		}

		/// <summary>
		/// The drawing function for the solid border style progress bar
		/// </summary>
		/// <param name="rect"></param>
		private void PaintProgressCellSolidBorderStyle(Rectangle rect, double dValue, Graphics oGraphics)
		{
			if (dValue > 100.0)
				dValue = 100.0;

			// calculate the width of the progress
			double fWidth = (float)rect.Width;
			fWidth = fWidth / 100.0 * dValue;
			
			rect.Width = (int)fWidth;

			rect.Height -= 2;
			rect.Width -= 2;
			rect.Y += 1;
			rect.X += 1;

			// draw the border
			oGraphics.FillRectangle(Brushes.Black, rect);

			rect.Height -= 2;
			rect.Width -= 2;
			rect.Y += 1;
			rect.X += 1;

			// draw the progress
			Brush oBrush = new SolidBrush(Color.BlanchedAlmond);

			oGraphics.FillRectangle(oBrush, rect);

			oBrush.Dispose();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			if(clientThread != null)
			{
				clientThread.Abort();
				clientThread = null;
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ClientUserInterface));
			this.sectionsTab = new System.Windows.Forms.TabControl();
			this.loginPage = new System.Windows.Forms.TabPage();
			this.rememberUserNameAndPassword = new System.Windows.Forms.CheckBox();
			this.loginButton = new System.Windows.Forms.Button();
			this.loginPasswordBox = new System.Windows.Forms.TextBox();
			this.loginBox = new System.Windows.Forms.TextBox();
			this.loginPasswordLabel = new System.Windows.Forms.Label();
			this.userNameLoginLabel = new System.Windows.Forms.Label();
			this.resourcesPage = new System.Windows.Forms.TabPage();
			this.numCatalogedLabel = new System.Windows.Forms.Label();
			this.progressCount = new System.Windows.Forms.Label();
			this.catalogProgressLabel = new System.Windows.Forms.Label();
			this.catalogProgressBar = new System.Windows.Forms.ProgressBar();
			this.resourceInfo = new System.Windows.Forms.RichTextBox();
			this.resourceTree = new System.Windows.Forms.TreeView();
			this.searchPage = new System.Windows.Forms.TabPage();
			this.binaryCheckBox = new System.Windows.Forms.CheckBox();
			this.commPointLabel = new System.Windows.Forms.Label();
			this.graphicsCheckBox = new System.Windows.Forms.CheckBox();
			this.documentCheckBox = new System.Windows.Forms.CheckBox();
			this.videoCheckBox = new System.Windows.Forms.CheckBox();
			this.audioCheckBox = new System.Windows.Forms.CheckBox();
			this.musicCheckBox = new System.Windows.Forms.CheckBox();
			this.queryHitsGrid = new System.Windows.Forms.DataGrid();
			this.searchButton = new System.Windows.Forms.Button();
			this.searchBox = new System.Windows.Forms.TextBox();
			this.ringsPage = new System.Windows.Forms.TabPage();
			this.ringNeighborsLabelText = new System.Windows.Forms.Label();
			this.ringNeighborsLabel = new System.Windows.Forms.Label();
			this.ringPasswordLabelText = new System.Windows.Forms.Label();
			this.ringPasswordLabel = new System.Windows.Forms.Label();
			this.ringUserNameLabelText = new System.Windows.Forms.Label();
			this.ringUserNameLabel = new System.Windows.Forms.Label();
			this.extensionLabelText = new System.Windows.Forms.Label();
			this.extensionLabel = new System.Windows.Forms.Label();
			this.ringNameLabelText = new System.Windows.Forms.Label();
			this.ringNameLabel = new System.Windows.Forms.Label();
			this.ringsPanel = new System.Windows.Forms.Panel();
			this.ringsTree = new System.Windows.Forms.TreeView();
			this.settingsPage = new System.Windows.Forms.TabPage();
			this.settingsStatusLabel = new System.Windows.Forms.Label();
			this.passwordBox2 = new System.Windows.Forms.TextBox();
			this.passwordLabel2 = new System.Windows.Forms.Label();
			this.zipCodeBox = new System.Windows.Forms.TextBox();
			this.zipCodeLabel = new System.Windows.Forms.Label();
			this.countryBox = new System.Windows.Forms.ComboBox();
			this.countryLabel = new System.Windows.Forms.Label();
			this.stateBox = new System.Windows.Forms.ComboBox();
			this.stateLabel = new System.Windows.Forms.Label();
			this.cityBox = new System.Windows.Forms.TextBox();
			this.cityLabel = new System.Windows.Forms.Label();
			this.streetAddrBox = new System.Windows.Forms.TextBox();
			this.streetAddrLabel = new System.Windows.Forms.Label();
			this.submitButton = new System.Windows.Forms.Button();
			this.suffixBox = new System.Windows.Forms.ComboBox();
			this.suffixLabel = new System.Windows.Forms.Label();
			this.lastNameBox = new System.Windows.Forms.TextBox();
			this.LastNameLabel = new System.Windows.Forms.Label();
			this.middleNameBox = new System.Windows.Forms.TextBox();
			this.middleNameLabel = new System.Windows.Forms.Label();
			this.firstNameBox = new System.Windows.Forms.TextBox();
			this.firstNameLabel = new System.Windows.Forms.Label();
			this.prefixBox = new System.Windows.Forms.ComboBox();
			this.prefixLabel = new System.Windows.Forms.Label();
			this.personalInfoDisclaimer = new System.Windows.Forms.Label();
			this.passwordBox = new System.Windows.Forms.TextBox();
			this.passwordLabel = new System.Windows.Forms.Label();
			this.userNameLabel = new System.Windows.Forms.Label();
			this.userNameBox = new System.Windows.Forms.TextBox();
			this.browsePage = new System.Windows.Forms.TabPage();
			this.webBrowser = new AxSHDocVw.AxWebBrowser();
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.fileMenuItem = new System.Windows.Forms.MenuItem();
			this.exitMenuItem = new System.Windows.Forms.MenuItem();
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
			// 
			// loginPage
			// 
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

		}
		#endregion
		
		#region main method (Entry point of ResourceRing)
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			try
			{
				clientUserInterfaceInstance = new ClientUserInterface();
				UI = clientUserInterfaceInstance;
				client = new Client(clientUserInterfaceInstance.callbackTable);
				
				Application.ApplicationExit += new EventHandler(client.exitHandler);

				clientThread = new Thread(new ThreadStart(client.start));	
				clientThread.Name = "Background Client->Server";
				clientThread.IsBackground = true;
				clientThread.Start();

				/* uncomment this out when not in debug mode, need to add flags?
				Process thisProcessInstance = Process.GetCurrentProcess();

				Process [] AllProcessInstances = Process.GetProcessesByName(thisProcessInstance.ProcessName);

				if (AllProcessInstances.Length > 1)
				{
					MessageBox.Show(thisProcessInstance.ProcessName + " is already running", thisProcessInstance.ProcessName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				*/
				Application.Run(clientUserInterfaceInstance);
				
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}
		}

		#endregion

		#region event handlers
		private void searchResultSelectHandler (object Sender, System.Windows.Forms.MouseEventArgs e)
		{
			System.Drawing.Point pt = new Point(e.X, e.Y); 
			DataGrid.HitTestInfo hti = this.queryHitsGrid.HitTest(pt); 
 
			if(hti.Type == DataGrid.HitTestType.Cell) 
			{ 
				this.queryHitsGrid.CurrentCell = new DataGridCell(hti.Row, hti.Column); 
				this.queryHitsGrid.Select(hti.Row); 
			} 
		}

		private void searchResultDoubleClickHandler (object Sender, System.EventArgs e)
		{
			int selectIndex = this.queryHitsGrid.CurrentRowIndex;
			DataRow dr = queryResultsTable.Rows[selectIndex];

			Peer peer = (Peer)dr[5];
			RingInfo ringInfo = (RingInfo)dr[6];
			ResourceHeader header = (ResourceHeader)dr[7];

			ProgressBar downloadBar = new ProgressBar();
			dr[4] = downloadBar;
			downloadBar.Maximum = (int)header.size;
			Rectangle cellBound = queryHitsGrid.GetCellBounds(selectIndex, 4);
			downloadBar.Bounds = cellBound;
			downloadBar.Location = new Point(cellBound.X, cellBound.Y);
			downloadBar.Name = "downloadProgressBar";
			downloadBar.Size = new Size(cellBound.Width, cellBound.Height);
			downloadBar.Visible = true;
			

			UpdateUIHandler downloadProgressBarUpdate = new UpdateUIHandler(updateDownloadProgressBar);
			UICallBack progressBarUpdate = new UICallBack(downloadProgressBarUpdate, downloadBar);

			client.RetreiveResource(peer, ringInfo, header, progressBarUpdate);

			//MessageBox.Show("downloading " + header.name + "...");
		}

		/// <summary>
		///  Called only when login info is not saved to handle the login page
		/// </summary>
		private void loginHandler(object Sender, System.EventArgs e)
		{
			if(rememberUserNameAndPassword.Checked)
			{
				//save the info to disk
			}
			
			User theUser = User.getInstance();
			commPointLabel.Text = "Node: (" + theUser.node.asyncCommunicationPoint.Port.ToString() + ", " +
				theUser.node.syncCommunicationPoint.Port.ToString() + ")\n";

			foreach (RingInfo ringInfo in theUser.ringsInfo)
			{
				ringInfo.userName = loginBox.Text;
				ringInfo.password = System.Text.ASCIIEncoding.ASCII.GetBytes(loginPasswordBox.Text);
				ringInfo.IE = new InformationEntropy[1];
				ringInfo.IE[0] = new InformationEntropy(ringInfo.ring.ringName, 1.0f);
			}

			//Login in the background and start acting as a responsible peer(serve others)	
			clientThread.Start();
		}

		private void searchHandler(object sender, System.EventArgs e)
		{
			int[] selectedRings = new int[Constants.NUM_RINGS];
			uint index = 0;
			if(audioCheckBox.Checked)
				selectedRings[index++] = (int)Constants.AUDIO_RING_ID;
			if(videoCheckBox.Checked)
				selectedRings[index++] = (int)Constants.VIDEO_RING_ID;
			if(graphicsCheckBox.Checked)
				selectedRings[index++] = (int)Constants.GRAPHICS_RING_ID;
			if(documentCheckBox.Checked)
				selectedRings[index++] = (int)Constants.DOCUMENTS_RING_ID;
			if(binaryCheckBox.Checked)
				selectedRings[index++] = (int)Constants.BINARY_RING_ID;

			//set the rest to -1 so they don't get processed
			for(uint remainder = index; remainder < selectedRings.Length; remainder++)
				selectedRings[remainder] = -1;
			//REVISIT: make sure the list of ring checkboxes are loaded based on subscription. 
			//Also, add sub-catagories(i.e. columns) for the ones that are configured that way (e.g. artist, album, etc.)
			client.search(selectedRings, null, searchBox.Text);
		}

		private void exitHandler(object sender, System.EventArgs e)
		{
			client.exitHandler(sender, e);
			Application.Exit();
		}

		private void aboutHandler(object sender, System.EventArgs e)
		{
			MessageBox.Show("ResourceRing 0.1\n\nCopyright 2003, Entropy Inc");
		}

		private void resourceSelectHandler(object sender, TreeViewEventArgs e)
		{
			if(e.Node.Tag == null ||! (e.Node.Tag is Resource))
				return;
			Resource selectedResource = (Resource)e.Node.Tag;
			this.resourceInfo.Clear();
			this.resourceInfo.Text = "Description: " + selectedResource.header.description + "\n\n" +
				"Information Entropy: " + InformationEntropy.Detokenize(selectedResource.IE, ',') + "\n\n" +
				"Size: " + LibUtil.formatSize(selectedResource.header.size) + "\n\n" + "Tokens: " +
				selectedResource.header.tokens;
		}

		private void ringSelectHandler(object sender, TreeViewEventArgs e)
		{
			if(e.Node.Tag == null ||! (e.Node.Tag is RingInfo))
				return;
			RingInfo ringInfo = (RingInfo)e.Node.Tag;
			this.ringNameLabelText.Text = ringInfo.ring.ringName;
			this.extensionLabelText.Text = LibUtil.detokenize(ringInfo.ring.extensions, ',');
			this.ringUserNameLabelText.Text = ringInfo.userName;
			this.ringPasswordLabelText.Text = System.Text.ASCIIEncoding.ASCII.GetString(ringInfo.password);
			this.ringNeighborsLabelText.Text = "";
			foreach (Neighbor neighbor in ringInfo.neighbors)
				this.ringNeighborsLabelText.Text += neighbor.peer.userInfo.nickname + " ";
		}

		private void Search_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}

		private void searchResultList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}

		private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
		
		}

		private void queryHitsGrid_Navigate(object sender, System.Windows.Forms.NavigateEventArgs ne)
		{
		
		}

		private void checkBox1_CheckedChanged(object sender, System.EventArgs e)
		{
			
		}

		private void webBrowser_Enter(object sender, System.EventArgs e)
		{
		
		}

		private void browserGoButtonHandler(object sender, System.EventArgs e)
		{
			object nullObject = null;
			Cursor.Current = Cursors.WaitCursor;
			this.browserStatusBar.Text = "Opening " + this.browserAddressBar.Text;
			webBrowser.Navigate(this.browserAddressBar.Text, ref nullObject, ref nullObject, ref nullObject, ref nullObject);
			Cursor.Current = Cursors.Default;
			this.browserStatusBar.Text = "";
		}		

		private void browserToolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == this.browserToolBarBackButton)
			{
				webBrowser.GoBack();
			} 
			else if (e.Button == this.browserToolBarForwardButton)
			{
				webBrowser.GoForward();
			}
			else if (e.Button == this.browserToolBarHomeButton)
			{
				webBrowser.GoHome();
			}
			else if (e.Button == this.browserToolBarStopButton)
			{
				webBrowser.Stop();
			}
			else if (e.Button == this.browserToolBarReloadButton)
			{
				webBrowser.Refresh();
			}
		}

		#endregion
	}
}
