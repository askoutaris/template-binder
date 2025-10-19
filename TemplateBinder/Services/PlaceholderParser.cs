using System.Collections.Specialized;

namespace TemplateBinder.Services
{
	public interface IPlaceholderParser
	{
		PlaceholderParameters Parse(string placeholder);
	}

	public class PlaceholderParser : IPlaceholderParser
	{
		public PlaceholderParameters Parse(string placeholder)
		{
			try
			{
				var parts = placeholder
					.Trim()
					.Replace("{{", "")
					.Replace("}}", "")
					.Split('|')
					.Select(p => p.Trim())
					.ToArray();

				if (parts.Length == 0 || string.IsNullOrWhiteSpace(parts[0]))
					throw new InvalidOperationException($"Invalid placeholder {placeholder}");

				var parameterName = parts[0];

				var pipeParameters = parts.Length > 1
					? GetPipeParameters(parts[1])
					: null;

				return new PlaceholderParameters(parameterName, pipeParameters);
			}
			catch (Exception ex) when (ex is not InvalidOperationException)
			{
				throw new InvalidOperationException($"Invalid placeholder {placeholder}", ex);
			}
		}

		private PipeParameters? GetPipeParameters(string pipeContext)
		{
			var parts = pipeContext
				.Split(':')
				.ToArray();

			var pipeName = parts[0];

			var parameters = new NameValueCollection();

			if (parts.Length > 1)
			{
				var parameterPairs = parts[1]
					.Split(",")
					.SelectMany(x => x.Split('='))
					.ToArray();

				for (var i = 0; i < parameterPairs.Length; i += 2)
				{
					var parameterName = parameterPairs[i];
					var parameterValue = parameterPairs[i + 1];
					parameters.Add(parameterName, parameterValue);
				}
			}

			return new PipeParameters(pipeName, parameters);
		}
	}
}
