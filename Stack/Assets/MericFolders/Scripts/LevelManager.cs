using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    
    public GameObject winPopUps;
    EndGame endgame;
    bool �ncreased;
    public TextMeshProUGUI textLevel;
    bool IsEnded;
    SceneManagers scenemanagers;
    UIManager uimanager;


    void Start()
    {
        uimanager = GameObject.FindObjectOfType<UIManager>();
        scenemanagers = GameObject.FindObjectOfType<SceneManagers>();
        endgame = GameObject.FindObjectOfType<EndGame>();
        DisplayProgress();
    }


    void Update()
    {
        //Final();
    }
    void Final()
    {
        if (endgame.IsDead == true && �ncreased == false)
        {
            GameObject objectPopUp = Instantiate(winPopUps, Vector3.zero, Quaternion.Euler(0, 0, 0));
            uimanager.LevelCompleted();
            Object.Destroy(objectPopUp, 15);            
            �ncreased = true;
            endgame.IsDead = false;
            IsEnded = true;
            StartCoroutine(Nextlevel());

        }
    }
    void DisplayProgress()
    {
        textLevel.text = "LEVEL " + scenemanagers.gameProgress;
    }
    IEnumerator Nextlevel()
    {
        yield return new WaitForSeconds(10);
        scenemanagers.LoadNextLevel();
    }

}
