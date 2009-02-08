// 
// RequestProxyFactory.cs
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
	public static class RequestProxyFactory
	{			
		static Dictionary<Type, Type> s_AvailableProxies;
		
		static RequestProxyFactory ()
		{
			s_AvailableProxies = new Dictionary<Type, Type>();
			s_AvailableProxies.Add(typeof(Dictionary<string, object>), typeof(MockRequestProxy));
			s_AvailableProxies.Add(typeof(System.Net.HttpWebRequest), typeof(HttpWebRequestProxy));
		}
		
		public static void RegisterProxyType (Type objectType, Type proxyType)
		{
			s_AvailableProxies.Add(objectType, proxyType);
		}		
	
		public static IRequestProxy CreateProxy (object obj)
		{
			return CreateProxy(obj, null);
		}
		
		public static IRequestProxy CreateProxy (object obj, Dictionary<string, string> parameters)
		{		
			Type objectType = obj.GetType();
			Type proxyType = FindProxyType(objectType);
			if (proxyType != null) {
				if (parameters == null)
					return (IRequestProxy)Activator.CreateInstance(proxyType, obj);
				else
					return (IRequestProxy)Activator.CreateInstance(proxyType, obj, parameters);
			} else {
				throw new Exception("No proxy available for type: " + objectType);
			}
		}
		
		static Type FindProxyType (Type objectType)
		{
			foreach (Type type in s_AvailableProxies.Keys) {
				if (type.IsAssignableFrom(objectType)) {
					return s_AvailableProxies[type];
				}
			}
			return null;
		}
	}
}