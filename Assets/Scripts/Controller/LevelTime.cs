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

    public class User
    {
        public string userID;
        public float time;
        public List<int> completedLevels;
    }
[Serializable]
public class dataToSave
{
    [SerializeField] public string userID;
    [SerializeField] public List<int> completedLevels;
    [SerializeField] public List<string> achievements;
    [SerializeField] public float time;
}

public class LevelTime : MonoBehaviour
{
    public static LevelTime Instance; // Singleton pattern
    public dataToSave datatosave;

    [Header("RankData")]
    public TMP_Text UserID1;
    public TMP_Text UserCompletedLevels1;
    public TMP_Text UserTime1;
    public TMP_Text UserID2;
    public TMP_Text UserCompletedLevels2;
    public TMP_Text UserTime2;
    public TMP_Text UserID3;
    public TMP_Text UserCompletedLevels3;
    public TMP_Text UserTime3;
    public TMP_Text UserID4;
    public TMP_Text UserCompletedLevels4;
    public TMP_Text UserTime4;
    public TMP_Text UserID5;
    public TMP_Text UserCompletedLevels5;
    public TMP_Text UserTime5;

    // public TMP_Text completedLevelsField;
    // public TMP_Text achievementsField;
    List<User> userDataList = new List<User>();
    public string userName;
    public string email;
    public string completedLevels;
    public string achievements;
    public float time;

    public TMP_Text username;
    public TMP_Text userEmail;
    public TMP_Text textLevelsCompleted;
    public TMP_Text textAchievements;
    public TMP_Text textAvgTime;
    DatabaseReference dbRef;

    //Level repeated storing in database
    //Time calculate average
    //Achievements
    //The levelTime in level 1,2,3... scenes 
    // public float startTime;

    private void Awake()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        // startTime = Time.time;
        if(PlayerPrefs.HasKey("Type")) {
            if (PlayerPrefs.GetString("Type") == "signin") {
                if (PlayerPrefs.HasKey("Username"))
                {
                    userName = PlayerPrefs.GetString("Username");
                    Debug.Log("UserName loaded from PlayerPrefs: " + userName);  //checking
                    username.text = userName;
                }
                else
                {
                    Debug.LogWarning("No username found in PlayerPrefs."); //checking
                }
                if (PlayerPrefs.HasKey("Email"))
                {
                    email = PlayerPrefs.GetString("Email");
                    Debug.Log("Email loaded from PlayerPrefs: "+ email);
                    userEmail.text = email;
                }
                FindUserData();
                ProcessLeaderboard();
            }
            else if (PlayerPrefs.GetString("Type")=="guests") {
                textLevelsCompleted.text = "Guest";
                textAchievements.text = "Guest";
                textAvgTime.text = "Guest";
                Debug.Log("Found type=guest");
            }
        }
    }
    public void SignOutButton() {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("AuthScene");
    }


