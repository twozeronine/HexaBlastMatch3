using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Logic
{
    public static class LogicUtil
    {
        public static Vector2 WorldPosToUiPos(Vector2 pos)
        {
            if (Camera.main is null) return Vector2.negativeInfinity;
            var canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            var canvasRect = canvas.GetComponent<RectTransform>();
            Vector2 viewportPosition = Camera.main.WorldToViewportPoint(pos);
            Vector2 screenPos = Camera.main.WorldToScreenPoint(pos);

            var sizeDelta = canvasRect.sizeDelta;
            var canvasWidth = sizeDelta.x;
            var canvasPos = new Vector2(
                viewportPosition.x * canvasWidth - canvasWidth * 0.5f,
                viewportPosition.y * sizeDelta.y - sizeDelta.y * 0.5f);

            return canvasPos;
        }
        public static Vector2 WorldToScreenPos(Vector2 world)
        {
            if (Camera.main is null) return Vector2.negativeInfinity;
            return Camera.main.WorldToScreenPoint(world);
        }

        public static Vector2 ScreenPosToWorldPos(Vector2 unityPos)
        {
            if (Camera.main is null) return Vector2.negativeInfinity;
            return (Vector2)Camera.main.ScreenToWorldPoint((Vector3)unityPos);
        }

        public static Vector2 ScreenPosToWorldPos(Vector2 unityPos, float height)
        {
            return Camera.main is null ? Vector2.negativeInfinity : RayToWorldPos(Camera.main.ScreenPointToRay(unityPos), height);
        }

        public static Vector2 RayToWorldPos(Ray ray)
        {
            return RayToWorldPos(ray, 0f);
        }

        private static Vector2 RayToWorldPos(Ray ray, float height)
        {
            return ray.origin - (ray.origin.y - height) / ray.direction.y * ray.direction;
        }
        
        public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
        {
            T component = go.GetComponent<T>();
            if (component == null)
                component = go.AddComponent<T>();

            return component;
        }

        public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
        {
            var transform = FindChild<Transform>(go, name, recursive);
            if (transform == null)
                return null;

            return transform.gameObject;
        }

        public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : Object
        {
            if (go == null) return null;

            // 재귀적으로 찾아야되나 즉, 부모의 자식의 자식으로 계속 찾아나갈 것 인지
            if (recursive == false)
                for (var i = 0; i < go.transform.childCount; i++)
                {
                    var transform = go.transform.GetChild(i);
                    if (string.IsNullOrEmpty(name) || transform.name == name)
                    {
                        var component = transform.GetComponent<T>();
                        if (component != null)
                            return component;
                    }
                }
            else
                foreach (var component in go.GetComponentsInChildren<T>())
                    if (string.IsNullOrEmpty(name) || component.name == name)
                        return component;

            return null;
        }
    }
}

