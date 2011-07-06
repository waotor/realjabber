using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Xml;

namespace RealJabber.RealTimeTextUtil
{
    /// <summary>
    /// HELPER UTILITY CLASS FOR:
    /// XMPP In-Band Real Time Text Extension (XEPP-0999), Version 0.9, Level 2, http://www.xmpp.org/(pending)
    /// Written by Mark D. Rejhon - mailto:markybox@gmail.com - http://www.marky.com/resume
    /// 
    /// LICENSE
    /// Pending
    ///
    /// NOTICE FOR COMMERCIAL USERS
    /// The copyright of the original (pre-fork) copy of this source code file is (C) Copyright 2011 by Mark D. Rejhon. All Rights reserved.
    /// To purchase the original commercial/proprietary version of this source code file, for your commercial project,
    /// please contact Mark Rejhon at markybox@gmail.com or at http://www.marky.com
    /// 
    /// SPECIFICATION SUPPORTED
    /// The class includes utility methods for encoding/decoding the following real time text (RTT) message editing control codes:
    /// Support for LEVEL 1 (Legacy) and LEVEL 2 (Message Editing) is REQUIRED.  
    /// Support for LEVEL 3 (Natural Typing) is OPTIONAL.
    /// 
    /// LEVEL   ACTION                  CODE    DESCRIPTION
    /// 1       Character quoting       «{»     Output a « character (Unicode U+00AB)
    /// 1       Character quoting       «}»     Output a »  character (Unicode U+00BB)
    /// 1       Backspace               «B»	    Backspace one character at current position of cursor.
    /// 1       Backspace Multi         «B#»    Backspace # characters at current position of cursor.
    /// 2       Delete                  «D»	    Delete one character at current position of cursor
    /// 2       Delete Multi            «D#»    Delete # characters at current position of cursor.
    /// 2       Cursor Position Index   «X#»    Set the cursor position within line, at specific character index # in line.
    /// 2       Clear Line              «K»     Clears the entire line, and puts cursor at the start of line. (position 0)
    /// 3       Delay                   «W#»    Insert an optional inter-character delay of # hundredths of a second. (Natural Typing Mode)
    /// </summary>
    public static class RealTimeTextOrig
    {
        public const string REALTIME_TAG = "rtt";
        public const int INTERVAL_DEFAULT = 500;
        public const string VERSION = "0.9";
        public const string LEVEL = "2";
        public const string PREFIX = "«";
        public const string SUFFIX = "»";
        public const string QUOTEPREFIX = "«{»";
        public const string QUOTESUFFIX = "«}»";
        public const string BACKSPACE = "B";
        public const string DELETE = "D";
        public const string XPOS = "X";
        public const string CLEARLINE = "K";
        public const string DELAY = "W";

        /// <summary>Move cursor to new cursor position index</summary>
        /// <param name="newPos">New cursor position index</param>
        /// <returns>Smallest code to execute cursor position change.  Empty string if no movement, otherwise «H(new pos)»</returns>
        public static string CurPos(int newPos)
        {
            // Set new cursor position.
            return PREFIX + XPOS + newPos.ToString() + SUFFIX;
        }

        /// <summary>Delete # of characters at current cursor position</summary>
        /// <param name="count">Number of delete's</param>
        /// <returns>Code to execute deletion of characters.</returns>
        public static string Delete(int count)
        {
            if (count <= 0)
            {
                return "";
            }
            else if (count == 1)
            {
                return PREFIX + DELETE + SUFFIX;
            }
            else
            {
                return PREFIX + DELETE + count.ToString() + SUFFIX;
            }
        }

        /// <summary>Backspace # of characters at current cursor position</summary>
        /// <param name="count">Number of backspace's</param>
        /// <returns>Code to execute deletion of characters.</returns>
        public static string Backspace(int count)
        {
            if (count <= 0)
            {
                return "";
            }
            else if (count == 1)
            {
                return PREFIX + BACKSPACE + SUFFIX;
            }
            else
            {
                return PREFIX + BACKSPACE + count.ToString() + SUFFIX;
            }
        }

        /// <summary>Returns clear line code</summary>
        /// <returns>Code to execute line clear.</returns>
        public static string ClearLine()
        {
            return PREFIX + CLEARLINE + SUFFIX;
        }

