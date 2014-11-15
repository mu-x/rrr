using System.Collections;
using System.Linq;
using System;
using UnityEngine;

/** Race Check Points against the time */
public class RaceModeCC : RaceMode
{
    int laps = 0;
    public RaceModeCC(int laps = 1) { this.laps = laps; }

    public override string info
    { get {
        return "Checkpoint Chase\nLaps: " + laps;
    } }

    public override void Prepare(GameObject playerModel, Action<string> endGame)
    {
        base.Prepare(playerModel, endGame);
        player.roundsExpected = laps;
        player.onFinish = delegate()
        {
            string spp = (playTime / player.pointsPassed).ToString("00.000");
            endGame(status + "\n" + spp + " seconds per checkpoint");
        };
    }

    public override string status {
    get {
        return string.Join("\n", new[]
        {
            base.status,
            "Lap " + (player.roundsPassed + 1) + " of " + laps + " total",
            "Checkpoints: " + player.pointsPassed
        });

    } }
}
