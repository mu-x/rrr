using UnityEngine;
using System.Collections;

public class PushButton : MonoBehaviour
{
    public bool push;

    public void Push(bool on)
    {
        push = on;
        var rect = GetComponent<RectTransform>();
        if (on) rect.localScale *= 0.9f; else rect.localScale /= 0.9f;
	}
}
