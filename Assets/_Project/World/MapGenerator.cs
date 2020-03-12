using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] Transform player;

    public int width;
    public int height;

    [SerializeField] Tilemap collisionTilemap;
    [SerializeField] RuleTile collisionTile;
    [SerializeField] Tilemap backWallTilemap;
    [SerializeField] Tile backWallTile;
    

    public string seed;
    public bool useRandomSeed;

    [Range(0, 100)]
    public int randomFillPercent;

    int[,] map;

    void Awake()
    {
        GenerateMap();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Insert))
        {
            GenerateMap();
        }
    }

    void GenerateMap()
    {
        ClearTiles();

        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < 5; i++)
        {
            SmoothMap();
        }

        PlaceTiles();

        PlacePlayerSpawnPoint();
    }

    void ClearTiles()
    {
        backWallTilemap.ClearAllTiles();
        collisionTilemap.ClearAllTiles();
    }

    void RandomFillMap()
    {
        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, y] = 0;

            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }


    //void OnDrawGizmos()
    //{
    //    if (map != null)
    //    {
    //        for (int x = 0; x < width; x++)
    //        {
    //            for (int y = 0; y < height; y++)
    //            {
    //                Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
    //                Vector3 pos = new Vector3(-width / 2 + x + .5f, 0, -height / 2 + y + .5f);
    //                Gizmos.DrawCube(pos, Vector3.one);
    //            }
    //        }
    //    }
    //}
   
    void PlaceTiles()
    {
        if (map != null)
        {
            //bool spawnPointFound = false;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3Int pos = new Vector3Int(-width / 2 + x, -height / 2 + y, 0);
                    backWallTilemap.SetTile(pos, backWallTile);
                    if(map[x,y] == 1)
                    {
                        collisionTilemap.SetTile(pos, collisionTile);
                    }
                    //else if (!spawnPointFound)
                    //{
                    //    //place spawn point
                    //    player.position = pos;
                    //    spawnPointFound = true;
                    //}
                }
            }
        }
    }

    void PlacePlayerSpawnPoint()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3Int pos = new Vector3Int(-width / 2 + y, -height / 2 + x, 0);
                if (!collisionTilemap.HasTile(pos))
                {
                    player.position = pos + new Vector3(0.5f, 0.5f);
                    return;
                }
            }
        }
    }

    //void PlaceTiles()
    //{
    //    if (map != null)
    //    {
    //        for (int x = 0; x < width; x++)
    //        {
    //            for (int y = 0; y < height; y++)
    //            {
    //                Tilemap tilemap = (map[x, y] == 1) ? collisionTilemap : backWallTilemap;
    //                Tile tile = (map[x, y] == 1) ? collisionTile : backWallTile;
    //                Vector3Int pos = new Vector3Int(-width / 2 + x, -height / 2 + y, 0);
    //                //Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
    //                tilemap.SetTile(pos, tile);
    //                
    //            }
    //        }
    //    }
    //}
}