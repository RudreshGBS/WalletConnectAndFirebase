using Firebase;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using System.Linq;

public class FirebaseManager : MonoBehaviour
{
    UserList userlist = new UserList();
    UserList LeaderboardList = new UserList();
    DatabaseReference databaseReference;
    void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        //for (int i = 1; i <= 10; i++)
        //{
        //    saveData(i + "00000000000000000xsdadhasdhads00000" + i, i * Random.Range(1000, 10000));
        //}
        PopulateLeaderBoard(5);
        StartCoroutine(ShowLeaderboard());
    }

    void saveData(string id, int score)
    {
        User user1 = new User();
        user1.id = id;
        user1.score = score.ToString();
        userlist.Users.Add(user1);
        var jsondata = JsonUtility.ToJson(userlist);
        Debug.Log("Saved Data : " + jsondata);
        databaseReference.SetRawJsonValueAsync(jsondata);

        /// Last Working Structure
        //databaseReference.Child("Users").Child(id).Child("Score").SetValueAsync(score);

    }


    void PopulateLeaderBoard(int LoadLimit)
    {
        var top5 = FirebaseDatabase.DefaultInstance.GetReference("Users").OrderByChild("score").LimitToFirst(LoadLimit);
        // Keep this query synced.
        top5.KeepSynced(true);

        top5.GetValueAsync().ContinueWith(task =>
        {
            if (task.Exception != null)
            {
                throw task.Exception;
            }
            if (!task.IsCompleted)
            {
                return;
            }

            // Iterate children in order based on "score"
            if (task.Result != null)
            {
                foreach (var record in task.Result.Children)
                {
                    User user1 = new User();
                    user1.id = record.Child("id").GetRawJsonValue();
                    user1.score = record.Child("score").GetRawJsonValue();
                    //Debug.Log(user1.id + " " + user1.score);
                    LeaderboardList.Users.Add(user1);
                    //Debug.Log("Loaded Data: ID: " + record.Child("id").GetRawJsonValue() + " Score: " + record.Child("score").GetRawJsonValue());
                }

            }
        });

        
        ///Last Working Get UserList Data into Json!!!
        //databaseReference.OrderByChild("score").GetValueAsync().ContinueWith(task =>
        //{
        //    if (task.IsCompleted && !task.IsFaulted)
        //    {
        //        DataSnapshot data = task.Result;
        //        var jsondata = data.GetRawJsonValue();
        //        //UserList finalList = JsonUtility.FromJson<UserList>(jsondata);
        //        UserList finalList = new UserList();
        //        JsonUtility.FromJsonOverwrite(jsondata, finalList);
        //        //Debug.Log("JSON: " + finalList.Users.Count);
        //        finalList.Users.ForEach(x =>
        //        {
        //            Debug.Log("ID: " + x.id + "Score: " + x.score);
        //        });
        //    }
        //});


        //databaseReference.GetValueAsync().ContinueWith(task =>
        //{
        //    if (task.IsCompleted && !task.IsFaulted)
        //    {
        //        DataSnapshot data = task.Result;
        //        var jsondata = data.GetRawJsonValue();
        //        //UserList finalList = JsonUtility.FromJson<UserList>(jsondata);
        //        UserList finalList = new UserList();
        //        JsonUtility.FromJsonOverwrite(jsondata, finalList);
        //        //Debug.Log("JSON: " + finalList.Users.Count);
        //        finalList.Users.ForEach(x =>
        //        {
        //            Debug.Log("ID: " + x.id + "Score: " + x.score);
        //        });
        //    }
        //});
    }

    IEnumerator ShowLeaderboard()
    {
        yield return new WaitForSeconds(5f);
        foreach (var child in LeaderboardList.Users)
        {
            Debug.Log(child.id + " " + child.score);
        }
    }
}

[System.Serializable]
public class UserList
{
    public List<User> Users = new List<User>();
}

[System.Serializable]
public class User
{
    public string id;
    public string score;
}

