﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Placeholdernamespace.Common.Sound
{

    public enum Soundtrack { title, battle};

    public class SoundManager : MonoBehaviour
    {

        private static SoundManager instance;
        public static SoundManager Instance
        {
            get { return instance; }
        }

        [SerializeField]
        private AudioSource musicPlayer;

        [SerializeField]
        public bool muted = false;
        public void setMuted(bool muted)
        {
            this.muted = muted;
        }

        public void SkipSong()
        {
            if(soundTrackCounter >= battleSoundTracks.Count)
            {
                soundTrackCounter = 0;
            }
            SetMusic(battleSoundTracks[soundTrackCounter++]);
        }

        [SerializeField]
        private List<AudioClip> battleSoundTracks;
        private int soundTrackCounter = 0;

        private List<Soundtrack> soundtrackEnum = new List<Soundtrack> {Soundtrack.title, Soundtrack.battle };
        [SerializeField]
        private List<AudioClip> soundtrackAudio;

        private Dictionary<Soundtrack, AudioClip> soundtracks = new Dictionary<Soundtrack, AudioClip>();

        private void Awake()
        {
            instance = this;
            for(int a = 0; a < soundtrackEnum.Count; a++)
            {
                if(a < soundtrackAudio.Count)
                {
                    soundtracks.Add(soundtrackEnum[a], soundtrackAudio[a]);
                }
            }
            SetMusic(Soundtrack.title);
        }

        private void Update()
        {

             musicPlayer.mute = muted;

        }

        public void SetMusic(Soundtrack track)
        {
            if(soundtracks.ContainsKey(track))
            {
                SetMusic(soundtracks[track]);
            }
        }

        public void SetMusic(AudioClip track)
        {
            musicPlayer.Stop();
            if(track.name == "battle1")
            {
                musicPlayer.volume = .5f;
            }
            else
            {
                musicPlayer.volume = 1.0f;
            }
            musicPlayer.loop = true;
            musicPlayer.clip = track;
            musicPlayer.Play();        
        }


    }
}
