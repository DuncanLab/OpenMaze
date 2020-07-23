using System;
using System.Collections.Generic;
using UnityEngine;

namespace data
{
    [Serializable]
    public class Data
    {
        public long ExperimentStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        // File locations
        public string SpritesPath = Application.streamingAssetsPath + "/2D_Objects/";
        public string PythonScriptsPath = Application.streamingAssetsPath + "/PythonScripts/";
        public string AutoRunConfigPath = Application.streamingAssetsPath + "/AutoRun_Config/";

        // whether or not to turn on timing diagnostics
        public bool TimingVerification;

        // value from which trials start incrementing in the config file. 
        public int TrialInitialValue = 1;

        // The amount of delay (in seconds) before a trial loads in. This value is used to 
        // ensure a consitent loading time between trials.
        public float TrialLoadingDelay = 3.0f;

        // The amount of time at the beginning of a trial when user input is ignore. This value
        // is used to avoid the user accidentally ending the trial by pressing the key pre-maturely.
        public float IgnoreUserInputDelay = 1.0f;

        // how often data is outputted. 
        public int OutputTimesPerSecond;

        //This is the object (defined below) which contains all data available for the main player
        public Character CharacterData;

        // The output file of the character's movements during an experiment
        public string OutputFile;

        //This is a LIST of the different types of pickup items that are defined below
        public List<Goal> Goals;

        // Runtime objects to load into the trials
        public List<LandMark> Landmarks;

        // Contains all pre-defined trials
        public List<Trial> Trials;

        // Contains all Enclosure config objects
        public List<Enclosure> Enclosures;

        public List<BlockData> Blocks;

        public List<int> BlockOrder;

        [Serializable]
        public class BlockData
        {
            public string BlockGoal; // percentage ___SPACE___ number. This is very arbitrary.
            public string BlockFunction; // The function name (if not present, we assume its always true)
            public string TrialGoal; // percentage ___SPACE___ number. This is very arbitrary.
            public string TrialFunction; // The function name (if not present, we assume its always false)

            public string BlockName; // Name (outputed at the end of the Block)
            public string Notes; // Notes about the given block
            public int Replacement; //Integer value representing replacement
            public List<RandomData> RandomlySelect; //Array that contains all the possible random values
            public List<int> TrialOrder; //Trial order (-1 means random)

            public bool ShowNumSuccesses; // Whether or not to display the number of successful trials
            public bool ShowBlockTotal; // Whether or not to display the amount of goals/pickups collected (resets each block)
            public bool ShowTrialTotal; // Whether or not to display the amount of goals/pickups collected (resets each trial)
            public string DisplayText; // Text appears at the bottom of every 3D trial in the block.

        }

        [Serializable]
        public class RandomData
        {
            public List<int> Order;
        }

        [Serializable]
        public class Trial
        {
            public int Rotate; // How long the delay can last in the rotation
            public int TwoDimensional; // Set to 1 iff trial is two dimensional
            public int Instructional; // Set to 1 iff trial is instructional
            public string FileLocation; // Is not null iff FileLocation exists (Image for 1D trials)
            public int Scene; // This is the environment type referenced.
            public int TrialTime; // Allotted amount of time
            public string TrialEndKey; // The key press which will end the current trial.
            public string DisplayText; // Note outputted out of the trial.
            public string DisplayImage; // Is not null iff FileLocation exists (Image for 1D trials)
            public EnclosureData Map; // The Map saved EnclosureData
            public List<int> InvisibleGoals; //The goal that are active and invisible
            public List<int> ActiveGoals; // Goals that are active and visible
            public List<int> InactiveGoals; // Goals that are visible but inactive
            public int Enclosure; // The index of the enclosure to load (starts from 1, 0 is reserved for no enclosure)
            public List<int> LandMarks; // List of all land marks
            public int Quota; // The quota that the person needs to pick up before the next trial is switched too
            public List<float> StartPosition; //The start position of the character (usually 0, 0) If left empty (ie. "[]") start position is random
            public float StartFacing; // The starting angle of the character (in degrees). if set to -1 start facing will be random
            public bool ExitButton; //When this is set to true a button that says "Exit Experiment" will appear at the bottom of screen, when pressed application will close
            public bool ShowBlockTotal; // Whether or not to display the amount of goals/pickups collected (resets each block)
            public bool ShowTrialTotal; // Whether or not to display the amount of goals/pickups collected (resets each trial)
            public bool ShowNumSuccesses; // Whether or not to display the number of successful trials
        }