        /// <summary>Returns delay code</summary>
        /// <param name="count">Number of centiseconds to delay (centisecond is 1/100th sec)</param>
        /// <returns>Code to execute delay in specified hundredths of sec.</returns>
        public static string Delay(int countCentiSecs)
        {
            return PREFIX + DELAY + countCentiSecs.ToString() + SUFFIX;
        }

        /// <summary>Insert text in specified cursor position and return new string/new cursor position</summary>
        /// <param name="text">Text to modify</param>
        /// <param name="cursorPos">Cursor position</param>
        /// <param name="insertText">Text to insert</param>
        private static void InsertText(ref string text, ref int cursorPos, string insertText)
        {
            if (insertText.Length > 0)
            {
                text = text.Insert(cursorPos, insertText);
                cursorPos = cursorPos + insertText.Length;
            }
        }

        /// <summary>Encode RTT edit control codes on a string of text to turn one string to the next string
        /// This follows the XMPP Real Time Text Specification, Version 0.9</summary>
        /// <param name="beforeText">Previous state of text</param>
        /// <param name="beforeCurPos">Previous state of cursor position index into text</param>
        /// <param name="afterText">Current state of text</param>
        /// <param name="afterCurPos">Current state of cursor position index into text</param>
        /// <returns>Efficiently encoded RTT fragment to send in XML "rtt" element to change "beforeText" into "afterText"</returns>
        public static string EncodeRawRTT(string beforeText, int beforeCurPos, string afterText, int afterCurPos)
        {
            string codedText = "";
            int curPos = beforeCurPos;

            // Compute alternative start cursor position if caller is not keeping track of cursor position (negative value for beforeCurPos)
            int curPosAlt = beforeCurPos;
            if (curPosAlt < 0)
            {
                curPosAlt = beforeText.Length;
            }

            if (beforeText == afterText)
            {
                // Handle cursor position change only.
                if (beforeCurPos != -1)
                {
                    if (beforeCurPos != afterCurPos)
                    {
                        codedText = CurPos(afterCurPos);
                    }
                }
            }
            else if (afterText == "")
            {
                codedText += ClearLine();
                curPos = 0;
            }
            else
            {
                int leadingSame = 0;
                int trailingSame = 0;
                int i = 0;
                int j = 0;

                // Find number of characters at start that remains the same
                while ((beforeText.Length > i) && (afterText.Length > i))
                {
                    if (beforeText[i] != afterText[i])
                    {
                        break;
                    }
                    i++;
                    leadingSame++;
                }

                // Find number of characters at end that remains the same
                i = beforeText.Length;
                j = afterText.Length;
                while ((i > leadingSame) && (j > leadingSame))
                {
                    i--;
                    j--;
                    if (beforeText[i] != afterText[j])
                    {
                        break;
                    }
                    trailingSame++;
                }

                // If before-cursor-position is a -1, the caller is not keeping track of cursor position.
                // Spec say we should assume end of line.
                curPos = curPosAlt;

                // Delete text if a deletion is detected anywhere in the string.
                int charsRemoved = beforeText.Length - trailingSame - leadingSame;
                if (charsRemoved > 0)
                {
                    int posEndOfRemovedChars = beforeText.Length - trailingSame;
                    int posStartOfRemovedChars = posEndOfRemovedChars - charsRemoved;
                    if (curPosAlt == posEndOfRemovedChars)
                    {
                        // Cursor is more ideally positioned to BACKSPACE removed text
                        if (curPos != posEndOfRemovedChars)
                        {
                            codedText += CurPos(posEndOfRemovedChars);
                            curPos = posEndOfRemovedChars;
                        }
                        codedText += Backspace(charsRemoved);
                        curPos -= charsRemoved;
                    }
                    else
                    {
                        // Cursor is more ideally positioned to DELETE removed text
                        if (curPos != posStartOfRemovedChars)
                        {
                            codedText += CurPos(posStartOfRemovedChars);
                            curPos = posStartOfRemovedChars;
                        }
                        codedText += Delete(charsRemoved);
                    }
                }

                // Move cursor to last changed character (if cursor not already correct location)
                int startPos = beforeText.Length - trailingSame;
                if (curPos != startPos)
                {
                    codedText += CurPos(startPos);
                    curPos = startPos;
                }

                // Retrieve snippet of text to insert, and properly quote all characters that are used as part of escape co
                int charsInserted = afterText.Length - trailingSame - leadingSame;
                string insertedText = afterText.Substring(leadingSame, charsInserted);

                // Quote the edit control prefix/suffixes.
                insertedText = insertedText.Replace(PREFIX, PREFIX + PREFIX).Replace(SUFFIX, PREFIX + SUFFIX);
                insertedText = insertedText.Replace(PREFIX + PREFIX, QUOTEPREFIX).Replace(PREFIX + SUFFIX, QUOTESUFFIX);

                // Output inserted text (if any insertion needed)
                codedText += insertedText;
                curPos += charsInserted;

                // Move cursor to final resting location (if cursor not already there)
                if (curPos != afterCurPos)
                {
                    codedText += CurPos(afterCurPos);
                    curPos = afterCurPos;
                }
            }

#if DEBUG
            // Verify that we encoded properly by executing a decode on the encoded control codes
            int testPos = curPosAlt;
            int delay = 0;
            string testCodedText = codedText;
            string testDecode = DecodeRawRTT(beforeText, ref testCodedText, ref testPos, ref delay, true);

            Debug.Assert(afterText == testDecode);
            if (beforeCurPos >= 0)
            {
                Debug.Assert(testPos == afterCurPos);
            }
            Console.WriteLine("------------");
            Console.WriteLine("Before Text : " + beforeText);
            Console.WriteLine("After Text  : " + afterText);
            Console.WriteLine("CODED Text  : " + codedText);
#endif
            return codedText;
        }

