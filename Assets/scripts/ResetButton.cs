using UnityEngine;

public class ResetButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        GridManager.Instance.ResetGame();
    }
}
