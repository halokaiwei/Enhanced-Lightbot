using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameClick : MonoBehaviour
{
    public void OnButtonClick()
    {
        // Load the Main_Menu scene
        SceneManager.LoadScene("Main_Menu");
    }
}
