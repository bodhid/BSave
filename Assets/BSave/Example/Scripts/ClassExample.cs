using UnityEngine;
public class ClassExample : MonoBehaviour
{
	public string path;
	public string extension;
	public bool absolutePath;
	public BSave.Format format;
	public int split;
	public string key;

	public SaveData saveData;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.S))
		{
			BSave.Save(saveData, path, extension, key,absolutePath,split,format);
		}
		if (Input.GetKeyDown(KeyCode.L))
		{
			bool splitted = (split != 1);
			saveData = BSave.Load<SaveData>(path, extension, key, absolutePath, splitted,format);
		}
	}
}
