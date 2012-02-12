// NOTE: Recommended code style & format at http://geosoft.no/development/javastyle.html
// which are generally considered superior to the old Sun code formatting rules.
//
// OPEN SOURCE LICENSE
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// FUNDING
// This work was supported with funding from the National Institute on
// Disability and Rehabilitation Research (NIDRR), U.S. Department of Education,
// under grants H133E090001 and H133E080022. However, no endorsement of the
// funding agency should be assumed.

// import java.util.*;
// import java.io.*;

import java.util.Timer;
import java.util.TimerTask;

import java.awt.event.HierarchyBoundsListener;
import java.awt.event.HierarchyEvent;
import java.awt.event.KeyEvent;
import java.awt.event.KeyListener;
import java.awt.Color;
import java.awt.EventQueue;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.Insets;

import javax.swing.JLabel;
import javax.swing.JFrame;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTextArea;
import javax.swing.SwingUtilities;
import javax.swing.event.CaretEvent;
import javax.swing.event.CaretListener;
import javax.swing.event.DocumentEvent;
import javax.swing.event.DocumentListener;
import java.io.IOException;

import org.jivesoftware.smack.Chat;
import org.jivesoftware.smack.ConnectionConfiguration;
import org.jivesoftware.smack.MessageListener;
import org.jivesoftware.smack.PacketListener;
import org.jivesoftware.smack.XMPPConnection;
import org.jivesoftware.smack.XMPPException;
import org.jivesoftware.smack.filter.PacketExtensionFilter;
import org.jivesoftware.smack.packet.Message;
import org.jivesoftware.smack.packet.Presence;
import org.jivesoftware.smack.packet.Packet;
import org.jivesoftware.smack.provider.ProviderManager;

/*
 // Not used in this app
 import org.jivesoftware.smack.PacketCollector;
 import org.jivesoftware.smack.PacketListener;
 import org.jivesoftware.smack.Roster;
 import org.jivesoftware.smack.RosterEntry;
 import org.jivesoftware.smack.filter.FromContainsFilter;
 import org.jivesoftware.smack.filter.MessageTypeFilter;
 import org.jivesoftware.smack.filter.PacketFilter;
 import org.jivesoftware.smack.filter.PacketTypeFilter;
 import org.jivesoftware.smack.packet.Packet;
 import org.jivesoftware.smack.packet.RosterPacket.ItemStatus;
 import org.jivesoftware.smack.util.StringUtils;
 import org.jivesoftware.smack.filter.AndFilter;
 */

