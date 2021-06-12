using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleShip : MonoBehaviour
{
    [SerializeField]
    int size = 2;

    public bool grabbed = false, horizontal = true;

    Vector3 startPosition_;

    [SerializeField]
    Image img_;

    RectTransform tr_;

    Vector2Int placedPosition_;

    private void Start()
    {
        tr_ = GetComponent<RectTransform>();
        startPosition_ = transform.position;
        placedPosition_ = new Vector2Int(-1, -1);
    }

    private void Update()
    {
        if (grabbed)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameManager.Instance().PlayerManager().ReleaseBattleShip(this);
                grabbed = false;
                img_.raycastTarget = true;
            }
            else if (Input.GetMouseButtonDown(1))
                RotateShip();
        }
    }

    private void RotateShip()
    {
        horizontal = !horizontal;

        tr_.pivot = new Vector2(0, horizontal ? 1 : 0);

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, horizontal ? 0 : -90));
    }

    public void ResetPosition()
    {
        transform.position = startPosition_;
        placedPosition_ = new Vector2Int(-1, -1);
        if (!horizontal)
            RotateShip();
    }

    public void SelectBattleShip()
    {
        if (grabbed)
            return;
        else if (GameManager.Instance().PlayerManager().SelectBattleShip(this))
        {
            grabbed = true;
            img_.raycastTarget = false;
        }
    }

    public Vector2Int PlacedPosition()
    {
        return placedPosition_;
    }

    public void SetPlacedPosition(int x, int y)
    {
        if (placedPosition_.x != x || placedPosition_.y != y)
            placedPosition_ = new Vector2Int(x, y);
    }

    public int GetSize()
    {
        return size;
    }
}
