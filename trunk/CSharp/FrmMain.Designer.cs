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
            this.groupBoxAbout = new System.Windows.Forms.GroupBox();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblAbout = new System.Windows.Forms.Label();
            this.richTextBoxAbout = new System.Windows.Forms.RichTextBox();
            this.groupBoxLogin = new System.Windows.Forms.GroupBox();
            this.cbServer = new System.Windows.Forms.ComboBox();
            this.lblServer = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.btnSignin = new System.Windows.Forms.Button();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblLine2 = new System.Windows.Forms.Label();
            this.lblLine1 = new System.Windows.Forms.Label();
            this.labelUser = new System.Windows.Forms.Label();
            this.panelContact = new System.Windows.Forms.Panel();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.rosterTree = new muzzle.RosterTree();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblUser = new System.Windows.Forms.Label();
            this.lblPresence = new System.Windows.Forms.Label();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.btnSignOut = new System.Windows.Forms.Button();
            this.panelCredentials.SuspendLayout();
            this.groupBoxAbout.SuspendLayout();
            this.groupBoxLogin.SuspendLayout();
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
            this.panelCredentials.Controls.Add(this.groupBoxAbout);
            this.panelCredentials.Controls.Add(this.groupBoxLogin);
            this.panelCredentials.Controls.Add(this.lblLine2);
            this.panelCredentials.Controls.Add(this.lblLine1);
            this.panelCredentials.Controls.Add(this.labelUser);
            this.panelCredentials.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCredentials.Location = new System.Drawing.Point(0, 0);
            this.panelCredentials.Name = "panelCredentials";
            this.panelCredentials.Size = new System.Drawing.Size(262, 528);
            this.panelCredentials.TabIndex = 0;
            // 
            // groupBoxAbout
            // 
            this.groupBoxAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxAbout.Controls.Add(this.lblVersion);
            this.groupBoxAbout.Controls.Add(this.lblAbout);
            this.groupBoxAbout.Controls.Add(this.richTextBoxAbout);
            this.groupBoxAbout.Location = new System.Drawing.Point(12, 226);
            this.groupBoxAbout.Name = "groupBoxAbout";
            this.groupBoxAbout.Size = new System.Drawing.Size(238, 288);
            this.groupBoxAbout.TabIndex = 25;
            this.groupBoxAbout.TabStop = false;
            this.groupBoxAbout.Text = "About This App";
            // 
            // lblVersion
            // 
            this.lblVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVersion.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.ForeColor = System.Drawing.Color.Black;
            this.lblVersion.Location = new System.Drawing.Point(1, 43);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(235, 16);
            this.lblVersion.TabIndex = 24;
            this.lblVersion.Text = "Version ###";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAbout
            // 
            this.lblAbout.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAbout.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAbout.ForeColor = System.Drawing.Color.Black;
            this.lblAbout.Location = new System.Drawing.Point(1, 27);
            this.lblAbout.Name = "lblAbout";
            this.lblAbout.Size = new System.Drawing.Size(236, 18);
            this.lblAbout.TabIndex = 23;
            this.lblAbout.Text = "Developer Experimental Demo";
            this.lblAbout.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // richTextBoxAbout
            // 
            this.richTextBoxAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxAbout.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBoxAbout.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxAbout.Location = new System.Drawing.Point(1, 78);
            this.richTextBoxAbout.Name = "richTextBoxAbout";
            this.richTextBoxAbout.ReadOnly = true;
            this.richTextBoxAbout.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.richTextBoxAbout.Size = new System.Drawing.Size(235, 204);
            this.richTextBoxAbout.TabIndex = 2;
            this.richTextBoxAbout.Text = resources.GetString("richTextBoxAbout.Text");
            this.richTextBoxAbout.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBox1_LinkClicked);
            // 
            // groupBoxLogin
            // 
            this.groupBoxLogin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxLogin.Controls.Add(this.cbServer);
            this.groupBoxLogin.Controls.Add(this.lblServer);
            this.groupBoxLogin.Controls.Add(this.txtPassword);
            this.groupBoxLogin.Controls.Add(this.lblUserName);
            this.groupBoxLogin.Controls.Add(this.btnSignin);
            this.groupBoxLogin.Controls.Add(this.txtUserName);
            this.groupBoxLogin.Controls.Add(this.lblPassword);
            this.groupBoxLogin.Location = new System.Drawing.Point(12, 77);
            this.groupBoxLogin.Name = "groupBoxLogin";
            this.groupBoxLogin.Size = new System.Drawing.Size(238, 136);
            this.groupBoxLogin.TabIndex = 14;
            this.groupBoxLogin.TabStop = false;
            this.groupBoxLogin.Text = "Log On To Chat";
            // 
            // cbServer
            // 
            this.cbServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbServer.FormattingEnabled = true;
            this.cbServer.Items.AddRange(new object[] {
            "talk.l.google.com",
            "jabber.org",
            "localhost"});
            this.cbServer.Location = new System.Drawing.Point(95, 72);
            this.cbServer.Name = "cbServer";
            this.cbServer.Size = new System.Drawing.Size(133, 21);
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
            this.txtPassword.Size = new System.Drawing.Size(134, 20);
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
            this.btnSignin.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.txtUserName.Size = new System.Drawing.Size(134, 20);
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
            // lblLine2
            // 
            this.lblLine2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLine2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLine2.Location = new System.Drawing.Point(0, 41);
            this.lblLine2.Name = "lblLine2";
            this.lblLine2.Size = new System.Drawing.Size(262, 17);
            this.lblLine2.TabIndex = 12;
            this.lblLine2.Text = "With Real-Time Text";
            this.lblLine2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLine1
            // 
            this.lblLine1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLine1.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLine1.Location = new System.Drawing.Point(0, 17);
            this.lblLine1.Name = "lblLine1";
            this.lblLine1.Size = new System.Drawing.Size(262, 24);
            this.lblLine1.TabIndex = 10;
            this.lblLine1.Text = "Jabber / Google Talk";
            this.lblLine1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelUser
            // 
            this.labelUser.AutoSize = true;
            this.labelUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUser.Location = new System.Drawing.Point(-18, -516);
            this.labelUser.Name = "labelUser";
            this.labelUser.Size = new System.Drawing.Size(33, 13);
            this.labelUser.TabIndex = 6;
            this.labelUser.Text = "User";
            // 
            // panelContact
            // 
            this.panelContact.Controls.Add(this.btnDelete);
            this.panelContact.Controls.Add(this.btnAdd);
            this.panelContact.Controls.Add(this.rosterTree);
            this.panelContact.Controls.Add(this.lblStatus);
            this.panelContact.Controls.Add(this.lblUser);
            this.panelContact.Controls.Add(this.lblPresence);
            this.panelContact.Controls.Add(this.txtStatus);
            this.panelContact.Controls.Add(this.btnSignOut);
            this.panelContact.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContact.Location = new System.Drawing.Point(0, 0);
            this.panelContact.Name = "panelContact";
            this.panelContact.Size = new System.Drawing.Size(262, 528);
            this.panelContact.TabIndex = 6;
            this.panelContact.Visible = false;
            this.panelContact.Click += new System.EventHandler(this.panelContact_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDelete.BackColor = System.Drawing.SystemColors.Control;
            this.btnDelete.Location = new System.Drawing.Point(61, 499);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(47, 23);
            this.btnDelete.TabIndex = 9;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.BackColor = System.Drawing.SystemColors.Control;
            this.btnAdd.Location = new System.Drawing.Point(8, 499);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(47, 23);
            this.btnAdd.TabIndex = 8;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
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
            this.rosterTree.Size = new System.Drawing.Size(246, 445);
            this.rosterTree.Sorted = true;
            this.rosterTree.StatusColor = System.Drawing.Color.Teal;
            this.rosterTree.TabIndex = 5;
            this.rosterTree.Click += new System.EventHandler(this.rosterTree_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblStatus.Location = new System.Drawing.Point(8, 26);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(246, 20);
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
            this.lblPresence.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPresence.Location = new System.Drawing.Point(198, 8);
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
            this.txtStatus.Size = new System.Drawing.Size(246, 20);
            this.txtStatus.TabIndex = 3;
            this.txtStatus.Visible = false;
            this.txtStatus.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtStatus_KeyDown);
            // 
            // btnSignOut
            // 
            this.btnSignOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSignOut.Location = new System.Drawing.Point(176, 498);
            this.btnSignOut.Name = "btnSignOut";
            this.btnSignOut.Size = new System.Drawing.Size(79, 24);
            this.btnSignOut.TabIndex = 0;
            this.btnSignOut.Text = "Sign Out";
            this.btnSignOut.UseVisualStyleBackColor = true;
            this.btnSignOut.Click += new System.EventHandler(this.btnSignOut_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(262, 528);
            this.Controls.Add(this.panelContact);
            this.Controls.Add(this.panelCredentials);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(270, 276);
            this.Name = "FrmMain";
            this.Text = "RealJabber -- Real Time Text over XMPP";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.panelCredentials.ResumeLayout(false);
            this.panelCredentials.PerformLayout();
            this.groupBoxAbout.ResumeLayout(false);
            this.groupBoxLogin.ResumeLayout(false);
            this.groupBoxLogin.PerformLayout();
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
        private System.Windows.Forms.Label lblLine1;
        private System.Windows.Forms.GroupBox groupBoxLogin;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.RichTextBox richTextBoxAbout;
        private System.Windows.Forms.ComboBox cbServer;
        private System.Windows.Forms.Label lblLine2;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblAbout;
        private System.Windows.Forms.GroupBox groupBoxAbout;
        private System.Windows.Forms.Label labelUser;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
    }
}

