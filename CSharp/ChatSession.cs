using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using RealJabber.RealTimeTextUtil;

///<summary>
/// Chat session helper classes with real time text support
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
/// IMPORTANT NOTICE
/// Mark Rejhon retains copyright to the use of the name "RealJabber".
/// Modified versions of this software not released by Mark Rejhon must be released under a 
/// different name than "RealJabber".
///</summary>
namespace RealJabber
{
    /// <summary>Defines a single line of chat</summary>
    public class ChatLine
    {
        public ChatLine(jabber.JID jidValue, string textValue, Color colorValue)
        {
            jid = jidValue;
            text = textValue;
            color = colorValue;
        }
        public jabber.JID JID
        {
            get { return jid; }
        }
        public String Text
        {
            get { return text; }
        }
        public Color Color
        {
            get { return color; }
            set { this.color = value; }
        }
        private jabber.JID jid;
        private string text;
        private Color color;
    }

    /// <summary>Defines a single line of real time text</summary>
    public class RealTimeMessage : ChatLine
    {
        public RealTimeMessage(string fullJidValue, Color colorValue) : base(fullJidValue, "", colorValue)
        {
        }
        public new String Text
        {
            get { return decoder.Text; }
        }
        public RealTimeText.Decoder Decoder
        {
            get { return decoder; }
            set { decoder = value; }
        }
        private RealTimeText.Decoder decoder;
    }

    /// <summary>Class that stores the text content of a chat session</summary>
    public class ChatSession
    {
        private ArrayList chatLines = new ArrayList();
        private Hashtable chatRealTime = new Hashtable();
        private Font fontChatUser = new Font("Arial Unicode MS", 10, FontStyle.Bold); // Default font
        private Font fontChatText = new Font("Arial Unicode MS", 10); // Default font

        /// <summary>Get/sets the font of the chat user</summary>
        public Font FontUser
        {
            get { return fontChatUser; }
            set { if (value != null) fontChatUser = value; }
        }

        /// <summary>Get/sets the font of the chat text</summary>
        public Font FontText
        {
            get { return fontChatText; }
            set { if (value != null) fontChatText = value; }
        }

        /// <summary>Return the conversation buffer</summary>
        public ArrayList Lines 
        {
            get { return chatLines; }
        }

        /// <summary>Font of chat user headings</summary>
        public Font ChatUserFont
        {
            get { return fontChatUser; }
            set { fontChatUser = value; }
        }

        /// <summary>Font of chat text</summary>
        public Font ChatTextFont
        {
            get { return fontChatText; }
            set { fontChatText = value; }
        }

        /// <summary>Generate a rich text formatted chat line</summary>
        public void AddFormattedChatLine(RichTextBox rtf, string formatString, string user, string line, Color colorUser, Color colorText)
        {
            int selStart = rtf.Text.Length;
            string heading = String.Format(formatString, user);
            rtf.AppendText(heading + line + "\n");
            rtf.Select(selStart, heading.Length);
            rtf.SelectionFont = fontChatUser;
            rtf.SelectionColor = colorUser;
            rtf.Select(selStart + heading.Length, line.Length + 1);
            rtf.SelectionFont = fontChatText;
            rtf.SelectionColor = colorText;
        }

        /// <summary>Generate RTF formatted version of chat</summary>
        /// <param name="rtf">RichTextBox to generate RTF to</param>
        /// <param name="formatString">String formatter for each line of chat</param>
        /// <param name="user">Chat lines by specific user, or null for entire chat</param>
        public void FormatAllLinesRTF(RichTextBox rtf, string formatString, string user)
        {
            foreach (ChatLine line in chatLines)
            {
                if ((user == null) || (user == line.JID.Bare))
                {
                    AddFormattedChatLine(rtf, formatString, line.JID.Bare, line.Text, line.Color, Color.Black);
                }
            }
        }

        /// <summary>Returns all lines of chat matching user, as a single string</summary>
        /// <param name="formatString">String formatter for each line of chat</param>
        /// <param name="user">Chat lines by specific user, or null for entire chat</param>
        /// <returns>String containing concatenated lines of chat</returns>
        public string FormatAllLines(string formatString, string user)
        {
            string chatText = "";
            foreach (ChatLine line in chatLines)
            {
                if ((user == null) || (user == line.JID.Bare))
                {
                    chatText += String.Format(formatString, line.JID.Bare) + line.Text + "\n";
                }
            }
            return chatText;
        }

        /// <summary>Generate RTF formatted version of real time text</summary>
        /// <param name="rtf">RichTextBox to generate RTF to</param>
        /// <param name="formatString">String formatter for each line of chat</param>
        /// <param name="user">Chat lines by specific user, or null for entire chat</param>
        public void FormatAllRealTimeRTF(RichTextBox rtf, string formatString, string specificUser, char cursorChar, Color rttTextColor)
        {
            string line;
            foreach (RealTimeMessage rttMsg in chatRealTime.Values)
            {
                if ((specificUser == null) || (specificUser == rttMsg.JID.Bare))
                {
                    line = rttMsg.Decoder.Text;
                    if (cursorChar != 0)
                    {
                        line = line.Insert(rttMsg.Decoder.CursorPos, cursorChar.ToString());
                    }
                    AddFormattedChatLine(rtf, formatString, rttMsg.JID.Bare, line, rttMsg.Color, rttTextColor);
                }
            }
        }

        /// <summary>Returns the real time message of specified JID or full JID</summary>
        /// <param name="jid">JID of user</param>
        /// <returns>Real time message, if any found</returns>
        public RealTimeMessage GetRealTimeMessage(jabber.JID jid)
        {
            return (RealTimeMessage)chatRealTime[(string)jid];
        }

        /// <summary>Clears the real time message of specified JID or full JID</summary>
        /// <param name="jid">JID of user</param>
        public void RemoveRealTimeMessage(jabber.JID jid)
        {
            RealTimeMessage rttMessage = (RealTimeMessage)chatRealTime[(string)jid];
            if (rttMessage != null)
            {
                rttMessage.Decoder.Dispose();
                chatRealTime.Remove((string)jid);
            }
        }

        /// <summary>Clears the chat session, including disposing of any running threads (key press interval playback) in each message</summary>
        public void ClearRealTimeMessages()
        {
            foreach (object rttMessage in chatRealTime)
            {
                if (rttMessage is RealTimeMessage)
                {
                    ((RealTimeMessage)rttMessage).Decoder.Dispose();
                }
            }
            chatRealTime.Clear();
        }

        /// <summary>Clears the chat session</summary>
        public void Clear()
        {
            chatLines.Clear();
            ClearRealTimeMessages();
        }

        /// <summary>Starts a new real-time message for a specific user</summary>
        /// <param name="user">User who started typing</param>
        /// <returns>A new, rael time decoder</returns>
        public RealTimeMessage NewRealTimeMessage(XmlDocument doc, jabber.JID jid, Color colorValue)
        {
            RealTimeMessage rtt = new RealTimeMessage(jid, colorValue);
            chatRealTime[(string)jid] = rtt;
            rtt.Decoder = new RealTimeText.Decoder(doc);
            return rtt;
        }
    }
}
