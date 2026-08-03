# Timeline
1. What does the typical timeline look like 2/3 day and 1/3 night?
	1. PITCH #1
		1. There could technically be 6 segments, 4 day, 2 night.
		2. Typical sleeping drugs can make this 3 day, 3 night and extreme ones could make it 2 day, 4 night.
2. How does that connect to the storyline
	1. To allow the player to be able to use their first 1/3 of day however they like (sleep through it via pills or play naturally while being awake) I think most important dialogue should happen in 2/3, I think a typical flow could look like (1/3 go to office, work on paperwork, talk amongst coworkers 2/3 get developments on a case, process evidence, talk amongst coworkers, 3/3 have night you do its thing)
		1. Some 3/3 (if awake) can be replaced with talking to the night janitor who knows a lot about your sleep walking issues but has stayed quiet about it
# Sleep/Awake States
1. Sleep
	1. Cycle
		1. Core
		2. Deep
		3. Core
		4. REM
	2. State Details
		1. Deep
			1. Minimal to no night terrors
			2. Stumbling into walls or tripping over debris will not wake you up just slow you down
			3. Doing work during a busy crime scene or lab will not have any additional QTEs from chattering cops
			4. Cannot jump
			5. Dark Purple (two-tone)
		2. Core
			1. More night terrors
			2. Stumbling into walls or tripping over debris will not wake you up just slow you down
			3. Doing work during a busy crime scene or lab will have the occasional cop chattering appear as a QTE
			4. Purple (two-tone)
		3. REM
			1. Even more night terrors
			2. Stumbling into walls or tripping over debris will wake you up
			3. Doing work during a busy crime scene or lab will often have cop chattering appear as a QTE
			4. Light Purple (two-tone)
	3. QTEs
		1. Night Terrors
			1. Spiky
				1. **Appearance & Behavior**: Shadowy figure that lingers in the corners of your screen, appears to have porcupine-like spikes protruding upwards from their body, long slender hands reach towards your work to try and undo it or stop you from interacting with whatever you're working with (tools, machines, etc.)
				2. **Counter Play**: Moving your mouse "slices" through the shadow getting four quick slashes will make it vanish.
			2. Teeny
				1. **Appearance & Behavior**: Little ball-like guy with 7 tentacle-like legs, sometimes balls up and rolls around your work station to cling onto evidence or tools. While rolling can also bump into your tools/evidence to move it around or into your mouse to shove it aside.
				2. **Counter Play**: Clicking on the guy two times will make it shrink then vanish once becoming a tiny spec.
			3. Smoky
				1. **Appearance & Behavior**: Appears taking up the edges with a cigar sticking out and smoking pouring out and covering up the screen top-down, evidence or tools fully covered in smoke clouds can't be interacted with
				2. **Counter Play**: You can click on smoke cloud to make them vanish and drag the cigar out of Smoky's mouth to make them vanish while chasing after it.
		2. Cops Chattering
			1. Thought Bubble
				1. Simply hover over this one and it'll pop and go away
			2. Speech Bubble
				1. Drag this and fling it towards the edges/corners, when it flings far away enough it'll despawn, pushing too lightly will have it gradually float back into place. When it's out of screen (despawned or not) it will not be actively effecting you that way an object you can't interact with or see isn't continuously threatening your sleep timer until it reappears.
			3. Exclamation Bubble
				1. Clicking will cause all the area near your mouse to vanish, you need to click in four-five distinct/separate parts to make at least ~80% of the bubble vanish before it all despawns as a whole
2. ~~Sleep Enablers~~
	1. ~~After getting knocked out by Gray in Motel Murders you're able to sleep by pressing Z~~
		1. ~~You can sleep after three actions for $T$ seconds???~~
		2. ~~You can sleep after four actions for $T+16$ seconds???~~
	2. ~~Natural~~
	3. ~~Chamomile Tea~~
		1. ~~No night terrors~~
	4. ~~Melatonin~~
		1. ~~Sleep gives you $+32$ seconds~~
