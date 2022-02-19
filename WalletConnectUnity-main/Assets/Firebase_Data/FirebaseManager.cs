using Firebase;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;

public class FirebaseManager : MonoBehaviour
{
    DatabaseReference databaseReference;
    void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        for(int i = 1; i <=10; i++)
        {
            saveData(i + "00000000000000000xsdadhasdhads00000" + i, i * Random.Range(1000, 10000));
        }
    }

    void saveData(string id, int score)
    {
        databaseReference.Child("Users").Child(id).Child("Score").SetValueAsync(score);

        //databaseReference.Child("Users").Child(key).SetValueAsync(id).ContinueWith(task =>
        //{
        //    if(task.IsCompleted)
        //    {
        //        databaseReference.Child("Users").Child(key).Child("Score").SetValueAsync(1000);
        //    }
        //    else
        //    {
        //        Debug.LogError("Error Creating a Databse Entry");
        //    }
        //});
        //Dictionary<string, System.Object> entryValues = user1.ToDictionary();
        //databaseReference.UpdateChildrenAsync(entryValues);
        ///
        /// Overwriting Code
        ///
        //User user1 = new User();
        //user1.ID = id;
        //string json = JsonUtility.ToJson(user1);

        //databaseReference.Child("Users").SetRawJsonValueAsync(json).ContinueWith(task =>
        //{
        //    if (task.IsCompleted)
        //    {
        //        Debug.Log("Successfully added data to firebase");
        //    }
        //    else
        //    {
        //        Debug.Log("Databse Update not successful");
        //    }
        //});
    }

    private void WriteNewScore(string userId, int score)
    {
        // Create new entry at /user-scores/$userid/$scoreid and at
        // /leaderboard/$scoreid simultaneously
        string key = databaseReference.Child("scores").Push().Key;
        LeaderboardEntry entry = new LeaderboardEntry(userId, score);
        Dictionary<string, System.Object> entryValues = entry.ToDictionary();

        Dictionary<string, System.Object> childUpdates = new Dictionary<string, System.Object>();
        childUpdates["/scores/" + key] = entryValues;
        childUpdates["/user-scores/" + userId + "/" + key] = entryValues;

        databaseReference.UpdateChildrenAsync(childUpdates);
    }
}

public class User
{
    public string ID;

    public Dictionary<string, System.Object> ToDictionary()
    {
        Dictionary<string, System.Object> result = new Dictionary<string, System.Object>();
        result["id"] = ID;

        return result;
    }
}

public class LeaderboardEntry
{
    public string uid;
    public int score = 0;


    public LeaderboardEntry(string uid, int score)
    {
        this.uid = uid;
        this.score = score;
    }

    public Dictionary<string, System.Object> ToDictionary()
    {
        Dictionary<string, System.Object> result = new Dictionary<string, System.Object>();
        result["id"] = uid;
        result["score"] = score;

        return result;
    }
}

