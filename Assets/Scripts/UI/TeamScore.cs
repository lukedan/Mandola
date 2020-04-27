using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamScore : MonoBehaviour
{
    public GameObject scoreCanvasPrefab;

    // Start is called before the first frame update
    void Start()
    {
        Teams teams = InGameCommon.CurrentGame.GetComponent<Teams>();
        for (int i = 0; i < teams.Colors.Count; ++i)
        {
            GameObject canvas = Instantiate(scoreCanvasPrefab, transform);
            TeamScoreCanvas logic = canvas.GetComponent<TeamScoreCanvas>();
            logic.TeamColor = teams.Colors[i];
            logic.TeamIndex = i;
        }
    }
}
