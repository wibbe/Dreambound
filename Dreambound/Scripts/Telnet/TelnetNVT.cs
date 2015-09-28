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
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace Dreambound.Telnet
{

	/**
	 * Represents a connected "Network Virtual Terminal".
	 */
	public class TelnetNVT
	{
		private TelnetServer m_server = null;
		private TcpClient m_client = null;
		private NetworkStream m_stream = null;

		private volatile Queue<string> m_inStream = new Queue<string>();
		private volatile Queue<string> m_outStream = new Queue<string>();
		private readonly object m_inLock = new object();
		private readonly object m_outLock = new object();

		private Thread m_inThread = null;
		private Thread m_outThread = null;

		private bool m_deadSocket = true;

		public int Id { get; private set; }

		public TelnetNVT(TelnetServer server, TcpClient client, int id)
		{
			m_server = server;
			Id = id;

			m_client = client;
			m_stream = m_client.GetStream();

			m_inThread = new Thread(InThreadMain);
			m_outThread = new Thread(OutThreadMain);
		}

		public void Write(string text)
		{
			lock (m_outLock)
			{
				m_outStream.Enqueue(text);
			}
		}

		public void WriteLine(string line)
		{
			lock (m_outLock)
			{
				m_outStream.Enqueue(line + "\r\n");
			}
		}

		public void Start()
		{
			m_deadSocket = false;
			m_inThread.Start();
			m_outThread.Start();
		}

		public void Stop()
		{
			m_inThread.Abort();
			m_outThread.Abort();
			m_deadSocket = true;

			m_stream.Close();
			m_client.Close();
		}

		public void Update()
		{
			if (m_client == null || !m_client.Connected)
				return;

			if (m_deadSocket)
			{
			}

			lock (m_inLock)
			{
				while (m_inStream.Count > 0)
					m_server.DataReceivedEvent(this, m_inStream.Dequeue());
			}
		}

		private void InThreadMain()
		{
			MemoryStream inputStream = new MemoryStream(1024);

			while (m_client.Connected)
			{
				int input = m_stream.ReadByte();
				
				switch (input)
				{
					case -1:
						break;

					case (int)RFC854.IAC:
						int verb = m_stream.ReadByte();
						if (verb == -1)
							break;

						switch (verb)
						{
							case (int)RFC854.IAC:
								inputStream.WriteByte((byte)verb);
								break;

							case (int)RFC854.DO:
							case (int)RFC854.DONT:
							case (int)RFC854.WILL:
							case (int)RFC854.WONT:
								int option = m_stream.ReadByte();
								if (option == -1)
									break;

								// Suppress everything
								m_stream.WriteByte((byte)RFC854.IAC);
								m_stream.WriteByte(verb == (int)RFC854.DO ? (byte)RFC854.WONT : (byte)RFC854.DONT);
								m_stream.WriteByte((byte)option);

								//Console.WriteLine("IAC, {0}, {1}", verb, option);
								break;
						}
						break;
		
					case (int)'\r':	// Ignore the line-feed character
						break;

					case (int)'\n':
						lock (m_inLock)
						{
							var data = inputStream.ToArray();
							inputStream.Position = 0;
							inputStream.SetLength(0);

							char[] encodedData = Encoding.UTF8.GetChars(data);
							m_inStream.Enqueue(new string(encodedData));
						}

						break;

					default:
						inputStream.WriteByte((byte)input);
						break;
				}
			}
		}

		private void OutThreadMain()
		{
			const int SLEEP_TIME_INC = 20;
			const int SLEEP_TIME_MAX = 200;
			int sleepTime = SLEEP_TIME_MAX;

			while (m_client.Connected)
			{
				string data;

				lock (m_outLock)
				{
					if (m_outStream.Count > 0)
						data = m_outStream.Dequeue();
					else
						data = "";
				}

				if (data.Length > 0)
				{
					sleepTime = 0;
					SendRaw(data);
				}
				else
				{
					Thread.Sleep(sleepTime);

					if (sleepTime < SLEEP_TIME_MAX)
						sleepTime += SLEEP_TIME_INC;
				}
			}
		}

		private void SendRaw(string data)
		{
			try
			{
				byte[] buffer = Encoding.UTF8.GetBytes(data);
				m_stream.Write(buffer, 0, buffer.Length);
			}
			catch (Exception exception)
			{
				if (exception is System.IO.IOException || exception is ObjectDisposedException)
					m_deadSocket = true;
				else
					throw;
			}
		}
	}
}

