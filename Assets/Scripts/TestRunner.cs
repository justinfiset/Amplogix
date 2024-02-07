using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRunner : MonoBehaviour
{
    private void Start()
    {
        print("Tests:");
        MatrixUtil.TestDetSolver();
    }
}
