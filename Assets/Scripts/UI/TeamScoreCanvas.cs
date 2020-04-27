using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamScoreCanvas : MonoBehaviour
{
    public Color TeamColor;
    public int TeamIndex;

    public Text scoreText;

    public int score = 0;
    // Start is called before the first frame update
    void Start()
    {
        scoreText = GetComponent<Text>();
        scoreText.color = TeamColor;
        scoreText.text = string.Format("Team{0} Score: " + score, TeamIndex + 1);
    }

    // Update is called once per frame
    void Update()
    {
        Teams teams = InGameCommon.CurrentGame.GetComponent<Teams>();
        score = teams.Scores[TeamIndex];
        scoreText.text = string.Format("Team{0} Score: " + score, TeamIndex + 1);
    }
}
