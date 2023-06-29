using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;


public class Country
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Color Color { get; set; }
    public int Respect { get; set; } = 3;
    public int Coins { get; set; } = 0;
    public List<City> Cities { get; set; } = new List<City>();
    public int VotesCount { get; set; }
    public City VoteCity;
    public City ProtetedCity;
    //public int VoteCityId { get; set; }
    private List<City> Votes = new List<City>();
    private List<City> Protections = new List<City>();
    public ActionsStuff ActionsStuff { get; set; } = new ActionsStuff();
   
    public Country(string name, City city, Color color, int id)
    {
        Name = name;
        Cities.Add(city);
        Color = color;
        Id = id;
    }
    public void AcceptVoting(City city, GameStage gs)
    {
        //Debug.Log("В классе страны: " + Name + "  "+ city +"  " + gs);
        if (gs == GameStage.Vote)
        {
            VoteCity = city;
            Votes.Add(city);
            city.ThisRoundVotes++;
        }
        else if (gs == GameStage.Protection)
        {
            ProtetedCity = city;
            Protections.Add(city);
            city.ThisRoundProtects++;
        }
    }
    //возвращает истину если в стране остались города
    public void RemoveCity(City city)
    {
        Cities.Remove(city);

    }
    public bool IsEmpty()
    {
        if (Cities.Count == 0)
        {
            //логика экономики при изменении количества городов
            return true;
        }
        else
        {


            return false;
        }
    }
    public List<City> FindAllNeighbours(bool votedCity)
    {
        var allNeighbours = new List<City>();

        foreach (var item in Cities)
        {
            allNeighbours.AddRange(item.Neighbours);
        }
        var distinctCities = allNeighbours.GroupBy(n => n).Select(n => n.First()).ToList();
        //var result = new List<City>();
        //исключение собственных городов
        foreach (var item in Cities)
        {
            distinctCities = distinctCities.Where(c => c != item).ToList();

        }
        //исключение возможности протектить того, за кого проголосовал
        if (votedCity)
        {
            distinctCities = distinctCities.Where(c => c != VoteCity).ToList();
        }
        return distinctCities;
    }
    public List<Country> FindAllNeighboursCountries(bool votedCity)
    {
        var cityNeighbours = FindAllNeighbours(votedCity);

        var countryNeighbours = new List<Country>();
        foreach (var item in cityNeighbours)
        {
            if (!countryNeighbours.Contains(item.Country))
            {
                countryNeighbours.Add(item.Country);
            }
        }
        return countryNeighbours;
    }

    public void AddCity(City city)
    {
        Cities.Add(city);
        city.SetNewCountry(this);
        ChangeRespectCouseAdding();
    }

    private void ChangeRespectCouseAdding()
    {
        var neighbours = FindAllNeighboursCountries(false);
        Respect -= neighbours.Count;
    }

    public void ResetStats()
    {
        VoteCity = null;
        ProtetedCity = null;
    }

    public void CalculateCoins()
    {
        Coins += Cities.Count;
    }
    public override string ToString()
    {
        var cities = "";
        foreach(var item in Cities)
        {
            cities += item.Id+ ", ";
        }
        return $"{Id}: respect {Respect}, couns {Coins}. Cities: {cities}. Vote {Votes.Last().CityGO.name}, protect {Protections.Last().CityGO.name}.";
    }
    public bool IsEnableAction(int cost, bool isCoin)
    {
        if (isCoin)
        {
            if (Coins >= cost)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (Respect >= cost)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public bool NextTimeVoteWithSomeOne(Country country)
    {
        //если страна, которая вызвала это действие, сама не голосует с кем-то
        if(country.ActionsStuff.VoteWith != null)
        {
            return false;
        }
        //и если текущая страна ещё не голосует с кем-то
        if(ActionsStuff.VoteWith != null)
        {
            return false;
        }
        //то действие применяется
        ActionsStuff.VoteWith = country;
        return true;
    }

    public bool NextTimeNoteVoteIn(Country country)
    {
        if (country.ActionsStuff.NotVoteIn != null)
        {
            return false;   
        }
        ActionsStuff.NotVoteIn = country;
        return true;
    }
}
public class ActionsStuff
{
    public Country VoteWith;
    public Country ProtectWith;
    public Country NotVoteIn;
    public Country NotProtectedTargetOf;
}
