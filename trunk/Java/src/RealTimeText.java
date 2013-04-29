import java.io.IOException;
import java.util.Vector;
import java.util.Random;
import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserException;

/** 
 * XML-based Real-Time Text Encoding/Decoding Module (Java version)     <br>  
 * Designed for XEP-0301 -- Jabber/XMPP Extension Protocol              <br>
 * http://www.xmpp.org/extensions/xep-0301.html
 * 
 * @author Mark D. Rejhon - <a href="mailto:mark@realjabber.org">mark@realjabber.org</a> - <a href="http://www.realjabber.org">www.realjabber.org</a><br>
 *          Extension: <a href="http://www.xmpp.org/extensions/xep-0301.html">www.xmpp.org/extensions/xep-0301.html</a><br>
 *          Resume: <a href="http://www.marky.com/resume">www.marky.com/resume</a><br>
 */
// DESCRIPTION:
//   -- Real-time text (RTT): Text transmitted instantly while it is being typed or created.
//   The recipient can immediately read the sender's text as it is written, without waiting.
//   -- Real-time text is used for general chat, improved text messaging, enhancement to instant messaging, 
//   voice transcription, court reporting, streaming news tickers, IP relay services, live closed captioning 
//   and live subtitles, gateways for TDD/TTY/text telephone for the deaf, captioned telephones, and more.<br> 
//   -- Although this module is designed for use with XMPP via XEP-0301, this module can also 
//   be used independently as general-purpose XML based encoder/decoder for any form of real-time text.
//   This module is not dependant on any specific XMPP library, and can be used with any XMPP libary.
//   -- For a FAQ, and for other versions of this module (C#, Java, etc) see http://www.realjabber.org
//
//       LANGUAGE: Java
//     XML PARSER: xmlpull
//   XMPP LIBRARY: No internal dependency (Recommended: smack for Java/JSP, asmack for Android)
//         AUTHOR: Mark D. Rejhon - mailto:mark@realjabber.org - http://www.realjabber.org
//      COPYRIGHT: Copyright 2012 by Mark D. Rejhon - http://www.marky.com/resume
//        LICENSE: Apache 2.0 - Open Source - Commercial usage is permitted.
//
// LICENSE
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//
// FUNDING OF DEVELOPMENT
//   This Java module was initially supported with funding from the National Institute 
//   on Disability and Rehabilitation Research (NIDRR), U.S. Department of Education, 
//   under grants H133E090001 and H133E080022. However, no endorsement of the 
//   funding agency should be assumed.
//
// TABLE OF ACTION ELEMENTS
//   For more information, see http://www.xmpp.org/extensions/xep-0301.html
//   ----------------------------------------------------------------------
//   ACTION           CODE               DESCRIPTION
//   Insert Text      <t p='#'>text</t>  (REQUIRED) Insert text at position p in message. Also good for block inserts (i.e. pastes).
//   Erase Text       <e p='#' n='#'/>   (REQUIRED) Deletes n characters of text to the left of position p in message.
//   Wait Interval    <w n='#'/>         (RECOMMENDED) Key press intervals. Execute a pause of n thousandths of a second. Allows multiple smooth keypresses in one packet.
//
public class RealTimeText {
    // Specification constants
    public static final String ROOT = "rtt";
    public static final String NAMESPACE = "urn:xmpp:rtt:0";
    public static final String VERSION = "0.6";

    /** @summary The default recommended transmission interval (in milliseconds) for real time text */
    public static final int DEFAULT_TRANSMIT_INTERVAL = 700;

    /** @summary The default recommended redundancy interval (in milliseconds) for error recovery */
    public static final int DEFAULT_REDUNDANCY_INTERVAL = 10000;

    /** @summary Debug: Displays XML of RTT elements to console */
    public static final boolean DEBUG_CONSOLE_XML = true;
    
    // ---------------------------------------------------------------------------------------------
    /** Class representing the state of a real time message */
    static public class Message implements Cloneable {
        private String text = "";
        private int pos = 0;
        private int currentKeyInterval = 0;
        private boolean enableKeyIntervals = false;

        /** Main constructor */
        public Message() {}
        
        /** Constructor that specifies message text and cursor position */
        public Message(String textValue, int posValue)
        {
            text = textValue;
            pos = posValue;
        }
        
        /** Reset real time message */
        public void reset() {
            text = "";
            pos = 0;
            currentKeyInterval = 0;
        }

        /** Copy real time message */
        public Message copy() throws CloneNotSupportedException {
            return (Message) this.clone();
        }

        /** get message text */
        public String getText() {
            return text;
        }

        /** set message text */
        public void setText(String textValue) {
            text = textValue;
        }

        /** get optional cursor position index */
        public int getCursorPos() {
            return pos;
        }

        /** set optional cursor position index */
        public void setCursorPos(int posValue) {
            pos = posValue;
        }

