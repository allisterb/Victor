using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Victor.Web.Server
{
	public class HttpResponse : IDisposable
	{
		Socket _socket;
		Encoding _encoding = Encoding.UTF8;
		bool _hasSentHeaders = false;
		int _status = 200;
		bool _isKeepAlive = true;
		string _version = null;
		string _statusText = null;
		string _contentType = "text/plain";
		StringBuilder _headers = new StringBuilder();
		WebServer _server;
		internal HttpResponse(WebServer server, HttpRequest request, Socket socket)
		{
			_server = server;
			_socket = socket;
			_version = request.Version;
		}
		void _CheckDisposed()
		{
			if (null == _socket)
				throw new ObjectDisposedException(nameof(HttpResponse));
		}
		void _CheckSentHeaders()
		{
			if (_hasSentHeaders)
				throw new InvalidOperationException("The response headers have already been sent.");
		}
		public void Write(string text, params object[] args)
		{
			_CheckDisposed();
			if (!_hasSentHeaders)
				SendHeaders();
			if (0 != args.Length)
				text = string.Format(text, args);
			_socket.SendHttpChunk(text, _encoding);
		}
		public void Write(object value)
		{
			Write(Convert.ToString(value));
		}
		public void WriteLine(string text, params object[] args)
		{
			Write(string.Concat(text, "\r\n"), args);
		}
		public void WriteLine(object value)
		{
			Write(Convert.ToString(value));
		}
		public void BinaryWrite(byte[] bytes)
		{
			_CheckDisposed();
			if (!_hasSentHeaders)
				SendHeaders();
			_socket.SendHttpChunkAsync(bytes);
		}
		public void SendHeaders()
		{
			_CheckDisposed();
			_CheckSentHeaders();
			var headers = new StringBuilder(_GetStatusLine(Status, StatusText, Version));
			var ka = IsKeepAlive;
			var isCustomKA = HasHeader("Connection");
			if (!isCustomKA)
			{
				headers.Append(string.Concat("Connection: ", ka ? "Keep-Alive" : "Close", "\r\n"));
			}
			if(!HasHeader("Content-Type"))
			{
				var ct = string.Concat("Content-Type: ", ContentType, string.Concat("; charset=", _encoding.HeaderName, "\r\n"));
				headers.Append(ct);
			}
			if (!HasHeader("Transfer-Encoding"))
				headers.Append("Transfer-Encoding: chunked\r\n");
			
			if (0 < _headers.Length)
				headers.Append(_headers);
			
			_socket.Send(string.Concat(headers.ToString(),"\r\n"),Encoding.ASCII);
			_hasSentHeaders = true;
		}
		static string _GetStatusLine(
			int statusCode = 200,
			string status = null,
			string version = null)
		{
			if (string.IsNullOrEmpty(status))
			{
				status = _GetHttpStatusFromStatusCode(statusCode);
				if (string.IsNullOrEmpty(status))
					throw new ArgumentException("The status was not specified and couldn't be determined from the status code.", "status");
			}
			if (null == version)
				version = "HTTP/1.1";
			else if (0 == version.Length)
				version = "HTTP/1.0";
			return string.Concat(version, " ", statusCode, " ", status, "\r\n");
		}
		static string _GetHttpStatusFromStatusCode(int statusCode)
		{
			// copied from wikipedia
			switch (statusCode)
			{
				case 100: return "Continue";
				case 101: return "Switching Protocols";
				case 102: return "Processing";
				case 103: return "Early Hints";
				case 200: return "OK";
				case 201: return "Created";
				case 202: return "Accepted";
				case 203: return "Non-Authoritative Information";
				case 204: return "No Content";
				case 205: return "Reset Content";
				case 206: return "Partial Content";
				case 207: return "Multi-Status";
				case 208: return "Already Reported";
				case 226: return "IM Used";
				case 300: return "Multiple Choices";
				case 301: return "Moved Permanently";
				case 302: return "Found"; //(Previously "Moved temporarily")
				case 303: return "See Other";
				case 304: return "Not Modified";
				case 305: return "Use Proxy";
				case 306: return "Switch Proxy";
				case 307: return "Temporary Redirect";
				case 308: return "Permanent Redirect";
				case 400: return "Bad Request";
				case 401: return "Unauthorized";
				case 402: return "Payment Required";
				case 403: return "Forbidden";
				case 404: return "Not Found";
				case 405: return "Method Not Allowed";
				case 406: return "Not Acceptable";
				case 407: return "Proxy Authentication Required";
				case 408: return "Request Timeout";
				case 409: return "Conflict";
				case 410: return "Gone";
				case 411: return "Length Required";
				case 412: return "Precondition Failed";
				case 413: return "Payload Too Large";
				case 414: return "URI Too Long";
				case 415: return "Unsupported Media Type";
				case 416: return "Range Not Satisfiable";
				case 417: return "Expectation Failed";
				// This code was defined in 1998 as one of the traditional IETF April Fools' jokes, in RFC 2324, Hyper Text Coffee Pot Control Protocol, 
				// and is not expected to be implemented by actual HTTP servers. The RFC specifies this code should be returned by teapots requested 
				// to brew coffee.[52] This HTTP status is used as an Easter egg in some websites, including Google.com.[53][54]
				case 418: return "I'm a teapot";
				case 421: return "Misdirected Request";
				case 422: return "Unprocessable Entity";
				case 423: return "Locked";
				case 424: return "Failed Dependency";
				case 426: return "Upgrade Required";
				case 428: return "Precondition Required";
				case 429: return "Too Many Requests";
				case 431: return "Request Header Fields Too Large";
				case 451: return "Unavailable For Legal Reasons";
				case 500: return "Internal Server Error";
				case 501: return "Not Implemented";
				case 502: return "Bad Gateway";
				case 503: return "Service Unavailable";
				case 504: return "Gateway Timeout";
				case 505: return "HTTP Version Not Supported";
				case 506: return "Variant Also Negotiates";
				case 507: return "Insufficient Storage";
				case 508: return "Loop Detected";
				case 510: return "Not Extended";
				case 511: return "Network Authentication Required";
				default:
					return null;
			}
		}
		public bool IsKeepAlive { 
			get 
			{
				return _isKeepAlive && "HTTP/1.1" == _version.ToUpperInvariant() && _server.IsStarted;
			}
			set {
				_CheckDisposed();
				_CheckSentHeaders();
				_isKeepAlive = value;
			}
		}
		public int Status {
			get {
				return _status;
			}
			set {
				_CheckDisposed();
				_CheckSentHeaders();
				_status = value;
			}
		}
		internal void SendEndChunk()
		{
			_CheckDisposed();
			//_socket.SendAsync("0\r\n\r\n",Encoding.ASCII);
			_socket.SendHttpChunk(new byte[0]);
		}
		public string StatusText {
			get {
				if (string.IsNullOrEmpty(_statusText))
					return _GetHttpStatusFromStatusCode(_status);
				return _statusText;
			}
			set {
				_CheckDisposed();
				_CheckSentHeaders();
				_statusText = value;
			}
		}
		public string ContentType {
			get {
				return _contentType;
			}
			set {
				_CheckDisposed();
				_CheckSentHeaders();
				_contentType = value;
			}
		}
		public string Version {
			get {
				return _version;
			}
			set {
				_CheckDisposed();
				_CheckSentHeaders();
				var s = value.ToUpperInvariant();
				if (null==s || "HTTP/1.1" == s)
					_version = s;
				else if (0==s.Length || "HTTP/1.0" == s)
					_version = s;
				else
					throw new NotSupportedException("The specified HTTP version is not supported.");
				
			}
		}
		public bool IsClosed {  get { return null == _socket; } }
		public bool HasHeader(string name)
		{
			name = string.Concat(name.ToUpperInvariant(), ":");
			var s = _headers.ToString().ToUpperInvariant();
			return s.StartsWith(name) || s.Contains(string.Concat("\r\n", name));
		}
		public bool HasSentHeaders {
			get {
				return _hasSentHeaders;
			}
		}
		public void AddHeader(string name,string value)
		{
			_CheckDisposed();
			_CheckSentHeaders();
			_headers.Append(string.Concat(string.Concat(name, ": ", value), "\r\n"));
		}
		public void Close()
		{
			if (null != _socket)
			{
				_socket.Close();
				_socket = null;
			}
			GC.SuppressFinalize(this);
		}
		void IDisposable.Dispose()
			=>Close();
		
		~HttpResponse()
		{
			(this as IDisposable).Dispose();
		}
	}
}
