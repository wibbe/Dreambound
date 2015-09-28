using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#if UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
using UnityEngine;
#define HAS_UNITY
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
			#if HAS_UNITY
			return Path.Combine(Application.persistentDataPath, name + ".data");
			#else
			return Path.Combine(Directory.GetCurrentDirectory(), name + ".data");
			#endif
		}
	}
}

