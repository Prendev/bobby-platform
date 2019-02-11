using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TradeSystem.FileContextCore.FileManager
{
    class DefaultFileManager : IFileManager
    {
        private readonly object _thisLock = new object();

	    private readonly IEntityType _type;
		private readonly string _databasename;
		private readonly string _baseDirectory;

        public DefaultFileManager(IEntityType type, string databasename, string baseDirectory)
        {
            _type = type;
			_databasename = databasename;
	        _baseDirectory = baseDirectory;
        }

        public string GetFileName()
        {
            var name = Path.GetInvalidFileNameChars().Aggregate(_type.Name, (current, c) => current.Replace(c, '_'));
	        var path = Path.Combine(_baseDirectory, "FileContext", _databasename);
            Directory.CreateDirectory(path);
            return Path.Combine(path, name + ".json");
        }

        public string LoadContent()
        {
            lock (_thisLock)
            {
                var path = GetFileName();
                return File.Exists(path) ? File.ReadAllText(path) : "";
            }
        }

        public void SaveContent(string content)
        {
            lock (_thisLock)
            {
                var path = GetFileName();
                File.WriteAllText(path, content);
            }
        }

        public bool Clear()
        {
            lock (_thisLock)
            {
                var fi = new FileInfo(GetFileName());
	            if (!fi.Exists) return false;
	            fi.Delete();
	            return true;
            }
        }

        public bool FileExists()
        {
            lock (_thisLock)
            {
                var fi = new FileInfo(GetFileName());
                return fi.Exists;
            }
        }
    }
}
