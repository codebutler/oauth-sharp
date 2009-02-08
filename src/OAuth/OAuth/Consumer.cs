// 
// Consumer.cs
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
using System.Web;
using System.Net;
using System.IO;
using System.Collections.Generic;

using OAuth;
using OAuth.RequestProxies;
using OAuth.Signatures;

namespace OAuth
{
	public class Consumer
	{
		string m_Key;
		string m_Secret;
		
		string m_AuthorizeUrl;
		
		public Consumer(string consumerKey, string consumerSecret)
		{
			m_Key = consumerKey;
			m_Secret = consumerSecret;
	
			// Defaults
			this.SignatureMethod  = "HMAC-SHA1";
			this.RequestTokenPath = "/oauth/request_token";
			this.AuthorizePath    = "/oauth/authorize";
			this.AccessTokenPath  = "/oauth/access_token";
			this.OAuthVersion     = "1.0";
		}
		
		public RequestToken GetRequestToken ()
		{
			var response = Request("GET", new Uri(this.RequestTokenUrl), null, null);
			var query = HttpUtility.ParseQueryString(response);
			
			return new RequestToken(this, query["oauth_token"], query["oauth_token_secret"]);
		}
		
		public string Key {
			get {
				return m_Key;
			}
		}
		
		public string Secret {
			get {
				return m_Secret;
			}
		}		
		
		public string Site {
			get;
			set;
		}
		
		public string SignatureMethod {
			get;
			set;
		}
		
		public string RequestTokenPath {
			get;
			set;
		}
		
		public string AccessTokenPath {
			get;
			set;
		}
		
		public string AuthorizePath {
			get;
			set;
		}
		
		public string OAuthVersion {
			get;
			set;
		}
		
		public string RequestTokenUrl {
			get {
				return this.Site + this.RequestTokenPath;
			}
		}
		
		public string AccessTokenUrl {
			get {
				return this.Site + this.AccessTokenPath;
			}
		}
		
		public string AuthorizeUrl {
			get {
				return String.IsNullOrEmpty(m_AuthorizeUrl) ? this.Site + this.AuthorizePath : m_AuthorizeUrl;
			}
			set {
				m_AuthorizeUrl = value;
			}
		}
		
		public string Request (string httpMethod, Uri uri, ConsumerToken token)
		{
			return Request(httpMethod, uri, token.Token, token.Secret);
		}
		
		string Request (string httpMethod, Uri uri, string token, string tokenSecret)
		{
			var request = (HttpWebRequest)HttpWebRequest.Create(uri);
			request.Method = httpMethod;
			
			// FIXME: It would be much cleaner if these parameters could be set on the request object.
			var oauthParams = new Dictionary<string, string>() {
				{ "oauth_consumer_key",     m_Key                      },
				{ "oauth_token",            token                      },
				{ "oauth_version",          this.OAuthVersion          },
				{ "oauth_signature_method", this.SignatureMethod       },
				{ "oauth_nonce",            Helper.GenerateNonce()     },
				{ "oauth_timestamp",        Helper.GenerateTimestamp() }
			};
			
			HttpWebRequestProxy proxy = (HttpWebRequestProxy)RequestProxyFactory.CreateProxy(request, oauthParams);
			proxy.Sign(m_Secret, tokenSecret);
			
			if (httpMethod.ToUpper() == "POST") {
				request.ContentType = "application/x-www-form-urlencoded";
				
				string postData = Helper.Normalize(proxy.Parameters);
				using (var writer = new StreamWriter(request.GetRequestStream())) {
					writer.Write(postData);
				}
			} else {
				request.Headers.Add("Authorization", proxy.GetOAuthHeader(null));
			}
			
			try {
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				
				using (StreamReader reader = new StreamReader(response.GetResponseStream())) {
					return reader.ReadToEnd();
				}
				
			} catch (WebException ex) {
				if (ex.Response != null) {					
					using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream())) {
						Console.Error.WriteLine();
						Console.Error.WriteLine("ERROR BODY: " + reader.ReadToEnd());
						Console.Error.WriteLine();
					}					
				}
				throw ex;
			}
		}
	}
}
