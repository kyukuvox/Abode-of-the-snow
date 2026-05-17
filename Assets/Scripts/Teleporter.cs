using System.Collections;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform destination;
    public float bumpScale = 1.2f;
    public float bumpDuration = 0.2f;
    public bool resetParallax = true;

    [Header("Cooldown")]
    public float teleportCooldown = 1f;

    [Header("Son")]
    public AudioClip teleportSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    private Transform player;
    private HoverParticleManager hoverParticles;
    private Vector3 originalScale;

    private static bool isTeleporting = false;

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
        if (CardGameManager.IsPlaying) return;
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
        if (PauseMenu.Instance.IsPaused()) return;
        if (MenuManager.Instance.IsMenuOpen()) return;
        if (DialogueManager.Instance.IsActive()) return;
        if (GameStateManager.Instance.IsCinematicMode()) return;
        if (CardGameManager.IsPlaying) return;
        if (isTeleporting) return;

        SoundSettings.PlaySound(teleportSound, soundVolume, this);

        StartCoroutine(Bump());
        StartCoroutine(TeleportCooldown());

        if (destination != null)
        {
            FadeManager.Instance.FadeToBlackAndBack(() =>
            {
                player.position = destination.position;

                CameraGround cam = Camera.main.GetComponent<CameraGround>();
                if (cam != null)
                    cam.SnapToPlayer();

                if (resetParallax && ParallaxManager.Instance != null)
                    ParallaxManager.Instance.ResetToOrigin();
            });
        }
    }

    IEnumerator TeleportCooldown()
    {
        isTeleporting = true;
        yield return new WaitForSeconds(teleportCooldown);
        isTeleporting = false;
    }

    IEnumerator Bump()
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
            transform.localScale = Vector3.Lerp(originalScale * bumpScale, originalScale, t);
            yield return null;
        }

        transform.localScale = originalScale;
    }

    void OnDrawGizmosSelected()
    {
        if (destination != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(destination.position, 0.3f);
            Gizmos.DrawLine(transform.position, destination.position);
        }
    }
}