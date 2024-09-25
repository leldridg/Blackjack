
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject lParticleSystem;
    public GameObject rParticleSystem;
    public GameObject menuPanel;
    public GameObject gamePanel;
    public TMP_Text gameOverTxt;
    public GameObject betPanel;
    public TMP_Text betTxt;
    public TMP_Text bankTxt;
    public GameObject deck;
    public List<Sprite> cards = new List<Sprite>();
    public List<Sprite> chips = new List<Sprite>();
    public Button hitBtn;
    public Button standBtn;
    public Button chipBtn;
    public TMP_Text instructionTxt;
    public TMP_Text playerHandValueTxt;
    public TMP_Text dealerHandValueTxt;
    public TMP_Text playerUpdateTxt;
    public TMP_Text dealerUpdateTxt;
    public float startDelay;
    public float dealDelay;
    public GameObject cardPrefab;
    public GameObject backPrefab;
    public GameObject playerHandParent;
    public GameObject dealerHandParent;
    private bool gameOngoing;
    private List<GameObject> playerHand = new List<GameObject>();
    private List<GameObject> dealerHand = new List<GameObject>();
    private Animator playerUpdateAnimator;
    private Animator dealerUpdateAnimator;
    private int currentOrder = 1;
    private int playerHandValue = 0;
    private int dealerHandValue = 0;
    private bool isPlayerTurn;
    private bool resetting;
    private bool isDealing;
    private int betAmount = 0;
    private int bankAmount = 1000;
    private Stack<Sprite> chipStack = new Stack<Sprite>();
    private bool isDeckOnScreen;

    // Start is called before the first frame update
    void Start()
    {
        playerUpdateAnimator = playerUpdateTxt.GetComponent<Animator>();
        dealerUpdateAnimator = dealerUpdateTxt.GetComponent<Animator>();

        bankTxt.text = "$" + bankAmount;
        betTxt.text = "$" + betAmount;

    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOngoing && chipStack.Count == 0)
        {
            betTxt.gameObject.SetActive(false);
            instructionTxt.text = "Click the chips to place a bet.";

            chipBtn.image.enabled = false;

            if (isDeckOnScreen)
            {
                deck.GetComponent<Deck>().MoveOffscreen();
                isDeckOnScreen = false;
            }
        }

        if (!gameOngoing && chipStack.Count > 0)
        {
            betTxt.gameObject.SetActive(true);
            instructionTxt.text = "Click the deck to start a round.";

            chipBtn.image.sprite = chipStack.Peek();
            chipBtn.image.enabled = true;

            if (!isDeckOnScreen)
            {
                deck.GetComponent<Deck>().MoveOnScreen();
                isDeckOnScreen = true;
            }
        }

        if (resetting)
        {
            dealerHandParent.GetComponent<Animator>().SetTrigger("HandOver");
            playerHandParent.GetComponent<Animator>().SetTrigger("HandOver");
            resetting = false;
        }

        if (gameOngoing)
        {
            // Player bust
            if (isPlayerTurn && playerHandValue > 21)
            {
                playerUpdateTxt.text = "Bust!";
                dealerUpdateTxt.text = "Dealer wins!";
                playerUpdateAnimator.Play("FadeInOut");
                dealerUpdateAnimator.Play("FadeInOut");
                hitBtn.gameObject.SetActive(false);
                standBtn.gameObject.SetActive(false);
                StartCoroutine(GameOverAndReset());
            }

            // Dealer bust
            if (!isPlayerTurn && dealerHandValue > 21)
            {
                playerUpdateTxt.text = "Player wins!";
                dealerUpdateTxt.text = "Bust!";
                playerUpdateAnimator.Play("FadeInOut");
                dealerUpdateAnimator.Play("FadeInOut");
                bankAmount += (int)System.Math.Round(betAmount * 1.5);
                StartCoroutine(GameOverAndReset());
            } 

            // Dealer hit
            if (!isPlayerTurn && dealerHandValue < 17 && !isDealing)
            {
                isDealing = true;
                StartCoroutine(DealAndWait());
            }

            // Dealer is done hitting, but didn't bust; time to determine winner (if any) and end game
            if (!isPlayerTurn && dealerHandValue >= 17 && dealerHandValue <= 21)
            {
                if (dealerHandValue == playerHandValue)
                {
                    dealerUpdateTxt.text = "Tie!";
                    dealerUpdateAnimator.Play("FadeInOut");
                    bankAmount += betAmount;
                }
                if (dealerHandValue > playerHandValue)
                {
                    playerUpdateTxt.text = "Player loses!";
                    dealerUpdateTxt.text = "Dealer wins!";
                    playerUpdateAnimator.Play("FadeInOut");
                    dealerUpdateAnimator.Play("FadeInOut");
                }
                if (dealerHandValue < playerHandValue)
                {
                    playerUpdateTxt.text = "Player wins!";
                    dealerUpdateTxt.text = "Dealer loses!";
                    playerUpdateAnimator.Play("FadeInOut");
                    dealerUpdateAnimator.Play("FadeInOut");
                    bankAmount += (int)System.Math.Round(betAmount * 1.5);
                }
                
                StartCoroutine(GameOverAndReset());
            }
        }
        
    }

    public void RemoveTopChip()
    {
        if (chipStack.Count > 0)
        {
            int value = int.Parse(chipStack.Pop().name.Split('_').Last());

            bankAmount += value;
            betAmount -= value;

            bankTxt.text = "$" + bankAmount;
            betTxt.text = "$" + betAmount;
        }
    }

    IEnumerator DealAndWait()
    {
        DealToDealer();
        yield return new WaitForSeconds(dealDelay);
        isDealing = false;
    }

    IEnumerator GameOverAndReset()
    {
        gameOngoing = false;
        bankTxt.text = "$" + bankAmount;

        yield return new WaitForSeconds(startDelay*5);

        resetting = true;
        playerHandValueTxt.gameObject.SetActive(false);
        dealerHandValueTxt.gameObject.SetActive(false);

        yield return new WaitForSeconds(startDelay*2);
        
        chipStack = new Stack<Sprite>();
        betAmount = 0;
        betTxt.text = "$" + betAmount;

        if (bankAmount <= 0)
        {
            // GAME OVER
            
            gameOverTxt.gameObject.SetActive(true);
            yield return new WaitForSeconds(startDelay*5);
            gameOverTxt.gameObject.SetActive(false);
            menuPanel.SetActive(true);
            lParticleSystem.SetActive(true);
            rParticleSystem.SetActive(true);
            gamePanel.SetActive(false);
            bankAmount = 1000;
            bankTxt.text = "$" + bankAmount;
        }
        else
        {
            betPanel.GetComponent<Animator>().SetTrigger("MoveOnScreen");
        }

        instructionTxt.gameObject.SetActive(true);
    }

    public void StartGame()
    {
        StartCoroutine(WaitAndStart());
    }

    IEnumerator WaitAndStart()
    {
        instructionTxt.gameObject.SetActive(false);
        currentOrder = 1;
        dealerHandValue = 0;
        dealerHand = new List<GameObject>();
        playerHandValue = 0;
        playerHand = new List<GameObject>();

        yield return new WaitForSeconds(startDelay);

        // Deal cards

        DealToPlayer();

        yield return new WaitForSeconds(dealDelay);

        GameObject card = Instantiate(backPrefab, dealerHandParent.transform);
        card.tag = "Dealer";
        dealerHand.Add(card);

        yield return new WaitForSeconds(dealDelay);
        
        DealToPlayer();

        yield return new WaitForSeconds(dealDelay);

        DealToDealer();

        yield return new WaitForSeconds(dealDelay);

        // Render hand values
        playerHandValueTxt.gameObject.SetActive(true);
        dealerHandValueTxt.gameObject.SetActive(true);

        // Render "hit" and "stand" buttons
        hitBtn.gameObject.SetActive(true);
        standBtn.gameObject.SetActive(true);

        gameOngoing = true;
        isPlayerTurn = true;
    }

    public void AddChip(int value)
    {
        bankAmount -= value;
        betAmount += value;

        bankTxt.text = "$" + bankAmount;
        betTxt.text = "$" + betAmount;

        foreach (Sprite chip in chips)
        {
            if (int.Parse(chip.name.Split('_').Last()) == value)
            {
                chipStack.Push(chip);
            }
        }
    }

    public void Stand()
    {
        isPlayerTurn = false;
        int index = Random.Range(0, cards.Count);
        dealerHand[0].GetComponent<SpriteRenderer>().sprite = cards[index];
        dealerHandValue = CalculateHand(dealerHand);
        dealerHandValueTxt.text = "" + dealerHandValue;
        hitBtn.gameObject.SetActive(false);
        standBtn.gameObject.SetActive(false);
    }

    public void DealToPlayer()
    {
        MoveLeft(playerHand);
        GameObject card = Instantiate(cardPrefab, playerHandParent.transform);
        SpriteRenderer spriteRenderer = card.GetComponent<SpriteRenderer>();
        int index = Random.Range(0, cards.Count);
        spriteRenderer.sprite = cards[index];
        card.tag = "Player";
        card.GetComponent<SpriteRenderer>().sortingOrder = currentOrder++;
        playerHand.Add(card);
        playerHandValue = CalculateHand(playerHand);
        playerHandValueTxt.text = "" + playerHandValue;
    }

    void DealToDealer()
    {
        MoveLeft(dealerHand);
        GameObject card = Instantiate(cardPrefab, dealerHandParent.transform);
        SpriteRenderer spriteRenderer = card.GetComponent<SpriteRenderer>();
        int index = Random.Range(0, cards.Count);
        spriteRenderer.sprite = cards[index];
        card.tag = "Dealer";
        card.GetComponent<SpriteRenderer>().sortingOrder = currentOrder++;
        dealerHand.Add(card);
        Debug.Log("added " + card.GetComponent<SpriteRenderer>().sprite.name);
        dealerHandValue = CalculateHand(dealerHand);
        dealerHandValueTxt.text = "" + dealerHandValue;
    }

    void MoveLeft(List<GameObject> hand)
    {
        foreach (GameObject card in hand)
        {
            card.transform.Translate(-0.3f, 0, 0);
            Debug.Log("Card: " + card.GetComponent<SpriteRenderer>().sprite.name);
            
        }
    }

    int CalculateHand(List<GameObject> hand)
    {
        int totalValue = 0;
        int aceCount = 0;

        // Calculate the total value and count Aces

        foreach (GameObject card in hand)
        {
            string value = card.GetComponent<SpriteRenderer>().sprite.name.Split('_').Last();
            Debug.Log("Sprite name: " + card.GetComponent<SpriteRenderer>().sprite.name + "Value: " + value);

            if (value.Equals("A"))
            {
                totalValue += 11;
                aceCount++;
            }
            else if (value.Equals("K") || value.Equals("Q") || value.Equals("J"))
            {
                totalValue += 10;
            }
            else if (!value.Equals("Back"))
            {
                totalValue += int.Parse(value);
            }
        }

        // Adjust for Aces if total value exceeds 21

        while (totalValue > 21 && aceCount > 0)
        {
            totalValue -= 10;
            aceCount--;
        }

        return totalValue;
    }
}
