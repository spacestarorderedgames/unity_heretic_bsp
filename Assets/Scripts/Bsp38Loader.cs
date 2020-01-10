using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BspTools.v38 {
	using BspTools.v38.Types;
	public class Bsp38Loader
	{
		public const float QUAKE_TO_UNITY_CONVERSION_SCALE = 0.03f;
		
		public BspFile LoadBsp(string path) {
			BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open));
			
			BspHeader header = new BspHeader(reader);
			
			BspFile parsed = ScriptableObject.CreateInstance<BspFile>();
			parsed.entities = ReadEntities(reader, header);
			parsed.verts = ReadVerts(reader, header);
			parsed.edges = ReadEdges(reader, header);
			parsed.faces = ReadFaces(reader, header);
			parsed.faceEdges = ReadFaceEdges(reader, header);
			parsed.texInfo = ReadTexInfo(reader, header);
			
			reader.BaseStream.Dispose();
			
			return parsed;
		}
		
		public string ReadEntities(BinaryReader reader, BspHeader header) {
			var entry = header.GetLump(Lumps.ENTITIES);
			reader.BaseStream.Seek(entry.Offset, SeekOrigin.Begin);
			return new string(reader.ReadChars(entry.Length));
		}
		
		public List<Vector3> ReadVerts(BinaryReader reader, BspHeader header) {
			List<Vector3> verts = new List<Vector3>();
			var entry = header.GetLump(Lumps.VERTICES);
			int numVerts = entry.Length / 12;
			reader.BaseStream.Seek(entry.Offset, SeekOrigin.Begin);
			for(int i = 0; i < numVerts; i++) {
				float x = reader.ReadSingle();
				float y = reader.ReadSingle();
				float z = reader.ReadSingle();
				// convert to unity axis
				Vector3 vec = new Vector3(-x, z, -y);
				// scale for unity
				vec.Scale(Vector3.one * QUAKE_TO_UNITY_CONVERSION_SCALE);
				verts.Add(vec);
			}
			
			return verts;
		}
		
		public List<Edge> ReadEdges(BinaryReader reader, BspHeader header) {
			List<Edge> edges = new List<Edge>();
			var entry = header.GetLump(Lumps.EDGES);
			int numEdges = entry.Length / 4;
			reader.BaseStream.Seek(entry.Offset, SeekOrigin.Begin);
			for(int i = 0; i < numEdges; i++) {
				edges.Add(new Edge(reader.ReadUInt16(), reader.ReadUInt16()));
			}
			
			return edges;
		}
		
		public List<Face> ReadFaces(BinaryReader reader, BspHeader header) {
			List<Face> faces = new List<Face>();
			var entry = header.GetLump(Lumps.FACES);
			int numFaces = entry.Length / 20;
			reader.BaseStream.Seek(entry.Offset, SeekOrigin.Begin);
			for(int i = 0; i < numFaces; i++) {
				var face = new Face();
				face.plane = reader.ReadUInt16();
				face.planeSide = reader.ReadUInt16();
				face.firstEdge = reader.ReadUInt32();
				face.edgeCount = reader.ReadUInt16();
				face.texInfo = reader.ReadUInt16();
				face.lightmapStyles[0] = reader.ReadChar();
				face.lightmapStyles[1] = reader.ReadChar();
				face.lightmapStyles[2] = reader.ReadChar();
				face.lightmapStyles[3] = reader.ReadChar();
				face.lightmapOffset = reader.ReadUInt32();
				faces.Add(face);
			}
			
			return faces;
		}
		
		public List<int> ReadFaceEdges(BinaryReader reader, BspHeader header) {
			List<int> faceEdges = new List<int>();
			var entry = header.GetLump(Lumps.FACE_EDGE_TABLE);
			int numEdges = entry.Length / 4;
			reader.BaseStream.Seek(entry.Offset, SeekOrigin.Begin);
			for(int i = 0; i < numEdges; i++) {
				faceEdges.Add(reader.ReadInt32());
			}
			
			return faceEdges;
		}
		
		public List<TexInfo> ReadTexInfo(BinaryReader reader, BspHeader header) {
			List<TexInfo> infos = new List<TexInfo>();
			
			var entry = header.GetLump(Lumps.TEXTURES);
			int numInfos = entry.Length / 76;
			Debug.Log(numInfos + " Texture Infos");
			reader.BaseStream.Seek(entry.Offset, SeekOrigin.Begin);
			for (int i = 0; i < numInfos; i++) { 
				var info = new TexInfo();
				
				float ux = reader.ReadSingle();
				float uy = reader.ReadSingle();
				float uz = reader.ReadSingle();
				info.uAxis = new Vector3(-ux, uz, -uy);
				info.uOffset = reader.ReadSingle();// * QUAKE_TO_UNITY_CONVERSION_SCALE; // * scale??
				float vx = reader.ReadSingle();
				float vy = reader.ReadSingle();
				float vz = reader.ReadSingle();
				info.vAxis = new Vector3(-vx, vz, -vy);
				info.vOffset = reader.ReadSingle();// * QUAKE_TO_UNITY_CONVERSION_SCALE; // * scale??
				
				info.flags = reader.ReadUInt32();
				info.value = reader.ReadUInt32();
				info.name = new string(reader.ReadChars(32));
				int pos = info.name.IndexOf('\0');
				if (pos >= 0)
					info.name = info.name.Substring(0, pos);
				info.nextTexInfo = reader.ReadUInt32();
				
				infos.Add(info);
			}
			
			return infos;
		}
	}
}