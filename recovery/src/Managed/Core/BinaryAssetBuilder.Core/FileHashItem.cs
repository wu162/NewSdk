using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using EALA.Hash;

namespace BinaryAssetBuilder.Core;

// Host metadata and an optional previously computed SHA256 receipt. A null
// receipt asks Core to calculate both hashes together when content is needed.
public sealed class FileHashInfo
{
	public bool Exists;
	public DateTime LastWriteTime;
	public string Identity;
	public string ContentHash;
}

[Serializable]
public class FileHashItem : IXmlSerializable
{
	public delegate void KeepAliveDelegate();

	public static KeepAliveDelegate _KeepAliveDelegate;

	private string _Path;

	private string _BuildConfiguration;

	private TargetPlatform _TargetPlatform;

	[NonSerialized]
	private FileState _State;

	private uint _Hash;

	private DateTime _LastDate = DateTime.MinValue;

	// A cache identity, separate from the game's legacy FastHash value. Old
	// serialized entries have no content identity and must be revalidated.
	[OptionalField]
	private string _ContentHash;

	[NonSerialized]
	private string _CurrentContentHash;

	[NonSerialized]
	private bool _Dirty;

	[NonSerialized]
	public DateTime _CurrentDate = DateTime.MaxValue;

	[NonSerialized]
	private bool _Exists;

	[NonSerialized]
	private FileHashInfo _Identity;

	[NonSerialized]
	private uint _CurrentHash;

	[NonSerialized]
	private bool _CurrentHashValid;

	public string Path => _Path;

	public string BuildConfiguration => _BuildConfiguration;

	public TargetPlatform TargetPlatform => _TargetPlatform;

	public uint Hash
	{
		get
		{
			CacheInputBoundary.Validate(_Path);
			if (_State < FileState.HashValid)
			{
				if (IsDirty)
				{
					if (Exists)
					{
						if (!_CurrentHashValid) CalculateHash();
						_Hash = _CurrentHash;
						_ContentHash = _CurrentContentHash;
						_LastDate = _CurrentDate;
						_Dirty = false;
					}
					else
					{
						_Hash = 0u;
						// The missing-file result no longer has the previous
						// content hash's identity. A same-time recreation must
						// recompute FastHash rather than reuse this zero value.
						_ContentHash = null;
						_CurrentContentHash = null;
						_Identity = null;
						_CurrentHashValid = false;
						_LastDate = DateTime.MinValue;
					}
				}
				_State |= FileState.HashValid;
			}
			return _Hash;
		}
	}

	public bool IsDirty
	{
		get
		{
			CacheInputBoundary.Validate(_Path);
			if (_State < FileState.DateValid)
			{
				if (Exists)
				{
					_CurrentDate = _Identity == null ? File.GetLastWriteTime(_Path) : _Identity.LastWriteTime;
					// Known dirty inputs defer all content reads until Hash is requested.
					if (_LastDate == _CurrentDate && _ContentHash != null)
					{
						_CurrentContentHash = _Identity == null ? null : _Identity.ContentHash;
						if (_CurrentContentHash == null) CalculateHash();
					}
				}
				_Dirty = !Exists || _LastDate != _CurrentDate || _ContentHash == null ||
					!string.Equals(_ContentHash, _CurrentContentHash, StringComparison.Ordinal);
				_State = FileState.DateValid;
			}
			return _Dirty;
		}
	}

	public bool Exists
	{
		get
		{
			CacheInputBoundary.Validate(_Path);
			if (_State < FileState.ExistsValid)
			{
				_Identity = Settings.Current?.FileIdentity?.Invoke(_Path);
				_Exists = _Identity == null ? File.Exists(_Path) : _Identity.Exists;
				_State = FileState.ExistsValid;
			}
			return _Exists;
		}
	}

	public XmlSchema GetSchema()
	{
		return null;
	}

	public void WriteXml(XmlWriter writer)
	{
		writer.WriteStartElement("hi");
		writer.WriteAttributeString("d", $"{_Path};{_Hash};{_LastDate.ToBinary()};{_BuildConfiguration};{(int)_TargetPlatform}");
		if (_ContentHash != null) writer.WriteAttributeString("sha256", _ContentHash);
		writer.WriteEndElement();
	}

	public void ReadXml(XmlReader reader)
	{
		_ContentHash = reader.GetAttribute("sha256");
		_State = FileState.AllInvalid;
		_CurrentContentHash = null;
		_Identity = null;
		_CurrentHashValid = false;
		reader.MoveToAttribute("d");
		string[] array = reader.Value.Split(';');
		_Path = array[0];
		_Hash = Convert.ToUInt32(array[1]);
		_LastDate = DateTime.FromBinary(Convert.ToInt64(array[2]));
		if (array.Length > 3)
		{
			if (array.Length > 4)
			{
				_TargetPlatform = (TargetPlatform)int.Parse(array[4]);
			}
			else
			{
				_TargetPlatform = TargetPlatform.Win32;
			}
			_BuildConfiguration = array[3];
		}
		else
		{
			_BuildConfiguration = "";
			_TargetPlatform = TargetPlatform.Win32;
		}
		_CurrentDate = DateTime.MaxValue;
		reader.Read();
	}

	public FileHashItem()
	{
	}

	public FileHashItem(string path, string configuration, TargetPlatform platform)
	{
		_Path = path;
		_BuildConfiguration = configuration;
		_TargetPlatform = platform;
	}

	public void Reset()
	{
		_State = FileState.AllInvalid;
		_CurrentDate = DateTime.MaxValue;
		_CurrentContentHash = null;
		_Identity = null;
		_CurrentHashValid = false;
		_Dirty = false;
	}

	private void CalculateHash()
	{
		AsynchronousFileReader asynchronousFileReader = new AsynchronousFileReader(_Path);
		uint hash = asynchronousFileReader.FileSize;
		using (var algorithm = SHA256.Create())
		{
			while (asynchronousFileReader.BeginRead())
			{
				byte[] data = asynchronousFileReader.CurrentChunk.Data;
				int count = asynchronousFileReader.CurrentChunk.BytesRead;
				hash = FastHash.GetHashCode(hash, data, count);
				algorithm.TransformBlock(data, 0, count, data, 0);
				asynchronousFileReader.EndRead();
			}
			algorithm.TransformFinalBlock(new byte[0], 0, 0);
			_CurrentContentHash = Convert.ToBase64String(algorithm.Hash);
		}
		_CurrentHash = hash;
		_CurrentHashValid = true;
		Settings.Current?.RememberFileContentHash?.Invoke(_Path, _CurrentContentHash);
	}
}
