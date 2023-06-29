using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityController : MonoBehaviour
{
    public AllCities AllCitiesScript;
    public PlayerController PlayerControllerScript;
     //public bool SelectedByPlayer = false;
    
    // Start is called before the first frame update
    void Start()
    {
        AllCitiesScript = GameObject.Find("CityManager").GetComponent<AllCities>();
        PlayerControllerScript = GameObject.Find("PlayerManager").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseDown()
    {
        if (!PlayerControllerScript.IsGameStarted)
        {
            AllCitiesScript.SetPlayerCountry(gameObject);
        }
        if(PlayerControllerScript.colorChangingSpeed != 0)
        {
            var city = AllCitiesScript.FindCityObject(gameObject);
            AllCitiesScript.SetPlayerVote(city);
        }
    }
    
}
