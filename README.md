# Unity-Dialogue-Example

For a newer dialogue system, see the other project "RevNPC". This has certain features that are not part of RevNPC, such as not relying on AI to understand player's voice. However, this was made 1 full year before that so there are many lessons I learned from this that I applied towards RevNPC

Branching dialogue system for use with Unity Engine. Features a basic GUI for player to select from multiple options, a branching system where player dialogue options can lead to different NPC responses configured by the user, and a system to optionally track events so NPCs can respond to previous quests or player choices, allowing for dynamic dialogue. Ideal for text-heavy games such as RPGs

This demo scene was made using Unity Demo Assets.

If you want to see a Demo Video just open up demo.mov. This is just a demo so you can easily substitute any sound files with your own by including your own paths, or art, or any assets you have.

There are still changes planned for this, so you can use it but keep in mind that there will be additional updates in the future.

**If you do not need the example project and only want the files to add to Game Objects:** (Recommended)
  Just clone the repo and then move the directories Assets/Scripts/Dialogue and Assets/Scripts/lib into your project.

If you **do** want to see the example project
  Clone the repo and open it
  
  **Note**: There is an issue with the way Unity stores scenes. For an unknown reason, Cloning the repo and opening the scene directly is showing all GameObjects as invisible. If this is the  case for you I would recommend just saving the scripts and experimenting with them in your own Scene. **The only directory you need is Assets/Scripts. Everything else is just examples**
  
  # Usage #
  
  I have attempted to make this as easy as possible for users to get up and running. I'm going to eventually be putting together a formal piece of documentation. Until that is done, here's what I would recommend:
  
Add the Scripts directory into your scene. Then Attach Dialogue Manager to any character who will be available to converse with

JSON Files:
Please see RobotBoy.json for an example file for an NPC. In this example, the player talks to a Robot. Here is the file structure

	id: {
	        "text": "What you want to say",
		"condition" : "Met Before",
		"responseIds": [id1, id2],
		"alternateLine" : otherId,
	        "applyCondition" : ["Completed a quest", true]
	}

id should be a string and is how it is identified. It should be unique among all dialogues but it doesn't matter its name, so long as it is a string. I usually just name them "<characterName>1", "<characterName>2", and so on.

text: The subtitle

Condition: If the condition is in the .csv file attached to this script, it will say this. Otherwise it will jump to 
alternateLine and say that instead. This way you can make a chain of dialogue options until you fall back on a specific one.

responseIds: The player has their own similar file. They will then be prompted to choose one of these IDs in their file to speak

applyCondition: This adds the specified condition as a value in the CSV file. True for the global CSV file, false otherwise

There are 2 CSV files which you need to attach in scene builder: A global and local one. If an option will only effect one character specifically, store it in local CSV. This is to avoid namespace pollution. For example, a condition "Has Met" would go in local. Global is for worlwide events. For example, if Player completed a big quest that everybody will want to talk about, you would add "Completed something" to the global CSV.

See RobotBoyResponses.json for an example of a player response file. Option is the field that will be passed to the GUI and text will be the subtitle displayed after choosing option.

In the Editor under the Dialogue Manager Script you will see Dialogue IDs and Dialogue Recordings properties. These are a map for each ID and the corresponding audio file that will play when that ID is spoken. Import the audio into your project and drag it into the same index as the corresponding ID.
