using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneManagers : MonoBehaviour
{
    public int gameProgress = 1;
    bool IsStarted;
    void Start()
    {
        DontDestroyOnLoad(this.gameObject); 
    }

    void Update()
    {
        
    }
    //void Load()
    //{
    //    switch (gameProgress)
    //    {
    //        case (gameProgress == 0):
    //            SceneManager.LoadScene("Scene01");
    //            break;
    //        case 2:
    //            SceneManager.LoadScene("Scene02");
    //            break;
    //        case 3:
    //            SceneManager.LoadScene("Scene03");
    //            break;
    //        case 4:
    //            SceneManager.LoadScene("Scene04");
    //            break;
    //        case 5:
    //            SceneManager.LoadScene("Scene05");
    //            break;
    //        case 6:
    //            SceneManager.LoadScene("Scene06");
    //            break;
    //        case 7:
    //            SceneManager.LoadScene("Scene07");
    //            break;
    //        case 8:
    //            SceneManager.LoadScene("Scene08");
    //            break;
    //        case 9:
    //            SceneManager.LoadScene("Scene09");
    //            break;
    //        case 10:
    //            SceneManager.LoadScene("Scene10");
    //            break;
    //    }
    //}
    public void LoadNextLevel()
    {
        
            if (gameProgress == 1)
            {
                Debug.Log("Level 2 Yükleniyor");
                SceneManager.LoadScene(1);
                gameProgress++;
            }
            else if (gameProgress == 2)
            {
                Debug.Log("Level 3 Yükleniyor");
                SceneManager.LoadScene(2);
                gameProgress++;
            }
            else if (gameProgress == 3)
            {
                Debug.Log("Level 4 Yükleniyor");
                SceneManager.LoadScene(3);
                gameProgress++;
            }
            else if (gameProgress == 4)
            {
                SceneManager.LoadScene(4);
            }    
    }
}

