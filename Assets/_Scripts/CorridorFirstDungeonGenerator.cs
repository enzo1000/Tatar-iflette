using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField]
    private int corridorLength = 14, corridorCount = 5;

    [SerializeField]
    [Range(0.1f, 1)]
    private float roomPercent = 0.8f;

    protected override void RunProceduralGeneration()
    {
        CorridorFirstGeneration();
    }

    private void CorridorFirstGeneration()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPosition = new HashSet<Vector2Int>();

        // List of List of Vector2Int pour pemettre de cr�er des couloirs avec une certaine �paisseur
        List<List<Vector2Int>> corridors = CreateCorridors(floorPositions, potentialRoomPosition);
        
        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPosition);

        // A list of all the corridors that are dead ends
        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);

        //On fournit les culs-de-sac et la liste des salles pour v�rifier si les culs-de-sac sont dans d�j� dans des salles
        CreateRoomsAtDeadEnd(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);

        /*for (int i = 0; i < corridors.Count; i++)
        {
            //On le fait apr�s avoir trouv� les diff�rentes DeadEnds car sela g�nerait notre algo pour d�tecter lesdites DeadEnds
            //corridors[i] = IncreaseCorridorSizeByOne(corridors[i]);
            corridors[i] = IncreaseCorridorBrush3by3(corridors[i]);
            floorPositions.UnionWith(corridors[i]);
            
        }*/

        //Apr�s avoir cr�� les couloirs, on Update les salles / murs car on a update floorPositions
        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    private List<Vector2Int> IncreaseCorridorBrush3by3(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        for (int i = 1; i < corridor.Count; i++)
        {
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    newCorridor.Add(corridor[i - 1] + new Vector2Int(x, y));
                }
            }
        }
        return newCorridor;
    }

    //This algorithm don't work as expected in certain cases be careful
    private List<Vector2Int> IncreaseCorridorSizeByOne(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        Vector2Int previousDirection = Vector2Int.zero;
        for (int i = 1; i < corridor.Count; i++)
        {
            //On d�finit notre direction pour chaque cellule
            Vector2Int directionFromCell = corridor[i] - corridor[i - 1];

            // Si on change de direction (alors on est dans un coin)
            if (previousDirection != Vector2Int.zero && directionFromCell != previousDirection)
            {
                // Simulation d'un pinceau 3x3 pour les coins
                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        //On prend notre coordonn�e pr�c�dente et on ajoute la direction (en vecteur) pour obtenir notre coin
                        newCorridor.Add(corridor[i - 1] + new Vector2Int(x, y));
                    }
                }
                //On sauvegarde notre nouvelle direction
                previousDirection = directionFromCell;
            }
            //Si on est pas dans un coin alors, fonctionnment attendu
            else
            {
                //Si on va en haut alors on ajoutera une case sur la droite et ... (90�)
                Vector2Int newCorridorTileOffset = GetDirection90Fom(directionFromCell);
                newCorridor.Add(corridor[i - 1]);   //Ajoute la tile originale (celle de la direction pr�c�dente)
                newCorridor.Add(corridor[i - 1] + newCorridorTileOffset); //Ajoute la tile de la direction 90�
            }
        }
        return newCorridor;
    }

    private Vector2Int GetDirection90Fom(Vector2Int direction)
    {
        if (direction == Vector2Int.up)
        {
            return Vector2Int.right;
        }
        if (direction == Vector2Int.down)
        {
            return Vector2Int.left;
        }
        if (direction == Vector2Int.left)
        {
            return Vector2Int.up;
        }
        if (direction == Vector2Int.right)
        {
            return Vector2Int.down;
        }
        return Vector2Int.zero;
    }

    private void CreateRoomsAtDeadEnd(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach (Vector2Int position in deadEnds)
        {
            //Si notre cul de sac n'est pas dans une salle, on cr�e une salle
            if (roomFloors.Contains(position) == false)
            {
                HashSet<Vector2Int> room = RunRandomWalk(randomWalkParameters, position);
                roomFloors.UnionWith(room);
            }
        }
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();

        //On parcourt toutes les positions du sol
        foreach (Vector2Int position in floorPositions)
        {
            int neighbourCount = 0;
            //Pour chaque position, on regarde si les positions voisines sont dans le sol (Up, Down, Left, Right)
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                //Si la position voisine est dans le sol, on incr�mente le nombre de voisins
                if (floorPositions.Contains(position + direction))
                {
                    neighbourCount++;
                }
            }
            //Si le nombre de voisins est �gal � 1, alors la position est un cul-de-sac (car un seul voisin)
            if (neighbourCount == 1)
            {
                deadEnds.Add(position);
            }
        }

        //On retourne la liste des culs-de-sac afin de cr�er des salles � ces positions
        return deadEnds;
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPosition)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPosition.Count * roomPercent);

        // Ordering les valeurs de mani�re al�atoire (car il n'y a pas de valeur GUID pr�d�finie)
        // La ligne enti�re : On s�lectionne des valeurs al�atoires dans la liste et on en tire un pourcentage qu'on converti en liste
        List<Vector2Int> roomsToCreate = potentialRoomPosition.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

        foreach (Vector2Int roomPosition in roomsToCreate)
        {
            HashSet<Vector2Int> roomFloor = RunRandomWalk(randomWalkParameters, roomPosition);
            roomPositions.UnionWith(roomFloor);
        }

        return roomPositions;
    }

    private List<List<Vector2Int>> CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPosition)
    {
        Vector2Int currentPosition = startPosition;
        potentialRoomPosition.Add(currentPosition); //On ajoute la premi�re position du couloir
        List<List<Vector2Int>> corridors = new List<List<Vector2Int>>();

        for (int i = 0; i < corridorCount; i++)
        {
            List<Vector2Int> corridor = ProceduralGenerationAlgotithms.RandomWalkCorridor(currentPosition, corridorLength);
            corridors.Add(corridor);
            currentPosition = corridor[corridor.Count - 1];
            potentialRoomPosition.Add(currentPosition); //On ajoute la derni�re position du couloir 
            floorPositions.UnionWith(corridor);
        }

        return corridors;
    }
}
