using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Xml;

namespace RealJabber.RealTimeTextUtil
{
    /// <summary>
    /// HELPER UTILITY CLASS FOR:
    ///   XMPP EXTENSION: http://www.xmpp.org/extensions/xep-0301.html
    ///      DESCRIPTION: XEP-0301 - In-Band Real Time Text - Version 0.0.3 - http://www.realjabber.org
    ///    
    ///         LANGUAGE: C# .NET
    ///       XML PARSER: System.Xml (part of .NET)
    ///     XMPP LIBRARY: No internal dependency
    ///           AUTHOR: Mark D. Rejhon - mailto:markybox@gmail.com - http://www.marky.com/resume
    ///        COPYRIGHT: Copyright 2011 by Mark D. Rejhon - Rejhon Technologies Inc.
    ///          LICENSE: Apache 2.0 - Open Source - Commercial usage is permitted.
    /// 
    /// LICENSE
    ///   Licensed under the Apache License, Version 2.0 (the "License");
    ///   you may not use this file except in compliance with the License.
    ///   You may obtain a copy of the License at
    /// 
    ///     http://www.apache.org/licenses/LICENSE-2.0
    ///     
    ///   Unless required by applicable law or agreed to in writing, software
    ///   distributed under the License is distributed on an "AS IS" BASIS,
    ///   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    ///   See the License for the specific language governing permissions and
    ///   limitations under the License.
    /// 
    /// TABLE OF SUPPORTED ACTION ELEMENTS
    ///   For more information, please see http://www.realjabber.org
    ///   ----------------------------------------------------------
    ///   TIER ACTION           CODE               DESCRIPTION
    ///     1  Insert           <t p='#'>text</t>  (REQUIRED) Insert text at position p in message. Also good for block inserts (i.e. pastes)
    ///     1  Backspace        <e p='#' n='#'/>   (REQUIRED) Deletes n characters of text to the left of position p in message.
    ///     1  Forward Delete   <d p='#' n='#'/>   (REQUIRED) Deletes n characters of text to the right of position p in message. Also good for block deletes (i.e. cuts)
    ///     2  Delay            <w n='#'/>         (RECOMMENDED) Key press intervals. Execute a pause of n thousandths of a second. Allows multiple smooth keypresses in one packet.
    ///     2  Cursor Position  <c p='#'/>         (OPTIONAL) Move cursor to position p in message. (Remote Cursor)
    ///     2  Beep             <g/>               (OPTIONAL) Executes a flash/beep/buzz. Deaf-friendly feature.
    /// 
    /// IMPORTANT NOTICE
    ///   Mark Rejhon retains copyright to the use of the name "RealJabber".
    ///   Modified versions of this software not released by Mark Rejhon must be released under a 
    ///   different name than "RealJabber".
    /// </summary>
    public class RealTimeText
    {
        // Specification constants
        public const string ROOT = "rtt";
        public const string NAMESPACE = "urn:xmpp:rtt:0";
        public const string VERSION = "0.0.3";

        /// <summary>The default recommended transmission for transmission of real time text</summary>
        public const int INTERVAL_DEFAULT = 1000;

        /// <summary>Tests the encoder/decoder against each other for debug purposes. This reduces performance.</summary>
        public const bool TEST_RTT_CODEC = false;

        // ################################################################################################################

        /// <summary>Class representing the state of a real time message</summary>
        public class Message : Object
        {
            private String text = "";
            private int pos = 0;
            private int delay = 0;
            private bool beeped = false;
            private bool delaysEnabled = false;

            /// <summary>Main constructor</summary>
            public Message() {}

            /// <summary>Constructor that specifies message text and cursor position</summary>
            public Message(string textValue, int posValue)
            {
                text = textValue;
                pos = posValue;
            }

            /// <summary>Resets the real time message</summary>
            public void Reset()
            {
                text = "";
                pos = 0;
                delay = 0;
                beeped = false;
            }

            /// <summary>Clones this object</summary>
            public Message Clone()
            {
                return (Message)this.MemberwiseClone();
            }

