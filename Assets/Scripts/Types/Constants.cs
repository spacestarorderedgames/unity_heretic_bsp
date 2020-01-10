using System.Collections;
using System.Collections.Generic;

namespace BspTools.v38.Types {
	public enum Lumps {
		ENTITIES = 0,
		PLANES,
		VERTICES,
		VIS,
		NODES,
		TEXTURES,
		FACES,
		LIGHTMAPS,
		LEAVES,
		LEAF_FACE_TABLE,
		LEAF_BRUSH_TABLE,
		EDGES,
		FACE_EDGE_TABLE,
		MODELS,
		BRUSHES,
		BRUSH_SIDES,
		POP,
		AREAS,
		AREA_PORTALS
	}
	
	public static class Constants
	{
		public static int NUM_LUMPS = 19;
	}
}