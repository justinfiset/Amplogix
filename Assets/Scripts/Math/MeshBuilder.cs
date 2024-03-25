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
        AnalyseConnections(current, previous, masterNodes, index);
    }

    public void CreateAndCalculateMeshes()
    {
        CreateMeshes();
        Matrix<float> voltageMatrix = GetVoltageMatrix(meshList);
    }

    public void CreateMeshes()
    {
        ElectricComponent[] componentList = ProjectManager.m_Instance.componentList.Keys.ToArray();
      
        ElectricComponent headComponent = componentList.First();
        if (headComponent != null) // on a assez de composants
        {
            int meshIndex = 0;
            HashSet<ElectricComponent> masterNodes = new HashSet<ElectricComponent> { headComponent };
            AnalyseConnections(headComponent, null, masterNodes, meshIndex);
        }
    }

    // modele pour les circuits en série - il faudra passer
    // par la récursivité pour faire les autres types de circuits
    // ici on fait l'équivalant pour UNE maille (série)
    public void AnalyseConnections(ElectricComponent current, ElectricComponent previous, HashSet<ElectricComponent> masterNodes, int index)
    {
        if (masterNodes != null && masterNodes.Contains(current) && previous != null) // signifie la fin d'une maille
        {
            return; // a fix pour plusieurs mailles
        } else
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
  

    // TODO tester
    // On analyse les sources en sens horaire selon les lois de kirchoff
    public static float GetMeshVoltage(List<ElectricComponent> mesh)
    {
        float meshVoltage = 0f;

        ElectricComponent head = mesh.First();
        ElectricComponent previous = null;
        foreach (ElectricComponent component in mesh)
        {
            float previousRot = (previous == null) ? 0 : previous.transform.localEulerAngles.z;
            float rot = component.transform.localEulerAngles.z % 360; // permet d'avoir des valeurs parmit 0, 90, 180, 

            if (component.type == ElectricComponentType.PowerSource)
            {
                PowerSource powerSource = (PowerSource)component;
                powerSource.SetColor(Color.green); // TODO REMOVE (FOR DEBUG)

                if (previous != null && (rot != previousRot || rot != previousRot + 90f)) // sens contraire -> on additionne
                {
                    meshVoltage += powerSource.voltage;
                }
                else // même sens on soustrait
                {
                    meshVoltage -= powerSource.voltage;
                }
            }
            previous = component;
        }
        print(meshVoltage);
        return meshVoltage;
    }

    public static Matrix<float> GetVoltageMatrix(ElectricMeshList meshList)
    {
        Matrix<float> voltageMatrix = Matrix<float>.Build.Dense(1, meshList.Count);

        /// TODO REMOVE THE DEBUG CODE
        foreach (KeyValuePair<int, List<ElectricComponent>> mesh in meshList)
        {
            foreach(ElectricComponent comp in mesh.Value)
            {
                comp._SetColor(Color.red);
            }
        }
        //////////////////////////////////////////////////

        foreach (KeyValuePair<int, List<ElectricComponent>> mesh in meshList)
        {
            float result = GetMeshVoltage(mesh.Value);
            voltageMatrix[0, mesh.Key] = result;
        }

        return voltageMatrix;
    }
}
