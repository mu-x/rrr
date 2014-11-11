using System.Collections;
using System.Linq;
using System;
using UnityEngine;

/** Race Check Points against the time */
public class CheckpointChaseRaceMode : RaceMode {
    public CheckpointChaseRaceMode(int laps = 1) { this.laps = laps; }

    public override string info {
        get { return "Check Point Chase\nLaps: " + laps; }
    }

    public override void Prepare(Player player, GameObject playerModel,
                                 GameObject[] mapRoute, Action<string> endGame) {
        (this.player = player).Prepare(
            playerModel, mapRoute, laps,
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
}
