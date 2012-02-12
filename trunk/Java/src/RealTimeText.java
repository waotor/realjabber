import java.io.IOException;
import java.util.Vector;
import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserException;

// HELPER UTILITY CLASS FOR:
///   XMPP EXTENSION: http://www.xmpp.org/extensions/xep-0301.html
///      DESCRIPTION: XEP-0301 - In-Band Real Time Text - Version 0.0.3 - http://www.realjabber.org
// 
//          LANGUAGE: Java
//        XML PARSER: xmlpull
//      XMPP LIBRARY: No internal dependency (Recommended: smack for Java/JSP, asmack for Android)
//            AUTHOR: Mark D. Rejhon - mailto:markybox@gmail.com - http://www.marky.com/resume
//         COPYRIGHT: Copyright 2011 by Mark D. Rejhon - Rejhon Technologies Inc.
//           LICENSE: Apache 2.0 - Open Source - Commercial usage is permitted.
//
// LICENSE
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
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
// TABLE OF SUPPORTED ACTION ELEMENTS
//   For more information, please see http://www.realjabber.org
//   ----------------------------------------------------------
//   TIER ACTION           CODE               DESCRIPTION
//     1  Insert           <t p='#'>text</t>  (REQUIRED) Insert text at position p in message. Also good for block inserts (i.e. pastes)
//     1  Backspace        <e p='#' n='#'/>   (REQUIRED) Deletes n characters of text to the left of position p in message.
//     1  Forward Delete   <d p='#' n='#'/>   (REQUIRED) Deletes n characters of text to the right of position p in message. Also good for block deletes (i.e. cuts)
//     2  Delay            <w n='#'/>         (RECOMMENDED) Key press intervals. Execute a pause of n thousandths of a second. Allows multiple smooth keypresses in one packet.
//     2  Cursor Position  <c p='#'/>         (OPTIONAL) Move cursor to position p in message. (Remote Cursor)
//     2  Beep             <g/>               (OPTIONAL) Executes a flash/beep/buzz. Deaf-friendly feature.

/**
 * Central helper classes for real time text processing for XMPP instant
 * messaging.<br>
 * XMPP In-Band Real Time Text Extension - <a
 * href="http://www.realjabber.org">www.realjabber.org</a>
 * 
 * @author Mark Rejhon<br>
 *         <a href="mailto:markybox@gmail.com">markybox@gmail.com</a><br>
 *         <a href="http://www.marky.com/resume">www.marky.com/resume</a><br>
 */
public class RealTimeText {
	// Specification constants
	public static final String ROOT = "rtt";
	public static final String NAMESPACE = "urn:xmpp:rtt:0";
	public static final String VERSION = "0.0.3";

	/**
	 * @summary The default recommended transmission for transmission of real
	 *          time text
	 */
	public static final int INTERVAL_DEFAULT = 1000;

	/** @summary Class representing the state of a real time message */
	public static final boolean TEST_RTT_CODEC = false;

	// ---------------------------------------------------------------------------------------------
	/** Class representing the state of a real time message */
	static public class Message implements Cloneable {
		private String text = "";
		private int pos = 0;
		private int delay = 0;
		private boolean beeped = false;
		private boolean delaysEnabled = false;

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
			delay = 0;
			beeped = false;
		}

		/** Copy real time message */
		public Message copy() throws CloneNotSupportedException {
			return (Message) this.clone();
		}

		/** getter for message text */
		public String getText() {
			return text;
		}

		/** setter for message text */
		public void setText(String textValue) {
			text = textValue;
		}

		/** getter for cursor position index */
		public int getCursorPos() {
			return pos;
		}

		/** setter for cursor position index */
		public void setCursorPos(int posValue) {
			pos = posValue;
		}

		/** getter for flash/beep status flag */
		public boolean getBeeped() {
			return beeped;
		}

		/** setter for flash/beep status flag */
		public void setBeeped(boolean beepedValue) {
			beeped = beepedValue;
		}

		/** getter to enable/disable processing of delay action elements */
		public boolean getDelaysEnabled() {
			return delaysEnabled;
		}

		/** setter to enable/disable processing of delay action elements */
		public void setDelaysEnabled(boolean value) {
			delaysEnabled = value;
		}

