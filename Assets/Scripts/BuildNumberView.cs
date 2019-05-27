using UnityEngine;
using UnityEngine.UI;

public class BuildNumberView : MonoBehaviour {
	public Text Text;

	void Start() {
		Text.text = Application.version;
	}
}
