using System.Collections;
using UnityEngine;

public class PortalAnimator : MonoBehaviour
{
    public static PortalAnimator Instance;

    public float descendSpeed = 2f;    
    public float targetYOffset = -3f; 

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isActivated = false;

    void Awake()
    {
        Instance = this;
        startPosition = transform.position;
        targetPosition = new Vector3(
            transform.position.x,
            transform.position.y + targetYOffset,
            transform.position.z
        );
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
        Debug.Log("Portail activé !");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(
            new Vector3(transform.position.x, transform.position.y + targetYOffset, transform.position.z),
            Vector3.one
        );
        Gizmos.DrawLine(transform.position,
            new Vector3(transform.position.x, transform.position.y + targetYOffset, transform.position.z)
        );
    }
}