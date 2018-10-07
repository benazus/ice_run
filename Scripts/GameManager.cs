using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    private const int COIN_SCORE_AMOUNT = 1;

	public static GameManager Instance { set; get; }

    public bool IsDead { set; get; }
    private bool isGameStarted = false;
    private PlayerMotor motor;

    // UI & fields
    public Animator gameCanvasAnimator, menuAnimator, diamondAnimator;
    public Text scoreText, coinText, modifierText, highScoreText;
    private float score, coin, modifier;
    private float lastScore;

    // Death Menu
    public Animator deathMenuAnimator;
    public Text deathScore, deathCoins;

    private void Awake() {
        Instance = this;
        modifier = 1f;
        score = 0;
        coin = 0;

        motor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();

        scoreText.text = score.ToString("0");
        coinText.text = coin.ToString("0");
        modifierText.text = "x" + modifier.ToString("0.0");
        highScoreText.text = PlayerPrefs.GetInt("Hiscore").ToString("0");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Return) && isGameStarted == false) {
            isGameStarted = true;
            motor.StartRunning();
            FindObjectOfType<GlacierSpawner>().IsScrolling = true;
            FindObjectOfType<CameraMotor>().IsMoving = true;
            gameCanvasAnimator.SetTrigger("Show");
            menuAnimator.SetTrigger("Hide");
        }

        if(isGameStarted == true && IsDead == false) {  
            score += (Time.deltaTime * modifier);
            if(lastScore != score) {
                lastScore = score;
                scoreText.text = score.ToString("0");
            }
        }
    }

    public void GetCoin() {
        diamondAnimator.SetTrigger("Collect");
        coin += COIN_SCORE_AMOUNT;
        coinText.text = coin.ToString("0");
    }

    public void UpdateModifier(float modifierAmount) {
        modifier = 1f + modifierAmount;
        modifierText.text = "x" + modifier.ToString("0.0");
    }

    public void OnPlayAgainButton() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public void OnDeath() {
        IsDead = true;
        FindObjectOfType<GlacierSpawner>().IsScrolling = false;
        deathScore.text = score.ToString("0");
        deathCoins.text = coin.ToString("0");
        deathMenuAnimator.SetTrigger("Death");
        gameCanvasAnimator.SetTrigger("Hide");

        // Check if high score
        if (score > PlayerPrefs.GetInt("Hiscore")) {
            float s = score;
            if(s % 1 == 0)
                s++;
            
            PlayerPrefs.SetInt("Hiscore", (int)s);
        }
    }

    public void CloseApplication() {
        Application.Quit();
    }
}