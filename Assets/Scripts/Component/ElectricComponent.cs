using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.EventSystems;
using System.ComponentModel;
using System.Reflection;

//[RequireComponent(typeof(SpriteRenderer))]
//[RequireComponent(typeof(ResizeWinglets))]
//[RequireComponent(typeof(WireTilesManager))]
//[RequireComponent(typeof(Connection))]
[RequireComponent(typeof(BoxCollider2D))]
public class ElectricComponent : MonoBehaviour
{
    [Header("Logic")]
    public ElectricComponentType type;
    public bool canBeRotated = true; // Le composant peut-il �tre rotation� par l'utilisateur?
    public bool canBeMoved = true; // Le composant peut-il �tre manipul� par l'utilisateur?
    public bool canStack = false;
    public bool snapToGrid = true;
    public bool respectOrientation = false;
    public bool canGenerateMeshes = true; // La création de mailles part-elle du composant?

    [Header("State")]
    protected bool isHover = false;
    protected bool isSelected = false;
    protected bool listenToInputs = true;
    public bool isMouseOverGUI = false;
    [HideInInspector] public string initialComponentData = "";
    [HideInInspector] public string initialConnectionData = "";
    [HideInInspector] private Color color;
    [HideInInspector] public Connection connectionManager;

    [Header("Move / Drag")]
    [HideInInspector] public bool hasReleasedSinceSelection = true;
    protected bool isBeingMoved = false;
    protected Vector3 lastClickMousePos;
    protected Vector3 lastClickPos;


    [Header("UI")]
    [SerializeField] protected SpriteRenderer outline;
    private ResizeWinglets resizeWinglets;
    // private WireTilesManager wireTilesManager;
    // private ConnectionTilesManager connectionTilesManager;
    private TilesManager tilesManager;
    [HideInInspector] public SpriteRenderer sprite;
    public static float DefaltGUIDivider = 3f;
    protected float GUIHeightDivider = 3f;

    [Header("Inputs")]
    private static KeyCode rotateKey = KeyCode.R;
    private static KeyCode mouseDeleteKey = KeyCode.Mouse2;
    private static KeyCode keyboardDeleteKey = KeyCode.Backspace;
    private static KeyCode systemDeleteKey = KeyCode.Delete;
    private static KeyCode unselectKey = KeyCode.Escape;
    private static KeyCode interactKey = KeyCode.Mouse1;

    [Header("Current")]
    public bool isLightSource;
    public bool showsCurrent = true;
    [HideInInspector] public float currentIntensity { get; private set; } = 0;
    [HideInInspector] public float componentPotential { get; private set; } = 0;
    [HideInInspector] public float resistance { get; protected set; } = 0f;

    private void Start()
    {
        resizeWinglets = GetComponent<ResizeWinglets>();
        tilesManager = GetComponent<TilesManager>();
        sprite = GetComponent<SpriteRenderer>();
        connectionManager = GetComponent<Connection>();

        Init();
        if (initialComponentData != "")
        {
            UnpackCustomComponentData(initialComponentData);
        }
        Setup();
    }

    public void InitConnections()
    {
        if (initialConnectionData != null)
        {
            Connection.ConnectionValueData data = UnserializeCustomComponentData<Connection.ConnectionValueData>(initialConnectionData);
            if (data != null)
            {
                connectionManager.CreateInitialConnections(data);
            }
        }
    }