# Game Loop
1. Solving a Case
	1. Crime gets committed
		1. You get told about it briefly
	2. You appear at the crime scene
	3. Your captain tells you the details of what they know
	4. You get either time event or like tap-out event where you have to move around a room or rooms to find clues
		1. You enter minigames to retain the quality of the evidence depending on your performance
			1. Lifting Prints
				1. Assets
					1. Dust Brush
					2. Fingerprint Lifting Tape
				2. Crime Scene
					1. <u>SLEEP WALK BONUS</u>: No requirements to being slow/steady with mouse movements. See subpoints below
					2. Pick up the brush then <u>gently wiggle</u> your mouse over a surface. Over-dusting will smudge the prints. This can be a timer/tap-out system where you need to properly brush within a small period of time. Don't rush or take too much time
						1. You can rapidly move your mouse and still finish with excellence
					3. Pull Lifting tape <u>evenly</u> (moving mouse away) then tear it gently (moving mouse sideways without jittering up/down)
						1. So long as your mouse mostly moves upwards then mostly sideways the task will finish with excellence
					4. Apply the lifting tape over by dragging it on top of the print then clicking (You're aiming to get 100% of the tape within the tape)
					5. Move your mouse <u>steadidly</u> along the tape (to remove air bubbles)
						1. The only requirement is that you move along the tape, a consistent movement rate is no longer required
					6. Grab at a corner/edge then slowly peel away, slipping your grip means you just need to move your mouse back to continue peeling it.
						1. No need to slowly peel away you can yank it out
				3. Lab
					1. <u>SLEEP WALK BONUS</u>: The minutiae stand out more (outline/highlight with dithering effect)
					2. Click to select the tape, move your mouse under a magnifier then click to release the tape.
					3. On the left you'll see a sheet of three to four minutiae, find them from the lifted print and select them. (See: https://www.bayometric.com/wp-content/uploads/2016/10/common-minutiae-patterns.jpg)
						1. These will have something to help them stand out such as an outline/particles/highlight effect
			2. Biological Samples
				1. Assets
					1. Swab
					2. Saline Bottle
					3. Volumetric Pipette
					4. Microcentrifuge Tube
					5. Bottle of Luminol with built in Dropper Lid
				2. Crime Scene
					1. <u>SLEEP WALK BONUS</u>: No requirements to being slow/steady with mouse movements. See subpoints below
					2. Select the swab and dip it into the saline bottle only for a second
					3. Rest your swab ontop of the dried sample. IF you miss the target: throw the swab to the far right of the screen (trash icon) grab a new swab then start over, dirt/debris will mess up your sample.
					4. Rotate your mouse <u>steadily</u> $360^\circ$ for 2 full rotations (this ROLLS the swab not spirals it)
						1. You needn't be steady, just quickly rotate in 2 full rotations
					5. Place your swab in the center of a microcentrifuge tube
				3. Lab
					1. <u>SLEEP WALK BONUS</u>: No requirements to being slow/steady with mouse movements. See subpoints below
					2. Use a pipette to draw fluid <u>steadily</u> (moving your mouse upwards slowly) from the Microcentrifuge Tube
						1. No need to be steady move your mouse quickly
					3. Try to <u>evenly disperse</u> the fluid among two-four (randomly selected) Microcentrifuge Tube
						1. A dashed line appears showing where all the tubes can be filled up until
					4. Drag and drop them into a centrifuge wheel <u>symmetrically</u>
						1. After placing the first the other possible choices are highlighted
					5. Set the correct timer <u>based off a datasheet</u> that corresponds # of tubes to position of the dial
						1. The position to move the dial in is already marked with a dot
			3. Trace Fibers
				1. Assets
					1. Magnifying glass
					2. Tweezers
					3. Microcentrifuge Tube
					4. Bottle of Luminol with built in Dropper Lid
					5. Glass Slides
				2. Crime Scene
					1. <u>SLEEP WALK BONUS</u>
					2. Use a Magnifying glass over a background to reveal hair or other fibers that are otherwise invisible. When at the right position the magnifying glass freezes in place
					3. Grab tweezers and carefully grab the fiber then <u>gently</u> pull away
						1. Pull at any rate
					4. Center the tweezers over a Microcentrifuge Tube to put the trace fibers into
				3. Lab
					1. Grab the fiber via tweezers and place it on top of a glass slide (glass slide is at an angle ($\sim45^\circ$) not top-down)
					2. Take a dropper from the Bottle of Luminol move it over the glass slide and click twice while hovering over the center of the slide
					3. Grab a glass slide, line it up above the current one and drop it into place (click when it's stable)
					4. Turn on a heat lamp and pull out a datasheet, correspond the datasheet to the reaction the trace has
						1. Straightens Out: Useless Trace
						2. Coils Up: Someone's Hair
			4. Ballistics/Impact
				1. Assets
					1. Line up a camera
					2. Take a photo
				2. Lab
						1. Hand the photos to Denis Bradshaw
	5. After gathering all the details of a case your sleep-walking variant is able to scribble down what they think happened in your notepad
		1. This allows you to submit a full report and theory to Bolton
	6. Bolton 