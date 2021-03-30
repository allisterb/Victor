using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Victor.Web.Server
{
	


	public sealed class HttpRequest
	{
		public string Headers { get; private set; }
		public Stream Body { get; private set; }
		public HttpRequest(string headers, Stream body = null)
		{
			Headers = headers;
			Body = body;
		}
		public bool IsHttp10 {
			get {

				string s = Version;
				if (null != s && ("" == s || 0 == string.Compare("HTTP/1.0", s, StringComparison.InvariantCultureIgnoreCase) || 0 == string.Compare("HTTP/1", s, StringComparison.InvariantCultureIgnoreCase)))
					return true;
				return false;
			}
		}
		public bool HasHeader(string name)
		{
			if (0 > Headers.IndexOf(string.Concat(name, ":"), StringComparison.InvariantCultureIgnoreCase))
				return false;
			return true;
		}
		public bool TryGetRequestHeaderValue(string name, out string value)
		{
			value = null;
			int i = Headers.IndexOf(string.Concat(name, ":"), StringComparison.InvariantCultureIgnoreCase);
			if (0 > i) return false;
			int j = i + (name.Length + 1);
			if (Headers.Length <= j)
				return false;
			i = j;
			j = Headers.IndexOf("\r\n", i);
			if (0 > j)
				return false;
			value = Headers.Substring(i, j - i).TrimStart();
			return true;
		}
		public string Method {
			get {
				int i = Headers.IndexOf(' ');
				if (-1 < i)
				{
					return Headers.Substring(0, i);
				}
				return null;
			}
		}
		private static readonly char[] _pathTerminators = new char[] { '?', '#', '|', '&' };
		public string Path {
			get {
				string pq = PathAndQuery;
				int i = pq.IndexOfAny(_pathTerminators);
				if (0 > i)
					return pq;
				return pq.Substring(0, i);
			}
		}
		public bool TryGetFirstQueryValue(string name, out string value)
		{
			value = null;
			int i, j;
			string q = Query;
			if (q.StartsWith(name + "="))
			{
				i = name.Length + 1;
				j = q.IndexOfAny(_pathTerminators, i);
				if (0 > j)
					j = q.Length;
				value = q.Substring(i, j - i);
				return true;
			}
			i = q.IndexOf("&" + name + "=");
			if (0 > i)
				return false;
			i = name.Length + 2 + i;
			j = q.IndexOfAny(_pathTerminators, i);
			if (0 > j)
				j = q.Length;
			value = q.Substring(i, j - i);
			return true;
		}
		public IEnumerable<KeyValuePair<string, string>> QueryValues {
			get {
				string[] sa = Query.Split('&');
				foreach (string s in sa)
				{
					int i = s.IndexOf('=');
					if (0 > i)
						yield return new KeyValuePair<string, string>(s, null);
					else
						yield return new KeyValuePair<string, string>(s.Substring(0, i), s.Substring(i + 1));
				}
			}
		}
		public string Query {
			get {
				string pq = PathAndQuery;
				int i = pq.IndexOf('?');
				if (0 > i)
					i = pq.IndexOf('&');
				if (0 > i)
					return "";
				return pq.Substring(i + 1);
			}
		}
		public string PathAndQuery {
			get {
				int i = Headers.IndexOf(' ');
				if (0 > i || i == Headers.Length - 1)
					return null;
				++i;
				int j = Headers.IndexOf(' ', i);
				if (0 > j)
					j = Headers.Length;

				return Headers.Substring(i, j - i);
			}
		}
		public string Version {
			get {
				int i = Headers.IndexOf(' ');
				if (0 > i || i == Headers.Length - 1)
					return null;
				i = Headers.IndexOf(' ', i + 1);
				if (0 > i)
					return null;
				++i;
				int j = Headers.IndexOf('\r', i + 1);
				if (0 > j)
					j = Headers.Length;
				return Headers.Substring(i, j - i);
			}
		}
	}
}