    void Update()
    {
        if (!ProjectManager.canInteract) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (isSelected && hasReleasedSinceSelection && !isMouseOverGUI)
            {
                if (Input.GetKey(KeyCode.LeftControl) && ProjectManager.selectionCount > 1)
                {
                    tilesManager.HideTiles();
                }

                else if (!EventSystem.current.IsPointerOverGameObject())
                {
                    UnselectAfterEndOfFrame();
                }
            }

            if (isHover)
            {
                lastClickPos = transform.position;
                if (snapToGrid)
                {
                    lastClickMousePos = GridSettings.GetCurrentSnapedPosition();
                }
                else
                {
                    lastClickMousePos = GridSettings.MouseInputToWorldPoint();
                }

                if (!isSelected)
                {
                    _Select();
                    hasReleasedSinceSelection = false;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (isSelected && hasReleasedSinceSelection && !isMouseOverGUI) {/* Unselect */}
            else
            {
                hasReleasedSinceSelection = true;
            }
        }

        if (isHover)
        {
            if (Input.GetKeyDown(interactKey))
            {
                Interact();
            }
        }

        if (isSelected)
        {
            if (listenToInputs)
            {
                if (canBeRotated && Input.GetKeyDown(rotateKey))
                {
                    _RotateComponent();
                }
                else if (Input.GetKeyDown(mouseDeleteKey)
                    || Input.GetKeyDown(keyboardDeleteKey))
                {
                    _DestroyComponent();
                }
                else if (Input.GetKeyDown(unselectKey))
                {
                    tilesManager.HideTiles();

                    _Unselect();
                }
            }
            // On supprime quand même si on pèse sur la touche 'delete'
            if (Input.GetKeyDown(systemDeleteKey))
            {
                _DestroyComponent();
            }

            if (!hasReleasedSinceSelection)// || Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.Mouse0))
            {
                if (!isBeingMoved)
                {
                    isBeingMoved = true;
                }

                if (isBeingMoved && canBeMoved)
                {
                    //if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.Mouse0))
                    //{
                    //    foreach (ElectricComponent electricComponent in ProjectManager.m_Instance.componentSelection)
                    //    {
                    //        Vector3 newPos;
                    //        if (snapToGrid)
                    //        {
                    //            print("in snap");
                    //            newPos = GridSettings.GetCurrentSnapedPosition();


                    //        }
                    //        else
                    //        {
                    //            Vector3 diffPos = lastClickMousePos - GridSettings.MouseInputToWorldPoint();
                    //            newPos = lastClickPos - diffPos;
                    //        }
                    //        electricComponent.MoveComponentForMany(newPos, electricComponent);

                    //    }
                    //}
                    //else
                    //{
                        Vector3 newPos;
                        if (snapToGrid)
                        {
                            newPos = GridSettings.GetCurrentSnapedPosition();
                        }
                        else
                        {
                            Vector3 diffPos = lastClickMousePos - GridSettings.MouseInputToWorldPoint();
                            newPos = lastClickPos - diffPos;
                        }
                        MoveComponent(newPos);
                    //}
                }
            }
            else // Si on relache le boutton
            {
                if (!canStack && isBeingMoved)
                {
                    if (ProjectManager.m_Instance.GetComponentCount(transform.position) > 1)
                    {
                        MoveComponent(lastClickPos);
                    }
                }
            }
        }

        OnUpdate(); // pour les sous composants
    }



    /** public void SetHypotheticalIntensity(float calculatedIntensity)
     {
         hypotheticalIntensity = calculatedIntensity;
         hypotheticalVoltage = calculatedIntensity * hypotheticalResistance;
     }*/

    public void SetCalculatedIntensity(float calculatedIntensity)
    {
        if (calculatedIntensity != currentIntensity)
        {
            currentIntensity = (calculatedIntensity); //TODO: REMETTRE LE MATH.ABS()
            SetCalculatedPotential(CalculatePotential(currentIntensity));
            OnCurrentChange(currentIntensity);
        }
    }

    public virtual void OnCurrentChange(float newCurrent)
    {
        if (showsCurrent)
        {
            CurrentVisualisation currentVisualisation = GetComponent<CurrentVisualisation>();
            // currentVisualisation.OnCurrentUpdate(newCurrent); //TODO see if important
        }
    }

    public void SetCalculatedPotential(float potential)
    {
        componentPotential = potential;
    }

    public virtual float CalculatePotential(float current)
    {
        return current * resistance;
    }

    #region Internal
    public void MoveComponent(Vector3 newPos)
    {
        if (newPos != transform.position)
        {
            _Unselect();
            transform.position = newPos;
            ProjectManager.m_Instance.ChangeComponentPos(this, transform.position);
            ProjectManager.OnModifyProject(ProjectModificationType.CircuitModification);

            connectionManager.UpdateConnection();

            _Select();
        }
    }

    private void MoveComponentForMany(Vector3 newPos, ElectricComponent electricComponent)
    {
        if (newPos != electricComponent.transform.position)
        {
            print("in move");
            electricComponent._Unselect();
            electricComponent.transform.position = newPos;
            ProjectManager.m_Instance.ChangeComponentPos(electricComponent, newPos);
            ProjectManager.OnModifyProject(ProjectModificationType.CircuitModification);
            electricComponent._Select();

        }
    }

    public void _SetColor(Color newColor, bool isTemporary = false)
    {
        StartCoroutine(WaitForColorChange(newColor, isTemporary));
    }

    // Used to prevent a bug where the sprite renderer is not instantiated yet
    private IEnumerator WaitForColorChange(Color newColor, bool isTemporary = false)
    {
        yield return new WaitForEndOfFrame();

        if (color != newColor && !isTemporary)
        {
            color = newColor;
            ProjectManager.OnModifyProject(ProjectModificationType.VisualModification);
        }

        if (isSelected) newColor = newColor * 0.5f;
        SetColor(newColor);

        if (connectionManager != null)
            connectionManager.SetColor(newColor);
    }


    public void _Select(bool executeInheritedCode = true)
    {

        isSelected = true;
        if (executeInheritedCode)
        {
            if (Input.GetKey(KeyCode.LeftControl) && ProjectManager.selectionCount >= 0)
            {
                Select();
            }
            else
            {
                Select();
                tilesManager.HideTiles();
            }
            if (!Input.GetKey(KeyCode.LeftControl))
            {
                Select();

            }


        }
        outline.color = Color.white;
        _SetColor(color * new Color(1f, 1f, 1f, 0.5f), true);
        ProjectManager.AddComponentToSelection(this);
    }

    public void _Unselect()
    {
        isSelected = false;
        Unselect();
        outline.color = Color.clear;
        _SetColor(color, true);
        ProjectManager.RemoveComponentFromSelection(this);
    }

    private void _RotateComponent()
    {
        _Unselect();
        RotateComponent();
        Connection connection = GetComponent<Connection>();
        connection.DeleteAllConnections();
        connection.AutoConnect();
        _Select();
        ProjectManager.OnModifyProject(ProjectModificationType.CircuitModification);
    }

    public void _DestroyComponent()
    {
        _Unselect();
        DestroyComponent();
        if (connectionManager != null)
            connectionManager.DeleteAllConnections();
        ComponentSpawner.DestroyComponent(gameObject);
        if (isHover) { ProjectManager.componentUnderPointerCount--; };
    }
    #endregion

    #region Inheritance
    public virtual void Interact() { }

    // Called before loading the data
    public virtual void Init() { }
    // Called after loading the data
    public virtual void Setup() { }

    public virtual void SetBaseResistance(float newValue)
    {
        if (resistance == 0)
        {
            resistance = newValue;
        }
    }

    public virtual void OnUpdate() { }

    public virtual void DestroyComponent() { }

    public virtual void RotateComponent()
    {
        transform.Rotate(Vector3.back * 90);
    }

    public virtual void SetColor(Color newColor)
    {
        if (sprite != null)
            sprite.color = newColor;
    }

    public virtual void RenderGUI() { }

    public virtual void UnpackCustomComponentData(string customDataString) { return; }

    public virtual string GetCustomComponentData() { return ""; }

    public virtual void Select()
    {
        resizeWinglets.GenerateWinglets(transform.position, transform.localScale);
        // wireTilesManager.ShowTiles();
        // connectionTilesManager.ShowTiles(this);
        tilesManager.ShowTiles(this);
        sprite.color = sprite.color * new Color(1, 1, 1, 0.5f);
    }

    public virtual void Unselect()
    {
        resizeWinglets.DestroyWinglets();
        // wireTilesManager.HideTiles();
        // connectionTilesManager.HideTiles();
        tilesManager.HideTiles();
        sprite.color = Color.white;
    }
    #endregion

    #region Data / Serialization / Unpack
    public string GetConnectionsnData()
    {
        Connection.ConnectionValueData data = connectionManager.GetData();
        return SerializeCustomComponentData(data);
    }

    public ElectricComponentData GetData()
    {
        return new ElectricComponentData(this);
    }

    public T UnserializeCustomComponentData<T>(string customDataString)
    {
        return JsonUtility.FromJson<T>(customDataString);
    }

    public string SerializeCustomComponentData<T>(T customDataClass)
    {
        return JsonUtility.ToJson(customDataClass);
    }
    #endregion

    #region Mouse Callback
    private void OnMouseEnter()
    {
        isHover = true;
        ProjectManager.componentUnderPointerCount++;
    }

    private void OnMouseExit()
    {
        isHover = false;
        ProjectManager.componentUnderPointerCount--;
    }
    #endregion

    private void OnGUI()
    {
        if (isSelected)
        {
            bool isUnique = ProjectManager.selectionCount < 2;
            string headerName = isUnique ? ElectricComponentTypeMethods.GetName(type) : "Sélection Multiple...";

            ComponentGUI.InitGUI(isUnique ? GUIHeightDivider : DefaltGUIDivider);
            ComponentGUI.CreateBackground(this, headerName); // Creation de la fenetre en bas � droite

            if (isUnique) RenderGUI(); // Creation du UI custom

            ComponentGUI.CreateColorPalette();
            ComponentGUI.CreateDeleteButton();

            if (GUI.changed) // Si on a modifié quelque chose, on relance les calculs
            {
                ProjectManager.OnModifyProject(ProjectModificationType.CircuitDataModification);
            }
        }
    }

    public void UnselectAfterEndOfFrame()
    {
        StartCoroutine(IUnselectAfterEndOfFrame());
    }

    private IEnumerator IUnselectAfterEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        _Unselect();
    }
}

