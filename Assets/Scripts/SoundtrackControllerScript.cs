using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EnvironmentType { Forest, Clearing, Warehouse };

public class SoundtrackControllerScript : MonoBehaviour
{
    // References to audio source components
    public AudioSource Track1;
    public AudioSource Track2;
    public AudioSource Layer1;
    public AudioSource Layer2;
    public AudioSource Layer3;
    public AudioSource SFX;

    public PlayerScript playerScript;
    public Slider Track1Slider;
    public Slider Track2Slider;
    public Transform[] enemyTransforms;

    // Chance for the soundtrack to transition into a new clip
    public int audioTransitionChance = 4;
    public int playLayerChance = 5;
    public float transitionSpeed = 0.05f;
    public float enemyDistanceSegmentLength = 60.0f;
    public float roamingVolume = 1.0f;
    public float escapeVolume = 0.7f;

    // Keep track of active audio source (1 or 2)
    public int activeSource = 1;

    // Represent an audio clip with its associated intensity value
    public struct soundData
    {
        public float intensity;
        public AudioClip clip;
        public EnvironmentType environment;
    }

    // Sound data loaded for a new crossfade
    AudioClip newClip;

    // The start time for a new crossfade
    float fadeStartTime;

    // The volume of tracks at the start and end of a crossfade
    float fromInitVol = 1.0f, toFinalVol = 1.0f;

    // Lists for each clip and its intensity and/or environment type
    List<soundData> roamingSounds = new List<soundData>();
    List<soundData> escapeSounds = new List<soundData>();
    List<soundData> suddenSounds = new List<soundData>();
    List<soundData> layeredSounds = new List<soundData>();

    List<AudioClip> hitSounds = new List<AudioClip>();

