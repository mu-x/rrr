using System.Collections;
using System;
using UnityEngine;

/** Controls part of the game process according to the race mode
 * (default: free race, inherit+override for other kind) */
public class RaceMode {
    public virtual string info { get { return "Free Ride\nUnleashed"; } }

    public virtual void FixedUpdate() {
        playTime += Time.deltaTime;
    }

    public virtual void Prepare(GameObject playerModel, Action<string> finish) {
        player = UnityEngine.Object.FindObjectOfType<Player>();
        player.Prepare(playerModel);
    }

    public virtual string status {
        get {
            var min = (int)playTime / 60;
            var sec = playTime - min * 60f;
            return "Race Time: " + min + ":" + sec.ToString("00.00");
        }
    }

    protected float playTime;
    protected Player player;
}