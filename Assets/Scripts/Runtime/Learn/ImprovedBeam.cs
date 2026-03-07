using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ImprovedBeam : MonoBehaviour
{
    [SerializeField] public float lifetime = .1f;
    [SerializeField] public float cooldown = 1f;
    [SerializeField] public float range = 10f;
    [SerializeField] private float widthMultiplier = .1f;
    
    private LineRenderer lineRenderer;
    private AnimationCurve animationCurve;
    private Transform target;
    private Transform source;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.widthMultiplier = widthMultiplier;
        animationCurve = new();
        animationCurve.AddKey(0, .05f);
        animationCurve.AddKey(1f, 1f);
        lineRenderer.widthCurve = animationCurve;
    }

    public void Fire(Transform source, Transform target)
    {
        // Debug.Log($"Beam {gameObject.name} {Time.time}");
        this.target = target;
        this.source = source;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, source.position);
        lineRenderer.SetPosition(1, target.position);
    }
}
