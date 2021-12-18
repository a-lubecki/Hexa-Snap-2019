/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */
using System;


public class MusicInfo {

    public readonly string audioName;
    public readonly float startTimeSec;
    public readonly float endTimeSec;

    public MusicInfo(string audioName, float startTimeSec, float endTimeSec) {

        if (audioName == null) {
            throw new ArgumentException();
        }

        if (startTimeSec < 0) {
            throw new ArgumentException();
        }

        if (endTimeSec <= 0) {
            throw new ArgumentException();
        }

        if (startTimeSec >= endTimeSec) {
            throw new ArgumentException();
        }

        this.audioName = audioName;
        this.startTimeSec = startTimeSec;
        this.endTimeSec = endTimeSec;
    }

}