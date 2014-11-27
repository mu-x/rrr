using System;
using System.Collections.Generic;

public static partial class Say
{
    /** Sets up english translations */
    public static void English()
    {
        RRR = "Real Russian Racing";
        TOUCH = "... touch to continue ...";
        START = "Start Race";

        TRACKS = new Dictionary<int, string>()
        {
            { 1, "Forest Ring" }, { 2, "Long Road" }
        };

        PRICE = "Price";
        STEARING = "Stearing";
        ARMOR = "Armor";
        FRONT = "Front drive";
        REAR = "Rear drive";

        COUNT = new[] { "GO!", "STADY", "READY" };

        RESUME = "Resume the game";
        RESTART = "Restart the race";
        MENU = "Main menu";

        RM_RF = "Free Ride";
        RM_CC = "Checkpoint Chase {0} lap(s)";
        RM_AI = "Real Race {0} lap(s), {1} oponent(s)";

        TIME = "Race Time";
        LAPS = "Lap {0} of {1} total";
        CHECK = "Checkpoints: {0}";
        PLACE = "Position {0} of {1}";
    }
}

