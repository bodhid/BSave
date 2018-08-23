using UnityEngine;
using UnityEngine.UI;
public class ExampleText : MonoBehaviour {
	void Start () {
		GetComponent<Text>().text = "Files are stored at " + Application.persistentDataPath + ". Unless an absolute path is given";
	}
}
