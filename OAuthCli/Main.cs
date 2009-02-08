// 
// Main.cs
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
using System.Web;
using System.Net;

using OAuth;
using OAuth.RequestProxies;

using Mono.Options;

namespace OAuthCli
{
	class MainClass
	{
		readonly string[] SUPPORTED_COMMANDS = new [] { "sign" };
		
		Dictionary<string, string> m_Options;
		string m_Command;
		
		public static void Main(string[] args)
		{
			new MainClass().Execute(args);
		}
		
		public MainClass ()
		{
			m_Options = new Dictionary<string, string>();
			m_Options["oauth_signature_method"] = "HMAC-SHA1";
			m_Options["oauth_version"] = "1.0";
			m_Options["parameters"] = null;
		}
		
		public void Execute (string[] args)
		{
			if (!ParseOptions(args)) {
				return;
			}
			
			switch (m_Command) {
			case "sign":
				Sign();
				break;
			}
		}
		
		void Sign ()
		{
			var parameters = PrepareParameters();
			
			var dict = new Dictionary<string,object> { 
				{ "method",     m_Options["method"] },
				{ "uri",        m_Options["uri"]    },
				{ "parameters", parameters          }				
			};
			
			var request = RequestProxyFactory.CreateProxy(dict);
			
			if (IsVerbose) {
				Console.WriteLine("OAuth parameters:");
				foreach (var p in request.OAuthParameters) {
					Console.WriteLine(String.Format("  {0}: {1}", p.Key, p.Value));
				}
				Console.WriteLine();
				
				if (request.NonOAuthParameters.Count > 0) {
					Console.WriteLine("Parameters: ");
					foreach (var p in request.NonOAuthParameters) {
						Console.WriteLine(String.Format("  {0}: {1}", p.Key, p.Value));
					}
				}
				Console.WriteLine();
			}
			
			request.Sign(m_Options["oauth_consumer_secret"], m_Options["oauth_token_secret"]);
			
			if (IsVerbose) {
				Console.WriteLine(String.Format("Method: {0}", request.Method));
				Console.WriteLine(String.Format("URI: {0}", request.Uri));
				if (m_Options["xmpp"] != "true") {
					Console.WriteLine(String.Format("Normalized params: {0}", request.NormalizedParameters));
				}
				Console.WriteLine(String.Format("Signature base string: {0}", request.SignatureBaseString));
				
				if (m_Options["xmpp"] == "true") {
					Console.WriteLine();
					Console.WriteLine("Xmpp Stanza:");					
					var xml = "<oauth xmlns='urn:xmpp:tmp:oauth'>                     \n" +
					          "  <oauth_consumer_key>{0}</oauth_consumer_key>         \n" +
					          "  <oauth_token>{1}</oauth_token>                       \n" +
					          "  <oauth_signature_method>{2}</oauth_signature_method> \n" +
					          "  <oauth_signature>{3}</oauth_signature>               \n" +
					          "  <oauth_timestamp>{4}</oauth_timestamp>               \n" +
					          "  <oauth_nonce>{4}</oauth_nonce>                       \n" +
					          "  <oauth_version>{4}</oauth_version>                   \n" +
					          "</oauth>";
					xml = String.Format(xml, request.OAuthConsumerKey, request.OAuthToken, request.OAuthSignatureMethod,
					                    request.OAuthSignature, request.OAuthTimestamp, request.OAuthNonce, 
					                    request.OAuthVersion);
					Console.WriteLine(xml);
					Console.WriteLine();
					Console.WriteLine("Note: You may want to use bare JIDs in your URI.");
					Console.WriteLine();
				} else {
					Console.WriteLine(String.Format("OAuth Request URI: {0}", request.GetSignedUri(true)));
					Console.WriteLine(String.Format("Request URI: {0}", request.GetSignedUri(false)));
					Console.WriteLine(String.Format("Authorization Header: {0}", request.GetOAuthHeader(m_Options["realm"])));
				}
				Console.WriteLine(String.Format("Signature:         {0}", request.OAuthSignature));
				Console.WriteLine(String.Format("Escaped Signature: {0}", Helper.UrlEncode(request.OAuthSignature)));
			}
		}
		
