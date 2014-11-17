using System.Collections;
using System.Linq;
using System;
using UnityEngine;

/** GUI halper */
class ExtraGUI
{
    public Color textColor;
    public Texture2D bgTexture;

    public ExtraGUI(Color color)
    {
        textColor = color;
    }

    /** Calculates @class Rect on @class @Screen from given percents */
    public Rect Select(float x, float y, float w, float h)
    {
        // Recalculate as percent of @class Screen
        x = (float)Screen.width * x / 100f;
        y = (float)Screen.height * y / 100f;
        w = (float)Screen.width * w / 100f;
        h = (float)Screen.height * h / 100f;

        // Move if @param x or @param y is negative
        if (x < 0) x += (float)Screen.width - w;
        if (y < 0) y += (float)Screen.height - h;

        return new Rect(x, y, w, h);
    }

    /** Setup font size to make text fit */
    public void SetupFont(GUIStyle style, Rect rect, string text, float k = 0.9f)
    {
        style.fontSize = 10;
        var size = style.CalcSize(new GUIContent(text));
        var scale = Mathf.Min(rect.width / size.x, rect.height / size.y) * k;

        style.fontSize = Mathf.RoundToInt(scale * 10f);
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = textColor;
    }

    public void Label(float x, float y, float w, float h, string text)
    {
        var rect = Select(x, y, w, h);
        SetupFont(GUI.skin.label, rect, text);
        GUI.Label(rect, text);
    }

    public void Box(float x, float y, float w, float h, string text)
    {
        var rect = Select(x, y, w, h);
        SetupFont(GUI.skin.label, rect, text);
        GUI.Box(rect, "");
        GUI.Label(rect, text);
    }

    public bool Button(float x, float y, float w, float h, string text)
    {
        var rect = Select(x, y, w, h);
        SetupFont(GUI.skin.button, rect, text);
        return GUI.Button(rect, text);
    }

    public bool Pusher(float x, float y, float w, float h, string text) {
        var rect = Select(x, y, w, h);
        SetupFont(GUI.skin.button, rect, text);
        return GUI.RepeatButton(rect, text);
    }

    public void Grid<TEnum>(float x, float y, float w, float h, ref TEnum value)
        where TEnum : struct, IConvertible, IComparable, IFormattable
    {
        var names = Enum.GetNames(typeof(TEnum));
        var values = Enum.GetValues(typeof(TEnum));

        var rect = Select(x, y, w, h);
        SetupFont(GUI.skin.button, rect, names.First(), 0.5f);

        int raw = Array.IndexOf(values, value);
        raw = GUI.SelectionGrid(rect, raw, names, names.Length);
        value = (TEnum)values.GetValue(raw);
    }

    public void Selection(float x, float y, float w, float h, string text,
                          ISelector selector)
    {
        var field = Select(x, y, w, h);
        var one = field.width / 10;

        var left = new Rect(field.x, field.y, one, field.height);
        var middle = new Rect(field.x + one, field.y, one * 8, field.height);
        var right = new Rect(field.x + one * 9, field.y, one, field.height);

        SetupFont(GUI.skin.label, middle, text);
        if (text != null)
        {
            GUI.Box(middle, "");
            GUI.Label(middle, text);
        }

        if (GUI.Button(left, "<")) selector.Previous();
        if (GUI.Button(right, ">")) selector.Next();
    }
}
