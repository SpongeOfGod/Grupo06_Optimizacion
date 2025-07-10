using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : ManagedUpdateBehaviourNoMono
{
    RectTransform rectTransform;
    public int index = 1;
    public int amountOfButtons = 3;

    public override void UpdateMe()
    {
        base.UpdateMe();

        var pos = GameManager.Instance.ButtonSelectGO.transform.position;

        GameManager.Instance.ButtonSelectGO.transform.position = new Vector3(pos.x, GameManager.Instance.GameOverButtonsList[index].transform.position.y, pos.z);

        if (Input.GetKeyDown(KeyCode.DownArrow) && index + 1 < amountOfButtons)
        {
            index++;
            GameManager.Instance.PlayAudio(GameManager.Instance.SelectClip);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && index - 1 >= 0)
        {
            index--;
            GameManager.Instance.PlayAudio(GameManager.Instance.SelectClip);
        }

        if (rectTransform == null)
            rectTransform = GameManager.Instance.ButtonSelectGO.GetComponent<RectTransform>();

#if UNITY_EDITOR
        Debug.Log("entrando");
#endif

        switch (index)
        {
            case 1:
                if (Input.GetKeyDown(KeyCode.Return))
                    SceneManager.LoadScene("Gameplay");

                rectTransform.anchoredPosition = new Vector3(1256, -682, 0);

                break;

            case 2:
                if (Input.GetKeyDown(KeyCode.Return))
                    SceneManager.LoadScene("MainMenu");

                rectTransform.anchoredPosition = new Vector3(1256, -800, 0);
                break;

            case 3:
                if (Input.GetKeyDown(KeyCode.Return))
                    Application.Quit();

                rectTransform.anchoredPosition = new Vector3(1256, -931, 0);
                break;
        }
    }
}
