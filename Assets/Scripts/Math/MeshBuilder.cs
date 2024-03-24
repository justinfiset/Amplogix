using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using System;

// simplification des listes
using ElectricMeshList = System.Collections.Generic.Dictionary<int, ElectricComponent>;

public class MeshBuilder : MonoBehaviour
{
    public static ElectricMeshList CreateMeshes()
    {
        ElectricMeshList meshList = new ElectricMeshList();

        ElectricComponent[] componentList = ProjectManager.m_Instance.componentList.Keys.ToArray();
        ElectricComponent headComponent = componentList[0];
        ElectricComponent previousComponent = headComponent;
        
        // TODO à faire pour plusieurs mailles / analyse apronfondi des mailles récursives
        int meshIndex = 1;
        if(headComponent != null) // on a assez de composants
        {
            Connection connection = headComponent.connectionManager;
            if(connection.IsConnected())
            {
                // modele pour les circuits en série - il faudra passer
                // par la récursivité pour faire les autres types de circuits
                // ici on fait l'équivalant pour UNE maille (série)
                meshIndex = 1;
                foreach(ElectricComponent component in connection.GetConnectedComponents()) 
                {
                    if(component == headComponent)
                    {
                        break;
                    }
                    else if(component != previousComponent)
                    {
                        meshList.Add(meshIndex, component);
                    }
                }
            }
        }

        return meshList;
    }

    // TODO tester
    // On analyse les sources en sens horaire selon les lois de kirchoff
    public float GetMeshVoltage(ElectricComponent[] mesh)
    {
        float meshVoltage = 0f;

        try // On capture toute les erruers eventueslles
        {
            ElectricComponent head = mesh[0];
            ElectricComponent previous = head;
    
            foreach (ElectricComponent component in mesh)
            {
                float previousRot = previous.transform.localEulerAngles.z;
                float rot = component.transform.localEulerAngles.z % 360; // permet d'avoir des valeurs parmit 0, 90, 180, 270

                if (component.type == ElectricComponentType.PowerSource)
                {
                    PowerSource powerSource = (PowerSource)component;
 
                    if(rot != previousRot || rot != previousRot + 90f) // sens contraire -> on additionne
                    {
                        meshVoltage += powerSource.voltage;
                    } else // même sens on soustrait
                    {
                        meshVoltage -= powerSource.voltage;
                    }
                }
                previous = component;
            }
        } catch(Exception e)
        {
            print(e);
            throw new System.Exception("Erreur une maille doit avoir au moin un composant");
        }

        return meshVoltage;
    }
}
