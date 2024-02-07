using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitMeshMatrix : MonoBehaviour
{
    private float meshCount;
    private float[][] resistanceMatrix;
    private float[] meshVoltageMatrix;

    CircuitMeshMatrix(int meshCount)
    {
        this.meshCount = meshCount;
        //meshDataMatrix = { { 1, 2, 3} };
        //meshVoltageMatrix = new float[meshCount];
    }

    public int GetDet()
    {
        // TODO : GetDet
        return 0;
    }

    public float[][] GetReverseResistanceMatrix()
    {
        // WARNING : Det should not be equal to 0
        // TODO : GetReverseDataMatrix
        return resistanceMatrix;
    }

    public float[] GetCurrentMatrix()
    {
        // WARNING : We need the reserse resistance matrix so det != 0
        // TODO : GetCurrentMatrix
        return meshVoltageMatrix;
    }
}
