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
                    wasFound = true;
                    foreach(ElectricComponent component in unsafeMesh) 
                    { 
                        if(!uniqueMesh.Contains(component))
                        {
                            wasFound = false;
                            break;
                        }
                    }

                    if (wasFound) break;
                    //if(uniqueMesh.All(unsafeMesh.Contains))
                    //{
                    //    wasFound = true;
                    //    break;
                    //}
                }
            }

            if(!wasFound)
            {
                uniqueMeshes.Add(unsafeMesh);
            }
        }
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

    public static void ExecuteAllVoltmeters()
    {
        foreach(ElectricComponent component in ProjectManager.m_Instance.componentList.Keys.ToList())
        {
            if(component.type == ElectricComponentType.Voltmeter)
            {
                float potential = 0f;
                List<ElectricComponent> connectedComponents = new List<ElectricComponent>(); // Les deux composants qu'on mesure
                ElectricComponent[] connections = component.connectionManager.connections.connections; // Tous les composants auquel le voltm�tre est connect�
                for (int i = 0; i < connections.Length; i++)
                {
                    if (connections[i] != null)
                    {
                        ElectricComponent revelant = FindNextRevelantComponent(connections[i], component);
                        if(revelant != null) connectedComponents.Add(revelant);
                    }
                }

                if(connectedComponents.Count == 2)
                {
                    // On va chercher les mailles qui passent d'un revelant et qui reviennent
                    List<List<ElectricComponent>> allMeshList = new();
                    AnalyseConnections(connectedComponents[0], null, connectedComponents[1], new HashSet<ElectricComponent>(), allMeshList);
                    AnalyseConnections(connectedComponents[1], null, connectedComponents[0], new HashSet<ElectricComponent>(), allMeshList);

                    // On va chercher toutes les mailles qui contiennent les deux composants
                    List<List<ElectricComponent>> unsafeMeshList = new();
                    foreach (List<ElectricComponent> mesh in allMeshList)
                    {
                        bool isValid = true;
                        foreach (ElectricComponent revelant in connectedComponents)
                        {
                            if(!mesh.Contains(revelant))
                            {
                                isValid = false;
                                break;
                            }
                        }

                        if(isValid) unsafeMeshList.Add(mesh);
                    }

                    // On enleve toutes les mailles en double
                    unsafeMeshList = RemoveDuplicatedMeshes(unsafeMeshList);

                    // Si on possède au moin une maille qui comprend les deux composants
                    if (unsafeMeshList.Count > 0)
                    {
                        //On va chercher la maille qui 
                        List<ElectricComponent> bestMesh = null;
                        float bestMeshResistance = 0f;
                        foreach (List<ElectricComponent> list in unsafeMeshList)
                        {
                            if (bestMesh == null)
                            {
                                bestMesh = list;
                                bestMeshResistance = GetMeshResistance(list);
                            }
                            else
                            {
                                float resistance = GetMeshResistance(list);
                                if (resistance < bestMeshResistance)
                                {
                                    bestMesh = list;
                                }
                                else if (resistance == bestMeshResistance)
                                {
                                    if (list.Count < bestMesh.Count)
                                        bestMesh = list;
                                    else if (list.Count == bestMesh.Count)
                                        print("Le voltmètre possède plusieurs mailles possibles. Cela ne change pas la valeur mais il faudrait trouver un moyen d'y remédier.");
                                }
                            }
                        }

                        float current = -1f;
                        foreach(ElectricComponent link in bestMesh)
                        {
                            if(current == -1f)
                            {
                                current = link.currentIntensity;
                            }
                            else
                            {
                                if(current != link.currentIntensity)
                                {
                                    current = 0f;
                                    print("plusieurs courant entre les deux bornes du voltmètre.");
                                    break;
                                }
                            }
                        }
                        potential = current * bestMeshResistance;
                        /*
                                            int count = 0;
                                if (count == 0) potential += revelant.componentPotential;
                                else potential -= revelant.componentPotential;
                        */
                    }
                }

                component.SetCalculatedPotential(potential);
            }
        }
    }

    public static float GetMeshResistance(List<ElectricComponent> list)
    {
        float resistance = 0f;
        foreach (ElectricComponent listComponent in list)
        {
            resistance += listComponent.resistance;
        }
        return resistance;
    }

    public static ElectricComponent FindNextRevelantComponent(ElectricComponent current, ElectricComponent previous)
    {
        // Si on a plus de 2 connections ou que ce n'est pas un fil
        if (current.type != ElectricComponentType.Voltmeter)
        {
            if (current.connectionManager.ConnectionCount() > 2 || current.type != ElectricComponentType.Wire)
            {
                return current;
            }
            else
            {
                List<ElectricComponent> connections = current.connectionManager.connections.connections.ToList(); ;
                foreach (ElectricComponent connection in connections)
                {
                    if (connection != previous && connection != null)
                    {
                        return FindNextRevelantComponent(connection, current);
                    }
                }
            }
        }
        return null;
    }

    #region Mesh Creation
    public static MatrixEquationSystem CreateAndCalculateMeshes(int currentModifier)
    {
        // Preps avant les calculs
        ProjectManager.ResetCurrentIntensity();

        try
        {
            ElectricMeshList meshList = CreateMeshes();
            Vector<float> voltageMatrix = GetVoltageMatrix(meshList);
            Matrix<float> resistanceMatrix = GetResistanceMatrix(meshList);
            MatrixEquationSystem system = new MatrixEquationSystem(meshList, resistanceMatrix, voltageMatrix, currentModifier);

            try
            {
                //print(system.resistanceMatrix.ToString());
                //print(system.meshVoltage.ToString());
                print(system.meshCurrent.ToString());
            } catch (Exception e) { }

            SetAllComponentCurrent(system, meshList);
            ExecuteAllVoltmeters();
            //HandleVisualCurrent(meshList, system.meshCurrent);

            return system;
        } catch (IncorrectCircuitException e)
        {
            string mes = e.Message;
            //print(e.Message);
        }

        return null;
    }

    // TODO optimiser en cas de probl�mes
    public static void SetAllComponentCurrent(MatrixEquationSystem system, ElectricMeshList meshList)
    {
        List<ElectricComponent> calledComponents = new List<ElectricComponent>();
        for (int i = 0; i < system.meshCount; i++)
        {
            float meshCurrent = system.meshCurrent[i];
            float secondMeshCurrent = 0f;
            foreach (ElectricComponent component in meshList[i])
            {
                if (!calledComponents.Contains(component)) // Si pas d�ja appel�
                {
                    // On v�rifie si un composant est contenu dans plusieurs mailles
                    for (int j = i + 1; j < system.meshCount; j++)
                    {
                        bool wasFound = false;
                        foreach (ElectricComponent temp in meshList[j])
                        {
                            if (component.GetInstanceID() == temp.GetInstanceID())
                            {
                                wasFound = true;
                                break;
                            }
                        }

                        if (wasFound)
                        {
                            secondMeshCurrent = system.meshCurrent[j];
                            break;
                        }
                    }

                    float current = Math.Abs(Math.Abs(meshCurrent) - Math.Abs(secondMeshCurrent));
                    //if (component.type == electriccomponenttype.ammeter)
                    //{
                    //    print(current + " first : " + meshcurrent + " ; second: " + secondmeshcurrent);
                    //}
                    component.SetCalculatedIntensity(current);
                    calledComponents.Add(component);
                    secondMeshCurrent = 0f; // On reset le courrant secondaire
                }
            }
        }
    }

    private static void HandleVisualCurrent(ElectricMeshList meshList, Vector<float> meshCurrent)
    {
        if (!CurrentVisualisationManager.isSetup && CurrentVisualisationManager.doVisualCurrent)
        {
            CurrentVisualisationManager.StartParticleEmissions(meshList, meshCurrent);
        }
    }

    public static ElectricMeshList CreateMeshes()
    {
        ElectricMeshList meshList = new ElectricMeshList();

        List<ElectricComponent> componentList = ProjectManager.GetAllConnectedComponents();

        List<List<ElectricComponent>> unsafeMeshList = new();
        if (ProjectManager.ComponentCountIsValid(componentList))
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
            throw new IncorrectCircuitException("Un circuit doit avoir au moins un composant ou une source de courant!");
        }

        unsafeMeshList = RemoveDuplicatedMeshes(unsafeMeshList);
        unsafeMeshList = RemoveIncorrectMeshes(unsafeMeshList);
        unsafeMeshList = RemoveParentMeshes(unsafeMeshList);
        DetectShortCircuit(unsafeMeshList);
        PopulateMeshMap(unsafeMeshList, meshList);
        return meshList;
    }

    public static List<List<ElectricComponent>> RemoveParentMeshes(List<List<ElectricComponent>> toCorrect)
    {
        toCorrect.Sort((a, b) => a.Count - b.Count); // On vient trier par ordre croissant
        List<List<ElectricComponent>> childMeshes = new List<List<ElectricComponent>>();

        foreach (List<ElectricComponent> parent in toCorrect)
        {
            int foundCount = 0;
            foreach(ElectricComponent comp in parent)
            {
                bool wasFound = false;
                foreach(List<ElectricComponent> child in childMeshes)
                {
                    foreach(ElectricComponent temp in child)
                    {
                        if(comp.GetHashCode() == temp.GetHashCode())
                        {
                            wasFound = true;
                            break;
                        }
                    }
                }
                if (wasFound) foundCount++;
            }

            if(foundCount != parent.Count)
            {
                childMeshes.Add(parent);
            }
            //print("Found : " + foundCount + " And needed : " + parent.Count);
        }
        return childMeshes;
    }

    public static List<List<ElectricComponent>> RemoveIncorrectMeshes(List<List<ElectricComponent>> toCorrect)
    {
        List<List<ElectricComponent>> validMeshes = new List<List<ElectricComponent>>();
        foreach(List<ElectricComponent> mesh in toCorrect)
        {
            if(mesh.Count >= 4) // Doit avoir plus de 4 composants
            {
                bool isValid = true;
                foreach(ElectricComponent component in mesh)
                {
                    // On Invalide les mailles qui contiennent des voltm�tres
                    if(component.type == ElectricComponentType.Voltmeter)
                    {
                        isValid = false;
                    }
                }

                if(isValid)
                {
                    validMeshes.Add(mesh);
                }
            }
        }
        return validMeshes;
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

    // retourne vrai si on a trouv� une maille, faux si on doit continuer la recherche
    public static bool AnalyseConnections(ElectricComponent node, ElectricComponent parent, ElectricComponent root, HashSet<ElectricComponent> ancestors, List<List<ElectricComponent>> list)
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


            List<ElectricComponent> childrens = node.connectionManager.GetConnectedComponents().ToList();
            bool foundMesh = false;
            foreach (ElectricComponent edge in childrens)
            {
                // pas null est pas égal au parent
                if (edge != null && edge != parent /*&& !ancestors.Contains(edge)*/)
                {
                    HashSet<ElectricComponent> childrenAncestors = new();
                    childrenAncestors.UnionWith(ancestors);

                    if(root != null && edge != root) childrenAncestors.Add(node);

                    if (AnalyseConnections(edge, node, root, childrenAncestors, list))
                    {
                        foundMesh = true;
                    }
                }
            }
            return foundMesh;
        }

        return false;
        //throw (new Exception("Node Analysis was called for a null node!"));
    }

    // TODO prendre en compte la fem pour une source
    public static void DetectShortCircuit(List<List<ElectricComponent>> setList)
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
                float angle = component.transform.localEulerAngles.z; // permet d'avoir des valeurs parmit 0, 90, 180, 

                Connection.Position orientation = (Connection.Position)i;
                switch (orientation)
                {
                    case Connection.Position.Right:
                        expectedAngle = 0f; break;
                    case Connection.Position.Left:
                        expectedAngle = 180f; break;
                    case Connection.Position.Top:
                        expectedAngle = 90f; break;
                    case Connection.Position.Bottom:
                        expectedAngle = 270f; break;
                }

                multiplier = (expectedAngle == angle) ? -1 : 1;
                break;
            }
        }
        return multiplier;
    }

    // On analyse les sources en sens horaire selon les lois de kirchoff
    public static float GetMeshVoltage(List<ElectricComponent> mesh)
    {
        ElectricComponent first = null;
        ElectricComponent next = null;
        float max = 0f; // La position en y max;
        bool isFirstIteraiton = true;
        ElectricComponent root = null; //= new();
        foreach(ElectricComponent component in mesh)
        {
            float pos = component.transform.position.y;
            if(isFirstIteraiton)
            {
                max = pos;
                root = component;
                isFirstIteraiton = false;
            }

            if(pos > max)
            {
                max = pos;
                root = component;
            }
            else if(pos == max)
            {
                root = component;
            }
        }

        if (root != null)
        {
            List<ElectricComponent> connections = root.connectionManager.GetConnectedComponents().ToList();
            foreach(ElectricComponent connection in connections)
            {
                if(connection != null)
                {
                    if (connection.transform.position.x < root.transform.position.x)
                    {
                        first = connection;
                        next = root;
                    } else if(connection.transform.position.x > root.transform.position.x)
                    {
                        first = root;
                        next = connection;
                    }
                    break;
                }
            }
        }

        float meshVoltage = AnalyseMeshVoltage(first, first, next, mesh, 0);
        return meshVoltage;
    }

    public static float AnalyseMeshVoltage(ElectricComponent root, ElectricComponent previous, ElectricComponent current, List<ElectricComponent> mesh, int callstackCount)
    {
        if (callstackCount > mesh.Count) return 0f;
        float voltage = 0;

        // On trouve le voltage et le sens
        float angle = current.transform.localEulerAngles.z; // permet d'avoir des valeurs parmit 0, 90, 180, 270 ,360, 
        //print("Mesh: " + mesh.Count + " Comp: " + current.type);

        if (current.type == ElectricComponentType.PowerSource)
        {
            PowerSource source = (PowerSource) current;

            int multiplier = 1;

            if (previous != null && previous.type == ElectricComponentType.Wire)
            {
                multiplier = GetPowerSourceMultiplier(previous.connectionManager, current);
            }
            else // si on a pas un fil avant
            {
                if(angle == 0f && previous.transform.localPosition.x < current.transform.localPosition.x)
                    multiplier = -1;
                else if (angle == 270f && previous.transform.localPosition.y > current.transform.localPosition.y)
                    multiplier = -1;
                else if (angle == 180f && previous.transform.localPosition.x > current.transform.localPosition.x)
                    multiplier = -1;
                else if (angle == 90f && previous.transform.localPosition.y < current.transform.localPosition.y)
                    multiplier = -1;
            }

            /////////////////////
            //if (multiplier == 1)
            //{
            //    source.SetColor(Color.green); // TODO REMOVE (FOR DEBUG)
            //}
            //else
            //{
            //    source.SetColor(Color.red); // TODO REMOVE (FOR DEBUG)
            //}
            ////////////////////
            voltage = source.voltage * multiplier;
        }

        // Gestion de la récursivité
        ElectricComponent[] connections = current.connectionManager.GetConnectedComponents();
        ElectricComponent next = null;
        foreach(ElectricComponent connection in connections)
        {
            if(connection != null && connection != previous && mesh.Contains(connection))
            {
                next = connection;
                break;
            }
        }
        if(next != null && current != root)
        {
            callstackCount++;
            voltage += AnalyseMeshVoltage(root, current, next, mesh, callstackCount);
        }

        return voltage;
    }

    public static Vector<float> GetVoltageMatrix(ElectricMeshList meshList)
    {
        Vector<float> voltageMatrix = Vector<float>.Build.Dense(meshList.Count);
        foreach (KeyValuePair<int, List<ElectricComponent>> mesh in meshList)
            voltageMatrix[mesh.Key] = GetMeshVoltage(mesh.Value);
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
            foreach (KeyValuePair<int, List<ElectricComponent>> secondary in meshList)
            {
                float resistance = 0f;
                // Si on passe pas sur la diagonale et que l'on n'a pas d�j� g�r� les r�sistances
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

                        // c'est �gale de chaque cot�, Ex: R12, R21, etc...
                        resistanceMatrix[secondary.Key, main.Key] = -resistance;
                        resistanceMatrix[main.Key, secondary.Key] = -resistance;
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
