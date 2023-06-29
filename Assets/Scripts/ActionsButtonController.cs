using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static UnityEditor.Progress;

delegate void Action( Country country, City city);
public class ActionsButtonController : MonoBehaviour
{
    private AllCities AllCitiesScript;
    
    private bool WaitingForAccept = false;

    public GameObject MarkerPrefab;
    public GameObject SelectedMarkerPrefab;

    private Dictionary<GameObject, City> CityMarkDictionary= new Dictionary<GameObject, City>();
    public GameObject SelectedCityMarkInstance;
    private City SelectedCity;
    // Start is called before the first frame update
    void Start()
    {
        AllCitiesScript = GameObject.Find("CityManager").GetComponent<AllCities>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Action1()
    {
        int cost = 1;
        bool isCoin = true;
        TryAction(cost, isCoin, Action1Action);
    }
    public void Action2()
    {
        int cost = 1;
        bool isCoin = false;
        TryAction(cost, isCoin, Action2Action);
    }

    private void TryAction(int cost, bool isCoin, Action action)
    {
        //ѕроверка по доступности ресурсов
        if(!AllCitiesScript.PlayerCountry.IsEnableAction(cost, isCoin))
        {
            return;
        }
        //¬ыбор города дл€ взаимодействи€
        SetMarks(cost, isCoin, action);
        
    }

    private void SetMarks(int cost, bool isCoin, Action action)
    {
        //ѕервый клик по кнопке дл€ вывода значков на доступных городах
        if (!WaitingForAccept)
        {
            CityMarkDictionary.Clear();
            var cities = AllCitiesScript.PlayerCountry.FindAllNeighbours(false);
            WaitingForAccept = true;
            foreach (var item in cities)
            {
                var mark = Instantiate(MarkerPrefab, item.CityGO.transform.position, MarkerPrefab.transform.rotation);
                CityMarkDictionary.Add(mark, item);
            } 
        }
        //второй клик по кнопке дл€ подтверждени€ города
        else if(SelectedCity != null)
        {
            //выполненение действи€ 
            PayForAction(cost, isCoin);
            Action doAction = action;
            doAction(AllCitiesScript.PlayerCountry, SelectedCity);
            SelectedCity = null;
            //очистка меток
            WaitingForAccept = false;
            foreach(var item in CityMarkDictionary.Keys)
            {
                Destroy(item);
            }
            Destroy(SelectedCityMarkInstance);        
        }
    }

    public void CitySelected(GameObject mark)
    {
        SelectedCity = CityMarkDictionary[mark];
        if (SelectedCityMarkInstance != null)
        {
            Destroy(SelectedCityMarkInstance);
        }
        SelectedCityMarkInstance = Instantiate(SelectedMarkerPrefab, CityMarkDictionary[mark].CityGO.transform.position, MarkerPrefab.transform.rotation);

    }
    private void PayForAction(int cost, bool isCoin)
    {
        if (isCoin)
        {
            AllCitiesScript.PlayerCountry.Coins -= cost;
        }
        else
        {
            AllCitiesScript.PlayerCountry.Respect -= cost;
        }

    }
    private void Action1Action( Country country, City city)
    {
        city.Country.NextTimeVoteWithSomeOne(country);
    }
    private void Action2Action(Country country, City city)
    {
        city.Country.NextTimeNoteVoteIn(country);
    }
}