		Dictionary<string, string> PrepareParameters ()
		{
			Console.WriteLine(String.Join(" ", m_Options.Keys.ToArray()));
			var parameters = new Dictionary<string, string> () {
				{ "oauth_consumer_key",     m_Options["oauth_consumer_key"]     },
				{ "oauth_nonce",            m_Options["oauth_nonce"]            },
				{ "oauth_timestamp",        m_Options["oauth_timestamp"]        },
				{ "oauth_token",            m_Options["oauth_token"]            },
				{ "oauth_signature_method", m_Options["oauth_signature_method"] },
				{ "oauth_version",          m_Options["oauth_version"]          }
			};
			
			if (m_Options.ContainsKey("params")) {
				var cliParams = HttpUtility.ParseQueryString((string)m_Options["params"]);
				foreach (string key in cliParams.AllKeys)
					parameters.Add(key, cliParams[key]);
			}
			
			return parameters;
		}
		
		bool ParseOptions (string[] args)
		{
			bool showHelp = false;
			
			var optionSet = new OptionSet() {
				{ "consumer-key=", "Specifies the consumer key to use.",
				  v => m_Options["oauth_consumer_key"] = v },
				
				{ "consumer-secret=", "Specifies the consumer secret to use.",
				  v => m_Options["oauth_consumer_secret"] = v },
				
				{ "method=", "Specifies the method (e.g. GET) to use when signing.",
				  v => m_Options["method"] = v },
				
				{ "nonce=", "Specifies the none to use.",
				  v => m_Options["oauth_nonce"] = v },
				
				{ "parameters=", "Specifies the parameters to use when signing.",
				  v => m_Options["params"] = v },
				
				{ "signature-method=", "Specifies the signature method to use; defaults to HMAC-SHA1.",
				  v => m_Options["oauth_signature_method"] = v },
				
				{ "secret=", "Specifies the token secret to use.",
				  v => m_Options["oauth_token_secret"] = v },
				
				{ "timestamp=", "Specifies the timestamp to use.",
				  v => m_Options["oauth_timestamp"] = v },
				
				{ "token=", "Specifies the token to use.",
				  v => m_Options["oauth_token"] = v },
				
				{ "realm=", "Specifies the realm to use.",
				  v => m_Options["realm"] = v },
				
				{ "uri=", "Specifies the URI to use when signing.",
				  v => m_Options["uri"] = v },
				
				{ "version=", "Specifies the OAuth version to use.",
				  v => m_Options["version"] = v },
				
				{ "xmpp", "Generate XMPP stanzas.",
				  v => { m_Options["xmpp"] = "true"; if (!m_Options.ContainsKey("method")) m_Options["method"] = "iq"; } },
				
				{ "v|verbose", "Be verbose.",
				  v => m_Options["verbose"] = "true" },
				
				{ "h|help", "Show this message and exit.",
				  v => showHelp = (v != null) }
			};
			
			try {
				var extra = optionSet.Parse(args);
				if (extra.Count() > 0) {
					m_Command = extra.Last();
				}
			} catch (OptionException ex) {
				Console.Write("oauth: ");
				Console.WriteLine(ex.Message);
				Console.WriteLine("Try `oauth --help' for more information.");
				Environment.Exit(-1);
				return false;
			}		
			
			if (showHelp || !IsSufficientOptions || !IsValidCommand) {
				ShowHelp(optionSet);
				return false;
			}
			
			return true;
		}
		
		bool IsSufficientOptions {
			get {
				var requiredOptions = new [] { "oauth_consumer_key", "oauth_consumer_secret", "method", "uri" };
				return requiredOptions.All(optionName =>
					m_Options.ContainsKey(optionName) && m_Options[optionName] != null
				);
			}
		}
		
		bool IsValidCommand {
			get {
				return SUPPORTED_COMMANDS.Contains(m_Command);
			}
		}	
		
		bool IsVerbose {
			get {
				return m_Options.ContainsKey("verbose") && m_Options["verbose"] == "true";
			}
		}
		
		void ShowHelp (OptionSet optionSet)
		{
			Console.WriteLine ("Usage: oauth [OPTIONS] <command>");
			Console.WriteLine ();
			Console.WriteLine ("Options:");
			optionSet.WriteOptionDescriptions (Console.Out);
			Console.WriteLine("Available commands:");
			foreach (var command in SUPPORTED_COMMANDS) {
				Console.WriteLine("   " + command);
			}
		}
	}
}