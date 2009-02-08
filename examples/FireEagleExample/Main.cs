// 
// Main.cs
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

using OAuth;

namespace FireEagleExample
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			if (args.Length != 2) {
				Console.WriteLine("Usage: FireEagleExample.exe consumer_key consumer_secret");
				return;
			}
			
			var consumer = new Consumer(args[0], args[1]) {
				Site         = "https://fireeagle.yahooapis.com",
				AuthorizeUrl = "https://fireeagle.yahoo.net/oauth/authorize"
			};
			
			var requestToken = consumer.GetRequestToken();
			
			Console.WriteLine("Visit this URL in your web browser, then press Enter:");
			Console.WriteLine(requestToken.AuthorizeUrl);
			Console.ReadLine();
			
			var accessToken = requestToken.ConvertToAccessToken();
			
			var response = consumer.Request("GET", new Uri("https://fireeagle.yahooapis.com/api/0.1/user"), accessToken);
			Console.WriteLine(response);
		}
	}
}