        /// <summary>Decode RTT edit control codes on a string of text.
        /// This follows the XMPP Real Time Text Specification, Version 0.9</summary>
        /// <param name="oldText">String to decode</param>
        /// <param name="controlCodes">Text to modify or append, with control code processing</param>
        /// <param name="cursorPos">Cursor position to update</param>
        /// <param name="ignoreDelayCodes">Ignore delay codes (simulation of typing inter-character delay)</param>
        /// <returns></returns>
        public static string DecodeRawRTT(string text, ref string controlCodes, ref int cursorPos, ref int delay, bool ignoreDelayCodes)
        {
            string controlText = "";
            int prevCursorPos = 0;
            int nextPrefix = 0;
            int nextSuffix = 0;
            uint codeValue = 0;
            int i = 0;
            delay = 0;

            // Verify that controlCodes is not null
            Debug.Assert(controlCodes != null);
            if (controlCodes == null) return text;

            // Verify that cursor is within bounds.
            if (cursorPos < 0) cursorPos = 0;
            if (cursorPos > text.Length) cursorPos = text.Length;

            // Loop to process the control codes string
            while (i < controlCodes.Length)
            {
                // Find START of next control code.
                nextPrefix = controlCodes.IndexOf(PREFIX, i);
                if (nextPrefix < 0)
                {
                    // No more control codes left. Output the rest of the text
                    InsertText(ref text, ref cursorPos, controlCodes.Substring(i));
                    i = controlCodes.Length;
                }
                else
                {
                    // Output text up to the control code.
                    InsertText(ref text, ref cursorPos, controlCodes.Substring(i, nextPrefix - i));

                    // Find END of control code
                    nextSuffix = controlCodes.IndexOf(SUFFIX, nextPrefix + 1);
                    if (nextSuffix < 0)
                    {
                        // No end to control code. Treat as printable text. Output the rest of the text.
                        i = controlCodes.Length;
                        InsertText(ref text, ref cursorPos, controlCodes.Substring(nextPrefix));
                    }
                    else
                    {
                        // Found both start and end of control code.  Process the control code based on XMPP In-Band RTT specification 
                        i = nextSuffix + 1;
                        controlText = controlCodes.Substring(nextPrefix + 1, nextSuffix - nextPrefix - 1);
                        if (controlText.Length <= 0)
                        {
                            InsertText(ref text, ref cursorPos, PREFIX + SUFFIX);
                        }
                        else
                        {
                            try
                            {
                                switch (controlText[0])
                                {
                                    case 'B':
                                        // Backspace	«B#»	Backspace # times. (Editor style behaviour; makes text at right move with cursor.)
                                        if (controlText.Length == 1)
                                        {
                                            codeValue = 1;
                                        }
                                        else
                                        {
                                            codeValue = uint.Parse(controlText.Substring(1));
                                        }
                                        prevCursorPos = cursorPos;
                                        cursorPos -= (int)codeValue;
                                        if (cursorPos < 0)
                                        {
                                            cursorPos = 0;
                                        }
                                        text = text.Substring(0, cursorPos) + text.Substring(prevCursorPos);
                                        break;
                                    case 'D':
                                        // Delete		«D#»	Delete # times. (Editor style behaviour; makes text at right move towards cursor.)
                                        if (controlText.Length == 1)
                                        {
                                            codeValue = 1;
                                        }
                                        else
                                        {
                                            codeValue = uint.Parse(controlText.Substring(1));
                                        }
                                        text = text.Substring(0, cursorPos) + text.Substring(cursorPos + (int)codeValue);
                                        break;
                                    case 'X':
                                        // X Position	«X#»	Cursor X Position. Jump to exact horiziontal cursor position in current line.
                                        if (controlText.Length > 1)
                                        {
                                            cursorPos = int.Parse(controlText.Substring(1));
                                        }
                                        break;
                                    case 'K':
                                        // Clear Line  «K»     Clears entire line. Puts cursor at the start of line.
                                        if (controlText.Length == 1)
                                        {
                                            cursorPos = 0;
                                            text = "";
                                        }
                                        break;
                                    case '{':
                                        // Character quoting	«{»     Output a « character (Unicode U+00AB)
                                        if (controlText.Length == 1)
                                        {
                                            InsertText(ref text, ref cursorPos, PREFIX);
                                        }
                                        break;
                                    case '}':
                                        // Character quoting	«}»		Output a »  character (Unicode U+00BB)
                                        if (controlText.Length == 1)
                                        {
                                            InsertText(ref text, ref cursorPos, SUFFIX);
                                        }
                                        break;
                                    case 'W':
                                        // Delay    «W#»    Insert an optional inter-character delay of # hundredths of a second.
                                        // NOTE: This is an advanced extension to the specification. (Natural Typing Mode)
                                        if (!ignoreDelayCodes)
                                        {
                                            if (controlText.Length > 1)
                                            {
                                                // Delay in 100ths of a second
                                                delay = int.Parse(controlText.Substring(1));
                                                goto ExitLoop;
                                            }
                                        }
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                // Usually NumberFormatException, OutOfBoundsException or similiar exception.
                                Console.Write("RTTUtil.UpdateText() exception: " + ex.ToString());
                            }
                        }
                    }
                }

                // Verify that cursor is within bounds.
                if (cursorPos < 0) cursorPos = 0;
                if (cursorPos > text.Length) cursorPos = text.Length;
            }
        ExitLoop:

            // Blank out the control codes, to indicate we've finished processing the control codes (i.e. delay control code processing)
            if (i > controlCodes.Length)
            {
                controlCodes = "";
            }
            else
            {
                controlCodes = controlCodes.Substring(i);
            }
            return text;
        }
    }

