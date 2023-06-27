using System;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig {
    public static int MIN_NUM_PLAYER = 2;
    public static int MAX_NUM_PLAYER = 4;
    public static List<Vector2> INITIAL_DIRECTION = new List<Vector2>(){
        Vector2.right,
        Vector2.down,
        Vector2.left,
        Vector2.up,
    };
}