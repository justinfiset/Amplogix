using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public bool isDebugging { get; private set; } = true;

    [SerializeField] private GameObject debugPrefab;

    private Dictionary<int, Dictionary<int, GameObject>> objectCollection; // key: hashcode, value: debug go isntance

    private static DebugManager m_Instance; // the Singleton instance, only usable by the class itselfs

    private void Awake()
    {
        if (m_Instance == null)
            m_Instance = this;
        else if (m_Instance != this)
            Destroy(this);

        // Disable debugging if there is no instance of DebugManager
        if (m_Instance == null) isDebugging = false;

        objectCollection = new Dictionary<int, Dictionary<int, GameObject>>();
    }

    #region Private Methods for internal usage

    private void AddObjectToCollection(int hashcode, int index, GameObject obj)
    {
        if(objectCollection.ContainsKey(hashcode))
        {
            // todo redo with c# map usage map[key].(value things) // redo for whole class
            objectCollection.TryGetValue(hashcode, out Dictionary<int, GameObject> map);
            if (map != null) map.Add(index, obj);
            else CreateObjectListInCollection(hashcode)[index] = obj;
        } else
        {
            CreateObjectListInCollection(hashcode)[index] = obj;
        }
    }

    private Dictionary<int, GameObject> CreateObjectListInCollection(int hashcode)
    {
        Dictionary<int, GameObject> map = new Dictionary<int, GameObject>();
        objectCollection.Add(hashcode, map);
        return map;
    }

    #endregion

    #region Public Methods

    public static bool IsDebugging()
    {
        return m_Instance.isDebugging;
    }

    public static GameObject CreateNewDebugObject(MonoBehaviour originClass, int index)
    {
        GameObject debugObj = Instantiate(m_Instance.debugPrefab);
        m_Instance.AddObjectToCollection(originClass.GetHashCode(), index, debugObj);
        return debugObj;
    }

    public static GameObject GetDebugPrefab()
    {
        return m_Instance.debugPrefab;
    }

    public static void SetDebugObjectLocalPos(MonoBehaviour originClass, int index, Vector3 newLocalPos)
    {
        m_Instance.objectCollection.TryGetValue(originClass.GetHashCode(), out Dictionary<int, GameObject> map);
        if(map != null)
        {
            map.TryGetValue(index, out GameObject obj);
            if (obj != null) obj.transform.localPosition = newLocalPos;
        }
    }

    #endregion
}
