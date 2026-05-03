using UnityEngine;

public class SpriteParticleHover : MonoBehaviour
{
    [Header("Particules")]
    public float particleSize = 0.1f;
    public int maxParticles = 50;
    public float particleSpeed = 0.3f;
    public float particleLifetime = 1.5f;
    public Color particleColor = Color.white;

    private ParticleSystem particles;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private Bounds emissionBounds;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        SetupParticles();
    }

    void SetupParticles()
    {
        if (col != null)
            emissionBounds = col.bounds;
        else if (spriteRenderer != null)
            emissionBounds = spriteRenderer.bounds;

        GameObject particleObj = new GameObject("HoverParticles");
        particleObj.transform.SetParent(transform);
        particleObj.transform.position = emissionBounds.center;
        particleObj.transform.rotation = Quaternion.identity;
        particleObj.transform.localScale = Vector3.one;

        particles = particleObj.AddComponent<ParticleSystem>();

        ParticleSystemRenderer psRenderer = particleObj.GetComponent<ParticleSystemRenderer>();
        psRenderer.material = new Material(Shader.Find("Sprites/Default"));
        psRenderer.sortingLayerName = spriteRenderer != null ?
            spriteRenderer.sortingLayerName : "Default";
        psRenderer.sortingOrder = spriteRenderer != null ?
            spriteRenderer.sortingOrder + 1 : 1;

        var main = particles.main;
        main.loop = true;
        main.playOnAwake = false;
        main.startLifetime = particleLifetime;
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(particleSize * 0.5f, particleSize);
        main.startColor = particleColor;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = maxParticles;

        var shape = particles.shape;
        shape.enabled = false;

        var emission = particles.emission;
        emission.rateOverTime = 0f;

        var velocity = particles.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.World;
        velocity.x = new ParticleSystem.MinMaxCurve(-0.1f, 0.1f);
        velocity.y = new ParticleSystem.MinMaxCurve(0.1f, particleSpeed);
        velocity.z = new ParticleSystem.MinMaxCurve(-0.1f, 0.1f);

        var sizeOverLifetime = particles.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 1f);
        sizeCurve.AddKey(1f, 0f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        var colorOverLifetime = particles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = gradient;

        particles.Stop();
        particleObj.SetActive(false);
    }

    void Update()
    {
        if (col != null)
            emissionBounds = col.bounds;
        else if (spriteRenderer != null)
            emissionBounds = spriteRenderer.bounds;

        if (particles != null && particles.isPlaying)
        {
            int toEmit = Mathf.Max(1, (int)(maxParticles * Time.deltaTime));
            for (int i = 0; i < toEmit; i++)
            {
                Vector3 randomPos = new Vector3(
                    Random.Range(emissionBounds.min.x, emissionBounds.max.x),
                    Random.Range(emissionBounds.min.y, emissionBounds.max.y),
                    0f
                );

                var emitParams = new ParticleSystem.EmitParams();
                emitParams.position = randomPos;
                emitParams.applyShapeToPosition = false;
                particles.Emit(emitParams, 1);
            }
        }
    }

    public void ShowParticles()
    {
        if (particles == null) return;
        particles.gameObject.SetActive(true);
        particles.Play();
    }

    public void HideParticles()
    {
        if (particles == null) return;
        particles.Stop();
        particles.Clear();
        particles.gameObject.SetActive(false);
    }
}