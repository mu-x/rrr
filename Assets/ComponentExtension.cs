using UnityEngine;
using System;
using System.Collections.Generic;

namespace UnityExtensions
{
    public static class Extension
    {
        public static void BindParent(this Transform self, Transform parent)
        {
            self.parent = parent;
            self.localPosition = self.localEulerAngles = new Vector3();
        }

        public static Transform Child(this Transform self, string name)
        {
            var c = self.FindChild(name);
            if (c == null) 
                throw new ArgumentException(string.Format(
                    "{0} has not child with name: {1}", self, name));
            return c;
        }

        public static Transform[] Children(this Transform self, string prefix) 
        {
            var found = new List<Transform>();
            foreach (var c in self.GetComponentsInChildren<Transform>())
                if (c.name.StartsWith(prefix))
                    found.Add(c);

            if (found.Count == 0)
                throw new ArgumentException(string.Format(
                    "{0} has not children with prefix: {1}", self, prefix));
            return found.ToArray();
        }

        public static GameObject MakeChild(this GameObject self, GameObject prefab,
                                           Transform position = null)
        {
            position = position ?? self.transform;
            var go = UnityEngine.Object.Instantiate(prefab,
                position.position, position.rotation) as GameObject;
            go.transform.parent = self.transform;
            go.name = go.name.Replace("(Clone)", "");
            return go;
        }

        public static GameObject MakeChild(this GameObject self, string name,
                                           Transform position = null) 
        {
            position = position ?? self.transform;
            var go = new GameObject(name);
            go.transform.parent = self.transform;
            go.transform.position = position.position;
            go.transform.rotation = position.rotation;
            return go;
        }

        public static T HasComponent<T>(this Component self) where T : Component
        {
            var c = self.GetComponents<T>();
            switch (c.Length)
            {
                case 0: return self.gameObject.AddComponent<T>();
                default: return c[0];
            }
        }

        public static void checkField<T>(
            this MonoBehaviour self, string field, T value) 
        {
            if (value == null) 
                throw new System.MissingFieldException(string.Format(
                    "Field '{0}' is null in {1}", field, self));
        }

        public static void checkField<T>(
            this MonoBehaviour self, string field, T[] value)  
        {
            if (value == null || value.Length == 0) 
                throw new System.MissingFieldException(string.Format(
                    "Field '{0}' is null or empty in {1}", field, self));
        }
    }
}
