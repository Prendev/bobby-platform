using System.Collections.Generic;

namespace QvaDev.FileContextCore.ValueGeneration.Internal
{
    class FileContextIntegerValueGeneratorCache
    {
		public readonly Dictionary<string, Dictionary<string, long>> LastIds = new Dictionary<string, Dictionary<string, long>>();
	}
}
