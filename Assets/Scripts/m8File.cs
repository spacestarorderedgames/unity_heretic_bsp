using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace BspTools {

	public class m8File : ScriptableObject
	{
		const int MIPLEVELS = 16;
		const int PAL_SIZE = 256;
		
		public int version;
		public string name;
		
		public uint[] width = new uint[MIPLEVELS];
		public uint[] height = new uint[MIPLEVELS];
		public uint[] offsets = new uint[MIPLEVELS];
		
		public string animName; // next frame in animation
		
		public List<Color> palette = new List<Color>(PAL_SIZE);
		
		public uint flags;
		public uint contents;
		public uint value;
		
		// just the highest mip, this is 2020
		[PreviewField]
		public Texture2D texture;
	}
}
