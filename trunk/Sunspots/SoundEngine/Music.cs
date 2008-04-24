using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace StarForce_PendingTitle_
{
    public static class Music
    {
        static AudioEngine MusicEngine;
        static SoundBank MusicSoundBank;
        static WaveBank MusicWaveBank;

        static List<Cue> MusicList = new List<Cue>();

        private static string MusicFilename;

        /// <summary>
        /// For playing songs
        /// Must have the 'infinite' checkbox marked in the XACT project
        /// Also plays stopped songs
        /// </summary>
        public static void PlaySong(string musicname)
        {
            int index = FindMusicIndex(musicname);
            if (index != -1)
                if (!MusicList[index].IsPlaying)
                    MusicList[index].Play();
                else
                {
                    MusicList.Add(MusicSoundBank.GetCue(musicname));
                    MusicList[index].Play();
                }
            else
            {
                MusicList.Add(MusicSoundBank.GetCue(musicname));
                int i = FindMusicIndex(musicname);
                MusicList[i].Play();
            }
        }

        /// <summary>
        /// Pause a looping songs.
        /// </summary>
        public static void PauseSong(string musicname)
        {
            int index = FindMusicIndex(musicname);

            if (index != -1)
                if (!MusicList[index].IsPaused)
                    MusicList[index].Pause();
        }

        /// <summary>
        /// Resumes a previously paused songs.
        /// </summary>
        public static void ResumeSong(string musicname)
        {
            int index = FindMusicIndex(musicname);

            if (index != -1)
                if (MusicList[index].IsPaused)
                    MusicList[index].Resume();
        }

        /// <summary>
        /// Stop a song.
        /// </summary>
        public static void StopSong(string musicname)
        {
            int index = FindMusicIndex(musicname);

            if (index != -1)
                if (MusicList[index].IsPlaying)
                    MusicList[index].Stop(AudioStopOptions.Immediate);
        }

        private static int FindMusicIndex(string musicname)
        {
            int index = 0;
            foreach (Cue c in MusicList)
            {
                if (c.Name.ToUpper().Equals(musicname.ToUpper()))
                    return index;

                index++;
            }

            return -1;
        }

        public static void SetMusicFilename(string fname)
        {
            MusicFilename = fname;
        }

        public static void ResetEngine()
        {
            try
            {
                MusicEngine = new AudioEngine(MusicFilename + ".xgs");
                MusicSoundBank = new SoundBank(MusicEngine, "Content\\Sound\\Music Sound Bank.xsb");
                MusicWaveBank = new WaveBank(MusicEngine, "Content\\Sound\\Music Wave Bank.xwb");
            }
            catch
            {
                //nothing
            }
        }
    }
}
