// 
// Token.cs
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

namespace OAuth
{
	public abstract class AbstractToken
	{
		string m_Token;
		string m_Secret;
		
		public AbstractToken(string token, string secret)
		{
			m_Token = token;
			m_Secret = secret;
		}
		
		public string Token {
			get {
				return m_Token;
			}
		}
		
		public string Secret {
			get {
				return m_Secret;
			}
		}
		
		public string ToQuery ()
		{
			return String.Format("oauth_token={0}&oauth_secret={1}", Helper.UrlEncode(m_Token), Helper.UrlEncode(m_Secret));
		}
	}
	
	public abstract class ConsumerToken : AbstractToken
	{
		Consumer m_Consumer;
		
		public ConsumerToken (Consumer consumer, string token, string secret)
			: base (token, secret)
		{
			m_Consumer = consumer;
		}
		
		public string Request (string httpMethod, Uri uri)
		{
			return m_Consumer.Request(httpMethod, uri, this);
		}
		
		protected Consumer Consumer {
			get {
				return m_Consumer;
			}
		}
	}
	
	public class RequestToken : ConsumerToken
	{
		public RequestToken (Consumer consumer, string token, string secret)
			: base (consumer, token, secret)
		{
		}
		
		public string AuthorizeUrl {
			get {
				return String.Format("{0}?oauth_token={1}", base.Consumer.AuthorizeUrl, Helper.UrlEncode(base.Token));
			}
		}
		
		public AccessToken ConvertToAccessToken ()
		{
			var response = base.Consumer.Request("POST", new Uri(Consumer.AccessTokenUrl), this);
			var query = HttpUtility.ParseQueryString(response);
			return new AccessToken(base.Consumer, query["oauth_token"], query["oauth_token_secret"]);
		}
	}
	
	public class AccessToken : ConsumerToken
	{
		public AccessToken (Consumer consumer, string token, string secret)
			: base (consumer, token, secret)
		{
		}
	}
}