        /** get current key interval values (the "W" action element) */
        public int getCurrentKeyInterval() {
            return currentKeyInterval;
        }

        /** set current key interval values (the "W" action element) */
        public void setCurrentKeyInterval(int value) {
            currentKeyInterval = value;
        }

        /** get enable/disable processing of key press interval action elements */
        public boolean getKeyIntervalsEnabled() {
            return enableKeyIntervals;
        }

        /** set enable/disable processing of key press interval action elements */
        public void setKeyIntervalsEnabled(boolean value) {
            enableKeyIntervals = value;
        }        
    }

    // ---------------------------------------------------------------------------------------------
    /** Class representing one action element */
    static public class ActionElement {
        private String element = null; // Should be one of the supported action elements
        private Integer p = null; // Position value
        private Integer n = null; // Count value
        private String text = null; // Valid only for the 't' element.

        /** Constructor */
        public ActionElement() {
        }

        /** Constructor with element name */
        public ActionElement(String elementValue) {
            element = elementValue;
        }

        /** Element name getter */
        public String getElement() {
            return element;
        }

        /** Element name setter */
        public void setElement(String elementValue) {
            element = elementValue;
        }

        /** Position getter */
        public Integer getPosition() {
            return p;
        }

        /** Position setter */
        public void setPosition(Integer posValue) {
            p = posValue;
        }

        /** Count getter */
        public Integer getCount() {
            return n;
        }

        /** Count setter */
        public void setCount(Integer countValue) {
            n = countValue;
        }

        /** Inner Text getter */
        public String getText() {
            return text;
        }

        /** Inner Text setter */
        public void setText(String textValue) {
            text = textValue;
        }

        /** Outputs this action element class into raw XML text */
        public String toXml() {
            if (element == null) throw new NullPointerException();

            StringBuilder rawXml = new StringBuilder("<" + element);
            if (p != null) rawXml.append(" p='" + p.toString() + "'");
            if (n != null) rawXml.append(" n='" + n.toString() + "'");
            if ((text != null) && !text.isEmpty()) {
                rawXml.append(">" + text + "</" + element + ">");
            } else {
                rawXml.append("/>");
            }
            return rawXml.toString();
        }

        /** Parses an XmlPullParser for an action element.
         * Upon entry: Parser must point to the start tag.
         * Upon return: Parser will point to the end tag.
         */
        public boolean fromXml(XmlPullParser parser)
                throws XmlPullParserException, IOException {
            boolean success = false;
            int eventType = parser.getEventType();
            if (eventType == XmlPullParser.START_TAG) {

                // Retrieve action element
                this.element = parser.getName();

                // Default values for n and p
                this.n = null;
                this.p = null;

                // Process attributes for n and p values
                for (int i = 0; i < parser.getAttributeCount(); i++) {
                    String attribName = parser.getAttributeName(i);
                    String attribValue = parser.getAttributeValue(i);
                    try {
                        if (attribName.equals("n")) {
                            this.n = Integer.parseInt(attribValue);
                        } else if (attribName.equals("p")) {
                            this.p = Integer.parseInt(attribValue);
                        }
                    } catch (NumberFormatException e) {
                        // Unsuccessful conversion of integer attribute. Ignore
                        // these.
                    }
                }

                if (parser.isEmptyElementTag()) {
                    this.text = "";
                    parser.next();
                } else {
                    // Process recursively to retrieve inner plaintext, while
                    // ignoring inner XML (not valid for action elements)
                    StringBuilder buffer = new StringBuilder();
                    int startDepth = parser.getDepth();
                    boolean done = false;
                    while (!done) {
                        parser.next();
                        switch (parser.getEventType()) {
                        case XmlPullParser.START_TAG:
                            // Ignore nested start tags
                            break;
                        case XmlPullParser.TEXT:
                            // Plain text is collected
                            buffer.append(parser.getText());
                            break;
                        case XmlPullParser.END_TAG:
                            // This shouldn't happen, but loop is finished if
                            // we're reaching an end tag below start depth.
                            if (parser.getDepth() == startDepth) {
                                done = true;
                                success = true;
                                // parser.next();
                                break;
                            }
                            break;
                        case XmlPullParser.END_DOCUMENT:
                            // Loop is finished if we unexpectedly reach end of
                            // document
                            done = true;
                            break;
                        }
                    }
                    this.text = buffer.toString();
                }
            }
            return success;
        }
    }

    // ---------------------------------------------------------------------------------------------
    /** Class representing the root rtt element */
    static public class RootElement {
        private String xmlns = RealTimeText.NAMESPACE;
        private String event = null;
        private int seq = 0;
        private Vector<ActionElement> actions = new Vector<ActionElement>();

        /** Get xmlns attribute */
        public String getXmlns() {
            return xmlns;
        }

        /** Set xmlns attribute */
        public void setXmlns(String xmlnsValue) {
            xmlns = xmlnsValue;
        }

