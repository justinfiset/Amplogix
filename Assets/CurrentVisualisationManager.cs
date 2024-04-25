using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using MathNet.Numerics.LinearAlgebra;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

using ElectricMeshList = System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<ElectricComponent>>;
public class CurrentVisualisationManager : MonoBehaviour
{
    private static HashSet<ElectricComponent> handledComponents;
    private static HashSet<ElectricComponent> emittingComponents;
    public static bool isSetup { get; private set; } = false;
    public static bool doVisualCurrent = true;

    public static void StartEmission(MatrixEquationSystem circuitData)
    {
        StartParticleEmissions(circuitData.meshList, circuitData.meshCurrent);
    }

    public static void ResumeEmission()
    {
        foreach(ElectricComponent component in emittingComponents)
        {
            component.GetComponent<CurrentVisualisation>().ResumeParticleMovements();
        }
    }

    public static void PauseEmission()
    {
        foreach (ElectricComponent component in emittingComponents)
        {
            component.GetComponent<CurrentVisualisation>().PauseParticles();
        }
    }

    public static void StopEmission()
    {
        ResetParticleEmissions();
    }

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

        handledComponents = new();
        emittingComponents = new();

        for (int i = 0; i < meshList.Count; i++)
        {
            List<ElectricComponent> clockwise = GetClockWiseOrder(meshList[i]);

            if (meshCurrents[i] > 0)
            {
                IterateAndStartEmitting(clockwise[0], null, 0, meshCurrents, meshList, i, handledComponents, clockwise);
            } else if (meshCurrents[i] < 0)
            {
                //clockwise.Reverse();
                IterateAndStartEmitting(clockwise[0], null, 0, meshCurrents, meshList, i, handledComponents, clockwise);
            }
            
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
            // m�me chose si on est d�j� handled
        }

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

    private static void AlternateIterateAndStartEmitting(ElectricComponent component, ElectricComponent lastCorner, 
        ElectricComponent firstCorner,  int componentIndex,  Vector<float> meshCurrents, ElectricMeshList meshList,
        int meshIndex, List<ElectricComponent> clockwise, BranchStorage branches)
    {
        if (lastCorner == null) // doesn't have previous corner
        {
            if (component.GetComponent<Connection>().IsFlatConnection()) // is not a corner
            {
                AlternateIterateAndStartEmitting(GetNextComponent(clockwise, componentIndex), component,
                    component, ++componentIndex, meshCurrents, meshList, meshIndex, clockwise, branches);
            } else // is a corner
            {
                AlternateIterateAndStartEmitting(GetNextComponent(clockwise, componentIndex), null,
                    null, ++componentIndex, meshCurrents, meshList, meshIndex, clockwise, branches);
            }

            return;
        }

        if (component == firstCorner) // if iteration is over
        {
            return;
        }

        

    }

    private static void CornerBasedEmission(ElectricMeshList meshList, Vector<float> meshCurrents)
    {
        Dictionary<int, (List<ElectricComponent>, BranchStorage)> meshCornerLists = BuildMeshCorners(meshList);

        BranchStorage handledBranches = new();

        for (int i = 0; i < meshCornerLists.Count; i++) // pour chaque mesh dans le systeme
        {
            foreach ((ElectricComponent, ElectricComponent) branch in meshCornerLists[i].Item2.branches) // pour chaque branche du mesh
            {
                if (handledBranches.Contains(branch))
                {
                    continue;
                }

                int sign = GetBranchSignInMesh(branch, i, meshCornerLists, meshCurrents); // sign du petit i de la branche
                int meshSign = Math.Sign(meshCurrents[i]); // sign du grand I dans le mesh

                if (sign == 0) // aucun courant dans la branche (possible?)
                {
                    continue;
                }

                if (sign == meshSign) // on emet du bon cote
                {
                    StartEmission(branch.Item1, branch.Item2);
                } else
                {
                    StartEmission(branch.Item2, branch.Item1);
                }
            }
        }
    }

