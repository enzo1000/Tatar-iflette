using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    void Start()
    {
        //Quand le jeu se lance, on lance la generation procedurale
        gameObject.GetComponent<RoomFirstDungeonGenerator>().GenerateDungeon();
    }

    void Update()
    {
        
    }

    //Positionne le personnage au debut du niveau
    private void createCharacter()
    {

    }
}
