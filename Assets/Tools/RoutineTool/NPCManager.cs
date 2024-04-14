using UnityEngine;
/// <summary>
/// Class to link the Routine Tool to the NPCs in the scene;
/// <br></br> Has no runtime value; thus, it is destroyed;
/// </summary>
public class NPCManager : MonoBehaviour {
    public T[] LoadEntities<T>() where T : Entity => GetComponentsInChildren<T>(true);
    void OnEnable() => Destroy(this);
}
