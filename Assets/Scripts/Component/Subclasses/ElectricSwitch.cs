using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricSwitch : ElectricComponent
{
    private SpriteRenderer spriteRenderer;

    public Sprite closedSprite;
    public Sprite openSprite;

    public bool isOpen = true;
    private SimulationManager simulationManager;

    public override void Init()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public new void Start()
    {
        simulationManager = SimulationManager.Instance;
        base.Start();
    }

    override public void Setup()
    {
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        spriteRenderer.sprite = isOpen ? openSprite : closedSprite;
    }

    override public void Interact()
    {
        isOpen = !isOpen;
        UpdateSprite();
        bool managerWasSimulating = simulationManager.isSimulating;

        ProjectManager.OnModifyProject(ProjectModificationType.CircuitDataModification);

        if (managerWasSimulating)
        {
            simulationManager.Play();
        }
    }

    public override void UnpackCustomComponentData(string customDataString)
    {
        ElectricSwitchData customData = UnserializeCustomComponentData<ElectricSwitchData>(customDataString);
        this.isOpen = customData.isOpen;
        UpdateSprite();
    }

    public override string GetCustomComponentData()
    {
        ElectricSwitchData customData = new ElectricSwitchData(this);
        return SerializeCustomComponentData(customData);
    }
}

[Serializable]
public class ElectricSwitchData
{
    public bool isOpen;

    public ElectricSwitchData(ElectricSwitch component) { 
        isOpen = component.isOpen;
    }
}