    // ################################################################################################################

    /// <summary>
    /// Utility class to encode real time text (RTT) before sending.
    /// </summary>
    public class RTTencoderOrig
    {
        private string prevText;
        private int prevCursorPos;
        private string rttCodes;
        private Stopwatch ticks;
        private bool delays;
        private int interval = RealTimeText.INTERVAL_DEFAULT;
        bool newMsg;
        UInt32 msg = 0;
        UInt32 seq = 0;

        /// <summary>Constructor.</summary>
        /// <param name="enableDelays">Flag to enable encoding of Natural Typing delays (LEVEL 3)</param>
        public RTTencoderOrig(bool enableDelays)
        {
            Reset();
            newMsg = true;
            msg = 0;
            seq = 0;
            delays = enableDelays;
            ticks = new Stopwatch();
        }

        /// <summary>Reinitialize the RTT encoder to a blank string state</summary>
        public void Reset()
        {
            prevCursorPos = 0;
            prevText = "";
            rttCodes = "";
        }

        /// <summary>Resets the RTT encoder to starts a new message</summary>
        public void NextMsg()
        {
            Reset();
            msg++; // Increment msg counter for the 'msg' attribute
            newMsg = true;
        }

        /// <summary>Returns the current RTT encoding as an XML element, and then immediately starts a new RTT fragment</summary>
        public XmlElement GetEncodedRTT(XmlDocument doc)
        {
            XmlElement rttNode = doc.CreateElement("rtt");
            rttNode.InnerText = rttCodes;
            rttNode.SetAttribute("seq", seq.ToString());
            rttNode.SetAttribute("msg", msg.ToString());
            if (newMsg)
            {
                // The first RTT fragment always has attribute 'type' with a value of 'new'
                rttNode.SetAttribute("type", "new");
                newMsg = false;
            }
            seq++; // Increment seq counter for the 'seq' attribute
            rttCodes = "";
            ticks.Reset();
            ticks.Start();
            return rttNode;
        }

