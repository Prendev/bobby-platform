using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using QvaDev.Communication;
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
		private class Forward
		{
			public ProfileProxy ProfileProxy { get; set; }
			public Uri ProxyUri { get; set; }
			public Uri DestUri { get; set; }
			public Task Forwarding { get; set; }
		}

		private volatile bool _isStarted;
		private List<ProfileProxy> _profileProxies;
		private List<Account> _accounts;
		private List<Forward> _forwards = new List<Forward>();

		public void Start(List<ProfileProxy> profileProxies, List<Account> accounts)
		{
			if (_isStarted) return;
			_accounts = accounts.Where(a => a.Run && a.ProfileProxyId.HasValue).ToList();
			_profileProxies = profileProxies;

			foreach (var profileProxy in _profileProxies)
			{
				profileProxy.Listener = new TcpListener(IPAddress.Any, profileProxy.LocalPort);
				profileProxy.Listener.Start();
			}
			Task.Factory.StartNew(InnerStart, TaskCreationOptions.LongRunning);

			_isStarted = true;
			Logger.Info("Proxies are started");
		}

		public void Stop()
		{
			_isStarted = false;

			foreach (var profileProxy in _profileProxies)
			{
				try
				{
					profileProxy.Listener?.Stop();
					profileProxy.Listener = null;
				}
				catch (Exception e)
				{
					Logger.Error("Proxy stopped exception", e);
				}
			}

			_forwards.Clear();
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
			Forward forward;
			lock (_forwards)
			{
				forward = _forwards.FirstOrDefault(f => f.ProfileProxy == pp && f.ProxyUri == proxyUri && f.DestUri == destUri);
				if (forward == null)
				{
					forward = new Forward()
					{
						ProfileProxy = pp,
						ProxyUri = proxyUri,
						DestUri = destUri
					};
					_forwards.Add(forward);
				}

				if (forward.Forwarding?.IsCompleted == true) return;
			}

			forward.Forwarding = StartForwardingInner(pp, proxyUri, destUri);
		}

		private async Task StartForwardingInner(ProfileProxy pp, Uri proxyUri, Uri destUri)
		{
			TcpClient forwardClient = null;
			TcpClient localClient = null;
			try
			{
				var proxyClient = GetProxyClient(pp.Proxy, proxyUri);
				// ReSharper disable once PossibleNullReferenceException
				forwardClient = proxyClient.CreateConnection(destUri.Host, destUri.Port);
				localClient = pp.Listener.AcceptTcpClient();

				var t1 = StreamFromTo(localClient.GetStream(), forwardClient.GetStream(), new byte[4096]);
				var t2 = StreamFromTo(forwardClient.GetStream(), localClient.GetStream(), new byte[4096]);
				await Task.WhenAll(t1, t2);
			}
			catch (Exception e)
			{
				Logger.Error($"ProxyService.StartForwardingInner({pp}) exception", e);
			}
			finally
			{
				localClient?.Close();
				forwardClient?.Close();
			}

		}

		private async Task StreamFromTo(NetworkStream from, NetworkStream to, byte[] buffer)
		{
			if (!_isStarted) return;
			var read = await from.ReadAsync(buffer, 0, buffer.Length);
			if (!_isStarted) return;
			if (read > 0) await to.WriteAsync(buffer, 0, read);
			await StreamFromTo(from, to, buffer);
		}

		private IProxyClient GetProxyClient(Proxy proxy, Uri proxyUri)
		{
			IProxyClient proxyClient = null;

			if (proxy.Type == Proxy.ProxyTypes.Http)
				proxyClient = string.IsNullOrWhiteSpace(proxy.User)
					? new HttpProxyClient(proxyUri.Host, proxyUri.Port)
					: new HttpProxyClient(proxyUri.Host, proxyUri.Port, proxy.User, proxy.Password);

			else if (proxy.Type == Proxy.ProxyTypes.Socks4)
				proxyClient = string.IsNullOrWhiteSpace(proxy.User)
					? new Socks4ProxyClient(proxyUri.Host, proxyUri.Port)
					: new Socks4ProxyClient(proxyUri.Host, proxyUri.Port, proxy.User);

			else if (proxy.Type == Proxy.ProxyTypes.Socks5)
				proxyClient = string.IsNullOrWhiteSpace(proxy.User)
					? new Socks5ProxyClient(proxyUri.Host, proxyUri.Port)
					: new Socks5ProxyClient(proxyUri.Host, proxyUri.Port, proxy.User, proxy.Password);

			return proxyClient;
		}
	}
}