        // Represents an Enclosure (Maze) for the user. TODO: Enclosures can be prebuilt; located in:...
        // Enclosures can be 'Simple' with basic dimensions specified in the config file
        [Serializable]
        public class Enclosure
        {
            public string EnclosureName; // Non functional (user can use this label to keep track of their enclosures)
            public int Sides; // Number of sides present in the trial.
            public int Radius; // Radius of the walls
            public float WallHeight; // This is the wall height
            public string WallColor; // HEX color of the walls
            public int GroundTileSides; // Number of sides on the ground pattern shapes - 0 for no tiles, 1 for solid color.
            public double GroundTileSize; // Relative size of the floor tiles - Range from 0 to 1
            public string GroundColor; // Colour of the ground
            public List<float> Position; // 2d position vector
        }

        // This is the given PickupItem. Note that in the JSON, this is contained with an ARRAY
        [Serializable]
        public class Goal
        {
            // In general this will always be one unless necessary to implement in the future.
            public string Tag; // The name of the pickup item
            public string Color; // The colour in Hex without #
            public string Sound; // The file path of the sound
            public string PythonFile; // The python file that will generate the position
            public string Type; // The object type (can be "2D" or "3D")
            public string Object; // The name of the prefab if it's 3D, image file name if it's 2D
            public string Image; // The location of the image file associated with the goal

            // Use list for serialization purposes
            public List<float> Position;
            public List<float> Rotation;
            public List<float> Scale;
            public int InitialRotation;

            public Vector3 PositionVector
            {
                get
                {
                    return Position.Count == 0 ? Vector3.zero : new Vector3(Position[0], Position[1], Position[2]);
                }
            }

            public Vector3 RotationVector
            {
                get
                {
                    return Rotation.Count == 0 ? Vector3.zero : new Vector3(Rotation[0], Rotation[1], Rotation[2]);
                }
            }

            public Vector3 ScaleVector
            {
                get
                {
                    return Scale.Count == 0 ? Vector3.zero : new Vector3(Scale[0], Scale[1], Scale[2]);
                }
            }
        }

        [Serializable]
        public class Character
        {
            public float MovementSpeed; //The movement speed of the character
            public float RotationSpeed; //The rotation speed of the character
            public float GoalRotationSpeed; //The rotation speed of the goals
            public float Height; //The height of the character
            public float DistancePickup; //The min distance of the pickup to the character
        }

        // This is a pillar object.
        [Serializable]
        public class LandMark
        {
            public string Type;
            public string Object;
            public string Color; // Color as a hex value
            public string ImageLoc; // The location (for 2Dimagedisplayer)

            // Use list for serialization purposes
            public List<float> Position;
            public List<float> Rotation;
            public List<float> Scale;

            public int InitialRotation;

            public Vector3 PositionVector
            {
                get
                {
                    return Position.Count == 0 ? Vector3.zero : new Vector3(Position[0], Position[1], Position[2]);
                }
            }

            public Vector3 RotationVector
            {
                get
                {
                    return Rotation.Count == 0 ? Vector3.zero : new Vector3(Rotation[0], Rotation[1], Rotation[2]);
                }
            }

            public Vector3 ScaleVector
            {
                get
                {
                    return Scale.Count == 0 ? Vector3.zero : new Vector3(Scale[0], Scale[1], Scale[2]);
                }
            }
        }

        [Serializable]
        public class EnclosureData
        {
            public List<float> TopLeft; //The top left of the enclosure
            public float TileWidth;
            public List<string> Map;
            public string Color;
        }
        //=========================== END OF JSON FIELDS ==========================================

        // This function converts the hex value to Colour.
        public static Color GetColour(string hex)
        {
            float[] l = { 0, 0, 0 };
            for (var i = 0; i < 6; i += 2)
            {
                float decValue = int.Parse(hex.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
                l[i / 2] = decValue / 255;
            }
            return new Color(l[0], l[1], l[2]);

        }

        public class Point
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
        }
    }
}