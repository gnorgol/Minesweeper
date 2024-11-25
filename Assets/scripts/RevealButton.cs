using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        GridManager.Instance.RevealAllMines();
    }
}
