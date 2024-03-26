using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using MathNet.Numerics.LinearAlgebra;
using System.Collections;

// simplification des listes
using ElectricMeshList = System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<ElectricComponent>>;
using Unity.VisualScripting;

public class MeshBuilder : MonoBehaviour
{
    ElectricMeshList meshList = new ElectricMeshList();

    // TODO Tester - RemoveDuplicatedMeshes
    public List<HashSet<ElectricComponent>> RemoveDuplicatedMeshes(List<HashSet<ElectricComponent>> original)
    {
        List<HashSet<ElectricComponent>> cleaned = new();

        cleaned.Add(original.First());

        return cleaned;
    }

    /*
if (clean.SetEquals(mesh))
{
    wasFound = clean.All(s => mesh.Contains(s));
    break;
}*/

    public void Update()
    {
        // TODO REMOVE TESET
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            CreateAndCalculateMeshes();
        }
    }

    // TODO REMOVE / POUR LE DEBUG -> affiche graphique le passage de crawler dans les composants
    public IEnumerator WaitForColor(ElectricComponent current, ElectricComponent previous, HashSet<ElectricComponent> masterNodes, ElectricMeshList meshList, int index)
    {
        previous._SetColor(Color.red);
        yield return new WaitForSeconds(0.5f);
        //AnalyseConnections(current, previous, masterNodes, index);
    }

    public void ClearState()
    {
        meshList = new ElectricMeshList();
    }

    #region Mesh Creation
    public void CreateAndCalculateMeshes()
    {
        try
        {
            CreateMeshes();
            Matrix<float> voltageMatrix = GetVoltageMatrix(meshList);
        } catch(Exception e)
        {
            print(e.Message);
            // TODO afficher l'excpetion à l'utilisateur (avec une popup si possible)
        }
    }

    public void CreateMeshes()
    {
        ElectricComponent[] componentList = ProjectManager.m_Instance.componentList.Keys.ToArray();

        List<HashSet<ElectricComponent>> unsafeMeshList = new();
        if (componentList.Length > 0)
        {
            ElectricComponent root = componentList.First();
            HashSet<ElectricComponent> ancestors = new HashSet<ElectricComponent> { root };
            AnalyseConnections(root, root, ancestors, unsafeMeshList);
            /*
            foreach (ElectricComponent root in componentList)
            {
                HashSet<ElectricComponent> ancestors = new HashSet<ElectricComponent> { root };
                AnalyseConnections(root, root, ancestors, unsafeMeshList);
            }*/
        } else
        {
            throw new IncorrectCircuitException("Un circuit doit avoir au moin un composant!");
        }

        unsafeMeshList = RemoveIncorrectMeshes(unsafeMeshList);
        unsafeMeshList = RemoveDuplicatedMeshes(unsafeMeshList);
        PopulateMeshMap(unsafeMeshList);
    }

    public List<HashSet<ElectricComponent>> RemoveIncorrectMeshes(List<HashSet<ElectricComponent>> toCorrect)
    {
        List<HashSet<ElectricComponent>> correctMeshes = new List<HashSet<ElectricComponent>>();
        foreach(HashSet<ElectricComponent> mesh in toCorrect)
        {
            if(mesh.Count >= 4) // Doit avoir plus de 4 composants
            {
                correctMeshes.Add(mesh);
            }
        }
        return correctMeshes;
    }

    public void PopulateMeshMap(List<HashSet<ElectricComponent>> setList)
    {
        ClearState();

        int index = 0;

        foreach(HashSet<ElectricComponent> set in setList)
        {
            meshList.Add(index, set.ToList());
            index++;
        }
    }

    // TODO optimiser le nombre d'appel
    public void AnalyseConnections(ElectricComponent node, ElectricComponent root, HashSet<ElectricComponent> ancestors, List<HashSet<ElectricComponent>> list)
    {
        if(node != null)
        {
            if(ancestors.Contains(node))
            {
                if (node == root && ancestors.Count > 1) // On a trouvé une maille
                {
                    HashSet<ElectricComponent> mesh = new HashSet<ElectricComponent>();
                    mesh.UnionWith(ancestors);
                    list.Add(mesh);
                    return;
                } else if(ancestors.Count > 2)
                {
                    return;
                }
            }

            List<ElectricComponent> childrens = node.connectionManager.connections.connections.ToList();
            if(childrens.Count > 0)
            {
                foreach (ElectricComponent edge in childrens)
                {
                    HashSet<ElectricComponent> childrenAncestors = new();
                    childrenAncestors.UnionWith(ancestors);
                    childrenAncestors.Add(node);
                    AnalyseConnections(edge, root, childrenAncestors, list);
                }
            }
        }
    }

    // modele pour les circuits en série - il faudra passer
    // par la récursivité pour faire les autres types de circuits
    // ici on fait l'équivalant pour UNE maille (série)
   /* public void AnalyseConnections(ElectricComponent current, ElectricComponent previous, HashSet<ElectricComponent> masterNodes, int index)
    {
        if (!IsMeshComponentValid(current))
        {
            meshList.Remove(index); // On enleve le node si il n'est pas valide
            // TODO envlever la maille
            return;
        }

        if (masterNodes != null && masterNodes.Contains(current) && previous != null) // signifie la fin d'une maille
        {
            return; // a fix pour plusieurs mailles
        }
        else
        {
            Connection connectionManager = current.connectionManager;
            if (connectionManager.IsConnected())
            {
                List<ElectricComponent> connections = connectionManager.GetConnectedComponents().ToList(); // on va chercher toutes les connections
                connections.Remove(previous); // on enleve le composant d'avant
                connections.RemoveAll(component => component == null); // on enleve toutes les connections null

                if (connections.Count > 1 && previous != null) // TODO peut etre enlever la derniere condition
                {
                    // TODO recurance pour lancer d'autre mailles
                    masterNodes.Add(current);
                    foreach (ElectricComponent component in connections)
                    {
                        meshIndex++;
                        AnalyseConnections(component, null, masterNodes, meshIndex);
                    }
                }
                else if (connections.Count == 1 || previous == null) // On a une seule connection interessante
                {
                    ElectricComponent component = connections.First();

                    if (meshList.ContainsKey(index))
                    {
                        List<ElectricComponent> mesh = meshList[index];
                        mesh.Add(component);
                        meshList[index] = mesh;
                    }
                    else
                    {
                        meshList.Add(index, new List<ElectricComponent> { component });
                    }

                    if (previous == null)
                        masterNodes.Add(current);

                    //TODO REMOVE FOR DEBUG
                    //StartCoroutine(WaitForColor(component, current, masterNodes, meshList, index));
                    AnalyseConnections(component, current, masterNodes, index);
                }
            }
        }
    }
   */

    public bool IsMeshComponentValid(ElectricComponent component)
    {
        switch (component.type)
        {
            case ElectricComponentType.Switch:
                return !((ElectricSwitch)component).isOpen;
            default: return true;
        }
    }

    /*
    // TODO À OPTIMISER
    public void RemoveMeshesWithErrors()
    {
        int index = 0;
        ElectricMeshList newMeshList = new ElectricMeshList();
        bool isModified = false;

        // On vérifie que chaucne des mailles revient au noeud d'origine
        foreach (KeyValuePair<int, List<ElectricComponent>> mesh in meshList)
        {
            // On vérifie si une maille est circulaire sinon on la supprime
            int count = 0;

            ElectricComponent head = mesh.Value.First();
            foreach (ElectricComponent component in mesh.Value)
            {
                if (component != head)
                {
                    foreach (ElectricComponent connection in component.connectionManager.connections.connections)
                    {
                        if(connection != null)
                        {
                            if (connection.GetInstanceID() == head.GetInstanceID())
                                count++;
                        }
                    }
                }
            }

            if (count != 2)
            {
                meshList.Remove(mesh.Key);
                throw new IncorrectCircuitException("Une maille doit être circulaire.");
            } else
            {
                newMeshList.Add(index, mesh.Value);
                index++;
                isModified = true;
            }
        }

        if(isModified)
        {
            meshList = newMeshList;
        }
    }
    */
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
                if (previous.type == ElectricComponentType.Wire)
                    multiplier = GetPowerSourceMultiplier(previous.connectionManager, component);
                else // si on a pas un fil avant
                    multiplier = (prevAngle == angle) ? -1 : 1;

                multiplier = multiplier * -1; // TODO bug fix
                meshVoltage += powerSource.voltage * multiplier;

                /*else if ((prevAngle + 180f) % 360 == angle)
                {
                    multiplier = 1;
                }*/
            }

            previous = component;
        }
        print(meshVoltage);
        return meshVoltage;
    }

    public static Matrix<float> GetVoltageMatrix(ElectricMeshList meshList)
    {
        Matrix<float> voltageMatrix = Matrix<float>.Build.Dense(1, meshList.Count);
        
        ////////////////////// TODO REMOVE THIS DEBUG CODE
        foreach (KeyValuePair<int, List<ElectricComponent>> mesh in meshList)
            foreach(ElectricComponent comp in mesh.Value)
                if(comp.type != ElectricComponentType.PowerSource)
                    comp._SetColor(Color.red);
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