    // Initialize
    void Start()
    {
        // Load in audio assets from Resources folder
        roamingSounds.Add(new soundData { intensity = 1, clip = Resources.Load<AudioClip>("Audio/R/R_C_1_1"), environment = EnvironmentType.Clearing });
        roamingSounds.Add(new soundData { intensity = 1, clip = Resources.Load<AudioClip>("Audio/R/R_C_1_2"), environment = EnvironmentType.Clearing });
        roamingSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/R/R_C_2_1"), environment = EnvironmentType.Clearing });
        roamingSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/R/R_C_2_2"), environment = EnvironmentType.Clearing });
        roamingSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/R/R_C_2_3"), environment = EnvironmentType.Clearing });
        roamingSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/R/R_C_2_4"), environment = EnvironmentType.Clearing });
        roamingSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/R/R_C_2_5"), environment = EnvironmentType.Clearing });
        roamingSounds.Add(new soundData { intensity = 3, clip = Resources.Load<AudioClip>("Audio/R/R_C_3_1"), environment = EnvironmentType.Clearing });
        roamingSounds.Add(new soundData { intensity = 3, clip = Resources.Load<AudioClip>("Audio/R/R_C_3_2"), environment = EnvironmentType.Clearing });
        roamingSounds.Add(new soundData { intensity = 3, clip = Resources.Load<AudioClip>("Audio/R/R_C_3_3"), environment = EnvironmentType.Clearing });
        roamingSounds.Add(new soundData { intensity = 3, clip = Resources.Load<AudioClip>("Audio/R/R_C_3_4"), environment = EnvironmentType.Clearing });
        roamingSounds.Add(new soundData { intensity = 3, clip = Resources.Load<AudioClip>("Audio/R/R_C_3_5"), environment = EnvironmentType.Clearing });
        roamingSounds.Add(new soundData { intensity = 3, clip = Resources.Load<AudioClip>("Audio/R/R_C_3_6"), environment = EnvironmentType.Clearing });
        roamingSounds.Add(new soundData { intensity = 3, clip = Resources.Load<AudioClip>("Audio/R/R_C_3_7"), environment = EnvironmentType.Clearing });
        roamingSounds.Add(new soundData { intensity = 3, clip = Resources.Load<AudioClip>("Audio/R/R_C_3_8"), environment = EnvironmentType.Clearing });
        roamingSounds.Add(new soundData { intensity = 4, clip = Resources.Load<AudioClip>("Audio/R/R_C_4_1"), environment = EnvironmentType.Clearing });
        roamingSounds.Add(new soundData { intensity = 4, clip = Resources.Load<AudioClip>("Audio/R/R_C_4_2"), environment = EnvironmentType.Clearing });
        roamingSounds.Add(new soundData { intensity = 5, clip = Resources.Load<AudioClip>("Audio/R/R_C_5_1"), environment = EnvironmentType.Clearing });
        roamingSounds.Add(new soundData { intensity = 5, clip = Resources.Load<AudioClip>("Audio/R/R_C_5_2"), environment = EnvironmentType.Clearing });
        roamingSounds.Add(new soundData { intensity = 1, clip = Resources.Load<AudioClip>("Audio/R/R_F_1_1"), environment = EnvironmentType.Forest });
        roamingSounds.Add(new soundData { intensity = 1, clip = Resources.Load<AudioClip>("Audio/R/R_F_1_2"), environment = EnvironmentType.Forest });
        roamingSounds.Add(new soundData { intensity = 1, clip = Resources.Load<AudioClip>("Audio/R/R_F_1_3"), environment = EnvironmentType.Forest });
        roamingSounds.Add(new soundData { intensity = 1, clip = Resources.Load<AudioClip>("Audio/R/R_F_1_4"), environment = EnvironmentType.Forest });
        roamingSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/R/R_F_2_1"), environment = EnvironmentType.Forest });
        roamingSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/R/R_F_2_2"), environment = EnvironmentType.Forest });
        roamingSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/R/R_F_2_3"), environment = EnvironmentType.Forest });
        roamingSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/R/R_F_2_4"), environment = EnvironmentType.Forest });
        roamingSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/R/R_F_2_5"), environment = EnvironmentType.Forest });
        roamingSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/R/R_F_2_6"), environment = EnvironmentType.Forest });
        roamingSounds.Add(new soundData { intensity = 3, clip = Resources.Load<AudioClip>("Audio/R/R_F_3_1"), environment = EnvironmentType.Forest });
        roamingSounds.Add(new soundData { intensity = 3, clip = Resources.Load<AudioClip>("Audio/R/R_F_3_2"), environment = EnvironmentType.Forest });
        roamingSounds.Add(new soundData { intensity = 3, clip = Resources.Load<AudioClip>("Audio/R/R_F_3_3"), environment = EnvironmentType.Forest });
        roamingSounds.Add(new soundData { intensity = 3, clip = Resources.Load<AudioClip>("Audio/R/R_F_3_4"), environment = EnvironmentType.Forest });
        roamingSounds.Add(new soundData { intensity = 3, clip = Resources.Load<AudioClip>("Audio/R/R_F_3_5"), environment = EnvironmentType.Forest });
        roamingSounds.Add(new soundData { intensity = 3, clip = Resources.Load<AudioClip>("Audio/R/R_F_3_6"), environment = EnvironmentType.Forest });
        roamingSounds.Add(new soundData { intensity = 3, clip = Resources.Load<AudioClip>("Audio/R/R_F_3_7"), environment = EnvironmentType.Forest });
        roamingSounds.Add(new soundData { intensity = 4, clip = Resources.Load<AudioClip>("Audio/R/R_F_4_1"), environment = EnvironmentType.Forest });
        roamingSounds.Add(new soundData { intensity = 4, clip = Resources.Load<AudioClip>("Audio/R/R_F_4_2"), environment = EnvironmentType.Forest });
        roamingSounds.Add(new soundData { intensity = 4, clip = Resources.Load<AudioClip>("Audio/R/R_F_4_3"), environment = EnvironmentType.Forest });
        roamingSounds.Add(new soundData { intensity = 1, clip = Resources.Load<AudioClip>("Audio/R/R_W_1_1"), environment = EnvironmentType.Warehouse });
        roamingSounds.Add(new soundData { intensity = 1, clip = Resources.Load<AudioClip>("Audio/R/R_W_1_2"), environment = EnvironmentType.Warehouse });
        roamingSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/R/R_W_2_1"), environment = EnvironmentType.Warehouse });
        roamingSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/R/R_W_2_2"), environment = EnvironmentType.Warehouse });
        roamingSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/R/R_W_2_3"), environment = EnvironmentType.Warehouse });
        roamingSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/R/R_W_2_4"), environment = EnvironmentType.Warehouse });
        roamingSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/R/R_W_2_5"), environment = EnvironmentType.Warehouse });
        roamingSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/R/R_W_2_6"), environment = EnvironmentType.Warehouse });
        roamingSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/R/R_W_2_7"), environment = EnvironmentType.Warehouse });
        roamingSounds.Add(new soundData { intensity = 3, clip = Resources.Load<AudioClip>("Audio/R/R_W_3_1"), environment = EnvironmentType.Warehouse });
        roamingSounds.Add(new soundData { intensity = 3, clip = Resources.Load<AudioClip>("Audio/R/R_W_3_2"), environment = EnvironmentType.Warehouse });
        roamingSounds.Add(new soundData { intensity = 4, clip = Resources.Load<AudioClip>("Audio/R/R_W_4_1"), environment = EnvironmentType.Warehouse });
        roamingSounds.Add(new soundData { intensity = 4, clip = Resources.Load<AudioClip>("Audio/R/R_W_4_2"), environment = EnvironmentType.Warehouse });

        escapeSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/E/E2_1") });
        escapeSounds.Add(new soundData { intensity = 3, clip = Resources.Load<AudioClip>("Audio/E/E3_1") });
        escapeSounds.Add(new soundData { intensity = 4, clip = Resources.Load<AudioClip>("Audio/E/E4_1") });
        escapeSounds.Add(new soundData { intensity = 4, clip = Resources.Load<AudioClip>("Audio/E/E4_2") });
        escapeSounds.Add(new soundData { intensity = 4, clip = Resources.Load<AudioClip>("Audio/E/E4_3") });
        escapeSounds.Add(new soundData { intensity = 4, clip = Resources.Load<AudioClip>("Audio/E/E4_4") });
        escapeSounds.Add(new soundData { intensity = 5, clip = Resources.Load<AudioClip>("Audio/E/E5_1") });
        escapeSounds.Add(new soundData { intensity = 5, clip = Resources.Load<AudioClip>("Audio/E/E5_2") });
        escapeSounds.Add(new soundData { intensity = 5, clip = Resources.Load<AudioClip>("Audio/E/E5_3") });

        suddenSounds.Add(new soundData { intensity = 1, clip = Resources.Load<AudioClip>("Audio/S/S_1_1") });
        suddenSounds.Add(new soundData { intensity = 1, clip = Resources.Load<AudioClip>("Audio/S/S_1_2") });
        suddenSounds.Add(new soundData { intensity = 1, clip = Resources.Load<AudioClip>("Audio/S/S_1_3") });
        suddenSounds.Add(new soundData { intensity = 1, clip = Resources.Load<AudioClip>("Audio/S/S_1_4") });
        suddenSounds.Add(new soundData { intensity = 1, clip = Resources.Load<AudioClip>("Audio/S/S_1_5") });
        suddenSounds.Add(new soundData { intensity = 1, clip = Resources.Load<AudioClip>("Audio/S/S_1_6") });
        suddenSounds.Add(new soundData { intensity = 1, clip = Resources.Load<AudioClip>("Audio/S/S_1_7") });
        suddenSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/S/S_2_1") });
        suddenSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/S/S_2_2") });
        suddenSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/S/S_2_3") });
        suddenSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/S/S_2_4") });
        suddenSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/S/S_2_5") });
        suddenSounds.Add(new soundData { intensity = 2, clip = Resources.Load<AudioClip>("Audio/S/S_2_6") });
        suddenSounds.Add(new soundData { intensity = 3, clip = Resources.Load<AudioClip>("Audio/S/S_3_1") });
        suddenSounds.Add(new soundData { intensity = 3, clip = Resources.Load<AudioClip>("Audio/S/S_3_2") });
        suddenSounds.Add(new soundData { intensity = 3, clip = Resources.Load<AudioClip>("Audio/S/S_3_3") });
        suddenSounds.Add(new soundData { intensity = 3, clip = Resources.Load<AudioClip>("Audio/S/S_3_4") });
        suddenSounds.Add(new soundData { intensity = 4, clip = Resources.Load<AudioClip>("Audio/S/S_4_1") });
        suddenSounds.Add(new soundData { intensity = 4, clip = Resources.Load<AudioClip>("Audio/S/S_4_2") });
        suddenSounds.Add(new soundData { intensity = 4, clip = Resources.Load<AudioClip>("Audio/S/S_4_3") });
        suddenSounds.Add(new soundData { intensity = 5, clip = Resources.Load<AudioClip>("Audio/S/S_5_1") });
        suddenSounds.Add(new soundData { intensity = 5, clip = Resources.Load<AudioClip>("Audio/S/S_5_2") });

        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_C_1"), environment = EnvironmentType.Clearing });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_C_2"), environment = EnvironmentType.Clearing });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_C_3"), environment = EnvironmentType.Clearing });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_C_4"), environment = EnvironmentType.Clearing });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_C_5"), environment = EnvironmentType.Clearing });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_C_6"), environment = EnvironmentType.Clearing });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_C_7"), environment = EnvironmentType.Clearing });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_C_8"), environment = EnvironmentType.Clearing });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_C_9"), environment = EnvironmentType.Clearing });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_C_10"), environment = EnvironmentType.Clearing });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_C_11"), environment = EnvironmentType.Clearing });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_C_12"), environment = EnvironmentType.Clearing });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_C_13"), environment = EnvironmentType.Clearing });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_F_1"), environment = EnvironmentType.Forest });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_F_2"), environment = EnvironmentType.Forest });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_F_3"), environment = EnvironmentType.Forest });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_F_4"), environment = EnvironmentType.Forest });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_F_5"), environment = EnvironmentType.Forest });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_F_6"), environment = EnvironmentType.Forest });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_F_7"), environment = EnvironmentType.Forest });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_F_8"), environment = EnvironmentType.Forest });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_F_9"), environment = EnvironmentType.Forest });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_F_10"), environment = EnvironmentType.Forest });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_F_11"), environment = EnvironmentType.Forest });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_F_12"), environment = EnvironmentType.Forest });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_F_13"), environment = EnvironmentType.Forest });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_W_1"), environment = EnvironmentType.Warehouse });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_W_2"), environment = EnvironmentType.Warehouse });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_W_3"), environment = EnvironmentType.Warehouse });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_W_4"), environment = EnvironmentType.Warehouse });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_W_5"), environment = EnvironmentType.Warehouse });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_W_6"), environment = EnvironmentType.Warehouse });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_W_7"), environment = EnvironmentType.Warehouse });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_W_8"), environment = EnvironmentType.Warehouse });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_W_9"), environment = EnvironmentType.Warehouse });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_W_10"), environment = EnvironmentType.Warehouse });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_W_11"), environment = EnvironmentType.Warehouse });
        layeredSounds.Add(new soundData { clip = Resources.Load<AudioClip>("Audio/L/L_W_12"), environment = EnvironmentType.Warehouse });

        hitSounds.Add(Resources.Load<AudioClip>("Audio/SFX/F1"));
        hitSounds.Add(Resources.Load<AudioClip>("Audio/SFX/F2"));
        hitSounds.Add(Resources.Load<AudioClip>("Audio/SFX/F3"));

        // Play initial soundtrack clip
        Track1.clip = roamingSounds[1].clip;
        Track2.volume = 0.0f;
        Track1.Play(1);

        // Update the soundtrack at a fixed delay
        InvokeRepeating("UpdateRoamingSoundtrack", 5, 5);
        InvokeRepeating("UpdateEscapeSoundtrack", 5, 1);
        InvokeRepeating("UpdateLayers", 8, 4);
    }

    // Update
    void Update ()
    {
        // Handle crossfades
        if (Track1.volume < toFinalVol && activeSource == 1)
        {
            Crossfade(Track2, Track1, transitionSpeed);

        }
        if (Track2.volume < toFinalVol && activeSource == 2)
        {
            Crossfade(Track1, Track2, transitionSpeed);
        }

        // Handle player view checking and escape progress
        if (playerScript.currentState == PlayerState.Roam)
        {
            int index = EnemySighted();

            if ( index != -1 && DistanceToEnemy(index) < 35.0f)
            {
                Stare();
            }
        }
        else if (playerScript.currentState == PlayerState.Escape)
        {
            UpdateEscapeSoundtrack();
        }
        else
        {
            print("invalid player state");
        }

        //Track1Slider.value = Track1.volume;
        //Track2Slider.value = Track2.volume;
    }

    // Get distance from the player to the closest enemy
    float DistanceToClosestEnemy()
    {
        float distance = 100.0f;

        foreach (Transform t in enemyTransforms)
        {
            float newDistance = Mathf.Abs((transform.position - t.position).magnitude);

            if (newDistance < distance) distance = newDistance;
        }

        //print("distancetoenemy: " + distance);
        return distance;
    }

    float DistanceToEnemy(int index)
    {
        return Mathf.Abs((transform.position - enemyTransforms[index].position).magnitude);
    }

    // Change the soundtrack when the player looks at an enemy
    public void Stare()
    {
        print("player saw the enemy!");

        // Play SFX for appropriate distance
        int intensity = 6 - Mathf.FloorToInt(Mathf.Clamp((DistanceToClosestEnemy() / 7.0f), 1.0f, 5.0f));
        //print("intensity = " + intensity);

        // Find a clip with this intensity in the list
        newClip = FindClip(intensity, suddenSounds);
        Layer3.clip = newClip;
        Layer3.Play();

        // Begin escape state
        playerScript.currentState = PlayerState.Escape;
        playLayerChance = 3;
        transitionSpeed = 0.1f;
        if (intensity == 5)
        {
            Track1.loop = false;
            Track2.loop = false;
        }

        // Transition to escape music
        newClip = FindClip(intensity, escapeSounds);
        if (Track1.volume > 0.9f || Track2.volume > 0.9f)
            AssignClip(newClip, escapeVolume);
        else
            ForceAssignClip(newClip, escapeVolume);

        fadeStartTime = Time.time;
    }

    void UpdateRoamingSoundtrack()
    {
        // Decide whether to begin an audio transition
        if (playerScript.currentState == PlayerState.Roam)
        {
            if (Random.Range(0, audioTransitionChance) < 1)
            {

                if (Track1.volume == 1.0f && activeSource == 1 || Track2.volume == 1.0f && activeSource == 2)
                {
                    print("updating roaming soundtrack");
                    // Calculate desirable intensity value from distance to closest enemy
                    //print("calculating intensity");
                    int intensity = Mathf.FloorToInt(Mathf.Clamp((DistanceToClosestEnemy() / enemyDistanceSegmentLength) + 1.0f, 1.0f, 5.0f));
                    //print("intensity = " + intensity);

                    // Find and store a clip with this intensity in the list
                    newClip = FindClip(intensity, playerScript.currentEnvironment, roamingSounds);
                    AssignClip(newClip, roamingVolume);
                    print("clip assigned: " + newClip.name);
                }
            }
        }
    }

    void UpdateEscapeSoundtrack()
    {
        if (playerScript.currentState == PlayerState.Escape)
        {
            //print("updating escape soundtrack");

            int intensity = 6 - Mathf.FloorToInt(Mathf.Clamp((DistanceToClosestEnemy() / 8.0f), 1.0f, 5.0f));
            //print("intensity = " + intensity);

            // Player state should be set to Roam, and a clip from roamingSounds should play, if distance to closest enemy becomes high enough
            if (intensity == 1)
            {
                print("transitioning to roaming state");
                playerScript.currentState = PlayerState.Roam;
                playLayerChance = 6;
                transitionSpeed = 0.05f;
                Track1.loop = true;
                Track2.loop = true;

                newClip = FindClip(1, roamingSounds);
                if (Track1.volume > 0.9f || Track2.volume > 0.9f)
                    AssignClip(newClip, roamingVolume);
                else
                    ForceAssignClip(newClip, roamingVolume);

                print("clip assigned: " + newClip.name);
            }
        }
    }

    void UpdateLayers()
    {
        if (Random.Range(0.0f, playLayerChance) < 1.0f)
        {
            //print("updating layers");
            //print("finding audio clip");
            newClip = FindClip(playerScript.currentEnvironment, layeredSounds);
            print("clip chosen: " + newClip.name);

            if (!Layer1.isPlaying)
            {
                //print("chose layer 1");
                Layer1.clip = newClip;
                Layer1.Play();
            }
            else if (!Layer2.isPlaying)
            {
                //print("chose layer 2");
                Layer2.clip = newClip;
                Layer2.Play();
            }
            else if (!Layer3.isPlaying)
            {
                //print("chose layer 3");
                Layer3.clip = newClip;
                Layer3.Play();
            }
            else
                print("all layers are full");
        }
    }

    public void PlayHitSound()
    {
        SFX.clip = hitSounds[Random.Range(0, 3)];
        SFX.Play();
    }

    // Fade between two audio clips
    void Crossfade(AudioSource from, AudioSource to, float fadeSpeed) {
        float timeElapsed = Time.time - fadeStartTime;
        float deltaVolume = timeElapsed * fadeSpeed;
        from.volume = Mathf.SmoothStep(fromInitVol, 0.0f, deltaVolume);
        to.volume = Mathf.SmoothStep(0.0f, toFinalVol, deltaVolume);
    }

    AudioClip FindClip(int intensity, List<soundData> data)
    {
        // Find a clip with this intensity in the list
        //print("finding audio clip");
        List<soundData> dataSelection = new List<soundData>();
        foreach (soundData s in data)
        {
            // Add to audio clips for random selection
            if (s.intensity == intensity)
                dataSelection.Add(s);
        }

        if (dataSelection.Count > 0)
            return dataSelection[Random.Range(0, dataSelection.Count)].clip;
        else
        {
            print("no clips found");
            return null;
        }
    }

    AudioClip FindClip(EnvironmentType type, List<soundData> data)
    {
        // Find a clip with matching intensity and environment type in the list
        List<soundData> dataSelection = new List<soundData>();
        foreach (soundData s in data)
        {
            // Add to audio clips for random selection
            if (s.environment == type)
                dataSelection.Add(s);
        }

        if (dataSelection.Count > 0)
            return dataSelection[Random.Range(0, dataSelection.Count)].clip;
        else
        {
            print("no clips found");
            return null;
        }
    }

    AudioClip FindClip(int intensity, EnvironmentType type, List<soundData> data)
    {
        // Find a clip with matching intensity and environment type in the list
        List<soundData> dataSelection = new List<soundData>();
        foreach (soundData s in data)
        {
            // Add to audio clips for random selection
            if (s.intensity == intensity && s.environment == type)
                dataSelection.Add(s);
        }

        if (dataSelection.Count > 0)
            return dataSelection[Random.Range(0, dataSelection.Count)].clip;
        else
        {
            print("no clips found");
            return null;
        }
    }

    void AssignClip(AudioClip clip, float targetVol)
    {
        toFinalVol = targetVol;

        // Establish which audio source should be transitioned to
        if (activeSource == 1)
        {
            // Set the other audio source as the active source and play the clip
            activeSource = 2;
            Track2.clip = clip;
            Track2.Play();
        }
        else if (activeSource == 2)
        {
            activeSource = 1;
            Track1.clip = clip;
            Track1.Play();
        }
        else
            print("invalid audio source");

        //print("clip assigned: " + clip.name);

        fromInitVol = 1.0f;
        fadeStartTime = Time.time;
    }

    void ForceAssignClip(AudioClip clip, float targetVol)
    {
        toFinalVol = targetVol;

        // Establish which audio source should be transitioned to
        if (Track1.volume > Track2.volume)
        {
            print("assigning clip to Track 2 by force!");
            Track2.volume = 0.0f;
            fromInitVol = Track1.volume;
            activeSource = 2;
            Track2.clip = clip;
            Track2.Play();
        }
        else
        {
            print("assigning clip to Track 1 by force!");
            Track1.volume = 0.0f;
            fromInitVol = Track2.volume;
            activeSource = 1;
            Track1.clip = clip;
            Track1.Play();
        }

        //print("clip assigned: " + clip.name);

        fadeStartTime = Time.time;
    }

    int EnemySighted()
    {
        int index = -1;

        for (int i = 0; i < enemyTransforms.Length; i++)
        {
            if (Vector3.Angle(transform.forward, -enemyTransforms[i].forward) < 50.0f)
                index = i;
        }

        return index;
    }
}