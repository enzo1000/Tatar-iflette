using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//Script en charge de creer des Dungeons composes de pieces (une piece est une carre / rectangle)
// Herite de Simple Random Walk Dungeon Generator
public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    //Defini la taille minimale d'une salle
    [SerializeField]
    private int minRoomWidth = 4, minRoomHeight = 4;

    //Defini une taille approximative pour le Dungeon
    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;

    [SerializeField]
    [Range(0, 10)]
    private int offset = 1;

    //Si on veut utiliser l'agorithme de RandomWalk / BoundingBox
    [SerializeField]
    private bool randomWalkRooms = false;

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    //Fonction regroupant toutes les fonctions necessaire a la creation de notre dongeon precedural
    private void CreateRooms()
    {
        //On envoi a l'algo de generation procedurale de piece un espace a decouper de taille "startPosition" -> "dungeonWidth, dungeonHeight"
        var roomsList = ProceduralGenerationAlgotithms.BinarySpacePartitioning(
            new BoundsInt((Vector3Int) startPosition,
            new Vector3Int(dungeonWidth, dungeonHeight, 0)), 
            minRoomWidth, 
            minRoomHeight);

        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        //Choix de l'algo pour la creation du Dungeon RandomWalk / BoundingBox
        if(randomWalkRooms)
        {
            floor = CreateRoomsRandomly(roomsList);
        }
        else
        {
            floor = CreateSimpleRooms(roomsList);
        }

        //On vient stocker le centre de chaques piece dans une liste
        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (BoundsInt room in roomsList)
        {
            roomCenters.Add((Vector2Int) Vector3Int.RoundToInt(room.center));
        }

        //Connexion des pieces (centre) par des couloirs
        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);

        //Union des listes de sol et de couloir pour la generation de mur
        floor.UnionWith(corridors);

        //Tiling des cases + generation des murs
        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);
    }

    //Premiere fonction de generation procedurale (randomWalkRooms == True)
    //Remplis les salles d'une manière plus organique tout en respectant les contraintes de la room
    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach (Vector2Int position in roomFloor)
            {
                if(position.x >= (roomBounds.xMin + offset) 
                    && position.x <= (roomBounds.xMax - offset) 
                    && position.y >= (roomBounds.yMin - offset) 
                    && position.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

    //Le but de cette fonction est de créer un chemin entre chaque salle de notre donjon
    // afin de les relier avec des couloirs.
    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        //On sélectionne une salle de dépat aléatoire
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];

        //On la supprime de notre liste
        roomCenters.Remove(currentRoomCenter);

        //Tant qu'on trouve des salles à relier
        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    //Creer les couloirs entre les pieces
    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);
        while (position.y != destination.y)
        {
            if(destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if(destination.y < position.y)
            {
                position += Vector2Int.down;
            }
            corridor.Add(position);
        }
        while(position.x != destination.x)
        {
            if(destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if(destination.x < position.x)
            {
                position += Vector2Int.left;
            }
            corridor.Add(position);
        }
        return corridor;
    }

    //Trouve la salle la plus proche à notre pieces actuelle en fonction du centre des salles
    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (Vector2Int position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if(currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    //Deuxieme fonction de generation procedurale (randomWalkRooms == False)
    //Creer le Tiling pour notre Dungeon d'une maniere tres basique (rectangle)
    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        //Pour tout les points de nos salles, on leurs ajoute une case floor
        foreach (BoundsInt room in roomsList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }
}
