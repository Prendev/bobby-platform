using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using QvaDev.Data.Models;
using Starksoft.Aspen.Proxy;

namespace QvaDev.Orchestration.Services
{
	public interface IProxyService
	{
		void Start(List<ProfileProxy> profileProxies, List<Account> accounts);
		void Stop();
	}

	public class ProxyService : IProxyService
	{
		private volatile bool _isStarted;
		private readonly ILog _log;
		private List<ProfileProxy> _profileProxies;
		private List<Account> _accounts;

		public ProxyService(ILog log)
		{
			_log = log;
		}

		public void Start(List<ProfileProxy> profileProxies, List<Account> accounts)
		{
			_accounts = accounts.Where(a => a.Run && a.ProfileProxyId.HasValue).ToList();
			_profileProxies = profileProxies;

			foreach (var profileProxy in _profileProxies)
			{
				profileProxy.Listener = new TcpListener(IPAddress.Any, profileProxy.LocalPort);
				profileProxy.Listener.Start();
			}
			Task.Factory.StartNew(InnerStart, TaskCreationOptions.LongRunning);

			_isStarted = true;
			_log.Info("Proxies are started");
		}

		public void Stop()
		{
			_isStarted = false;

			foreach (var profileProxy in _profileProxies)
			{
				profileProxy.Listener?.Stop();
				profileProxy.Listener = null;
			}
		}

		private void InnerStart()
		{
			while (_isStarted)
			{
				foreach (var pp in _profileProxies) Check(pp);
				foreach (var acc in _accounts) Check(acc);
				Thread.Sleep(1000);
			}
		}

		private void Check(ProfileProxy pp)
		{
			if (!_isStarted) return;
			Check(pp, pp.DestinationHost, pp.DestinationPort);
		}

		private void Check(Account acc)
		{
			if (!_isStarted) return;
			if (string.IsNullOrWhiteSpace(acc.DestinationHost)) return;
			Check(acc.ProfileProxy, acc.DestinationHost, acc.DestinationPort);
		}

		private void Check(ProfileProxy pp, string destHost, int destPort)
		{
			if (!Uri.TryCreate($"https://{pp.Proxy.Host}:{pp.Proxy.Port}", UriKind.Absolute, out Uri proxyUri)) return;
			if (!Uri.TryCreate($"https://{destHost}:{destPort}", UriKind.Absolute, out Uri dest)) return;
			if (pp.Listener?.Pending() != true) return;
			StartForwarding(pp, proxyUri, dest);
		}

		private void StartForwarding(ProfileProxy pp, Uri proxyUri, Uri destUri)
		{
			var proxy = pp.Proxy;
			IProxyClient proxyClient = null;
			if (proxy.Type == Proxy.ProxyTypes.Socks4)
				proxyClient = string.IsNullOrWhiteSpace(proxy.User)
					? new Socks4ProxyClient(proxyUri.Host, proxyUri.Port)
					: new Socks4ProxyClient(proxyUri.Host, proxyUri.Port, proxy.User);
			else if (proxy.Type == Proxy.ProxyTypes.Socks5)
				proxyClient = string.IsNullOrWhiteSpace(proxy.User)
					? new Socks5ProxyClient(proxyUri.Host, proxyUri.Port)
					: new Socks5ProxyClient(proxyUri.Host, proxyUri.Port, proxy.User, proxy.Password);
			if (proxyClient == null) return;

			var forwardClient = proxyClient.CreateConnection(destUri.Host, destUri.Port);
			var localClient = pp.Listener.AcceptTcpClient();

			Task.Factory.StartNew(() => StreamFromTo(localClient, forwardClient), TaskCreationOptions.LongRunning);
			Task.Factory.StartNew(() => StreamFromTo(forwardClient, localClient), TaskCreationOptions.LongRunning);
		}

		void StreamFromTo(TcpClient from, TcpClient to)
		{
			var fs = from.GetStream();
			var ts = to.GetStream();

			var byteSize = 4096;
			var buffer = new byte[byteSize];
			int read;

			while (_isStarted)
			{
				read = fs.Read(buffer, 0, buffer.Length);
				if (read <= 0) continue;
				ts.Write(buffer, 0, read);
			}

			from.Close();
			to.Close();
			from.Dispose();
			to.Dispose();
		}
	}
}
