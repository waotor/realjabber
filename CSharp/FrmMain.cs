using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using jabber.client;
using System.Threading;
using System.Diagnostics;
using jabber.protocol.iq;
using jabber;
using Google.GData.Contacts;
using Google.GData.Extensions;
using jabber.protocol;


namespace RealJabber
{
    /// <summary>
    /// This is an experimental demonstration XMPP Chat client that implements a new XMPP extension:
    /// XMPP In-Band Real Time Text - Version 0.0.2 - http://www.realjabber.org
    /// Written by Mark D. Rejhon - mailto:markybox@gmail.com - http://www.marky.com/resume
    /// 
    /// COPYRIGHT
    /// Copyright 2011 by Mark D. Rejhon - Rejhon Technologies Inc.
    /// 
    /// LICENSE
    /// Licensed under the Apache License, Version 2.0 (the "License");
    /// you may not use this file except in compliance with the License.
    /// You may obtain a copy of the License at
    ///     http://www.apache.org/licenses/LICENSE-2.0
    /// Unless required by applicable law or agreed to in writing, software
    /// distributed under the License is distributed on an "AS IS" BASIS,
    /// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    /// See the License for the specific language governing permissions and
    /// limitations under the License.
    ///
    /// NOTES
    /// Small snippets of code based on the public-domain C# .NET Google Chat client at CodeProject
    /// http://www.codeproject.com/KB/gadgets/googletalk.aspx
    /// 
    /// IMPORTANT NOTICE
    /// Mark Rejhon retains copyright to the use of the name "RealJabber".
    /// Modified versions of this software not released by Mark Rejhon must be released under a 
    /// different name than "RealJabber".
    /// </summary>
    public partial class FrmMain : Form
    {
        // Recommended, see http://code.google.com/p/jabber-net/wiki/FAQ_GoogleTalk
        // May need to use "jabberClient1.NetworkHost = talk.l.google.com"; // If using Mono on Linux/Mac etc.
        const string DEFAULT_SERVER = "talk.l.google.com";
        static ManualResetEvent done = new ManualResetEvent(false);

        RosterManager rosterMgr;
        PresenceManager presenceMgr;
        Dictionary<string, bool> chatInRoster = new Dictionary<string, bool>();
        Dictionary<string, string> chatNicknames = new Dictionary<string, string>();
        Dictionary<string, FrmChat> chatForms = new Dictionary<string, FrmChat>();

        /// <summary>Constructor</summary>
        public FrmMain()
        {
            InitializeComponent();
            jabberClient.OnMessage +=new MessageHandler(jabberClient_OnMessage);
            jabberClient.OnDisconnect += new bedrock.ObjectHandler(jabberClient_OnDisconnect);
            jabberClient.OnError += new bedrock.ExceptionHandler(jabberClient_OnError);
            jabberClient.OnAuthError += new jabber.protocol.ProtocolHandler(jabberClient_OnAuthError);
        }

        //#############################################################################################################
        // Startup and signin

        /// <summary>Called when our main window is created for the first time</summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            txtUserName.Text = "";
            cbServer.Text = DEFAULT_SERVER;
            richTextBoxAbout.SelectAll();
            richTextBoxAbout.SelectionAlignment = HorizontalAlignment.Center;
            richTextBoxAbout.SelectionLength = 0;

            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = assembly.GetName();
            lblVersion.Text = "Version " + assemblyName.Version.ToString();
#if DEBUG
            txtUserName.Text = "mr.devtest@gmail.com";
            txtPassword.Text = "";
            //btnSignin_Click(this, EventArgs.Empty);
#endif
            txtUserName.Focus();
        }

        /// <summary>Signin button</summary>
        private void btnSignin_Click(object sender, EventArgs e)
        {
            panelCredentials.Enabled = false;

            JID jid = new JID(txtUserName.Text);
            if (String.IsNullOrEmpty(jid.User))
            {
                jabberClient.User = txtUserName.Text;
                jabberClient.Server = cbServer.Text.Equals(DEFAULT_SERVER) ? "gmail.com" : cbServer.Text;
            }
            else
            {
                jabberClient.User = jid.User;
                jabberClient.Server = jid.Server;
            }
            jabberClient.NetworkHost = cbServer.Text;
            jabberClient.Password = txtPassword.Text;
            jabberClient.AutoRoster = true;
            jabberClient.AutoStartTLS = true;
            jabberClient.AutoPresence = true;
            jabberClient.AutoLogin = true;
            jabberClient.Resource = "realjabber";
            //jabberClient.PlaintextAuth = true;
            jabberClient.OnAuthenticate += new bedrock.ObjectHandler(jabberClient_OnAuthenticate);

            rosterMgr = new RosterManager();
            rosterMgr.Stream = jabberClient;
            rosterMgr.AutoSubscribe = true;
            rosterMgr.AutoAllow = jabber.client.AutoSubscriptionHanding.AllowAll;
            rosterMgr.OnRosterBegin += new bedrock.ObjectHandler(RosterMgr_OnRosterBegin);
            rosterMgr.OnRosterEnd += new bedrock.ObjectHandler(RosterMgr_OnRosterEnd);
            rosterMgr.OnRosterItem += new RosterItemHandler(RosterMgr_OnRosterItem);
            rosterMgr.OnSubscription += new SubscriptionHandler(rosterMgr_OnSubscription);
            rosterMgr.OnUnsubscription += new UnsubscriptionHandler(rosterMgr_OnUnsubscription);

            presenceMgr = new PresenceManager();
            presenceMgr.Stream = jabberClient;

            rosterTree.RosterManager = rosterMgr;
            rosterTree.PresenceManager = presenceMgr;
            rosterTree.DoubleClick += new EventHandler(rosterTree_DoubleClick);

            lblUser.Text = jabberClient.User;
            jabberClient.Connect();
        }

