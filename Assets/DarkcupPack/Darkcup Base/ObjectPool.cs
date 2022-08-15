using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
   public static ObjectPool Instance;
   private Dictionary<string, Queue<GameObject>> allPools = new Dictionary<string, Queue<GameObject>>();

   private void Awake()
   {
      Instance = this;
   }

  

   public GameObject GetGameObjectFromPool(string objectName, Vector3 position)
   {
      var allPoolList = Instance.allPools;
      
      if (!allPoolList.ContainsKey(objectName))
      {
         allPoolList.Add(objectName, new Queue<GameObject>());
      }
      Queue<GameObject> queue = allPoolList[objectName];
      foreach (var obj in queue)
      {
         if (obj.activeSelf == false)
         {
            obj.transform.position = position;
            obj.SetActive(true);
            return obj;
         }
      }

      var created =  Instantiate(Resources.Load<GameObject>(objectName), position, Quaternion.identity);
      allPoolList[objectName].Enqueue(created);
      //created.transform.SetParent(Instance.transform);
      return created;
   }
}
