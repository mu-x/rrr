using System.Collections;
using UnityEngine;

/** Controlls gart of the game process according to the race mode (default: 
 *  free race, inherit+override for other kind) */
public abstract class RaceMode {
    public delegate void EndGame(string message);
    public EndGame endGame;

    public virtual void FixedUpdate() { playTime += Time.deltaTime; }
    public virtual void Start(CheckPoint[] mapRoute, EndGame endGame) {}

    public virtual string Status() { 
        var min = (int)playTime / 60;
        var sec = playTime - min * 60f;
        return "Race Time: " + min + ":" + sec.ToString("00.00");
    }

    protected float playTime;
}

/** Default @interface RaceMode implementation */
public  class FreeRaceMode : RaceMode {
    public override string ToString() { return "Free Ride\nUnleashed"; }
}

/** Race Check Points against the time */
public class CheckpointRaceMode : RaceMode {
    public CheckpointRaceMode(int laps = 1) { this.laps = laps; }
    public override string ToString() { return "Check Point Chase\nLaps: " + laps; }

    public override void Start(CheckPoint[] mapRoute, EndGame endGame) {
        checkPoints = new Selector<CheckPoint>(
            mapRoute, null,
            delegate (CheckPoint next) { passed++; next.isEnabled = true; },
            delegate () { if (++lap > laps) endGame(Status()); }
        );

        foreach (var point in mapRoute)
            point.playerEnter = delegate () { checkPoints.Next(); };

        passed = 0;
        lap = 1;
    }

    public override string Status() {
        return base.Status() +  "\n" +
            "Lap: " + lap + "/" + laps + ", Points: " + passed;
    }

    int laps, lap, passed = 0;
    ISelector checkPoints;
}
