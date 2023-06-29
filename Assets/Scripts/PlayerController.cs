using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Transactions;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float colorChangingSpeed = 0;

    public CityController CityControllerScript;
    public AllCities AllCitiesScript;
    public ButtonsController ButtonsControllerScript;
    public RespectCalculator RespectCalculatorScript;
    public OtherCountriesController OtherCountriesControllerScript;

    public bool IsGameStarted = false;
    //public bool IsVoting = false;
    private bool StopColorBlink = false;
    public bool WaitingOtherCountries = false;

    public GameObject FlagPrefab;
   
    public RoundInfo RoundInfo;
    public TMP_Text TextRoundInfo;
    public TMP_Text TextRoundResult;
    public TMP_Text TextRespect;
    public TMP_Text TextCoin;

    public Sprite AttackSprite;
    public Sprite ProtectSprite;

    public Sprite[] GraySprites;
    public Sprite[,] CityColorSprites;
    private int CityCount = 10;
    public void Start()
    {
        CityControllerScript = GameObject.Find("CityManager").GetComponent<CityController>();
        AllCitiesScript = GameObject.Find("CityManager").GetComponent<AllCities>();
        ButtonsControllerScript = GameObject.Find("ButtonsManager").GetComponent<ButtonsController>();
        RespectCalculatorScript = GameObject.Find("CityManager").GetComponent<RespectCalculator>();
        OtherCountriesControllerScript = GameObject.Find("CountriesManager").GetComponent<OtherCountriesController>();
        //загрузка спрайтов
        LoadCitySprites();
        
        RoundInfo = new RoundInfo();

        
    }

    private void LoadCitySprites()
    {
        GraySprites = Resources.LoadAll<Sprite>("Sprites/SpriteSheets/CityGray");
        CityColorSprites = new Sprite[CityCount, CityCount];
        for(int i = 0; i< CityCount; i++)
        {
            var sprites = Resources.LoadAll<Sprite>($"Sprites/SpriteSheets/CityCity{i}Color");
            for(int j = 0; j< CityCount; j++)
            {
                CityColorSprites[i, j] = sprites[j];
            }
        }
    }
    //Вызывается кнопкой Start
    public void StartGame()
    {
        //TODO: проверка на выбор страны
        IsGameStarted = true;
        //Видимость кнопок.
        ButtonsControllerScript.GameStarted();
    }
    //Вызывается по кнопке select
    public void Select()
    {

        colorChangingSpeed = 0.7f;
        //переменная нужна для перехода от окончания голосования до окончания мерцания
       // IsVoting = true;
        StopColorBlink = false;
        //Видимость кнопок.
        ButtonsControllerScript.ShowButtons(RoundInfo.CurrentStage, !StopColorBlink);
    }
    //Вызывается кнопкой Complete
    public void CompleteRound()
    {
        //есть проблема. Не получается функцию
        //RoundInfo.NextRound();
        //вызывать внутри этого метода, в случае с первым if
        if (RoundInfo.CurrentStage == GameStage.Vote || RoundInfo.CurrentStage == GameStage.Protection)
        {
            //IsVoting = false;
            AllCitiesScript.SaveVote(AllCitiesScript.PlayersVote, AllCitiesScript.PlayerCountry);
            StopColorBlink = true; // запускает механзим остановки мерцания
            //WaitingOtherCountries = true;
            OtherCountriesControllerScript.MakeVotes(RoundInfo.CurrentStage == GameStage.Protection);
            //RoundInfo.NextRound();
            //Видимость кнопок.
            ButtonsControllerScript.ShowButtons(RoundInfo.CurrentStage, RoundInfo.CurrentStage == GameStage.Protection);
        }
        //после окончания стадии резалтс подсчитывается репутация
        else if (RoundInfo.CurrentStage == GameStage.Results)
        {
            RespectCalculatorScript.CalculateRespect(RoundInfo.VotedCity.Last(), RoundInfo.CurrentStage);
            //RoundInfo.NextRound();
            ButtonsControllerScript.ShowButtons(RoundInfo.CurrentStage);
        }
        else if (RoundInfo.CurrentStage == GameStage.Join)
        {
            CompeleteRoundeCycle();
            // RoundInfo.NextRound();
            ButtonsControllerScript.ShowButtons(RoundInfo.CurrentStage, false);
        }
        RoundInfo.NextRound();
       
        //Поменять спрайт метки уже зная какой новый раунд
        //TODO: Возможно лучше перенести в другой скрипт и передавать currentstage аргументом
        ChangeMarkSprites();
    }
    
    public void Update()
    {
        if (colorChangingSpeed != 0 || StopColorBlink)
        {
            ChangeSpriteAlpha(AllCitiesScript.PlayerCountry.FindAllNeighbours(GameStage.Protection == RoundInfo.CurrentStage));
        }

        TextRoundInfo.text = RoundInfo.ToString();

        if (RoundInfo.CurrentStage == GameStage.Results)
        { 
            //Город больше не принадлежит стране
            if (RoundInfo.NeedChooseCity())
            {
                CalculateVotes();
                ChangeCitySpriteOnGray();
                var city = RoundInfo.VotedCity.Last();
                RemoveCityFromList(city);
                RoundInfo.FreeCityExist = true;
                if (city.Country.IsEmpty())
                {
                    AllCitiesScript.RemoveCountry(city.Country);
                }

                TextRoundResult.text = RoundInfo.VotedCity.Last().ToString();
            }
        }
        WritePlayersResources();

    }

    private void WritePlayersResources()
    {
        if (AllCitiesScript.PlayerCountry != null)
        {
            TextRespect.text = Convert.ToString(AllCitiesScript.PlayerCountry.Respect);
            TextCoin.text = Convert.ToString(AllCitiesScript.PlayerCountry.Coins);
        }
        else
        {
            TextRespect.text = "0";
        }
    }

    private void RemoveCityFromList(City votedCity)
    {
        votedCity.LostVoting();
        //Неадекватный код. Закомментил по ошибке 7 от 24.06.2023
        //foreach(var item in AllCitiesScript.CitiesObject)
        //{
        //    item.UpdateNeighbours(votedCity);
        //}
    }

    private void CalculateVotes()
    {
        //var sortedVotes = AllCitiesScript.CitiesObject.OrderByDescending(city => city.ThisRoundVotes).ToList();
        var resultVotes = AllCitiesScript.CitiesObject.OrderByDescending(city => city.ThisRoundSummary).ToList();
        var sameResult = resultVotes.Where(city => city.ThisRoundSummary == resultVotes[0].ThisRoundSummary).ToList();
        int cityIndex = UnityEngine.Random.Range(0, sameResult.Count);
        RoundInfo.VotedCity.Add(sameResult[cityIndex]);
    }
    private void ChangeCitySpriteOnGray()
    {
        var city = RoundInfo.VotedCity.Last();
        var spriteRenderer = city.CityGO.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = GraySprites[city.Id];
    }
    void ChangeSpriteAlpha(List<City> cities)
    {
        UnityEngine.Color color = new UnityEngine.Color();
        //На данный момент в голосовании могут участвовать собственные города тоже.
        var minAlphaValue = 0.5f;
        var maxAlphaValue = 1f;
        foreach (var city in cities)
        {
            //будет получаться много много раз
            var sprite = city.CityGO.GetComponent<SpriteRenderer>();
            color = sprite.color;
            if(color.a > maxAlphaValue)
            {
                color.a = maxAlphaValue;
            }
            color.a -= colorChangingSpeed * Time.deltaTime;
            sprite.color = color;
        }
        
       
        if (color.a < minAlphaValue || color.a >= maxAlphaValue)
        {
            colorChangingSpeed *= -1; //reverse 
        }
        if (StopColorBlink && color.a >= maxAlphaValue)
        {
            colorChangingSpeed = 0;
            StopColorBlink = false;
        }
    }
    

    private void ChangeMarkSprites()
    {
        if(RoundInfo.CurrentStage == GameStage.Vote)
        {
            AllCitiesScript.AtackMarkSpriteRenderer.sprite = AttackSprite;
        }
        if(RoundInfo.CurrentStage == GameStage.Protection)
        {
            AllCitiesScript.AtackMarkSpriteRenderer.sprite = ProtectSprite;
        }

        
    }
    private void CompeleteRoundeCycle()
    {
        if (RoundInfo.FreeCityExist)
        {
            var newOwner = AllCitiesScript.FindHiestRespectAroundCity(RoundInfo.VotedCity.Last(), RoundInfo.CurrentStage);
            newOwner.AddCity(RoundInfo.VotedCity.Last());
            RoundInfo.FreeCityExist = false;
            ChangeCitySprite(RoundInfo.VotedCity.Last(),newOwner);
            //обнулить показатели голосования
            AllCitiesScript.ResetCountryRoundStats(RoundInfo.RoundNum);
        }    
    }

    private void ChangeCitySprite(City city, Country newOwner)
    {
        var spriteRenderer = city.CityGO.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = CityColorSprites[newOwner.Id, city.Id];
    }
}

public class RoundInfo
{

    public int RoundNum { get; set; } = 0;
    public GameStage CurrentStage { get; set; } = 0;
    public List<City> VotedCity { get; set; } = new List<City>();
    public bool FreeCityExist { get; set; } = false;
    public void NextRound()
    {
        RoundNum++;
        if (CurrentStage == GameStage.Join)
        {
            CurrentStage = 0;
        }
        else
        {
            CurrentStage++;
        }
    }
    public override string ToString()
    {
        return $"Раунд № {RoundNum+1}\tстадия: {CurrentStage}";
    }
    public bool NeedChooseCity()
    {
        if (CurrentStage == GameStage.Results && VotedCity.Count != (RoundNum+2)/4)
        {
            return true;
        }
        else
        { 
            return false; 
        }
    }
}

public enum GameStage
{
    Vote,
    Protection,
    Results,
    Join
}
