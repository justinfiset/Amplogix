using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ComponentSelection : MonoBehaviour
{
    public DuplicateComponent duplicator;
    public GameObject prefab;
    private bool buttonPressed = false;

    public void SetAtivePrefab()
    {   
        
        duplicator.SetCurrentPrefab(prefab);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (buttonPressed)
        {
            SetAtivePrefab();
        }
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonPressed = true;
  
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonPressed = false;
    }


}
