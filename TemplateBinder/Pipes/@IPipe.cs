using TemplateBinder.Parameters;

namespace TemplateBinder.Pipes
{
	/// <summary>
	/// Transforms a parameter value into formatted output.
	/// </summary>
	public interface IPipe
	{
		/// <summary>
		/// Transforms the input parameter into a new parameter with formatted value.
		/// </summary>
		/// <param name="parameter">The parameter to transform.</param>
		/// <returns>A new parameter, typically TextParameter, with the transformed value.</returns>
		IParameter Transform(IParameter parameter);
	}
}
