using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BspTools.v38.Types {
	[Serializable]
	public class TexInfo
	{
		public Vector3 uAxis { get; set; }
		public float uOffset { get; set; }
		public Vector3 vAxis { get; set; }
		public float vOffset { get; set; }
		public uint flags { get; set; }
		public uint value { get; set; }
		public string name { get; set; }
		public uint nextTexInfo { get; set; }
	}
}