using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> DirtFootSteps = new List<AudioClip>();
    [SerializeField] private List<AudioClip> StoneFootsteps = new List<AudioClip>();
    [SerializeField] private List<AudioClip> WoodFootSteps = new List<AudioClip>();

    [SerializeField] private List<AudioClip> GlideSounds = new List<AudioClip>();

    [SerializeField] private List<AudioClip> JumpSounds = new List<AudioClip>();
    [SerializeField] private List<AudioClip> DoubleJumpSounds = new List<AudioClip>();

    [SerializeField] private List<AudioClip> InjuredSounds = new List<AudioClip>();
    [SerializeField] private List<AudioClip> IdleSounds = new List<AudioClip>();
    [SerializeField] private AudioClip Dying;

    [SerializeField] private AudioClip AuraEffect;
    [SerializeField] private AudioClip AuraStart;
    [SerializeField] private AudioClip AuraEnd;

    [SerializeField] private AudioClip Heartbeat;

    public AudioSource footsteps;
    public AudioSource voice;
    public AudioSource wings;
    public AudioSource jump;
    public AudioSource aura;
    public AudioSource dying;
    public AudioSource heartbeat;
    public AudioSource crystal;
    private static System.Random random = new System.Random();

    private float lastFootstepTime = -footstepCooldown;
    private float lastGlideSoundTime = -soundCooldown;
    private float lastJumpSoundTime = -jumpCooldown;
    private float lastDoubleJumpSoundTime = -jumpDoubleCooldown;
    private float lastHeartbeatSoundTime = -heartbeatCooldown;
    private float lastInjuredSoundTime = 0f; // Initialize to 0 to allow immediate play
    private float lastIdleSoundTime = 0f; // Initialize to 0 to allow immediate play
    private const float soundCooldown = 6f;
    private const float heartbeatCooldown = 0.6f;
    private const float footstepCooldown = 0.7f;
    private const float jumpCooldown = 0.2f;
    private const float jumpDoubleCooldown = 0.3f;
    private const float idleCooldown = 60f;
    private const float injuredCooldown = 60f;

    public void PlayFootsteps(float floor)
    {
        AudioClip clipToPlay = null;

        switch (floor)
        {
            case 0:
                clipToPlay = GetRandomClip(DirtFootSteps);
                break;
            case 1:
                clipToPlay = GetRandomClip(StoneFootsteps);
                break;
            case 2:
                clipToPlay = GetRandomClip(WoodFootSteps);
                break;
        }

        if (clipToPlay != null && Time.time - lastFootstepTime >= footstepCooldown)
        {
            lastFootstepTime = Time.time;
            footsteps.clip = clipToPlay;
            footsteps.Play();
            Debug.Log("Playing footstep sound: " + clipToPlay.name);
        }
        else
        {
            Debug.Log("Footstep sound not played. Either clip is null or cooldown not met.");
        }
    }

    public void StopFootsteps()
    {
        if (footsteps.isPlaying)
        {
            footsteps.Stop();
            Debug.Log("Footstep sound stopped.");
        }
    }

    public void PlayGlideSounds()
    {
        AudioClip clipToPlay = GetRandomClip(GlideSounds);
        if (clipToPlay != null && Time.time - lastGlideSoundTime >= soundCooldown)
        {
            lastGlideSoundTime = Time.time;
            wings.clip = clipToPlay;
            wings.Play();
        }
    }

    public void PlayJumpSounds()
    {
        AudioClip clipToPlay = GetRandomClip(JumpSounds);
        if (clipToPlay != null && Time.time - lastJumpSoundTime >= jumpCooldown)
        {
            lastJumpSoundTime = Time.time;
            jump.clip = clipToPlay;
            jump.Play();
        }
    }

    public void PlayDoubleJumpSounds()
    {
        AudioClip clipToPlay = GetRandomClip(DoubleJumpSounds);
        if (clipToPlay != null && Time.time - lastDoubleJumpSoundTime >= jumpDoubleCooldown)
        {
            lastDoubleJumpSoundTime = Time.time;
            jump.clip = clipToPlay;
            jump.Play();
        }
    }

    public void PlayInjuredSounds()
    {
        AudioClip clipToPlay = GetRandomClip(InjuredSounds);
        if (clipToPlay != null && (Time.time - lastInjuredSoundTime >= injuredCooldown || lastInjuredSoundTime == 0f))
        {
            lastInjuredSoundTime = Time.time;
            voice.clip = clipToPlay;
            voice.Play();
        }
    }

    public void PlayIdleSounds()
    {
        AudioClip clipToPlay = GetRandomClip(IdleSounds);
        if (clipToPlay != null && (Time.time - lastIdleSoundTime >= idleCooldown || lastIdleSoundTime == 0f))
        {
            lastIdleSoundTime = Time.time;
            voice.clip = clipToPlay;
            voice.Play();
        }
    }
    public void PlayHeartbeat()
    {
        if (heartbeat.clip != null && Time.time - lastHeartbeatSoundTime >= heartbeatCooldown)
        {
            lastIdleSoundTime = Time.time;
            voice.Play();
        }
    }
    public void PowerupStart()
    {
        AudioClip clipToPlay = AuraStart;
        if (clipToPlay != null)
        {
            aura.clip = clipToPlay;
            aura.Play();
        }
    }
    public void PowerupEnd()
    {
        AudioClip clipToPlay = AuraEnd;
        if (clipToPlay != null)
        {
            aura.clip = clipToPlay;
            aura.Play();
        }
    }
    public void PowerupContinuous()
    {
        AudioClip clipToPlay = AuraEffect;
        if (clipToPlay != null)
        {
            aura.clip = clipToPlay;
            aura.Play();
        }
    }

    private AudioClip GetRandomClip(List<AudioClip> clips)
    {
        if (clips.Count == 0) return null;
        int index = random.Next(clips.Count);
        return clips[index];
    }
}