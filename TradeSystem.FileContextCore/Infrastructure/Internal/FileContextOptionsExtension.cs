using System.Text;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using TradeSystem.FileContextCore.Extensions;

namespace TradeSystem.FileContextCore.Infrastructure.Internal
{
	internal class FileContextOptionsExtension : IDbContextOptionsExtension
    {
		private string _databasename = "";
		private string _baseDirectory;
		private string _logFragment;

        protected virtual FileContextOptionsExtension Clone() => new FileContextOptionsExtension();

		public virtual string DatabaseName => _databasename;

		public virtual string BaseDirectory => _baseDirectory;

		public virtual FileContextOptionsExtension WithSerializerAndFileManager(string databasename, string baseDirectory)
        {
            FileContextOptionsExtension clone = Clone();

			clone._databasename = databasename;
			clone._baseDirectory = baseDirectory;

			return clone;
        }

        public virtual bool ApplyServices(IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));

            services.AddEntityFrameworkFileContext();

            return true;
        }

        public virtual long GetServiceProviderHashCode() => DatabaseName.GetHashCode();

        public virtual void Validate(IDbContextOptions options)
        {
        }

        public virtual string LogFragment
        {
            get
            {
                if (_logFragment == null)
                {
                    StringBuilder builder = new StringBuilder();

                    builder.Append("databasename=").Append(_databasename)
	                    .Append(";baseDirectory=").Append(_baseDirectory)
						.Append(' ');

                    _logFragment = builder.ToString();
                }

                return _logFragment;
            }
        }
    }
}
