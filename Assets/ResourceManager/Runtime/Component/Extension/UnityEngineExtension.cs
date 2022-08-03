using UnityEngine;
namespace Alva.Runtime.Extension
{
	public static class UnityEngineExtension
	{
		#region Transform
		public static void SetPositionX(this Transform transform, float x)
		{
			transform.position = new Vector3(x, transform.position.y, transform.position.z);
		}
		public static void SetPositionY(this Transform transform, float y)
		{
			transform.position = new Vector3(transform.position.x, y, transform.position.z);
		}
		public static void SetPositionZ(this Transform transform, float z)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, z);
		}
		public static void SetEulerAnglesX(this Transform transform, float x)
		{
			transform.eulerAngles = new Vector3(x, transform.eulerAngles.y, transform.eulerAngles.z);
		}
		public static void SetEulerAnglesY(this Transform transform, float y)
		{
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, y, transform.eulerAngles.z);
		}
		public static void SetEulerAnglesZ(this Transform transform, float z)
		{
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, z);
		}
		public static void SetLocalScaleX(this Transform transform, float x)
		{
			transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
		}
		public static void SetLocalScaleY(this Transform transform, float y)
		{
			transform.localScale = new Vector3(transform.localScale.x, y, transform.localScale.z);
		}
		public static void SetLocalScaleZ(this Transform transform, float z)
		{
			transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, z);
		}
		public static Transform FindInAllChild(this Transform transform, string name)
		{
			var list = transform.GetComponentsInChildren<Transform>();
			for (var i = 0; i < list.Length; i++)
			{
				var t = list[i];
				if (t.gameObject.name == name)
				{
					return t;
				}
			}
			return null;
		}
		public static void ClearChild(this Transform transform)
		{
			for (var i = 0; i < transform.childCount; i++)
			{
				var go = transform.GetChild(i).gameObject;
				Object.Destroy(go);
			}
		}
		public static T FindComponentInParents<T>(this Transform transform) where T : Component
		{
			var component = transform.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			return transform.parent != null ? FindComponentInParents<T>(transform.parent) : null;
		}
		public static T GetOrAddComponent<T>(this Transform transform) where T : Component
		{
			return transform.GetComponent<T>() ?? transform.gameObject.AddComponent<T>();
		}
		public static void ResetLocal(this Transform transform)
		{
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
		}
		#endregion
		#region RectTransform
		public static void SetSizeDeltaX(this RectTransform rectTransform, float x)
		{
			rectTransform.sizeDelta = new Vector2(x, rectTransform.sizeDelta.y);
		}
		public static void SetSizeDeltaY(this RectTransform rectTransform, float y)
		{
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, y);
		}
		public static void SetAnchoredPositionX(this RectTransform rectTransform, float x)
		{
			rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y);
		}
		public static void SetAnchoredPositionY(this RectTransform rectTransform, float y)
		{
			rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, y);
		}
		#endregion
	}
}
