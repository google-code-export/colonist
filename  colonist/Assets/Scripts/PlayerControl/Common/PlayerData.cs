using UnityEngine;
using System.Collections;

/// <summary>
/// Player effect data.
/// When applying a damage form, the effect object will be created.
/// </summary>
[System.Serializable]
public class PlayerEffectData : EffectData
{
    public DamageForm DamageForm = DamageForm.Common;
    /// <summary>
    /// If PlayParticle = true, then the particlesystem must not be null.
    /// When receiving damage form attack, the particlesystem will play.
    /// </summary>
    public bool PlayParticle = false;
    public ParticleSystem particlesystem = null;
}