        /** Get event attribute */
        public String getEvent() {
            return event;
        }

        /** Set event attribute */
        public void setEvent(String eventValue) {
            event = eventValue;
        }

        /** Get seq value */
        public int getSeq() {
            return seq;
        }

        /** Set seq value */
        public void setSeq(int seqValue) {
            seq = seqValue;
        }

        /** Provide action elements vector */
        public Vector<ActionElement> getActionElements() {
            return actions;
        }
        
        /** Returns true if no action elements in this RTT element */
        public boolean isEmpty() {
            return actions.isEmpty();
        }

        /** Outputs in XML format as an RTT element containing action elements */
        public String toXML() {
            // Create the <rtt> element
            StringBuilder xml = new StringBuilder("<rtt");

            // Add the <rtt> attributes
            if ((xmlns != null) && !xmlns.isEmpty()) {
                xml.append(" xmlns='" + xmlns + "'");
            }
            if ((event != null) && !event.isEmpty()) {
                xml.append(" event='" + event + "'");
            }
            xml.append(" seq='" + (new Integer(seq)).toString() + "'");

            // Add the action elements, if any
            if (!actions.isEmpty()) {
                xml.append(">");
                for (int i = 0; i < actions.size(); i++) {
                    xml.append(actions.get(i).toXml());
                }
                xml.append("</rtt>");
            } else {
                // Make <rtt> a unary tag if no action elements
                xml.append("/>");
            }

            return xml.toString();
        }

        /**
         * Parses an XmlPullParser for an rtt element.<br>
         * Upon entry: Parser must point to the start tag.<br>
         * Upon return: Parser will point to the end tag.
         */
        public boolean fromXml(XmlPullParser parser)
                throws XmlPullParserException, IOException {
            boolean success = false;

            actions.clear();
            this.xmlns = "";
            this.event = null;
            this.seq = -1;

            int eventType = parser.getEventType();
            if (eventType == XmlPullParser.START_TAG) {
                this.xmlns = parser.getNamespace();
                if (parser.getName().equals(RealTimeText.ROOT)) {
                    // Process attributes for xmlns, event and seq attributes
                    for (int i = 0; i < parser.getAttributeCount(); i++) {
                        String attribName = parser.getAttributeName(i);
                        String attribValue = parser.getAttributeValue(i);
                        try {
                            if (attribName.equals("event")) {
                                this.event = attribValue.toLowerCase();
                            } else if (attribName.equals("seq")) {
                                this.seq = Integer.parseInt(attribValue);
                            }
                        } catch (NumberFormatException e) {
                            // Unsuccessful conversion of integer attribute.
                            // Ignore these.
                        }
                    }

                    // Loop to parse all action elements inside the rtt element.
                    int startDepth = parser.getDepth();
                    parser.next();
                    while ((parser.getDepth() > startDepth)
                            && (parser.getEventType() != XmlPullParser.END_DOCUMENT)) {
                        ActionElement action = new ActionElement();
                        action.fromXml(parser);
                        parser.next();
                        this.actions.add(action);
                    }

                    // Closing rtt tag
                    if (parser.getEventType() == XmlPullParser.END_TAG) {
                        if (parser.getName().equals(RealTimeText.ROOT)) {
                            success = true;
                        }
                    }
                }
            }
            return success;
        }
    }

    // ---------------------------------------------------------------------------------------------
    /** Class to generate action elements */
    static public class AppendElement {
        /**
         * T Element - Insert Text - Inserts text at specified position, or appends text to end of line
         * 
         * @param text Text to insert
         * @param pos Position to insert text at
         * @param len Length of original text to insert into (for automatic omission of 'pos' if at end of string)
         * @return Action element represented
         */
        public static void insertText(Vector<ActionElement> actions, String text, int pos, int len) {
            if ((text == null) || text.isEmpty()) return;

            ActionElement action = new ActionElement("t");
            if ((pos >= 0) && (pos < len)) {
                action.setPosition(pos);
            }
            action.setText(text);
            actions.add(action);
        }

        /**
         * E Element - Erase Text - Erase # characters before specified position
         * 
         * @param pos Position to begin backspacing at
         * @param count Count of characters to backspaces
         * @param len Length of original string
         * @return Action element represented
         */
        public static void eraseText(Vector<ActionElement> actions, int pos, int count, int len) {
            if (count <= 0) return;

            ActionElement action = new ActionElement("e");
            if (count != 1) {
                action.setCount(count);
            }
            if ((pos >= 0) && (pos < len)) {
                action.setPosition(pos);
            }
            actions.add(action);
        }

        /**
         * W Element - Wait Interval - Pause specified amount (i.e. intervals between key presses)
         * 
         * @param milliseconds Number of thousandths of seconds to delay
         * @return Action element represented
         */
        public static void waitInterval(Vector<ActionElement> actions, int milliseconds) {
            ActionElement action = new ActionElement("w");
            action.setCount(milliseconds);
            actions.add(action);
        }

