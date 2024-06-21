using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject loginUI;
    public GameObject registerUI;
    public GameObject loginSelectUI;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != null) {
            Debug.Log("Instance alrdy exists, destroying object!");
            Destroy(this);
        }
    }

    public void ClearScreen() {
        loginSelectUI.SetActive(false);
        loginUI.SetActive(false);
        registerUI.SetActive(false);
    }
    public void LoginScreen() {
        ClearScreen();
        loginSelectUI.SetActive(true);
    }
    public void signinScreen() {
        ClearScreen();
        loginUI.SetActive(true);
    }
    public void signupScreen() {
         ClearScreen();
        registerUI.SetActive(true);
    }


}
