using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyButton : MonoBehaviour
{
    Button button_;
    Text text_;

    void Start()
    {
        button_ = GetComponent<Button>();
        if (GameManager.Instance())
        {
            GameManager.Instance().SetReadyButton(this);
            button_.onClick.AddListener(OnReadyClick);
        }

        text_ = GetComponentInChildren<Text>();
        text_.text = "READY";
    }

    void OnReadyClick()
    {
        // CABMIAR COLOR ETC
        GameManager.Instance().OnReadyClick();
    }

    public void OnStateChanged(GameManager.GameState state)
    {
        button_.interactable = true;
        switch (state)
        {
            case GameManager.GameState.PREPARING:
                text_.text = "READY";
                break;
            case GameManager.GameState.SELECTING:
                text_.text = "ATTACK";
                break;
            case GameManager.GameState.ATTACKING:
                text_.text = "ATTACKING";
                button_.interactable = false;
                break;
            default:
                break;
        }
    }
}
