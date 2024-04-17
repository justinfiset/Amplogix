using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using MathNet.Numerics.LinearAlgebra;
using System.Collections;

// simplification des listes
using ElectricMeshList = System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<ElectricComponent>>;

public class MeshBuilder : MonoBehaviour
{
    public static List<List<ElectricComponent>> RemoveDuplicatedMeshes(List<List<ElectricComponent>> original)
    {
        List<List<ElectricComponent>> uniqueMeshes = new();

        foreach(List<ElectricComponent> unsafeMesh in original)
        {
            bool wasFound = false;
            foreach(List<ElectricComponent> uniqueMesh in uniqueMeshes)
            {
                if(unsafeMesh.Count == uniqueMesh.Count)
                {
                    if(uniqueMesh.All(unsafeMesh.Contains))
                    {
                        wasFound = true;
                        break;
                    }
                }
            }

            if(!wasFound)
            {
                uniqueMeshes.Add(unsafeMesh);
            }
        }

        print("Unique mesh detected: " + uniqueMeshes.Count);
        return uniqueMeshes;
    }

    /*
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            CreateAndCalculateMeshes();
        }
    }
    */

    #region Mesh Creation
    public static MatrixEquationSystem CreateAndCalculateMeshes()
    {
        // Preps avant les calculs
        ProjectManager.ResetCurrentIntensity();

        try
        {
            ElectricMeshList meshList = CreateMeshes();
            Vector<float> voltageMatrix = GetVoltageMatrix(meshList);
            Matrix<float> resistanceMatrix = GetResistanceMatrix(meshList);
            MatrixEquationSystem system = new MatrixEquationSystem(resistanceMatrix, voltageMatrix);

            // todo séparer dans une autre méthode
            List<ElectricComponent> calledComponents = new List<ElectricComponent>();
            for(int i = 0; i < system.meshCount; i++)
            {
                float meshCurrent = system.meshCurrent[i];
                float secondMeshCurrent = 0f;
                foreach(ElectricComponent component in meshList[i])
                {
                    if(!calledComponents.Contains(component)) // Si pas déja appelé
                    {
                        // On vérifie si un composant est contenu dans plusieurs mailles
                        for (int j = 0; j < system.meshCount; j++)
                        {
                            if (j != i)
                            {
                                bool wasFound = false;
                                foreach (ElectricComponent temp in meshList[j])
                                {
                                    if (component == temp)
                                    {
                                        wasFound = true;
                                        break;
                                    }
                                }

                                if(wasFound)
                                {
                                    secondMeshCurrent = system.meshCurrent[j];
                                    break;
                                }
                            }
                        }

                        float current = Math.Abs(Math.Abs(meshCurrent) - Math.Abs(secondMeshCurrent));
                        component.SetCalculatedIntensity(current);
                        calledComponents.Add(component);
                    }
                }
            }

            return system;
        } catch (IncorrectCircuitException e)
        {
            print(e.Message);
        }

        return null;
    }

    public static ElectricMeshList CreateMeshes()
    {
        ElectricMeshList meshList = new ElectricMeshList();

        List<ElectricComponent> componentList = ProjectManager.GetAllConnectedComponents();

        List<List<ElectricComponent>> unsafeMeshList = new();
        if (componentList.Count > 0)
        {
            foreach (ElectricComponent root in componentList)
            {
                if(root.type != ElectricComponentType.Wire)
                {
                    HashSet<ElectricComponent> ancestors = new HashSet<ElectricComponent>();
                    AnalyseConnections(root, null, root, ancestors, unsafeMeshList);
                }
            }
        } else
        {
            throw new IncorrectCircuitException("Un circuit doit avoir au moin un composant!");
        }

        unsafeMeshList = RemoveIncorrectMeshes(unsafeMeshList);
        unsafeMeshList = RemoveDuplicatedMeshes(unsafeMeshList);
        DetectShortCircuit(unsafeMeshList);
        PopulateMeshMap(unsafeMeshList, meshList);
        return meshList;
    }

