using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimpleJSON;
using UnityStandardAssets._2D;

public class DialogueManager : MonoBehaviour {

	public TextAsset NpcDialogue;
	public TextAsset PlayerDialogue;

	public TextAsset localEvents;
	public TextAsset globalEvents;

	public bool clearEventsOnStart = true; // For testing

	public string startingIndex;
	public int talkingDistance = 4;
	public float pauseBetweenLines = 0.2f;

	public List<string> dialogueIDs;
	public List<AudioClip> dialogueRecordings;

	public List<string> playerDialogueIDs;
	public List<AudioClip> playerDialogueRecordings;

	private PlatformerCharacter2D moveController; // For if movement is required
	private JSONNode dialogueCatalog;
	private JSONNode playerDialogueCatalog;
	private bool isConversing = false;

	// Using these for if we ever change json schema
	private const string TEXT_KEY = "text";
	private const string AUDIO_KEY = "voice";
	private const string NEXT_KEY = "nextLine";
	private const string ALTERNATE_KEY = "alternateLine";
	private const string RESPONSE_KEY = "responseIds";
	private const string OPTION_KEY = "option";
	private const string CONDITION_KEY = "condition";
	private const string APPLY_CONDITION_KEY = "applyCondition";

	private GameObject player;
	private GameObject clickToTalk;
	private UIDialogue uiDialogue;
	private AudioSource audioSource;

	private Dictionary<string, AudioClip> dialogueDict;
	private Dictionary<string, AudioClip> playerDialogueDict;

	public void initiateDefaultConversation(){
		if (!isConversing) {
			isConversing = true;
			StartCoroutine(say (startingIndex, false));
		}
	}

	public void initiateSpecificConversation(string startingId){
		if (!isConversing) {
			isConversing = true;
			StartCoroutine(say (startingId, false));
		}
	}

	void endDialogue(){
		isConversing = false;
		uiDialogue.hideSubtitle ();
	}

	private JSONNode getProperDialogueSet(bool isPlayer){
		return isPlayer ? playerDialogueCatalog : dialogueCatalog;
	}

	private Dictionary<string, AudioClip> getProperVoiceSet(bool isPlayer){
		return isPlayer ? playerDialogueDict : dialogueDict;
	}

	public bool eventHasHappened(string eventName){
		bool local = UnityEditor.ArrayUtility.Contains (localEvents.text.Split(','), eventName);
	
		return local ||  UnityEditor.ArrayUtility.Contains (globalEvents.text.Split (','), eventName);
	}

	public void recordEvent(string eventName, bool isGlobal){
		if (!eventHasHappened (eventName)) {
			appendText (isGlobal ? globalEvents : localEvents, eventName);
		}
	}

	public void appendText(TextAsset file, string eventName){
		File.AppendAllText(UnityEditor.AssetDatabase.GetAssetPath(file), "," + eventName);
		UnityEditor.EditorUtility.SetDirty (file);
		UnityEditor.AssetDatabase.Refresh ();
	}

	public IEnumerator say(string index, bool isPlayer){

		JSONNode dialogueSet = getProperDialogueSet (isPlayer);

		JSONNode conditon = dialogueSet [index] [CONDITION_KEY];

		if ( !conditon.Equals(null) ) {
			Debug.Log ("A");

			Debug.Log (conditon);
			Debug.Log (!eventHasHappened (conditon));

			if (!eventHasHappened (conditon)) {
				Debug.Log ("B");

				StartCoroutine(say (dialogueSet [index][ALTERNATE_KEY], isPlayer));
				yield break;
			}
		}

		JSONNode applyCondition = dialogueSet [index] [APPLY_CONDITION_KEY];
		if (applyCondition.Count > 0) {
			recordEvent (applyCondition [0], applyCondition [1].AsBool);
			Debug.Log ("Applying condition " + applyCondition[0]);
		}

		string subtitle = dialogueSet [index] [TEXT_KEY];
		AudioClip audio = getProperVoiceSet(isPlayer)[index];

		uiDialogue.displaySubtitle (subtitle);
		audioSource.clip = audio;
		audioSource.Play ();

		yield return new WaitForSeconds(audio.length + pauseBetweenLines);

		string nextLine = dialogueSet[index][NEXT_KEY];

		string[] ids = toStringArray(dialogueSet[index][RESPONSE_KEY].AsArray);

		if (nextLine != null && nextLine.Length > 0) {
			
			StartCoroutine (say (nextLine, isPlayer));

		} else if (ids.Length > 1) {
			
			// Player will only ever get to make choices
			// so we know this must be a player-controlled choice
			uiDialogue.showOptions(getOptionsText (index, ids), ids, this);

		} else if (ids.Length == 1){
			// This could be player or an NPC so just flip isPlayer and speak
			StartCoroutine (say(ids[0], !isPlayer));

		} else if (ids.Length == 0) {
			endDialogue ();
		}
	}

