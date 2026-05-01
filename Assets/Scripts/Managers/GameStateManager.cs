using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    private bool isCinematicMode = false;

    void Awake()
    {
        Instance = this;
    }

    public void SetCinematicMode(bool active)
    {
        isCinematicMode = active;
    }

    public bool IsCinematicMode() { return isCinematicMode; }
}