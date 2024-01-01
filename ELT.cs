using System.Reflection;

namespace ExtraLandscapingTools 
{
    public class ExtraLandscapingTools
	{
		internal static System.IO.Stream GetEmbedded(string embeddedPath) {
			return Assembly.GetExecutingAssembly().GetManifestResourceStream("ExtraLandscapingTools.embedded."+embeddedPath);
        }
    }
}