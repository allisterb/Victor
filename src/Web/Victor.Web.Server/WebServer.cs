using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Net;
using System.Globalization;

namespace Victor.Web.Server
{
	public class WebServer : Component
	{
		EventHandlerList _eventHandlers = new EventHandlerList();
		static readonly object _processRequestEventKey = new object();
		static readonly object _endPointChangedEventKey = new object();
		static readonly object _isStartedChangedEventKey = new object();
		const int _backLog = 10;
		Socket _listener;
		IPEndPoint _endPoint;
		readonly object _lockStart = new object();
		void _Start()
		{
			_Stop();
			lock (_lockStart)
			{
				_listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
			}
			_listener.Bind(_endPoint);
			_listener.Listen(_backLog);
			_listener.BeginAccept(new AsyncCallback(_HandleAccept),_listener);
			//while(true)
			//	_HandleAcceptImpl(_listener.Accept());


		}
		void _Stop()
		{
			lock (_lockStart)
			{
				if (null != _listener)
				{
					_listener.Close();
					_listener = null;
				}
			}
		}
		[TypeConverter(typeof(_EndPointConverter))]
		public IPEndPoint EndPoint {
			get {
				return _endPoint;
			}
			set {
				if (value != _endPoint)
				{
					// we don't want existing connections to close in the middle of the restart
					// so we lock the entire process of restarting
					lock (_lockStart)
					{
						if(null!=_listener)
						{
							_listener.Close();
							_listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
							_listener.Bind(value);
							_listener.Listen(_backLog);
							_listener.BeginAccept(new AsyncCallback(_HandleAccept), _listener);
						}
					}
					_endPoint = value;
					OnEndPointChanged(EventArgs.Empty);
				}
			}
		}
		public event EventHandler EndPointChanged {
			add {
				_eventHandlers.AddHandler(_endPointChangedEventKey, value);
			}
			remove {
				_eventHandlers.RemoveHandler(_endPointChangedEventKey, value);
			}
		}
		protected virtual void OnEndPointChanged(EventArgs args)
		{
			(_eventHandlers[_endPointChangedEventKey] as EventHandler)?.Invoke(this, args);
		}
		bool _IsDesignMode {
			get {
				if (null == Site)
					return false;
				return Site.DesignMode;
			}
		}
		public bool IsStarted {
			get {
				lock (_lockStart)
				{
					return null != _listener;
				}
			}
			set {
				if (!_IsDesignMode)
				{
					if (value)
					{
						lock (_lockStart)
						{
							if (null != _listener)
								return;
							_listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
						}
						_listener.Bind(_endPoint);
						_listener.Listen(_backLog);
						_listener.BeginAccept(new AsyncCallback(_HandleAccept), _listener);
					}
					else
					{
						lock (_lockStart)
						{
							if (null == _listener)
								return;
							_listener.Close();
							_listener = null;
						}
					}
				}
				OnIsStartedChanged(EventArgs.Empty);
			}
		}
		protected virtual void OnIsStartedChanged(EventArgs args)
		{
			(_eventHandlers[_isStartedChangedEventKey] as EventHandler)?.Invoke(this, args);
		}
		public event EventHandler IsStartedChanged {
			add {
				_eventHandlers.AddHandler(_isStartedChangedEventKey, value);
			}
			remove {
				_eventHandlers.RemoveHandler(_isStartedChangedEventKey, value);
			}
		}
		public event ProcessRequestEventHandler ProcessRequest {
			add {
				_eventHandlers.AddHandler(_processRequestEventKey, value);
			}
			remove {
				_eventHandlers.RemoveHandler(_processRequestEventKey, value);
			}
		}
		protected virtual void OnProcessRequest(ProcessRequestEventArgs args)
		{
			(_eventHandlers[_processRequestEventKey] as ProcessRequestEventHandler)?.Invoke(this, args);
		}
		void _HandleAccept(IAsyncResult result)
		{
			var listener = result.AsyncState as Socket;
			var socket=listener.EndAccept(result);
			_HandleAcceptImpl(socket);
			if(IsStarted)
				listener.BeginAccept(new AsyncCallback(_HandleAccept), listener);
		}
		void _HandleAcceptImpl(Socket socket)
		{
			var done = false;
			HttpRequest hreq = null;
			while (!done)
			{
				Task<HttpRequest> t= socket.ReceiveHttpRequestAsync();
				t.Wait();
				var newHreq =t.Result;
				if (null != newHreq)
					hreq = newHreq;
				else
					done = true;
				if (null != hreq)
				{
					var hres = new HttpResponse(this, hreq, socket);
					OnProcessRequest(new ProcessRequestEventArgs(hreq, hres));
					if (!hres.IsClosed)
					{
						if (!hres.HasSentHeaders)
							hres.SendHeaders();
						hres.SendEndChunk();

						if (!hres.HasHeader("Connection") && !hres.IsKeepAlive)
						{
							hres.Close();
							done = true;
						}

					}
					else
						done = true;
				}
				else
					done = true;
			}
		}
		class _EndPointConverter : TypeConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				if (typeof(string) == sourceType || typeof(IPEndPoint) == sourceType)
					return true;
				return base.CanConvertFrom(context, sourceType);
			}
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				if (typeof(string) == destinationType || typeof(IPEndPoint) == destinationType)
					return true;
				return base.CanConvertTo(context, destinationType);
			}
			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				var s = value as string;
				if (null != s)
				{
					if (0 == s.Length)
						return new IPEndPoint(IPAddress.Any, 80);

					var i = s.LastIndexOf(':');
					var port = 80;
					if (0 > i)
					{
						port = int.Parse(s.Substring(i+1));
						s = s.Substring(0, i);
					}
					var addr = (0 == s.Length || "*" == s) ? IPAddress.Any : IPAddress.Parse(s);
					
					return new IPEndPoint(addr, port);
				}

				var ep = value as IPEndPoint;

				if (null != ep)
				{
					var a = ep.Address.ToString();
					if ("0.0.0.0" == a)
						a = "*";
					return string.Concat(a, ":", ep.Port.ToString());
				}
					
				

				return base.ConvertFrom(context, culture, value);
			}
		}

	}
	public delegate void ProcessRequestEventHandler(object sender, ProcessRequestEventArgs args);
	public class ProcessRequestEventArgs : EventArgs
	{
		internal ProcessRequestEventArgs(HttpRequest request,HttpResponse response)
		{
			Request = request;
			Response = response;
		}
		public HttpRequest Request { get; }
		public HttpResponse Response { get; }
	}

}
