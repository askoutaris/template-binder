using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TemplateBinder.Factories;
using TemplateBinder.Parameters;
using TemplateBinder.Pipes;

namespace TemplateBinder
{
	public class Binder
	{
		private readonly IPipeFactory _pipeFactory;
		private readonly string _template;
		private Placeholder[] _placeholders;

		public Binder(IPipeFactory pipeFactory, string template)
		{
			_pipeFactory = pipeFactory;
			_template = template;
			_placeholders = Array.Empty<Placeholder>();

			ParseTemplate();
		}

		public string Bind(Parameter[] parameters)
		{
			var sb = new StringBuilder(_template);
			foreach (var placeholder in _placeholders)
			{
				var parameter = parameters.FirstOrDefault(x => x.Name == placeholder.ParameterName);
				var value = placeholder.Pipe.Transform(parameter);
				sb.Replace(placeholder.PlaceholderText, value.GetValue()?.ToString() ?? string.Empty);
			}

			return sb.ToString();
		}

		private void ParseTemplate()
		{
			var placeholders = new List<Placeholder>();
			var placeholderMatches = GetPlaceHolders();
			foreach (Match placeholderMatch in placeholderMatches)
			{
				var placeholderText = placeholderMatch.ToString();
				var parameterNameMatch = Constants.Regex.ParameterName.Match(placeholderText);
				if (!parameterNameMatch.Success)
					continue;

				var placeholder = GetPlaceholderContext(placeholderText, parameterNameMatch.ToString());
				placeholders.Add(placeholder);
			}

			_placeholders = placeholders.ToArray();
		}
		private Placeholder GetPlaceholderContext(string placeholderText, string parameterName)
		{
			var pipeName = GetPipeName(placeholderText);
			var pipeParameters = GetPipeParameters(placeholderText);
			var pipe = _pipeFactory.Create(pipeName, pipeParameters);
			return new Placeholder(placeholderText, parameterName, pipe);
		}
		private NameValueCollection GetPipeParameters(string placeholderText)
		{
			var parameters = new NameValueCollection();

			var pipeParametersMatch = Constants.Regex.PipeParameters.Match(placeholderText);
			if (pipeParametersMatch.Success)
			{
				var parameterPairs = pipeParametersMatch.ToString().Split(',');
				foreach (var pair in parameterPairs)
				{
					var parts = pair.Split('=');
					if (parts.Length != 2)
						throw new Exception($"Invalid pipe parameters {parameterPairs} in {placeholderText}");

					var name = parts[0];
					var value = parts[1];
					parameters.Add(name, value);
				}
			}

			return parameters;
		}
		private string GetPipeName(string placeholderText)
		{
			var pipeNameMatch = Constants.Regex.PipeName.Match(placeholderText);
			return pipeNameMatch.Success ? pipeNameMatch.ToString() : Constants.Pipes.Text;
		}
		private MatchCollection GetPlaceHolders()
		{
			return Constants.Regex.Placeholder.Matches(_template);
		}
	}

	class Placeholder
	{
		public string PlaceholderText { get; }
		public string ParameterName { get; }
		public IPipe Pipe { get; }

		public Placeholder(string placeholderText, string parameterName, IPipe pipe)
		{
			PlaceholderText = placeholderText;
			ParameterName = parameterName;
			Pipe = pipe;
		}
	}
}
