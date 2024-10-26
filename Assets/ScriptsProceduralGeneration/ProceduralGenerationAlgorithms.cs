using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Contient toutes les fonctions de generation procedurale accessible pour toute les classes
public static class ProceduralGenerationAlgotithms
{
    //On renvoie un HashSet pour supprimer plus simplement les doublons (étant donnée que RandomWalk est aléatoire)
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>
        {
            startPosition
        };

        Vector2Int previousPosition = startPosition;

        for (int i = 0; i < walkLength; i++)
        {
            Vector2Int newPosition = previousPosition + Direction2D.GetRandomCardinalDirection();
            path.Add(newPosition);
            previousPosition = newPosition;
        }

        return path;
    }

    //Using a List to access it's last element more easily
    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength)
    {
        List<Vector2Int> corridor = new List<Vector2Int>
        {
            startPosition
        };

        Vector2Int direction = Direction2D.GetRandomCardinalDirection();
        Vector2Int currentPosition = startPosition;

        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
        }

        return corridor;
    }

    //BoundsInt est une structure représentant une bounding box en 3D, plus d'info sur la doc Unity (AABB)
    //Fonction servant a decouper un espace en plusieurs boite 
    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();

        //Les autres roomsQueue.Enqueue se font dans les fonctions SplitHorizontally et SplitVertically
        roomsQueue.Enqueue(spaceToSplit);
        while(roomsQueue.Count > 0)
        {
            BoundsInt room = roomsQueue.Dequeue();

            //Si notre piece peut encore etre decoupee
            if (room.size.y >= minHeight && room.size.x >= minWidth)
            {
                //Genere une valeur aleatoire entre 0.0f et 1.0f
                if (Random.value < 0.5f)
                {
                    //Verification de la possibilite de split l'espace en deux
                    // Verification Horizontale
                    if(room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }
                    // Verification Verticale
                    else if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }
                    // Sinon, on ajoute la room tel quelle
                    else if (room.size.x >= minWidth && room.size.y > minHeight)
                    {
                        roomsList.Add(room);
                    }
                }
                //Idem que pour la partie au dessus mais cette fois avec une inversion de l'ordre de split
                else
                {
                    if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }
                    else if (room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }
                    else if (room.size.x >= minWidth && room.size.y > minHeight)
                    {
                        roomsList.Add(room);
                    }
                }
            }
        }
        return roomsList;
    }

    //Fonction de decoupage de l'espace verticale
    private static void SplitVertically(int minWidth, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var xSplit = Random.Range(1, room.size.x);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));

        BoundsInt room2 = new BoundsInt( new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z), 
                                         new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
    
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    //Fonction de decoupage de l'espace horizontale
    private static void SplitHorizontally(int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var ySplit = Random.Range(1, room.size.y);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));

        BoundsInt room2 = new BoundsInt( new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
                                         new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }
}

//Classe definissant les direction dans un espace 2D
public static class Direction2D
{
    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0, 1), //UP
        new Vector2Int(1, 0), //RIGHT
        new Vector2Int(0,-1), //DOWN
        new Vector2Int(-1,0)  //LEFT
    };

    public static List<Vector2Int> diagonalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(1,  1),  //UP - RIGHT
        new Vector2Int(1, -1),  //RIGHT - DOWN
        new Vector2Int(-1,-1),  //DOWN - LEFT
        new Vector2Int(-1, 1)   //LEFT - UP
    };

    public static List<Vector2Int> eightDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0, 1),   //UP
        new Vector2Int(1,  1),  //UP - RIGHT
        new Vector2Int(1, 0),   //RIGHT
        new Vector2Int(1, -1),  //RIGHT - DOWN
        new Vector2Int(0,-1),   //DOWN
        new Vector2Int(-1,-1),  //DOWN - LEFT
        new Vector2Int(-1,0),   //LEFT
        new Vector2Int(-1, 1)   //LEFT - UP
    };

    //Fonction qui renvoi une direction aleatoire (UP, RIGHT, DOWN ou LEFT)
    public static Vector2Int GetRandomCardinalDirection()
    {
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
    }
}