        void rosterMgr_OnUnsubscription(RosterManager manager, jabber.protocol.client.Presence pres, ref bool remove)
        {
        }

        void rosterMgr_OnSubscription(RosterManager manager, Item ri, jabber.protocol.client.Presence pres)
        {
        }

        /// <summary>Hitting Enter after entering password, triggers the signin button</summary>
        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) btnSignin.PerformClick();
        }

        /// <summary>Hitting Enter after entering server, triggers the signin button</summary>
        private void cbServer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) btnSignin.PerformClick();
        }

        /// <summary>Called upon successful signin</summary>
        void jabberClient_OnAuthenticate(object sender)
        {
            panelCredentials.Visible = false;
            panelContact.Visible = true;
            done.Set();
            txtStatus.Text = "";
            txtPassword.Text = "";
        }

        /// <summary>Called upon signin error</summary>
        void jabberClient_OnAuthError(object sender, System.Xml.XmlElement rp)
        {
            if (rp.Name == "failure")
            {
                MessageBox.Show("Invalid User Name or Password", "Error!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                panelCredentials.Enabled = true;
                txtUserName.SelectAll();
                txtUserName.Focus();
            }
        }

        /// <summary>Called upon other Jabber errors</summary>
        void jabberClient_OnError(object sender, Exception ex)
        {
            MessageBox.Show(ex.Message, "Error!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            panelCredentials.Enabled = true;
            txtUserName.SelectAll();
            txtUserName.Focus();
        }

        /// <summary>Called upon Jabber disconnection</summary>
        void jabberClient_OnDisconnect(object sender)
        {
            panelContact.Visible = false;
            panelCredentials.Enabled = true;
            panelCredentials.Visible = true;
            txtUserName.Focus();
        }

        //#############################################################################################################
        // Roster (buddy list)

        /// <summary>Called before display of buddy list</summary>
        void RosterMgr_OnRosterBegin(object sender)
        {
            chatNicknames = new Dictionary<string, string>();
            rosterTree.BeginUpdate();
        }

        /// <summary>Called for each item in buddy list</summary>
        void RosterMgr_OnRosterItem(object sender, Item ri)
        {
            try
            {
                chatInRoster.Add(ri.JID.Bare, true);
                chatNicknames.Add(ri.JID.Bare, ri.Nickname);
            }
            catch { }
        }

        /// <summary>Call after display of buddy list finished</summary>
        void RosterMgr_OnRosterEnd(object sender)
        {
            RosterManager rm1 = (RosterManager)sender;
            done.Set();
            rosterTree.EndUpdate();
            jabberClient.Presence(jabber.protocol.client.PresenceType.available, txtStatus.Text, null, 0);
            lblPresence.Text = "Available";
            rosterTree.ExpandAll();
        }

        /// <summary>Doubleclicking on a buddy in the list, opens a new chat window (or activates existing window)</summary>
        void rosterTree_DoubleClick(object sender, EventArgs e)
        {
            muzzle.RosterTree.ItemNode selectedNode = rosterTree.SelectedNode as muzzle.RosterTree.ItemNode;
            if (selectedNode != null)
            {
                string bareJID = selectedNode.JID.Bare;
                string nickName = !String.IsNullOrEmpty(selectedNode.Nickname) ? selectedNode.Nickname : bareJID;
                FrmChat chatWindow = InitializeChatWindow(selectedNode.JID, nickName);
                chatWindow.Show();
                chatWindow.BringToFront();
            }
        }

        //#############################################################################################################
        // Incoming messages

        private void jabberClient_OnMessage(object sender, jabber.protocol.client.Message msg)
        {
            // Handle messages on the same thread as UI.
            if (this.InvokeRequired)
            {
                this.Invoke(new MessageHandler(jabberClient_OnMessage), sender, msg);
            }

            bool newWindow = false;
            string bareJID = msg.From.Bare;
            string nickName = null;
            if (chatNicknames.ContainsKey(bareJID)) nickName = chatNicknames[bareJID];

            // Do we already have a chat window for this message?

            FrmChat chatWindow = InitializeChatWindow(msg.From, nickName);
            if (msg.Body != null)
            {
                if (!chatWindow.Visible)
                {
                    // Show chat window only when receiving first full-line message.
                    newWindow = true;
                    chatWindow.Show();
                    chatWindow.BringToFront();
                    chatWindow.WindowState = FormWindowState.Normal;
                    chatWindow.Flash();
                }
                else if (chatWindow.WindowState == FormWindowState.Minimized)
                {
                    // Flash chat window in taskbar, if minimized
                    chatWindow.Flash();
                }
                chatWindow.ReceiveFlag = true;
            }

            // Show window and handle chat message
            if (chatWindow.Visible)
            {
                chatWindow.HandleMessage(msg);
            }

            // Special behaviour if we're invited to a group chat
            if (!chatInRoster.ContainsKey(bareJID))
            {
                Console.WriteLine("NOTICE: Message from unrecogized bareJID: " + bareJID);
                if (newWindow && bareJID.EndsWith("groupchat.google.com"))
                {
                    MessageBox.Show("You have been invited to a group chat. However, group chat is not yet supported in this specific client. (XEP-0045 not implemented)");
                }
            }
        }

        //#############################################################################################################
        // Chat window creation

        /// <summary>Gets the active chat window for a JID</summary>
        /// <param name="bareJID">JID of chat</param>
        /// <returns>FrmChat if chat exists, null otherwise</returns>
        private FrmChat ActiveChatWindow(JID jid)
        {
            if (chatForms.ContainsKey(jid.Bare))
            {
                FrmChat chatWindow = chatForms[jid.Bare];
                if ((chatWindow != null) &&
                    (chatWindow.Enabled != false) &&
                    !chatWindow.IsDisposed)
                {
                    return chatWindow;
                }
            }
            return null;
        }

        /// <summary>Initializes a new chat window, and adds it to the hashtable of chat windows</summary>
        /// <param name="jid">JID of recipient</param>
        /// <param name="nickName">nickname of recipient</param>
        /// <returns>FrmChat object of new chat window</returns>
        private FrmChat InitializeChatWindow(jabber.JID jid, string nickName)
        {
            // Attempt to use existing chat window
            FrmChat chatWindow = ActiveChatWindow(jid);
            if (chatWindow == null)
            {
                // Create new chat window
                chatWindow = new FrmChat();
                chatWindow.JabberObject = jabberClient;
                chatWindow.JID = jid;
                chatWindow.Nickname = (nickName != null) ? nickName : jid.User;
                chatWindow.Text = chatWindow.Nickname;
                chatForms[jid.Bare] = chatWindow;
            }
            return chatWindow;
        }

        //#############################################################################################################
        // Status message functions

        /// <summary>Clicking on status label allows you to change your online status</summary>
        private void labelStatus_Click(object sender, EventArgs e)
        {
            lblStatus.Visible = false;
            txtStatus.Visible = true;
            if (txtStatus.Text.Length > 0)
            {
                txtStatus.Select(0, txtStatus.Text.Length);
            }
        }

        /// <summary>Hitting enter saves status message</summary>
        private void txtStatus_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SaveStatusMessage();
            }
        }

        /// <summary>Clicking on roster tree saves the status message</summary>
        private void rosterTree_Click(object sender, EventArgs e)
        {
            SaveStatusMessage();
        }

        /// <summary>Clicking on contact list saves the status message</summary>
        private void panelContact_Click(object sender, EventArgs e)
        {
            SaveStatusMessage();
        }

        /// <summary>Saves the new status message</summary>
        private void SaveStatusMessage()
        {
            if (txtStatus.Visible)
            {
                txtStatus.Visible = false;
                lblStatus.Visible = true;
                lblStatus.Text = txtStatus.Text;
                jabberClient.Presence(jabber.protocol.client.PresenceType.available, txtStatus.Text, null, 0);
            }
        }

        //#############################################################################################################
        // Click events

        /// <summary>Signout button: Closes all chat windows and signs out of the XMPP server</summary>
        private void btnSignOut_Click(object sender, EventArgs e)
        {
            foreach (FrmChat chatWindow in chatForms.Values)
            {
                chatWindow.Close();
            }
            chatForms.Clear();
            jabberClient.Close(true);
        }

        /// <summary>Window close event also signs out.</summary>
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            btnSignOut_Click(sender, (EventArgs)e);
        }

        /// <summary>Make links clickable in the main startup screen</summary>
        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);           
        }

        /// <summary>Add a buddy to chat</summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            FrmAdd addDialog = new FrmAdd();
            if (addDialog.ShowDialog() == DialogResult.OK)
            {
                jabberClient.Subscribe(addDialog.AddJID, addDialog.AddJID, null);
                MessageBox.Show("User has been added:\n" + addDialog.AddJID);
            }
        }

        /// <summary>Deletes a buddy from chat</summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            muzzle.RosterTree.ItemNode selectedNode = rosterTree.SelectedNode as muzzle.RosterTree.ItemNode;
            if (selectedNode != null)
            {
                string bareJID = selectedNode.JID.Bare;
                string nickName = !String.IsNullOrEmpty(selectedNode.Nickname) ? selectedNode.Nickname : bareJID;
                DialogResult result;
                result = MessageBox.Show("Are you sure you want to remove '" + nickName + "'?",
                    "Delete '" + nickName + "'",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                {
                    jabberClient.RemoveRosterItem(bareJID);
                }
            }
        }
    }
}