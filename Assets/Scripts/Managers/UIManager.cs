using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // -------------------------------------------------------- //
    // THIS SCRIPT HAS BEEN DEPRICATED AND IS NO LONGER IN USE! //
    // -------------------------------------------------------- //

    //[Header("Resources")]
    [HideInInspector]
    public UIPanel[] panelsDepricated;
    public UIPanels panels;

    [System.Serializable]
    public class UIPanel
    {
        public string name;
        public GameObject panel;
    }

    [System.Serializable]
    public class UIPanels
    {
        public GameObject panelTest;
    }

    public enum Panels
    {
        Test
    }

    public GameObject[] panelObjects = new GameObject[Enum.GetNames(typeof(Panels)).Length];

    private void Awake()
    {
        Registry.AddRegister("manager_ui", this);
    }

    public void SetPanelState(string _name, bool _state)
    {
        for (int i = 0; i < panelsDepricated.Length; i++)
            if (panelsDepricated[i].name == _name)
                panelsDepricated[i].panel.SetActive(_state);
    }

    public void TogglePanelState(string _name)
    {
        for (int i = 0; i < panelsDepricated.Length; i++)
            if (panelsDepricated[i].name == _name)
                panelsDepricated[i].panel.SetActive(!panelsDepricated[i].panel.activeSelf);
    }

    public void TogglePanelState(GameObject panel)
    {

    }
}
