using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap, wallTilemap;

    [SerializeField]
    private TileBase floorTile, wallTop, wallSideRight, wallSideLeft, wallBottom, wallFull,
        wallInnerCornerDownLeft, wallInnerCornerDownRight, 
        wallDiagonalCornerDownRight, wallDiagonalCornerDownLeft, wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPosition)
    {
        PaintTiles(floorPosition, floorTilemap, floorTile);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (Vector2Int position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    internal void PaintSingleBasicWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        //Debug.Log(position + " type: " + binaryType);
        TileBase tile = null;
        if (WallTypesHelper.wallTop.Contains(typeAsInt))
        {
            //Debug.Log("Wall top : " + binaryType);
            tile = wallTop;
        }
        else if (WallTypesHelper.wallSideRight.Contains(typeAsInt))
        {
            //Debug.Log("Wall side right : " + binaryType);
            tile = wallSideRight;
        }
        else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt))
        {
            //Debug.Log("Wall side left : " + binaryType);
            tile = wallSideLeft;
        }
        else if (WallTypesHelper.wallBottom.Contains(typeAsInt))
        {
            //Debug.Log("Wall bottom : " + binaryType);
            tile = wallBottom;
        }
        else if (WallTypesHelper.wallFull.Contains(typeAsInt))
        {
            //Debug.Log("Wall full : " + binaryType);
            tile = wallFull;
        }

        if (tile != null)
        {
            //Debug.Log("ON PRINT : " + binaryType);
            PaintSingleTile(wallTilemap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        //Convert the position of the tile to the position of the world
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    internal void PaintSingleCornerWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;

        if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeAsInt))
        {
            tile = wallInnerCornerDownLeft;
        }
        else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeAsInt))
        {
            tile = wallInnerCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerDownLeft;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerUpRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerUpLeft;
        }
        else if (WallTypesHelper.wallFullEightDirections.Contains(typeAsInt))
        {
            tile = wallFull;
        }
        else if (WallTypesHelper.wallBottmEightDirections.Contains(typeAsInt))
        {
            tile = wallBottom;
        }

        if (tile != null)
        {
            PaintSingleTile(wallTilemap, tile, position);
        }
    }
}