        /**
         * Optional Cursor Position using an empty T element (Insert Text) using the 'p' attribute.
         * 
         * @param pos New cursor position index
         * @return Action element represented
         */
        public static void cursorPosition(Vector<ActionElement> actions, int pos) {
            ActionElement action = new ActionElement("t");
            action.setPosition(pos);
            actions.add(action);
        }        
    }

    /**
     * Decode Action Elements into a message string.
     * 
     * @param rtt The rtt element containing Action Elements
     * @param message Real time message to update
     * @return New text of message <br>
     *         ('message' is updated, processed elements are removed from 'rtt')
     */
    static public String DecodeRawRTT(RootElement rtt, Message message) {
        String text = message.text;
        int count;
        int pos;
        boolean quit = false;
        ActionElement action;
        
        message.setCurrentKeyInterval(0);

        // Verify rtt element is defined
        if (rtt == null) return message.text;
        if (rtt.actions.isEmpty()) return message.text;

        // Loop to process the XML Action Elements
        while (!quit && !rtt.actions.isEmpty()) {
            try {
                action = rtt.actions.remove(0);

                // Get the "p" attribute (cursor position). p always default to
                // end of line if not provided.
                pos = (action.p == null) ? text.length() : action.p;
                if (pos > text.length()) pos = text.length();
                if (pos < 0) pos = 0;

                // Get the "n" attribute (count). n always default to 1 if not
                // provided.
                count = (action.n == null) ? 1 : action.n;
                if (count < 0) count = 0;

                switch (action.element.charAt(0)) {
                // --------------------------------------------------------------------
                case 't': // Insert Text action element: <t p="#">text</i>
                    message.pos = pos;  // Reposition cursor even if <t/> is empty.
                    if ((action.text != null) && !action.text.isEmpty())
                    {
                        text = text.substring(0, message.pos) + action.text + text.substring(message.pos);
                        message.pos = message.pos + action.text.length();
                    }
                    break;
                // --------------------------------------------------------------------
                case 'e': // Erase Text action element: <e p="#" n="#"/>
                    message.pos = pos;
                    if (count > message.pos) count = message.pos;
                    text = text.substring(0, message.pos - count) + text.substring(message.pos);
                    message.pos -= count;
                    break;
                // --------------------------------------------------------------------
                case 'w': // Wait Interval action element <w n="#"/>
                    if (message.enableKeyIntervals) {
                        message.currentKeyInterval = count;
                        quit = true;
                        // We exit the loop so that the caller can decide to use a timer for the delay code,
                        // and come back to this loop later to finish remaining nodes that have not finished processing.
                    }
                    break;
                }
            } catch (Exception ex) {
                System.out.println("EXCEPTION: DecodeRawRTT - " + ex.toString());
            }
        }
        message.text = text;
        return text;
    }

    /**
     * Encode RTT edit action elements on a string of text to turn one string to
     * the next string. This follows the XMPP Real Time Text Specification.
     * 
     * @param rtt The rtt element
     * @param before Previous real time message
     * @param after Current real time message
     * @returns Efficiently encoded RTT fragment to send in XML "rtt" element to change "before" into "after"
     */
    static public void EncodeRawRTT(RootElement rtt, Message before, Message after) {        
        if (after.pos < 0) after.pos = 0;
        if (after.pos > after.text.length()) after.pos = after.text.length();        

        int curPos = before.pos;
        boolean textChanged = !before.text.equals(after.text);
        boolean posChanged = (before.pos != after.pos);
        if (!textChanged && !posChanged) return;
                
         if (textChanged) {
            // Before text and after text are different
            int leadingSame = 0;
            int trailingSame = 0;
            int i = 0;
            int j = 0;

            // Find number of characters at start that remains the same
            while ((before.text.length() > i) && (after.text.length() > i)) {
                if (before.text.charAt(i) != after.text.charAt(i)) {
                    break;
                }
                i++;
                leadingSame++;
            }

            // Find number of characters at end that remains the same
            i = before.text.length();
            j = after.text.length();
            while ((i > leadingSame) && (j > leadingSame)) {
                i--;
                j--;
                if (before.text.charAt(i) != after.text.charAt(j)) {
                    break;
                }
                trailingSame++;
            }

            // Erase text if a deletion is detected anywhere in the string.
            int charsRemoved = before.text.length() - trailingSame - leadingSame;
            if (charsRemoved > 0) {
                int posEndOfRemovedChars = before.text.length() - trailingSame;
                int posStartOfRemovedChars = posEndOfRemovedChars - charsRemoved;
                // Do <e/> ERASE TEXT action element to delete text block
                AppendElement.eraseText(rtt.actions, posEndOfRemovedChars, charsRemoved, before.text.length());
                curPos = posStartOfRemovedChars;
            }

            // Do <t/> INSERT TEXT action element if any text insertion is detected anywhere in the string
            int charsInserted = after.text.length() - trailingSame - leadingSame;
            try {
                String insertedText = after.text.substring(leadingSame, leadingSame + charsInserted);
                AppendElement.insertText(rtt.actions, insertedText, curPos, before.text.length());
            } catch (Exception ex) {
                System.out.println("Oh no!! ... " + ex.toString());
            }
            curPos += charsInserted;
        }

        // To assist in the optional remote cursor, do a blank <t/> INSERT TEXT action element
        if ((before.pos != -1) &&
            (curPos != after.pos))
        {
            AppendElement.cursorPosition(rtt.actions, after.pos);
        }
    }

