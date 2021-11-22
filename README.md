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

There are 18 different decoration groups, consisting of a type of props.
There are 7 different decoration sets, consisting of a gradient (for coloring the terrain) and as many differerent decoration groups as the developer wants.
There are 3 types of bumpiness for the terrain, 7 different skybox materials, and 2520 different names for the randomly generated places.

The random generation of a place depends on the y coordinate of the list-element it's coupled with.
