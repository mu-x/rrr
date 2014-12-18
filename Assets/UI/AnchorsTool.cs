#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class AnchorsTool : MonoBehaviour 
{
    [MenuItem("Anchors Tool/Anchors to Corners %[")]
	static void AnchorsToCorners()
	{
		var t = Selection.activeTransform as RectTransform;
		var pt = Selection.activeTransform.parent as RectTransform;
		if(t == null || pt == null) return;
		
		t.anchorMin = new Vector2(
			t.anchorMin.x + t.offsetMin.x / pt.rect.width,
			t.anchorMin.y + t.offsetMin.y / pt.rect.height);
		t.anchorMax = new Vector2(
			t.anchorMax.x + t.offsetMax.x / pt.rect.width,
			t.anchorMax.y + t.offsetMax.y / pt.rect.height);
		t.offsetMin = t.offsetMax = new Vector2(0, 0);
	}
}

#endif // UNITY_EDITOR