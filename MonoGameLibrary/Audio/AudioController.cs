using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace MonoGameLibrary.Audio;

public class AudioController : IDisposable
{
    private readonly List<SoundEffectInstance> _activeSoundEffectInstances;

    private float _previousSongVolume;
    private float _previousSoundEffectVolume;

    public bool IsMuted { get; private set; }

    public float SongVolume
    {
        get
        {
            if (IsMuted)
            {
                return 0.0f;
            }

            return MediaPlayer.Volume;
        }
        set
        {
            if (IsMuted)
            {
                return;
            }

            MediaPlayer.Volume = Math.Clamp(value, 0.0f, 1.0f);
        }
    }

    public float SoundEffectVolume
    {
        get
        {
            if (IsMuted)
            {
                return 0.0f;
            }

            return SoundEffect.MasterVolume;
        }
        set
        {
            if (IsMuted)
            {
                return;
            }

            SoundEffect.MasterVolume = Math.Clamp(value, 0.0f, 1.0f);
        }
    }

    public bool IsDisposed { get; private set; }

    public AudioController()
    {
        _activeSoundEffectInstances = new List<SoundEffectInstance>();
    }

    ~AudioController() => Dispose(false);

    // Проверка на наличие экземпляров с истекшим сроком действия и удаление
    public void Update()
    {
        for (int i = _activeSoundEffectInstances.Count - 1; i >= 0; i--)
        {
            SoundEffectInstance instance = _activeSoundEffectInstances[i];

            if (instance.State == SoundState.Stopped)
            {
                if (!instance.IsDisposed)
                {
                    instance.Dispose();
                }
                _activeSoundEffectInstances.RemoveAt(i);
            }
        }
    }

    // Воспроизведение
    public SoundEffectInstance PlaySoundEffect(SoundEffect soundEffect)
    {
        return PlaySoundEffect(soundEffect, 1.0f, 0.0f, 0.0f, false);
    }

    public SoundEffectInstance PlaySoundEffect(SoundEffect soundEffect, float volume, float pitch, float pan, bool isLooped)
    {
        SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();

        soundEffectInstance.Volume = volume;
        soundEffectInstance.Pitch = pitch;
        soundEffectInstance.Pan = pan;
        soundEffectInstance.IsLooped = isLooped;

        soundEffectInstance.Play();

        _activeSoundEffectInstances.Add(soundEffectInstance);

        return soundEffectInstance;
    }

    public void PlaySong(Song song, bool isRepeating = true)
    {
        if (MediaPlayer.State == MediaState.Playing)
        {
            MediaPlayer.Stop();
        }

        MediaPlayer.Play(song);
        MediaPlayer.IsRepeating = isRepeating;
    }

    // Управление состоянием контроллера
    public void PauseAudio()
    {
        // Pause any active songs playing
        MediaPlayer.Pause();

        // Pause any active sound effects
        foreach (SoundEffectInstance soundEffectInstance in _activeSoundEffectInstances)
        {
            soundEffectInstance.Pause();
        }
    }

    public void ResumeAudio()
    {
        MediaPlayer.Resume();

        foreach (SoundEffectInstance soundEffectInstance in _activeSoundEffectInstances)
        {
            soundEffectInstance.Resume();
        }
    }

    public void MuteAudio()
    {
        // Store the volume so they can be restored during ResumeAudio
        _previousSongVolume = MediaPlayer.Volume;
        _previousSoundEffectVolume = SoundEffect.MasterVolume;

        // Set all volumes to 0
        MediaPlayer.Volume = 0.0f;
        SoundEffect.MasterVolume = 0.0f;

        IsMuted = true;
    }

    public void UnmuteAudio()
    {
        // Restore the previous volume values
        MediaPlayer.Volume = _previousSongVolume;
        SoundEffect.MasterVolume = _previousSoundEffectVolume;

        IsMuted = false;
    }

    // Toggles the current audio mute state.
    public void ToggleMute()
    {
        if (IsMuted)
        {
            UnmuteAudio();
        }
        else
        {
            MuteAudio();
        }
    }

    /// <summary>
    /// Disposes this audio controller and cleans up resources.
    /// </summary>
    /// <param name="disposing">Indicates whether managed resources should be disposed.</param>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        if (disposing)
        {
            foreach (SoundEffectInstance soundEffectInstance in _activeSoundEffectInstances)
            {
                soundEffectInstance.Dispose();
            }
            _activeSoundEffectInstances.Clear();
        }
        IsDisposed = true;
    }
}