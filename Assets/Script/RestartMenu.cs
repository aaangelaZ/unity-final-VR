using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartMenu : MonoBehaviour
{
    public void restartBtn()
    {
        SceneManager.LoadScene("StartScene");
    }

}
