using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace EditorScene
{
	[CreateAssetMenu(fileName = "MarkerNameInputValidator", menuName = "TextMeshPro/Input Validators/Marker Name Input Validator", order = 100)]
	public class MarkerNameInputValidator : TMP_InputValidator
	{
		private readonly Regex _rx = new(@"^[\wа-яА-Я ]+$", RegexOptions.Singleline);

		public override char Validate(ref string text, ref int pos, char ch)
		{
			var newText = text + ch;
			if (_rx.Match(newText).Success)
			{
				text = newText;
				pos += 1;
				return ch;
			}

			return '\0';
		}
	}
}