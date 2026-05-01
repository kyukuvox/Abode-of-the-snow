using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraGround : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float smoothSpeed = 0.3f;

    [Header("Cinématique")]
    public float cinematicZoom = 6f;      
    public float normalZoom = 8f;       
    public float zoomSpeed = 2f;          
    public Image topBar;                  
    public Image bottomBar;              
    public float barHeight = 60f;       
    public float barAnimSpeed = 3f;      

    private Camera cam;
    private bool isCinematic = false;
    private Coroutine cinematicCoroutine;

    void Start()
    {
        player = GameObject.Find("JOUEUR");
        cam = GetComponent<Camera>();
        normalZoom = cam.orthographicSize;

        if (topBar != null) SetBarAlpha(topBar, 0f);
        if (bottomBar != null) SetBarAlpha(bottomBar, 0f);
    }

    void FixedUpdate()
    {
        Vector3 desiredPosition = new Vector3(
            player.transform.position.x,
            player.transform.position.y + 9,
            -10
        );
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        float targetZoom = isCinematic ? cinematicZoom : normalZoom;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
    }

    public void EnterCinematicMode()
    {
        isCinematic = true;
        if (cinematicCoroutine != null) StopCoroutine(cinematicCoroutine);
        cinematicCoroutine = StartCoroutine(AnimateBars(true));
    }

    public void ExitCinematicMode()
    {
        isCinematic = false;
        if (cinematicCoroutine != null) StopCoroutine(cinematicCoroutine);
        cinematicCoroutine = StartCoroutine(AnimateBars(false));
    }

    IEnumerator AnimateBars(bool show)
    {
        float elapsed = 0f;
        float startAlpha = show ? 0f : 1f;
        float targetAlpha = show ? 1f : 0f;
        float startHeight = show ? 0f : barHeight;
        float targetHeight = show ? barHeight : 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * barAnimSpeed;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed);
            float height = Mathf.Lerp(startHeight, targetHeight, elapsed);

            if (topBar != null)
            {
                SetBarAlpha(topBar, alpha);
                topBar.rectTransform.sizeDelta = new Vector2(topBar.rectTransform.sizeDelta.x, height);
            }
            if (bottomBar != null)
            {
                SetBarAlpha(bottomBar, alpha);
                bottomBar.rectTransform.sizeDelta = new Vector2(bottomBar.rectTransform.sizeDelta.x, height);
            }

            yield return null;
        }

        if (topBar != null)
        {
            SetBarAlpha(topBar, targetAlpha);
            topBar.rectTransform.sizeDelta = new Vector2(topBar.rectTransform.sizeDelta.x, targetHeight);
        }
        if (bottomBar != null)
        {
            SetBarAlpha(bottomBar, targetAlpha);
            bottomBar.rectTransform.sizeDelta = new Vector2(bottomBar.rectTransform.sizeDelta.x, targetHeight);
        }
    }

    void SetBarAlpha(Image bar, float alpha)
    {
        Color c = bar.color;
        c.a = alpha;
        bar.color = c;
    }
}