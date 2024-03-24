using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// simplification des listes
using Mesh = System.Collections.Generic.List<ElectricComponent>;
using MeshList = System.Collections.Generic.List<System.Collections.Generic.List<ElectricComponent>>;

public class MeshBuilder
{
    public static MeshList CreateMeshes()
    {
        MeshList meshList = new MeshList();

        ElectricComponent[] componentList = ProjectManager.m_Instance.componentList.Keys.ToArray();
        ElectricComponent headComponent = componentList[0];
        if(headComponent != null) // on a assez de composants
        {
            Connection connection = headComponent.connectionManager;
            if(connection.IsConnected())
            {
                // modele pour les circuits en série - il faudra passer
                // par la récursivité pour faire les autres types de circuits

            }
        }

        return meshList;
    }
}