    // ---------------------------------------------------------------------------------------------
    /**
     * Class to calculate very accurate delay intervals, used for recording
     * intervals between actions (such as the pauses between key presses) for
     * encoding & decoding of the 'w' action element during real time text
     * communications. Delay interval values are calculated using an accumulated
     * delay total since the beginning of an &lt;rtt&rt; element. This
     * calculation is much more accurate than simply trying to directly measure
     * the time between actions, and more immune to system/performance
     * variances. Timers and other methods of delay calculations, are more prone
     * to software/CPU performance variations. The calculations in this class is
     * immune to system & software performance variations AND is also immune to
     * accumulated rounding errors.
     * NOTE: This ultra-precision is NOT REQUIRED for XEP-0301. However, it improves quality.
     */
    static class DelayCalculator {
        private long startTime = 0;
        private long delayTotal = 0;

        /** Start the delay measurement */
        public void start() {
            startTime = System.currentTimeMillis();
            delayTotal = 0;
        }

        /** Stops the delay measurement */
        public void stop() {
            startTime = 0;
        }

        /** Returns true if the delay measurement is running */
        public boolean isRunning() {
            return (startTime != 0);
        }

        /**
         * For Decoding: Adjusts a delay interval value, that self-compensates
         * for system/software performance This means, the more 'late' this
         * function is called (i.e. slow performance, etc) the returned value is
         * automatically smaller. In the ideal situation (fast performance) and
         * typical usage, the aim is that the returned value will typically
         * exactly equal delayInterval.
         * 
         * @param currentKeyInterval Recommended delay interval. (typically, from an interval action element)
         * @returns Adjusted delay interval
         */
        public int getCompensatedDelay(int delayInterval) {
            // Running total of delay intervals
            delayTotal += delayInterval;

            // Calculates recommended interval
            int newInterval = (int) (delayTotal - (System.currentTimeMillis() - startTime));
            if (newInterval < 0) newInterval = 0;
            return newInterval;
        }

        /**
         * For Encoding: Gets the number of milliseconds elapsed since the last
         * call to this method. Self-compensating for rounding errors by using a
         * delay total to help average out the errors. Example: Repeatedly
         * calling this function regularly at a fractional millisecond interval,
         * such as every 11.5ms will alternate between adjacent values such as
         * 11, then 12, then 11, and so on.
         * 
         * @returns Number of milliseconds since last call
         */
        public int getMillisecondsSinceLastCall() {
            int newInterval = (int) (System.currentTimeMillis() - startTime - delayTotal);
            delayTotal += newInterval;
            return newInterval;
        }
    }

    // ---------------------------------------------------------------------------------------------
    /** Class to decode incoming real time text (action elements) upon receiving 
     *  This class is thread-safe for multithreading, to optionally support key press intervals.
     */
    static public class Decoder implements Runnable {

        private Message message = new Message();
        private Vector<RootElement> rttElementQueue = new Vector<RootElement>();
        private boolean sync = false;
        private boolean activated = false;
        private int seq = 0;

        /** Listener for decoder events */
        public EventListener decoderListener = null;
        public interface EventListener {
            /** Callback event for asynchronous RTT decoding (supports playback of typing at original key press intervals) */
            public void rttTextUpdated();
            /** Callback event that is called every time sync state changes (error recovery for loss of sync, including lost messages) */
            public void rttSyncStateChanged(boolean isInSync);
            /** Callback event that is called every time real-time text is activated or deactivated */
            public void rttActivationChanged(boolean isActivated);
        }

        private boolean enableThread = false;
        private Object rttThreadEvent = new Object();
        private static final int IDLE_THREAD_QUIT_INTERVAL = 5000;

        /** Constructor */
        public Decoder(EventListener listener)
        {
            decoderListener = listener; 
            reset();
        }
        
        /** Stops the decoder thread */
        public void dispose()
        {
            synchronized (rttElementQueue) {
                enableThread = false;
                synchronized (rttThreadEvent) {
                    rttThreadEvent.notify();
                }
            }
        }
        
