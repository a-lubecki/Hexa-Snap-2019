/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioManager : MonoBehaviour {


    private static readonly float DELAY_MUSIC_PRELOADING = 2f;
    private static readonly float DURATION_FADE_OUT = 0.5f;

    private static readonly string COROUTINE_TAG_PLAY_MUSIC = "playMusic";
    private static readonly string COROUTINE_TAG_STOP_MUSIC = "stopMusic";


    private AudioSource sourceSounds;
    private AudioSource sourceMusic1;
    private AudioSource sourceMusic2;

    private readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();

    private MusicInfo currentMusicInfo;


    private void Awake() {
        
        var sources = GetComponents<AudioSource>(); 
        sourceSounds = sources[0];
        sourceMusic1 = sources[1];
        sourceMusic2 = sources[2];
    }


    private AudioClip loadAudio(string audioPath) {

        if (audioPath == null) {
            throw new ArgumentException();
        }

        if (clips.ContainsKey(audioPath)) {
            return clips[audioPath];
        }

        var clip = GameHelper.Instance.loadAudioAsset(audioPath);
        clips[audioPath] = clip;

        return clip;
    }

    public void playSound(string audioName) {

        if (GameHelper.Instance.getGameManager().isSoundsOptionDeactivated) {
            return;
        }

        var clip = loadAudio(Constants.PATH_DESIGNS_SOUNDS + audioName);
        if (clip != null) {
            sourceSounds.PlayOneShot(clip);
        }
    }

    public void playMusic(MusicInfo musicInfo) {

        if (GameHelper.Instance.getGameManager().isMusicOptionDeactivated) {
            return;
        }

        if (musicInfo == currentMusicInfo && 
            (sourceMusic1.isPlaying || sourceMusic2.isPlaying)) {
            //already playing
            return;
        }

        if (Async.isRunning(COROUTINE_TAG_PLAY_MUSIC)) {

            //use a coroutine to control music start/end in a loop
            stopMusic(() => Async.call(playMusicLoop(musicInfo), COROUTINE_TAG_PLAY_MUSIC));

        } else {

            if (Async.isRunning(COROUTINE_TAG_STOP_MUSIC)) {

                //stop immediately
                Async.cancel(COROUTINE_TAG_STOP_MUSIC);

                sourceMusic1.Stop();
                sourceMusic2.Stop();
            }

            //use a coroutine to control music start/end in a loop
            Async.call(playMusicLoop(musicInfo), COROUTINE_TAG_PLAY_MUSIC);    
        }
    }

    private IEnumerator playMusicLoop(MusicInfo musicInfo) {
        
        var clip = loadAudio(Constants.PATH_DESIGNS_MUSICS + musicInfo.audioName);
        if (clip == null) {
            yield break;
        }

        currentMusicInfo = musicInfo;
        var currentSource = sourceMusic2;

        var startTime = musicInfo.startTimeSec;
        var endTime = musicInfo.endTimeSec;
        var nextDspTime = AudioSettings.dspTime + DELAY_MUSIC_PRELOADING;

        playScheduledMusic(currentSource, clip, nextDspTime, 0);

        //wait to the end first before triggering the loop
        yield return new WaitForSeconds(endTime);

        //update dspTime for next iteration
        nextDspTime += endTime;

        var semiClipDurationSec = endTime - startTime;

        //play music on alternated sources infinitely
        while (true) {

            //swap sources
            currentSource = (currentSource == sourceMusic1) ? sourceMusic2 : sourceMusic1;

            playScheduledMusic(currentSource, clip, nextDspTime, startTime);

            //update dspTime for next iteration
            nextDspTime += semiClipDurationSec;

            //wait then play from a time to time, to have a loop
            yield return new WaitForSeconds(semiClipDurationSec);

            //wait until 1sec before the next clip call in case the wait was delayed
            var diff = nextDspTime - AudioSettings.dspTime - 1;
            if (diff > 0) {
                yield return new WaitForSeconds((float)diff);
            }
        }
    }

    private void playScheduledMusic(AudioSource source, AudioClip clip, double dspTime, float clipBegin) {
        
        source.volume = 1;
        source.clip = clip;
        source.time = clipBegin;
        source.PlayScheduled(dspTime);
    }

    public void stopMusic(Action completion = null) {
        
        if (!sourceMusic1.isPlaying && !sourceMusic2.isPlaying) {
            //not playing
            completion?.Invoke();
            return;
        }

        Async.cancel(COROUTINE_TAG_PLAY_MUSIC);

        if (Async.isRunning(COROUTINE_TAG_STOP_MUSIC)) {
            //already fading, wait until the end of the coroutine
            completion?.Invoke();
            return;
        }

        currentMusicInfo = null;

        //use a coroutine to fade out the music
        Async.call(fadeMusic(sourceMusic1, DURATION_FADE_OUT, 0, sourceMusic1.Stop), COROUTINE_TAG_STOP_MUSIC);

        Async.call(fadeMusic(sourceMusic2, DURATION_FADE_OUT, 0, () => {

            sourceMusic2.Stop();

            //finish
            completion?.Invoke();

        }), COROUTINE_TAG_STOP_MUSIC);
    }

    private IEnumerator fadeMusic(AudioSource source, float durationSec, float endVolume, Action completion = null) {

        float startVolume = source.volume;
        float diff = endVolume - startVolume;

        int nbFrames = (int)(durationSec / Constants.COROUTINE_FIXED_UPDATE_S);

        for (int i = 0; i < nbFrames; i++) {

            source.volume = startVolume + diff * (i / (float)nbFrames);

            yield return new WaitForSeconds(Constants.COROUTINE_FIXED_UPDATE_S);
        }

        completion?.Invoke();
    }

}