            /// <summary>Gets/sets the message text</summary>
            public String Text
            {
                get { return text; }
                set { text = value; }
            }

            /// <summary>Gets/sets the cursor position index</summary>
            public int CursorPos
            {
                get { return pos; }
                set { pos = value; }
            }

            /// <summary>Gets/sets the flash/beep status flag</summary>
            public bool Beeped
            {
                get { return beeped; }
                set { beeped = value; }
            }

            /// <summary>Gets/sets the current delay value</summary>
            public int Delay
            {
                get { return delay; }
                set { delay = value; }
            }

            /// <summary>Enable processing of delay action elements</summary>
            public bool DelaysEnabled
            {
                get { return delaysEnabled; }
                set { delaysEnabled = value; }
            }
        }

        // ################################################################################################################

        /// <summary>Class to generate action elements</summary>
        public static class AppendElement
        {
            /// <summary>T Element - Insert/Append Text - Inserts text at specified position, or appends text to end of line</summary>
            /// <param name="rtt">The rtt element to append to</param>
            /// <param name="text">Text to insert</param>
            /// <param name="pos">Position to insert text at</param>
            /// <param name="len">Length of original text to insert into (for automatic omission of 'pos' if at end of string)</param>
            public static void Insert(XmlElement rtt, string text, int pos, int len)
            {
                if (String.IsNullOrEmpty(text)) return;

                XmlElement insertCode = rtt.OwnerDocument.CreateElement("t");
                if ((pos >= 0) && (pos < len))
                {
                    insertCode.SetAttribute("p", pos.ToString());
                }
                insertCode.InnerText = text;
                rtt.AppendChild(insertCode);
            }

            /// <summary>E Element - Backspace - Erase # characters to left of specified position</summary>
            /// <param name="rtt">The rtt element to append to</param>
            /// <param name="pos">Position to begin backspacing at</param>
            /// <param name="count">Count of characters to backspaces</param>
            /// <param name="len">Length of original string</param>
            public static void Backspace(XmlElement rtt, int pos, int count, int len)
            {
                if (count <= 0) return;

                XmlElement backspaceCode = rtt.OwnerDocument.CreateElement("e");
                if (count != 1)
                {
                    backspaceCode.SetAttribute("n", count.ToString());
                }
                if ((pos >= 0) && (pos < len))
                {
                    backspaceCode.SetAttribute("p", pos.ToString());
                }
                rtt.AppendChild(backspaceCode);
            }

            /// <summary>D Element - Forward Delete - Delete # of characters to right of specified position</summary>
            /// <param name="rtt">The rtt element to append to</param>
            /// <param name="pos">Position to delete characters at.</param>
            /// <param name="count">Count of characters to delete.</param>
            public static void ForwardDelete(XmlElement rtt, int pos, int count)
            {
                if (count <= 0) return;

                XmlElement deleteCode = rtt.OwnerDocument.CreateElement("d");
                if (count != 1)
                {
                    deleteCode.SetAttribute("n", count.ToString());
                }
                deleteCode.SetAttribute("p", pos.ToString());
                rtt.AppendChild(deleteCode);
            }

            /// <summary>C Element - Cursor Position - Set cursor position to specified index</summary>
            /// <param name="rtt">The rtt element to append to</param>
            /// <param name="pos">New cursor position index</param>
            public static void CursorPosition(XmlElement rtt, int pos)
            {
                XmlElement cursorCode = rtt.OwnerDocument.CreateElement("c");
                cursorCode.SetAttribute("p", pos.ToString());
                rtt.AppendChild(cursorCode);
            }

            /// <summary>W Element - Delay - Pause specified amount for a delay between actions (including keypresses)</summary>
            /// <param name="rtt">The rtt element to append to</param>
            /// <param name="milliseconds">Number of thousandths of seconds to delay</param>
            public static void Delay(XmlElement rtt, int milliseconds)
            {
                if (milliseconds > 0)
                {
                    XmlElement delayCode = rtt.OwnerDocument.CreateElement("w");
                    delayCode.SetAttribute("n", milliseconds.ToString());
                    rtt.AppendChild(delayCode);
                }
            }