	private string[] getOptionsText(string index, string[] ids){
		string[] options = new string[ids.Length];

		for (int i = 0; i < ids.Length; i++) {
			options [i] = playerDialogueCatalog[ids[i]][OPTION_KEY];
		}

		return options;
	}

	private string[] toStringArray(JSONArray arr){
		string[] strArr = new string[arr.Count];

		for (int i = 0; i < arr.Count; i++) {
			strArr [i] = arr [i];
		}

		return strArr;
	}

	void facePlayer(){
		bool direction = player.transform.position.x > transform.position.x;

		// Map 0/1 to 0.0/0.1, then subtract .05 to get 0.05/-0.05
		// This will barely move NPC in direction to face player
		float movement = (System.Convert.ToInt16(direction) / 10.0f) - 0.05f;

		// Move 1 frame in direction
		// Just enough to turn around, then stop
		moveController.Move (movement, false, false);
		moveController.Move (0, false, false);

		// TODO: Fix click to talk bug
	}

	void parseDialogue(){
		dialogueCatalog = JSON.Parse (NpcDialogue.text);
		playerDialogueCatalog = JSON.Parse (PlayerDialogue.text);
	}

	void displayTalkButton(){
		clickToTalk.SetActive (true);
	}

	void hideTalkButton(){
		clickToTalk.SetActive (false);
	}

	private Dictionary<string, AudioClip> createDictionaryDialogue(List<string> names, List<AudioClip> sounds){
		if (names.Count != sounds.Count) {
			throw new System.ArgumentException ("Must have equal number of names and audio files");
		} else {
			Dictionary<string, AudioClip> dict = new Dictionary<string, AudioClip>();

			for (int i = 0; i < names.Count; i++) {
				dict.Add (names[i], sounds[i]);
			}

			return dict;
		}
	}

	void Start(){
		player = GameObject.Find ("Player");
		moveController = this.GetComponent<PlatformerCharacter2D> ();
		clickToTalk = transform.Find ("ClickToTalk").gameObject;
		uiDialogue = GameObject.Find("Canvas").GetComponent<UIDialogue>();
		audioSource = GetComponent<AudioSource>();
		dialogueDict = createDictionaryDialogue (dialogueIDs, dialogueRecordings);
		playerDialogueDict = createDictionaryDialogue (playerDialogueIDs, playerDialogueRecordings);
		parseDialogue ();

		if (clearEventsOnStart) {
			File.WriteAllText(UnityEditor.AssetDatabase.GetAssetPath(localEvents), "");
			UnityEditor.EditorUtility.SetDirty (localEvents);

			File.WriteAllText(UnityEditor.AssetDatabase.GetAssetPath(globalEvents), "");
			UnityEditor.EditorUtility.SetDirty (globalEvents);
			UnityEditor.AssetDatabase.Refresh ();
		}
	}

	void Update(){
		if (!isConversing && Vector2.Distance (this.transform.position, player.transform.position) < talkingDistance) {
			displayTalkButton ();
		} else {
			hideTalkButton ();
		}

		if(Input.GetMouseButtonDown(0) && clickToTalk.GetComponent<PolygonCollider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition) ) ){
			initiateDefaultConversation ();
		}

		if (isConversing) {
			facePlayer ();
		}
	}
}
