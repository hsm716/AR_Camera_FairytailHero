using System.Collections.Generic;
using UnityEngine;

public class ANAExample : MonoBehaviour
{
	// Each piece of audio needs two variables, file ID and sound ID
	int FileID_1;
	int FileID_2;
	int FileID_3;
	int FileID_4;
	int SoundID;

	public UI_Manager ui_m;

	void Start()
	{
		// Set up Android Native Audio
		AndroidNativeAudio.makePool();
		FileID_1 = AndroidNativeAudio.load("Android Native Audio/Tone Native.wav");
		FileID_2 = AndroidNativeAudio.load("Android Native Audio/camera_shot.wav");
		FileID_3 = AndroidNativeAudio.load("Android Native Audio/cam01.wav");
		FileID_4 = AndroidNativeAudio.load("Android Native Audio/cam02.wav");

	}
	public void CameraShot()
    {
		if(ui_m.rec_mode%2 == 1)
        {
			SoundID = AndroidNativeAudio.play(FileID_3);
		}
        else
        {
			SoundID = AndroidNativeAudio.play(FileID_2);
		}
	}
	public void CameraCam2()
	{
		SoundID = AndroidNativeAudio.play(FileID_4);
	}

	void OnApplicationQuit()
	{
		// Clean up when done
		AndroidNativeAudio.unload(FileID_1);
		AndroidNativeAudio.unload(FileID_2);
		AndroidNativeAudio.unload(FileID_3);
		AndroidNativeAudio.unload(FileID_4);
		AndroidNativeAudio.releasePool();
	}


	void ModifySound()
	{
		// These aren't necessary, but show how you could work with a loaded sound.

		AndroidNativeAudio.pause(SoundID);
		AndroidNativeAudio.resume(SoundID);
		AndroidNativeAudio.stop(SoundID);

		AndroidNativeAudio.pauseAll();
		AndroidNativeAudio.resumeAll();

		AndroidNativeAudio.setVolume(SoundID, 0.5f);
		AndroidNativeAudio.setLoop(SoundID, 3);
		AndroidNativeAudio.setPriority(SoundID, 5);
		AndroidNativeAudio.setRate(SoundID, 0.75f);
	}
}
