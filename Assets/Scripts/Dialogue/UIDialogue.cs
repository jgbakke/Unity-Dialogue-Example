using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogue : MonoBehaviour {

	private AudioSource audioPlayer;
	private GameObject dialogueChoices;
	private DialogueManager dialogueManager;
	public bool subtitlesEnabled = true;
	private Text subtitle;
	private string[] storedIds;

	public GameObject subtitleObject;
	public List<GameObject> buttons;

	// Use this for initialization
	void Start () {
		subtitle = subtitleObject.GetComponent<Text> ();
		hideSubtitle ();
		audioPlayer = GetComponent<AudioSource> ();
		dialogueChoices = GameObject.Find ("Dialogue Choices");
		hideDialogue ();
	}

	private void hideDialogue(){
		buttons.ForEach (btn => btn.SetActive(false) );
	}

	public void hideSubtitle(){
		subtitle.enabled = false;
	}

	public void displaySubtitle(string text){
		subtitle.enabled = subtitlesEnabled;
		subtitle.text = text;
	}

	public void showOptions(string[] options, string[] ids, DialogueManager caller){
		hideSubtitle ();
		dialogueManager = caller; // Save this so we know which dialogueManager to go back to
		storedIds = ids; // Save the IDs here so we know which dialogue ID to call based on option

		for (int i = 0; i < options.Length; i++) {
			buttons [i].SetActive (true);
			buttons [i].transform.Find ("Text").GetComponent<Text> ().text = options[i];
		}
	}

	public void selectOption(int option){
		hideDialogue ();
		StartCoroutine (dialogueManager.say(storedIds[option], true));
	}
}
