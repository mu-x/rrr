using UnityEngine;
using UnityExtensions;

/** The Main music player in the game */
[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    public AudioClip[] soundtrack;
    public AudioSource audioSource;
    public int selectedTrack;

    // TODO: look up for better editor event
    void OnDrawGizmos()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        this.checkField("soundtrack", soundtrack);
        audioSource.clip = soundtrack[0];
    }

	void Start() { RandomTrack(); }

    /** Play random track from the @var soundtrack */
    public void RandomTrack(bool isInGame = false)
    {
        selectedTrack = Random.Range(0, soundtrack.Length - 1);
        audioSource.clip = soundtrack[selectedTrack];
        audioSource.Play();
        InGame(isInGame);
    }

    /** Changes volume if player is in game */
	public void InGame(bool isInGame = false) 
    {
        audioSource.volume = isInGame ? 0.3f : 1f;
	}

}
