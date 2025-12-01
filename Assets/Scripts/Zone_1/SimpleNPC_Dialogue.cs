using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleNPC_Dialogue : MonoBehaviour
{
    [Header("Data & UI")]
    public Dialogue_1 dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;

    // Internal references
    private PlayerController control;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    void Update()
    {
        // Listen for input only when dialogue is active
        if (isDialogueActive)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1"))
            {
                NextLine();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Trigger automatically when player enters
        if (other.CompareTag("Player")) 
        {
            control = other.GetComponent<PlayerController>();
            if(control != null) 
            {
                control.canControl = false; // Freeze player
                // If using the updated PlayerController I gave you earlier, 
                // ensure it stops moving immediately:
                Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
                if(rb) rb.linearVelocity = Vector2.zero; 
            }
            
            StartDialogue();
        }
    }

    // Optional: Keep this if you want to trigger it via a button E instead of walking into it
    public void beInteracted()
    {
        if (dialogueData == null) return;

        if (!isDialogueActive)
        {
            StartDialogue();
        }
        else
        {
            NextLine();
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;
        
        if (dialogueData.npcName != null) nameText.SetText(dialogueData.npcName);
        if (dialogueData.npcPortrait != null) portraitImage.sprite = dialogueData.npcPortrait;
        
        dialoguePanel.SetActive(true);
        StartCoroutine(TypeLine());
    }

    void NextLine()
    {
        if (isTyping)
        {
            // If typing, show full line immediately
            StopAllCoroutines();
            dialogueText.SetText(dialogueData.dialogueLine[dialogueIndex]);
            isTyping = false;
        }
        else 
        {
            // If finished typing, go to next line
            dialogueIndex++;

            if (dialogueIndex < dialogueData.dialogueLine.Length)
            {
                StartCoroutine(TypeLine());
            }
            else
            {
                EndDialogue();
            }
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");

        // Logic to switch portraits based on your ScriptableObject data
        if (dialogueIndex < dialogueData.autoProgressLine.Length)
        {
            if (dialogueData.autoProgressLine[dialogueIndex])
                portraitImage.sprite = dialogueData.npcPortrait;
            else
                portraitImage.sprite = dialogueData.playerPortrait;
        }

        // Typewriter effect
        foreach (char letter in dialogueData.dialogueLine[dialogueIndex].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;
    }

    public void EndDialogue()
    {
        // 1. Unfreeze Player
        if(control != null) control.canControl = true;
        
        // 2. Cleanup UI
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);
        
        // 3. IMPORTANT: We do NOT destroy the object anymore, 
        // so the player can talk to this NPC again if they re-enter the trigger.
    }
}