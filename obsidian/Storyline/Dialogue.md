# Motel Murders
```yarn
title: motelStart
//setting: motel, first murder
---
Bolton: So Gibbs, what do you see?

You: Uhhm, a sink...? Dead woman... other bathroom stuff?

Gray: Riveting stuff.

Bolton: Gibbs this isn't the time to be joking around!

> PRESS Z <
// doesn't need to be "dialogue" it can be something popping up covering the screen like a giant Z button
```

```yarn
title: motelTalkToGray
//setting: motel, gibbs asleep
---
You: ...

Gray: Coroner said this looks like the work of poison.

You: ...

Gray: Okay you're making me uncomfortable now.
===
```

```yarn
title: motelTalkToBolton
//setting: motel, gibbs asleep
---
Bolton: Anything to report?

You: ...

Bolton: Hello?
===
```
## EVIDENCE COLLECTION
```yarn
title: motelInteractFrontDoor
//setting: motel, gibbs asleep
---
You: No signs of a break-in, was the killer let in willingly?
===
title: motelInteractDroppedNail
---
You: This looks like her acrylic nail, it broke off while... *looking around*

You: Clawing at the door? Those scratches haven't collected dust yet, I should take a photo for forensics.

<<collectEvidence("MotelScratches")>>
===
title: motelInteractDeadWoman
---
You: She's curled up - seems like poison.

You: Half a face of makeup, was she applying makeup before being poisoned...?
===
title: motelInteractHairbrush
---
You: Hairbrush is on the ground, she must've been holding it or reached for it while collapsing to the ground?
===
```
## BACK TO BEING AWAKE

```yarn
title: motelWakeUp
//setting: motel, gibbs awake
---
You: He moved my notepad from my left to right pocket.

You: That means he has updates to noftify me of.

<<openNotepad()>>
===
```
### Didn't collect all evidence
```yarn
title: motelTalkToGray
//setting: motel, gibbs asleep
---
You: I got some evidence here.

Gray: Doubt it's anything I didn't gather myself.

You: Yeah... maybe.
===
```

```yarn
title: motelTalkToBolton
//setting: motel, gibbs asleep
---

Bolton: Anything to report?

You: Yes, I got some evidence

Bolton: Some? Do you know what happened here?

You: I'm not sure yet.
===
```
### Collected all evidence
```yarn
title: motelTalkToGray
//setting: motel, gibbs asleep
---

You: I gomt some evidence here.

Gray: Doubt it's anything I didn't gather myself.

You: Heh, doubt it. I think i've just about cracked this case.

Gray: We'll see about that.
===
```

```yarn
title: motelTalkToBolton
//setting: motel, gibbs asleep
---

Bolton: Anything to report?

You: Yes, I think I gathered all the evidence I need.

Bolton: Good work, ready to head back to the department?

-> Yes.
<<transitionTo("policeDepartment")>>
-> Hold that thought.
Bolton: Let me know when you're ready to head back.

You: Okay.
===
```

---
# Police Department
```yarn
title: postMotelBreakroom
//setting: breakroom with marty & gray
---
Marty: My sister told me about your work at the motel.

You: Oh yeah? All good I hope.

Gray: Don't count on it.

Marty: How do you do it?

You: Just... lot of focus.

Marty: So cool, I heard Bradshaw is processing your evidence right now.

You: Maybe I should pay him a visit.

Gray: You know Gibbs, detective I never saw but lab geek? It suits you.
```

```yarn
title: postMotelBreakroomTalkToMarty
//setting: breakroom with marty & gray
---
Marty: Hey.

You: Hello.

Marty: I hope to see you work someday you know?

You: Yeah, you probably will.
```

```yarn
title: postMotelBreakroomTalkToGray
//setting: breakroom with marty & gray
---
You: You really think I could do lab geek stuff?

Gray: You heard a compliment in all of that?

You: Yeah?

Gray: Jeez, I was meaning to call you a dork Gibbs.
===
```

```yarn
title: postMotelLabTalkToBradshaw
//setting: forensics lab with bradshaw
---
You: Hey how's the evidence processing going?

Bradshaw: Fine.

You: I was thinking I could maybe help you?

Bradshaw: Seriously? I mean I guess you can do some of the simple stuff.

You: If it helps catch the killer sooner, why not?

Bradshaw: I'll teach ya' in exchange for some coffee.
===

//NEED SOMETHING TO DETECT REPEAT DIALOGUE

title: postMotelLabTalkToBradshawLoop
//setting: forensics lab with bradshaw (looped)
---
Bradshaw: Did you get the coffee?

-> No. #default:7
Bradshaw: Well get on that.
=> Here it is. //(only if they have)
Bradshaw: You know Gibbs I don't see being barista in your future.

You: What?

Bradshaw: This coffee is terrible, like a... 8.1 out of 10.

You: 8.1 is terrible?

Bradshaw: For coffee? Yes.
===
```

```yarn
title: postMotelBreakroomCoffeeMaker
//setting: breakroom with marty & gray after talking to Bradshaw
---
You: Alright, time to figure this thing out.
<<brewCoffee()>> //love this to be a minigame but probs no time.
===
```
---
# Greenroom