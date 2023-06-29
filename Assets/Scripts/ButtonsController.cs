using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsController : MonoBehaviour
{
    public GameObject StartButton;
    public GameObject SelectButton;
    public GameObject CompleteButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowButtons(GameStage gs, bool waitingForVote = true)
    {
        if(gs == GameStage.Vote || gs == GameStage.Join)
        {
            Voting(waitingForVote);
        }
        else
        {
            CompleteButton.SetActive(true);
            SelectButton.SetActive(false);
        }
    }
    public void GameStarted()
    {
        StartButton.SetActive(false);
        CompleteButton.SetActive(false);
        SelectButton.SetActive(true);
    }
    private void Voting(bool waitingForVote)
    {
        CompleteButton.SetActive(waitingForVote);
        SelectButton.SetActive(!waitingForVote);
    }
}
