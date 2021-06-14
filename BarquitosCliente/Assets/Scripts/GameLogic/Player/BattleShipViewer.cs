using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleShipViewer : MonoBehaviour
{
    [SerializeField]
    int size = 2;

    public bool grabbed = false;

    Vector3 startPosition_;
    [SerializeField]
    Image img_;

    RectTransform tr_;

    BattleShip btShip_;

    private void Start()
    {
        tr_ = GetComponent<RectTransform>();
        startPosition_ = transform.position;
        btShip_ = new BattleShip(size);
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
        btShip_.horizontal = !btShip_.horizontal;

        tr_.pivot = new Vector2(0, btShip_.horizontal ? 1 : 0);

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, btShip_.horizontal ? 0 : -90));
    }

    public BattleShip BattleShip()
    {
        return btShip_;
    }

    public void ResetPosition()
    {
        transform.position = startPosition_;
        btShip_.PlacedPositions().Clear();
        if (!btShip_.horizontal)
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
}
