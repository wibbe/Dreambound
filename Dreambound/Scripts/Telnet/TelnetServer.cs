/**
 * The MIT License (MIT)
 * 
 * Copyright (c) 2015 Dreambound Studios AB
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

namespace Dreambound.Telnet
{

	public class TelnetServer
	{
		private TcpListener m_server = null;
		private IPAddress m_bindAddress = null;
		private int m_port = 23;
		private int m_clientId = 1;

		private Dictionary<int, TelnetNVT> m_activeClients = null;
		private List<TelnetNVT> m_pendingRemoval = null;

		public delegate void ClientConnected(TelnetNVT client);
		public delegate void ClientDisconnected(TelnetNVT client);
		public delegate void DataReceived(TelnetNVT client, string line);

		public event ClientConnected OnClientConnected;
		public event ClientDisconnected OnClientDisconnected;
		public event DataReceived OnDataReceived;
		
		public TelnetServer(int port = 23)
		{
			m_port = port;
			m_activeClients = new Dictionary<int, TelnetNVT>();
			m_pendingRemoval = new List<TelnetNVT>();
		}

		public void Start()
		{
			m_bindAddress = GetHostAddress();
			m_server = new TcpListener(m_bindAddress, m_port);
			m_server.Start();
			m_clientId = 1;

			Console.WriteLine("Telnet server running on address {0}", m_bindAddress.ToString());
		}

		public void Stop()
		{
			foreach (var pair in m_activeClients)
			{
				pair.Value.Stop();

				if (OnClientDisconnected != null)
					OnClientDisconnected(pair.Value);
			}

			m_activeClients.Clear();
			m_pendingRemoval.Clear();

			m_server.Stop();
			m_server = null;
		}

		public void Update()
		{
			if (m_server == null)
				return;

			IEnumerator<KeyValuePair<int, TelnetNVT>> it = m_activeClients.GetEnumerator();
			while (it.MoveNext())
				it.Current.Value.Update();

			if (m_server.Pending())
			{
				TcpClient client = m_server.AcceptTcpClient();

				Console.WriteLine("Client connected from {0}", client.Client.RemoteEndPoint.ToString());

				TelnetNVT telnetClient = new TelnetNVT(this, client, m_clientId++);
				telnetClient.Start();
				m_activeClients.Add(telnetClient.Id, telnetClient);

				if (OnClientConnected != null)
					OnClientConnected(telnetClient);
			}

			if (m_pendingRemoval.Count > 0)
			{
				for (int i = 0; i < m_pendingRemoval.Count; i++)
				{
					var client = m_pendingRemoval[i];
					m_activeClients.Remove(client.Id);
					client.Stop();

					if (OnClientDisconnected != null)
						OnClientDisconnected(client);				
				}

				m_pendingRemoval.Clear();
			}
		}

		internal void RemoveClient(TelnetNVT client)
		{
			m_pendingRemoval.Add(client);
		}

		internal void DataReceivedEvent(TelnetNVT client, string text)
		{
			if (OnDataReceived != null)
				OnDataReceived(client, text);
		}

		private IPAddress GetHostAddress()
		{
			//IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
			//if (addresses.Length > 0)
			//	return addresses[0];
			//else
				return IPAddress.Parse("127.0.0.1");
		}
	}
}
