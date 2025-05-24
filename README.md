# MadBox_Test

● How you approached this test: what were your different game making phases?

The first step was to open up the project and see what assets were available. 
After this I created a Trello board and listed everything that would be needed / that I would want to do. 
Next thing was to roughly estimate the time of each task and then orders them between core feature and nice to have ones. 
I then spend some extra time to think about how I needed to code everything and how I would organise my code. 
Once all of this was done, I started developping.

● The time you spent on each phase of the exercise

I spent a total of 5 days working on the project.

Evening Day 0 : 
	-Controller
	-Player 
	-Basic enemy movements

Day 1 : 
	-Player animations
	-Global enemy managment
	-Paterns of Basic enemy
	-Rework Enemy movement to use NavMesh
	-Wave setup managment and basic pulling
	
Day 2 :
	-Refacto global des stats (SO_Stats / SO_Weapons etc)
	-Detection system
	-Player attack
	-HP Bars
	-Debugging
	-Main Menu

Day 3 : 
	-Finishing Wave System (Multiple waves)
	-Enemy & Player Death (Pulling and triggers)
	-HUD + Wave progress bar + feedbacks
	-New UI Screen Inbetween waves
Day 4 :
	-Small tweaks
	-Loose Screen / Win Screen + restart
	-ScreenShake utils
	-HitStops
	-Particle systems
	-Debugging build
Day 5 :
	-Various polish on numerous elements
	-Adding some feedbacks for Clarity
	-Playtesting / Debugging

● The features that were difficult for you and why

The hardest features for me to implement were probably the Joystick and the particules flying to the progress bar. It needed a lot of coordinates conversion, and in UI, which is not my strong suit at all.

● The features you think you could do better and how

My pulling system is really basic and doesn't extend to anything except Enemy. I would have liked to pull in a cleaner way, and extend it to particles & FX aswell

● What you would do if you could go a step further on this game

Implementing different enemy with different patterns, different weapons, maybe projectile based ones. An epic boss fight at the end of each level. Maybe experiment with other type of controls too.

● Any comment you may have

I sadly didn't implement any SFX in the project even tho it would add a lot to the overall Game Feel. Searching on the internet for free SFX that match your needs is a very time consuming thing, and I wanted to priotise finishing the prototype, hence the abscence of SFX & Musics.

Some downloaded assets have been used. They are mostly visuals (Stored in the "Download" folder).
There is also 2 others assets downloaded and used in this project :
DoTWeen
PhysicsDebugExtension (RotaryHeart) : It's an extension that let you directly debug any type of Raycast/Spherecast etc. It just saves you the time and potential errors when using Debug.DrawRay.