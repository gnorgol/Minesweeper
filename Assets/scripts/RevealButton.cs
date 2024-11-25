using UnityEngine;

public class RevealButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        GridManager.Instance.RevealAllMines();
    }
}
