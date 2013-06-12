using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Forms.Design;
using System.Data;
using RRLib;

namespace ResourceRingServer
{
	/// <summary>
	/// Summary description for ServerUserInterface.
	/// </summary>
	public class ServerUserInterface : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TabPage clientListPage;
		private System.Windows.Forms.DataGrid clientsDataGrid;
		private System.Windows.Forms.Label numClientsLabel;
		private System.Windows.Forms.Label numClients;
		private System.Windows.Forms.TabControl clientListTab;


		/// <summary>
		/// Defines the delegates for changing the user interface layer
		/// </summary>
		public class Callbacks
		{
			public delegate void addClientDelegate(Member newClient);
			public delegate void removeClientDelegate(Member existingClient);
			public delegate void updateClientDelegate(Member existingClient);
		}
	
		// Table of user interface delegates

		public struct callbackTable_
		{
			public Callbacks.addClientDelegate addNewClient;
			public Callbacks.removeClientDelegate removeExistingClient;
			public Callbacks.updateClientDelegate updateExistingClient;
		};

		public callbackTable_ callbackTable;
		private DataTable clientsTable;
		
		//For global access to the UI form to do callbacks from other threads to update the UI
		public static ServerUserInterface serverUserInterfaceInstance;
		private static Thread serverThread;

		public ServerUserInterface()
		{
			//initialize callbacks
			callbackTable.addNewClient = new Callbacks.addClientDelegate(addNewClient);
			callbackTable.removeExistingClient = new Callbacks.removeClientDelegate(removeExistingClient);
			callbackTable.updateExistingClient = new Callbacks.updateClientDelegate(updateExistingClient);
			InitializeComponent();
			setupDataGrid();
		}

		private void setupDataGrid()
		{
			DataSet clientsSet = new DataSet("clientsSet");
			clientsTable = new DataTable("clientsTable");
			clientsTable.Columns.Add(new DataColumn("User ID", typeof(uint)));
			clientsTable.Columns.Add(new DataColumn("User Name", typeof(string)));
			clientsTable.Columns.Add(new DataColumn("IP Address", typeof(string)));
			clientsTable.Columns.Add(new DataColumn("Real Name", typeof(string)));
			clientsTable.Columns.Add(new DataColumn("Street Address", typeof(string)));
			clientsTable.Columns.Add(new DataColumn("City", typeof(string)));
			clientsTable.Columns.Add(new DataColumn("State", typeof(string)));
			clientsTable.Columns.Add(new DataColumn("Country", typeof(string)));

			clientsSet.Tables.Add(clientsTable);
			clientsDataGrid.SetDataBinding(clientsSet,"clientsTable");
			clientsDataGrid.ReadOnly = true;
		}

		private DataRow getClientRow(Member client)
		{
			PublicUserInfo publicUserInfo = client.publicUserInfo;
			PrivateUserInfo privateUserInfo = client.privateUserInfo;

			DataRow dr = clientsTable.NewRow();
			dr[0] = privateUserInfo.userID;
			dr[1] = client.userName;
			dr[2] = (new System.Net.IPAddress(client.node.syncCommunicationPoint.Address)).ToString();
			dr[3] = privateUserInfo.prefix+" "+privateUserInfo.firstName + 
				" " + privateUserInfo.middleName + " " + privateUserInfo.lastName +
				" " + privateUserInfo.suffix;
			dr[4] = privateUserInfo.streetAddress;
			dr[5] = privateUserInfo.city;
			dr[6] = privateUserInfo.state;
			dr[7] = privateUserInfo.country;

			System.Text.Encoding.ASCII.GetString(client.password);
			return dr;
		}

		public void addNewClient(Member newClient)
		{
			//update connection list	
			updateExistingClient(newClient);			
			numClients.Text = Convert.ToString((Convert.ToUInt32(numClients.Text)+1));
		}

		public void updateExistingClient(Member existingClient)
		{
			int index = 0;
			bool found = false;
			//update connection list
			foreach(DataRow dr in clientsTable.Rows)
			{
				if((uint)dr[0] == existingClient.privateUserInfo.userID)
				{
					clientsTable.Rows.InsertAt(getClientRow(existingClient), index);
					clientsTable.Rows.Remove(dr);
					numClients.Text = Convert.ToString((Convert.ToUInt32(numClients.Text)-1));
					found = true;
					break;
				}
				index++;
			}
			if (!found)
				clientsTable.Rows.Add(getClientRow(existingClient));
		}

