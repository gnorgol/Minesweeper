using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public int width; // Largeur de la grille
    public int height; // Hauteur de la grille
    public int mineCount; // Nombre de mines
    public GameObject tilePrefab; // Préfabriqué pour les cases
    public Camera mainCamera; // Caméra principale


    private Tile[,] grid; // Grille de tuiles
    private List<Tile> allTiles = new List<Tile>();
    //Background
    private GameObject Background;

    public static GridManager Instance { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        GenerateGrid();
        PlaceMines();
        CalculateAdjacentMines();
        CreateBackground();
        //Set camera position
        mainCamera.transform.position = new Vector3(width / 2, height / 2, -10);
        //Set camera size
        mainCamera.orthographicSize = height / 2 + 1;

    }

    private void CreateBackground()
    {
        //Create plane 
        Background = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Background.transform.position = new Vector3(width / 2, height / 2, 0);
        Background.transform.localScale = new Vector3(width, height, 1);
        Background.transform.rotation = Quaternion.Euler(-90, 0, 0);
        Background.GetComponent<Renderer>().material.color = Color.gray;
    }

    // Génère la grille de tuiles
    void GenerateGrid()
    {
        grid = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject tileObj = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                Tile tile = tileObj.GetComponent<Tile>();
                tile.SetCoordinates(x, y);
                grid[x, y] = tile;
                allTiles.Add(tile);
            }
        }
    }

    // Place des mines de manière aléatoire sur la grille
    void PlaceMines()
    {
        for (int i = 0; i < mineCount; i++)
        {
            int randomIndex = Random.Range(0, allTiles.Count);
            Tile randomTile = allTiles[randomIndex];
            if (!randomTile.isMine)
            {
                randomTile.SetMine();
            }
            else
            {
                i--; // Si la case a déjà une mine, on réessaie
            }
        }
    }

    // Calcule le nombre de mines adjacentes pour chaque case
    void CalculateAdjacentMines()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = grid[x, y];
                if (!tile.isMine)
                {
                    tile.adjacentMines = GetAdjacentMineCount(x, y);
                }
            }
        }
    }

    // Compte les mines adjacentes à une case donnée
    int GetAdjacentMineCount(int x, int y)
    {
        int count = 0;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int newX = x + dx;
                int newY = y + dy;

                if (newX >= 0 && newY >= 0 && newX < width && newY < height)
                {
                    if (grid[newX, newY].isMine)
                    {
                        count++;
                    }
                }
            }
        }
        return count;
    }
    public void RevealAdjacentTiles(int x, int y)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int newX = x + dx;
                int newY = y + dy;

                if (newX >= 0 && newY >= 0 && newX < width && newY < height)
                {
                    Tile tile = grid[newX, newY];
                    if (!tile.isRevealed)
                    {
                        tile.Reveal();
                    }
                }
            }
        }
    }
}
