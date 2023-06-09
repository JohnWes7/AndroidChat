using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;

public class ReverseEngineering : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
           // DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
            Debug.Log("开始读取");
            /*FirebaseDatabase.DefaultInstance
     .GetReference("Users/lHutIeAly4QKNPASN5HxotT2CL23/Friend")
     .GetValueAsync().ContinueWithOnMainThread(task => {
         if (task.IsFaulted)
         {
              // Handle the error...
          }
         else if (task.IsCompleted)
         {
             DataSnapshot snapshot = task.Result;
             List<string> myList = new List<string>();
             myList.Add("AaIKtkIF5lRTSNgELwfHHquckPz2");
             myList.Add("6AxgcfVMjMY8Mt3sdvkFKHv7oYC2");
            reference.SetValueAsync(myList);
             
            /* Debug.Log("读取成功");
             foreach (DataSnapshot childSnapshot in snapshot.Children)
             {
                 string childKey = childSnapshot.Key;
                 string childValue = childSnapshot.Value.ToString();
                 Debug.Log("键 :" + childKey + ": " +" 值 ： "+ childValue);
                 //childSnapshot = snapshot.Child(childKey);
             }
             Debug.Log("结束");
             // Do something with snapshot...*/
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("Users/lHutIeAly4QKNPASN5HxotT2CL23/Friend/2");
            /* List<string> myList = new List<string>();
             myList.Add("aaaa");
             myList.Add("bb");*/
            string newfriend = "abc";
            reference.SetValueAsync(newfriend);
        }
    }



}
