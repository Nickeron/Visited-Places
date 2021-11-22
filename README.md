# Visited Places
Project of random worlds

In this project we show a list of randomly generated worlds live-rendered along with their description.

![screenshot of the app](https://github.com/Nickeron/Visited-Places/blob/main/Release/Screenshot.png?raw=true)

A total of three (3) elements in the list are recycled for emulating the effect of an endlessly scrollable list.
Each element is connected with a lively-rendered mesh. Each element's renderTexture is set on the output of the camera of each place.
When the user scrolls and an item on the list is recycled, the place connected to that item is redecorated with new props and its mesh is redrawn, with different parameters.

For gaining performance, the meshes of the places are not populated entirely with props.
They are populated with a large amount of GameObjects (Spawners) that consist only of a sphere collider and a script.
Those GameObjects act as triggers and when they collide with the camera's boundary meshes they request a prop at their position.
The props that the spawners request, are handled by Unity's ObjectPool system, so that they are recycled too.

![screenshot of the app](https://github.com/Nickeron/Visited-Places/blob/main/Release/Spawner%20System.gif?raw=true)
