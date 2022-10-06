![logo-500](https://user-images.githubusercontent.com/100143610/193881093-a45bb6d2-acd9-439b-996d-f64ee961fddb.png)

Stream Scoreboard is a desktop application equipped with the standard functions of a general scoreboard for OBS and fighting games but adding the ability to input keyboard shortcuts directly into OBS itself and connecting to start.gg tournaments, making looking for bracket matches information easier.

First of all we have the main screen

![screen1](https://user-images.githubusercontent.com/100143610/193868992-71f1a693-d868-4669-a9cb-d4308c70c462.png)

Here we could opt to use the manual interface where we input all the information, yes, manually, including player nicknames, rounds or scores 

![screen2](https://user-images.githubusercontent.com/100143610/193869099-eeeaf518-9222-4c91-86f3-cb9c1aaf6433.png)

Did you see those tabs up there? We'll talk about them later. First let's have a look at connecting to start.gg. We're using The Big House 10 as an example. Once we search the name of the tournament we can select it from a list. It will also be saved on a recent tournaments list for easier access 

![1](https://user-images.githubusercontent.com/100143610/194208732-22de75c6-60e9-42b6-ac97-1762867d3ce1.png)

If a tournament is already over it won't be able for selection and the application will raise an alert (like Genesis 8, for instance)

![2](https://user-images.githubusercontent.com/100143610/194208087-d3ff39b5-1775-4a3e-b308-82ed3e935284.png)

Lightening start.gg API requests size is crucial to minimize errors, that's why we're getting information about the tournament in small bits. Selecting field values will unlock more fields to select until we can check the sets field. Here we can select a set and it will complete the information just as if we did it manually

![screen4](https://user-images.githubusercontent.com/100143610/193870488-a0dfe7b6-8943-4adf-81ff-83dab3c5df26.png)

![setunlocked](https://user-images.githubusercontent.com/100143610/193874708-9a12c5b0-d9e8-4df8-89e0-f1e3fe0fbb61.png)

Note: if a set is complete or invalid in some form (say it's missing a player because they're still playing the previous set or the bracket hasn't been updated) it won't be able for selection. That's why there's a refresh button right next to the set selection, it can be clicked and it will update all the sets available

Pretty straightforward right? Let's get to the tabs part. First we have the "Extra" tab where we can enter caster names and some miscellaneous information or message to be displayed on screen

![casters](https://user-images.githubusercontent.com/100143610/194168203-0e6b1209-8d20-45b3-82a1-8fc1dfc62164.png)

Right after that it's the "OBS Tools" tab. Here we can save keyboard shortcuts (using key sequences) and input them directly into OBS so we don't have to remember every single one of them. This feature uses the websocket plugin for OBS so make sure it's installed beforehand (already included with OBS v28.0.0+)

![socket1](https://user-images.githubusercontent.com/100143610/194168221-ffda28c6-0798-4c6e-94fa-8d11a568ea47.png)

![socket2](https://user-images.githubusercontent.com/100143610/194168229-a847e2f8-7ae6-47a4-8eb3-c41794d4f442.png)

The final tab "Settings" is pretty intuitive. Here you can change the application theme and choose your output directory of preference. It should point to the folder containing the text files OBS is using to show names, scores and such

![3](https://user-images.githubusercontent.com/100143610/194208391-384494d9-9e3e-4af0-b079-4d322d9876be.png)

![4](https://user-images.githubusercontent.com/100143610/194208392-aca1498e-93e1-4a3c-9a28-b5d21709d1bf.png)
