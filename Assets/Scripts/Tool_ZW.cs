using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class Tool_ZW
{
    public static IEnumerator CheckUserData(string UserId, UnityEvent dataExistEvent = null, UnityEvent failEvent = null,UnityEvent dataInexistenceEvent = null)
    {
        //��ʼֵ�ֵ�

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        var task = FirebaseDatabase.DefaultInstance.GetReference($"Users/{UserId}" ).GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogWarning($"Tool:CheckUserData �û����ݲ�ѯʧ�� : {UserId}");
            if (failEvent != null)
            {
                failEvent.Invoke();
            }
        }
        else
        {
            DataSnapshot snapshot = task.Result;
            if (snapshot.HasChild("Friend") && snapshot.HasChild("Image") && snapshot.HasChild("Name")){
                Debug.Log($"Tool:CheckUserData ���û��������Ѵ��� : {UserId} ");
                if (dataExistEvent != null)
                {
                    dataExistEvent.Invoke();
                }
            }
            else
            {
                Debug.Log($"Tool:CheckUserData ���û������ݲ����� : {UserId} ");
                if (dataInexistenceEvent != null)
                {
                    dataInexistenceEvent.Invoke();
                }
            }
            
        }
    }
}
