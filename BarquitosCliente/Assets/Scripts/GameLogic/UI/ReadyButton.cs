using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyButton : MonoBehaviour
{
    Button button_;
    Text text_;
    Image imgCmp_;

    bool ready = false;

    void Start()
    {
        button_ = GetComponent<Button>();
        if (GameManager.Instance())
        {
            GameManager.Instance().SetReadyButton(this);
            button_.onClick.AddListener(OnReadyClick);
        }

        imgCmp_ = GetComponent<Image>();
        text_ = GetComponentInChildren<Text>();
        text_.text = "READY";
    }

    void OnReadyClick()
    {
        ready = !ready;
        imgCmp_.color = ready ? Color.grey : Color.white;
        GameManager.Instance().OnReadyClick();
    }

    public void OnStateChanged(GameManager.GameState state)
    {
        button_.interactable = true;
        ready = false;
        imgCmp_.color = Color.white;
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
            case GameManager.GameState.END:
                text_.text = "return";
                break;
            default:
                break;
        }
    }
}
