using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width; // Largeur de la grille
    public int height; // Hauteur de la grille
    public int mineCount; // Nombre de mines
    public GameObject tilePrefab; // Préfabriqué pour les cases
    public Camera mainCamera; // Caméra principale


    private Tile[,] grid; // Grille de tuiles
    private List<Tile> allTiles = new List<Tile>();

    public static GridManager Instance { get; private set; }

    //Timer variables 
    public float timer;
    //Timer object text
    private TMPro.TextMeshPro timerText;

    //Flag count
    public int flagCount;
    //Flag object text
    private TMPro.TextMeshPro flagText;

    // Reset button
    public GameObject resetButton;

    public GameObject RevealButton;



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
        CreateTimer();
        CreateFlagsText();
        CreateResetButton();
        CreateRevealButton();

        //Set camera position
        mainCamera.transform.position = new Vector3(width / 2, height / 2, -10);
        //Set camera size
        mainCamera.orthographicSize = height / 2 + 1;
    }
    void Update()
    {
        //Update timer only seconds format 000
        timerText.text = ((int)(Time.time - timer)).ToString("000");
    }

    private void CreateResetButton()
    {
        resetButton = Instantiate(resetButton);
        resetButton.transform.position = new Vector3(width / 2, height , 0);

        //Add collider to reset button
        resetButton.AddComponent<BoxCollider2D>();

    }
    private void CreateRevealButton()
    {
        RevealButton = Instantiate(RevealButton);
        // Position a gauche de la grid
        RevealButton.transform.position = new Vector3(width, 0,0);

        RevealButton.AddComponent<BoxCollider2D>();

    }

    public void increaseFlagCount()
    {
        flagCount++;
        flagText.text = flagCount.ToString();
    }
    public void decreaseFlagCount() {
        flagCount--;
        flagText.text = flagCount.ToString();
    }
    private void CreateFlagsText()
    {
        flagText = new GameObject("Flags").AddComponent<TMPro.TextMeshPro>();
        flagText.transform.position = new Vector3(width / 2 + 3, height, 0);
        flagText.text = flagCount.ToString();
        flagText.fontSize = 8;
        flagText.alignment = TMPro.TextAlignmentOptions.Center;
        flagText.color = Color.red;
        flagText.enabled = true;
    }

    private void CreateTimer()
    {
        timer = Time.time;
        timerText = new GameObject("Timer").AddComponent<TMPro.TextMeshPro>();
        timerText.transform.position = new Vector3(width / 2 - 3, height , 0);
        timerText.text = "0";
        timerText.fontSize = 8;
        timerText.alignment = TMPro.TextAlignmentOptions.Center;
        timerText.color = Color.red;
        timerText.enabled = true;

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
    public void ResetGame()
    {
        Debug.Log("Reset game");
        //reset timer Flag count and tiles and create new grid
        timer = Time.time;
        flagCount = 0;
        flagText.text = flagCount.ToString();
        timerText.text = "0";

        //Destroy all tiles
        foreach (Tile tile in allTiles)
        {
            Destroy(tile.gameObject);
        }
        allTiles.Clear();

        //Create new grid
        GenerateGrid();
        PlaceMines();
        CalculateAdjacentMines();
    }

    public void RevealAllMines()
    {
        foreach (Tile tile in allTiles)
        {
            if (tile.isMine)
            {
                tile.Reveal();
            }
        }
    }
}
