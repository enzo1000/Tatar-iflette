using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//Editeur custom pour AbstractDungeonGenerator
// sert a autoriser la modification de la view depuis l'inspector (bouton "Generate Dungeon")
[CustomEditor(typeof(AbstractDungeonGenerator), true)]
public class RandomDungeonGeneratorEditor : Editor
{
    AbstractDungeonGenerator generator;

    private void Awake()
    {
        generator = (AbstractDungeonGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Generate Dungeon"))
        {
            generator.GenerateDungeon();
        }
    }
}
