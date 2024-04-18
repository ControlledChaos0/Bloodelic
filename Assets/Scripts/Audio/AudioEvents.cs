using UnityEngine;

public class AudioEvents : MonoBehaviour
{
    
    /*
     * Add FMOD Event References Here
     */
    
    //BackgroundMusic
    public static string backgroundMusic = "event:/bgm_ambience";
    
    //SFX
    public static string glassShatter = "event:/sfx_glassshatter";
    
    public static string menuSelect = "event:/sfx_churp";
    
    
    /*
     * Singleton Insurance
     */
    public static AudioEvents instance { get; private set; }
    private void Awake()
    {
        if (instance != null )
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}