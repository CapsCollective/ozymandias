using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPersistentEvent
{
    int Turns { get; set; }

    void Init();
}
