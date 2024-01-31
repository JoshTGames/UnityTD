using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGenerator : MonoBehaviour
{
    [SerializeField] MapGen mapGen;
    [SerializeField] int depth = 0;

    // Update is called once per frame
    void LateUpdate(){
        mapGen.TARGET_DEPTH = depth;
    }
}
