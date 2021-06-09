using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleShip : MonoBehaviour
{
    [SerializeField]
    int size = 2;

    bool grabbed = false, horizontal = true;

    Vector3 startPosition_;

    [SerializeField]
    Image img_;

    RectTransform tr_;

    private void Start()
    {
        tr_ = GetComponent<RectTransform>();
        startPosition_ = transform.position;
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
