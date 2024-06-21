using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SendHttpRequest : MonoBehaviour
{
    public string url = "http://127.0.0.1:5080/";

    public void sendHttpRequest()
    {
        StartCoroutine(SendRequest());
    }

    IEnumerator SendRequest()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // 发送请求并等待响应
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // 请求成功，处理响应数据
                string output = webRequest.downloadHandler.text;
                Debug.Log("Received response: " + output);
            }
            else
            {
                // 请求失败，输出错误信息
                Debug.Log("Error: " + webRequest.error);
            }
        }
    }
}
