using Alteruna;
using System.Collections.Generic;
using UnityEngine;

public class CustomMethods
{
    /*
      
     // EXAMPLEE OF HOW TO USEE
         private void Start()
    {
        Queue<Transform> temp = new Queue<Transform>();
        temp.Enqueue(animator.transform);
        FindChildRecursively(temp, "krawatte");
        animationTie = foundRecursively;
    }
     */

    public static GameObject foundRecursively = null;
    public static void FindChildRecursively(Queue<Transform> branchesToCheck, string searchedName)
    {

        Transform currentCheck = branchesToCheck.Dequeue();
        if (currentCheck.gameObject.name == searchedName)
        {
            foundRecursively = currentCheck.gameObject;
            return;
        }
        else
        {
            foreach (Transform child in currentCheck.GetComponentsInChildren<Transform>())
            {
                branchesToCheck.Enqueue(child);
            }
            FindChildRecursively(branchesToCheck, searchedName);
        }
    }
    
}
