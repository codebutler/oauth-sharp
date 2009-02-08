// 
// AbstractRequestProxy.cs
//  
// Copyright (C) 2009 Eric Butler
// 
// Authors:
//   Eric Butler <eric@extremeboredom.net>
// 
// Based on the Ruby OAuth GEM library - http://oauth.rubyforge.org/
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
using System.Linq;

using OAuth;
using OAuth.Signatures;

namespace OAuth.RequestProxies
{
	public abstract class AbstractRequestProxy : IRequestProxy
	{
		public AbstractRequestProxy()
		{
			
		}
		
		public abstract void Sign (string consumerSecret, string tokenSecret);
		
		public bool IsSigned {
			get {
				return !String.IsNullOrEmpty(Parameters["oauth_signature"]);
			}
		}
		
		public string GetOAuthHeader (string realm)
		{
			var headerParamsStr = String.Join(", ", OAuthParameters.Select(entry => String.Format("{0}=\"{1}\"", entry.Key, Helper.UrlEncode(entry.Value))).ToArray());
			if (!String.IsNullOrEmpty(realm)) {
				return String.Format("OAuth realm=\"{0}\", {1}", realm, headerParamsStr);
			} else {
				return String.Format("OAuth {0}", headerParamsStr);
			}
		}
		
		public string GetSignedUri (bool withOAuth)
		{
			if (IsSigned) {
				var parameters = withOAuth ? Parameters : NonOAuthParameters;
				return String.Format("{0}?{1}", Uri, Helper.Normalize(parameters));
			} else {
				throw new Exception("This request has not yet been signed!");
			}
		}
		
		#region OAuth Parameters
		public string OAuthConsumerKey {
			get {
				return Parameters["oauth_consumer_key"];
			}
		}
		
		public string OAuthNonce {
			get {
				return Parameters["oauth_nonce"];
			}
		}
		
		public string OAuthSignature {
			get {
				return Parameters["oauth_signature"];
			}
		}
		
		public string OAuthSignatureMethod {
			get {
				return Parameters["oauth_signature_method"];
			}
		}
		
		public string OAuthTimestamp {
			get {
				return Parameters["oauth_timestamp"];
			}
		}
		
		public string OAuthToken {
			get {
				return Parameters["oauth_token"];
			}
		}
		
		public string OAuthVersion {
			get {
				return Parameters["oauth_version"];
			}
		}
		#endregion
		
		public abstract string Method {
			get;
		}
		
		public abstract string Uri {
			get;
		}
		
		public string SignatureBaseString {
			get {
				var baseArgs = new [] { Method, NormalizedUri, NormalizedParameters };
				return String.Join("&", baseArgs.Select(v => Helper.UrlEncode(v)).ToArray());
			}
		}		
		
		public virtual string NormalizedUri {
			get {
				Uri u = new Uri(Uri);
				var port = ((u.Scheme.ToLower() == "http" && u.Port != 80) || (u.Scheme.ToLower() == "https" && u.Port != 443)) ? String.Format(":{0}") : String.Empty;
				return String.Format("{0}://{1}{2}{3}", u.Scheme.ToLower(), u.Host.ToLower(), u.AbsolutePath, port);
			}
		}
		
		public abstract IDictionary<string, string> Parameters {
			get;
		}
		
		public IDictionary<string, string> OAuthParameters {
			get {
				return Parameters
					.Where(entry => Constants.OAUTH_PARAMETERS.Contains(entry.Key))
					.Where(entry => !String.IsNullOrEmpty(entry.Value))
					.ToDictionary(entry => entry.Key, entry => entry.Value);
			}
		}
		
		public IDictionary<string, string> NonOAuthParameters {
			get {
				return Parameters
					.Where(entry => !Constants.OAUTH_PARAMETERS.Contains(entry.Key))
					.Where(entry => !String.IsNullOrEmpty(entry.Value))
					.ToDictionary(entry => entry.Key, entry => entry.Value);
			}
		}
		
		public IDictionary<string, string> ParametersForSignature {
			get {
				return Parameters
					.Where(entry => entry.Key != "oauth_signature")
					.ToDictionary(entry => entry.Key, entry => entry.Value);
			}
		}
		
		public string NormalizedParameters {
			get {
				var parameters = ParametersForSignature
					.Where(entry => !String.IsNullOrEmpty(entry.Value))
					.ToDictionary(entry => entry.Key, entry => entry.Value);
				
				return Helper.Normalize(parameters);
			}
		}
		
		protected string CreateSignature (string consumerSecret, string tokenSecret)	
		{
			return SignatureFactory.CreateSignature(this, consumerSecret, tokenSecret).Base64Signature;
		}
	}
}
