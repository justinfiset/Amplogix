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
    ElectricMeshList meshList = new ElectricMeshList();

    // TODO Tester - RemoveDuplicatedMeshes
    public List<List<ElectricComponent>> RemoveDuplicatedMeshes(List<List<ElectricComponent>> original)
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

    public void Update()
    {
        // TODO REMOVE TESET
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            CreateAndCalculateMeshes();
        }
    }

    public void ClearState()
    {
        meshList = new ElectricMeshList();
    }

    #region Mesh Creation
    public void CreateAndCalculateMeshes()
    {
        CreateMeshes();
        Matrix<float> voltageMatrix = GetVoltageMatrix(meshList);
    }

    public void CreateMeshes()
    {
        List<ElectricComponent> componentList = ProjectManager.GetAllConnectedComponents();

        List<List<ElectricComponent>> unsafeMeshList = new();
        if (componentList.Count > 0)
        {
            /*
            ElectricComponent root = componentList.First();
            HashSet<ElectricComponent> ancestors = new HashSet<ElectricComponent> { root };
            AnalyseConnections(root, null, root, ancestors, unsafeMeshList);
            */
            
            foreach (ElectricComponent root in componentList)
            {
                // TODO : enlever?????
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

        print("Meshes to correct: " + unsafeMeshList.Count);
        unsafeMeshList = RemoveIncorrectMeshes(unsafeMeshList);
        unsafeMeshList = RemoveDuplicatedMeshes(unsafeMeshList);
        DetectShortCircuit(unsafeMeshList);
        PopulateMeshMap(unsafeMeshList);
    }

    public List<List<ElectricComponent>> RemoveIncorrectMeshes(List<List<ElectricComponent>> toCorrect)
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
                print("Mesh had not enought components");
            }
        }
        return correctMeshes;
    }

    public void PopulateMeshMap(List<List<ElectricComponent>> setList)
    {
        ClearState();

        int index = 0;

        foreach(List<ElectricComponent> set in setList)
        {
            meshList.Add(index, set.ToList());
            index++;
        }
    }

    // TODO optimiser le nombre d'appel
    // retourne vrai si on a trouvé une maille, faux si on doit continuer la recherche
    public bool AnalyseConnections(ElectricComponent node, ElectricComponent parent, ElectricComponent root, HashSet<ElectricComponent> ancestors, List<List<ElectricComponent>> list)
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
        return false; // ne doit pas être appelé
    }

    // TODO prendre en compte la fem pour une source
    public void DetectShortCircuit(List<List<ElectricComponent>> setList)
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

    public bool IsMeshComponentValid(ElectricComponent component)
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
                ////////////////////////////////////////////////////////////
                powerSource.SetColor(Color.green); // TODO REMOVE (FOR DEBUG)
                ////////////////////////////////////////////////////////////

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
        print(meshVoltage);
        // TODO FIX DOIT DONNER LE BON SIGNE
        return meshVoltage;
    }

    public static Matrix<float> GetVoltageMatrix(ElectricMeshList meshList)
    {
        Matrix<float> voltageMatrix = Matrix<float>.Build.Dense(1, meshList.Count);

        ////////////////////// TODO REMOVE THIS DEBUG CODE
        List<Color> colors = new List<Color> { Color.red, Color.blue, Color.green };
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

        foreach (KeyValuePair<int, List<ElectricComponent>> mesh in meshList)
        {
            float result = GetMeshVoltage(mesh.Value);
            voltageMatrix[0, mesh.Key] = result;
        }

        return voltageMatrix;
    }

    #endregion

    #region Matrice de résistance
    public static Matrix<float> GetResistanceMatrix(ElectricMeshList meshList)
    {
        Matrix<float> voltageMatrix = Matrix<float>.Build.Dense(meshList.Count, meshList.Count);

        foreach (KeyValuePair<int, List<ElectricComponent>> mesh in meshList)
        {
            float result = GetMeshVoltage(mesh.Value);
            voltageMatrix[0, mesh.Key] = result;
        }

        return voltageMatrix;
    }
    #endregion
}
