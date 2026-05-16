using System.Collections;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;
    public float bumpScale = 1.3f;
    public float bumpDuration = 0.2f;

    [Header("Son")]
    public AudioClip pickupSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    private HoverParticleManager hoverParticles;
    private Transform player;
    private Vector3 originalScale;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        hoverParticles = GetComponent<HoverParticleManager>();
        originalScale = transform.localScale;
    }

    void OnMouseEnter()
    {
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (GameStateManager.Instance.IsCinematicMode()) return;
        if (hoverParticles != null)
            hoverParticles.Show();
    }

    void OnMouseExit()
    {
        if (hoverParticles != null)
            hoverParticles.Hide();
    }

    void OnMouseDown()
    {
        if (DialogueManager.Instance.IsActive()) return;
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (GameStateManager.Instance.IsCinematicMode()) return;

        // Son 2D sur GameObject temporaire
        if (pickupSound != null)
        {
            GameObject tempAudio = new GameObject("TempAudio");
            AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
            tempSource.clip = pickupSound;
            tempSource.volume = soundVolume;
            tempSource.spatialBlend = 0f;
            tempSource.Play();
            Destroy(tempAudio, pickupSound.length);
        }

        StartCoroutine(BumpAndPickup());
    }

    IEnumerator BumpAndPickup()
    {
        float elapsed = 0f;
        float halfDuration = bumpDuration / 2f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * bumpScale, t);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            transform.localScale = Vector3.Lerp(originalScale * bumpScale, Vector3.zero, t);
            yield return null;
        }

        if (PickedUpItemsTracker.Instance != null)
            PickedUpItemsTracker.Instance.AddPickedUpItem(item.itemName);

        Inventory.Instance.AddItem(item);
        ItemDescriptionManager.Instance.ShowItemDescription(item);
        Destroy(gameObject);
    }
}