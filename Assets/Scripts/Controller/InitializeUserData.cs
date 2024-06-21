using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Firebase.Database;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Extensions;

public class InitializeUserData : MonoBehaviour
{
    public dataToSave datatosave;
    DatabaseReference dbRef;
    private void Awake()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public void InitializeuserData(string userName) {
        datatosave = new dataToSave
        {
            userID = userName,
            completedLevels = new List<int>{0},
            achievements = new List<String>{"0"},
            time = 0,
        };
         Dictionary<string, object> initialData = new Dictionary<string, object>
        {
            { "userID", datatosave.userID },
            { "completedLevels", datatosave.completedLevels },
            { "achievements", datatosave.achievements },
            { "time", datatosave.time }
        };
        dbRef.Child("users").Child(datatosave.userID).SetValueAsync(initialData);
    }
}