		/** getter for Delay value */
		public int getDelay() {
			return delay;
		}

		/** setter for Delay value */
		public void setDelay(int value) {
			delay = value;
		}
	}

	// ---------------------------------------------------------------------------------------------
	/** Class representing one action element */
	static public class ActionElement {
		private String element = null; // Should be one of the supported action
										// elements
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
			if (element == null)
				throw new NullPointerException();

			StringBuilder rawXml = new StringBuilder("<" + element);
			if (p != null)
				rawXml.append(" p='" + p.toString() + "'");
			if (n != null)
				rawXml.append(" n='" + n.toString() + "'");
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

		/** Getter for xmlns attribute */
		public String getXmlns() {
			return xmlns;
		}

		/** Setter for xmlns attribute */
		public void setXmlns(String xmlnsValue) {
			xmlns = xmlnsValue;
		}

		/** Getter for event attribute */
		public String getEvent() {
			return event;
		}

		/** Setter for event attribute */
		public void setEvent(String eventValue) {
			event = eventValue;
		}

		/** Getter for seq value */
		public int getSeq() {
			return seq;
		}

		/** Setter for seq value */
		public void setSeq(int seqValue) {
			seq = seqValue;
		}

		/** Getter for action elements vector */
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
		 * T Element - Insert/Append Text Inserts text at specified position, or
		 * appends text to end of line
		 * 
		 * @param text Text to insert
		 * @param pos Position to insert text at
		 * @param len Length of original text to insert into (for automatic
		 *            omission of 'pos' if at end of string)
		 * @return Action element represented
		 */
		public static void Insert(Vector<ActionElement> actions, String text, int pos, int len) {
			if ((text == null) || text.isEmpty())
				return;

			ActionElement action = new ActionElement("t");
			if ((pos >= 0) && (pos < len)) {
				action.setPosition(pos);
			}
			action.setText(text);
			actions.add(action);
		}

		/**
		 * E Element - Backspace Erase # characters to left of specified position
		 * 
		 * @param pos Position to begin backspacing at
		 * @param count Count of characters to backspaces
		 * @param len Length of original string
		 * @return Action element represented
		 */
		public static void Backspace(Vector<ActionElement> actions, int pos,
				int count, int len) {
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
		 * D Element Forward Delete - Delete # of characters to right of
		 * specified position
		 * 
		 * @param pos Position to delete characters at
		 * @param count Count of characters to delete
		 * @return Action element represented
		 */
		public static void ForwardDelete(Vector<ActionElement> actions,
				int pos, int count) {
			if (count <= 0)
				return;

			ActionElement action = new ActionElement("d");
			if (count != 1) {
				action.setCount(count);
			}
			action.setPosition(pos);
			actions.add(action);
		}

		/**
		 * C Element - Cursor Position Set cursor position to specified index
		 * 
		 * @param pos
		 *            New cursor position index
		 * @return Action element represented
		 */
		public static void CursorPosition(Vector<ActionElement> actions, int pos) {
			ActionElement action = new ActionElement("c");
			action.setPosition(pos);
			actions.add(action);
		}

		/**
		 * W Element - Delay Pause specified amount for a delay between actions
		 * (including keypresses)
		 * 
		 * @param centiSecs
		 *            Number of hundredth of seconds to delay
		 * @return Action element represented
		 */
		public static void Delay(Vector<ActionElement> actions, int centiSecs) {
			ActionElement action = new ActionElement("w");
			action.setCount(centiSecs);
			actions.add(action);
		}

		/**
		 * G Element - Flash Brief visual flash/buzz/beep
		 * 
		 * @return Action element represented
		 */
		public static void Flash(Vector<ActionElement> actions) {
			actions.add(new ActionElement("g"));
		}
	}

	/**
	 * Decode Action Elements on a string of text. This follows the XEP-0301 Real Time Text spec.
	 * 
	 * @param rtt The root rtt element containing Action Elements
	 * @param message Real time message to update
	 * @return New text of message <br>
	 *         (message is updated, processed elements are removed from rtt)
	 */
	static public String DecodeRawRTT(RootElement rtt, Message message) {
		String text = message.text;
		int count;
		int pos;
		boolean quit = false;
		ActionElement action;

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
				case 't': // Insert action element: <t p="#">text</i>
					message.pos = pos;
					text = text.substring(0, message.pos) + action.text + text.substring(message.pos);
					message.pos = message.pos + action.text.length();
					break;
				// --------------------------------------------------------------------
				case 'e': // Backspace action element: <e p="#" n="#"/>
					message.pos = pos;
					if (count > message.pos) count = message.pos;
					text = text.substring(0, message.pos - count) + text.substring(message.pos);
					message.pos -= count;
					break;
				// --------------------------------------------------------------------
				case 'd': // Delete action element: <d p="#" n="#"/>
					message.pos = pos;
					if ((message.pos + count) > message.text.length()) count = message.text.length() - message.pos;
					text = text.substring(0, message.pos) + text.substring(message.pos + count);
					break;
				// --------------------------------------------------------------------
				case 'c': // Cursor Position action element: <c p="#"/>
					message.pos = pos;
					break;
				// --------------------------------------------------------------------
				case 'w': // Delay code <w n="#"/>
					if (message.delaysEnabled) {
						message.delay = count;
						quit = true;
						// We exit the loop so that the caller can decide to use
						// a timer for the delay code,
						// and come back to this loop later to finish remaining
						// nodes that have not finished processing.
					}
					break;
				// --------------------------------------------------------------------
				case 'g': // Flash action element: <g/>
					message.beeped = true;
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
	 * @returns Efficiently encoded RTT fragment to send in XML "rtt" element to
	 *          change "before" into "after"
	 */
	static public void EncodeRawRTT(RootElement rtt, Message before,
			Message after) {
		int curPos = before.getCursorPos();
		if (after.pos < 0) after.pos = 0;
		if (after.pos > after.text.length()) after.pos = after.text.length();

		if (before.getText().equals(after.getText())) {
			// Handle cursor position change only.
			if (before.pos != -1) {
				if (before.pos != after.pos) {
					AppendElement.CursorPosition(rtt.actions, after.pos);
				}
			}
		} else if (after.text.isEmpty()) {
			// The whole line got cleared.
			AppendElement.Backspace(rtt.actions, before.text.length(), before.text.length(), before.text.length());
			curPos = 0;
		} else {
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

			// Delete text if a deletion is detected anywhere in the string.
			int charsRemoved = before.text.length() - trailingSame - leadingSame;
			if (charsRemoved > 0) {
				int posEndOfRemovedChars = before.text.length() - trailingSame;
				int posStartOfRemovedChars = posEndOfRemovedChars - charsRemoved;
				if ((before.pos == posEndOfRemovedChars)
						|| (posEndOfRemovedChars == before.text.length())) {
					// Cursor ideally positioned for <e> BACKSPACE operations
					// rather than delete
					AppendElement.Backspace(rtt.actions, posEndOfRemovedChars, charsRemoved, before.text.length());
					curPos = posStartOfRemovedChars;
				} else {
					// Cursor ideally positioned for <d> FORWARD DELETE
					// operations rather than backspace
					AppendElement.ForwardDelete(rtt.actions, posStartOfRemovedChars, charsRemoved);
					curPos = posStartOfRemovedChars;
				}
			}

			// Do an <t> INSERT operation if any text insertion is detected
			// anywhere in the string
			int charsInserted = after.text.length() - trailingSame - leadingSame;
			try {
				String insertedText = after.text.substring(leadingSame, leadingSame + charsInserted);
				AppendElement.Insert(rtt.actions, insertedText, curPos, before.text.length());
			} catch (Exception ex) {
				System.out.println("Oh no!! ... " + ex.toString());
			}
			curPos += charsInserted;

			// Execute a <c> CURSOR POSITION operation to move cursor to final
			// location, if last edit action element didn't put cursor there.
			if (curPos != after.pos) {
				AppendElement.CursorPosition(rtt.actions, after.pos);
				curPos = after.pos;
			}
		}
	}

	// ---------------------------------------------------------------------------------------------
	/**
	 * Class to calculate very accurate delay intervals, used for recording
	 * delays between actions (such as the pauses between key presses) for
	 * encoding & decoding of the 'w' action element during real time text
	 * communications. Delay interval values are calculated using an accumulated
	 * delay total since the beginning of an &lt;rtt&rt; element. This
	 * calculation is much more accurate than simply trying to directly measure
	 * the time between actions, and more immune to system/performance
	 * variances. Timers and other methods of delay calculations, are more prone
	 * to software/CPU performance variations. The calculations in this class is
	 * immune to system & software performance variations AND is also immune to
	 * accumulated rounding errors.
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
		 * @param delay
		 *            Recommended delay interval. (typically, from a delay
		 *            action element)
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
	/** Class to decode incoming real time text (action elements) */
	static public class Decoder implements Runnable {

		private Message message = new Message();
		private Vector<RootElement> rttElementQueue = new Vector<RootElement>();
		private boolean sync = false;
		private boolean activated = false;
		private int seq = 0;

        /** Listener for decoder */
        public EventListener decoderListener = null;
		public interface EventListener {
			/** Callback event for asynchronous RTT decoding (This mainly used for Natural Typing mode) */
			public void rttTextUpdated();
			/** Callback event that is called everytime sync state changes (loss of sync, caused by missing or out-of-order rtt packets) */
			public void rttSyncStateChanged(boolean syncState);
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
                message.setDelaysEnabled(true);
                sync = false;
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

                            if (message.delay != 0) {
                                interval = message.delay;
                                break;
                            }
                        }
                    }
                    if (!enableThread) break;
                }

                // (MUST be lock-free) Calls the user-defined event for one step of text decode 
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
			
			synchronized (rttElementQueue) {
				try {
					if (rttNew.seq < 0) {
						this.sync = false;
					} else if (rttNew.event != null) {
						if ("new".equals(rttNew.event)) {
							// New RTT message. This brings us into SYNC.
							reset();
							this.activated = true;
							this.sync = true;
							this.seq = rttNew.seq;
							rttElementQueue.add(rttNew);
							if (decoderListener != null) decoderListener.rttSyncStateChanged(true);
						} else if ("reset".equals(rttNew.event)) {
							// Reset RTT message. This brings us into SYNC.
							message.reset();
							this.activated = true;
							this.sync = true;
							this.seq = rttNew.seq;
							rttElementQueue.add(rttNew);
							if (decoderListener != null) decoderListener.rttSyncStateChanged(true);
						} else if ("start".equals(rttNew.event)) {
							this.activated = true;
						} else if ("stop".equals(rttNew.event)) {
							this.activated = false;
						} else {
							// Undocumented 'event' attribute value is specified as an OUT-OF-SYNC condition.
							this.sync = false; 
						}
					} else if (rttNew.seq != (this.seq + 1)) {
						// Non-consecutive increment in 'seq' is specified as an OUT-OF-SYNC condition.
						this.sync = false;
					} else {
						this.seq = rttNew.seq;
						if (this.sync) rttElementQueue.add(rttNew);
					}
				} catch (Exception ex) {
                    // Any exception occurring at this point, should be assumed as an OUT-OF-SYNC condition. (i.e. NumberFormatException, etc)
					System.out.println("EXCEPTION: thrown in RTTdecode.Queue: " + ex.toString());
                    this.sync = false;
				}
				
	            if (!sync) {
	                // If we're no longer in sync, then inform this object's owner
	                if (decoderListener != null) decoderListener.rttSyncStateChanged(false);
	            }
	            triggerDecodeThread();
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
                message.setDelaysEnabled(false);
				for (int i = 0; i < rttElementQueue.size(); i++) {
                    RealTimeText.DecodeRawRTT(rttElementQueue.get(i), message);
                }
                rttElementQueue.clear();
                message.setDelaysEnabled(true);
                return message.text;
            }
        }
		
        /** Getter for the current state of the message string (Thread safe) */
		public String getText() {
        	synchronized (rttElementQueue) { return message.text; }
        }

        /** Alias for decoder.text */
        public String toString()
        {
        	synchronized (rttElementQueue) { return message.text; }
        }

        /** Getter for the current character index position of the cursor (Thread safe) */
        public int getCursorPos()
        {
        	synchronized (rttElementQueue) { return message.pos; }
        }
		
		/** getter for activated flag. False means real time text is not activated. (Thread safe) */
		public boolean isActivated() {
			synchronized (rttElementQueue) { return activated; }
		}

		/** getter for sync flag. False means real time text is out of sync. (Thread safe) */
		public boolean inSync() {
			synchronized (rttElementQueue) { return sync; }
		}
	}

	// ---------------------------------------------------------------------------------------------
	/** Class to encode outgoing real time text (action elements) */
	static public class Encoder {
		RootElement rtt = new RootElement();
		private Message prevMessage = new Message();
		private int transmitInterval = RealTimeText.INTERVAL_DEFAULT;
		private boolean newMsg;
		private int seq = 0;
		private DelayCalculator delayCalculator = new DelayCalculator();
		
		/** Constructor.
		 *  @param enableDelays Flag to enable encoding of delays between key presses
		 */
		public Encoder(boolean enableDelays) {
			newMsg = true;
			seq = 0;
			prevMessage.setDelaysEnabled(enableDelays);
			reset();
		}

		/** Reinitialize the RTT encoder to a blank string state */
		public void reset() {
			prevMessage.reset();
			rtt = new RealTimeText.RootElement();
		}

		/** Resets the RTT encoder to starts a new message */
		public void nextMsg() {
			reset();
			newMsg = true;
		}

		/**
		 * Returns the current RTT encoding as an XML element, and then
		 * immediately starts a new RTT fragment
		 */
		public RootElement getEncodedRTT() {
			RootElement rttEncoded = rtt;
			rttEncoded.setXmlns(RealTimeText.NAMESPACE);
			rttEncoded.setSeq(this.seq);
			seq++;

			// 'event' attribute. The first RTT element of a real time message
			// always has event='new'
			if (newMsg) {
				rttEncoded.setEvent("new");
				newMsg = false;
			}

			// Create new RTT element for next message.
			rtt = new RealTimeText.RootElement();
			return rttEncoded;
		}

		/** Encodes message changes into a real time action elements in an &lt;rtt&rt; element */
		public void encode(String text, int cursorPos) {
			if ((text != prevMessage.getText()) || (cursorPos != prevMessage.getCursorPos())) {
				
				// If necessary, generate the 'g' beep action element
				if (prevMessage.beeped) {
					RealTimeText.AppendElement.Flash(rtt.actions);
					prevMessage.beeped = false;
				}

				// If necessary, generate the 'w' delay action element
				if (prevMessage.getDelaysEnabled()) {
					// Delay calculator starts on first rtt element.
					if (rtt.actions.isEmpty()) delayCalculator.start();

					int milliseconds = delayCalculator.getMillisecondsSinceLastCall();
					if (milliseconds > 0) {
						// Limit delays to maximum interval.
						if (milliseconds > transmitInterval) milliseconds = transmitInterval;
						RealTimeText.AppendElement.Delay(rtt.actions, milliseconds);
					}
				}

				// Encode the text differences since last message, into action elements.
				RealTimeText.Message newMessage = new RealTimeText.Message(text, cursorPos);
				RealTimeText.EncodeRawRTT(rtt, prevMessage, newMessage);
				prevMessage.setText(text);
				prevMessage.setCursorPos(cursorPos);
			}
		}

		/** Gets the last state of the text */
		public String getText() {
			return prevMessage.getText();
		}

		/** Alias for Encoder.Text */
		public String toString() {
			return prevMessage.getText();
		}

		/** Get the last state of the cursor position index */
		public int getCursorPos() {
			return prevMessage.getCursorPos();
		}

		/** Gets the status of the RTT encoder, if there is any rtt action elements to transmit */
		public boolean isEmpty() {
			return rtt.actions.isEmpty();
		}

		/** Gets transmission interval of real time text */
		public int getTransmitInterval() {
			return transmitInterval;
		}

		/** Sets transmission interval of real time text */
		public void setTransmitInterval(int value) {
			transmitInterval = value;
		}

		/** Gets enabled state of transmission of 'W' interval elements, for encoding of pauses between keypresses */
		public boolean getDelaysEnabled() {
			return prevMessage.getDelaysEnabled();
		}

		/** Sets enabled state of transmission of 'W' interval elements, for encoding of pauses between keypresses */
		public void setDelaysEnabled(boolean value) {
			prevMessage.setDelaysEnabled(value);
		}
	}
}
