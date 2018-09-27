// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Update;
using QvaDev.FileContextCore.FileManager;
using QvaDev.FileContextCore.Infrastructure.Internal;
using QvaDev.FileContextCore.Serializer;
using QvaDev.FileContextCore.ValueGeneration.Internal;

namespace QvaDev.FileContextCore.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    class FileContextTable<TKey> : IFileContextTable
    {
		private readonly IPrincipalKeyValueFactory<TKey> _keyValueFactory;
        private readonly Dictionary<TKey, object[]> _rows;
		private readonly FileContextIntegerValueGeneratorCache _idCache;
		private readonly FileContextOptionsExtension _options;
		private readonly IEntityType _entityType;
		private readonly IReadOnlyList<IProperty> _primaryKey;
		
		private IFileManager _fileManager;
        private ISerializer _serializer;
        private string _filetype;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public FileContextTable(
	        [NotNull] IPrincipalKeyValueFactory<TKey> keyValueFactory,
	        IEntityType entityType,
	        FileContextIntegerValueGeneratorCache idCache,
	        FileContextOptionsExtension options)
        {
			_idCache = idCache;
            _entityType = entityType;
			_options = options;
            _keyValueFactory = keyValueFactory;
	        _primaryKey = _entityType.FindPrimaryKey().Properties;
			_rows = Init();
		}

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual IReadOnlyList<object[]> SnapshotRows()
            => _rows.Values.ToList();

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void Create(IUpdateEntry entry)
        {
            _rows.Add(CreateKey(entry), CreateValueBuffer(entry));
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void Delete(IUpdateEntry entry)
        {
            var key = CreateKey(entry);

            if (_rows.ContainsKey(key))
            {
                _rows.Remove(key);
            }
            else
            {
                throw new DbUpdateConcurrencyException(FileContextStrings.UpdateConcurrencyException, new[] { entry });
            }
        }

        public void Save()
        {
            _updateMethod(_rows);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void Update(IUpdateEntry entry)
        {
            var key = CreateKey(entry);

            if (_rows.ContainsKey(key))
            {
                var properties = entry.EntityType.GetProperties().ToList();
                var valueBuffer = new object[properties.Count];

                for (var index = 0; index < valueBuffer.Length; index++)
                {
                    valueBuffer[index] = entry.IsModified(properties[index])
                        ? entry.GetCurrentValue(properties[index])
                        : _rows[key][index];
                }

                _rows[key] = valueBuffer;
            }
            else
            {
				throw new DbUpdateConcurrencyException(FileContextStrings.UpdateConcurrencyException, new[] { entry });
            }
        }

        private TKey CreateKey(IUpdateEntry entry)
            => _keyValueFactory.CreateFromCurrentValues((InternalEntityEntry)entry);

        private static object[] CreateValueBuffer(IUpdateEntry entry)
            => entry.EntityType.GetProperties().Select(entry.GetCurrentValue).ToArray();

        private Action<Dictionary<TKey, object[]>> _updateMethod;

        private Dictionary<int, string> GetAutoGeneratedFields()
        {
            var props = _entityType.GetProperties().ToArray();
            var autoGeneratedFields = new Dictionary<int, string>();

            for (var i = 0; i < props.Length; i++)
            {
	            if (new[] {ValueGenerated.OnAdd, ValueGenerated.OnAddOrUpdate, ValueGenerated.OnUpdate}
		                .Contains(props[i].ValueGenerated) == false)
		            continue;

	            if (new[] { typeof(long), typeof(int), typeof(short), typeof(byte),
			                typeof(ulong), typeof(uint), typeof(ushort), typeof(sbyte) }
		                .Contains(props[i].ClrType) == false)
		            continue;

	            autoGeneratedFields.Add(i, props[i].Name);
			}

            return autoGeneratedFields;
        }

        private void GenerateLastAutoPropertyValues(Dictionary<TKey, object[]> list)
        {
            var fields = GetAutoGeneratedFields();

	        if (!fields.Any()) return;
	        var values = new Dictionary<string, long>();

	        foreach (var val in fields)
	        {
		        var last = list.Select(p => p.Value[val.Key]).OrderByDescending(p => p).FirstOrDefault();
		        var value = last != null ? (long) Convert.ChangeType(last, typeof(long), CultureInfo.InvariantCulture) : 0;
				values.Add(val.Value, value);
	        }

	        _idCache.LastIds[_entityType.Name] = values;
        }

        private Dictionary<TKey, object[]> Init()
        {
            _filetype = _options.Serializer;

            _serializer = new JsonSerializer(_entityType);

            var fmgr = _options.FileManager;

            if (fmgr.Length >= 9 && fmgr.Substring(0, 9) == "encrypted")
            {
                string password = "";

                if (fmgr.Length > 9)
                {
                    password = fmgr.Substring(10);
                }

                _fileManager = new EncryptedFileManager(_entityType, _filetype, password, _options.DatabaseName);
            }
            else if (fmgr == "private")
            {
                _fileManager = new PrivateFileManager(_entityType, _filetype, _options.DatabaseName);
            }
            else
            {
                _fileManager = new DefaultFileManager(_entityType, _filetype, _options.DatabaseName);
            }

            _updateMethod = list =>
            {
				var cnt = _serializer.Serialize(list);
	            _fileManager.SaveContent(cnt);
            };

            var content = _fileManager.LoadContent();
            var newList = new Dictionary<TKey, object[]>(_keyValueFactory.EqualityComparer);
            var result = _serializer.Deserialize(content, newList, _primaryKey);

            GenerateLastAutoPropertyValues(result);
            return result;
        }
    }
}
