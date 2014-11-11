using System.Collections;
using System;
using UnityEngine;

/** Controls part of the game process according to the race mode
 * (default: free race, inherit+override for other kind) */
public abstract class RaceMode {
    public abstract string info { get; } /**< mode description */

    public virtual void FixedUpdate() {
        playTime += Time.deltaTime;
    }

    public virtual void Prepare(Player player, GameObject playerModel,
                                GameObject[] checkpoints = null,
                                Action<string> finish = null) {
        (this.player = player).Prepare(playerModel);
    }

    public virtual void Race() {
        player.isEnabled = true;
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

/** Default @interface RaceMode implementation */
public class FreeRideRaceMode : RaceMode {
    public override string info { get { return "Free Ride\nUnleashed"; } }
}