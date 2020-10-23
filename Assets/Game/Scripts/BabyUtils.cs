using System.Collections;
using System.Linq;
using UnityEngine;

public static class BabyUtils {

	/// Vectors & angles

	public static float VectorAngle(Vector2 a_in, Vector2 b_in) {
		Vector2 a = new Vector2(a_in.x - b_in.x, a_in.y - b_in.y);
		if (a.x < 0) {
			return 360 - (Mathf.Atan2(a.x, a.y) * Mathf.Rad2Deg * -1);
		} else {
			return Mathf.Atan2(a.x, a.y) * Mathf.Rad2Deg;
		}
	}

	public static Vector2 RadianToVector2(float radian) {
		return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
	}

	public static Vector2 DegreeToVector2(float degree) {
		return RadianToVector2(degree * Mathf.Deg2Rad);
	}

	public static float InvertAngle(float angle) {
		angle = 360 - angle;
		return angle;
	}

	/// Arrays

	public static T[] arrayRandomize<T>(T[] array) {
		if (array == null)return null;
		System.Random rnd = new System.Random();
		return array.OrderBy(x => rnd.Next()).ToArray();
	}

	public static T[] arrayCopy<T>(T[] array) {
		if (array == null)return null;
		T[] copy = new T[array.Length];
		for (int i = 0; i < array.Length; i++) {
			copy[i] = array[i];
		}
		return copy;
	}

	public static T arrayRandom<T>(T[] array) {
		if (array == null)return default(T);
		return array[Random.Range(0, array.Length)];
	}

	/// Colors

	public static Color colorByHex(int hex, float alpha = 1.0f) {
		string colorcode = hex.ToString("X2");
		float r = int.Parse(colorcode.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
		float g = int.Parse(colorcode.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
		float b = int.Parse(colorcode.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255f;

		return new Color(r, g, b, alpha);
	}

	/// Camera 

	public static Vector2 sizeToFit(Texture2D texture, float pixelsPerUnit) {
		return sizeToFit(new Vector2(texture.width / pixelsPerUnit, texture.height / pixelsPerUnit));
	}

	public static Vector3 sizeToFit(SpriteRenderer spriteRenderer) {
		return sizeToFit(spriteRenderer.sprite.bounds.size);
	}

	public static Vector3 sizeToFit(Vector2 spriteSize) {
		float cameraHeight = Camera.main.orthographicSize * 2;
		Vector2 cameraSize = new Vector2(Camera.main.aspect * cameraHeight, cameraHeight);

		Vector3 scale = Vector3.one;
		float screenAspect = cameraSize.x / cameraSize.y;
		float rectAspect = spriteSize.x / spriteSize.y;

		if (screenAspect < rectAspect)
			scale *= cameraSize.y / spriteSize.y;
		else
			scale *= cameraSize.x / spriteSize.x;

		return scale;
	}

	public static Rect screenBoundsPixels { get { return new Rect(0, 0, Camera.main.pixelWidth, Camera.main.pixelHeight); } }
	public static Rect screenBoundsUnits {
		get {
			Vector3 min = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
			Vector3 max = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
			return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
		}
	}

	public static float pixelsPerUnit { get { return screenBoundsPixels.width / screenBoundsUnits.width; } }

	/// Coroutine
	public static void ExecuteAfterTime(MonoBehaviour component, float time, System.Action callback) {
		component.StartCoroutine(DelayedAction(time, callback));
	}

	public static IEnumerator DelayedAction(float time, System.Action callback) {
		yield return new WaitForSeconds(time);
		callback();
	}

	/// Extensions
	public static bool intersects(this Rect r1, Rect r2, out Rect area) {
		area = new Rect();

		if (r2.Overlaps(r1)) {
			float x1 = Mathf.Min(r1.xMax, r2.xMax);
			float x2 = Mathf.Max(r1.xMin, r2.xMin);
			float y1 = Mathf.Min(r1.yMax, r2.yMax);
			float y2 = Mathf.Max(r1.yMin, r2.yMin);
			area.x = Mathf.Min(x1, x2);
			area.y = Mathf.Min(y1, y2);
			area.width = Mathf.Max(0.0f, x1 - x2);
			area.height = Mathf.Max(0.0f, y1 - y2);

			return true;
		}

		return false;
	}

	/// Misc
	public static bool randomBool() {
		return Random.Range(0f, 1f) < 0.5f;
	}

	public static bool chance(float rate) {
		return Random.Range(0f, 1f) < rate;
	}
}