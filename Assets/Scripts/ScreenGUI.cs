using UnityEngine;
using System.Collections;

/** Base class for simplified GUI creation */
public class ScreenGUI : MonoBehaviour {
	public Font font;
	public int fontSize;

	protected bool button(float left, float top, float width, float height, 
	                      string text) {
		GUI.skin.button.font = font;
		GUI.skin.button.fontSize = fontSize;
		return GUI.Button(percent(left, top, width, height), text);
	}
	protected bool repeatButton(float left, float top, float width, float height, 
	                            string text) {
		GUI.skin.button.font = font;
		GUI.skin.button.fontSize = fontSize;
		return GUI.RepeatButton(percent(left, top, width, height), text);
	}
	protected bool toggle(float left, float top, float width, float height, 
	                      bool value, string text) {
		return GUI.Toggle(percent(left, top, width, height), value, text);
	}
	protected float scroll(float left, float top, float width, float height, 
	                       float value, float min, float max) {
		return GUI.HorizontalScrollbar(percent(left, top, width, height), 
		                               value, (max - min) / 15, min, max);
	}
	Rect percent(float left, float top, float width, float height) {
		float xd = Screen.width / 100f, yd = Screen.height / 100f;
		if (left < 0) left += (100 - width);
		if (top < 0) top += (100 - height);
		return new Rect(left * xd, top * yd, width * xd, height * yd);
	}
}
