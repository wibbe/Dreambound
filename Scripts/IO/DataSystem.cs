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
using System.Runtime.Serialization.Formatters.Binary;

#if UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
using UnityEngine;
#endif

namespace Dreambound.IO
{
	public static class DataSystem
	{
		public static bool Save(Data data, string name)
		{
			BinaryFormatter serializer = new BinaryFormatter();

			using (FileStream stream = new FileStream(GetSavePath(name), FileMode.Create))
			{
				try 
				{
					serializer.Serialize(stream, data);
				}
				catch (Exception exception)
				{
					Log.Warning("Could not save '", GetSavePath(name), "' - ", exception.ToString());
					return false;
				}
			}

			return true;
		}

		public static Data Load(string name)
		{
			if (!HasData(name))
				return null;

			BinaryFormatter serializer = new BinaryFormatter();
			using (FileStream stream = new FileStream(GetSavePath(name), FileMode.Open))
			{
				try
				{
					return serializer.Deserialize(stream) as Data;
				}
				catch (Exception exception)
				{
					Log.Warning("Could not load '", GetSavePath(name), "' - ", exception.ToString());
					return null;
				}
			}
		}

		public static bool DeleteSave(string name)
		{
			try
			{
				File.Delete(GetSavePath(name));
			}
			catch (Exception exception)
			{
				Log.Warning("Could not delete '", GetSavePath(name), "' - ", exception.ToString());
				return false;
			}

			return true;
		}

		public static bool HasData(string name)
		{
			return File.Exists(GetSavePath(name));
		}	

		private static string GetSavePath(string name)
		{
			#if UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
			return Path.Combine(Application.persistentDataPath, name + ".data");
			#else
			return Path.Combine(Directory.GetCurrentDirectory(), name + ".data");
			#endif
		}
	}
}

