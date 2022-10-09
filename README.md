![logo-500](https://user-images.githubusercontent.com/100143610/193881093-a45bb6d2-acd9-439b-996d-f64ee961fddb.png)

Stream Scoreboard is a desktop application equipped with the standard functions of a general scoreboard for OBS and fighting games with some aditional traits: the ability to input keyboard shortcuts directly into OBS itself, searching and connecting to start.gg tournaments, making looking for tournaments and bracket matches information easier

First of all we have the main screen

![screen1](https://user-images.githubusercontent.com/100143610/193868992-71f1a693-d868-4669-a9cb-d4308c70c462.png)

Here we could opt to use the manual interface where we input all the information, yes, manually, including player nicknames, rounds or scores 

![2](https://user-images.githubusercontent.com/100143610/194766053-08c8432f-4ccf-4348-ac6d-e26f0d7387c4.png)

Did you see those tabs up there? We'll talk about them later. Let's have a look at connecting to start.gg but first we'll need an authentication token to do so. You can generate it following this guide: https://developer.start.gg/docs/authentication 

Once you have it you must set it in the settings screen. Here you also can set your output directory for the OBS text files and the application theme

![2](https://user-images.githubusercontent.com/100143610/194765778-616af39f-c026-4782-aeb4-bd106dad6eb0.png)

We're using The Big House 10 as an example. Once we search the name of the tournament we can select it from a list. It will also be saved on a recent tournaments list for easier access 

![3](https://user-images.githubusercontent.com/100143610/194765886-007cf684-ceb8-4923-b5bf-9f2edc59ff41.png)

If a tournament is already over it won't be able for selection and the application will raise an alert (like Genesis 8, for instance)

![4](https://user-images.githubusercontent.com/100143610/194766065-10c68471-adf9-4b73-988c-0d3e18cf95ce.png)

Lightening start.gg API requests size is crucial to minimize errors, that's why we're getting information about the tournament in small bits. Selecting field values will unlock more fields to select until we can check the sets field. Here we can select a set and it will complete the information just as if we did it manually

![5](https://user-images.githubusercontent.com/100143610/194766156-f0f5babf-c9f0-48d3-933f-dea97e857dbc.png)

![6](https://user-images.githubusercontent.com/100143610/194766160-cd3007cf-d852-47e5-8b80-af0aa20c6847.png)

Note: if a set is complete or invalid in some form (say it's missing a player because they're still playing the previous set or the bracket hasn't been updated) it won't be able for selection. That's why there's a refresh button right next to the set selection, it can be clicked and it will update all the sets available

Pretty straightforward right? Let's get to the tabs part. First we have the "Extra" tab where we can enter caster names and some miscellaneous information or message to be displayed on screen

![7](https://user-images.githubusercontent.com/100143610/194766338-72153e3f-4523-4b73-a9fb-346154e04740.png)

And finally there's the "OBS Tools" tab. Here we can add, edit and remove keyboard shortcuts (using key sequences) and input them directly into OBS so we don't have to remember every single one of them. This feature uses the websocket plugin for OBS so make sure it's installed beforehand (already included with OBS v28.0.0+)

![8](https://user-images.githubusercontent.com/100143610/194766345-19d96e0c-0fdc-4250-a6f0-80bad7702916.png)

![9](https://user-images.githubusercontent.com/100143610/194766358-3704948e-12c9-41e4-9ce1-8bfac06c1760.png)
