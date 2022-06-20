Stream Scoreboard is a desktop application equipped with the standard functions of a general scoreboard for OBS and fighting games but adding the ability to input keyboard shortcuts directly into OBS itself and connecting to start.gg tournaments, making looking for bracket matches information easier.

First of all we have the main screen

![1](https://user-images.githubusercontent.com/100143610/172506605-e1c4c398-54d0-4be9-9972-12e4966717eb.png)

Here we could opt to use the manual interface where we input all the information, yes, manually, including player nicknames, rounds or scores 

![2](https://user-images.githubusercontent.com/100143610/172506642-be169449-3913-456f-9c7c-38d2a36b93de.png)

Did you see those tabs up there? We'll talk about them later. First let's have a look at connecting to start.gg. We're using Genesis 8 as an example. Once we input the tournament url or slug (ex: https://start.gg/tournament/this-is-the-slug) it will be saved on recent tournaments for easier access 

![3](https://user-images.githubusercontent.com/100143610/172506838-256b84fe-cc00-4cfb-b9f1-30b263f5366c.png)

Lightening start.gg API requests size is crucial to minimize errors, that's why we're getting information about the tournament in small bits

![4](https://user-images.githubusercontent.com/100143610/172507363-6dcabe61-ed10-4a46-b1d8-d2486c0ec38b.png)

Selecting field values will unlock more fields to select until we can check the sets field. Here we can select a set and it will complete the information just as if we did it manually. 

Note: if a set is complete or invalid in some form (say it's missing a player because they're still playing the previous set or the bracket hasn't been updated) it won't be able for selection. At this time Genesis 8 is already over so that's why we have no options elegible. Let's use a test tournament to see the difference

![5](https://user-images.githubusercontent.com/100143610/172507810-c7630295-858c-441f-9928-3abac04cb974.png)

Pretty straightforward right? Let's get to the tabs part. First we have the "Extra" tab where we can enter caster names and some miscellaneous information or message to be displayed on screen

![6](https://user-images.githubusercontent.com/100143610/172508002-da75e77d-926b-466e-b3d5-89b7819f8c95.png)

Right after that it's the "OBS Tools" tab. Here we can save keyboard shortcuts and input them directly into OBS so we don't have to remember every single one of them. This feature uses the websocket plugin for OBS so make sure it's installed beforehand

![7](https://user-images.githubusercontent.com/100143610/172508655-ba40fc8a-5d8e-4577-9518-89a8042ecdf5.png)

The final tab "Settings" is pretty intuitive. The resources folder directory can be changed to use .png files to show the score instead of text files, but it's rarely used. The output directory should point to the folder containing the text files OBS is using to show names, scores and such

![8](https://user-images.githubusercontent.com/100143610/172509066-d92ea3bc-035d-4449-9926-efa834cc29c5.png)

And that's all. To be honest this is not the latest version I made. That one was a bit more refined and detailed covering some errors and removing unnecessary stuff but there were problems with repositories and files and I got to rescue this one. I dont' have much time right now to redo all that stuff so feel free to change, improve or do whatever you want with anything in the code
