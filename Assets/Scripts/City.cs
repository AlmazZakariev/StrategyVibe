using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City
{

    public string Name { get; set; }
    public int Id { get; set; }
    public Country Country { get; set; }
    public GameObject CityGO { get; set; }
    public int ThisRoundVotes { get; set; }
    public int ThisRoundProtects { get; set; }
    public int ThisRoundSummary { get; set; }
    public List<City> Neighbours { get; set; }
    public City(GameObject city, int id)
    {
        CityGO = city;
        Id = id;
    }
    public void SetNeighbours(List<City> neighbours)
    {
        Neighbours = neighbours;
    }
    public void SetCountry(Country country)
    {
        Country = country;
    }
    public void CalculateRoundSummary()
    {
        ThisRoundSummary = ThisRoundVotes - ThisRoundProtects;
    }
    public override string ToString()
    {
        return CityGO.name + " +" + ThisRoundVotes + " -" + ThisRoundProtects;
    }

    public void LostVoting()
    {
        //тут потенциальная логика для свободного города
        Country.RemoveCity(this);
    }

    public void UpdateNeighbours(City votedCity)
    {
        //Неадекватный код. Закомментил по ошибке 7 от 24.06.2023
        //Neighbours.Remove(votedCity);
    }

    public void ResetStats()
    {
        ThisRoundVotes = 0;
        ThisRoundProtects = 0;
        ThisRoundSummary = 0;
    }
    public void SetNewCountry(Country country)
    {
        Country = country;
    }
}
