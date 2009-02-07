// 
// HMACSHA1Signature.cs
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
using System.Text;
using System.Security.Cryptography;

using OAuth;
using OAuth.RequestProxies;

namespace OAuth.Signatures
{	
	public class HMACSHA1Signature : AbstractSignature
	{
		string m_Signature;
			
		public HMACSHA1Signature(IRequestProxy request, string consumerSecret, string tokenSecret) 
			: base (request, consumerSecret, tokenSecret)
		{
			Sign();
		}
		
		public override string Base64Signature {
			get {
				return m_Signature;
			}
		}
		
		void Sign ()
		{
			var hmac = new HMACSHA1();
			hmac.Key = Encoding.ASCII.GetBytes(base.Secret);
			
			byte[] dataBuffer = Encoding.ASCII.GetBytes(base.Request.SignatureBaseString);
			byte[] hashBytes = hmac.ComputeHash(dataBuffer);
			
			m_Signature = Convert.ToBase64String(hashBytes);
		}
	}
}
