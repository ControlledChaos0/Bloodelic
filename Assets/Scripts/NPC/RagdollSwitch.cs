using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollSwitch : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    List<Rigidbody> rigidBodies = new List<Rigidbody>();
    [SerializeField]
    private bool ragdoll = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ragdoll) {
            TurnOnRagdoll();
        }
    }

    public void TurnOnRagdoll() {
        _animator.enabled = false;
        foreach (Rigidbody rigidbody in rigidBodies) {
            rigidbody.isKinematic = false;
        }
    }
    public void TurnOffRagdoll() {
        foreach (Rigidbody rigidbody in rigidBodies) {
            rigidbody.isKinematic = true;
        }
    }
}
