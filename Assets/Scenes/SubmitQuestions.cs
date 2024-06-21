using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SubmitQuestions : MonoBehaviour
{
    public ToggleGroup toggleGroup1;
    public ToggleGroup toggleGroup2;
    public TMP_Text outputText;

    public string url = "http://127.0.0.1:5080/";
    public void Submit()
    {
        string selectedValueInterest = GetSelectedToggleValue(toggleGroup1);
        string selectedValueLearningMode = GetSelectedToggleValue(toggleGroup2);
        sendHttpRequest(selectedValueInterest, selectedValueLearningMode);
    }

    private string GetSelectedToggleValue(ToggleGroup toggleGroup)
    {
        string selectedValue = "";
        foreach (Toggle toggle in toggleGroup.ActiveToggles())
        {
            selectedValue += toggle.GetComponentInChildren<TMP_Text>().text + " ";
        }
        if (selectedValue == "")
        {
            return "None";
        }
        else
        {
            return selectedValue.Trim();
        }
        return "None";
    }

    public void sendHttpRequest(string interest, string learningMode)
    {
        string fullUrl = url + "?interest=" + WWW.EscapeURL(interest) + 
        "&learningMode=" + WWW.EscapeURL(learningMode);
        StartCoroutine(SendRequest(fullUrl));
    }

    IEnumerator SendRequest(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // 发送请求并等待响应
            outputText.text="Loading...";
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // 请求成功，处理响应数据
                string output = webRequest.downloadHandler.text;
                outputText.text=output;
            }
            else
            {
                // 请求失败，输出错误信息
                Debug.LogWarning("Error: " + webRequest.error);
            }
        }
    }
}