#region Component Type
[Serializable]
public enum ElectricComponentType
{
    None,
    [Description("Resistance")]
    Resistor,
    [Description("Source")]
    PowerSource,
    [Description("Interrupteur")]
    Switch,
    [Description("Ampoule")]
    LightBulb,
    [Description("Moteur")]
    Motor,
    [Description("Condensateur")]
    Condensator,
    [Description("Ampéremètre")]
    Ammeter,
    [Description("Voltmètre")]
    Voltmeter,
    [Description("Diode")]
    Diode,
    [Description("Bobine")]
    Coil,
    [Description("Fil")]
    Wire,
    [Description("Fil")]
    WireCorner,
    [Description("Fil")]
    WireThreeWay,
    [Description("Fil")]
    WireFourWay,
    [Description("Texte")]
    TextLabel = 99
}

public static class ElectricComponentTypeMethods
{
    public static bool IsWire(ElectricComponent component)
    {
        ElectricComponentType[] wireTypes =
        {
            ElectricComponentType.Wire,
            ElectricComponentType.WireCorner,
            ElectricComponentType.WireThreeWay,
            ElectricComponentType.WireFourWay
        };

        return wireTypes.Contains(component.type);
    }

    public static string GetName(ElectricComponentType type)
    {
        FieldInfo field = type.GetType().GetField(type.ToString());
        DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                as DescriptionAttribute;
        return attribute == null ? type.ToString() : attribute.Description;
    }
}
#endregion

#region Component Data
[Serializable]
public class ElectricComponentData
{
    public int type;
    public float x = 0f;
    public float y = 0f;
    public float rot = 0f;
    public float scaleX = 1f;
    public float scaleY = 1f;
    public string customComponentData = "";
    public float r = 1f;
    public float g = 1f;
    public float b = 1f;
    public string connectionData = "";

    public ElectricComponentData(ElectricComponent component)
    {
        type = (int)component.type;
        x = component.transform.position.x;
        y = component.transform.position.y;
        rot = component.transform.localEulerAngles.z;
        scaleX = component.transform.localScale.x;
        scaleY = component.transform.localScale.y;
        customComponentData = component.GetCustomComponentData();
        // Color
        r = component.sprite.color.r;
        g = component.sprite.color.g;
        b = component.sprite.color.b;
        // Connections
        connectionData = component.GetConnectionsnData();
    }
}
#endregion