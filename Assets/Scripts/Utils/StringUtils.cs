using System.Globalization;

namespace Utils
{
	public static class StringUtils
	{
		public static int ToInt(this string value) => string.IsNullOrEmpty(value)
			? 0
			: int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i) ? i : 0;
		public static float ToFloat(this string value) => string.IsNullOrEmpty(value)
			? 0f
			: float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var f) ? f : 0f;
	}
}