using System.Collections;
using System;
using UnityEngine;

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
                    Action<T> changed = null, Action round = null) {
        this.items = items;
        if (items == null || items.Length == 0)
            throw new ArgumentNullException("items");

        this.label = label;
        this.selected = (label != null) ? PlayerPrefs.GetInt(label, 0) : 0;

        this.changed = changed ?? delegate(T t) { };
        this.changed(items[this.selected]);
        this.round = round ?? delegate { };
    }

    public void Next() {
        if (++selected == items.Length) {
            selected = 0;
            round();
        }

        change();
    }

    public void Previous() {
        if (--selected < 0) {
            selected = items.Length - 1; // last
            round();
        }

        change();
    }

    void change() {
        changed(items[selected]);
        if (label != null)
            PlayerPrefs.SetInt(label, selected);
    }

    T[] items;
    Action<T> changed;
    Action round;
    string label;
    int selected;
}
