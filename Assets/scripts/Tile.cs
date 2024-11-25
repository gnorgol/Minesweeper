using UnityEngine;
using TMPro;
public class Tile : MonoBehaviour
{
    public bool isMine; // Indique si cette tuile contient une mine
    public int adjacentMines; // Nombre de mines adjacentes
    //Bombe sprite
    public Sprite mineSprite;
    public Sprite flagSprite;
    public bool isRevealed = false; // Indique si cette tuile a été révélée

    //
    private SpriteRenderer spriteRendererMine;
    private SpriteRenderer spriteRendererFlag;
    private TextMeshPro textMesh;


    private int x, y; // Coordonnées de la tuile
    private bool isFlagged;

    // Attribuer des coordonnées à la tuile
    public void SetCoordinates(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    void Start()
    {
        if (isMine) {
            //Debug.Log("Mine at " + x + ", " + y);
            //create sprite object
            GameObject spriteObject = new GameObject("Sprite");
            spriteObject.transform.SetParent(this.transform);
            spriteObject.transform.localPosition = new Vector3(0, 0, -0.001f);
            spriteObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            spriteObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            spriteRendererMine = spriteObject.AddComponent<SpriteRenderer>();
            spriteRendererMine.sprite = mineSprite;
            spriteRendererMine.enabled = false;
            spriteRendererMine.sortingOrder = 0;
        }
        else
        {
            //Debug.Log("No mine at " + x + ", " + y);
            //create text mesh pro object
            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(this.transform);
            textObject.transform.localPosition = new Vector3(0, 0, -0.001f);
            textObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            textObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            textMesh = textObject.AddComponent<TextMeshPro>();
            textMesh.text = adjacentMines.ToString();
            textMesh.fontSize = 32;
            textMesh.alignment = TextAlignmentOptions.Center;
            switch (adjacentMines)
            {
                case 0:
                    textMesh.text = "";
                    break;
                case 1:
                    textMesh.color = Color.blue;
                    break;
                case 2:
                    textMesh.color = Color.green;
                    break;
                default:
                    textMesh.color = Color.red;
                    break;
            }
            textMesh.enabled = false;
        }
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }

    }

    // Marquer la tuile comme contenant une mine
    public void SetMine()
    {
        isMine = true;
    }

    // Révéler le contenu de la tuile (appelé quand le joueur clique dessus)
    public void Reveal(bool isRecursive = false)
    {
        if (isRevealed)
        {
            return;
        }
        isRevealed = true;
        if (isMine)
        {
            // Révéler la mine (activer le sprite de la mine)
            spriteRendererMine.enabled = true;
            Debug.Log("Mine! Game Over.");
        }
        else
        {
            // Si la case n'est pas une mine, afficher le texte correspondant au nombre de mines adjacentes
            textMesh.enabled = true;

            // Optionnel: si la tuile n'a aucune mine adjacente, tu peux automatiquement révéler ses voisins
            if (adjacentMines == 0 && !isMine )
            {
                //Set the tile to color green
                GetComponent<Renderer>().material.color = Color.green;

                // Appelle une fonction du GridManager pour révéler les cases adjacentes (à implémenter)
                GridManager.Instance.RevealAdjacentTiles(x, y);
            }
        }
    }
    public void CreateFlag()
    {
        if (isRevealed)
        {
            return;
        }
        isFlagged = !isFlagged;
        if (isFlagged)
        {
            // Créer un sprite de drapeau
            GameObject spriteObject = new GameObject("Flag");
            spriteObject.transform.SetParent(this.transform);
            spriteObject.transform.localPosition = new Vector3(0, 0, -0.001f);
            spriteObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            spriteObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            spriteRendererFlag = spriteObject.AddComponent<SpriteRenderer>();
            spriteRendererFlag.sprite = flagSprite;
            spriteRendererFlag.enabled = true;
            spriteRendererFlag.sortingOrder = 0;

            GridManager.Instance.increaseFlagCount();
        }
        else
        {
            // Supprimer le sprite de drapeau
            Destroy(spriteRendererFlag.gameObject);
            GridManager.Instance.decreaseFlagCount();
        }
    }


    // Lorsque la tuile est cliquée
    private void OnMouseOver()
    {

        if (Input.GetMouseButtonDown(0)) // Clic gauche pour révéler
        {
            Debug.Log("Left click detected on tile at (" + x + ", " + y + ")");
            Reveal();
        }
        else if (Input.GetMouseButtonDown(1)) // Clic droit pour poser un drapeau
        {
            Debug.Log("Left click detected on tile at (" + x + ", " + y + ")");
            CreateFlag();
        }
    }
}
