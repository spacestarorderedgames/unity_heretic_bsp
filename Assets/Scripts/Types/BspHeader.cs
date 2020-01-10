using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BspTools.v38.Types {
	
	public class BspHeader
	{
		public class DirectoryEntry
		{
			public string Name { get; private set; }
			public int Offset { get; private set; }
			public int Length { get; private set; }

			public DirectoryEntry(string name, int offset, int length)
			{
				Name = name;
				Offset = offset;
				Length = length;
			}

			public override string ToString()
			{
				return Name + " Offset: " + Offset + " Length: " + Length + "\r\n";
			}
		}

		List<DirectoryEntry> directory = new List<DirectoryEntry>();
		uint version;
		string magic;
		const int NUM_LUMPS = 19;
		
		public uint Version { get { return version; } }
		
		public DirectoryEntry GetLump(Lumps lump) {
			return directory[(int)lump];
		}
		
		public BspHeader(BinaryReader bspFile)
		{
			bspFile.BaseStream.Seek(0, SeekOrigin.Begin);
			magic = new string(bspFile.ReadChars(4)); // IBSP
			version = bspFile.ReadUInt32();
			
			Debug.Log("Magic " + magic + " \r\nVersion: " + version);

			for (int i = 0; i < NUM_LUMPS; i++)
			{
				directory.Add(new DirectoryEntry(Enum.GetName(typeof(Lumps), i), bspFile.ReadInt32(), bspFile.ReadInt32()));
				Debug.Log("Directory [" + i + "] " + directory[i]);
			}
		}
		
		public override string ToString() {
			string blob = "=== BSP Header === \r\n";
			blob += "Magic Number: " + magic + " ";
			blob += "BSP Version: " + Version + "\r\n";
			blob += "Header Directory:\r\n";
			int count = 0;
			foreach (DirectoryEntry entry in directory)
			{
				blob += "Lump " + count + ": " + entry.ToString();
				count++;
			}

			return blob;
		}
	}
}