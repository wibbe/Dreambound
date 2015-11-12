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
using System.Text;

#if UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
using UnityEngine;
#endif

namespace Dreambound
{
	public static class Log
	{
		public static string Debug(params object[] data)
		{
			StringBuilder sb = new StringBuilder();

			#if UNITY_EDITOR
			sb.Append("Debug: ".Bold().Colored(Colors.green));
			#else
			sb.Append("Debug: ");
			#endif

			for (int i = 0; i < data.Length; i++)
				sb.Append(data[i].ToString());

			string s = sb.ToString();

			#if UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
			UnityEngine.Debug.Log(s);
			#else
			Console.WriteLine(s);
			#endif

			return s;
		}

		public static string Info(params object[] data)
		{
			StringBuilder sb = new StringBuilder();

			#if UNITY_EDITOR
			sb.Append("Info: ".Bold());
			#else
			sb.Append("Info: ");
			#endif

			for (int i = 0; i < data.Length; i++)
				sb.Append(data[i].ToString());

			string s = sb.ToString();

			#if UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
			UnityEngine.Debug.Log(s);
			#else
			Console.WriteLine(s);
			#endif

			return s;
		}

		public static string Warning(params object[] data)
		{
			StringBuilder sb = new StringBuilder();

			#if UNITY_EDITOR
			sb.Append("Warning: ".Bold().Colored(Colors.yellow));
			#else
			sb.Append("Warning: ");
			#endif

			for (int i = 0; i < data.Length; i++)
				sb.Append(data[i].ToString());

			string s = sb.ToString();

			#if UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
			UnityEngine.Debug.LogWarning(s);
			#else
			Console.WriteLine(s);
			#endif

			return s;
		}

		public static string Error(params object[] data)
		{
			StringBuilder sb = new StringBuilder();

			#if UNITY_EDITOR
			sb.Append("Error: ".Bold().Colored(Colors.red));
			#else
			sb.Append("Error: ");
			#endif

			for (int i = 0; i < data.Length; i++)
				sb.Append(data[i].ToString());

			string s = sb.ToString();

			#if UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
			UnityEngine.Debug.LogError(s);
			#else
			Console.WriteLine(s);
			#endif

			return s;
		}
	}
}