//---------------------------------------------------------------------------------------------
/** Simple chat client */
public class ChatClient extends JFrame implements MessageListener, Runnable,
		KeyListener, DocumentListener, CaretListener,
		HierarchyBoundsListener, RealTimeText.Decoder.EventListener {

	// Constants for account -- MODIFY THESE FOR TESTING!!!!!!
	private static final String loginUsername = "YOUR_GTALK@gmail.com";   // Your GTALK username
	private static final String loginPassword = "YOUR_PASSWORD";          // Your GTALK password
	private static final String to = "RECIPIENT_HERE@gmail.com";          // Recipient to send messages to

	// Constants for server
	private static final String host = "talk.l.google.com";
	private static final int port = 5222;
	private static final String serviceName = "gmail.com";

	// Constants for UI
	private static final long serialVersionUID = 5899199681599388079L;
	private static final String newline = "\n";
	private static final float size = 20.0f;
	private static final float sizeLabel = 15.0f;
	private static final int transmitInterval = 1000;

	// User interface
	private static JFrame jtfMainFrame;
	private static JPanel jplPanel;
	private static JLabel jlblPrompt;
	private static JTextArea jtfmsg;
	private static JTextArea jtAreaOutput;
	private static int lineHeight;
	private static JScrollPane scrollPane;

	// Smack XMPP
	private Chat chat = null;
	private XMPPConnection connection;

	// Real time text display variables
	private boolean rttDisplayed = false;
	private int rttOffset = 0;
	private String rttUser = "";
	private RealTimeText.Decoder rttIncoming = new RealTimeText.Decoder(this);
	private RealTimeText.Encoder rttOutgoing = new RealTimeText.Encoder(true);
	private Timer rttTimer = null;

	class TimerEventTask extends TimerTask {
		public void run() {
			EventQueue.invokeLater(ChatClient.this);
		}
	}

	// ---------------------------------------------------------------------------------------------
	/** Main application entry */
	public static void main(String args[]) throws XMPPException, IOException {
		jtfMainFrame = new JFrame("Chat");
		jtfMainFrame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);

		jplPanel = new JPanel();
		jplPanel.setLayout(new GridBagLayout());
		jtfMainFrame.getContentPane().add(jplPanel);

		// Chat history, with scrollbar
		jtAreaOutput = new JTextArea(20, 30);
		jtAreaOutput.setLineWrap(true);
		jtAreaOutput.setWrapStyleWord(true);
		jtAreaOutput.setAutoscrolls(true);
		jtAreaOutput.setFont(jtAreaOutput.getFont().deriveFont(size));
		jtAreaOutput.setEditable(false);
		scrollPane = new JScrollPane(jtAreaOutput);

		// Chat history takes up most of screen
		GridBagConstraints gridBag = new GridBagConstraints();
		gridBag.fill = GridBagConstraints.HORIZONTAL;
		gridBag.anchor = GridBagConstraints.PAGE_START;
		gridBag.gridx = 0;
		gridBag.gridy = 0;
		gridBag.gridheight = 1;
		gridBag.weighty = 0.8;
		gridBag.weightx = 1.0;
		gridBag.gridwidth = 1;
		jplPanel.add(scrollPane, gridBag);

		// Label
		jlblPrompt = new JLabel("Write message below & press Enter to send:");
		jlblPrompt.setFont(jtAreaOutput.getFont().deriveFont(sizeLabel));
		gridBag = new GridBagConstraints();
		gridBag.fill = GridBagConstraints.HORIZONTAL;
		gridBag.gridx = 0;
		gridBag.gridy = 1;
		gridBag.gridheight = 1;
		gridBag.gridwidth = 1;
		gridBag.weightx = 1.0;
		gridBag.insets = new Insets(10, 0, 0, 0); // Empty space above label
		jplPanel.add(jlblPrompt, gridBag);

		// Message entry
		jtfmsg = new JTextArea(3, 30);
		jtfmsg.setFont(jtAreaOutput.getFont());
		jtfmsg.setLineWrap(true);
		jtfmsg.setWrapStyleWord(true);
		jtfmsg.setForeground(Color.BLUE);
		JScrollPane entryScroll = new JScrollPane(jtfmsg);

		// Message entry takes up bottom of grid
		gridBag = new GridBagConstraints();
		gridBag.fill = GridBagConstraints.HORIZONTAL;
		gridBag.anchor = GridBagConstraints.PAGE_END;
		gridBag.gridx = 0;
		gridBag.gridy = 2;
		gridBag.gridheight = 1;
		gridBag.gridwidth = 1;
		gridBag.weighty = 0.1;
		jplPanel.add(entryScroll, gridBag);

		jtfMainFrame.setSize(500, 1000);
		jtfMainFrame.pack();
		jtfMainFrame.setVisible(true);
		lineHeight = jtAreaOutput.getHeight() / jtAreaOutput.getRows();
		jtfmsg.requestFocus();

		ChatClient c = new ChatClient();
		c.initializeClient();
	}

	// ---------------------------------------------------------------------------------------------
	/** Client initialization */
	public void initializeClient() throws XMPPException {
		login(loginUsername, loginPassword);
		jtfmsg.addKeyListener(this);
		jtfmsg.addCaretListener(this);
		jtfmsg.getDocument().addDocumentListener(this);
		jtfMainFrame.getContentPane().addHierarchyBoundsListener(this);
	}

	// ---------------------------------------------------------------------------------------------
	/** Listener for keyboard press event */
	public void keyPressed(KeyEvent event) {
		// Key pressed
		int keyCode = event.getKeyCode();
		if (keyCode == '\n') {
			event.consume();
			executeSendMessageOperation();
		}
	}

	// ---------------------------------------------------------------------------------------------
	/** Listener for window move event */
	@Override
	public void ancestorMoved(HierarchyEvent e) {
		// Window moved. Ignore.
	}

	// ---------------------------------------------------------------------------------------------
	/** Listener for window resize event */
	@Override
	public void ancestorResized(HierarchyEvent e) {
		// TODO: Doesn't always work reliably; i.e. maximizing and de-maximizing
		jtfMainFrame.validate();
		int heightRoom = jlblPrompt.getY();
		jtAreaOutput.setRows((int) ((heightRoom / lineHeight) - 0.5));
	}

	// ---------------------------------------------------------------------------------------------
	/** Listener for keyboard release event */
	public void keyReleased(KeyEvent event) {
		// Key released
	}

	// ---------------------------------------------------------------------------------------------
	/** Listener for keyboard typed character */
	public void keyTyped(KeyEvent event) {
	}

	// ---------------------------------------------------------------------------------------------
	/** Listener for text-changed event */
	public void changedUpdate(DocumentEvent event) {
		// Chat message entry changed.
		rttEncodeMessageChanges();
	}

	// ---------------------------------------------------------------------------------------------
	/** Listener for text-insert event */
	public void insertUpdate(DocumentEvent event) {
		// Text inserted into chat message
		rttEncodeMessageChanges();
	}

	// ---------------------------------------------------------------------------------------------
	/** Listener for text-removal event */
	public void removeUpdate(DocumentEvent event) {
		// Text removed from chat message
		rttEncodeMessageChanges();
	}

	// ---------------------------------------------------------------------------------------------
	/** Listener for cursor movement. */
	public void caretUpdate(CaretEvent event) {
		// Capture of cursor movements
		rttEncodeMessageChanges();
	}

	// ---------------------------------------------------------------------------------------------
	/** User login */
	public void login(String userName, String password) throws XMPPException {
		outputLine("*** Connecting...");

		// Define the Real Time Text extension in smack
		ProviderManager.getInstance().addExtensionProvider(RealTimeText.ROOT,
				RealTimeText.NAMESPACE, new RttExtension.Provider());

		ConnectionConfiguration config = new ConnectionConfiguration(host, port, serviceName);
		connection = new XMPPConnection(config);
		try {
			connection.connect();
			System.out.println("Connected to " + connection.getHost());
		} catch (XMPPException ex) {
			outputLine("*** Failed to connect to " + connection.getHost());
			System.exit(1);
		}

		try {
			connection.login(userName, password);
			// Removed this, since it's more 'in sync' to use the existing MessageListener
			//connection.addPacketListener(this, new PacketExtensionFilter(RealTimeText.NAMESPACE));
			outputLine("*** Logged in as " + userName);
			Presence presence = new Presence(Presence.Type.available);
			connection.sendPacket(presence);
			chat = connection.getChatManager().createChat(to, this);
		} catch (XMPPException ex) {
			outputLine("*** Failed to log in as " + userName);
			System.exit(1);
		}
	}

	// ---------------------------------------------------------------------------------------------
	/** Process incoming chat messages (MessageListener) */
	public void processMessage(Chat chat, Message message) {
		boolean isChat = (message.getType() == Message.Type.chat);
		boolean isNormal = (message.getType() == Message.Type.normal);
		if (isChat || isNormal) {

			// Decode RTT
			RttExtension ext = (RttExtension) message.getExtension(RealTimeText.NAMESPACE);
			if (ext != null) {
				if (!ext.rtt.getActionElements().isEmpty()) {
					rttIncoming.decode(ext.rtt);
				}
			}

			// We have to do a check for message body being NULL
			// Real time text <message> contain NO <body>.
			// Also bug: MAC returns null while typing followed by the msg
			if (message.getBody() != null) {
				// <body> message transmissions
				rttIncoming.reset();
				clearRealTimeMessage();
				outputMessage(chat.getParticipant(), message.getBody());
			}

		}
	}

	// ---------------------------------------------------------------------------------------------
	/** Initiate a messages-send event */
	public void executeSendMessageOperation() {
		String msg = jtfmsg.getText();
		try {
			if ((chat != null) && (msg != null)) {
				jtfmsg.selectAll();
				chat.sendMessage(msg);
				outputMessage(loginUsername, msg);
				jtfmsg.setText("");
				rttOutgoing.nextMsg();
			}
		} catch (XMPPException e) {
			e.printStackTrace();
		}
	}

	/** RTT decoder listener for sync state changes */
	public void rttSyncStateChanged(boolean syncStateChanged) {
	}

	/** RTT decoder listener for text playback */
	public void rttTextUpdated() {
		SwingUtilities.invokeLater(new Runnable() {
		    public void run() {
			    updateRealTimeMessage(to, rttIncoming);
			}
		});
	}

	// ---------------------------------------------------------------------------------------------
	/** Timer event for real time text update */
	@Override
	public void run() {
		try {
			rttTransmitOperation();
		} catch (CloneNotSupportedException e) {
			e.printStackTrace();
		}
	}

	/**
	 * Encodes changes to the message, for real time transmission.
	 * RealTimeText.java is responsible for figuring out what has changed in the
	 * message, and transmitting the modifications to the message. See section
	 * 6.2 of XEP-0301 for more info.
	 */
	public void rttEncodeMessageChanges() {
		// Please see section 6.2 "Monitoring Message Edits" for more information.
		// This will compare the old and new messages to determine what's changed.
		// http://xmpp.org/extensions/xep-0301.html#monitoring_message_edits
		rttOutgoing.encode(jtfmsg.getText(), jtfmsg.getCaretPosition());
		if (rttTimer == null) {
			rttTimer = new Timer();
			rttTimer.scheduleAtFixedRate(new TimerEventTask(), transmitInterval, transmitInterval);
		}
	}

	// ---------------------------------------------------------------------------------------------
	/**
	 * Initiate real time text transmission
	 *
	 * @throws CloneNotSupportedException
	 */
	public void rttTransmitOperation() throws CloneNotSupportedException {
		try {
			if (chat != null) {
				// Any last-minute encoding (normally encoded in the listener)
				rttEncodeMessageChanges();

				// Prep for actual transmission
				if (!rttOutgoing.isEmpty()) {
					RealTimeText.RootElement rtt = rttOutgoing.getEncodedRTT();
					RttExtension extension = new RttExtension(rtt);
					Message msgRTT = new Message();
					msgRTT.addExtension(extension);
					chat.sendMessage(msgRTT);
				} else {
					// Nothing new to transmit in the last interval cycle,
					// so disable RTT transmission timer until the next text
					// change event.
					rttTimer.cancel();
					rttTimer = null;
				}
			}
		} catch (XMPPException e) {
			e.printStackTrace();
		}
	}

	// ---------------------------------------------------------------------------------------------
	/**
	 * Updates a real time message at the bottom of the chat history
	 * (continually-changing chat message with real time typing)
	 */
	public void updateRealTimeMessage(String user, RealTimeText.Decoder decoder) {
		rttUser = user;
		String text = decoder.getText();
		int pos = decoder.getCursorPos();
		// Insert artificial remote cursor into string using the '|' character.
		text = text.substring(0, pos) + "|" + text.substring(pos);

		// Generate the chat line of text
		String chatLine = newline + user + " (is typing): " + text + newline;

		if (rttDisplayed == false) {
			// Turn on real time message.
			rttOffset = jtAreaOutput.getText().length();
			jtAreaOutput.append(chatLine);
		} else {
			// Update existing real time message.
			if (jtAreaOutput.getText().length() < rttOffset) {
				rttOffset = jtAreaOutput.getText().length();
			}
			jtAreaOutput.replaceRange(chatLine, rttOffset, jtAreaOutput.getText().length());
		}
		jtAreaOutput.setCaretPosition(jtAreaOutput.getText().length());
		rttDisplayed = true;
	}

	// ---------------------------------------------------------------------------------------------
	/** Clears the real time message from the bottom of the chat history */
	public void clearRealTimeMessage() {
		if (rttDisplayed == true) {
			// Clear real time message.
			rttDisplayed = false;
			if (jtAreaOutput.getText().length() > rttOffset) {
				jtAreaOutput.replaceRange("", rttOffset, jtAreaOutput.getText()
						.length());
			}
		}
	}

	// ---------------------------------------------------------------------------------------------
	/** Adds a completed chat message to the chat history */
	public void outputMessage(String user, String msg) {
		outputLine(user + " says: " + msg);
	}

	// ---------------------------------------------------------------------------------------------
	/**
	 * Adds a line to the chat history.<br>
	 * This still keeps the real time message (if any) at the very bottom of the
	 * chat history, below any added lines.
	 */
	public void outputLine(String text) {
		// Temporary clear real time message (if any)
		boolean wasEnabled = rttDisplayed;
		if (wasEnabled) clearRealTimeMessage();

		// Add chat text
		jtAreaOutput.append(text + newline);
		System.out.println(text);
		jtAreaOutput.setCaretPosition(jtAreaOutput.getText().length());
	}
}
