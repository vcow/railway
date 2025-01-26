namespace Utils
{
	public static class StringUtils
	{
		public static int ToInt(this string value) => string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
		public static float ToFloat(this string value) => string.IsNullOrEmpty(value) ? 0f : float.Parse(value);
	}
}