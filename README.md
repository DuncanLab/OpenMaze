<h1 align="center">OpenMaze</h1>
<h1 align="center">visit our website: https://openmaze.duncanlab.org </h1>
<p align="center"><img src="https://i.imgur.com/M1Ep92b.png"></p>
<p align="center">A simulation of the <a href="https://en.wikipedia.org/wiki/Morris_water_navigation_task">Morris Maze experiment</a> created using the OpenMaze Toolbox. 

<p align="center">
<a href="LICENSE"><img src="https://img.shields.io/github/license/DuncanLab/OpenMaze.svg"></a>
<a href="https://github.com/DuncanLab/OpenMaze/releases"><img src="https://img.shields.io/github/release/DuncanLab/OpenMaze.svg"></a>

## Getting Started

### IMPORTANT
If you are following along with the OpenMaze tutorials there may be discrepencies between instructions in the tutorial videos and most recent version of the OpenMaze software. Please make sure that you carefully read over the **Release History / Change Log** below to see how these changes impact the tutorial videos. 

### Prerequisites
- Unity 2019.x or above.
- Your PC must meet the minimum requirements to run Unity, found [here](https://unity3d.com/unity/system-requirements).
- OpenMaze has been extensively tested on Mac OSX and Windows 10; however, the project should still behave normally on 
  most Linux distros.

### Installation
To use this software in your experiments you will need to download the
following software packages: **Unity, Open Maze, Sublime Text/Text editor of choice**.

1. Download and Install the Unity Hub
   1. Go to: https://store.unity.com/download?ref=personal.
   2. Follow Installation instructions for your specific computer.
   3. Open the Unity Hub application and create an account or login.

2. Download Open Maze Software
   1. Go to https://github.com/DuncanLab/OpenMaze (you're already here!).
   2. In the grey dropdown on the left side of the page called 'branch' make sure
      that **master** is selected. Alternatively, checkout the [releases section](https://github.com/DuncanLab/OpenMaze/releases)
      for the latest release. 
   3. Now, click the green button on the right hand side called **Clone or
      Download** and make sure to select **Download ZIP**.
   4. Unzip the file you downloaded in an easy to access place. For example, your
      desktop.

3. Download and Install Sublime Text:

   > Please note, if you already have a text editor on your computer that is
   > compatible with .json and .py files you can choose to skip this step and use
   > the text editor you are most comfortable with.

   1. Go to https://www.sublimetext.com/3
   2. Follow the instruction and select the correct download for your computer.

4. Set up Unity with Open Maze
   1. Open the Unity Hub application.
   2. If you have not done so already, please log in or create a free account.
   3. Once signed in, click **Open** near the top right of the window.
   4. Navigate to and select the entire folder that you just unzipped and placed
      in an easy to access place. This folder should be titled
      **OpenMaze-Release**. Select the folder and click **Open**.
   5. Once the OpenMaze project has been loaded into the Unity Hub click the small 
      yellow triangle below the project. This will create a propmt at the bottom
      of the screen to install the appropriate Unity version. Click install (NOTE:
      installing Unity takes longer than you may expect - be patient!)
   6. Once installed click the back arrow in the top left of the Unity hub window.
      You will now see the Unity Version code has been added beside the OpenMaze
      project and the yellow triangle no longer appears. 
   7. Click the project to Open it in Unity. If prompted to update Unity to the 
      newest version click no. 

Congratulations! You have set up the Open Maze project. To start building your OpenMaze experiment, visit the OpenMaze website an check out our User's Manual and tutorial videos  
https://duncanlab.github.io/OpenMazeSite

## Release History / Change log
- 1.0.1
  >**new features added** 
    1. Start button - a "Start Experiment" button has been added to the ***+Launch Experiment*** Scene. 
    2. Exit Experiment Button - ***Exit Experiment*** button can be added instruction trials by adding the attribute value pair:                **"ExitButton": true**. When this is added a button will appear at the bottom of the instruction trial and when pressed the              application will close. Note: Data will be automatically saved. 
    3. Default Configuration File - the folder ***Default_Config*** has been added to the ***StreamingAssets*** folder. If there is a          configuration file within this folder, the experiment will automatically load this configuration file instead of prompting the          experimenter/participant to select a configuration file through the file browser. Only one configuration file should be added to        this folder. 
  >**Changes to Download & Setup**
    1. Unity has recently deprecated the ***Standard Assets*** package. Because of this we have changed the release so that it already 
    includes all the standard assets needed. There is no longer a need to download any additional asset packs from the Unity asset
    store after launching OpenMaze. 
    
  >**Tutorial Videos Discrepencies**
    1. Tutorial 1 - Download, Install and Setup - OpenMaze no longer requires you to download **Nature Starter Kit 2** or
    **StandardAssets** disregard these instructions in the tutorial video.
    2. Tutorial 2: Building 3D Environment - Textures used when creating terrains differ from the textures now available with OpenMaze.
    Simple choose the textures that closely resemble those in the tutorial, or pick your own from the new textures included.
    3. All Tutorials - Start button is now used to start the experiment rather than the space bar. 

- 1.0.0
This is the first official release version of OpenMaze Experiment Toolbox for the Unity Engine. This version was released with the publication "OpenMaze: An Open Source Unity Toolbox for Creating Virtual Environment Experiments" and was used to create the "Learn OpenMaze in a Day" video tutorial series (https://www.youtube.com/playlist?list=PLppXGUtW-XlIuTh-lW6URgm5Cim_DH4gL). For more information about the OpenMaze Toolbox please visit https://openmaze.organica.dev/

- 0.1.0
Beta Testing Release
    

## Contributing

Please read [Contributing](https://github.com/DuncanLab/OpenMaze/wiki/Contributing) for details on the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [releases on this repository](https://github.com/DuncanLab/OpenMaze/releases).

## Authors

### Current project maintainers: 

Please feel free to reach out to current project maintainers for help working with OpenMaze.

* **Kyle Nealy** - *Project Owner* - [GitHub](https://github.com/kbnealy)
* **Alex Gordienko** - *Contributor* - [GitHub](https://github.com/AlexGordienko)
* **Cody Howarth** - *Contributor* - [GitHub](https://github.com/codyhowarth)

See also the list of [contributors](https://github.com/DuncanLab/OpenMaze/wiki/Contributors) who participated in this project.

## License

This project is licensed under the MIT License.


## Acknowledgments

- File browser used: https://github.com/gkngkc/UnityStandaloneFileBrowser
