using System;
using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

using ElectricMeshList = System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<ElectricComponent>>;
public class CurrentVisualisationManager : MonoBehaviour 
{
    private static HashSet<ElectricComponent> handledComponents;
    public static void StartParticleEmissions(ElectricMeshList meshList, Vector<float> meshCurrent)
    {
        /*
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
        */

        //commencer dans le sens inverse des meshlist?
        handledComponents = new();
        



    }

    private static void IterateAndStartEmitting(ElectricComponent component, HashSet<ElectricComponent> handledComponents, Vector<float> voltageMatrix,
        ElectricComponent firstHandledInMesh, System.Collections.Generic.List<ElectricComponent> componentList, ElectricComponent lastCorner, int i)
    {
        if (component == firstHandledInMesh)
        {
            return;
        }

        Connection connection = component.GetComponent<Connection>();
        if (lastCorner == null || connection.IsFlatConnection())
        {
            IterateAndStartEmitting(GetNextComponent(componentList, i), handledComponents, voltageMatrix, firstHandledInMesh, componentList, lastCorner, i++);
            return;
        } 

        if (handledComponents.Contains(component))
        {
            IterateAndStartEmitting(GetNextComponent(componentList, i), handledComponents, voltageMatrix, firstHandledInMesh, componentList, lastCorner, i++);
            return;
        }

        if (Math.Sign(voltageMatrix[i]) == component.hypotheticalCurrentSign)
        {
            StartEmission(lastCorner, component);
        } else
        {
            StartEmission(component, lastCorner);
        }

        IterateAndStartEmitting(GetNextComponent(componentList, i), handledComponents, voltageMatrix, firstHandledInMesh, componentList, component, i++);
    }

    private static void StartEmission(ElectricComponent source, ElectricComponent target)
    {
        CurrentVisualisation emitter = source.GetComponent<CurrentVisualisation>();

        Vector2 builtVector = new();
        builtVector.x = target.transform.position.x;
        builtVector.y = target.transform.position.y;

        emitter.SetupTarget(builtVector);
        emitter.StartParticleEmission();
    }

    private static ElectricComponent GetNextComponent(System.Collections.Generic.List<ElectricComponent> componentList, int i)
    {
        return componentList[(i + 1) % componentList.Count];
    }

    /*
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
    */

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
