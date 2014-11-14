using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using UnityEngine;

public class RaceModeAI : RaceMode {
    public RaceModeAI(int laps = 1, int oponents = 1) {
        this.laps = laps;
        this.oponents = oponents;
    }

    public override string info {
        get { return "Real Race\nLaps: " + laps + ", Oponents: " + oponents; }
    }

    public override void Prepare(GameObject playerModel, Action<string> endGame) {
        UnityEngine.Random.seed = (int)(DateTime.Now.ToOADate() * 1000);
        var cars = Resources.LoadAll<GameObject>("Car Models");
        var route = UnityEngine.Object.FindObjectOfType<Route>().Points();
        var all = new List<Driver>();
        Action end = delegate() {
            endGame(status + "\nYou " + (
                (drivers[0] == player) ? "WON!" : "LOSE!"));
        };

        player = UnityEngine.Object.FindObjectOfType<Player>();
        player.Prepare(playerModel, route, laps, end);
        all.Add(player);

        var ais = UnityEngine.Object.FindObjectsOfType<RacerAI>();
        foreach (var r in ais.Take(oponents).ToArray()) {
            int index = UnityEngine.Random.Range(0, cars.Length);
            r.Prepare(cars[index], route, laps, end);
            all.Add(r);
        }

        drivers = all.ToArray();
    }

    public override string status {
        get {
            return string.Join("\n", new[] {
                base.status,
                "Lap " + (player.roundsPassed + 1) + " of " + laps + " total",
                position
            });
        }
    }

    public string position {
        get {
            Array.Sort(drivers, (a, b) => -Driver.Compare(a, b));
            int place = Array.IndexOf(drivers, player) + 1;
            return "Place: " + place + " of " + drivers.Length;
        }
    }

    int oponents;
    Driver[] drivers;

    int laps;
    Transform[] route;
}