        /** Resets the real time message */
        public void reset() {
            synchronized (rttElementQueue) {
                rttElementQueue.clear();
                message.reset();
                message.setKeyIntervalsEnabled(true);
                sync = true;
                synchronized (rttThreadEvent) {
                    rttThreadEvent.notify();
                }
            }
        }

        /** Stop the background decoder */
        private void triggerDecodeThread()
        {
            synchronized (rttElementQueue) {
                if (!enableThread) {
                    enableThread = true;
                    new Thread(this).start();
                    System.out.println("...STARTED rtt decoder thread");
                }
                else {
                    synchronized (rttThreadEvent) {
                        rttThreadEvent.notify();
                    }
                }
            }
        }

        /** Background decode thread */
        public void run()
        {
            RootElement rtt;
            boolean elementsFound = false;
            int interval = 0;
            DelayCalculator delayCalculator = new DelayCalculator();

            while (true) {
                synchronized (rttElementQueue) {
                    if (!enableThread) break;
                    
                    elementsFound = (rttElementQueue.size() > 0);
                    if (elementsFound) {
                        // Start the delay calculator on the moment of the first RTT element received into empty queue
                        if (!delayCalculator.isRunning()) delayCalculator.start();

                        // Loop to decode RTT elements
                        while (enableThread && !rttElementQueue.isEmpty()) {
                            rtt = rttElementQueue.get(0);
                            RealTimeText.DecodeRawRTT(rtt, message);
                            if (rtt.isEmpty()) rttElementQueue.remove(0);

                            if (message.currentKeyInterval != 0) {
                                interval = message.currentKeyInterval;
                                break;
                            }
                        }
                    }
                    if (!enableThread) break;
                }

                // (MUST be outside 'synchronized' section) Calls the user-defined event for one step of text decode 
                if (elementsFound && (decoderListener != null)) {
                    decoderListener.rttTextUpdated();
                }

                if (interval > 0) {
                    // Calculate the delay interval and then sleep for that interval if needed. 
                    // IMPORTANT: This SAVES BATTERY on mobile devices! (avoids unnecessary processing)
                    interval = delayCalculator.getCompensatedDelay(interval);
                    if (interval > 0) {
                        try {
                            Thread.sleep(interval);
                        } catch (InterruptedException e) {
                            e.printStackTrace();
                        }
                    }
                    interval = 0;
                } else {
                    // Zero interval. Stop the delay calculator, and suspend this thread until the next RTT element
                    // IMPORTANT: This also SAVES BATTERY on mobile devices! (turns off unnecessary timers when idling with no typing going on)
                    synchronized (rttThreadEvent) {
                        delayCalculator.stop();
                        try {
                            rttThreadEvent.wait(IDLE_THREAD_QUIT_INTERVAL);
                        } catch (InterruptedException e) {
                            e.printStackTrace();
                        }
                    }

                    // Exit thread if we've waited long enough with no elements arrived. Thread will restart automatically.
                    // This saves system resources (and battery life on mobiles) if there's lots of chat windows.
                    synchronized (rttElementQueue) {
                        if (rttElementQueue.isEmpty()) {
                            enableThread = false;
                            break;
                        }
                    }
                }
            }
            System.out.println("...STOPPED rtt decoder thread");
        }

        /** Add RTT text to the decode queue
         * 
         * @param rtt RTT element that contains RTT action elements
         * @returns true if successfully buffered
         */
        public boolean decode(RootElement rttNew)
        {
            if (rttNew == null) return false;
            if (DEBUG_CONSOLE_XML) System.out.println("INCOMING RTT: " + rttNew.toXML());
            
            synchronized (rttElementQueue) {
                boolean oldSync = sync;
                boolean oldActivated = activated;
                try {
                    if (rttNew.seq < 0) {
                        sync = false;
                    } else if (rttNew.event != null) {
                        if ("new".equals(rttNew.event) || "reset".equals(rttNew.event)) {
                            // New/Reset RTT message. This brings us into SYNC.
                            reset();
                            activated = true;
                            seq = rttNew.seq;
                            rttElementQueue.add(rttNew);
                        } else if ("cancel".equals(rttNew.event)) {
                            activated = false;
                        } else {
                            // Undocumented 'event' attribute value is specified as an OUT-OF-SYNC condition.
                            sync = false; 
                        }
                    } else if (rttNew.seq != (this.seq + 1)) {
                        // Non-consecutive increment in 'seq' is specified as an OUT-OF-SYNC condition.
                        sync = false;
                    } else {
                        // Append to existing RTT message queue
                        seq = rttNew.seq;
                        rttElementQueue.add(rttNew);
                    }
                } catch (Exception ex) {
                    // Any exception occurring at this point, should be assumed as an OUT-OF-SYNC condition. (i.e. NumberFormatException, etc)
                    System.out.println("EXCEPTION: thrown in RTTdecode.Queue: " + ex.toString());
                    this.sync = false;
                }

                // If the activation state changed (real time text started/stopped), notify our event.
                if (activated != oldActivated) {
                    if (decoderListener != null) decoderListener.rttActivationChanged(activated);
                }

                // If the sync state changed (we lose sync with real time text), notify our event.
                if (sync != oldSync) {
                    if (decoderListener != null) decoderListener.rttSyncStateChanged(sync);
                }
                
                // Start/resume the decoder thread if we've got RTT elements to decode
                if (!rttElementQueue.isEmpty()) triggerDecodeThread();
            }
            return sync;
        }
        
