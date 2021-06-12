using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGridPosition : GridObject
{

    public void OnHover()
    {
        GameManager.Instance().PlayerManager().OnGridHover(this);
    }

    public void OnHoverExit()
    {
        GameManager.Instance().PlayerManager().OnGridHoverExit(this);
    }
}
