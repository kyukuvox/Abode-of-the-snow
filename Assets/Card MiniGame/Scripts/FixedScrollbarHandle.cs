using UnityEngine;
using UnityEngine.UI;

public class FixedScrollbarHandle : MonoBehaviour
{
    public float fixedSize = 0.2f;
    private Scrollbar scrollbar;

    void Start()
    {
        scrollbar = GetComponent<Scrollbar>();
        if (scrollbar != null)
            scrollbar.size = fixedSize;
    }

    void LateUpdate()
    {
        if (scrollbar != null && scrollbar.size != fixedSize)
            scrollbar.size = fixedSize;
    }
}