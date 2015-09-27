using System;
using Dreambound;
using Dreambound.Telnet;

namespace ServerTest
{
	public class TelnetTestServer
	{
		private TelnetServer m_server = null;

		public TelnetTestServer()
		{
			m_server = new TelnetServer();
			m_server.OnClientConnected += ClientConnected;
			m_server.OnClientDisconnected += ClientDisconnected;
			m_server.OnDataReceived += DataReceived;
			m_server.Start();
		}

		public void Update()
		{
			m_server.Update();
		}

		private void ClientConnected(TelnetNVT client)
		{
			Console.WriteLine("Client {0} connected", client.Id);
			client.WriteLine("Welcome to the Telnet server!");
			client.Write("> ");
		}

		private void ClientDisconnected(TelnetNVT client)
		{
			Console.WriteLine("Client {0} disconnected", client.Id);
		}

		private void DataReceived(TelnetNVT client, string text)
		{
			Console.WriteLine("Client {0} got '{1}'", client.Id, text);
			client.Write("> ");
		}

		public static void Main(string[] args)
		{
			TelnetTestServer server = new TelnetTestServer();

			while (true)
				server.Update();
		}
	}
}
