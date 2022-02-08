
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using System.Text;
using System.Globalization;

public class PlayFabManager : MonoBehaviour
{






    [Header("PlayFab")]

    public GameObject nameWindow;
    public GameObject LeaderboardWindow;

    public InputField nameInput;
    public GameObject nameError;


    public GameObject rowPrefab;
    public Transform rowsParent;


    string LoggedId;

    public static PlayFabManager Instance { get; set; }


    // Start is called before the first frame update
    void Start()
    {


        login();



    }






    public void AddScoreToLeaderboard(int score)
    {
        Debug.Log("Pontos " + score);
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate> { new StatisticUpdate
            {
                StatisticName="Semanal",
                Value =score
            } }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnSuccess, Onerror);
    }
    // Update is called once per frame

    void OnSuccess(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Enviou pro LeaderBoard /send to leaderboard");
    }


    void login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }

        };

        PlayFabClientAPI.LoginWithCustomID(request, onLoginSucces, Onerror);

    }

    void onLoginSucces(LoginResult result)
    {


        LoggedId = result.PlayFabId;
        Debug.Log("deu certo login");
        name = null;
        if (result.InfoResultPayload.PlayerProfile != null)
        {


            name = result.InfoResultPayload.PlayerProfile.DisplayName;

        }
        else
        {

        }
       AddScoreToLeaderboard(20);
        SubimittName();



        if (name == null || name.Equals(""))
        {
            Debug.Log("name null, mostrar nameWindow");
            Debug.Log(LoggedId);

            nameWindow.SetActive(true);
            LeaderboardWindow.SetActive(false);

        }
        else
        {
            Debug.Log("name completp, mostrar leadrarboar");
            Debug.Log(name);

            nameWindow.SetActive(false);
            LeaderboardWindow.SetActive(true);
            getLeaderBoardAround();

        }


    }

    public void SubimittName()
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {

            DisplayName = "Baroni"
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OndisplayNameUpdate, Onerror);
    }

    void OndisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {

        Debug.Log("OK");

        //nameWindow.SetActive(false);
        //LeaderboardWindow.SetActive(true);
        //getLeaderBoardAround();

    }

    void Onerror(PlayFabError playFabError)
    {
        Debug.Log("deu erro leadeboard");

        Debug.Log(playFabError.GenerateErrorReport());

    }


    public void getLeaderBoardAround()
    {
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = "teste",
            MaxResultsCount = 5

        };
        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, onLeaderboardAroundGet, Onerror);
    }




    public void onLeaderboardAroundGet(GetLeaderboardAroundPlayerResult result)
    {


        foreach (Transform item in rowsParent)
        {
            Destroy(item.gameObject);

        }
        foreach (var item in result.Leaderboard)
        {
            GameObject newGo = Instantiate(rowPrefab, rowsParent);
            Text[] texts = newGo.GetComponentsInChildren<Text>();
            texts[0].text = (item.Position + 1).ToString();

            if (item.DisplayName != null)
            {
                texts[1].text = item.DisplayName.ToString();

            }
            else
            {
                texts[1].text = item.PlayFabId.ToString();

            }
            texts[2].text = item.StatValue.ToString();

            if (item.PlayFabId == LoggedId)
            {
                texts[0].color = Color.green;
                texts[1].color = Color.green;
                texts[2].color = Color.green;

            }


            Debug.Log(item.Position + " " + item.PlayFabId + item.StatValue);
        }

    }


    public void getLeaderBoard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "teste",
            StartPosition = 0,
            MaxResultsCount = 20

        };
        PlayFabClientAPI.GetLeaderboard(request, onLeaderboardGet, Onerror);
    }


    public void onLeaderboardGet(GetLeaderboardResult result)
    {


        foreach (Transform item in rowsParent)
        {
            Destroy(item.gameObject);

        }
        foreach (var item in result.Leaderboard)
        {
            GameObject newGo = Instantiate(rowPrefab, rowsParent);
            Text[] texts = newGo.GetComponentsInChildren<Text>();
            texts[0].text = (item.Position + 1).ToString();
            texts[1].text = item.DisplayName.ToString();
            texts[2].text = item.StatValue.ToString();



            Debug.Log(item.Position + " " + item.PlayFabId + item.StatValue);
        }

    }

}