using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target; //need to cast because target is just the element selected

        if (DrawDefaultInspector()) //check if modification on Inspector like changing a value
        {
            if (mapGen.AutoUpdate) mapGen.GenerateMap();
        }

        if (GUILayout.Button("Generate")) mapGen.GenerateMap();
    }
}
