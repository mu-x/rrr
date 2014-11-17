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
        return string.Format("Checkpoint Chase {0} lap(s)", laps);
    } }

    public override void Prepare(GameObject playerModel,
                                GameObject[] others,
                                Action<string> finish)
    {
        base.Prepare(playerModel, others, finish);
        player.roundsExpected = laps;
        player.onFinish = delegate()
        {
            string spp = (playTime / player.pointsPassed).ToString("00.000");
            finish(status + "\n" + spp + " seconds per checkpoint");
        };
    }

    public override string status
    { get {
        return string.Format("{0}\nLap {1} / {2}, Checkpoints {3}", base.status,
            (player.roundsPassed + 1), laps, player.pointsPassed);
    } }
}
