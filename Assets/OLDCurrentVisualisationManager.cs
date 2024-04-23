using System;
using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

using ElectricMeshList = System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<ElectricComponent>>;
public class CurrentVisualisationManager : MonoBehaviour 
{
    public static void StartParticleEmissions(ElectricMeshList meshList, Vector<float> voltageMatrix)
    {
        HashSet<ElectricComponent> handledCorners = new();

        for (int i = 0; i < meshList.Count; i++)
        {
            foreach (ElectricComponent component in meshList[i])
            {
                if (component.GetComponent<PowerSource>() != null)
                {
                    break;
                }

                if (Math.Sign(component.componentPotential) == Math.Sign(voltageMatrix[i]))
                {
                    IterateAndStartEmitting(handledCorners, component);
                    break;
                }
            }
        }
    }

    private static void IterateAndStartEmitting(HashSet<ElectricComponent> handledCorners, ElectricComponent source)
    {
        IterateAndStartEmitting(handledCorners, source, null);
    }

    private static void IterateAndStartEmitting(HashSet<ElectricComponent> handledCorners, ElectricComponent source, ElectricComponent previous)
    {
        Connection sourceConnection = source.GetComponent<Connection>();
        if (sourceConnection.IsFlatConnection())
        {
            IterateAndStartEmitting(handledCorners, sourceConnection.GetOppositeComponent(previous), previous);
        }

        float sourceIntensity = source.currentIntensity;

        ElectricComponent[] otherComponents = sourceConnection.GetAllOtherConnections(source);


    }

    private static bool IsHighestValue(float value, float[] others)
    {
        foreach (float f in  others)
        {
            if (value <= f)
            {
                return false;
            }
        }

        return true;
    }

    private static float GetHighestValue(float[] values)
    {
        float currentHighest = 0;
        foreach (float f in values)
        {
            if (f > currentHighest)
            {
                currentHighest = f;
            }
        }

        return currentHighest;
    }
}
