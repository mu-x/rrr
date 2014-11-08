using System.Collections;
using System.Linq;
using UnityEngine;

/** Default @interface RaceMode implementation */
public  class FreeRaceMode : RaceMode {
    public override string info { get { return "Free Ride\nUnleashed"; } }
}

/** Controls checkpoints visiting */
public class CheckpointControl {
    public int passed, rounds;

    public delegate void Finish();
    public CheckpointControl(GameObject[] route, int roundsExpected,
                             Finish finish, bool isPlayer = false) {
        this.selector = new Selector<GameObject>(
            route, null,
            delegate (GameObject next) {
                if (isPlayer)
                    next.renderer.enabled = true;

                passed++;
                expected = next;
            },
            delegate() {
                if (++rounds == roundsExpected)
                    finish();
            }
        );

        passed = rounds = 0;
        this.isPlayer = isPlayer;
    }

    public void enter(GameObject cp) {
        if (cp != expected)
            return;

        if (isPlayer)
            cp.renderer.enabled = false;

        selector.Next();
    }

    bool isPlayer;
    ISelector selector;
    GameObject expected;
}

/** Race Check Points against the time */
public class CheckpointRaceMode : RaceMode {
    public CheckpointRaceMode(int laps = 1) { this.laps = laps; }

    public override string info {
        get { return "Check Point Chase\nLaps: " + laps; }
    }

    public override CarControl Prepare(
                    Transform[] carSpots, GameObject[] mapRoute,
                    GameObject playerModel, EndGame endGame) {
        checkpoints = new CheckpointControl(
            mapRoute, laps,
            delegate () {
                string secs = (playTime / checkpoints.passed).ToString("00.000");
                endGame(status + "\n" + secs + " sec. per point");
            },
            isPlayer: true
        );

        var car = base.Prepare(carSpots, mapRoute, playerModel, endGame);
        car.checkpoints = checkpoints;
        return car;
    }

    public override string status {
        get {
            return string.Join("\n", new[] {
                base.status,
                "Lap " + checkpoints.rounds + " of " + laps + " total",
                "Checkpoints: " + checkpoints.passed
            });
        }
    }

    int laps;
    CheckpointControl checkpoints;
}
