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
using Dreambound.Telnet;

#if UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
using UnityEngine;
#endif

namespace Dreambound
{
	public class RemoteDebugger : MonoBehaviour
	{
		private TelnetServer m_server = null;

		private void Awake()
		{
			m_server = new TelnetServer();
			m_server.OnClientConnected += ClientConnected;
			m_server.OnClientDisconnected += ClientDisconnected;
			m_server.OnDataReceived += DataReceived;
		}

		private void OnEnable()
		{
			m_server.Start();
		}

		private void OnDisable()
		{
			m_server.Stop();
		}

		private void Update()
		{
			m_server.Update();
		}

		private void ClientConnected(TelnetNVT client)
		{
			Log.Debug("Client ", client.Id, " connected");
		}

		private void ClientDisconnected(TelnetNVT client)
		{
		}

		private void DataReceived(TelnetNVT client, string data)
		{
		}
	}
}

