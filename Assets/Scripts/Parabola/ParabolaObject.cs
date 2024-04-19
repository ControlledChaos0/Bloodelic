using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class ParabolaObject : MonoBehaviour
{
    [Header("Curve")] [Space]
    [Range(0, 10.0f)] public float height;
    [Range(0, 1.0f)] public float interpolation;

    [SerializeField] public Transform startTransform;
    [SerializeField] public Transform endTransform;

    Vector3 start => startTransform != null ? startTransform.position : transform.position;
    Vector3 end => endTransform != null ? endTransform.position : transform.position;

    [Header("Animation")][Space]
    // Controls how the thrown object moves along the curve
    [SerializeField] AnimationCurve animationCurve;
    // How long the thrown object takes to travel from launch point to landing point
    [SerializeField] [Min(0f)] private float totalTravelTime = 1.5f;
    public GameObject thrownObject;
    
    public void Start()
    {
    }

    public void Update()
    {
        // Test throw
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(MoveAlongCurve());
        }
    }

    public IEnumerator MoveAlongCurve()
    {
        if (thrownObject == null)
        {
            Debug.LogError("Attempted to trgger MoveAlongCurve on Parabola object but thrownObject was null!!");
            yield break;
        }
        
        float timeElapsed = 0f;
        while (timeElapsed < totalTravelTime)
        {
            yield return null;
            timeElapsed += Time.deltaTime;

            float t = timeElapsed / totalTravelTime;
            float interp = animationCurve != null ? animationCurve.Evaluate(t) : t;

            Vector3 position = MathParabola.PointOnParabola(start, end, height, interp);
            thrownObject.transform.position = position;
        }
    }
    
    #if UNITY_EDITOR

    [Header("Debug")] [Space]
    [SerializeField] [Range(2, 16)] private int DEBUG_Points;
    [SerializeField] [Min(0.1f)] private float DEBUG_LineWidth = 2f;
    private void OnDrawGizmos()
    {
        if (startTransform != null && endTransform != null)
        {
            for(int i = 0; i < DEBUG_Points; i++)
            {
                float t1 = i / (float)DEBUG_Points;
                float t2 = (i + 1) / (float)DEBUG_Points;

                Vector3 a = MathParabola.PointOnParabola(start, end, height, t1);
                Vector3 b = MathParabola.PointOnParabola(start, end, height, t2);

                Handles.color = Color.red;
                Handles.DrawAAPolyLine(DEBUG_LineWidth, a, b);
                Handles.color = Color.white;
            }
        }
    }
    #endif
}