    private static Dictionary<int, (List<ElectricComponent>, BranchStorage)> BuildMeshCorners(ElectricMeshList meshList)
    {
        List<ElectricComponent> cornerList = new();
        Dictionary<int, (List<ElectricComponent>, BranchStorage)> meshCornerLists = new();

        for (int i = 0; i < meshList.Count; i++) // build meshCornerLists
        {
            foreach (ElectricComponent component in GetClockWiseOrder(meshList[i])) // build corner list for mesh
            {
                if (!component.connectionManager.IsFlatConnection())
                {
                    cornerList.Add(component);
                }
            }
            meshCornerLists.Add(i, (cornerList, new BranchStorage()));


            for (int j = 1; j < cornerList.Count; j++) // build branch list for mesh
            {
                meshCornerLists[i].Item2.AddBranch((cornerList[j - 1], cornerList[j]));
            }
            meshCornerLists[i].Item2.AddBranch((cornerList[cornerList.Count], cornerList[0]));

            cornerList = new();
        }

        return meshCornerLists;
    }

    private static int GetBranchSignInMesh((ElectricComponent, ElectricComponent) branch, int meshIndex,
        Dictionary<int, (List<ElectricComponent>, BranchStorage)> meshCornerLists, Vector<float> meshCurrents)
    {
        float thisMeshCurrent = meshCurrents[meshIndex];
        int otherMeshIndex = -1;
        bool otherMeshFound = false;

        for (int i = 0; i < meshCornerLists.Count; i++) // trouver l'autre maille avec la branch
        {
            if (i != meshIndex && meshCornerLists[i].Item2.Contains(branch))
            {
                otherMeshIndex = i;
                otherMeshFound = true;
                break;
            }
        }

        if (!otherMeshFound)
        {
            return Math.Sign(meshCurrents[meshIndex]);
        }

        float currentInBranch = thisMeshCurrent - meshCurrents[otherMeshIndex];

        return Math.Sign(currentInBranch);
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
                     //print("component found in common");
                     //print("in this mesh i = " + meshCurrents[thisMeshIndex]);
                     //print("in other mesh i = " + meshCurrents[i]);
                     if (Math.Abs(meshCurrents[thisMeshIndex]) > Math.Abs(meshCurrents[i]))
                     {
                         return 1;
                     }
                     else if (Math.Abs(meshCurrents[thisMeshIndex]) < Math.Abs(meshCurrents[i]))
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
        //print("starting emission from " + source + " to " + target);

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

    private class BranchStorage
    {
        public readonly HashSet<(ElectricComponent, ElectricComponent)> branches;

        public BranchStorage()
        {
            branches = new();
        }

        public void Reset()
        {
            branches.Clear();
        }

        public bool AddBranch((ElectricComponent, ElectricComponent) branch)
        {
            foreach ((ElectricComponent, ElectricComponent) componentPair in  branches)
            {
                if (AreBranchesEqual(componentPair, branch))
                {
                    return false;
                }
            }

            branches.Add(branch);
            return true;
        }

        public bool Contains((ElectricComponent, ElectricComponent) branch)
        {
            foreach ((ElectricComponent, ElectricComponent) componentPair in branches)
            {
                if (AreBranchesEqual(componentPair, branch))
                {
                    return true;
                }
            }
            return false;
        }

        public int Count()
        {
            return branches.Count;
        }

        public static bool AreBranchesEqual((ElectricComponent, ElectricComponent) branch1, (ElectricComponent, ElectricComponent) branch2)
        {
            List<ElectricComponent> branch1Items = new List<ElectricComponent>(2)
            {
                [0] = branch1.Item1,
                [1] = branch1.Item2
            };

            List<ElectricComponent> branch2Items = new List<ElectricComponent>(2)
            {
                [0] = branch2.Item1,
                [1] = branch2.Item2
            };

            foreach (ElectricComponent component1 in branch1Items)
            {
                if (!branch2Items.Contains(component1))
                {
                    return false;
                }
            }

            return true;
        }
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
