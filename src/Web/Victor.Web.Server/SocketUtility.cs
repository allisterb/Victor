using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Victor.Web.Server
{
	public static class SocketUtility
	{
		// from https://blogs.msdn.microsoft.com/pfxteam/2011/12/15/awaiting-socket-operations/
		sealed class _SocketAwaitable : INotifyCompletion
		{
			private readonly static Action _sentinel = () => { };

			internal bool _wasCompleted;
			internal Action _continuation;
			internal SocketAsyncEventArgs _eventArgs;

			public _SocketAwaitable(SocketAsyncEventArgs eventArgs)
			{
				if (null == eventArgs) throw new ArgumentNullException("eventArgs");
				_eventArgs = eventArgs;
				eventArgs.Completed += delegate
				{
					var prev = _continuation ?? Interlocked.CompareExchange(
						ref _continuation, _sentinel, null);
					if (prev != null) prev();
				};
			}

			internal void Reset()
			{
				_wasCompleted = false;
				_continuation = null;
			}

			public _SocketAwaitable GetAwaiter() { return this; }

			public bool IsCompleted { get { return _wasCompleted; } }

			public void OnCompleted(Action continuation)
			{
				if (_continuation == _sentinel ||
					Interlocked.CompareExchange(
						ref _continuation, continuation, null) == _sentinel)
				{
					Task.Run(continuation);
				}
			}

			public void GetResult()
			{
				if (_eventArgs.SocketError != SocketError.Success)
					throw new SocketException((int)_eventArgs.SocketError);
			}
		}
		public static Task<int> ReceiveAsync(
	this Socket socket, byte[] buffer, int offset, int size,
	SocketFlags socketFlags)
		{
			var tcs = new TaskCompletionSource<int>(socket);
			socket.BeginReceive(buffer, offset, size, socketFlags, iar =>
			{
				var t = (TaskCompletionSource<int>)iar.AsyncState;
				var s = (Socket)t.Task.AsyncState;
				try { t.TrySetResult(s.EndReceive(iar)); }
				catch (Exception exc) { t.TrySetException(exc); }
			}, tcs);
			return tcs.Task;
		}
		public static Task<int> SendAsync(
	this Socket socket, byte[] buffer, int offset, int size,
	SocketFlags socketFlags)
		{
			var tcs = new TaskCompletionSource<int>(socket);
			socket.BeginSend(buffer, offset, size, socketFlags, iar =>
			{
				var t = (TaskCompletionSource<int>)iar.AsyncState;
				var s = (Socket)t.Task.AsyncState;
				try { t.TrySetResult(s.EndSend(iar)); }
				catch (Exception exc) { tcs.TrySetException(exc); }
			}, tcs);
			return tcs.Task;

		}

		public static Task<int> SendAsync(this Socket socket, string text, Encoding encoding, SocketFlags flags = 0)
		{
			byte[] ba = encoding.GetBytes(text);
			return SendAsync(socket, ba, 0, ba.Length, flags);
		}
		public static int Send(this Socket socket, string text, Encoding encoding, SocketFlags flags = 0)
		{
			byte[] ba = encoding.GetBytes(text);
			return socket.Send(ba, 0, ba.Length, flags);
		}

		static byte[] _MakeChunk(byte[] data)
		{
			int i = data.Length;
			string len = i.ToString("x");
			byte[] result = new byte[i + 4 + len.Length];
			Buffer.BlockCopy(Encoding.ASCII.GetBytes(len), 0, result, 0, len.Length);
			i = len.Length;
			result[i] = (byte)'\r';
			result[i + 1] = (byte)'\n';
			i += 2;
			Buffer.BlockCopy(data, 0, result, i, data.Length);
			i += data.Length;
			result[i] = (byte)'\r';
			result[i + 1] = (byte)'\n';
			return result;
		}
		public static Task<int> SendHttpChunkAsync(this Socket socket, byte[] data, SocketFlags flags = 0)
		{
			byte[] chunk = _MakeChunk(data);
			return SendAsync(socket, chunk, 0, chunk.Length, flags);
		}
		public static Task<int> SendHttpChunkAsync(this Socket socket, string text, Encoding encoding, SocketFlags flags = 0)
		{
			byte[] data = encoding.GetBytes(text);
			return SendHttpChunkAsync(socket, data, flags);
		}

		public static int SendHttpChunk(this Socket socket, byte[] data, SocketFlags flags = 0)
		{
			byte[] chunk = _MakeChunk(data);
			return socket.Send(chunk, 0, chunk.Length, flags);
		}
		public static int SendHttpChunk(this Socket socket, string text, Encoding encoding, SocketFlags flags = 0)
		{
			byte[] data = encoding.GetBytes(text);
			return SendHttpChunk(socket, data, flags);
		}
		/// <summary>
		/// Asynchronously and minimally processes an http request on the specified socket. Reads the data, including any headers and the request body, if present, and then leaves the socket in a state ready to send a response.
		/// </summary>
		/// <param name="socket">The socket connected to the remote client that made the request</param>
		/// <param name="requestBodyCallback">An optional callback that will substitute large data handling in the request body. If null, the data sent with the request will be available in the RequestBody field.</param>
		/// <returns>The info associated with the request, encapsulated in a task.</returns>
		public static async Task<HttpRequest> ReceiveHttpRequestAsync(this Socket socket, ProcessRequestBody requestBodyCallback = null)
		{
			StringBuilder reqheaders = null;
			Stream body = null;
			int bytesRead = -2;
			byte[] recv = new byte[1024];
			if (0 != bytesRead)
			{
				reqheaders = new StringBuilder();
				string s = (0<bytesRead)?Encoding.ASCII.GetString(recv, 0, bytesRead):"";
				reqheaders.Append(s);
				int i = reqheaders.ToString().IndexOf("\r\n\r\n");
				while (0 > i && 0 != bytesRead)
				{
					bytesRead = await socket.ReceiveAsync(recv, 0, recv.Length, 0);
					if (0 != bytesRead)
					{
						s = Encoding.ASCII.GetString(recv, 0, bytesRead);
						reqheaders.Append(s);
						i = reqheaders.ToString().IndexOf("\r\n\r\n");
					}
				}
				if (0 > i)
					throw new Exception("Bad Request");
				long rr = 0;
				if (i + 4 < reqheaders.Length)
				{
					byte[] data = Encoding.ASCII.GetBytes(reqheaders.ToString(i + 4, reqheaders.Length - (i + 4)));
					rr = data.Length;
					// process request body data
					if (null != requestBodyCallback)
					{
						requestBodyCallback(socket, reqheaders.ToString(), data);
					}
					else
					{
						if (null == body)
							body = new MemoryStream();
						body.Write(data,0,data.Length);
					}
				}
				int ci = reqheaders.ToString().IndexOf("Content-Length:", StringComparison.InvariantCultureIgnoreCase);
				if (-1 < ci)
				{
					// we have more post data
					ci += 15;
					while (ci < reqheaders.Length && char.IsWhiteSpace(reqheaders[ci]))
						++ci;
					long cl = 0;
					while (ci < reqheaders.Length && char.IsDigit(reqheaders[ci]))
					{
						cl *= 10;
						cl += reqheaders[ci] - '0';
						++ci;
					}
					if ('\r' != reqheaders[ci] && '\n' != reqheaders[ci])
						throw new Exception("Bad Request");
					long l = rr;
					while (l < cl)
					{
						bytesRead = await socket.ReceiveAsync(recv, 0, recv.Length, 0);
						if (0 < bytesRead)
						{
							l += bytesRead;
							byte[] data = new byte[bytesRead];
							Buffer.BlockCopy(recv, 0, data, 0, bytesRead);
							// process request body data
							if (null != requestBodyCallback)
							{
								requestBodyCallback(socket, reqheaders.ToString(), data);
							}
							else
							{
								if (null == body)
									body = new MemoryStream();
								body.Write(data,0,data.Length);
							}
						}
					}
				}
				reqheaders.Length = i + 2;
				return new HttpRequest(reqheaders.ToString(), body);
			}
			socket.Close();
			return null;
		}

		public static string GetHttpResponseHeaders(string contentType = null, long contentLength = -1L, bool chunkedEncoding = false, int keepAlive = -1)
		{
			StringBuilder result = new StringBuilder();
			result.Append("Date: ");
			result.Append(DateTime.Now.ToUniversalTime().ToString("r"));
			result.Append("\r\n");
			if (chunkedEncoding)
				result.Append("Transfer-Encoding: Chunked\r\n");
			if (!string.IsNullOrEmpty(contentType))
			{
				result.Append("Content-Type: ");
				result.Append(contentType);
				result.Append("\r\n");
			}
			if (-1L != contentLength)
			{
				result.Append("Content-Length: ");
				result.Append(contentLength);
				result.Append("\r\n");
			}
			if (0 < keepAlive)
			{
				result.Append("Connection: Keep-Alive\r\n");
			}
			else if (0 == keepAlive)
			{
				result.Append("Connection: Closed\r\n");
			}

			return result.ToString();
		}
		
		/// <summary>
		/// Synchronously and minimally processes an http request on the specified socket. Reads the data, including any headers and the request body, if present, and then leaves the socket in a state ready to send a response.
		/// </summary>
		/// <param name="socket">The socket connected to the remote client that made the request</param>
		/// <param name="requestBodyCallback">An optional callback that will substitute large data handling in the request body. If null, the data sent with the request will be available in the RequestBody field.</param>
		/// <returns>The info associated with the request.</returns>
		/// <remarks>You're virtually always better off using the asynchronous method. However, if you're already spawning threads for handling connections, this might be a decent way to put some to work. However, the asynchronous methods may still provide better performance in that scenario. Benchmark if you care. If you care about performance and scalability, this isn't the right method server for you anyway.</remarks>
		public static HttpRequest ReceiveHttpRequest(this Socket socket, ProcessRequestBody requestBodyCallback = null)
		{
			StringBuilder reqheaders = null;
			Stream body = null;
			int bytesRead = -2;
			byte[] recv = new byte[1024];
			bytesRead = socket.Receive(recv, 0, recv.Length, 0);
			if (0 != bytesRead)
			{
				reqheaders = new StringBuilder();
				string s = Encoding.ASCII.GetString(recv, 0, bytesRead);
				reqheaders.Append(s);
				int i = reqheaders.ToString().IndexOf("\r\n\r\n");
				while (0 > i && 0 != bytesRead)
				{
					bytesRead = socket.Receive(recv, 0, recv.Length, 0);
					if (0 != bytesRead)
					{
						s = Encoding.ASCII.GetString(recv, 0, bytesRead);
						reqheaders.Append(s);
						i = reqheaders.ToString().IndexOf("\r\n\r\n");
					}
				}
				if (0 > i)
					throw new Exception("Bad Request");
				long rr = 0;
				if (i + 4 < reqheaders.Length)
				{
					byte[] data = Encoding.ASCII.GetBytes(reqheaders.ToString(i + 4, reqheaders.Length - (i + 4)));
					rr = data.Length;
					// process request body data
					if (null != requestBodyCallback)
					{
						requestBodyCallback(socket, reqheaders.ToString(), data);
					}
					else
					{
						if (null == body)
							body = new MemoryStream();
						body.Write(data,0,data.Length);
					}
				}
				int ci = reqheaders.ToString().IndexOf("Content-Length:", StringComparison.InvariantCultureIgnoreCase);
				if (-1 < ci)
				{
					// we have more post data
					ci += 15;
					while (ci < reqheaders.Length && char.IsWhiteSpace(reqheaders[ci]))
						++ci;
					long cl = 0;
					while (ci < reqheaders.Length && char.IsDigit(reqheaders[ci]))
					{
						cl *= 10;
						cl += reqheaders[ci] - '0';
						++ci;
					}
					if ('\r' != reqheaders[ci] && '\n' != reqheaders[ci])
						throw new Exception("Bad Request");
					long l = rr;
					while (l < cl)
					{
						bytesRead = socket.Receive(recv, 0, recv.Length, 0);
						if (0 < bytesRead)
						{
							l += bytesRead;
							byte[] data = new byte[bytesRead];
							Buffer.BlockCopy(recv, 0, data, 0, bytesRead);
							// process request body data
							if (null != requestBodyCallback)
							{
								requestBodyCallback(socket, reqheaders.ToString(), data);
							}
							else
							{
								if (null == body)
									body = new MemoryStream();
								body.Write(data,0,data.Length);
							}
						}
					}
				}
				reqheaders.Length = i + 2;
				return new HttpRequest(reqheaders.ToString(), body);
			}
			socket.Close();
			return null;
		}
		
	}
	public delegate void ProcessRequestBody(Socket socket, string headers, byte[] data);

}
