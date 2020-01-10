using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BspTools.v38.Types {
	[Serializable]
	public class Face
	{
		public ushort plane;
		public ushort planeSide;
		public uint firstEdge;
		public ushort edgeCount;
		public ushort texInfo;
		public char[] lightmapStyles = new char[4];
		public uint lightmapOffset;
	}
}