using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TradeSystem.FileContextCore.Infrastructure.Internal;

// ReSharper disable once CheckNamespace
namespace TradeSystem.FileContextCore.Extensions
{
	public static class FileContextDbContextOptionsExtensions
	{
		public static DbContextOptionsBuilder UseFileContext(
			[NotNull] this DbContextOptionsBuilder optionsBuilder,
			string databasename,
			string baseDirectory)
		{
			Check.NotNull(optionsBuilder, nameof(optionsBuilder));

			FileContextOptionsExtension extension = optionsBuilder.Options.FindExtension<FileContextOptionsExtension>()
				?? new FileContextOptionsExtension();

			extension = extension.WithSerializerAndFileManager(databasename, baseDirectory);

			((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

			return optionsBuilder;
		}
	}
}
