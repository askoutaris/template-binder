namespace TemplateBinder
{
	public static class Constants
	{
		public static class Pipes
		{
			public const string Text = "text";
			public const string Date = "date";
			public const string Decimal = "decimal";
			public const string BooleanText = "booleantext";
		}

		public static class Regex
		{
			public static readonly System.Text.RegularExpressions.Regex Placeholder = new System.Text.RegularExpressions.Regex(@"(\{)(.*?)(\})");
			public static readonly System.Text.RegularExpressions.Regex ParameterName = new System.Text.RegularExpressions.Regex(@"[^{][^|]*");
			public static readonly System.Text.RegularExpressions.Regex PipeName = new System.Text.RegularExpressions.Regex(@"(?<=\|)(.*?)(?=\:)");
			public static readonly System.Text.RegularExpressions.Regex PipeParameters = new System.Text.RegularExpressions.Regex(@"(?<=\:)(.*)[^}]");
		}
	}
}
