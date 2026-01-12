using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target; //need to cast because target is just the element selected
       
        AutoUpdate(mapGen);
     
        
        AutoUpdateButton(mapGen);

        GUILayout.BeginHorizontal();
        ButtonGenerate(mapGen);
        ButtonReset(mapGen);
        GUILayout.EndHorizontal();
        
        
    }

    private void ButtonReset(MapGenerator mapGen)
    {
        GUI.backgroundColor = Color.red;
        
        if (GUILayout.Button("Reset"))
        {
            mapGen.ResetValue();
            mapGen.GenerateMap();
        }
    }

    private void ButtonGenerate(MapGenerator mapGen)
    {
        GUI.backgroundColor = Color.lawnGreen;
        
        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap();
        }
    }

    private void AutoUpdate(MapGenerator mapGen)
    {
        if (DrawDefaultInspector()) //check if modification on Inspector like changing a value
        {
            if (mapGen.AutoUpdate) mapGen.GenerateMap();
        }
    }

    private void AutoUpdateButton(MapGenerator mapGen)
    {
        var text = mapGen.AutoUpdate ? "Cancel Auto Update" : "Launch Auto Update";
        
        if (GUILayout.Button(text))
        {
            mapGen.AutoUpdate = !mapGen.AutoUpdate;
        }
    }

}
