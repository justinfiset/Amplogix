using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using MathNet.Numerics.LinearAlgebra;
using System.Collections;

// simplification des listes
using ElectricMeshList = System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<ElectricComponent>>;
using UnityEngine.Rendering.VirtualTexturing;

public class MeshBuilder : MonoBehaviour
{
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

    #region Mesh Creation
    public void CreateAndCalculateMeshes()
    {
        try
        {
            ElectricMeshList meshList = CreateMeshes();
            Matrix<float> voltageMatrix = GetVoltageMatrix(meshList);
            Matrix<float> resistanceMatrix = GetResistanceMatrix(meshList);

            Debug.Log(voltageMatrix.ToString());
            Debug.Log(resistanceMatrix.ToString());
        } catch (Exception e)
        {
            print(e.Message);
            // TODO afficher les erreurs dans le UI pour l'utilisateur
        }
    }

    public ElectricMeshList CreateMeshes()
    {
        ElectricMeshList meshList = new ElectricMeshList();

        List<ElectricComponent> componentList = ProjectManager.GetAllConnectedComponents();

        List<List<ElectricComponent>> unsafeMeshList = new();
        if (componentList.Count > 0)
        {
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
        PopulateMeshMap(unsafeMeshList, meshList);
        return meshList;
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
                throw new IncorrectCircuitException("A mesh needs to have more than four components");
            }
        }
        return correctMeshes;
    }

    public void PopulateMeshMap(List<List<ElectricComponent>> setList, ElectricMeshList meshList)
    {
        int index = 0;

        foreach(List<ElectricComponent> set in setList)
        {
            meshList.Add(index, set.ToList());
            index++;
        }
    }

    // TODO optimiser le nombre d'appel
    // retourne vrai si on a trouv� une maille, faux si on doit continuer la recherche
    public bool AnalyseConnections(ElectricComponent node, ElectricComponent parent, ElectricComponent root, HashSet<ElectricComponent> ancestors, List<List<ElectricComponent>> list)
    {
        ancestors.Remove(root); // Par mesure de s�curit�, on veut s�parer les ancetres de la racine
        if(node != null) // Empeche de lancer une exception
        {
            // Les conditions d'arr�ts
            if (!IsMeshComponentValid(node)) // Si un composant n'est pas valide ex interrupteur ouvert
            {
                return false;
            }
            else if (parent != null)
            { // Si on est pas � la premi�re it�ration (1 de base)
                if (root != null && node == root) // On a trouv� une maille
                {
                    List<ElectricComponent> mesh = new List<ElectricComponent>() { root, parent };
                    mesh.AddRange(ancestors);
                    list.Add(mesh);
                    return true;
                }
                else if (ancestors.Contains(node)) // Par s�curit�
                {
                    return false; // Ca ne sert � rien d'explorer cette branche
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
    public void DetectShortCircuit(List<List<ElectricComponent>> setList)
    {
        // Les composants qui n'ont aucune r�sistance
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
        // TODO FIX DOIT DONNER LE BON SIGNE
        return meshVoltage;
    }

    public static Matrix<float> GetVoltageMatrix(ElectricMeshList meshList)
    {
        Matrix<float> voltageMatrix = Matrix<float>.Build.Dense(1, meshList.Count);

        ////////////////////// TODO REMOVE THIS DEBUG CODE
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

        foreach (KeyValuePair<int, List<ElectricComponent>> mesh in meshList)
        {
            float result = GetMeshVoltage(mesh.Value);
            voltageMatrix[0, mesh.Key] = result;
        }

        return voltageMatrix;
    }

    #endregion

    #region Matrice de r�sistance
    // TODO : prendre en compte la FEM et la r�sistivit� des fils
    public static Matrix<float> GetResistanceMatrix(ElectricMeshList meshList)
    {
        // Matrice [Count, Count]
        Matrix<float> resistanceMatrix = Matrix<float>.Build.Dense(meshList.Count, meshList.Count);

        List<int> managedKeys = new List<int>();
        foreach (KeyValuePair<int, List<ElectricComponent>> main in meshList)
        {
            List<ElectricComponent> resistors = ProjectManager.GetAllElectricComponentsOfType(main.Value, ElectricComponentType.Resistor);

            foreach (KeyValuePair<int, List<ElectricComponent>> secondary in meshList)
            {
                float resistance = 0f;
                // Si on passe pas sur la diagonale et que l'on n'a pas d�j� g�r� les r�sistances
                if (main.Key != secondary.Key && !managedKeys.Contains(secondary.Key))
                {
                    if (!managedKeys.Contains(secondary.Key))
                    {
                        foreach (Resistor resistor in resistors)
                        {
                            if(secondary.Value.Contains(resistor))
                            {
                                resistance += resistor.resistance;
                            }
                        }

                        // c'est �gale de chaque cot�, Ex: R12, R21, etc...
                        resistanceMatrix[secondary.Key, main.Key] = resistance;
                        resistanceMatrix[main.Key, secondary.Key] = resistance;
                    }
                }
                else // La diagonale ex : R11, R22, R33
                {
                    foreach (Resistor resistor in resistors)
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
