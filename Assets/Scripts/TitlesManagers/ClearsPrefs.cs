using UnityEngine;

public class ClearPrefs : MonoBehaviour
{
    void Awake()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("Tous les PlayerPrefs supprimés !");
    }
}