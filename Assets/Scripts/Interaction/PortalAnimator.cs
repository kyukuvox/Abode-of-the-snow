using System.Collections;
using UnityEngine;

public class PortalAnimator : MonoBehaviour
{
    public float descendSpeed = 2f;
    public float targetYOffset = -3f;
    public DialogueData triggerDialogue;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isActivated = false;

    void Awake()
    {
        startPosition = transform.position;
        targetPosition = new Vector3(
            transform.position.x,
            transform.position.y + targetYOffset,
            transform.position.z
        );
    }

    public void TryActivate(DialogueData dialogue)
    {
        if (dialogue == triggerDialogue)
            ActivatePortal();
    }

    public void ActivatePortal()
    {
        if (isActivated) return;
        isActivated = true;
        StartCoroutine(DescendPortal());
    }

    IEnumerator DescendPortal()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                Time.deltaTime * descendSpeed
            );
            yield return null;
        }

        transform.position = targetPosition;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Vector3 target = new Vector3(
            transform.position.x,
            transform.position.y + targetYOffset,
            transform.position.z
        );
        Gizmos.DrawWireCube(target, Vector3.one);
        Gizmos.DrawLine(transform.position, target);
    }
}