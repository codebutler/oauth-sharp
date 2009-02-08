// 
// IRequestProxy.cs
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

namespace OAuth.RequestProxies
{
	public interface IRequestProxy
	{
		string OAuthConsumerKey {
			get;
		}
		
		string OAuthNonce {
			get;
		}
		
		string OAuthSignature {
			get;
		}
		
		string OAuthSignatureMethod {
			get;
		}
		
		string OAuthTimestamp {
			get;
		}
		
		string OAuthToken {
			get;
		}
		
		string OAuthVersion {
			get;
		}
		
		string Method {
			get;
		}
		
		string Uri {
			get;
		}
		
		string SignatureBaseString {
			get;
		}
		
		string NormalizedUri {
			get;
		}
		
		string NormalizedParameters {
			get;
		}
		
		IDictionary<string, string> Parameters {
			get;
		}
		
		IDictionary<string, string> OAuthParameters {
			get;
		}
		
		IDictionary<string, string> NonOAuthParameters {
			get;
		}
		
		void Sign (string consumerSecret, string tokenSecret);
		
		string GetOAuthHeader (string realm);
		
		string GetSignedUri (bool withOAuth);
	}
}