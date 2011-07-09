namespace RealJabber
{
    partial class FrmMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.jabberClient = new jabber.client.JabberClient(this.components);
            this.panelCredentials = new System.Windows.Forms.Panel();
            this.lblLine4 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbServer = new System.Windows.Forms.ComboBox();
            this.lblServer = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.btnSignin = new System.Windows.Forms.Button();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblLine3 = new System.Windows.Forms.Label();
            this.lblLine1 = new System.Windows.Forms.Label();
            this.lblLine2 = new System.Windows.Forms.Label();
            this.panelContact = new System.Windows.Forms.Panel();
            this.rosterTree = new muzzle.RosterTree();
            this.xmppDebugger1 = new muzzle.XmppDebugger();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblUser = new System.Windows.Forms.Label();
            this.lblPresence = new System.Windows.Forms.Label();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.btnSignOut = new System.Windows.Forms.Button();
            this.panelCredentials.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panelContact.SuspendLayout();
            this.SuspendLayout();
            // 
            // jabberClient
            // 
            this.jabberClient.AutoReconnect = 30F;
            this.jabberClient.AutoStartCompression = true;
            this.jabberClient.AutoStartTLS = true;
            this.jabberClient.InvokeControl = this;
            this.jabberClient.KeepAlive = 30F;
            this.jabberClient.LocalCertificate = null;
            this.jabberClient.Password = null;
            this.jabberClient.User = null;
            // 
            // panelCredentials
            // 
            this.panelCredentials.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelCredentials.Controls.Add(this.lblLine4);
            this.panelCredentials.Controls.Add(this.richTextBox1);
            this.panelCredentials.Controls.Add(this.groupBox1);
            this.panelCredentials.Controls.Add(this.lblLine3);
            this.panelCredentials.Controls.Add(this.lblLine1);
            this.panelCredentials.Controls.Add(this.lblLine2);
            this.panelCredentials.Location = new System.Drawing.Point(12, 12);
            this.panelCredentials.Name = "panelCredentials";
            this.panelCredentials.Size = new System.Drawing.Size(263, 476);
            this.panelCredentials.TabIndex = 0;
            // 
            // lblLine4
            // 
            this.lblLine4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLine4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLine4.Location = new System.Drawing.Point(0, 63);
            this.lblLine4.Name = "lblLine4";
            this.lblLine4.Size = new System.Drawing.Size(263, 17);
            this.lblLine4.TabIndex = 20;
            this.lblLine4.Text = "Build ###";
            this.lblLine4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Location = new System.Drawing.Point(3, 266);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.richTextBox1.Size = new System.Drawing.Size(257, 186);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            this.richTextBox1.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBox1_LinkClicked);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cbServer);
            this.groupBox1.Controls.Add(this.lblServer);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.lblUserName);
            this.groupBox1.Controls.Add(this.btnSignin);
            this.groupBox1.Controls.Add(this.txtUserName);
            this.groupBox1.Controls.Add(this.lblPassword);
            this.groupBox1.Location = new System.Drawing.Point(3, 94);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(257, 136);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            // 
            // cbServer
            // 
            this.cbServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbServer.FormattingEnabled = true;
            this.cbServer.Items.AddRange(new object[] {
            "talk.l.google.com",
            "chat.facebook.com",
            "jabber.org",
            "localhost"});
            this.cbServer.Location = new System.Drawing.Point(95, 72);
            this.cbServer.Name = "cbServer";
            this.cbServer.Size = new System.Drawing.Size(152, 21);
            this.cbServer.TabIndex = 2;
            this.cbServer.Text = "talk.l.google.com";
            this.cbServer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cbServer_KeyDown);
            // 
            // lblServer
            // 
            this.lblServer.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblServer.Location = new System.Drawing.Point(6, 72);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(82, 20);
            this.lblServer.TabIndex = 10;
            this.lblServer.Text = "Server:";
            this.lblServer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(94, 46);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(153, 20);
            this.txtPassword.TabIndex = 1;
            this.txtPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPassword_KeyDown);
            // 
            // lblUserName
            // 
            this.lblUserName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserName.Location = new System.Drawing.Point(6, 20);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(82, 20);
            this.lblUserName.TabIndex = 8;
            this.lblUserName.Text = "Username:";
            this.lblUserName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnSignin
            // 
            this.btnSignin.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSignin.Location = new System.Drawing.Point(93, 99);
            this.btnSignin.Name = "btnSignin";
            this.btnSignin.Size = new System.Drawing.Size(87, 27);
            this.btnSignin.TabIndex = 3;
            this.btnSignin.Text = "Sign In";
            this.btnSignin.UseVisualStyleBackColor = true;
            this.btnSignin.Click += new System.EventHandler(this.btnSignin_Click);
            // 
            // txtUserName
            // 
            this.txtUserName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserName.Location = new System.Drawing.Point(94, 20);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(153, 20);
            this.txtUserName.TabIndex = 0;
            // 
            // lblPassword
            // 
            this.lblPassword.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPassword.Location = new System.Drawing.Point(6, 46);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(82, 20);
            this.lblPassword.TabIndex = 9;
            this.lblPassword.Text = "Password:";
            this.lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblLine3
            // 
            this.lblLine3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLine3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLine3.Location = new System.Drawing.Point(0, 46);
            this.lblLine3.Name = "lblLine3";
            this.lblLine3.Size = new System.Drawing.Size(263, 17);
            this.lblLine3.TabIndex = 12;
            this.lblLine3.Text = "XMPP In-Band Real Time Text";
            this.lblLine3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLine1
            // 
            this.lblLine1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLine1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLine1.Location = new System.Drawing.Point(0, 5);
            this.lblLine1.Name = "lblLine1";
            this.lblLine1.Size = new System.Drawing.Size(263, 17);
            this.lblLine1.TabIndex = 11;
            this.lblLine1.Text = "Mark Rejhon\'s";
            this.lblLine1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLine2
            // 
            this.lblLine2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLine2.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLine2.Location = new System.Drawing.Point(0, 22);
            this.lblLine2.Name = "lblLine2";
            this.lblLine2.Size = new System.Drawing.Size(263, 24);
            this.lblLine2.TabIndex = 10;
            this.lblLine2.Text = "Jabber / Google Talk";
            this.lblLine2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelContact
            // 
            this.panelContact.Controls.Add(this.rosterTree);
            this.panelContact.Controls.Add(this.xmppDebugger1);
            this.panelContact.Controls.Add(this.lblStatus);
            this.panelContact.Controls.Add(this.lblUser);
            this.panelContact.Controls.Add(this.lblPresence);
            this.panelContact.Controls.Add(this.txtStatus);
            this.panelContact.Controls.Add(this.btnSignOut);
            this.panelContact.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContact.Location = new System.Drawing.Point(0, 0);
            this.panelContact.Name = "panelContact";
            this.panelContact.Size = new System.Drawing.Size(287, 500);
            this.panelContact.TabIndex = 6;
            this.panelContact.Visible = false;
            this.panelContact.Click += new System.EventHandler(this.panelContact_Click);
            // 
            // rosterTree
            // 
            this.rosterTree.AllowDrop = true;
            this.rosterTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rosterTree.Client = this.jabberClient;
            this.rosterTree.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.rosterTree.ImageIndex = 1;
            this.rosterTree.Location = new System.Drawing.Point(8, 47);
            this.rosterTree.Name = "rosterTree";
            this.rosterTree.PresenceManager = null;
            this.rosterTree.RosterManager = null;
            this.rosterTree.SelectedImageIndex = 0;
            this.rosterTree.ShowLines = false;
            this.rosterTree.ShowRootLines = false;
            this.rosterTree.Size = new System.Drawing.Size(271, 417);
            this.rosterTree.Sorted = true;
            this.rosterTree.StatusColor = System.Drawing.Color.Teal;
            this.rosterTree.TabIndex = 5;
            this.rosterTree.Click += new System.EventHandler(this.rosterTree_Click);
            // 
            // xmppDebugger1
            // 
            this.xmppDebugger1.ErrorColor = System.Drawing.Color.Red;
            this.xmppDebugger1.Location = new System.Drawing.Point(50, 137);
            this.xmppDebugger1.Name = "xmppDebugger1";
            this.xmppDebugger1.OtherColor = System.Drawing.Color.Green;
            this.xmppDebugger1.ReceiveColor = System.Drawing.Color.Orange;
            this.xmppDebugger1.SendColor = System.Drawing.Color.Blue;
            this.xmppDebugger1.Size = new System.Drawing.Size(150, 150);
            this.xmppDebugger1.Stream = this.jabberClient;
            this.xmppDebugger1.TabIndex = 8;
            this.xmppDebugger1.TextColor = System.Drawing.SystemColors.WindowText;
            this.xmppDebugger1.Visible = false;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblStatus.Location = new System.Drawing.Point(8, 26);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(271, 20);
            this.lblStatus.TabIndex = 7;
            this.lblStatus.Text = "Click to enter your status message";
            this.lblStatus.Click += new System.EventHandler(this.labelStatus_Click);
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUser.Location = new System.Drawing.Point(8, 8);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(33, 13);
            this.lblUser.TabIndex = 6;
            this.lblUser.Text = "User";
            // 
            // lblPresence
            // 
            this.lblPresence.Location = new System.Drawing.Point(207, 8);
            this.lblPresence.Name = "lblPresence";
            this.lblPresence.Size = new System.Drawing.Size(56, 13);
            this.lblPresence.TabIndex = 4;
            this.lblPresence.Text = "Offline";
            this.lblPresence.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtStatus
            // 
            this.txtStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStatus.Location = new System.Drawing.Point(8, 24);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(271, 20);
            this.txtStatus.TabIndex = 3;
            this.txtStatus.Visible = false;
            this.txtStatus.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtStatus_KeyDown);
            // 
            // btnSignOut
            // 
            this.btnSignOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSignOut.Location = new System.Drawing.Point(192, 470);
            this.btnSignOut.Name = "btnSignOut";
            this.btnSignOut.Size = new System.Drawing.Size(88, 24);
            this.btnSignOut.TabIndex = 0;
            this.btnSignOut.Text = "Sign Out";
            this.btnSignOut.UseVisualStyleBackColor = true;
            this.btnSignOut.Click += new System.EventHandler(this.btnSignOut_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(287, 500);
            this.Controls.Add(this.panelCredentials);
            this.Controls.Add(this.panelContact);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(270, 276);
            this.Name = "FrmMain";
            this.Text = "RealJabber -- Real Time Text over XMPP";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.panelCredentials.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panelContact.ResumeLayout(false);
            this.panelContact.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelCredentials;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Button btnSignin;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Panel panelContact;
        private System.Windows.Forms.Button btnSignOut;
        private System.Windows.Forms.Label lblPresence;
        private System.Windows.Forms.TextBox txtStatus;
        private muzzle.RosterTree rosterTree;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblStatus;
        private jabber.client.JabberClient jabberClient;
        private muzzle.XmppDebugger xmppDebugger1;
        private System.Windows.Forms.Label lblLine2;
        private System.Windows.Forms.Label lblLine3;
        private System.Windows.Forms.Label lblLine1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label lblLine4;
        private System.Windows.Forms.ComboBox cbServer;
    }
}

