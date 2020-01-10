#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;
using BspTools.v38;

namespace BspTools {
	public class BspImporter : OdinEditorWindow
	{
		[MenuItem("Tools/BspImporter")]
		private static void OpenWindow()
		{
			var window = GetWindow<BspImporter>();
	
			// Nifty little trick to quickly position the window in the middle of the editor.
			window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
		}
		
		string path = null;
		
		[Button()]
		public void ImportBspFile() {
			path = EditorUtility.OpenFilePanel("BSP File", "", "bsp");
			if (path.Length != 0)
			{
				string mapName = Path.GetFileNameWithoutExtension(path);
				Bsp38Loader loader = new Bsp38Loader();
				var result = loader.LoadBsp(path);
				result.name = mapName;
				Debug.Log("Loaded: " + mapName);
				
				if (!AssetDatabase.IsValidFolder("Assets/ImportedBSP")) {
					AssetDatabase.CreateFolder("Assets", "ImportedBSP");
				}
				AssetDatabase.CreateAsset(result, "Assets/ImportedBSP/" + mapName + ".asset");
				AssetDatabase.SaveAssets();
				
				// load all the textures
				// the textures should be located up 1 from maps
				string texturesRootPath = Path.GetDirectoryName(path).Replace("maps", "textures");
				Debug.Log("Textures Path: " + texturesRootPath);
				foreach(var info in result.texInfo) {
					m8Loader loaderm8 = new m8Loader();
					string filePath = string.Format("{0}/{1}.m8", texturesRootPath, info.name);
					filePath = filePath.Replace('\\', '/'); // this is prolly only working on Windows, but that's what I'm using and it's late
					Debug.Log(string.Format("Path: {0}", filePath));
					var assetm8 = loaderm8.Import(filePath);
					if (!AssetDatabase.IsValidFolder("Assets/ImportedBSP/Textures")) {
						AssetDatabase.CreateFolder("Assets/ImportedBSP", "Textures");
					}
					if (!AssetDatabase.IsValidFolder("Assets/ImportedBSP/Materials")) {
						AssetDatabase.CreateFolder("Assets/ImportedBSP", "Materials");
					}
					if (info.name.Contains("/")) {
						var folder = info.name.Split('/')[0];
						if (!AssetDatabase.IsValidFolder("Assets/ImportedBSP/Textures/" + folder)) {
							AssetDatabase.CreateFolder("Assets/ImportedBSP/Textures", folder);
						}
						if (!AssetDatabase.IsValidFolder("Assets/ImportedBSP/Materials/" + folder)) {
							AssetDatabase.CreateFolder("Assets/ImportedBSP/Materials", folder);
						}
					}
					AssetDatabase.CreateAsset(assetm8, "Assets/ImportedBSP/Textures/" + info.name + ".asset");
					AssetDatabase.SaveAssets();
					
					File.WriteAllBytes(Path.Combine(Application.dataPath, "ImportedBSP/Textures/" + info.name + ".png"), assetm8.texture.EncodeToPNG());
					AssetDatabase.Refresh();
					Material mat = new Material(Shader.Find("Standard"));
					var dbTx = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/ImportedBSP/Textures/" + info.name + ".png", typeof(Texture2D));
					mat.mainTexture = dbTx;
					AssetDatabase.CreateAsset(mat, "Assets/ImportedBSP/Materials/" + info.name + ".mat");
					AssetDatabase.SaveAssets();
				}
			}
		}
		
		[Button]
		public void ImportM8File() {
			path = EditorUtility.OpenFilePanel("m8 File", "", "m8");
			Debug.Log(string.Format("Path: {0}", path));
			if (path.Length != 0)
			{
				string matName = Path.GetFileNameWithoutExtension(path);
				m8Loader loader = new m8Loader();
				var result = loader.Import(path);
				if (!AssetDatabase.IsValidFolder("Assets/ImportedBSP")) {
					AssetDatabase.CreateFolder("Assets", "ImportedBSP");
				}
				if (!AssetDatabase.IsValidFolder("Assets/ImportedBSP/Materials")) {
					AssetDatabase.CreateFolder("Assets/ImportedBSP", "Materials");
				}
				AssetDatabase.CreateAsset(result, "Assets/ImportedBSP/Materials/" + matName + ".asset");
				AssetDatabase.SaveAssets();
			}
		}
	}
}
#endif