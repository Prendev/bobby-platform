using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
			_accounts = accounts.Where(a => a.ProfileProxyId.HasValue).ToList();
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
			foreach (var account in _accounts)
			{
				account.Listener?.Stop();
				account.Listener = null;
			}
		}

		private void InnerStart()
		{
			while (_isStarted)
			{
				foreach (var pp in _profileProxies) Check(pp);
				foreach (var acc in _accounts) Check(acc);
			}
		}

		private void Check(ProfileProxy pp)
		{
			if (!Uri.TryCreate($"https://{pp.Proxy.Url}", UriKind.Absolute, out Uri proxy)) return;
			if (!Uri.TryCreate($"https://{pp.Destination}", UriKind.Absolute, out Uri local)) return;
			StartForwarding(pp.Proxy, proxy, local, pp.Listener);
		}

		private void Check(Account acc)
		{
			if (string.IsNullOrWhiteSpace(acc.Destination)) return;
			if (!Uri.TryCreate($"https://{acc.ProfileProxy.Proxy.Url}", UriKind.Absolute, out Uri proxy)) return;
			if (!Uri.TryCreate($"https://{acc.Destination}", UriKind.Absolute, out Uri local)) return;
			StartForwarding(acc.ProfileProxy.Proxy, proxy, local, acc.Listener);
		}

		private void StartForwarding(Proxy proxy, Uri proxyUri, Uri localUri, TcpListener listener)
		{
			if (listener == null || !listener.Pending()) return;

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

			var forwardClient = proxyClient.CreateConnection(localUri.Host, localUri.Port);
			var localClient = listener.AcceptTcpClient();

			Task.Factory.StartNew(() => StreamFromTo(localClient.GetStream(), forwardClient.GetStream()),
				TaskCreationOptions.LongRunning);
			Task.Factory.StartNew(() => StreamFromTo(forwardClient.GetStream(), localClient.GetStream()),
				TaskCreationOptions.LongRunning);
		}

		void StreamFromTo(NetworkStream from, NetworkStream to)
		{
			while (_isStarted)
			{
				if (!from.DataAvailable) continue;
				from.CopyTo(to);
			}
		}
	}
}
