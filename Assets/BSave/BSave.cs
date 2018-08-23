/*
Copyright 2018 Bodhi Donselaar

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
public class BSave
{
	public enum Format
	{
		JSON,
		JSON_PrettyPrint,
		Binary
	}
	public static bool Save(
		object data,
		string path,
		string extension,
		string key = null,
		bool absolutePath = false,
		int split = 1,
		Format format = Format.JSON
		)
	{
		try
		{
			if (split < 1 || split > 100)
			{
				throw new System.Exception("Invalid split value ( " + split + " ). Between 1 and 100 is allowed");
			}
			FileInfo fileInfo = new FileInfo((absolutePath ? "" : (Application.persistentDataPath + "/")) + path);
			byte[] bytes;
			switch (format)
			{
				case Format.JSON:
					bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data, false));
					break;
				case Format.JSON_PrettyPrint:
					bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data, true));
					break;
				default:
					BinaryFormatter formatter = new BinaryFormatter();
					using (MemoryStream stream = new MemoryStream())
					{
						formatter.Serialize(stream, data);
						bytes = stream.ToArray();
					}
					break;
			}

			if (!string.IsNullOrEmpty(key))
			{
				bytes = Encrypt(bytes, key);
			}

			Write(bytes, fileInfo, extension, split);
			return true;
		}
		catch (System.Exception e)
		{
			Debug.LogError(e.Message + " - " + e.StackTrace);
			return false;
		}
	}
	public static T Load<T>(
		string path,
		string extension,
		string key = null,
		bool absolutePath = false,
		bool splitted = false,
		Format format = Format.JSON
		)
	{
		try
		{
			FileInfo fileInfo = new FileInfo((absolutePath ? "" : Application.persistentDataPath) + "/" + path);
			byte[] bytes = Read(fileInfo, extension, splitted);
			if (!string.IsNullOrEmpty(key))
			{
				bytes = Decrypt(bytes, key);
			}
			switch (format)
			{
				case Format.JSON:
				case Format.JSON_PrettyPrint:
					return JsonUtility.FromJson<T>(Encoding.UTF8.GetString(bytes));
				default:
					BinaryFormatter formatter = new BinaryFormatter();
					using (MemoryStream stream = new MemoryStream(bytes))
					{
						return (T)formatter.Deserialize(stream);
					}
			}
		}
		catch (System.Exception e)
		{
			Debug.LogError(e.Message + " - " + e.StackTrace);
			return default(T);
		}
	}
	public static bool Save(
		string text,
		string path,
		string extension,
		string key = null,
		bool absolutePath = false,
		int split = 1)
	{
		try
		{
			if (split < 1 || split > 100)
			{
				throw new System.Exception("Invalid split value ( " + split + " ). Between 1 and 100 is allowed");
			}
			FileInfo fileInfo = new FileInfo((absolutePath ? "" : (Application.persistentDataPath + "/")) + path);
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			if (!string.IsNullOrEmpty(key))
			{
				bytes = Encrypt(bytes, key);
			}
			Write(bytes, fileInfo, extension, split);
			return true;
		}
		catch (System.Exception e)
		{
			Debug.LogError(e.Message + " - " + e.StackTrace);
			return false;
		}
	}
	public static string Load(
		string path,
		string extension,
		string key = null,
		bool absolutePath = false,
		bool splitted = false
		)
	{
		try
		{
			FileInfo fileInfo = new FileInfo((absolutePath ? "" : Application.persistentDataPath) + "/" + path);

			byte[] bytes = Read(fileInfo, extension, splitted);

			if (!string.IsNullOrEmpty(key))
			{
				bytes = Decrypt(bytes, key);
			}

			return Encoding.UTF8.GetString(bytes);
		}
		catch (System.Exception e)
		{
			Debug.LogError(e.Message + " - " + e.StackTrace);
			return null;
		}
	}
	private static byte[] Encrypt(byte[] bytes, string key)
	{
		for (int i = 0; i < bytes.Length; ++i)
		{
			bytes[i] = (byte)((bytes[i] + key[i % key.Length]) % 256);
		}
		return bytes;
	}
	private static byte[] Decrypt(byte[] bytes, string key)
	{
		for (int i = 0; i < bytes.Length; ++i)
		{
			int v = bytes[i] - key[i % key.Length];
			if (v < 0) v += 256;
			bytes[i] = (byte)v;
		}
		return bytes;
	}

	private static void Write(byte[] bytes, FileInfo fileInfo, string extension, int split)
	{
		if (split == 1)
		{
			string filename = fileInfo.FullName + "." + extension;
			File.WriteAllBytes(filename, bytes);
			Debug.Log("Saved data to: " + filename);
		}
		else
		{
			int splitSize = Mathf.CeilToInt(((float)bytes.Length) / split);
			List<byte> list = new List<byte>(bytes);
			int index = 0;
			while (list.Count > 0)
			{
				int chunkSize = Mathf.Min(splitSize, list.Count);
				string filename = fileInfo.FullName + "_" + index.ToString() + "." + extension;
				File.WriteAllBytes(filename, list.GetRange(0, chunkSize).ToArray());

				list.RemoveRange(0, chunkSize);
				index++;
				if (index > 128) break; //128 should not be possible, just in case
			}
			while (index < split)
			{
				string filename = fileInfo.FullName + "_" + index.ToString() + "." + extension;
				File.WriteAllBytes(filename, new byte[0]);
				index++;
				if (index > 128) break; //128 should not be possible, just in case
			}
			Debug.Log("Saved data to: " + fileInfo.FullName + "_(0-" + (index - 1).ToString() + ")." + extension);
		}
	}
	private static byte[] Read(FileInfo fileInfo, string extension, bool splitted)
	{
		if (!splitted)
		{
			return File.ReadAllBytes(fileInfo.FullName + "." + extension);
		}
		else
		{
			List<byte> list = new List<byte>();
			int index = 0;
			while (index < 128) //128 should not be possible, just in case
			{
				string filename = fileInfo.FullName + "_" + index.ToString() + "." + extension;
				if (!File.Exists(filename)) break;
				list.AddRange(File.ReadAllBytes(filename));
				index++;
			}
			return list.ToArray();
		}
	}
}