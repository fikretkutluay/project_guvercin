using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public Transform player1Area;
    public Transform player2Area;
    public Image player1PigeonImage;
    public Image player2PigeonImage;
    public Slider player1HealthBar;
    public Slider player2HealthBar;
    public Text turnIndicatorText;
    public Button attackButton;
    public Button skill1Button;
    public Button skill2Button;
    public Button skill3Button;

    private int currentPlayerTurn = 0; // 0: Player1, 1: Player2
    private int[] playerHealth = new int[2];
    private int maxHealth = 100;
    private bool battleEnded = false;

    private GameObject player1PigeonInstance;
    private GameObject player2PigeonInstance;

    void Start()
    {
        // Seçilen güvercinleri instantiate et
        var gm = GameManager.Instance;
        int p1Index = gm.players[0].selectedCharacterIndex;
        int p2Index = gm.players[1].selectedCharacterIndex;
        var p1Data = gm.pigeons[p1Index];
        var p2Data = gm.pigeons[p2Index];

        if (player1PigeonInstance != null) Destroy(player1PigeonInstance);
        if (player2PigeonInstance != null) Destroy(player2PigeonInstance);

        player1PigeonInstance = Instantiate(
            p1Data.pigeonPrefab,
            player1Area.position,
            Quaternion.identity,
            player1Area
        );
        player2PigeonInstance = Instantiate(
            p2Data.pigeonPrefab,
            player2Area.position,
            Quaternion.identity,
            player2Area
        );

        // Başlangıç değerleri
        maxHealth = p1Data.maxHealth; // Her iki güvercin için farklı maxHealth istersen ayrı ayrı tutabilirsin
        playerHealth[0] = p1Data.maxHealth;
        playerHealth[1] = p2Data.maxHealth;
        player1HealthBar.maxValue = p1Data.maxHealth;
        player2HealthBar.maxValue = p2Data.maxHealth;
        player1HealthBar.value = p1Data.maxHealth;
        player2HealthBar.value = p2Data.maxHealth;
        UpdateTurnIndicator();
        EnableActionButtons(true);
    }

    public void OnAttackButton()
    {
        if (battleEnded) return;
        int target = (currentPlayerTurn == 0) ? 1 : 0;
        int damage = 20;
        playerHealth[target] -= damage;
        if (playerHealth[target] < 0) playerHealth[target] = 0;
        UpdateHealthBars();
        CheckBattleEnd();
        if (!battleEnded) NextTurn();
    }

    public void OnSkill1Button()
    {
        if (battleEnded) return;
        int target = (currentPlayerTurn == 0) ? 1 : 0;
        int damage = 30;
        playerHealth[target] -= damage;
        if (playerHealth[target] < 0) playerHealth[target] = 0;
        UpdateHealthBars();
        CheckBattleEnd();
        if (!battleEnded) NextTurn();
    }

    public void OnSkill2Button()
    {
        if (battleEnded) return;
        int target = (currentPlayerTurn == 0) ? 1 : 0;
        int damage = 15;
        // Örnek: Skill 2 az vurur ama belki başka bir efekt eklenir
        playerHealth[target] -= damage;
        if (playerHealth[target] < 0) playerHealth[target] = 0;
        UpdateHealthBars();
        CheckBattleEnd();
        if (!battleEnded) NextTurn();
    }

    public void OnSkill3Button()
    {
        if (battleEnded) return;
        int target = (currentPlayerTurn == 0) ? 1 : 0;
        int damage = 10;
        // Örnek: Skill 3 çok az vurur ama başka bir avantajı olabilir
        playerHealth[target] -= damage;
        if (playerHealth[target] < 0) playerHealth[target] = 0;
        UpdateHealthBars();
        CheckBattleEnd();
        if (!battleEnded) NextTurn();
    }

    void NextTurn()
    {
        currentPlayerTurn = 1 - currentPlayerTurn;
        UpdateTurnIndicator();
        EnableActionButtons(true);
    }

    void UpdateTurnIndicator()
    {
        turnIndicatorText.text = (currentPlayerTurn == 0) ? "1. Oyuncunun Sırası" : "2. Oyuncunun Sırası";
    }

    void UpdateHealthBars()
    {
        player1HealthBar.value = playerHealth[0];
        player2HealthBar.value = playerHealth[1];
    }

    void CheckBattleEnd()
    {
        if (playerHealth[0] <= 0 || playerHealth[1] <= 0)
        {
            battleEnded = true;
            EnableActionButtons(false);
            if (playerHealth[0] <= 0 && playerHealth[1] <= 0)
                turnIndicatorText.text = "Berabere!";
            else if (playerHealth[0] <= 0)
                turnIndicatorText.text = "2. Oyuncu Kazandı!";
            else
                turnIndicatorText.text = "1. Oyuncu Kazandı!";
            // Burada GameManager'a bildirebilirsin
        }
    }

    void EnableActionButtons(bool enable)
    {
        attackButton.interactable = enable;
        skill1Button.interactable = enable;
        skill2Button.interactable = enable;
        skill3Button.interactable = enable;
    }
} 