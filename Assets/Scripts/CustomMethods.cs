using Alteruna;
using System.Collections.Generic;
using UnityEditor.Search;
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
    
    private static void SetActiveMeshColliderRecursivelyQueue(Queue<Transform> branchesToCheck, bool newState)
    {
        Transform currentCheck = branchesToCheck.Dequeue();

        //enabling / disabling colliders
        Collider col = currentCheck.gameObject.GetComponent<Collider>();
        MeshCollider stup = currentCheck.gameObject.GetComponent<MeshCollider>();
        if (col!=null) currentCheck.gameObject.GetComponent<Collider>().enabled = newState;
        if (stup!=null) currentCheck.gameObject.GetComponent<MeshCollider>().enabled = newState;


            foreach (Transform child in currentCheck.GetComponentsInChildren<Transform>())
            {
                branchesToCheck.Enqueue(child);
            }
        SetActiveMeshColliderRecursivelyQueue(branchesToCheck, newState);
    }
    public static void SetActiveMeshColliderRecursively(Transform start, bool newState)
    {
        Queue<Transform> branches = new Queue<Transform>();
        branches.Enqueue(start);
        SetActiveMeshColliderRecursivelyQueue(branches, newState);
    }
}
