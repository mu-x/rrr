using System.Collections;
using System;
using UnityEngine;

/** Method set to map screen for GUI */
namespace ScreenMap {
    public static class Extension {
        public static Rect ScreenRect(this MonoBehaviour mb) {
            return new Rect(0, 0, Screen.width, Screen.height);
        }

        /** Selects @param part of @param total by X */
        public static Rect X(this Rect r, float part, float total,
                             float slots = 1, float border = 0.03f) {
            part += (part < 0) ? total : -1;
            float step = r.width / total, offset = r.width * border;
            return new Rect(r.x + part * step + offset, r.y,
                            step * slots - 2 * offset, r.height);
        }

        /** Selects @param part of @param total by Y */
        public static Rect Y(this Rect r, float part, float total,
                             float slots = 1, float border = 0.03f) {
            part += (part < 0) ? total : -1;
            float step = r.height / total, offset = r.height * border;
            return new Rect(r.x, r.y + part * step + offset,
                            r.width, step * slots - 2 * offset);
        }
    }
}

/** Interface to change any changable value in game */
public interface ISelector {
    void Next();
    void Previous();
}

/** Generic @interface ISelector for any kind of objects in game */
public class Selector<T> : ISelector {
    public delegate void Changed(T item);
    public delegate void Round();

    /** Iterates over @param items (default @param selected) and invokes
     *  @rapam changed. Also invokes @param round when hits 1st element */
    public Selector(T[] items, string label = null,
                    Changed changed = null, Round round = null) {
        this.items = items;
        this.changed = changed;
        this.round = round;
		this.label = label;
		this.selected = (label != null) ? PlayerPrefs.GetInt(label, 0) : 0;

        if (changed != null)
            changed(items[this.selected]);
    }

    public void Next() {
        if (++selected == items.Length) {
            selected = 0;
            if(round != null) round();
        }

		if (label != null)
			PlayerPrefs.SetInt(label, selected);

        if (changed != null)
            changed(items[selected]);
    }

    public void Previous() {
        if (--selected < 0) {
            selected += items.Length;
            if(round != null) round();
		}

		if (label != null)
			PlayerPrefs.SetInt(label, selected);

        if (changed != null)
            changed(items[selected]);
    }

    T[] items;
    Changed changed;
    Round round;
	string label;
    int selected = 0;
}
