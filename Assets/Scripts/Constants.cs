using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotsGame
{
    // constants. Some of these could come from the server, like max symbols or max runs
    // used to allocate memory for pools
    public class Constants
    {
        public static int NUM_REELS = 3;
        public static int MAX_SYMBOLS = 32;
        public static int MAX_RUNS = 1024;
    }
}