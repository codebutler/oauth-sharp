// 
// SignatureFactory.cs
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

using OAuth;
using OAuth.RequestProxies;

namespace OAuth.Signatures
{	
	public static class SignatureFactory
	{
		static Dictionary<string, Type> s_AvailableMethods;
		
		static SignatureFactory()
		{
			s_AvailableMethods = new Dictionary<string, Type>();
			s_AvailableMethods.Add("HMAC-SHA1", typeof(HMACSHA1Signature));
		}
		
		public static void RegisterSignatureType (string name, Type signatureType)
		{
			s_AvailableMethods.Add(name, signatureType);
		}
		
		public static ISignature CreateSignature (IRequestProxy request, string consumerSecret, string tokenSecret)
		{
			if (s_AvailableMethods.ContainsKey(request.OAuthSignatureMethod)) {
				Type signatureType = s_AvailableMethods[request.OAuthSignatureMethod];
				return (ISignature) Activator.CreateInstance(signatureType, request, consumerSecret, tokenSecret);
			} else {
				throw new Exception("Unsupported signature type: " + request.OAuthSignatureMethod);
			}
		}
	}
}
