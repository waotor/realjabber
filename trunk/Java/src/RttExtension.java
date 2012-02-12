import org.jivesoftware.smack.packet.PacketExtension;
import org.jivesoftware.smack.provider.PacketExtensionProvider;
import org.xmlpull.v1.XmlPullParser;

// XMPP In-Band Real Time Text Extension (XEPP-0xxx), Version 0.0.2 -- http://www.realjabber.org
// Written by Mark D. Rejhon - mailto:markybox@gmail.com - http://www.marky.com/resume
// 
// COPYRIGHT
// Copyright 2011 by Mark D. Rejhon - Rejhon Technologies Inc.
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


/** PacketExtension implementation of the <rtt> element for use by Smack XMPP class.
 */
public class RttExtension 
 	implements PacketExtension 
{
	protected RealTimeText.RootElement rtt;

	/** Default constructor */
	public RttExtension() { }
	
	/** Constructor that sets the &lt;rtt&gt; root element object */
	public RttExtension(RealTimeText.RootElement setRtt)
	{
		rtt = setRtt; 
	}
	
	/** Root element name */
    public String getElementName()
    {
    	return RealTimeText.ROOT;
    }

    /** XML namespace */
    public String getNamespace()
    {
    	return RealTimeText.NAMESPACE;
    }

    /** XML representation of <rtt> element. */
    public String toXML()
    {
    	return rtt.toXML();
    }

    /** Getter for the &lt;rtt&gt; root element object */
    public RealTimeText.RootElement getRTT()
    {
    	return rtt;
    }
    
    /** Packet extension provider class for the &lt;rtt&gt; root element. */
    public static class Provider implements PacketExtensionProvider
    {
	    /**
	     * Parses a packet containing an &lt;rtt&gt; root element.<br>
	     * (extension sub-packet compatible with the SMACK XMPP API client library).
	     *
	     * @param parser the XML parser, positioned at the starting element of the extension.
	     * @return a PacketExtension.
	     * @throws Exception if a parsing error occurs.
	     */
	    //
	    public PacketExtension parseExtension(XmlPullParser parser)
	        throws Exception
	    {
			RttExtension rttExtension = new RttExtension();
			rttExtension.rtt = new RealTimeText.RootElement();
			rttExtension.rtt.fromXml(parser);
			return rttExtension;
	    }
    }
}
