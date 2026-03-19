using System.Collections;
using UnityEngine;

public class MatchController : MonoBehaviour
{
    [SerializeField] private float roundDuration = 60f;
    [SerializeField] private int roundsToWin = 2;

    private FighterGameplay fighterA;
    private FighterGameplay fighterB;
    private float timer;
    private int scoreA;
    private int scoreB;
    private bool roundActive;
    private bool matchFinished;

    public float TimerNormalized => Mathf.Clamp01(timer / Mathf.Max(1f, roundDuration));
    public float TimerRemaining => Mathf.Max(0f, timer);
    public float RoundDuration => roundDuration;
    public int ScoreA => scoreA;
    public int ScoreB => scoreB;
    public int CurrentRound => scoreA + scoreB + 1;
    public bool RoundActive => roundActive;
    public bool MatchFinished => matchFinished;
    public string OverlayMessage { get; private set; } = string.Empty;

    public void Initialize(FighterGameplay a, FighterGameplay b)
    {
        fighterA = a;
        fighterB = b;

        fighterA.Defeated -= OnFighterDefeated;
        fighterB.Defeated -= OnFighterDefeated;
        fighterA.Defeated += OnFighterDefeated;
        fighterB.Defeated += OnFighterDefeated;

        StopAllCoroutines();
        StartCoroutine(RunMatchLoop());
    }

    private void OnDestroy()
    {
        if (fighterA != null)
        {
            fighterA.Defeated -= OnFighterDefeated;
        }

        if (fighterB != null)
        {
            fighterB.Defeated -= OnFighterDefeated;
        }
    }

    private void Update()
    {
        if (!roundActive || matchFinished)
        {
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = 0f;
            roundActive = false;
            fighterA.SetRoundActive(false);
            fighterB.SetRoundActive(false);
            ResolveRoundTimeout();
        }
    }

    private IEnumerator RunMatchLoop()
    {
        while (scoreA < roundsToWin && scoreB < roundsToWin)
        {
            ResetFighters();
            yield return RoundIntro();
            StartRound();

            while (roundActive)
            {
                yield return null;
            }

            yield return new WaitForSeconds(1.4f);
        }

        matchFinished = true;
        fighterA.SetRoundActive(false);
        fighterB.SetRoundActive(false);
        OverlayMessage = scoreA > scoreB ? fighterA.FighterName + " Claims Olympus" : fighterB.FighterName + " Claims Olympus";
    }

    private IEnumerator RoundIntro()
    {
        OverlayMessage = "Round " + CurrentRound;
        yield return new WaitForSeconds(1f);
        OverlayMessage = "Fight!";
        yield return new WaitForSeconds(0.65f);
        OverlayMessage = string.Empty;
    }

    private void StartRound()
    {
        timer = roundDuration;
        roundActive = true;
        fighterA.SetRoundActive(true);
        fighterB.SetRoundActive(true);
    }

    private void OnFighterDefeated(FighterGameplay defeated)
    {
        if (!roundActive || matchFinished)
        {
            return;
        }

        roundActive = false;
        fighterA.SetRoundActive(false);
        fighterB.SetRoundActive(false);

        if (defeated == fighterA)
        {
            scoreB++;
            OverlayMessage = fighterB.FighterName + " KO!";
        }
        else
        {
            scoreA++;
            OverlayMessage = fighterA.FighterName + " KO!";
        }
    }

    private void ResolveRoundTimeout()
    {
        if (fighterA.HealthNormalized >= fighterB.HealthNormalized)
        {
            scoreA++;
            OverlayMessage = fighterA.FighterName + " Wins by Decision";
        }
        else
        {
            scoreB++;
            OverlayMessage = fighterB.FighterName + " Wins by Decision";
        }
    }

    private void ResetFighters()
    {
        fighterA.ResetForRound(new Vector3(-3.6f, 0.5f, 0f));
        fighterB.ResetForRound(new Vector3(3.6f, 0.5f, 0f));
    }
}
