using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

using ElectricMeshList = System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<ElectricComponent>>;
public class CurrentVisualisationManager : MonoBehaviour
{
    private static HashSet<ElectricComponent> handledComponents;
    private static HashSet<ElectricComponent> emittingComponents;
    public static bool isSetup { get; private set; } = false;
    public static bool doVisualCurrent = true;
    public static void StartParticleEmissions(ElectricMeshList meshList, Vector<float> meshCurrents)
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
        isSetup = true;

        if (meshList == null || meshList.Count == 0)
        {
            return;
        }

        print("started particle emission");

        handledComponents = new();
        emittingComponents = new();

        for (int i = 0; i < meshList.Count; i++)
        {
            /*
            print("starting iteration");
            print("meshList : ");
            
            foreach (ElectricComponent c in meshList[i])
            {
                print(c);
                print("connections : ");
                foreach (ElectricComponent comp in c.GetComponent<Connection>().connections.connections)
                {
                    print(comp);
                }
            }
            */
            List<ElectricComponent> clockwise = GetClockWiseOrder(meshList[i]);
            /*
            print("clockwise order : ");
            foreach (ElectricComponent component in clockwise)
            {
                print(component);
            }
            */
            IterateAndStartEmitting(clockwise[0], null, 0, meshCurrents, meshList, i, handledComponents, clockwise);
        }
    }

    private static List<ElectricComponent> GetClockWiseOrder(List<ElectricComponent> originalList)
    {
        float maxY = 0;
        List<ElectricComponent> builtList = new();
        bool firstIteration = true;
        ElectricComponent leftMostMaximum = null;
        ElectricComponent secondLeftMost = null;

        //find left most component on top
        foreach (ElectricComponent component in originalList)
        {
            if (firstIteration)
            {
                maxY = component.transform.position.y;
                leftMostMaximum = component;
                firstIteration = false;
            } else
            {
                if (component.transform.position.y == maxY)
                {
                    if (component.transform.position.x < leftMostMaximum.transform.position.x)
                    {
                        secondLeftMost = leftMostMaximum;
                        leftMostMaximum = component;
                    } else if (secondLeftMost == null || component.transform.position.x < secondLeftMost.transform.position.x)
                    {
                        secondLeftMost = component;
                    }

                } else if (component.transform.position.y > maxY)
                {
                    maxY = component.transform.position.y;
                    leftMostMaximum = component;
                    secondLeftMost = null;
                }
            }
        }

        builtList.Add(leftMostMaximum);
        firstIteration = true;
        ElectricComponent iterated = secondLeftMost;
        HashSet<ElectricComponent> handled = new();
        ElectricComponent previous = leftMostMaximum;

        IterateClockWiseAndAdd(secondLeftMost, leftMostMaximum, leftMostMaximum, builtList, originalList);
        //while (iterated.GetComponent<Connection>().GetAllOtherConnections(previous))
        return builtList;

    }

    private static void IterateClockWiseAndAdd(ElectricComponent component, ElectricComponent previous, ElectricComponent leftMostMaximum, 
        List<ElectricComponent> builtList, List<ElectricComponent> originalList)
    {
        if (previous == leftMostMaximum)
        {
            builtList.Add(component);
            foreach (ElectricComponent comp in component.GetComponent<Connection>().connections.connections)
            {
                if (comp != null && comp != leftMostMaximum)
                {
                    if (originalList.Contains(comp))
                    {
                        IterateClockWiseAndAdd(comp, component, leftMostMaximum, builtList, originalList);
                        return;
                    }
                }
            }
        } else if (component == leftMostMaximum)
        {
            //print("back at leftMostMax");
            return;
        } else
        {
            builtList.Add(component);
            foreach (ElectricComponent comp in component.GetComponent<Connection>().connections.connections)
            {
                if (comp != null && comp != previous)
                {
                    if (originalList.Contains(comp))
                    {
                        IterateClockWiseAndAdd(comp, component, leftMostMaximum, builtList, originalList);
                        return;
                    }
                }
            }
        }
        throw new Exception("next component not found");
    }

    private static void IterateAndStartEmitting(ElectricComponent component, ElectricComponent lastCorner, int componentIndex, Vector<float> meshCurrents, 
        ElectricMeshList meshList, int meshIndex, HashSet<ElectricComponent> handledComponents, List<ElectricComponent> clockwise)
    {
        //print("componentIndex = " + componentIndex);

        if (componentIndex == meshList[meshIndex].Count)
        {
            //print("back at first component");
            handledComponents.Add(component);
            StartEmission(lastCorner, component);
            return; // si on retombe sur le premier component on sort
        }

        Connection connection = component.GetComponent<Connection>();
        //List<ElectricComponent> compList = GetClockWiseOrder(meshList[meshIndex]);

        if (componentIndex == 0 && lastCorner == null)
        {
            //print("detected root");
            //print(component);

            handledComponents.Add(component);
            IterateAndStartEmitting(GetNextComponent(clockwise, 0), null, ++componentIndex, meshCurrents, 
                meshList, meshIndex, handledComponents, clockwise);
            return;
        }
        if (connection.IsFlatConnection())
        {
            //print("detected flat connection");
            //print(component);
            handledComponents.Add(component);
            IterateAndStartEmitting(GetNextComponent(clockwise, componentIndex), lastCorner, ++componentIndex, 
                meshCurrents, meshList, meshIndex, handledComponents, clockwise);
            return; // si on est pas encore tombe sur un coin
        } 

        if (lastCorner == null || handledComponents.Contains(component))
        {
            if (lastCorner == null)
            {
                //print("detected first corner");
                //print(component);
                StartEmission(clockwise[0], component);
            } else if (handledComponents.Contains(component))
            {
                //print("detected handled component");
                //print(component);
            }
            handledComponents.Add(component);
            IterateAndStartEmitting(GetNextComponent(clockwise, componentIndex), component, ++componentIndex, 
                meshCurrents, meshList, meshIndex, handledComponents, clockwise);
            return; // si on est un coin et qu'il n'y a pas de coin precedant, on itere en se settant comme lastCorner
            // même chose si on est déjà handled
        }

        /*
        if (handledComponents.Contains(component))
        {
            IterateAndStartEmitting(GetNextComponent(componentList, i), handledComponents, meshCurrent, firstHandledInMesh, componentList, lastCorner, i++);
            return; // si on est deja handled ailleurs, on itere en gardant le meme lastCorner
        }
        
        print("handling component");
        print(component);
        */

        if (GetSignInMesh(component, meshList, componentIndex, meshIndex, meshCurrents) == 1)
        {
            StartEmission(lastCorner, component); // si on a le meme signe que le courant on emet les particules depuis le dernier coin (horaire)
        } else
        {
            StartEmission(component, lastCorner); // sinon, on emet jusqu'au dernier coin (antihoraire) (si c'est zero on pleure)
        }
        // on itere en se settant comme lastCorner
        IterateAndStartEmitting(GetNextComponent(clockwise, componentIndex), component, ++componentIndex, 
            meshCurrents, meshList, meshIndex, handledComponents, clockwise);
    }

    private static int GetSignInMesh(ElectricComponent component, ElectricMeshList meshList, int componentIndex, int thisMeshIndex, Vector<float> meshCurrents)
    {
        for (int i = 0; i < meshList.Count; i++) // through each mesh in the list
        {
            if (i != thisMeshIndex) // if not the mesh we're in originally
            {
                //if it contains both the current component and the previous (common branch)
                 if (meshList[i].Contains(component) && meshList[i].Contains(meshList[thisMeshIndex][componentIndex - 1])) 
                 {
                     print("component found in common");
                    print("in this mesh i = " + meshCurrents[thisMeshIndex]);
                    print("in other mesh i = " + meshCurrents[i]);
                     if (meshCurrents[thisMeshIndex] > meshCurrents[i])
                     {
                         return 1;
                     }
                     else if (meshCurrents[thisMeshIndex] < meshCurrents[i])
                     {
                         return -1;
                     }
                     else
                     {
                         return 0; // panic
                     }
                    }
            }
        }

        return 1;
    }

    private static void StartEmission(ElectricComponent source, ElectricComponent target)
    {
        print("starting emission from " + source + " to " + target);

        CurrentVisualisation emitter = source.GetComponent<CurrentVisualisation>();

        Vector2 builtVector = new();
        builtVector.x = target.transform.position.x;
        builtVector.y = target.transform.position.y;

        emitter.StartParticleEmission(builtVector);

        handledComponents.Add(source);
        handledComponents.Add(target);
        emittingComponents.Add(source);
    }

    private static ElectricComponent GetNextComponent(List<ElectricComponent> componentList, int i)
    {
        return componentList[(i + 1) % componentList.Count];
    }

    public static void ResetParticleEmissions()
    {
        isSetup = false;
        if (emittingComponents == null)
        {
            emittingComponents = new();
            return;
        }

        foreach (ElectricComponent component in emittingComponents)
        {
            if (component != null)
            {
                CurrentVisualisation currentVisualisation = component.GetComponent<CurrentVisualisation>();
                currentVisualisation.KillParticleEmission();
            }
        }

        emittingComponents = new();
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
    */
}
