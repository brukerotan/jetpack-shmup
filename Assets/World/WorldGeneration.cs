using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Grid))]
public class WorldGeneration : MonoBehaviour
{
    Vector2Int walkableWorldSize = new Vector2Int(25, 50);
    Vector2Int totalWorldSize = new Vector2Int(26, 51);

    [SerializeField] Tilemap collisionTilemap;
    [SerializeField] Tile collisionTile;
    [SerializeField] Tilemap backWallTilemap;
    [SerializeField] Tile backWallTile;

    // Start is called before the first frame update
    void Awake()
    {
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            Generate();
        }
    }

    void Generate()
    {
        collisionTilemap.ClearAllTiles();
        backWallTilemap.ClearAllTiles();

        //Generate Back Walls
        for (int y = 0; y <= walkableWorldSize.y; y++)
        {
            for (int x = 0; x <= walkableWorldSize.x; x++)
            {
                float numberNeededToTile = 25;
                if(Random.Range(0f, 255f) > numberNeededToTile)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    backWallTilemap.SetTile(tilePosition, backWallTile);
                }
            }
        }

        //Generate Walls
        for (int y = 0; y < totalWorldSize.y; y++)
        {
            for (int x = 0; x < totalWorldSize.x; x++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                if (!backWallTilemap.HasTile(tilePosition))
                {
                    Debug.Log("No tile");
                    collisionTilemap.SetTile(tilePosition, collisionTile);
                }
            }
        }
    }
}
