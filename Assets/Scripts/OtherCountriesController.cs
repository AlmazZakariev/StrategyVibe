using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class OtherCountriesController : MonoBehaviour
{
    //public PlayerController PlayerControllerScript;
    public AllCities AllCitiesScript;
    public CityController CityControllerScript;
    public ButtonsController ButtonsControllerScript;
    // Start is called before the first frame update
    void Start()
    {
        //PlayerControllerScript = GameObject.Find("PlayerManager").GetComponent<PlayerController>();
        AllCitiesScript = GameObject.Find("CityManager").GetComponent<AllCities>();
        ButtonsControllerScript = GameObject.Find("ButtonsManager").GetComponent<ButtonsController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void MakeVotes(bool isProtection)
    {
        //var gameStage = PlayerControllerScript.RoundInfo.CurrentStage;

        
        MakeOtherCountriesVotes(isProtection);
        if (isProtection)
        {
            SaveResultInFile();
        }
        //PlayerControllerScript.WaitingOtherCountries = false;


    }
    private void MakeOtherCountriesVotes(bool isProtection)
    {
        foreach(var country in AllCitiesScript.Countries)
        {

            if(country == AllCitiesScript.PlayerCountry)
            {
                continue;
            }
            //для action1
            if(country.ActionsStuff.VoteWith != null&& !isProtection)
            {
                continue;
            }
            //для action3
            if(country.ActionsStuff.ProtectWith != null&& isProtection)
            {
                continue;
            }
            var neighbours = country.FindAllNeighbours(isProtection);
            //для action 2
            if(country.ActionsStuff.NotVoteIn != null)
            {
                if(neighbours.Count > 1)
                {
                    var num = Random.Range(0, country.ActionsStuff.NotVoteIn.Cities.Count);
                    neighbours.Remove(country.ActionsStuff.NotVoteIn.Cities[num]);
                }
                country.ActionsStuff.NotVoteIn = null;
            }
            var voteCity = Random.Range(0, neighbours.Count);
            AllCitiesScript.SaveVote(neighbours[voteCity], country); 
        }
        //голосование с другой страной
        foreach(var country in AllCitiesScript.Countries)
        {
            if(country.ActionsStuff.VoteWith != null && !isProtection)
            {
                AllCitiesScript.SaveVote(country.ActionsStuff.VoteWith.VoteCity, country);
                country.ActionsStuff.VoteWith = null;
            }
            if(country.ActionsStuff.ProtectWith != null && isProtection)
            {

            }
        }
        foreach(var city in AllCitiesScript.CitiesObject)
        {
            city.CalculateRoundSummary();
        }
        //TODO: переделать систему смещения раунда
        //PlayerControllerScript.RoundInfo.NextRound();
        
        //var isWaiting = PlayerControllerScript.IsVoting;
        //var gameStage = PlayerControllerScript.RoundInfo.CurrentStage;
        //ButtonsControllerScript.ShowButtons(gameStage, isWaiting);
    }
    private void SaveResultInFile()
    {
        using(StreamWriter sw = new StreamWriter("C:\\Users\\Almaz\\Desktop\\Unity_Projects\\StrategyVibe\\votes.txt"))
        {
            foreach ( var item in AllCitiesScript.Countries)
            {
                sw.WriteLine("Голос" + item.VoteCity + "\t" + "Протекция" + item.ProtetedCity);
            }
        }
    }
           
}
