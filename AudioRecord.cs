using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Android;
using System.Collections;
using System.Collections.Generic;

public class AudioRecord : MonoBehaviour
{
    private string fileName;
    private bool isRecording = false;
    private AudioClip recordedClip;
    private AudioSource audioSource; // Add an AudioSource component to the GameObject

    void Start()
    {
        fileName = Path.Combine(Application.persistentDataPath, "audio3.wav");
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
    }

    public void StartRecording()
    {
        if (!isRecording)
        {
            isRecording = true;
            recordedClip = Microphone.Start(null, false, 50, AudioSettings.outputSampleRate);
            Debug.Log("Recording Started........");
        }
    }

    public void StopRecording()
    {
        if (isRecording)
        {
            isRecording = false;
            Debug.Log("Recording Stopped........");
            Microphone.End(null);

            SaveRecordingToFile();
        }
    }
    public void PlayRecording()
    {
        if (File.Exists(fileName))
        {
            // Load the audio clip from the saved file
            StartCoroutine(LoadAudioClip());
            
        }
        else
        {
            Debug.LogError("Audio file not found: " + fileName);
        }
    }

    private IEnumerator LoadAudioClip()
    {
        // Load the audio clip from the file asynchronously
        using (var www = new WWW("file://" + fileName))
        {
            yield return www;

            if (string.IsNullOrEmpty(www.error))
            {
                audioSource.clip = www.GetAudioClip();
                audioSource.Play();
                Debug.Log("Playback Started........");
            }
            else
            {
                Debug.LogError("Error loading audio clip: " + www.error);
            }
        }
    }
    private void SaveRecordingToFile()
    {
        SavWav.Save(fileName, recordedClip);
        Debug.Log("Audio saved to file path: " + fileName);
    }
}
