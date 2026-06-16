using UnityEngine;
using System.Collections.Generic;

public class Cursor : MonoBehaviour
{
    public static Cursor Instance; 

    public LineRenderer line;

    [Header("Input")]
    public float minDistance = 0.05f;

    [Header("Trail")]
    public float lifeTime = 0.15f;
    public int maxPoints = 30;

    [Header("Slash Shape")]
    public float startWidth = 0.12f;
    public float endWidth = 0.0f;

    private Camera cam;

    
    public List<Vector3> Points => points;

    private List<Vector3> points = new List<Vector3>();
    private List<float> pointTimes = new List<float>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cam = Camera.main;
        line.startWidth = startWidth;
        line.endWidth = endWidth;
    }

    void Update()
    {
        bool down = Input.GetMouseButtonDown(0);
        bool hold = Input.GetMouseButton(0);

        Vector3 inputPos = Input.mousePosition;

        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            inputPos = t.position;
            down = t.phase == TouchPhase.Began;
            hold = t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary;
        }

        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(inputPos.x, inputPos.y, 10f));
        worldPos.z = 0;

        if (down)
        {
            Clear();
            AddPoint(worldPos);
        }

        if (hold)
        {
            AddPoint(worldPos);
        }

        UpdateTrail();
    }

    void AddPoint(Vector3 p)
    {
        if (points.Count == 0 || Vector3.Distance(points[^1], p) > minDistance)
        {
            points.Add(p);
            pointTimes.Add(Time.time);

            if (points.Count > maxPoints)
            {
                points.RemoveAt(0);
                pointTimes.RemoveAt(0);
            }
        }
    }

    void UpdateTrail()
    {
        float now = Time.time;

        while (points.Count > 0 && now - pointTimes[0] > lifeTime)
        {
            points.RemoveAt(0);
            pointTimes.RemoveAt(0);
        }

        line.positionCount = points.Count;

        for (int i = 0; i < points.Count; i++)
        {
            line.SetPosition(i, points[i]);
        }
    }

    void Clear()
    {
        points.Clear();
        pointTimes.Clear();
        line.positionCount = 0;
    }
}