using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[Obsolete("Old config for rooms before the auto detection system")]
[Serializable]
public class Room : MonoBehaviour
{
    public bool Start;
    public bool TopExit;
    public bool LeftExit;
    public bool RightExit;
    public bool BottomExit;
    public bool Boss;
}