public void FindUserData()
{
    List<int> completedLevelsList = new List<int>();
    dbRef.Child("users").Child(userName).GetValueAsync().ContinueWithOnMainThread(task =>
    {
        if (task.IsFaulted)
        {
            foreach (var exception in task.Exception.InnerExceptions)
            {
                Debug.LogError("Database operation failed: " + exception.Message);
            }
            return;
        }
        if (task.IsCanceled)
        {
            Debug.LogError("Database operation was canceled.");
            return;
        }
        DataSnapshot dataSnapshot = task.Result;
        if (dataSnapshot.Exists)
        {
            dataToSave userData = JsonUtility.FromJson<dataToSave>(dataSnapshot.GetRawJsonValue());
            Dictionary<string, object> dataDict = new Dictionary<string, object>
            {
                { "userID", userData.userID },
                { "completedLevels", userData.completedLevels },
                { "achievements", userData.achievements },
                { "time", userData.time }
            };
            time=userData.time;
            completedLevelsList = userData.completedLevels;
            textLevelsCompleted.text = string.Join(", ", completedLevelsList);
            textAvgTime.text = time.ToString();
        }
        else
        {
            Debug.LogWarning($"User with ID {userName} not found."); //checking
        }
    });
}

    // public void UpdateTimeUsedToDatabase()
    // {
    //     DatabaseReference userRef = dbRef.Child("users").Child(userName);
    //     //Update timeused to database
    //     userRef.Child("time").SetValueAsync(datatosave.time);
    //     Debug.Log($"Updating time data for user: {userName}, time: {datatosave.time}");
    // }

    // public void CheckAchievements() {
    //     if (datatosave.time <5)
    //     {
    //         UnlockAchievement("Speed Demon");
    //     }
        
    // }

    // private void UnlockAchievement(string achievementName) {
    //     Debug.Log($"Achievement Unlocked: {achievementName}!");
    //     datatosave.achievements.Add(achievementName);
    //     Debug.Log($"Added achievement: {achievementName}");
    //     DatabaseReference achievementsRef = dbRef.Child("users").Child(userName).Child("achievements");
    //     Debug.Log("current UnlockAchivement userName is "+userName); //checking

    //     achievementsRef.RunTransaction(transaction =>
    //     {
    //         Debug.Log("Transaction.Value = "+transaction.Value); //checking
    //         if (transaction.Value != null)
    //         {
    //             //completedLevels from database change to List type
    //             List<object> achievementsData = transaction.Value as List<object>;
                
    //             //Level will repeat in database 
    //             if (achievementsData == null) {
    //                 achievementsData = new List<object>();
    //             }
                
    //             achievementsData.Add(achievementName);
    //             transaction.Value = achievementsData;
    //             return TransactionResult.Success(transaction);
    //         }
    //         else
    //         {
    //             List<object> achievementsData = new List<object>();
    //             achievementsData.Add(achievementName);
    //             transaction.Value = achievementsData;

    //             return TransactionResult.Success(transaction);
    //         }
    //     }).ContinueWith(task =>
    //     {
    //         if (task.IsCompleted && !task.IsFaulted)
    //         {
    //             Debug.Log($"Added achivement: {achievementName}");
    //         }
    //         else
    //         {
    //             Debug.Log("Failed to add achivement.");
    //         }
    //     });
    // }

    private int GetUniqueCommandsCount()
    {
        // Implement logic to count the number of unique commands used in a level
        // Return the count
        return 1; // Placeholder, replace with your implementation
    }

    // public void SetCompletedLevels(int completedLevel)
    // {
    //     DatabaseReference completedLevelsRef = dbRef.Child("users").Child(userName).Child("completedLevels");
    //     Debug.Log("current SetCompletedLevels userName is "+userName); //checking

    //     completedLevelsRef.RunTransaction(transaction =>
    //     {
    //         Debug.Log("Transaction.Value = "+transaction.Value); //checking
    //         if (transaction.Value != null)
    //         {
    //             //completedLevels from database change to List type
    //             List<object> completedLevelsData = transaction.Value as List<object>;
    //             if (completedLevelsData != null)
    //             {
    //                 for (int i = 0; i < completedLevelsData.Count; i++)
    //                 {
    //                     // 在这里处理每个元素
    //                     Debug.Log(completedLevelsData[i].ToString());
    //                 }
    //             }
    //             // Check if completedLevel already exists in the list
    //             if (completedLevelsData.Contains(completedLevel))
    //             {
    //                 Debug.Log($"Level {completedLevel} is already completed before.");
    //                 return TransactionResult.Success(transaction); // Don't update if already completed
    //             }
    //             //if it does not have, initilize it
    //             if (completedLevelsData == null) {
    //                 completedLevelsData = new List<object>();
    //             }
    //             completedLevelsData.Add(completedLevel);
    //             transaction.Value = completedLevelsData;
    //             return TransactionResult.Success(transaction);
    //         }
    //         else
    //         {
    //             List<object> completedLevelsData = new List<object>();
    //             completedLevelsData.Add(completedLevel);
    //             transaction.Value = completedLevelsData;

    //             return TransactionResult.Success(transaction);
    //         }
    //     }).ContinueWith(task =>
    //     {
    //         if (task.IsCompleted && !task.IsFaulted)
    //         {
    //             Debug.Log($"Added completed level: {completedLevel}");
    //         }
    //         else
    //         {
    //             Debug.Log("Failed to add completed level.");
    //         }
    //     });

    // }

    // public void UpdateTimeUsed() {
    //     //Debug.Log("UpdateTimeUsed userName:" + userName);
    //     timeUsed = Time.time - startTime;
    //     datatosave.time = timeUsed;
    //     //应该要从第一个command开始算时间
    // }

    // public void DisplayTime() {
    //     textTimeCount.text = timeUsed.ToString("F2");
    // }

