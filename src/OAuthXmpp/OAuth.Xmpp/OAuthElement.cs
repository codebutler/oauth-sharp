// 
// OAuthElement.cs
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
using System.Xml;

using jabber.protocol;

using OAuth;

namespace OAuth.Xmpp
{	
	public class OAuthElement : Element
	{
		public OAuthElement (Consumer consumer, AccessToken token, XmlDocument doc) 
			: base ("oauth", "urn:xmpp:tmp:oauth", doc)
		{
			this.ConsumerKey     = consumer.Key;
			this.Token           = token.Token;
			this.Version         = consumer.OAuthVersion;
			this.Nonce           = OAuth.Helper.GenerateNonce();
			this.Timestamp       = OAuth.Helper.GenerateTimestamp();
			this.SignatureMethod = consumer.SignatureMethod;
		}
		
		public string ConsumerKey {
			get {
				return GetElem("oauth_consumer_key");
			}
			set {
				SetElem("oauth_consumer_key", value);
			}
		}
		
		public string Nonce {
			get {
				return GetElem("oauth_nonce");
			}
			set {
				SetElem("oauth_nonce", value);
			}
		}
		
		public string Signature {
			get {
				return GetElem("oauth_signature");
			}
			set {
				SetElem("oauth_signature", value);
			}
		}
		
		public string SignatureMethod {
			get {
				return GetElem("oauth_signature_method");
			}
			set {
				SetElem("oauth_signature_method", value);
			}
		}
		
		public string Timestamp {
			get {
				return GetElem("oauth_timestamp");
			}
			set {
				SetElem("oauth_timestamp", value);
			}
		}
		
		public string Token {
			get {
				return GetElem("oauth_token");
			}
			set {
				SetElem("oauth_token", value);
			}
		}
		
		public string Version {
			get {
				return GetElem("oauth_version");
			}
			set {
				SetElem("oauth_version", value);
			}
		}
	}
}