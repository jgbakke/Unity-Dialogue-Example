# Unity-Dialogue-Example

Branching dialogue system for use with Unity Engine. Features a basic GUI for player to select from multiple options, a branching system where player dialogue options can lead to different NPC responses configured by the user, and a system to optionally track events so NPCs can respond to previous quests or player choices, allowing for dynamic dialogue. Ideal for text-heavy games such as RPGs

There are still changes planned for this so it is **not completely** ready for production yet.

**If you do not need the example project and only want the files to add to Game Objects:** (Recommended)
  Just clone the repo and then move the directories Assets/Scripts/Dialogue and Assets/Scripts/lib into your project.

If you **do** want to see the example project
  Clone the repo and open it
  Note: It has come to my attention that cloning the repo and attempting to run it in Unity is not working. If this is the  case for you I would recommend just saving the scripts and experimenting with them in your own Scene.
  
  Any questions or want to report a bug? Email bakkejor [at] msu.edu for any questions or open up an issue and I will get back to you.
  
  # Usage #
  
  I have attempted to make this as easy as possible for users to get up and running. I'm going to eventually be putting together a formal piece of documentation. Until that is done, here's what I would recommend:
  
Add the Scripts directory into your scene
Attach Dialogue Manager to any character who will be available to converse with

JSON Files:
Please see RobotBoy.json for an example file for an NPC. In this example, the player talks to a Robot. Here is the file structure

id: {
    "text": "What you want to say",
		"condition" : "Met Before",
		"responseIds": [id1, id2],
		"alternateLine" : otherId,
    "applyCondition" : ["Completed a quest", true]
}

id should be a string and is how it is identified

text: The subtitle

Condition: If the condition is in the .csv file attached to this script, it will say this. Otherwise it will jump to 
alternateLine and say that instead. This way you can make a chain of dialogue options until you fall back on a specific one.

responseIds: The player has their own similar file. They will then be prompted to choose one of their IDs to speak

applyCondition: This adds the specified condition as a value in the CSV file. True for the global CSV file, false otherwise

There are 2 CSV files which you need to attach in scene builder: A global and local one. If an option will only effect one character specifically, store it in local CSV. This is to avoid namespace pollution. For example, a condition "Has Met" would go in local. Global is for worlwide events. For example, if Player completed a big quest that everybody will want to talk about, you would add "Completed something" to the global CSV.

See RobotBoyResponses.json for an example of a player response file. Option is the field that will be passed to the GUI and text will be the subtitle displayed after choosing option. 
