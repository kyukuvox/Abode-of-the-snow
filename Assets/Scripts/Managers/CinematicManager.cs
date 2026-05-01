using System.Collections;
using UnityEngine;

public class CinematicManager : MonoBehaviour
{
    public static CinematicManager Instance;

    [Header("PNJ à faire disparaître")]
    public GameObject npcToDisappear;

    [Header("Sprite qui marche")]
    public GameObject walkingSprite;
    public Animator walkingAnimator;
    public float walkSpeed = 2f;
    public float walkDistance = 20f; 

    [Header("PNJ à désactiver après la cinématique")]
    public GameObject npcToDisable;

    [Header("Déclencheur")]
    public DialogueData triggerDialogue; 

    private bool isPlaying = false;

    void Awake()
    {
        Instance = this;
    }

    public void TryTrigger(DialogueData dialogue)
    {
        if (dialogue == triggerDialogue && !isPlaying)
            StartCoroutine(PlayCinematic());
    }

    IEnumerator PlayCinematic()
    {
        isPlaying = true;
        GameStateManager.Instance.SetCinematicMode(true);

        CameraGround cam = Camera.main.GetComponent<CameraGround>();
        if (cam != null) cam.EnterCinematicMode();

        yield return new WaitForSeconds(0.5f);

        yield return new WaitForSeconds(0.5f);

        if (npcToDisappear != null)
        {
            npcToDisappear.SetActive(false);
        }

        yield return new WaitForSeconds(0.5f);


        if (walkingAnimator != null)
            walkingAnimator.SetBool("isWalking", false);

        yield return new WaitForSeconds(1f);

        if (walkingAnimator != null)
            walkingAnimator.SetBool("isWalking", true);

        SpriteRenderer sr = walkingSprite.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.flipX = true;

        float distanceTravelled = 0f;
        while (distanceTravelled < walkDistance)
        {
            float step = walkSpeed * Time.deltaTime;
            walkingSprite.transform.Translate(Vector2.left * step);
            distanceTravelled += step;
            yield return null;
        }

        walkingSprite.SetActive(false);


        yield return new WaitForSeconds(0.5f);

        if (npcToDisable != null)
        {
            NPCWithItemDialogue npcWithItem = npcToDisable.GetComponent<NPCWithItemDialogue>();
            NPCInteraction npcInteraction = npcToDisable.GetComponent<NPCInteraction>();

            if (npcWithItem != null)
                npcWithItem.enabled = false;
            else if (npcInteraction != null)
                npcInteraction.enabled = false;

            Collider2D col = npcToDisable.GetComponent<Collider2D>();
            if (col != null)
                col.enabled = false;

        }

        GameStateManager.Instance.SetCinematicMode(false);
        isPlaying = false;

        if (cam != null) cam.ExitCinematicMode();

        GameStateManager.Instance.SetCinematicMode(false);
        isPlaying = false;
        Debug.Log("Cinématique terminée !");
    }
}