using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneButton : MonoBehaviour, IPointerClickHandler
{
    public string SceneName;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (SceneName == "Exit")
            Application.Quit();
        else
            SceneManager.LoadScene(SceneName);
    }
}
