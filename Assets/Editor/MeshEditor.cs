using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshGenerator))]
public class MeshEditor : Editor
{
	public override void OnInspectorGUI()
	{
		MeshGenerator mapGen = (MeshGenerator)target;

		if (DrawDefaultInspector())
		{
			if (mapGen.autoUpdate)
			{
				mapGen.UpdateMesh();
			}
		}

		if (GUILayout.Button("Generate"))
		{
			mapGen.CreateShape();
		}
	}
}