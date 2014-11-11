using System.Collections;
using System.Linq;
using System;
using UnityEngine;

/** Race Check Points against the time */
public class RaceModeCC : RaceMode {
    public RaceModeCC(int laps = 1) { this.laps = laps; }

    public override string info {
        get { return "Checkpoint Chase\nLaps: " + laps; }
    }

    public override void Prepare(GameObject playerModel, Action<string> endGame) {
        var route = UnityEngine.Object.FindObjectOfType<Route>();
        player = UnityEngine.Object.FindObjectOfType<Player>();
        player.Prepare(
            playerModel, route.Points(), laps,
            delegate () {
                endGame(status + "\n" +
                    (playTime / player.pointsPassed).ToString("00.000") +
                    " sec. per point");
            }
        );
    }

    public override string status {
        get {
            return string.Join("\n", new[] {
                base.status,
                "Lap " + (player.roundsPassed + 1) + " of " + laps + " total",
                "Checkpoints: " + player.pointsPassed
            });
        }
    }

    int laps;
    Transform[] route;
}
