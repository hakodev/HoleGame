using UnityEngine;
using System.Collections.Generic;

public class HandObjects : MonoBehaviour
{
    public Vector3 OriginalPos { get; private set; }
    public static List<GameObject> allHandObjects = new List<GameObject>();

    private void Awake()
    {
        allHandObjects.Add(gameObject);
    }

    private void Start()
    {
        OriginalPos = transform.localPosition;
        SwitchActive(gameObject, false);
    }
    public static void ToggleActive(string name, bool isActive)
    {
        for (int i = 0; i < allHandObjects.Count; i++)
        {
            if (allHandObjects[i].name.Contains(name))
            {
                SwitchActive(allHandObjects[i], isActive);
                break;
            }
        }
    }
    private static void SwitchActive(GameObject item, bool isActive)
    {
        if (isActive) item.transform.localPosition = item.GetComponent<HandObjects>().OriginalPos;
        if (!isActive) item.transform.localPosition = new Vector3(-9999, -9999, -9999);       
    }

}
