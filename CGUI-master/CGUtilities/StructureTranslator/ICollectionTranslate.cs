using System.Collections.Generic;

namespace CGUtilities.StructureTranslator
{
	public interface ICollectionTranslate
	{
		string Encode<T>(List<T> elements);
		List<object> Decode(string content);
	}
}
