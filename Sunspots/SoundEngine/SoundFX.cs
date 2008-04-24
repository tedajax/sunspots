using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace StarForce_PendingTitle_
{
    public static class SoundFX
    {
        static AudioEngine SoundEngine;
        static SoundBank FXSoundBank;
        static WaveBank FXWaveBank;

        static List<Cue> LoopCueList = new List<Cue>();
        static List<Cue> PlayCueList = new List<Cue>();

        private static string SoundFilename;

        /// <summary>
        /// For use with none looping sounds
        /// </summary>
        /// <param name="soundname"></param>
        public static void PlaySound(string soundname)
        {
            Cue tempcue = FXSoundBank.GetCue(soundname);
            PlayCueList.Add(tempcue);
            int i = PlayCueList.Count - 1;
            PlayCueList[i].Play();
        }

        /// <summary>
        /// For playing sounds that are supposed to loop
        /// Must have the 'infinite' checkbox marked in the XACT project
        /// Also plays stopped sounds
        /// </summary>
        /// <param name="soundname"></param>
        public static void LoopSound(string soundname)
        {
            int index = FindLoopingIndex(soundname);
            if (index != -1)
            {
                if (!LoopCueList[index].IsPlaying)
                    LoopCueList[index].Play();
            }
            else
            {
                LoopCueList.Add(FXSoundBank.GetCue(soundname));
                index = LoopCueList.Count - 1;
                LoopCueList[index].Play();
            }
        }

        /// <summary>
        /// Pause a looping sound.  ONLY WORKS WITH LOOPING SOUNDS
        /// </summary>
        /// <param name="soundname"></param>
        public static void PauseSound(string soundname)
        {
            int index = FindLoopingIndex(soundname);

            if (index != -1)
                if (LoopCueList[index].IsPlaying)
                    LoopCueList[index].Pause();            
        }

        /// <summary>
        /// Resumes a previously paused sound.  ONLY WORKS WITH LOOPING SOUNDS
        /// </summary>
        /// <param name="soundname"></param>
        public static void ResumeSound(string soundname)
        {
            int index = FindLoopingIndex(soundname);

            if (index != -1)
                if (LoopCueList[index].IsPaused)
                    LoopCueList[index].Resume();
        }

        /// <summary>
        /// Stop a LOOPING sound
        /// </summary>
        /// <param name="soundname"></param>
        public static void StopSound(string soundname)
        {
            int index = FindLoopingIndex(soundname);

            if (index != -1)
                if (LoopCueList[index].IsPlaying)
                    LoopCueList[index].Stop(AudioStopOptions.Immediate);
        }

        private static int FindLoopingIndex(string soundname)
        {
            int index = 0;
            foreach (Cue c in LoopCueList)
            {
                if (c.Name.ToUpper().Equals(soundname.ToUpper()))
                    return index;

                index++;
            }

            return -1;
        }

        public static void UpdatePlayedSounds()
        {
            int index = 0;
            List<int> RemoveIndexes = new List<int>();
            foreach (Cue c in PlayCueList)
            {
                RemoveIndexes.Add(index);
                index++;
            }

            foreach (int i in RemoveIndexes)
            {
                PlayCueList.RemoveAt(i);
            }
        }

        private static int FindPlayingIndex(string soundname)
        {
            int index = 0;
            foreach (Cue c in PlayCueList)
            {
                if (c.ToString().ToUpper().Equals(soundname.ToUpper()))
                    return index;

                index++;
            }

            return -1;
        }

        public static void SetSoundFilename(string fname)
        {
            SoundFilename = fname;
        }

        public static bool IsPlaying(string soundname)
        {
            foreach (Cue c in LoopCueList)
            {
                if (c.Name.ToUpper().Equals(soundname.ToUpper()) && c.IsPlaying)
                    return true;
            }

            foreach (Cue c in PlayCueList)
            {
                if (c.Name.ToUpper().Equals(soundname.ToUpper()) && c.IsPlaying)
                    return true;
            }

            return false;
        }

        public static bool LoopingSoundExist(string soundname)
        {
            if (FindLoopingIndex(soundname) > -1)
                return true;
            else
                return false;
        }

        public static SoundBank GetSoundFXSoundBank() { return FXSoundBank; }

        public static void ResetEngine()
        {
            try
            {
                SoundEngine = new AudioEngine(SoundFilename + ".xgs");
                FXSoundBank = new SoundBank(SoundEngine, "Content\\Sound\\SoundFX Sound Bank.xsb");
                FXWaveBank = new WaveBank(SoundEngine, "Content\\Sound\\SoundFX Wave Bank.xwb");
            }
            catch
            {
                
            }
        }
    }
}
