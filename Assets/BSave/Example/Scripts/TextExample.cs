using UnityEngine;

public class TextExample : MonoBehaviour {
	public string text=" :) ";
	public string path="SavedText";
	public string extension="dat";
	public bool absolutePath;
	public int split=1;
	public string key;
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.S))
		{
			BSave.Save(text, path, extension, key, absolutePath, split);
		}
		if (Input.GetKeyDown(KeyCode.L))
		{
			bool splitted = (split != 1);
			text = BSave.Load(path, extension, key, absolutePath, splitted);
		}
	}
}
