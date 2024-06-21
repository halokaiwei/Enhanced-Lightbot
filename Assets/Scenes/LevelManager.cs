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
using Game.UI;


public class LevelManager : MonoBehaviour
    {
        dataToSave datatosave;
        DatabaseReference dbRef;
        public static LevelManager Instance;
        public string username;
        public string redirectUrl;
        public TMP_Text textTimeCount;
        public TMP_Text completedBefore;
        public float startTime;

            private void Awake()
            {
                startTime = Time.time;
                Debug.Log("Run Level Manager Awake()" + username); //checking
                if (Instance != null)
                {
                    Debug.Log("Destroyed"); //checking
                    Destroy(gameObject);
                    return;
                }
                Instance = this;
                username = PlayerPrefs.GetString("Username", "");
                Debug.Log("Loaded username: " + username); //checking
                dbRef = FirebaseDatabase.DefaultInstance.RootReference;
                datatosave = new dataToSave();
                datatosave.achievements = new List<String>();
            }

            public void Update() {
                UpdateTimeUsed();
                DisplayTime();
            }

            public void updateInDatabase() {
                Debug.Log("Update In Database"); //checking

                int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
                if (nextLevelIndex < SceneManager.sceneCountInBuildSettings)
                {
                    Debug.Log("nextLevelIndex < SceneManager.sceneCountInBuildSettings"); //checking
                        //checking
                        Debug.Log("UpdateUsername " + username);
                        //levelTime.UpdateUsername(username);
                        Debug.Log("UpdateTimeUsed");
                        UpdateTimeUsed(); // Update the time before saving
                        Debug.Log("CheckAchievements");
                        CheckAchievements();
                        Debug.Log("UpdateTimeUsedDatabase");
                        UpdateTimeUsedToDatabase();
                        Debug.Log("SetCompletedLevels");
                        SetCompletedLevels(nextLevelIndex-1); // Set the current level in LevelTime
                        Debug.Log("SetCompletedLevel to"+(nextLevelIndex));
                }
            }

        public void SetCompletedLevels(int completedLevel)
        {
        bool updated = false;
        bool completed=false;
        DatabaseReference completedLevelsRef = dbRef.Child("users").Child(username).Child("completedLevels");
        completedLevelsRef.RunTransaction(transaction =>
        {
            if (transaction.Value != null)
            {
                //completedLevels from database change to List type
                List<object> completedLevelsData = transaction.Value as List<object>;
                if (completedLevelsData != null)
                {
                    for (int i = 0; i < completedLevelsData.Count; i++)
                    {
                        // 在这里处理每个元素
                        if(int.TryParse(completedLevelsData[i].ToString(), out int level)) {
                            Debug.Log("Parsed");
                            if (level == completedLevel)
                            {
                                completed=true;
                                completedBefore.text = "This level had completed before.";
                                return TransactionResult.Success(transaction); // Don't update if already completed
                            }
                        }
                    }
                }
                //if it does not have, initilize it
                if (completedLevelsData == null) {
                    completedLevelsData = new List<object>();
                }
                if (!completed) {
                    updated = true;
                    completedLevelsData.Add(completedLevel);
                    transaction.Value = completedLevelsData;
                }
            }
            return TransactionResult.Success(transaction);
        }).ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                if (updated) {
                    Debug.Log($"Level {completedLevel} had updated in database successfully");
                }
            }
            else
            {
                Debug.Log("Failed to access database");
            }
        });
    }

        public void UpdateTimeUsed() {
            float elapsedTime = Time.time - startTime;
            datatosave.time = elapsedTime;
        }

        private string FormatTime(float timeInSeconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(timeInSeconds);
            return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        }

        public void DisplayTime() {
            textTimeCount.text = FormatTime(datatosave.time);
        }

        public void UpdateTimeUsedToDatabase()
        {
            DatabaseReference userRef = dbRef.Child("users").Child(username);
            userRef.Child("time").SetValueAsync(datatosave.time);
            Debug.Log($"Updating time data for user: {username}, time: {datatosave.time}");
        }

    public void CheckAchievements() {
        if (datatosave.time <5)
        {
            UnlockAchievement("Speed Demon");
        }
    }
        private void UnlockAchievement(string achievementName) {
        datatosave.achievements.Add(achievementName);
        DatabaseReference achievementsRef = dbRef.Child("users").Child(username).Child("achievements");

        achievementsRef.RunTransaction(transaction =>
        {
            if (transaction.Value != null)
            {
                //completedLevels from database change to List type
                List<object> achievementsData = transaction.Value as List<object>;
                
                //Level will repeat in database 
                if (achievementsData == null) {
                    achievementsData = new List<object>();
                }
                
                achievementsData.Add(achievementName);
                transaction.Value = achievementsData;
                return TransactionResult.Success(transaction);
            }
            else
            {
                List<object> achievementsData = new List<object>();
                achievementsData.Add(achievementName);
                transaction.Value = achievementsData;

                return TransactionResult.Success(transaction);
            }
        }).ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log($"Added achivement: {achievementName}");
            }
            else
            {
                Debug.Log("Failed to add achivement.");
            }
        });
    }
            public void LoadNextLevel()
            {
                int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
                SceneManager.LoadScene(nextLevelIndex);
            }
            
            public void redirectTo() {
                redirectUrl = "http://127.0.0.1:5000/";
                Application.OpenURL(redirectUrl);
                LoadNextLevel();
            }

            // public void SetUsername(string _username) {
            //     username = _username;
            //     Debug.Log("The levelManager userID had changed to "+username);
            // }

            public void LoadLevel(int index)
            {
                SceneManager.LoadScene(index);
            }

            public void RestartLevel()
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            public void QuitGame()
            {
                Application.Quit();
            }
    }