    public static List<List<ElectricComponent>> RemoveIncorrectMeshes(List<List<ElectricComponent>> toCorrect)
    {
        List<List<ElectricComponent>> correctMeshes = new List<List<ElectricComponent>>();
        foreach(List<ElectricComponent> mesh in toCorrect)
        {
            if(mesh.Count >= 4) // Doit avoir plus de 4 composants
            {
                correctMeshes.Add(mesh);
            }
            else
            {
                throw new IncorrectCircuitException("A mesh needs to have more than four components");
            }
        }
        return correctMeshes;
    }

    public static void PopulateMeshMap(List<List<ElectricComponent>> setList, ElectricMeshList meshList)
    {
        int index = 0;

        foreach(List<ElectricComponent> set in setList)
        {
            meshList.Add(index, set.ToList());
            index++;
        }
    }

    // retourne vrai si on a trouvé une maille, faux si on doit continuer la recherche
    public static bool AnalyseConnections(ElectricComponent node, ElectricComponent parent, ElectricComponent root, HashSet<ElectricComponent> ancestors, List<List<ElectricComponent>> list)
    {
        ancestors.Remove(root); // Par mesure de sécurité, on veut séparer les ancetres de la racine
        if(node != null) // Empeche de lancer une exception
        {
            // Les conditions d'arrêts
            if (!IsMeshComponentValid(node)) // Si un composant n'est pas valide ex interrupteur ouvert
            {
                return false;
            }
            else if (parent != null)
            { // Si on est pas à la première itération (1 de base)
                if (root != null && node == root) // On a trouvé une maille
                {
                    List<ElectricComponent> mesh = new List<ElectricComponent>() { root, parent };
                    mesh.AddRange(ancestors);
                    list.Add(mesh);
                    return true;
                }
                else if (ancestors.Contains(node)) // Par sécurité
                {
                    return false; // Ca ne sert à rien d'explorer cette branche
                }
            }


            List<ElectricComponent> childrens = node.connectionManager.connections.connections.ToList();
            //childrens.RemoveAll(component => component == null);
            foreach (ElectricComponent edge in childrens)
            {
                if (edge != null && edge != parent && !ancestors.Contains(edge))
                {
                    HashSet<ElectricComponent> childrenAncestors = new();
                    childrenAncestors.UnionWith(ancestors);

                    if(root != null && edge != root) childrenAncestors.Add(node);

                    if (AnalyseConnections(edge, node, root, childrenAncestors, list))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
        //throw (new Exception("Node Analysis was called for a null node!"));
    }

    // TODO prendre en compte la fem pour une source
    public static void DetectShortCircuit(List<List<ElectricComponent>> setList)
    {
        // Les composants qui n'ont aucune résistance
        List<ElectricComponentType> unsafeTypes = new() { 
            ElectricComponentType.Wire, 
            ElectricComponentType.PowerSource, // IMPLEMENTER LA FEM 
            ElectricComponentType.Switch
        };

        foreach(List<ElectricComponent> set in setList)
        {
            bool isShortCircuit = true;
            foreach(ElectricComponent component in set)
            {
                ElectricComponentType type = component.type;
                if(!unsafeTypes.Contains(type))
                {
                    isShortCircuit = false;
                    break;
                }
            }

            if(isShortCircuit)
            {
                throw new ShortCircuitException();
            }
        }
    }

    public static bool IsMeshComponentValid(ElectricComponent component)
    {
        switch (component.type)
        {
            case ElectricComponentType.Switch:
                return !((ElectricSwitch)component).isOpen;
            default: return true;
        }
    }
    #endregion

    #region Matrice de potentiel
    public static int GetPowerSourceMultiplier(Connection previous, ElectricComponent component)
    {
        int multiplier = 1;

        for (int i = 0; i < previous.connections.connections.Length; i++)
        {
            if (previous.connections.connections[i] == component)
            {
                float expectedAngle = 0f;
                float angle = component.transform.localEulerAngles.z % 360; // permet d'avoir des valeurs parmit 0, 90, 180, 

                Connection.Position orientation = (Connection.Position)i;
                switch (orientation)
                {
                    case Connection.Position.Left:
                        expectedAngle = 0f; break;
                    case Connection.Position.Right:
                        expectedAngle = 180f; break;
                    case Connection.Position.Top:
                        expectedAngle = 90f; break;
                    case Connection.Position.Bottom:
                        expectedAngle = 270f; break;
                }

                multiplier = (expectedAngle == angle) ? 1 : -1;
                break;
            }
        }
        return multiplier;
    }

    // TODO tester
    // TODO prendre en compte les condensateurs et / ou bobines
    // On analyse les sources en sens horaire selon les lois de kirchoff
    public static float GetMeshVoltage(List<ElectricComponent> mesh)
    {
        float meshVoltage = 0f;

        ElectricComponent head = mesh.First();
        ElectricComponent previous = null;
        foreach (ElectricComponent component in mesh)
        {
            float prevAngle = (previous == null) ? 0 : previous.transform.localEulerAngles.z;
            float angle = component.transform.localEulerAngles.z % 360; // permet d'avoir des valeurs parmit 0, 90, 180, 

            if (component.type == ElectricComponentType.PowerSource)
            {
                PowerSource powerSource = (PowerSource)component;

                /*
                ////////////////////////////////////////////////////////////
                powerSource.SetColor(Color.green); // TODO REMOVE (FOR DEBUG)
                ////////////////////////////////////////////////////////////
                */

                int multiplier = 1;
                // TODO aller cherche le previous manuellement?
                if (previous != null && previous.type == ElectricComponentType.Wire)
                    multiplier = GetPowerSourceMultiplier(previous.connectionManager, component);
                else // si on a pas un fil avant
                    multiplier = (prevAngle == angle) ? -1 : 1;

                multiplier = multiplier * -1; // TODO bug fix
                meshVoltage += powerSource.voltage * multiplier;
            }

            previous = component;
        }
        // TODO FIX DOIT DONNER LE BON SIGNE
        return meshVoltage;
    }

    public static Vector<float> GetVoltageMatrix(ElectricMeshList meshList)
    {
        Vector<float> voltageMatrix = Vector<float>.Build.Dense(meshList.Count);

        /*
        ////////////////////// DEBUG CODE
        ProjectManager.ChangeAllComponentsColor(Color.black);
        List<Color> colors = new List<Color> { Color.red, Color.blue, Color.cyan, Color.yellow };
        int index = 0;
        foreach (KeyValuePair<int, List<ElectricComponent>> mesh in meshList)
        {
            string value = "Composants: ";
            foreach (ElectricComponent comp in mesh.Value)
            {
                if (comp.type != ElectricComponentType.PowerSource)
                {
                    value += comp.name + ", ";
                    comp._SetColor(colors[index]);
                }
            }
            print(value);
            index++;
        }
        //////////////////////////////////////////////////
        */

        foreach (KeyValuePair<int, List<ElectricComponent>> mesh in meshList)
        {
            float result = GetMeshVoltage(mesh.Value);
            voltageMatrix[mesh.Key] = result;
        }

        return voltageMatrix;
    }

    #endregion

    #region Matrice de résistance
    // TODO : prendre en compte la FEM et la résistivité des fils
    public static Matrix<float> GetResistanceMatrix(ElectricMeshList meshList)
    {
        // Matrice [Count, Count]
        Matrix<float> resistanceMatrix = Matrix<float>.Build.Dense(meshList.Count, meshList.Count);

        List<int> managedKeys = new List<int>();
        foreach (KeyValuePair<int, List<ElectricComponent>> main in meshList)
        {
            foreach (KeyValuePair<int, List<ElectricComponent>> secondary in meshList)
            {
                float resistance = 0f;
                // Si on passe pas sur la diagonale et que l'on n'a pas déjà géré les résistances
                if (main.Key != secondary.Key && !managedKeys.Contains(secondary.Key))
                {
                    if (!managedKeys.Contains(secondary.Key))
                    {
                        foreach (ElectricComponent component in main.Value)
                        {
                            if(secondary.Value.Contains(component))
                            {
                                resistance += component.resistance;
                            }
                        }

                        // c'est égale de chaque coté, Ex: R12, R21, etc...
                        resistanceMatrix[secondary.Key, main.Key] = resistance;
                        resistanceMatrix[main.Key, secondary.Key] = resistance;
                    }
                }
                else // La diagonale ex : R11, R22, R33
                {
                    foreach (ElectricComponent resistor in main.Value)
                    {
                        resistance += resistor.resistance;
                    }
                    resistanceMatrix[main.Key, main.Key] = resistance;
                }
            }
        }

        return resistanceMatrix;
    }
    #endregion
}
