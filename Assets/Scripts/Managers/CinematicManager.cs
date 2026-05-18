using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CinematicManager : MonoBehaviour
{
    public static CinematicManager Instance;

    [Header("PNJ à faire disparaître")]
    public GameObject npcToDisappear;

    [Header("Sprite qui marche")]
    public GameObject walkingSprite;
    public Animator walkingAnimator;
    public string walkAnimationName = "Walk";
    public float walkSpeed = 2f;
    public float walkDistance = 20f;

    [Header("PNJ à désactiver après la cinématique")]
    public GameObject npcToDisable;

    [Header("Déclencheur")]
    public DialogueData triggerDialogue;

    [Header("Transition")]
    public float fadeInDuration = 0.5f;
    public float fadeOutDuration = 0.5f;
    public AudioClip transitionSound;
    [Range(0f, 1f)]
    public float transitionSoundVolume = 1f;

    [Header("Son de marche")]
    public AudioClip walkLoopSound;
    [Range(0f, 1f)]
    public float walkLoopVolume = 1f;

    [Header("Panel noir dédié")]
    public Image blackPanel;

    private bool isPlaying = false;
    private AudioSource walkAudioSource;

    void Awake()
    {
        Instance = this;

        if (blackPanel != null)
        {
            blackPanel.gameObject.SetActive(false);
            blackPanel.color = new Color(0f, 0f, 0f, 0f);
        }

        walkAudioSource = gameObject.AddComponent<AudioSource>();
        walkAudioSource.loop = true;
        walkAudioSource.playOnAwake = false;
        walkAudioSource.spatialBlend = 0f;
        walkAudioSource.volume = 0f;
    }

    public void TryTrigger(DialogueData dialogue)
    {
        if (dialogue == triggerDialogue && !isPlaying)
            StartCoroutine(PlayCinematic());
    }

    IEnumerator FadeToBlack()
    {
        blackPanel.gameObject.SetActive(true);
        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            blackPanel.color = new Color(0f, 0f, 0f, Mathf.Lerp(0f, 1f, elapsed / fadeInDuration));
            yield return null;
        }

        blackPanel.color = new Color(0f, 0f, 0f, 1f);
        yield return null;
        yield return null;
    }

    IEnumerator FadeFromBlack()
    {
        blackPanel.color = new Color(0f, 0f, 0f, 1f);
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            blackPanel.color = new Color(0f, 0f, 0f, Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration));
            yield return null;
        }

        blackPanel.color = new Color(0f, 0f, 0f, 0f);
        blackPanel.gameObject.SetActive(false);
    }

    IEnumerator FadeInWalkSound()
    {
        float elapsed = 0f;
        float targetVolume = walkLoopVolume * SoundSettings.SFXVolume;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            walkAudioSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / fadeOutDuration);
            yield return null;
        }

        walkAudioSource.volume = targetVolume;
    }

    void DisableNPC()
    {
        if (npcToDisable == null) return;

        NPCWithItemDialogue npcWithItem = npcToDisable.GetComponent<NPCWithItemDialogue>();
        NPCInteraction npcInteraction = npcToDisable.GetComponent<NPCInteraction>();

        if (npcWithItem != null)
            npcWithItem.enabled = false;
        else if (npcInteraction != null)
            npcInteraction.enabled = false;

        Collider2D col = npcToDisable.GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        HoverParticleManager hover = npcToDisable.GetComponent<HoverParticleManager>();
        if (hover != null)
            hover.Hide();
    }

    IEnumerator PlayCinematic()
    {
        isPlaying = true;
        GameStateManager.Instance.SetCinematicMode(true);

        CameraGround cam = Camera.main.GetComponent<CameraGround>();
        if (cam != null) cam.EnterCinematicMode();

        yield return new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(0.5f);

        if (walkingAnimator != null)
            walkingAnimator.SetBool("isWalking", false);

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(FadeToBlack());

        if (npcToDisappear != null)
            npcToDisappear.SetActive(false);

        DisableNPC();

        if (transitionSound != null)
        {
            SoundSettings.PlaySound(transitionSound, transitionSoundVolume, this);
            yield return new WaitForSeconds(transitionSound.length);
        }

        if (walkingAnimator != null)
            walkingAnimator.Play(walkAnimationName, 0, 0f);

        if (walkLoopSound != null)
        {
            walkAudioSource.clip = walkLoopSound;
            walkAudioSource.volume = 0f;
            walkAudioSource.Play();
        }

        StartCoroutine(FadeFromBlack());
        StartCoroutine(FadeInWalkSound());

        float distanceTravelled = 0f;
        while (distanceTravelled < walkDistance)
        {
            float step = walkSpeed * Time.deltaTime;
            walkingSprite.transform.Translate(Vector2.left * step);
            distanceTravelled += step;
            yield return null;
        }

        walkAudioSource.Stop();
        walkAudioSource.volume = 0f;
        walkingSprite.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        if (cam != null) cam.ExitCinematicMode();

        GameStateManager.Instance.SetCinematicMode(false);
        isPlaying = false;
    }
}