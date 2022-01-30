// GameDev.tv Challenge Club. Got questions or want to share your nifty solution?
// Head over to - http://community.gamedev.tv

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateEnvironment : MonoBehaviour
{
    // configs
    [Header("Grid")]
    [SerializeField, Range(5, 1000)] int minGridSize = 5;
    [SerializeField, Range(5, 1000)] int maxGridSize = 20;
    [Header("Tiles")]
    [SerializeField] GameObject groundTilePrefab;
    [SerializeField] Sprite[] groundSprites;
    [Header("Rocks")]
    [SerializeField] GameObject rockTilePrefab;
    [SerializeField] Sprite[] rockSprites;
    [SerializeField, Range(0, 100)] int percentOfGridToBeRocks = 40;
    [Header("Food")]
    [SerializeField] GameObject foodPrefab;
    [SerializeField] Sprite[] foodSprites;
    [SerializeField, Range(0, 100)] int percentOfGridToBeFood = 10;
    [Header("Exits")]
    [SerializeField] GameObject exitPrefab;
    [SerializeField, Range(1, 4)] int amountOfExits = 1;
    [Header("Enemies")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] int amountOfEnemies;
    [SerializeField] int closestSpawnDistanceFromPlayer = 2;

    // cache
    Transform player;

    // state
    int gridSize;
    Dictionary<Vector2, GroundTile> grid = new Dictionary<Vector2, GroundTile>();

    // Unity messages
    void Awake()
    {
        player = FindObjectOfType<PlayerMovement>().transform;
        gridSize = Random.Range(minGridSize, maxGridSize + 1);
        player.transform.position = new Vector2(gridSize / 2, gridSize / 2);
    }

    void Start()
    {
        CheckForValidGrid();
        GenerateFloor();
        SpawnExits();
        GenerateObjects(rockTilePrefab, rockSprites, percentOfGridToBeRocks);
        GenerateObjects(foodPrefab, foodSprites, percentOfGridToBeFood);        
        SpawnEnemies();
    }

    // private methods
    void CheckForValidGrid()
    {
        int percentOccupied = percentOfGridToBeRocks + percentOfGridToBeFood;

        if (Mathf.Pow(gridSize, 2) * (1 - percentOccupied / 100) < amountOfEnemies + amountOfExits + 1)
        {
            print("Grid is invalid. Needs to be enough spaces to fit specified player, rocks, food, enemies, and exits.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    void GenerateFloor()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                InstantiateFloorTile(x, y);
            }
        }
        SetPlayerTileOccupied();
    }



    void InstantiateFloorTile(int x, int y)
    {
        Vector2 position = new Vector2(x, y);
        GameObject tileInstance = Instantiate(groundTilePrefab, position, Quaternion.identity, this.transform);
        tileInstance.GetComponent<SpriteRenderer>().sprite = groundSprites[Random.Range(0, groundSprites.Length)];
        grid.Add(position, tileInstance.GetComponent<GroundTile>());
    }

    void SetPlayerTileOccupied()
    {
        int playerXRoundedDown = (int)player.position.x;
        int playerYRoundedDown = (int)player.position.y;
        Vector2 playerPositionRoundedDown = new Vector2(playerXRoundedDown, playerYRoundedDown);
        grid[playerPositionRoundedDown].Occupied = true;
    }

    void GenerateObjects(GameObject prefabToSpawn, Sprite[] possibleSprites, int percentOfGridToUse)
    {
        int numberToSpawn = (int)Mathf.Pow(gridSize, 2) * percentOfGridToUse / 100;

        for (int i = 0; i < numberToSpawn; i++)
        {
            Vector2 spawnPosition = GetRandomGridPosition();
            while (grid[spawnPosition].Occupied)
            {
                spawnPosition = GetRandomGridPosition();
            }

            GameObject instance = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity, this.transform);
            instance.GetComponent<SpriteRenderer>().sprite = possibleSprites[Random.Range(0, possibleSprites.Length)];
            grid[spawnPosition].Occupied = true;
        }
    }

    void SpawnExits()
    {
        for (int i = 0; i < amountOfExits; i++)
        {
            Vector2 spawnPosition = GetRandomCorner();
            while (grid[spawnPosition].Occupied)
            {
                spawnPosition = GetRandomCorner();
            }

            Instantiate(exitPrefab, spawnPosition, Quaternion.identity, this.transform);
            grid[spawnPosition].Occupied = true;
        }        
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < amountOfEnemies; i++)
        {
            Vector2 spawnPosition = GetRandomGridPosition();

            while (Vector2.Distance(player.position, spawnPosition) < closestSpawnDistanceFromPlayer ||
                grid[spawnPosition].Occupied)
            {
                spawnPosition = GetRandomGridPosition();
            }

            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    Vector2 GetRandomGridPosition()
    {
        int randomX = Random.Range(0, gridSize);
        int randomY = Random.Range(0, gridSize);
        return new Vector2(randomX, randomY);
    }

    Vector2 GetRandomCorner()
    {
        Vector2 bottomLeft = new Vector2(0, 0);
        Vector2 topLeft = new Vector2(0, gridSize - 1);
        Vector2 bottomRight = new Vector2(gridSize - 1, 0);
        Vector2 topRight = new Vector2(gridSize - 1, gridSize - 1);

        Vector2[] corners = new Vector2[]
            {
                bottomLeft,
                topLeft,
                bottomRight,
                topRight
            };

        return corners[Random.Range(0, corners.Length)];
    }
}