public async void ProcessLeaderboard()
{
    try
    {
        DataSnapshot dataSnapshot = await dbRef.Child("users").GetValueAsync();
        if (dataSnapshot != null && dataSnapshot.HasChildren)
        {
            List<Task> tasks = new List<Task>();
            foreach (DataSnapshot userSnapshot in dataSnapshot.Children)
            {
                Debug.Log("UserSnapshot.Key: "+userSnapshot.Key);
                tasks.Add(GetUserData(userSnapshot));
                await Task.WhenAll(tasks);
            }
            await Task.WhenAll(tasks);
        }
    }
    catch (Exception ex)
    {
        Debug.LogError("Error while processing leaderboard: " + ex.Message);
    }
}
private async Task GetUserData(DataSnapshot userSnapshot)
{
    try
    {
        DataSnapshot innerSnapshot = await dbRef.Child("users").Child(userSnapshot.Key).GetValueAsync();
        dataToSave userData = JsonUtility.FromJson<dataToSave>(innerSnapshot.GetRawJsonValue());
        if (innerSnapshot.Exists)
        {
            // Deserialize user data
            User user = new User();
            user.userID = userData.userID;
            user.time = userData.time;
            user.completedLevels = userData.completedLevels;
            userDataList.Add(user);
        }
        else
        {
            Debug.Log($"User with ID {userSnapshot.Key} not found.");
        }
        Debug.Log("hi5");
        userDataList = userDataList
        .Where(u => u.time != 0) 
        .OrderBy(u => u.time)
        .ThenByDescending(u => u.completedLevels.Max())
        .ToList();
        int i=0;
        foreach (User userDataItem in userDataList) {
            if (i==0) {
                UserID1.text = userDataItem.userID;
                UserCompletedLevels1.text = string.Join(", ", userDataItem.completedLevels);
                UserTime1.text = userDataItem.time.ToString();
            }
            else if (i==1) {
                UserID2.text = userDataItem.userID;
                UserCompletedLevels2.text = string.Join(", ", userDataItem.completedLevels);
                UserTime2.text = userDataItem.time.ToString();                
            }
            else if (i==2) {
                UserID3.text = userDataItem.userID;
                UserCompletedLevels3.text = string.Join(", ", userDataItem.completedLevels);
                UserTime3.text = userDataItem.time.ToString();                
            }
            else if (i==3) {
                UserID4.text = userDataItem.userID;
                UserCompletedLevels4.text = string.Join(", ", userDataItem.completedLevels);
                UserTime4.text = userDataItem.time.ToString();                
            }
            else if (i==4) {
                UserID5.text = userDataItem.userID;
                UserCompletedLevels5.text = string.Join(", ", userDataItem.completedLevels);
                UserTime5.text = userDataItem.time.ToString();                
            }
            i++;
        }
    }
    catch (Exception ex)
    {
        Debug.LogError("Error while getting user data: " + ex.Message);
    }
}



        // // Find TMP_Text objects and set their values
        // TMP_Text userIDText = GameObject.Find(userIDTextName)?.GetComponent<TMP_Text>();
        // TMP_Text userCompletedLevelsText = GameObject.Find(userCompletedLevelsTextName)?.GetComponent<TMP_Text>();
        // TMP_Text userTimeText = GameObject.Find(userTimeTextName)?.GetComponent<TMP_Text>();

        // Debug.Log("UserID Text Name: " + userIDTextName);
        // Debug.Log("UserCompletedLevels Text Name: " + userCompletedLevelsTextName);
        // Debug.Log("UserTime Text Name: " + userTimeTextName);

        // if (userIDText != null && userCompletedLevelsText != null && userTimeText != null)
        // {
        //     // Set TMP Text values
        //     userIDText.text = userData.userID;
        //     userCompletedLevelsText.text = "Completed Levels: " + string.Join(", ", userData.completedLevels);
        //     userTimeText.text = "Time: " + userData.time.ToString();
        // }
        // else
        // {
        //     Debug.LogWarning("One or more TMP Text objects not found under 'Rank Canva/1st' hierarchy.");
        // }
    
// public void displayRank() {
//     dbRef.Child("users").GetValueAsync().ContinueWith(task => {
//         if (task.IsFaulted || task.IsCanceled) {
//             Debug.LogError("Failed to retrieve data from the database: " + task.Exception);
//             return;
//         }

//         //Snapshot of data
//         DataSnapshot dataSnapshot = task.Result;
//         if (dataSnapshot != null && dataSnapshot.HasChildren) {

//             // Create a list to store the retrieved data
//             List<dataToSave> userDataList = new List<dataToSave>();

//             foreach (DataSnapshot userSnapshot in dataSnapshot.Children) {
//                 Debug.Log("Finding user gameplay data..." + userSnapshot.Key); //checking
//                 Task<DataSnapshot> innerTask = dbRef.Child("users").Child(userSnapshot.Key).GetValueAsync();
//                 innerTask.Wait(); 
//                     if (innerTask.IsFaulted || innerTask.IsCanceled) {
//                         Debug.LogError("Failed to retrieve gameplay data for user: " + userSnapshot.Key);
//                         return;
//                     }

//                     DataSnapshot innerSnapshot = innerTask.Result; 
//                     if (innerSnapshot.Exists)
//                     {
//                         Debug.Log("Found INNER user gameplay data: " + userSnapshot.Key); //checking
//                         dataToSave userData = JsonUtility.FromJson<dataToSave>(innerSnapshot.GetRawJsonValue());

//                         // Output user data
//                         Debug.Log($"gpAchievements: {string.Join(", ", userData.achievements)}");
//                         Debug.Log($"gpCompleted Levels: {string.Join(", ", userData.completedLevels)}");
//                         Debug.Log($"gpTime: {userData.time}");
//                     }
//                     else
//                     {
//                         Debug.Log($"User with ID {userSnapshot.Key} not found."); //checking
//                     }
//             }
//         }
//     });
// }






    // public void UpdateUsername(string _username)
    // {
    //     userName = _username;
    //     datatosave.userID = _username;
    //     Debug.Log("The levelTime userID had changed to "+datatosave.userID);
    // }

}
