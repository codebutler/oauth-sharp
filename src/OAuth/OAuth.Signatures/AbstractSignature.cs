// 
// AbstractSignature.cs
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

using OAuth;
using OAuth.RequestProxies;

namespace OAuth.Signatures
{
	public abstract class AbstractSignature : ISignature
	{
		IRequestProxy m_Request;
		string m_ConsumerSecret;
		string m_TokenSecret;
		
		public AbstractSignature(IRequestProxy request, string consumerSecret, string tokenSecret)
		{
			m_Request = request;
			m_ConsumerSecret = consumerSecret;
			m_TokenSecret = tokenSecret;
		}
		
		public string Secret {
			get {
				return String.Format("{0}&{1}", m_ConsumerSecret, m_TokenSecret);
			}
		}
		
		public abstract string Base64Signature {
			get;
		}		
		
		protected IRequestProxy Request {
			get {
				return m_Request;
			}
		}
	}
}
