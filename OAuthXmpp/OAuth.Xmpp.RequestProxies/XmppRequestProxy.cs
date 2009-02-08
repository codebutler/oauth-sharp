// 
// XmppRequestProxy.cs
//  
// Copyright (C) 2009 Eric Butler
// 
// Authors:
//   Eric Butler <eric@extremeboredom.net>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Xml;

using OAuth;
using OAuth.RequestProxies;

using jabber.protocol;

namespace OAuth.Xmpp.RequestProxies
{	
	public class XmppRequestProxy : AbstractRequestProxy
	{
		public static void Register ()
		{
			RequestProxyFactory.RegisterProxyType(typeof(Packet), typeof(XmppRequestProxy));
		}
		
		Packet m_Packet;
		
		public XmppRequestProxy (Packet packet)
		{
			m_Packet = packet;
		}
		
		public override string Uri {
			get {
				return String.Format("{0}&{1}", m_Packet.From.Bare, m_Packet.To.Bare);
			}
		}

		public override IDictionary<string, string> Parameters {
			get {
				var oauthElementNames = new [] { 
					"oauth_token", "oauth_consumer_key", "oauth_signature_method",
					"oauth_signature", "oauth_timestamp", "oauth_nonce", "oauth_version" 
				};
				
				var parameters = new Dictionary<string, string>();
				
			
				var nsmgr = new XmlNamespaceManager(m_Packet.OwnerDocument.NameTable);
				nsmgr.AddNamespace("oauth", "urn:xmpp:tmp:oauth");
				var oauthElement = m_Packet.SelectSingleNode("//oauth:oauth", nsmgr);
				if (oauthElement == null) {
					throw new Exception("oauth element not found");
				}
				
				foreach (var elementName in oauthElementNames) {
					if (oauthElement[elementName] == null)
						oauthElement.AppendChild(m_Packet.OwnerDocument.CreateElement(elementName));
					parameters[elementName] = oauthElement[elementName].InnerText;
				}
				
				return parameters;
			}
		}

		public override string NormalizedUri {
			get {
				return Uri;
			}
		}

		public override string Method {
			get {
				return m_Packet.Name;
			}
		}	
		
		public override void Sign (string consumerSecret, string tokenSecret)
		{
			var signature = base.CreateSignature(consumerSecret, tokenSecret);
			
			var nsmgr = new XmlNamespaceManager(m_Packet.OwnerDocument.NameTable);
			nsmgr.AddNamespace("oauth", "urn:xmpp:tmp:oauth");
			var oauthElement = m_Packet.SelectSingleNode("//oauth:oauth", nsmgr);
			if (oauthElement == null) {
				throw new Exception("oauth element not found");
			}
			
			if (oauthElement["oauth_signature"] == null) {
				oauthElement.AppendChild(new Element("oauth_signature", m_Packet.OwnerDocument));
			}
			
			oauthElement["oauth_signature"].InnerText = signature;
		}
	}
}
