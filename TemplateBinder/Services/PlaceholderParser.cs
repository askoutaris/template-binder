using System.Collections.Specialized;

namespace TemplateBinder.Services
{
	/// <summary>
	/// Parses placeholder strings to extract parameter name and pipe information.
	/// </summary>
	public interface IPlaceholderParser
	{
		/// <summary>
		/// Parses a placeholder string like {{Name|pipe:param=value}}.
		/// </summary>
		/// <param name="placeholder">The placeholder string including {{ and }}.</param>
		/// <returns>Parsed parameter name and optional pipe parameters.</returns>
		PlaceholderParameters Parse(string placeholder);
	}

	/// <summary>
	/// Default implementation that parses placeholders by splitting on | and : delimiters.
	/// </summary>
	public class PlaceholderParser : IPlaceholderParser
	{
		/// <summary>
		/// Parses a placeholder string into components.
		/// </summary>
		/// <param name="placeholder">Placeholder string like {{Name|pipe:param=value}}.</param>
		/// <returns>Parsed components with parameter name and optional pipe.</returns>
		/// <exception cref="InvalidOperationException">Thrown when placeholder format is invalid.</exception>
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
