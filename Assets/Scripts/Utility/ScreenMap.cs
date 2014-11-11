using System.Collections;
using System;
using UnityEngine;

/** Method set to map screen for GUI */
namespace ScreenMap {
    public static class ScreenExts {
        /** Selects full @class Screen os @class Rect */
        public static Rect ScreenRect(this MonoBehaviour mb) {
            return new Rect(0, 0, Screen.width, Screen.height);
        }

        /** Selects @param part of @param total by X on @param r */
        public static Rect X(this Rect r, float part, float total,
                             float slots = 1, float border = 0.03f) {
            part += (part < 0) ? total : -1;
            float step = r.width / total, offset = r.width * border;
            return new Rect(r.x + part * step + offset, r.y,
                            step * slots - 2 * offset, r.height);
        }

        /** Selects @param part of @param total by Y on @param r */
        public static Rect Y(this Rect r, float part, float total,
                             float slots = 1, float border = 0.03f) {
            part += (part < 0) ? total : -1;
            float step = r.height / total, offset = r.height * border;
            return new Rect(r.x, r.y + part * step + offset,
                            r.width, step * slots - 2 * offset);
        }
    }
}