        /// <summary>Encodes RTT fragment for the specified text. This encoder compares this text to the previous state of text, and generates a compact RTT code for this text</summary>
        /// <param name="text">Current state of message text</param>
        /// <param name="cursorPos">Current position index of cursor into text</param>
        public void Encode(string text, int cursorPos)
        {
            if ((text != prevText) || (cursorPos != prevCursorPos))
            {
                if (delays)
                {
                    int centisecs = (int)(ticks.ElapsedMilliseconds / 10);
                    if (centisecs > 0)
                    {
                        if (centisecs > (interval / 10))
                        {
                            centisecs = (interval / 10);
                        }
                        // Add a delay code, but not longer than the RTT interval
                        rttCodes += RealTimeTextOrig.Delay(centisecs);
                        ticks.Reset();
                        ticks.Start();
                    }
                }
                rttCodes += RealTimeTextOrig.EncodeRawRTT(prevText, prevCursorPos, text, cursorPos);
                prevText = text;
                prevCursorPos = cursorPos;
            }
        }

        /// <summary>Gets the last state of the text</summary>
        public string Text
        {
            get { return prevText; }
        }
        
        /// <summary>Alias for RTTencoder.Text</summary>
        public override string ToString()
        {
            return prevText;
        }

        /// <summary>Get the last state of the cursor position index</summary>
        public int CurPos
        {
            get { return prevCursorPos; }
        }

        /// <summary>Get the current encoded RTT fragment, for use for transmission</summary>
        public string Codes
        {
            get { return rttCodes; }
        }

        /// <summary>Get the current real time text interval</summary>
        public int Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        /// <summary>Enable transmission of delay codes</summary>
        public bool Delays
        {
            get { return delays; }
            set { delays = value; }
        }
    }

    // ################################################################################################################

    /// <summary>
    /// Utility class to decode real time text (RTT) upon receiving.
    /// This class is thread-safe for multithreading, to optionally support inter-key press delays (Natural Typing Mode)
    /// </summary>
    public class RTTdecoderOrig
    {
        private string user;
        private string text;
        private string rttCodes;
        private int cursorPos;
        private int delay;
        private bool sync;
        private UInt32 seq;
        private UInt32 msg;

        // Member variables to support Natural Typing delay codes (LEVEL 3) This requires asynchronous decoding, in a timer-driven manner.
        private TimerCallback timerDelegate;
        private Timer timer;

        /// <summary>Callback event for asynchronous RTT decoding (This mainly used for Natural Typing mode)</summary>
        public event TextUpdatedHandler TextUpdated = null;
        public delegate void TextUpdatedHandler(RTTdecoderOrig decoder);

        /// <summary>Callback event that is called everytime sync state changes (loss of sync, caused by missing or out-of-order rtt packets)</summary>
        public event SyncStateChangedHandler SyncStateChanged = null;
        public delegate void SyncStateChangedHandler(bool isInSync);

        /// <summary>Constructor</summary>
        /// <param name="setUser">XMPP user for this Real Time Text (RTT) decoder</param>
        public RTTdecoderOrig(string setUser)
        {
            user = setUser;
            timerDelegate = new TimerCallback(DecodeTimer);
            timer = new Timer(timerDelegate);
            Reset();
        }

        /// <summary>Reinitialize the RTT decoder to a blank string state (new message)</summary>
        public void Reset()
        {
            lock (user)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                text = "";
                rttCodes = "";
                cursorPos = 0;
                delay = 0;
                sync = true;
            }
        }

