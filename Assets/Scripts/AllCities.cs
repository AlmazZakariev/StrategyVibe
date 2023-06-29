using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AllCities : MonoBehaviour
{
    public PlayerController PlayerControllerScript;

    public GameObject[] Cities;
    public List<City> CitiesObject = new List<City>();
    //public List<GameObject>[] Neighbors;
    public List<Country> Countries = new List<Country>();

    public GameObject Flag;
    public GameObject AtackMark;
    public SpriteRenderer AtackMarkSpriteRenderer;

    
    
    
    public Country PlayerCountry;
    public City PlayersVote;





    // Start is called before the first frame update
    void Start()
    {
        PlayerControllerScript = GameObject.Find("PlayerManager").GetComponent<PlayerController>();

        //Заполнение стран.
        for (int i = 0; i < Cities.Length; i++)
        {
            var color = Cities[i].GetComponent<SpriteRenderer>().color;
          
            var city = new City(Cities[i], i);
            var country = new Country("Страна" + i, city, color, i);
            Countries.Add(country);
            CitiesObject.Add(city);
            city.SetCountry(country);
        }

        //заполнение соседних городов, неизменные данные
        //TODO: подумать над более производительной реализацией.
        //Neighbors = new List<GameObject>[Cities.Length];

        CitiesObject[0].SetNeighbours(new List<City> 
        {
            CitiesObject[1],
            CitiesObject[4]
        });
        CitiesObject[1].SetNeighbours(new List<City> 
        {
            CitiesObject[0],
            CitiesObject[2],
            CitiesObject[4],
            CitiesObject[5]
        });
        CitiesObject[2].SetNeighbours(new List<City>
        {
            CitiesObject[1],
            CitiesObject[3],
            CitiesObject[5],
            CitiesObject[6]
        });
        CitiesObject[3].SetNeighbours(new List<City>
        {
            CitiesObject[2],
            CitiesObject[6],
            CitiesObject[9]
        });
        CitiesObject[4].SetNeighbours(new List<City>
        {
            CitiesObject[0],
            CitiesObject[1],
            CitiesObject[5],
            CitiesObject[7]
        });
        CitiesObject[5].SetNeighbours(new List<City>
        {
            //Cities[0],
            CitiesObject[1],
            CitiesObject[2],
            CitiesObject[4],
            CitiesObject[6],
            CitiesObject[7],
            CitiesObject[8]
        });
        CitiesObject[6].SetNeighbours(new List<City>
        {
            CitiesObject[2],
            CitiesObject[3],
            CitiesObject[5],
            CitiesObject[8],
            CitiesObject[9]
        });
        CitiesObject[7].SetNeighbours(new List<City>
        {
            CitiesObject[4],
            CitiesObject[5],
            CitiesObject[8]
        });
        CitiesObject[8].SetNeighbours(new List<City>
        {
            CitiesObject[5],
            CitiesObject[6],
            CitiesObject[7],
            CitiesObject[9]
        });
        CitiesObject[9].SetNeighbours(new List<City>
        {
            CitiesObject[3],
            CitiesObject[6],
            CitiesObject[8]
        });
        

        AtackMarkSpriteRenderer = AtackMark.GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetPlayerCountry(GameObject city)
    {
       
        for(int i=0; i<Cities.Length; i++)
        {
            if (Cities[i] == city)
            {

                PlayerCountry = Countries[i];
                Flag.SetActive(true);
                var a = Flag.transform.position = PlayerCountry.Cities[0].CityGO.transform.position;
                break;
            }
        }
    }
    // Сохраняет значение временного выбора до фиксации
    public void SetPlayerVote(City city)
    {
        PlayersVote= city;
        //PlayerCountry.AcceptVoting(city);

        AtackMark.SetActive(true);
        AtackMark.transform.position = city.CityGO.transform.position;
    }
    //фиксация значения
    public void SaveVote(City city, Country country)
    {
        country.AcceptVoting(city, PlayerControllerScript.RoundInfo.CurrentStage);
        if (country == PlayerCountry)
        {
            AtackMark.SetActive(false);
        }
    }
    public City FindCityObject(GameObject city)
    {
        foreach (var item in CitiesObject)
        {
            if(item.CityGO == city)
            {
                return item;
                
            }
        }
        return null;
    }

    public void RemoveCountry(Country country)
    {
        Countries.Remove(country);
    }
    public Country FindHiestRespectAroundCity(City city, GameStage gs)
    {
        //Debug.Log("FindHiestRespectAroundCity##"+city);
        var countries = new List<Country>();
        foreach(var item in city.Neighbours)
        {
            //Debug.Log("FindHiestRespectAroundCity##" + item);
            if (item.Country != city.Country)
            {
                countries.Add(item.Country);
            }
        }
        var results = countries.OrderByDescending(x => x.Respect).ToList();
        var sameResults = results.Where(c => c.Respect == results[0].Respect).ToList();
        var resultIndex = UnityEngine.Random.Range(0, sameResults.Count);
        return sameResults[resultIndex];
    }

    public void ResetCountryRoundStats(int num)
    {
        foreach(var item in Countries)
        {
            item.ResetStats();
        }
        foreach (var item in CitiesObject)
        {
            item.ResetStats();
        }
        CalculateCoins();

        //Логи
        SaveInfo(num);
    }

    private void SaveInfo(int RoundNum)
    {
        string path = "C:\\Users\\Almaz\\Desktop\\GAMEDEV\\PetProjects\\Game#1_StrategyVibes\\GameLogs.txt";
        using (var fw = new StreamWriter(path, true))
        {
            fw.WriteLine(RoundNum);
            foreach (var item in Countries)
            {
                fw.WriteLine(item.ToString());
            }
        }
    }

    private void CalculateCoins()
    {
        foreach(var item in Countries)
        {
            item.CalculateCoins();
        }
    }
}




