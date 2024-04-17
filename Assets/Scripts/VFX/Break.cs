using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Break : MonoBehaviour
{
    [CustomEditor(typeof(Break))]
    private class BreakEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
                SerializedProperty canBreakOnImpact = serializedObject.FindProperty("canBreakOnImpact");
                canBreakOnImpact.boolValue = EditorGUILayout.Toggle("Break On Impact", canBreakOnImpact.boolValue);
                EditorGUI.BeginDisabledGroup(!canBreakOnImpact.boolValue);
                    SerializedProperty minBreakVelocity = serializedObject.FindProperty("minBreakVelocity");
                    minBreakVelocity.floatValue = Mathf.Max(0, EditorGUILayout.FloatField("Break Velocity", minBreakVelocity.floatValue));
                EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            SerializedProperty breakBoostForce = serializedObject.FindProperty("breakBoostForce");
            breakBoostForce.floatValue = Mathf.Max(0, EditorGUILayout.FloatField("Break Force", breakBoostForce.floatValue));

            EditorGUILayout.BeginHorizontal();
                SerializedProperty destroyBottleAfterBreak = serializedObject.FindProperty("destroyBottleAfterBreak");
                destroyBottleAfterBreak.boolValue = EditorGUILayout.Toggle("Destroy Bottle", destroyBottleAfterBreak.boolValue);
                EditorGUI.BeginDisabledGroup(!destroyBottleAfterBreak.boolValue);
                    EditorGUILayout.BeginVertical();
                        SerializedProperty destroyDelaySeconds = serializedObject.FindProperty("destroyDelaySeconds");
                        destroyDelaySeconds.floatValue = Mathf.Max(0, EditorGUILayout.FloatField("Delay", destroyDelaySeconds.floatValue));
                        SerializedProperty fadeDurationSeconds = serializedObject.FindProperty("fadeDurationSeconds");
                        fadeDurationSeconds.floatValue = Mathf.Max(0, EditorGUILayout.FloatField("Fade Duration", fadeDurationSeconds.floatValue));
                    EditorGUILayout.EndVertical();
                EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            if (GUILayout.Button("Break"))
            {
                Break breakScript = (Break)target;
                breakScript.Trigger();
            }
            EditorGUI.EndDisabledGroup();
        }
    }

    [SerializeField]
    private bool canBreakOnImpact = true;
    [SerializeField]
    private float minBreakVelocity = 2f;
    [SerializeField]
    private float breakBoostForce = 2f;
    [SerializeField]
    private bool destroyBottleAfterBreak = true;
    [SerializeField]
    private float destroyDelaySeconds = 5f;
    [SerializeField]
    private float fadeDurationSeconds = 1f;

    private bool _isBroken = false;
    private MeshRenderer _meshRenderer;
    private Rigidbody _rigidbody;
    private List<Collider> _collider;
    private AudioSource _audio;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _rigidbody = GetComponent<Rigidbody>();

        _collider = new List<Collider>();
        GetComponents(_collider);

        _audio = GetComponent<AudioSource>();

        setPiecesActive(false);
    }

    private void FixedUpdate()
    {
        if (_isBroken)
        {

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (canBreakOnImpact && collision.relativeVelocity.magnitude >= minBreakVelocity)
        {
            Trigger();
        }
    }

    async public void Trigger()
    {
        if (_isBroken) return;
        _isBroken = true;

        _meshRenderer.enabled = false;
        _collider.ForEach(c => c.enabled = false);
        _rigidbody.isKinematic = true;

        setPiecesActive(true);

        _audio.Play();

        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            Vector3 force = (rb.transform.position - (transform.position + transform.up * 0.15f)).normalized * breakBoostForce;
            rb.AddForce(force);
        }

        if (destroyBottleAfterBreak)
        {
            await Task.Delay((int)(destroyDelaySeconds * 1000));
            if (!Application.isPlaying || gameObject is null) return;

            List<MeshRenderer> renderers = new List<MeshRenderer>(transform.GetComponentsInChildren<MeshRenderer>());
            float step = 0.05f / fadeDurationSeconds;
            for (float alpha = 1f; alpha >= 0; alpha -= step)
            {
                foreach (MeshRenderer renderer in renderers)
                {
                    if (!Application.isPlaying || !Application.isPlaying) return;
                    Color c = renderer.material.color;
                    c.a = alpha;
                    renderer.material.color = c;
                }
                await Task.Delay(50);
            }

            if (gameObject is null || !Application.isPlaying) return;
            Destroy(gameObject);
        }
    }

    private void setPiecesActive(bool active)
    {
        foreach (Transform piece in transform)
        {
            piece.gameObject.SetActive(active);
        }
    }
}
