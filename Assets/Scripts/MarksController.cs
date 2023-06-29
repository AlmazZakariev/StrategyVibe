using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MarksController : MonoBehaviour
{
    private ActionsButtonController ActionsButtonControllerScript;
    // Start is called before the first frame update
    void Start()
    {
        ActionsButtonControllerScript = GameObject.Find("ButtonsManager").GetComponent<ActionsButtonController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseDown()
    {
        ActionsButtonControllerScript.CitySelected(gameObject);
    }
}
