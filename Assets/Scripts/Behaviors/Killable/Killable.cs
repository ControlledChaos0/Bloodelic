using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//So this is just a behavior that interacts with the movement function within the Entity class
//Maybe I should just move out all the interaction code to here
//huh
public class Killable : Behavior
{
    /**
        Controlling Thing
    **/
    private Human _human;
    public Human Human {
        get => _human;
        set => _human = value;
    }

    /**
        Behavior Specific Variables
    **/
    private bool _isDead = false;
    public bool IsDead {
        get => _isDead;
        set => _isDead = value;
    }

    /**
        Identifiers
    **/
    public override string Name {
        get => "Drain";
    }
    public override bool NPCInteract {
        get => false;
    }
    public override bool Resetable {
        get => false;
    }
    
    #region Effects
    [SerializeField] ParticleSystem DeathParticles;
    [SerializeField] private Transform DeathEffectSpawnPoint;
    
    #endregion

    void Start() {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override BehaviorRoutine CreateRoutine()
    {
        return new KillableRoutine();
    }
    public override bool CheckValid()
    {
        if (IsDead) {
            return false;
        }
        foreach (GridCell gridCell in TurnSystem.Instance.ActiveEntity.OccupiedCell.Neighbors) {
            if (gridCell.IsOccupied() && gridCell.Occupant.Equals(_human)) {
                return true;
            }
        }
        return false;
    }
    public override void ResetBehaviorSpecifics()
    {
        IsDead = _human.IsDead;
    }
    public override void GetControllingComponent()
    {
        _human = GetComponent<Human>();
    }
    public override IEnumerator ExecuteBehaviorCoroutine()
    {
        Debug.Log("Executing Moveable Routine");
        
        // Spawn blood effects
        if (DeathParticles != null)
        {
            Transform targetTransform = DeathEffectSpawnPoint != null ? DeathEffectSpawnPoint : transform;
            ParticleSystem SpawnedParticles = 
                Instantiate<ParticleSystem>(DeathParticles, transform.position + Vector3.up * 0.65f, Quaternion.identity);
        }
        _human.IsDead = IsDead;
        _human.Die();
        
        SelectStateMachine.Instance.EndRoutine();
        enabled = false;
        yield break;
    }
}