        /** Decodes all RTT text immediately
         * 
         * @return Resulting decoded message
         */
        public String fullDecodeNow()
        {
            synchronized (rttElementQueue)
            {
                message.setKeyIntervalsEnabled(false);
                for (int i = 0; i < rttElementQueue.size(); i++) {
                    RealTimeText.DecodeRawRTT(rttElementQueue.get(i), message);
                }
                rttElementQueue.clear();
                message.setKeyIntervalsEnabled(true);
                return message.text;
            }
        }
        
        /** Gets the current state of the message string (Thread safe) */
        public String getText() {
            synchronized (rttElementQueue) { return message.text; }
        }

        /** Alias for getText() */
        public String toString()
        {
            synchronized (rttElementQueue) { return message.text; }
        }

        /** Gets the current character index position of the cursor (Thread safe) */
        public int getCursorPos()
        {
            synchronized (rttElementQueue) { return message.pos; }
        }
        
        /** Gets the activated flag. False means real time text is not activated. (Thread safe) */
        public boolean isActivated() {
            synchronized (rttElementQueue) { return activated; }
        }

        /** Gets the sync flag. False means real time text is out of sync. (Thread safe) */
        public boolean inSync() {
            synchronized (rttElementQueue) { return sync; }
        }
    }

    // ---------------------------------------------------------------------------------------------
    /** Class to encode outgoing real time text (action elements) upon sending.
      * This class is thread-safe for multithreading, to optionally support key press intervals.
      */
    static public class Encoder {
        RootElement rtt = new RootElement();
        private Message messagePrevious = new Message();
        private Message message = new Message();
        private boolean newMsg;
        private int seq = 0;
        private Random random = new Random();
        
        // Transmit interval variables
        private DelayCalculator delayCalculator = new DelayCalculator();
        private int transmitInterval = RealTimeText.DEFAULT_TRANSMIT_INTERVAL;
        
        // Fault tolerant redundancy variables
        private boolean fullMessageTransmit = false;
        private boolean redundancy = true;
        private long redundancyClockStart = System.currentTimeMillis();
        private int redundancyInterval = RealTimeText.DEFAULT_REDUNDANCY_INTERVAL;
        
        /** Constructor.
         *  @param enableIntervals Flag to enable encoding of intervals between key presses
         */
        public Encoder(boolean enableIntervals) {
            message.setKeyIntervalsEnabled(enableIntervals);
            clear();
        }

        /** Reinitialize the RTT encoder to a blank string state */
        public void clear() {
            fullMessageTransmit = false;
            message.reset();
            messagePrevious.reset();
            rtt = new RealTimeText.RootElement();
        }

        /** Resets the RTT encoder to starts a new message */
        public void nextMsg() {
            clear();
            newMsg = true;
        }

        /** Returns the current RTT encoding as an XML element, and then immediately starts a new RTT fragment */
        public RootElement getEncodedRTT() {
            RootElement rttEncoded = rtt;
            rttEncoded.setXmlns(RealTimeText.NAMESPACE);
            
            // The first RTT element of a new message always has event='new'
            // (i.e. This rtt element contains the first character of real time text)
            if (newMsg) {
                rttEncoded.setEvent("new");
                newMsg = false;
                seq = random.nextInt(1000000);   // Generates up to a 6-digit random number.  XEP-0301 recommends randomization, and keeping numbers compact for bandwidth savings
                if (redundancy) {
                    redundancyClockStart = System.currentTimeMillis();
                }
            }
            else
            {
                // Error recovery/fault tolerance: Automatic retransmission
                if (redundancy && 
                    ((System.currentTimeMillis() - redundancyClockStart) > redundancyInterval)) {
                    fullMessageTransmit = true;
                    redundancyClockStart = System.currentTimeMillis();
                }
                // Automatic and/or manually forced message retransmission
                if (fullMessageTransmit) {
                    rttEncoded.setEvent("reset");
                    if ((messagePrevious.pos != -1) &&
                        (messagePrevious.pos != messagePrevious.text.length())) {
                        // For clients supporting an optional remote cursor position, a blank INSERT TEXT <t/> elements sets the cursor position.
                        ActionElement resetPos = new ActionElement("t");
                        resetPos.p = messagePrevious.pos;
                        rtt.actions.add(0, resetPos); // Prepend
                    }
                    if (!messagePrevious.text.isEmpty())
                    {
                        // Retransmission of full message text in an INSERT TEXT <t/> element.
                        ActionElement resetText = new ActionElement("t");
                        resetText.text = messagePrevious.text;
                        rtt.actions.add(0, resetText); // Prepend
                    }
                }
            }
            rttEncoded.setSeq(this.seq);
            seq++;
            
            fullMessageTransmit = false;
            try {
                messagePrevious = message.copy();
            } catch (CloneNotSupportedException e) {
                e.printStackTrace();
            }

            // Create new RTT element for next message.
            rtt = new RealTimeText.RootElement();
            if (DEBUG_CONSOLE_XML) System.out.println("outgoing rtt: " + rttEncoded.toXML());
            return rttEncoded;
        }

