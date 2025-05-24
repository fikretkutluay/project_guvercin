using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public Transform player1Area;
    public Transform player2Area;
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

    Animator player1Animator;
    Animator player2Animator;

    void Start()
    {
        Debug.Log("BattleManager Start called"); // Start fonksiyonunun çağrılıp çağrılmadığını kontrol et

        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("GameManager.Instance is null!");
            return;
        }

        Debug.Log("BattleManager Start: P1 index = " + GameManager.Instance.players[0].selectedCharacterIndex);

        // Seçilen güvercinleri instantiate et
        int p1Index = gm.players[0].selectedCharacterIndex;
        int p2Index = gm.players[1].selectedCharacterIndex;
        Debug.Log($"Player indices - P1: {p1Index}, P2: {p2Index}");

        if (p1Index < 0 || p1Index >= gm.pigeons.Length || p2Index < 0 || p2Index >= gm.pigeons.Length)
        {
            Debug.LogError($"Invalid pigeon indices! P1: {p1Index}, P2: {p2Index}, Total pigeons: {gm.pigeons.Length}");
            return;
        }

        var p1Data = gm.pigeons[p1Index];
        var p2Data = gm.pigeons[p2Index];

        if (player1PigeonInstance != null) Destroy(player1PigeonInstance);
        if (player2PigeonInstance != null) Destroy(player2PigeonInstance);

        // Debug log ekleyelim
        Debug.Log($"Instantiating pigeons - P1: {p1Data.pigeonName}, P2: {p2Data.pigeonName}");
        Debug.Log($"Player1Area position: {player1Area.position}, Player2Area position: {player2Area.position}");

        if (player1Area == null || player2Area == null)
        {
            Debug.LogError("Player areas are not assigned!");
            return;
        }

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
        player2PigeonInstance.transform.localPosition = Vector3.zero;

        // SpriteRenderer'ı flip.x ile çevir
        var sr = player2PigeonInstance.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.flipX = true;
        
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

        player1Animator = player1PigeonInstance.GetComponent<Animator>();
        player2Animator = player2PigeonInstance.GetComponent<Animator>();
    }

    public void OnAttackButton()
    {
        if (battleEnded) return;
        int target = (currentPlayerTurn == 0) ? 1 : 0;
        int damage = 20;
        playerHealth[target] -= damage;
        if (playerHealth[target] < 0) playerHealth[target] = 0;
        UpdateHealthBars();

        // Animasyon tetikle
        if (currentPlayerTurn == 0)
            player1PigeonInstance.GetComponent<Animator>().SetTrigger("attack");
        else
            player2PigeonInstance.GetComponent<Animator>().SetTrigger("attack");

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

        // Animasyon tetikle
        if (currentPlayerTurn == 0)
            player1Animator.SetTrigger("skill1");
        else
            player2Animator.SetTrigger("skill1");

        CheckBattleEnd();
        if (!battleEnded) NextTurn();
    }

    public void OnSkill2Button()
    {
        if (battleEnded) return;
        int target = (currentPlayerTurn == 0) ? 1 : 0;
        int damage = 15;
        playerHealth[target] -= damage;
        if (playerHealth[target] < 0) playerHealth[target] = 0;
        UpdateHealthBars();

        // Animasyon tetikle
        if (currentPlayerTurn == 0)
            player1Animator.SetTrigger("skill2");
        else
            player2Animator.SetTrigger("skill2");

        CheckBattleEnd();
        if (!battleEnded) NextTurn();
    }

    public void OnSkill3Button()
    {
        if (battleEnded) return;
        int target = (currentPlayerTurn == 0) ? 1 : 0;
        int damage = 10;
        playerHealth[target] -= damage;
        if (playerHealth[target] < 0) playerHealth[target] = 0;
        UpdateHealthBars();

        // Animasyon tetikle
        if (currentPlayerTurn == 0)
            player1Animator.SetTrigger("skill3");
        else
            player2Animator.SetTrigger("skill3");

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