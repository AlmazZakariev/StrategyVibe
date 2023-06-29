using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class RespectCalculator : MonoBehaviour
{
    public AllCities AllCitiesScript;
    // Start is called before the first frame update
    void Start()
    {
        AllCitiesScript = GameObject.Find("CityManager").GetComponent<AllCities>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CalculateRespect(City voteResultCity, GameStage gs)
    {
        foreach(var country in AllCitiesScript.Countries)
        {
            var neighbours = country.FindAllNeighboursCountries(gs == GameStage.Protection);
            //≈сли страна проголосовала за победител€ голосовани€
            if (country.VoteCity == voteResultCity)
            {
                PointsDestribution(country, neighbours, voteResultCity, new(-1, -2, 1));             
            }
            //≈сли страна протектила победител€ голосовани€
            else if(country.ProtetedCity == voteResultCity) 
            {
                PointsDestribution(country, neighbours, voteResultCity, new(1, 2, -1));               
            }
            //≈сли страна проголосовала не за победител€ и не протектила
            else
            {
                PointsDestribution(country, neighbours, voteResultCity, new(1, 2, -1));               
            }           
        }
        
    }
    private void PointsDestribution(Country country, List<Country> neighbours, City voteResultCity, (int,int,int) points)
    {
        foreach (var neighbor in neighbours)
        {
            //≈сли сосед не проголосовал за победител€ голосовани€
            if (neighbor.VoteCity != voteResultCity)
            {
                country.Respect += points.Item1;
            }
            //≈сли сосед протектил победител€ голосовани€
            else if (neighbor.ProtetedCity == voteResultCity)
            {
                country.Respect += points.Item2;
            }
            //≈сли сосед голосовал за победител€ голосовани€
            else
            {
                country.Respect += points.Item3;
            }
        }
    }
}