        /** Encodes RTT fragment for the specified text. This encoder compares this text 
         *  to the previous state of text, and generates a compact RTT code for this text
         *  
         *  @param text Current state of message text
         */
        public void encode(String text)
        {
            encode(text, -1);
        }

        /** Encodes a cursor movement for the optional remote cursor 
         *  
         *  @param cursorPos Current position index within text, for optional remote cursor
         */
        public void encode(int cursorPos)
        {
            encode(null, cursorPos);
        }
        
        /** Encodes RTT fragment for the specified text. This encoder compares this text 
         *  to the previous state of text, and generates a compact RTT code for this text
         *  
         *  @param text Current state of message text
         *  @param cursorPos Current position index within text, for optional remote cursor
         */
        public void encode(String text, int cursorPos) {
            if (text == null) text = message.text;
            boolean textChanged = !message.text.equals(text);
            boolean posChanged = (cursorPos != -1) && (cursorPos != message.pos);
            if (textChanged || posChanged) {
                // If necessary, generate the 'w' interval action element
                if (message.getKeyIntervalsEnabled()) {
                    if (rtt.actions.isEmpty()) delayCalculator.start();  // Delay calculator starts on first rtt element.

                    int milliseconds = delayCalculator.getMillisecondsSinceLastCall();
                    if (milliseconds > 0) {
                        if (milliseconds > transmitInterval) milliseconds = transmitInterval;  // Limit delays to maximum interval.
                        RealTimeText.AppendElement.waitInterval(rtt.actions, milliseconds);
                    }
                }

                // Encode the text differences since last message, into action elements.
                RealTimeText.Message newMessage = new RealTimeText.Message(text, cursorPos);
                RealTimeText.EncodeRawRTT(rtt, message, newMessage);
                message.text = text;
                message.pos = cursorPos;
            }
        }

        /** Gets flag indicating that there is no real time text encoded in the queue. */
        public boolean isEmpty() {
            return rtt.actions.isEmpty();
        }
        
        /** Gets flag whether or not this is a new message */
        public boolean isNew() {
            return newMsg;
        }

        /** Gets the current state of the real time text */
        public String getText() {
            return message.getText();
        }

        /** Alias for Encoder.getText() */
        public String toString() {
            return message.getText();
        }

        /** Gets the current cursor position index into message text, used only if an optional remote cursor is used */
        public int getCursorPos() {
            return message.getCursorPos();
        }

        /** Gets transmission interval, in milliseconds, of regular real time text */
        public int getTransmitInterval() {
            return transmitInterval;
        }

        /** Sets transmission interval, in milliseconds, of regular real time text */
        public void setTransmitInterval(int value) {
            transmitInterval = value;
        }

        /** Gets flag to enable automatic redundancy for error recovery/fault tolerance */
        public boolean getRedundancyEnabled() {
            return redundancy;
        }

        /** Sets flag to enable automatic redundancy for error recovery/fault tolerance */
        public void setRedundancyEnabled(boolean value) {
            redundancy = value;
        }

        /** Gets redundancy interval, in milliseconds, for real time text */
        public int getRedundancyInterval() {
            return redundancyInterval;
        }

        /** Sets redundancy interval, in milliseconds, for real time text */
        public void setRedundancyInterval(int value) {
            redundancyInterval = value;
        }

        /** Gets flag to force retransmission of the whole message in next RTT element */
        public boolean getForceRetransmit() {
            return fullMessageTransmit;
        }

        /** Sets flag to force retransmission of the whole message in next RTT element */
        public void setForceRetransmit(boolean value) {
            fullMessageTransmit = value;
        }
        
        /** Gets flag that enable transmission of <w/> Wait Interval action elements between action elements, for encoding of pauses between key presses */
        public boolean getKeyIntervalsEnabled() {
            return message.getKeyIntervalsEnabled();
        }

        /** Sets flag that enable transmission of <w/> Wait Interval action elements between action elements, for encoding of pauses between key presses */
        public void setKeyIntervalsEnabled(boolean value) {
            message.setKeyIntervalsEnabled(value);
        }
    }
}
