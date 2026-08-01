```python
setting: "Abandoned Factory"
---
You: So basically it begins with me being down on my luck, looking for a place to sleep.

You: I encountered an Abandoned Factory while roaming through the docks.

You: So I uhh, found a way to sneak in...
// Shows the player breaking the window
<<nextFrame("window_break")>>
<<soundEffect("window_break")>>

You: Inside I found a dead body so I heroically got to work looking for clues.
// Shows the player feinting and passing out
<<soundEffect("collapsing")>>

You: And then you know the boring detective stuff happened.
// You (sleep walking) examining clues shows a snapshot of the crime scene.
// I think UI elements can do most of the explaining for the game mechanics, something along the lines of: "Cause of Sleep: 'Feinting'"

-> Select the ???
-> Select the ???
-> Select the ???

You: The factory doors slam open...
<<nextFrame("sleep_bubble_pops & eyes_open")>>
<<soundEffect("door_slam & sleep_interrupted")>> //I think we should have a consistent motiff for when sleep gets interrupted so the player gets used to that sound effect

You: Then six... maybe, eight... cops barged in. I tell them about all my amazing detective work I did and you know what they say?

Cop: The only way you could know the crime scene in that much detail is if you were involved in the crime or you're some sort of super genius
<<soundEffect("chuckling")>>

setting: "Baltimore Police Department's Breakroom, You're speaking with a detective who asked about how you ended up here"

You: So that's the story of how I got hired on as the super genius consultant, you know, after they cleared me from that case which took a week of being locked up.

Gray: Boy that's something alright, so that's how our department ended up getting stuck with you? Lucky us. //sarcastic

You: You know, I sense some sarcasm but I know you'll come around.
```