		public void removeExistingClient(Member existingClient)
		{
			//update connection list
			foreach(DataRow dr in clientsTable.Rows)
			{
				if((uint)dr[0] == existingClient.privateUserInfo.userID)
				{
					clientsTable.Rows.Remove(dr);
					numClients.Text = Convert.ToString((Convert.ToUInt32(numClients.Text)-1));
					break;
				}
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			if(serverThread != null)
			{
				serverThread.Interrupt();
				serverThread = null;
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
			this.clientListPage = new System.Windows.Forms.TabPage();
			this.clientsDataGrid = new System.Windows.Forms.DataGrid();
			this.numClientsLabel = new System.Windows.Forms.Label();
			this.numClients = new System.Windows.Forms.Label();
			this.clientListTab = new System.Windows.Forms.TabControl();
			this.clientListPage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.clientsDataGrid)).BeginInit();
			this.clientListTab.SuspendLayout();
			this.SuspendLayout();
			// 
			// clientListPage
			// 
			this.clientListPage.Controls.AddRange(new System.Windows.Forms.Control[] {
																						 this.clientsDataGrid,
																						 this.numClientsLabel,
																						 this.numClients});
			this.clientListPage.Location = new System.Drawing.Point(4, 22);
			this.clientListPage.Name = "clientListPage";
			this.clientListPage.Size = new System.Drawing.Size(648, 598);
			this.clientListPage.TabIndex = 0;
			this.clientListPage.Text = "Clients";
			// 
			// clientsDataGrid
			// 
			this.clientsDataGrid.CaptionText = "Connected Clients";
			this.clientsDataGrid.DataMember = "";
			this.clientsDataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.clientsDataGrid.Location = new System.Drawing.Point(8, 40);
			this.clientsDataGrid.Name = "clientsDataGrid";
			this.clientsDataGrid.ReadOnly = true;
			this.clientsDataGrid.Size = new System.Drawing.Size(632, 536);
			this.clientsDataGrid.TabIndex = 6;
			// 
			// numClientsLabel
			// 
			this.numClientsLabel.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.numClientsLabel.Location = new System.Drawing.Point(16, 16);
			this.numClientsLabel.Name = "numClientsLabel";
			this.numClientsLabel.Size = new System.Drawing.Size(168, 23);
			this.numClientsLabel.TabIndex = 2;
			this.numClientsLabel.Text = "Number Of Clients Connected =";
			// 
			// numClients
			// 
			this.numClients.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.numClients.Location = new System.Drawing.Point(184, 16);
			this.numClients.Name = "numClients";
			this.numClients.Size = new System.Drawing.Size(16, 16);
			this.numClients.TabIndex = 5;
			this.numClients.Text = "0";
			// 
			// clientListTab
			// 
			this.clientListTab.Controls.AddRange(new System.Windows.Forms.Control[] {
																						this.clientListPage});
			this.clientListTab.ItemSize = new System.Drawing.Size(43, 18);
			this.clientListTab.Name = "clientListTab";
			this.clientListTab.SelectedIndex = 0;
			this.clientListTab.Size = new System.Drawing.Size(656, 624);
			this.clientListTab.TabIndex = 6;
			// 
			// ServerUserInterface
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(656, 622);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.clientListTab});
			this.Name = "ServerUserInterface";
			this.Text = "Server";
			this.clientListPage.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.clientsDataGrid)).EndInit();
			this.clientListTab.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private static void Main(string[] argv)
		{
			try
			{
				serverUserInterfaceInstance = new ServerUserInterface();
				Server server = new Server(serverUserInterfaceInstance.callbackTable);
				//Start the server in the background
				serverThread = new Thread(new ThreadStart(server.start));
				serverThread.IsBackground = true;
				serverThread.Name = "Background Server(process requests)";
				serverThread.Start();

				Application.ApplicationExit += new EventHandler(server.exitHandler);
				Application.Run(serverUserInterfaceInstance);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}
		}

		private void process1_Exited(object sender, System.EventArgs e)
		{
		
		}
	}
}
