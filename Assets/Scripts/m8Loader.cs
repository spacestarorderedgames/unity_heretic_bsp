using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BspTools.v38 {
	public class m8Loader
	{
		public m8File Import(string path) {
			m8File file = ScriptableObject.CreateInstance<m8File>();
			Debug.Log("m8 import path: " + path);
			BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open));
			reader.BaseStream.Seek(0, SeekOrigin.Begin);
			
			file.version = reader.ReadInt32();
			file.name = new string(reader.ReadChars(32));
			for (int i=0; i<16; i++) {
				file.width[i] = reader.ReadUInt32();
			}
			for (int j=0; j<16; j++) {
				file.height[j] = reader.ReadUInt32();
			}
			for (int k=0; k<16; k++) {
				file.offsets[k] = reader.ReadUInt32();
			}
			file.animName = new string(reader.ReadChars(32));
			for (int p=0; p<256; p++) {
				var r = reader.ReadByte();
				var g = reader.ReadByte();
				var b = reader.ReadByte();
				
				Color color = new Color32(r, g, b, 255);
				file.palette.Add(color);
			}
			file.flags = reader.ReadUInt32();
			file.contents = reader.ReadUInt32();
			file.value = reader.ReadUInt32();
			
			// just do the highest detail mipmap
			reader.BaseStream.Seek(file.offsets[0], SeekOrigin.Begin);
			var width = file.width[0];
			var height = file.height[0];
			Color[] colors = new Color[width*height];
			int colorIndex = 0;
			for (int h=0; h<height-1; h++) {
				for (int w=0; w<width-1; w++) {
					var pixel = reader.ReadByte();
					colors[colorIndex] = file.palette[pixel];
					colorIndex++;
				}
			}
			file.texture = new Texture2D((int)width, (int)height);
			file.texture.SetPixels(colors);
			file.texture.Apply(false);
			
			#if UNITY_EDITOR
			//Material mat = new Material(Shader.Find("Standard"));
			////mat.SetTexture("_MainTex", file.texture);
			//if (!AssetDatabase.IsValidFolder("Assets/ImportedBSP/Materials")) {
			//	AssetDatabase.CreateFolder("Assets/ImportedBSP", "Materials");
			//}
			//File.WriteAllBytes(Path.Combine(Application.dataPath, "ImportedBSP/Materials/", Path.GetFileNameWithoutExtension(path) + ".png"), file.texture.EncodeToPNG());
			//AssetDatabase.Refresh();
			//var dbTx = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/ImportedBSP/Materials/" + Path.GetFileNameWithoutExtension(path) + ".png", typeof(Texture2D));
			//mat.mainTexture = dbTx;
			//AssetDatabase.CreateAsset(mat, "Assets/ImportedBSP/Materials/" + Path.GetFileNameWithoutExtension(path) + ".mat");
			//AssetDatabase.SaveAssets();
			#endif
			
			reader.BaseStream.Dispose();
			
			return file;
		}    
	}
}