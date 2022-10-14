![logo-500](https://user-images.githubusercontent.com/100143610/193881093-a45bb6d2-acd9-439b-996d-f64ee961fddb.png)

Stream Scoreboard is a desktop application equipped with the standard functions of a general scoreboard for OBS and fighting games with some aditional traits: the ability to input keyboard shortcuts directly into OBS itself, searching and connecting to start.gg tournaments, making looking for tournaments and bracket matches information easier

First of all we have the main screen

![1](https://user-images.githubusercontent.com/100143610/195660544-c4aee492-dd1e-487f-82a9-4f3db9eec4af.png)

Here we could opt to use the manual interface where we input all the information, yes, manually, including player nicknames, rounds or scores 

![2](https://user-images.githubusercontent.com/100143610/195660574-677f3f8f-95ad-487c-8a81-e3958a58699f.png)

Did you see those tabs up there? We'll talk about them later. Let's have a look at connecting to start.gg but first we'll need an authentication token to do so. You can generate it following this guide: https://developer.start.gg/docs/authentication 

Once you have it you must set it in the settings screen. Here you also can set your output directory for the OBS text files and the application theme

![3](https://user-images.githubusercontent.com/100143610/195660616-433c63d9-8991-4eee-baec-ec87400e3a72.png)

Once we search the name of the tournament we can select it from a list. It will also be saved on a recent tournaments list for easier access 

![4](https://user-images.githubusercontent.com/100143610/195664056-b154434d-c6f1-41ab-9dc1-d94cf5cc6b2c.png)

If a tournament is already over it won't be able for selection and the application will raise an alert (like Genesis 8, for instance)

![4](https://user-images.githubusercontent.com/100143610/194766065-10c68471-adf9-4b73-988c-0d3e18cf95ce.png)

Lightening start.gg API requests size is crucial to minimize errors, that's why we're getting information about the tournament in small bits. Selecting field values will unlock more fields to select until we can check the sets field. Here we can select a set and it will complete the information just as if we did it manually

![5](https://user-images.githubusercontent.com/100143610/195884917-2b975614-e55b-4aa8-a328-cbf842f3866c.png)

![6](https://user-images.githubusercontent.com/100143610/195884933-46c44725-61de-4cab-81d5-ce59f9a0b899.png)

Note: if a set is complete or invalid in some form (say it's missing a player because they're still playing the previous set or the bracket hasn't been updated) it won't be able for selection. That's why there's a refresh button right next to the set selection, it can be clicked and it will update all the sets available

Pretty straightforward right? Let's get to the tabs part. First we have the "Extra" tab where we can enter caster names and some miscellaneous information or message to be displayed on screen

![7](https://user-images.githubusercontent.com/100143610/195661050-54ed18fe-3b4a-4c06-acd0-0534530b1b0f.png)

And finally there's the "OBS Tools" tab. Here we can add, edit and remove keyboard shortcuts (using key sequences) and input them directly into OBS so we don't have to remember every single one of them. This feature uses the websocket plugin for OBS so make sure it's installed beforehand (already included with OBS v28.0.0+)

![8](https://user-images.githubusercontent.com/100143610/195661084-9585f9ec-8c66-421f-a9ee-a97b95e042f8.png)

![9](https://user-images.githubusercontent.com/100143610/195661095-3b77cf46-ec34-47f3-af0b-795f93660859.png)
