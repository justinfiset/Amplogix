using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DuplicateComponent : MonoBehaviour
{

    private bool mouseIsOver;
    Vector3 mousePos;
    [HideInInspector] public GameObject newGameObject;
    public Transform parent;
    Quaternion rotation;

    public void SetCurrentPrefab(GameObject prefab)
    {
        newGameObject = prefab;
    }

    // Start is called before the first frame update
    void Start()
    {
        // TODO A CHANGER ABSOLUMENT
       // parent = Camera.main.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SpawnPrefab();
        }
    }

    private void SpawnPrefab()
    {
        if(newGameObject != null)
        {
            mousePos = GridSettings.GetCurrentSnapedPosition();
        
            Instantiate(newGameObject, mousePos, rotation, parent);
        }
    }

    private void OnMouseEnter()
    {
        
        mouseIsOver = true;
    }

    private void OnMouseExit()
    {
        mouseIsOver = false;
    }
}
