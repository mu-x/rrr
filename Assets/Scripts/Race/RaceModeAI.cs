using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using UnityEngine;

public class RaceModeAI : RaceMode
{
    int laps, oponents, position;

    public RaceModeAI(int laps = 1, int oponents = 1)
    {
        this.laps = laps;
        this.oponents = oponents;
    }

    public override string info
    { get {
        return string.Format(
            "Race against {0} oponent(s) {1} lap(s)", oponents, laps);
    } }

    public override void Prepare(GameObject playerModel,
                                 GameObject[] others,
                                 Action<string> finish)
    {
        base.Prepare(playerModel, others, finish);
        player.onFinish = delegate()
        {
            int stars = Mathf.Clamp(4 - position, 0, allDrivers.Count - position);
            finish(status + "\nYou won " + stars + " star(s)!");
        };

        UnityEngine.Random.seed = (int)DateTime.Now.ToFileTime();
        var ais = UnityEngine.Object.FindObjectsOfType<DriverAI>();
        foreach (var ai in ais.Take(oponents).ToArray())
        {
            ai.carModel = others[UnityEngine.Random.Range(0, others.Length)];
            allDrivers.Add(ai);
        }

        foreach (var driver in allDrivers)
        {
            driver.roundsExpected = laps;
            driver.onVisit = delegate()
            {
                // NOTE: Build in sort is a BULLSHIT!!
                //       It alweys swap EQUAL values
                // FIXME: Replace with the other sort
                for (int i = 0; i != allDrivers.Count - 2; ++i)
                    if (allDrivers[i].pointsPassed <
                        allDrivers[i + 1].pointsPassed)
                    {
                        var tmp = allDrivers[i];
                        allDrivers[i] = allDrivers[i + 1];
                        allDrivers[i + 1] = tmp;
                    }
                position = allDrivers.IndexOf(player) + 1;
            };
        }
    }

    public override string status
    { get {
        return string.Format("{0}\nLap {1} / {2}, Place {3} / {4}", base.status,
            (player.roundsPassed + 1),  laps, position, allDrivers.Count);
    } }
}
