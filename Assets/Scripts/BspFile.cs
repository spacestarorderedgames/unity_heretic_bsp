using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BspTools {
	using Sirenix.OdinInspector;
	using BspTools.v38.Types;
	
	public class MeshData {
		public string txName;
		public List<Vector3> vertices = new List<Vector3>();
		public List<int> triangles = new List<int>();
		public List<Vector2> uvs = new List<Vector2>();
	}
	
	
	public class BspFile : ScriptableObject
	{
		private const int SKYBOX_FLAG = 4;
		
		public string name = "map";
		
		[Title("Entities")]
		[HideLabel]
		[TextArea(10, 50)]
		public string entities;
		
		public List<Vector3> verts = new List<Vector3>();
		public List<Edge> edges = new List<Edge>();
		public List<Face> faces = new List<Face>();
		public List<int> faceEdges = new List<int>();
		
		public List<TexInfo> texInfo = new List<TexInfo>();
		
		// private Dictionary<string, MeshData> subMeshes = new Dictionary<string, MeshData>();
		
		[Button("Build")]
		public void BuildMeshes() {
			GameObject map = new GameObject("map_" + name);
			
			Dictionary<string, List<Mesh>> meshesByTexture = new Dictionary<string, List<Mesh>>();
			
			int i = 0;
			foreach(var face in faces) {
				var faceMesh = BuildFaceMesh(face);
				
				List<Mesh> combineMeshes;
				if (!meshesByTexture.ContainsKey(faceMesh.txName)) {
					combineMeshes = new List<Mesh>();
					meshesByTexture.Add(faceMesh.txName, combineMeshes);
				} else {
					combineMeshes = meshesByTexture[faceMesh.txName];
				}
				
				Mesh mesh = new Mesh();
				mesh.name = "mesh_" + faceMesh.txName + " " + i;
				mesh.SetVertices(faceMesh.vertices);
				mesh.SetTriangles(faceMesh.triangles, 0);
				mesh.SetUVs(0, faceMesh.uvs);
				mesh.RecalculateBounds();
				mesh.RecalculateNormals();
				
				combineMeshes.Add(mesh);
				
				i++;
			}
			
			foreach(var txName in meshesByTexture.Keys) {
				GameObject sm = new GameObject("sm_" + txName, typeof(MeshFilter), typeof(MeshRenderer));
				sm.transform.parent = map.transform;
				sm.isStatic = true;
				var mesh = CombineMeshes(meshesByTexture[txName], sm);
				sm.GetComponent<MeshFilter>().mesh = mesh;
				var mat = new Material(Shader.Find("Standard"));
				#if UNITY_EDITOR
				mat = (Material)AssetDatabase.LoadAssetAtPath("Assets/ImportedBSP/Materials/" + txName + ".mat", typeof(Material));
				#endif
				sm.GetComponent<MeshRenderer>().material = mat;
			}
		}
		
		private Mesh CombineMeshes(List<Mesh> meshes, GameObject go)
		{
			var combine = new CombineInstance[meshes.Count];
			for (int i = 0; i < meshes.Count; i++)
			{
				combine[i].mesh = meshes[i];
				combine[i].transform = go.transform.localToWorldMatrix;
			}

			var mesh = new Mesh();
			mesh.CombineMeshes(combine);
			return mesh;
		}
		
		private MeshData BuildFaceMesh(Face face) {
			var txInfo = texInfo[face.texInfo];
			//if (txInfo.flags & (1 << SKYBOX_FLAG)) { // TODO: other flags
			//	Debug.Log("not draw skybox!");
			//	return;
			//}
			
			MeshData subMesh = new MeshData();
			subMesh.txName = txInfo.name;
			
			Vector3[] vertices = new Vector3[face.edgeCount];
			int[] tris = new int[(face.edgeCount - 2) * 3];
			Vector2[] uvs = new Vector2[face.edgeCount];

			// edges
			int edgeIndex = (int)face.firstEdge;
			for (int i = 0; i < face.edgeCount; i++) {
				edgeIndex = faceEdges[(int)face.firstEdge + i];
				var edge = edges[Mathf.Abs(edgeIndex)];
				var vertIndex = edgeIndex < 0 ? edge.vert1 : edge.vert2;
				// Debug.Log("EdgeIdx: " + edgeIndex + " VertIndex: " + vertIndex + " VertsLn: " + vertices.Length + " Edge: " + edge);
				var vert = verts[vertIndex];
				vertices[i] = vert;
			}
			
			// triangles
			int tristep = 1;
			for (int i = 1; i < vertices.Length - 1; i++)
			{
				tris[tristep - 1] = 0;
				tris[tristep] = i;
				tris[tristep + 1] = i + 1;
				tristep += 3;
			}
			
			// uvs
			for (int j = 0; j < face.edgeCount; j++) {
				var vert = vertices[j];
				var u = Vector3.Dot(vert, txInfo.uAxis) + txInfo.uOffset;
				var v = Vector3.Dot(vert, txInfo.vAxis) + txInfo.vOffset;
				var uv = new Vector2(u, v);
				uvs[j] = uv;
			}
			
			subMesh.vertices.AddRange(vertices);
			subMesh.triangles.AddRange(tris);
			subMesh.uvs.AddRange(uvs);
			
			return subMesh;
		}
	}
}
