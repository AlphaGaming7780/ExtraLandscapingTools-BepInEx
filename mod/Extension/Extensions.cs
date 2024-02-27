using System.Collections.Generic;

namespace ExtraLandscapingTools;

public class Extensions {

	public enum ExtensionType : int
	{
		Other = 0,
		Surfaces = 1,
		Decals = 2,
		Assets = 3
	}

	public static List<Extension> ExtensionList {private set; get;} = [];
	public static void RegisterELTExtension( Extension extension ) {
		if(ExtensionList.Contains(extension)) return;
		ExtensionList.Add(extension);
		extension.OnCreate();
	}
}