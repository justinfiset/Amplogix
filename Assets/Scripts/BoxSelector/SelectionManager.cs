using System.Collections.Generic;
using UnityEngine.UI;

public class SelectionManager 
{
  private static SelectionManager instance;
    public static SelectionManager Instance
    {
        get
        {   if (instance == null)
            {
                instance = new SelectionManager ();
            }
         return instance;
        }
        private set { 
            instance = value; }
    }

    public HashSet<Selectable> SelectedUnits = new HashSet<Selectable> ();

    private SelectionManager()  { }

    public void Selects(Selectable Units)
    {
        SelectedUnits.Add(Units);
    }

    public void DeSelects(Selectable Units)
    {
        SelectedUnits.Remove(Units);
    }

    public void DeselectAll()
    {
        SelectedUnits.Clear();
    }
}