        /// <summary>Add RTT text to the decode queue</summary>
        /// <param name="rtt">Text that may also contain RTT editing control codes</param>
        /// <returns>true if successfully buffered</returns>
        public bool Decode(XmlNode rtt)
        {
            if (sync && (rtt != null))
            {
                lock (user)
                {
                    try
                    {
                        XmlAttribute typeAttr = rtt.Attributes["type"];
                        XmlAttribute seqAttr = rtt.Attributes["seq"];
                        XmlAttribute msgAttr = rtt.Attributes["msg"];
                        if (seqAttr == null)
                        {
                            // Missing "seq" attribute is specified as an OUT-OF-SYNC condition
                            sync = false;
                        }
                        else if (msgAttr == null)
                        {
                            // Missing "msg" attribute is specified as an OUT-OF-SYNC condition
                            sync = false;
                        }
                        else
                        {
                            UInt32 seqNewValue = UInt32.Parse(seqAttr.Value);
                            UInt32 msgNewValue = UInt32.Parse(msgAttr.Value);
                            if (typeAttr != null)
                            {
                                if (typeAttr.Value.ToLower() == "new")
                                {
                                    // Begin a new RTT message. This brings us into SYNC.
                                    Reset();
                                    seq = seqNewValue;
                                    msg = msgNewValue;
                                    rttCodes += rtt.InnerText;
                                    if (SyncStateChanged != null) SyncStateChanged(true);
                                }
                                else
                                {
                                    // Undocumented 'type' attribute value is specified as an OUT-OF-SYNC condition.
                                    sync = false;
                                }
                            }
                            else if (msgNewValue != msg)
                            {
                                // Change in 'msg' attribute without a 'type' defined, is specified as an OUT-OF-SYNC condition.
                                sync = false;
                            }
                            else if (seqNewValue != (UInt32)(seq + 1))
                            {
                                // Non-consecutive increment in 'seq', is specified as an OUT-OF-SYNC condition.
                                sync = false;
                            }
                            else
                            {
                                // Append to existing RTT message queue
                                seq = seqNewValue;
                                rttCodes += rtt.InnerText;
                            }
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
                }
                return sync;
            }
            return false;
        }

        /// <summary>Decodes all RTT text immediately</summary>
        /// <returns>Resulting decoded message</returns>
        public string FullDecodeNow()
        {
            lock (user)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                if (!String.IsNullOrEmpty(rttCodes))
                {
                    text = RealTimeTextOrig.DecodeRawRTT(text, ref rttCodes, ref cursorPos, ref delay, true);
                }
                return text;
            }
        }

        /// <summary>Asynchronous decoder with delay-code awareness</summary>
        public void AsyncDecodeWithDelays()
        {
            lock (user)
            {
                if (!String.IsNullOrEmpty(rttCodes))
                {
                    text = RealTimeTextOrig.DecodeRawRTT(text, ref rttCodes, ref cursorPos, ref delay, false);
                    if (delay != 0)
                    {
                        timer.Change(delay * 10, Timeout.Infinite);
                    }
                    // Calls the user-defined event for one step of text decode
                    if (TextUpdated != null)
                    {
                        TextUpdated(this);
                    }
                }
            }
        }

        /// <summary>Timer event for RTT delay codes</summary>
        private void DecodeTimer(Object info)
        {
            AsyncDecodeWithDelays();
        }
        
        /// <summary>Gets the XMPP user of this real time text object</summary>
        public string User
        {
            get
            {
                lock (user)
                {
                    return user;
                }
            }
        }

        /// <summary>Gets the current state of the message string</summary>
        public string Text
        {
            get
            {
                lock (user)
                {
                    return text;
                }
            }
        }

        /// <summary>Alias for RTTdecoder.Text</summary>
        public override string ToString()
        {
            lock (user)
            {
                return text;
            }
        }

        /// <summary>Gets the current character index position of the cursor</summary>
        public int CurPos
        {
            get
            {
                lock (user)
                {
                    return cursorPos;
                }
            }
        }

        /// <summary>Gets the current delay, used for inter-character keypress delay</summary>
        public int Delay
        {
            get
            {
                lock (user)
                {
                    return delay;
                }
            }
        }

        /// <summary>Returns true if there's no RTT codes left to decode</summary>
        public bool Complete
        {
            get
            {
                lock (user)
                {
                    return String.IsNullOrEmpty(rttCodes);
                }
            }
        }
    }
}