            /// <summary>G Element - Flash - Brief visual flash/buzz/beep</summary>
            /// <param name="rtt">The rtt element to flash/beep/buzz</param>
            public static void Flash(XmlElement rtt)
            {
                XmlElement resetCode = rtt.OwnerDocument.CreateElement("g");
                rtt.AppendChild(resetCode);
            }
        }

        // ################################################################################################################

        /// <summary>Decode Action Elements on a string of text.
        /// This follows the XMPP Real Time Text Specification, Version 0.0.2</summary>
        /// <param name="rtt">The rtt element containing Action Elements</param>
        /// <param name="message">Real time message to update</param>
        /// <returns>New text of message</returns>
        public static String DecodeRawRTT(XmlElement rtt, Message message)
        {
            String text = message.Text;
            int count;
            int pos;
            XmlNode editElement;
            XmlAttribute p;
            XmlAttribute n;

            message.Delay = 0;

            // Verify rtt element is defined
            Debug.Assert(rtt != null);
            if (rtt == null) return message.Text;
            if (!rtt.HasChildNodes) return message.Text;

            // Loop to process the XML Action Elements
            while (rtt.HasChildNodes)
            {
                editElement = rtt.FirstChild;
                rtt.RemoveChild(editElement);
                                                                                                                   
                try
                {
                    // Get the "p" attribute (cursor position).  p always default to end of line if not provided.
                    p = editElement.Attributes["p"];
                    pos = (p == null) ? text.Length : int.Parse(p.Value);
                    if (pos > text.Length) pos = text.Length;
                    if (pos < 0) pos = 0;

                    // Get the "n" attribute (count).  n always default to 1 if not provided.
                    n = editElement.Attributes["n"];
                    count = (n == null) ? 1 : int.Parse(n.Value);
                    if (count < 0) count = 0;

                    switch (editElement.Name)
                    {
                        case "t": // Insert action element:              <t p="#">text</i>
                            message.CursorPos = pos;
                            text = text.Insert(message.CursorPos, editElement.InnerText);
                            message.CursorPos = message.CursorPos + editElement.InnerText.Length;
                            break;
                        case "e": // Backspace action element:           <e p="#" n="#"/>
                            message.CursorPos = pos;
                            if (count > message.CursorPos) count = message.CursorPos;
                            text = text.Remove(message.CursorPos - count, count);
                            message.CursorPos -= count;
                            break;
                        case "d": // Delete action element:              <d p="#" n="#"/>
                            message.CursorPos = pos;
                            text = text.Remove(message.CursorPos, count);
                            break;
                        case "c": // Cursor Position action element:     <c p="#"/>
                            message.CursorPos = pos;
                            break;
                        case "w": // Delay code                          <w n="#"/>
                            if (message.DelaysEnabled)
                            {
                                message.Delay = count;
                                goto ExitLoop;
                                // We exit the loop so that the caller can decide to use a timer for the delay code,
                                // and come back to this loop later to finish remaining nodes that have not finished processing.
                            }
                            break;
                        case "g": // Flash action element:               <g/>
                            message.Beeped = true;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.Write("EXCEPTION: DecodeRawRTT - " + ex.ToString());
                }
            }
        ExitLoop:
            message.Text = text;
            return text;
        }

        // ################################################################################################################

        /// <summary>Encode RTT edit action elements on a string of text to turn one string to the next string.
        /// This follows the XMPP Real Time Text Specification.</summary>
        /// <param name="rtt">The rtt element object</param>
        /// <param name="before">Previous real time message</param>
        /// <param name="after">Current real time message</param>
        /// <returns>Efficiently encoded RTT fragment to send in XML "rtt" element to change "before" into "after"</returns>
        public static XmlNode EncodeRawRTT(XmlElement rtt, Message before, Message after)
        {
            int curPos = before.CursorPos;
            XmlElement lastChild = (XmlElement)rtt.LastChild;
            if (after.CursorPos < 0) after.CursorPos = 0;
            if (after.CursorPos > after.Text.Length) after.CursorPos = after.Text.Length;

            if (before.Text == after.Text)
            {
                // Handle cursor position change only.
                if (before.CursorPos != -1)
                {
                    if (before.CursorPos != after.CursorPos)
                    {
                        AppendElement.CursorPosition(rtt, after.CursorPos);
                    }
                }
            }
            else if (after.Text == "")
            {
                // The whole line got cleared.
                AppendElement.Backspace(rtt, before.Text.Length, before.Text.Length, before.Text.Length);
                curPos = 0;
            }
            else
            {
                int leadingSame = 0;
                int trailingSame = 0;
                int i = 0;
                int j = 0;

                // Find number of characters at start that remains the same
                while ((before.Text.Length > i) && (after.Text.Length > i))
                {
                    if (before.Text[i] != after.Text[i])
                    {
                        break;
                    }
                    i++;
                    leadingSame++;
                }

                // Find number of characters at end that remains the same
                i = before.Text.Length;
                j = after.Text.Length;
                while ((i > leadingSame) && (j > leadingSame))
                {
                    i--;
                    j--;
                    if (before.Text[i] != after.Text[j])
                    {
                        break;
                    }
                    trailingSame++;
                }

                // Delete text if a deletion is detected anywhere in the string.
                int charsRemoved = before.Text.Length - trailingSame - leadingSame;
                if (charsRemoved > 0)
                {
                    int posEndOfRemovedChars = before.Text.Length - trailingSame;
                    int posStartOfRemovedChars = posEndOfRemovedChars - charsRemoved;
                    if ((before.CursorPos == posEndOfRemovedChars) ||
                        (posEndOfRemovedChars == before.Text.Length))
                    {
                        // Cursor ideally positioned for <e> BACKSPACE operations rather than delete
                        AppendElement.Backspace(rtt, posEndOfRemovedChars, charsRemoved, before.Text.Length);
                        curPos = posStartOfRemovedChars;
                    }
                    else
                    {
                        // Cursor ideally positioned for <d> FORWARD DELETE operations rather than backspace
                        AppendElement.ForwardDelete(rtt, posStartOfRemovedChars, charsRemoved);
                        curPos = posStartOfRemovedChars;
                    }
                }

                // Do an <t> INSERT operation if any text insertion is detected anywhere in the string
                int charsInserted = after.Text.Length - trailingSame - leadingSame;
                string insertedText = after.Text.Substring(leadingSame, charsInserted);
                AppendElement.Insert(rtt, insertedText, curPos, before.Text.Length);
                curPos += charsInserted;

                // Execute a <c> CURSOR POSITION operation to move cursor to final location, if last edit action element didn't put cursor there.
                if (curPos != after.CursorPos)
                {
                    AppendElement.CursorPosition(rtt, after.CursorPos);
                    curPos = after.CursorPos;
                }
            }

#if TEST_RTT_CODEC
            // Verify that we encoded properly by executing a decode on the encoded action elements
            int testPos = before.CursorPos;
            int delay = 0;
            XmlElement rttTest;
            if (lastChild == null)
            {
                rttTest = (XmlElement)rtt.CloneNode(true);
            }
            else
            {
                rttTest = rtt.OwnerDocument.CreateElement(RealTimeText.ROOT);
                lastChild = (XmlElement)lastChild.NextSibling;
                while (lastChild != null)
                {
                    rttTest.AppendChild(lastChild.Clone());
                    lastChild = (XmlElement)lastChild.NextSibling;
                }
            }
            string codedText = rttTest.InnerXml.ToString();
            Message msg = new Message();
            msg.Text = before.Text;
            msg.CursorPos = testPos;
            msg.Delay = delay;
            msg.DelaysEnabled = false;
            DecodeRawRTT(rttTest, msg);

            Console.WriteLine("------------");
            Console.WriteLine("Before Text : " + before.Text);
            Console.WriteLine("After Text  : " + after.Text);
            Console.WriteLine("CODED Text  : " + codedText);
            Debug.Assert(after.Text == msg.Text, "*** ASSERT FAIL! RTT TEXT MISMATCH.");
            Debug.Assert(after.CursorPos == msg.CursorPos, "*** ASSERT FAIL! RTT CURSOR POS MISMATCH: " + testPos.ToString() + " vs " + after.CursorPos.ToString());
#endif
            return rtt;
        }

        // ################################################################################################################

        /// <summary>Class to calculate very accurate delay intervals, used for recording delays between actions
        /// (such as the pauses between key presses) for encoding & decoding of the 'w' action element during 
        /// real time text communications.  Delay interval values are calculated using an accumulated delay total
        /// since the beginning of an &lt;rtt&rt; element.  This calculation is much more accurate than simply
        /// trying to directly measure the time between actions, and more immune to system/performance variances.
        /// Timers and other methods of delay calculations, are more prone to software/CPU performance variations.
        /// The calculations in this class is immune to system & software performance variations AND is also 
        /// immune to accumulated rounding errors.</summary>
        public class DelayCalculator
        {
            // NOTE: Stopwatch object is apparently more accurate under older Windows XP than DateTime.Now.Ticks and System.Environment.TickCount!
            private Stopwatch ticks = new Stopwatch();
            private long delayTotal;

            /// <summary>Start the delay measurement</summary>
            public void Start()
            {
                delayTotal = 0;
                ticks.Reset();
                ticks.Start();
            }

            /// <summary>Stops the delay measurement</summary>
            public void Stop()
            {
                ticks.Stop();
            }

            /// <summary>Returns true if the delay measurement is running</summary>
            public bool IsRunning 
            {
                get { return ticks.IsRunning; }
            }

            /// <summary>For Decoding:
            /// Adjusts a delay interval value, that self-compensates for system/software performance
            /// This means, the more 'late' this function is called (i.e. slow performance, etc) the returned
            /// value is automatically smaller.  In the ideal situation (fast performance) and typical usage, 
            /// the aim is that the returned value will typically exactly equal delayInterval</summary>
            /// <param name="delayInterval">Recommended delay interval. (typically, from a delay action element)</param>
            /// <returns>Adjusted delay interval</returns>
            public int GetCompensatedDelay(int delayInterval)
            {
                // Running total of delay intervals
                delayTotal += delayInterval;

                // Calculates recommended interval 
                int newInterval = (int)(delayTotal - ticks.ElapsedMilliseconds);
                if (newInterval < 0) newInterval = 0;
                return newInterval;
            }

            /// <summary>For Encoding:
            /// Gets the number of milliseconds elapsed since the last call to this method.
            /// Self-compensating for rounding errors by using a delay total to help average out the errors.
            /// Example: Repeatedly calling this function regularly at a fractional millisecond interval, 
            /// such as every 11.5ms will alternate between adjacent values such as 11, then 12, then 11, and so on.
            /// </summary>
            /// <returns>Number of milliseconds since last call</returns>
            public int GetMillisecondsSinceLastCall()
            {
                int newInterval = (int)(ticks.ElapsedMilliseconds - delayTotal);
                delayTotal += newInterval;
                return newInterval;
            }
        }

        // ################################################################################################################

        /// <summary>
        /// Utility class to decode real time text (RTT) upon receiving.
        /// This class is thread-safe for multithreading, to optionally support inter-key press delays (Natural Typing Mode)
        /// </summary>
        public class Decoder
        {
            private XmlDocument doc;
            private RealTimeText.Message message = new RealTimeText.Message();
            private List<XmlElement> rttElementQueue = new List<XmlElement>();
            private bool sync = false;
            private bool activated = false;
            private uint seq = 0;

            /// <summary>Callback event for asynchronous RTT decoding (This mainly used for Natural Typing mode)</summary>
            public event TextUpdatedHandler TextUpdated = null;
            public delegate void TextUpdatedHandler(Decoder decoder);

            /// <summary>Callback event that is called everytime sync state changes (loss of sync, caused by missing or out-of-order rtt packets)</summary>
            public event SyncStateChangedHandler SyncStateChanged = null;
            public delegate void SyncStateChangedHandler(bool isInSync);

            Thread decodeThread;
            private bool enableThread = false;
            private AutoResetEvent rttElementEvent = new AutoResetEvent(false);
            private const int IDLE_THREAD_QUIT_INTERVAL = 2000;

            /// <summary>Constructor</summary>
            /// <param name="setUser">XMPP user for this Real Time Text (RTT) decoder</param>
            public Decoder(XmlDocument setDoc)
            {
                doc = setDoc;
                Reset();
            }

            /// <summary>Stops the decoder thread</summary>
            public void Dispose()
            {
                lock (rttElementQueue)
                {
                    enableThread = false;
                    rttElementEvent.Set();
                }
            }

            /// <summary>Reinitialize the RTT decoder to a blank string state (new message)</summary>
            public void Reset()
            {
                lock (rttElementQueue)
                {
                    rttElementQueue.Clear();
                    message.Reset();
                    message.DelaysEnabled = true;
                    sync = true;
                }
            }

            /// <summary>Stop the background decoder</summary>
            private void TriggerDecodeThread()
            {
                lock (rttElementQueue)
                {
                    if (!enableThread)
                    {
                        enableThread = true;
                        decodeThread = new Thread(DecodeThread);
                        decodeThread.Name = "RTT Decoder";
                        decodeThread.Start();
                        Debug.Write("STARTED rtt decoder thread\n");
                    }
                    else
                    {
                        rttElementEvent.Set();
                    }
                }
            }

            /// <summary>Background decode thread</summary>
            private void DecodeThread()
            {
                XmlElement rtt;
                bool elementsFound = false;
                int interval = 0;
                DelayCalculator delayCalculator = new DelayCalculator();

                while (true)
                {
                    lock (rttElementQueue)
                    {
                        if (!enableThread) break;
                        
                        elementsFound = (rttElementQueue.Count > 0);
                        if (elementsFound)
                        {
                            // Start the delay calculator on the moment of the first RTT element received into empty queue
                            if (!delayCalculator.IsRunning) delayCalculator.Start();

                            // Loop to decode RTT elements
                            while (enableThread && (rttElementQueue.Count > 0))
                            {
                                rtt = rttElementQueue[0];
                                RealTimeText.DecodeRawRTT(rtt, message);
                                if (!rtt.HasChildNodes) rttElementQueue.RemoveAt(0);

                                if (message.Delay != 0)
                                {
                                    interval = message.Delay;
                                    break;
                                }
                            }
                        }
                        if (!enableThread) break;
                    }

                    // (MUST be lock-free) Calls the user-defined event for one step of text decode 
                    if (elementsFound && (TextUpdated != null))
                    {
                        TextUpdated(this);
                    }

                    if (interval > 0)
                    {
                        // Calculate the delay interval and then sleep for that interval if needed. 
                        // IMPORTANT: This SAVES BATTERY on mobile devices!
                        interval = delayCalculator.GetCompensatedDelay(interval);
                        if (interval > 0) Thread.Sleep(interval);
                        interval = 0;
                    }
                    else
                    {
                        // Zero interval. Stop the delay calculator, and suspend this thread until the next RTT element
                        delayCalculator.Stop();
                        rttElementEvent.WaitOne(IDLE_THREAD_QUIT_INTERVAL);
                        rttElementEvent.Reset();

                        // Exit thread if we've waited long enough with no elements arrived. Thread will restart automatically.
                        // This saves system resources (and battery life on mobiles) if there's lots of chat windows.
                        lock (rttElementQueue)
                        {
                            if (rttElementQueue.Count == 0)
                            {
                                enableThread = false;
                                break;
                            }
                        }
                    }
                }
                Debug.Write("STOPPING rtt decoder thread\n");
            }

            /// <summary>Add RTT text to the decode queue</summary>
            /// <param name="rtt">Text that may also contain RTT edit action elements</param>
            /// <returns>true if successfully buffered</returns>
            public bool Decode(XmlElement rttNew)
            {
                if (rttNew == null) return false;

                lock (rttElementQueue)
                {
                    try
                    {
                        XmlAttribute eventAttr = rttNew.Attributes["event"];
                        XmlAttribute seqAttr = rttNew.Attributes["seq"];
                        UInt32 seqNewValue = UInt32.Parse(seqAttr.Value);
                        if (seqAttr == null)
                        {
                            // Missing "seq" attribute is specified as an OUT-OF-SYNC condition
                            sync = false;
                        }
                        else if (eventAttr != null)
                        {
                            switch (eventAttr.Value.ToLower())
                            {
                                case "new":
                                    // New RTT message. This brings us into SYNC.
                                    activated = true;
                                    Reset();
                                    seq = seqNewValue;
                                    rttElementQueue.Add(rttNew);
                                    if (SyncStateChanged != null) SyncStateChanged(true);
                                    break;
                                case "reset":
                                    // Reset RTT message. This brings us into SYNC.
                                    activated = true;
                                    Reset();
                                    seq = seqNewValue;
                                    rttElementQueue.Add(rttNew);
                                    if (SyncStateChanged != null) SyncStateChanged(true);
                                    break;
                                case "start":
                                    activated = true;
                                    break;
                                case "stop":
                                    activated = false;
                                    break;
                                default:
                                    // Undocumented 'event' attribute value is specified as an OUT-OF-SYNC condition.
                                    sync = false;
                                    break;
                            }
                        }
                        else if (seqNewValue != (UInt32)(seq + 1))
                        {
                            // Non-consecutive increment in 'seq' is specified as an OUT-OF-SYNC condition.
                            sync = false;
                        }
                        else
                        {
                            // Append to existing RTT message queue
                            seq = seqNewValue;
                            rttElementQueue.Add(rttNew);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Any exception occuring at this point, should be assumed as an OUT-OF-SYNC condition. (i.e. NumberFormatException, etc)
                        Debug.Write("EXCEPTION: thrown in RTTdecode.Queue: " + ex.ToString());
                        sync = false;
                    }

                    if (!sync)
                    {
                        // If we're no longer in sync, then inform this object's owner
                        if (SyncStateChanged != null) SyncStateChanged(false);
                    }
                    TriggerDecodeThread();
                }
                return sync;
            }

            /// <summary>Decodes all RTT text immediately</summary>
            /// <returns>Resulting decoded message</returns>
            public string FullDecodeNow()
            {
                lock (rttElementQueue)
                {
                    message.DelaysEnabled = false;
                    foreach (XmlElement rtt in rttElementQueue)
                    {
                        RealTimeText.DecodeRawRTT(rtt, message);
                    }
                    message.DelaysEnabled = true;
                    rttElementQueue.Clear();
                    return message.Text;
                }
            }

            /// <summary>Gets the current state of the message string (Thread safe)</summary>
            public string Text
            {
                get { lock (rttElementQueue) return message.Text; }
            }

            /// <summary>Alias for Decoder.Text</summary>
            public override string ToString()
            {
                lock (rttElementQueue) return message.Text;
            }

            /// <summary>Gets the current character index position of the cursor (Thread safe)</summary>
            public int CursorPos
            {
                get { lock (rttElementQueue) return message.CursorPos; }
            }

            /// <summary>Returns true if activated (Thread safe)</summary>
            public bool IsActivated
            {
                get { lock (rttElementQueue) return activated; } 
            }

            /// <summary>Returns true if currently synchronized (Thread safe)</summary>
            public bool InSync
            {
                get { lock (rttElementQueue) return sync; }
            }
        }

        // ################################################################################################################

        /// <summary>
        /// Utility class to encode real time text (RTT) before sending.
        /// </summary>
        public class Encoder 
        {
            private XmlDocument doc;
            private RealTimeText.Message prevMessage = new RealTimeText.Message();
            private XmlElement rtt;
            private int transmitInterval = RealTimeText.INTERVAL_DEFAULT;
            private bool newMsg;
            private UInt32 seq = 0;
            private DelayCalculator delayCalculator = new DelayCalculator();

            /// <summary>Constructor.</summary>
            /// <param name="enableDelays">Flag to enable encoding of delays between key presses</param>
            public Encoder(XmlDocument setDoc, bool enableDelays)
            {
                newMsg = true;
                seq = 0;
                prevMessage.DelaysEnabled = enableDelays;
                doc = setDoc;
                Reset();
            }

            /// <summary>Reinitialize the RTT encoder to a blank string state</summary>
            public void Reset()
            {
                prevMessage.Reset();
                rtt = doc.CreateElement(RealTimeText.ROOT);
            }

            /// <summary>Resets the RTT encoder to starts a new message</summary>
            public void NextMsg()
            {
                Reset();
                newMsg = true;
            }

            /// <summary>Returns the current RTT encoding as an XML element, and then immediately starts a new RTT fragment</summary>
            public XmlElement GetEncodedRTT()
            {
                XmlElement rttEncoded = rtt;
                rttEncoded.SetAttribute("xmlns", RealTimeText.NAMESPACE);
                rttEncoded.SetAttribute("seq", seq.ToString());
                seq++;

                // 'event' attribute.  The first RTT element of a real time message always has event='new'
                if (newMsg)
                {
                    rttEncoded.SetAttribute("event", "new");
                    newMsg = false;
                }

                // Create new RTT element for next message.
                rtt = doc.CreateElement(RealTimeText.ROOT);
                return rttEncoded;
            }

            /// <summary>Encodes RTT fragment for the specified text. This encoder compares this text to the previous state of text, and generates a compact RTT code for this text</summary>
            /// <param name="text">Current state of message text</param>
            /// <param name="cursorPos">Current position index of cursor into text</param>
            public void Encode(string text, int cursorPos)
            {
                if ((text != prevMessage.Text) || (cursorPos != prevMessage.CursorPos))
                {
                    // If necessary, generate the 'g' beep action element
                    if (prevMessage.Beeped)
                    {
                        RealTimeText.AppendElement.Flash(rtt);
                        prevMessage.Beeped = false;
                    }

                    // If necessary, generate the 'w' delay action element
                    if (prevMessage.DelaysEnabled)
                    {
                        if (!rtt.HasChildNodes) delayCalculator.Start(); // Delay calculator starts on first rtt element.

                        int milliseconds = delayCalculator.GetMillisecondsSinceLastCall();
                        if (milliseconds > 0)
                        {
                            if (milliseconds > transmitInterval) milliseconds = transmitInterval; // Limit delays to maximum interval.
                            RealTimeText.AppendElement.Delay(rtt, milliseconds);
                        }
                    }

                    // Encode the text differences since last message, into action elements.
                    RealTimeText.Message newMessage = new RealTimeText.Message(text, cursorPos);
                    RealTimeText.EncodeRawRTT(rtt, prevMessage, newMessage);
                    prevMessage.Text = text;
                    prevMessage.CursorPos = cursorPos;
                }
            }

            /// <summary>Gets the last state of the text</summary>
            public string Text
            {
                get { return prevMessage.Text; }
            }

            /// <summary>Alias for Encoder.Text</summary>
            public override string ToString()
            {
                return prevMessage.Text;
            }

            /// <summary>Get the last state of the cursor position index</summary>
            public int CursorPos
            {
                get { return prevMessage.CursorPos; }
            }

            /// <summary>Get the status of the RTT encoder, if there is any rtt action elements to transmit</summary>
            public bool IsEmpty
            {
                get { return !rtt.HasChildNodes; }
            }

            /// <summary>Transmission interval of real time text</summary>
            public int TransmitInterval
            {
                get { return transmitInterval; }
                set { transmitInterval = value; }
            }

            /// <summary>Enabled/Disabled state of transmission of interval elements, for encoding of pauses between keypresses</summary>
            public bool DelaysEnabled
            {
                get { return prevMessage.DelaysEnabled; }
                set { prevMessage.DelaysEnabled = value; }
            }
        }
    }
}
