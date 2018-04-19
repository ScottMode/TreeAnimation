using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager> 
{
	//Mixers
	public AudioMixer masterMixer;
	public AudioMixer musicMixer;
	public AudioMixer effectMixer;
	
	//Sources
	public AudioSource musicSource;
	public AudioSource effectSource;
	public AudioSource UIAudioSource;
	public GameObject effectContainer;
	
	//Sound vars
	public bool isMusicOn{ get; internal set; }
	public bool isEffectOn { get; internal set; }
	Dictionary<string , AudioClip> soundEffects;
	Dictionary<string , AudioClip> musicTracks;
	string currentTrack;
	
	//String constants
	public const string musicLocation = "Resources/Sound/Music";
	public const string effectsLocation = "Resources/Sound/Effects";
	const string musicOnString = "MusicOn";
	const string effectOnString = "EffectOn";
	const string defaultSnapshotStr = "Snapshot";
	const string muteSnapshotStr = "Mute";


	protected SoundManager(){}
	
	void Awake () 
	{
		soundEffects = new Dictionary<string, AudioClip>();
		musicTracks = new Dictionary<string, AudioClip>();
		
		//Sources
		Object[] tracks = Resources.LoadAll(musicLocation);
		foreach(Object obj in tracks)
		{
			musicTracks.Add(obj.name, (AudioClip)obj);
		}
		
		Object[] effects = Resources.LoadAll(effectsLocation);
		foreach(Object obj in effects)
		{
			soundEffects.Add(obj.name, (AudioClip)obj);
		}
		
		currentTrack = "";
	}
	
	void Start()
	{
		SetIsMusicOn(System.Convert.ToBoolean(PlayerPrefs.GetInt(musicOnString, System.Convert.ToInt16(true))));
		SetIsEffectOn(System.Convert.ToBoolean(PlayerPrefs.GetInt(effectOnString, System.Convert.ToInt16(true))));
	}
	
	public void PlayMusic(AudioClip track, bool forceRestart, bool loop = true)
	{
		if (track == null)
		{
			Debug.Log ("Track parameter is null");
			return;
		}
		
		//Debug.Log("Play Track:" + track.name);
		if(forceRestart || track != musicSource.clip)
		{
			musicSource.clip = track;
			musicSource.loop = loop;
			musicSource.Play();
		}
	}

	public void PlayMusic(string trackName, bool forceRestart, bool loop = true)
	{
		if (musicTracks.ContainsKey(trackName))
		{
			PlayMusic(musicTracks[trackName], forceRestart, loop);
		}
		else
		{
			Debug.Log("soundTracks dictionary does not have key: " + trackName);
		}
	}
	
	/// <summary>
	/// Plays sound effect by name.
	/// </summary>
	/// <returns><c>true</c>, if sound effect exists and was played, <c>false</c> otherwise.</returns>
	/// <param name="soundName">Sound name.</param>
	public bool PlaySoundEffect(string soundName)
	{
		if(soundEffects.ContainsKey(soundName))
		{
			effectContainer.GetComponent<AudioSource>().PlayOneShot(soundEffects[soundName]);
			
			return true;
		}
		else
		{
			Debug.Log("soundEffects dictionary does not have key: " + soundName);
		}
		
		return false;
	}
	
	public void MuteSoundEffects(bool mute)
	{
		try
		{
			//effectsMixer.
			effectContainer.SetActive(!mute);
		}
		catch (UnassignedReferenceException e)
		{
			Debug.LogError("Missing reference to effects container.\n" + e.Message);
		}
	}
	
	public void MuteGame(bool value)
	{
		Debug.Log("MuteGame: " + value);
		if(value)
			AudioListener.volume = 0.0f;
		else
			AudioListener.volume = 1.0f;
	}
	
	public void StopAllEffects()
	{
		try
		{
			effectContainer.GetComponent<AudioSource>().Stop();
		}
		catch (UnassignedReferenceException e)
		{
			Debug.LogError("Missing reference to a warning timer.\n" + e.Message);
		}
	}
	
	public void SetIsMusicOn(bool value)
	{
		isMusicOn = value;
		
		if(isMusicOn)
			musicMixer.FindSnapshot(defaultSnapshotStr).TransitionTo(0);
		else
			musicMixer.FindSnapshot(muteSnapshotStr).TransitionTo(0);
		
		PlayerPrefs.SetInt(musicOnString, System.Convert.ToInt16(isMusicOn));
	}
	
	public void SetIsEffectOn(bool value)
	{
		isEffectOn = value;
		
		if(isEffectOn)
			effectMixer.FindSnapshot(defaultSnapshotStr).TransitionTo(0);
		else
			effectMixer.FindSnapshot(muteSnapshotStr).TransitionTo(0);
		
		PlayerPrefs.SetInt(effectOnString, System.Convert.ToInt16(isEffectOn));
	}
}