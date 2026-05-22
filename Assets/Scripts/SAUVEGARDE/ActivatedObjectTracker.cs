using System.Collections.Generic;
using UnityEngine;

public class ActivatedObjectsTracker : MonoBehaviour
{
    public static ActivatedObjectsTracker Instance;

    private HashSet<string> activatedObjects = new HashSet<string>();

    void Awake()
    {
        Instance = this;
    }

    public void RegisterActivated(string objectName)
    {
        activatedObjects.Add(objectName);
    }

    public bool IsActivated(string objectName)
    {
        return activatedObjects.Contains(objectName);
    }

    public List<string> GetActivatedObjects()
    {
        return new List<string>(activatedObjects);
    }

    public void LoadActivatedObjects(List<string> names)
    {
        activatedObjects = new HashSet<string>(names);
    }
}