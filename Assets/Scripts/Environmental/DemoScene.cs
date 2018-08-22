using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoScene : MonoBehaviour {

	private GameObject npc;
	private GameObject crate;
	private bool questCompleted = false;

	// Use this for initialization
	void Start () {
		npc = GameObject.Find ("NPC");
		crate = GameObject.Find ("CratePink");
	}
	
	// Update is called once per frame
	void Update () {
		if (!questCompleted && Vector2.Distance (npc.transform.position, crate.transform.position) < 2) {
			questCompleted = true;
			npc.GetComponent<DialogueManager> ().recordEvent ("Box Given", false);
			npc.GetComponent<DialogueManager> ().initiateDefaultConversation ();
		} 
	}
}
