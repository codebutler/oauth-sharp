// 
// HttpRequestProxy.cs
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
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Net;
using System.IO;

namespace OAuth.RequestProxies
{
	public class HttpWebRequestProxy : AbstractRequestProxy
	{
		HttpWebRequest m_Request;
		Dictionary<string, string> m_OAuthParams;
		
		public HttpWebRequestProxy (HttpWebRequest request, Dictionary<string, string> oauthParams)
		{
			m_Request = request;
			
			m_OAuthParams = oauthParams;
		}	
		
		public override string Uri {
			get {
				return m_Request.RequestUri.ToString();
			}
		}
		
		public override IDictionary<string, string> Parameters {
			get {
				var allParameters = new Dictionary<string, string>();
				
				var requestParameters = HttpUtility.ParseQueryString(m_Request.RequestUri.Query);
				foreach (string key in requestParameters) {
					allParameters.Add(key, requestParameters[key]);
				}
				
				/* FIXME: This doesn't work...
				if (m_Request.Method == "POST" && m_Request.ContentType == "application/x-www-form-urlencoded") {
					using (var reader = new StreamReader(m_Request.GetRequestStream())) {
						string postBody = reader.ReadToEnd();
						var postParameters = HttpUtility.ParseQueryString(postBody);
						foreach (string key in postParameters.AllKeys) {
							allParameters.Add(key, postParameters[key]);
						}
					}
				}
				*/
				
				foreach (var pair in m_OAuthParams) {
					allParameters.Add(pair.Key, pair.Value);
				}
				
				return allParameters;
			}
		}
		
		public override string Method {
			get {
				return m_Request.Method;
			}
		}
		
		public override void Sign (string consumerSecret, string tokenSecret)
		{
			m_OAuthParams["oauth_signature"] = base.CreateSignature(consumerSecret, tokenSecret);			
		}
	}
}
