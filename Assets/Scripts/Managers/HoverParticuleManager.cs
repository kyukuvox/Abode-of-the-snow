using UnityEngine;

public class HoverParticleManager : MonoBehaviour
{
    private SpriteParticleHover particleHover;

    void Start()
    {
        particleHover = GetComponent<SpriteParticleHover>();
    }

    public void Show()
    {
        if (particleHover != null)
            particleHover.ShowParticles();
    }

    public void Hide()
    {
        if (particleHover != null)
            particleHover.HideParticles();
    }
}