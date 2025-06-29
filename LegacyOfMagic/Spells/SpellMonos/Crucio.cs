using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LegacyOfMagic.Management;
using LegacyOfMagic.Spells.VFX;
using ThunderRoad;
using UnityEngine;
using UnityEngine.VFX;

namespace LegacyOfMagic.Spells.SpellMonos
{
    public class Crucio : Spell
    {
        private Creature hitCreature;
        private bool effectHappening;
        private GameObject startSFX;
        private GameObject loopSFX;
        private GameObject endSFX;
        private Gradient mainGradient;
        private Gradient secondaryGradient;

        private EffectData effectData;
        public override void Start()
        {
            base.Start();
            usedWand = GetComponent<Item>();
            usedWand.OnHeldActionEvent += HeldAction;
            if (!startSFX) startSFX = Instantiate(ModEntry.local.crucioStartEffect);
            if (!loopSFX) loopSFX = Instantiate(ModEntry.local.crucioLoopEffect);
            if (!endSFX) endSFX = Instantiate(ModEntry.local.crucioEndEffect);
            Cast();
        }

        private void HeldAction(RagdollHand ragdollhand, Handle handle, Interactable.Action action)
        {
            if (action == Interactable.Action.AlternateUseStart && hitCreature)
            {
                effectHappening = false;
                spellActive = false;
                activeCast.Stop();
                endSFX.GetComponent<AudioSource>().Play();
                VFXPool.local.GetPoolByReference(ModEntry.crucioCastEffect).Enqueue(activeCast.transform.root.gameObject);
                activeCast = null;
                hitCreature = null;
            }
        }

        public override void Cast()
        {
            if (!spellActive)
            {
                hitCreature = GetCreaturesWithinAngleAndDistance(usedWand.flyDirRef.transform.position,
                    usedWand.flyDirRef.forward, 45f, 60f);

                if (hitCreature != null) ExecuteCrucio(hitCreature);
            }
        }

        private void CastSpellEffect(Creature creature)
        {
            if(!startSFX.GetComponent<AudioSource>().isPlaying) startSFX.GetComponent<AudioSource>().Play();
            spellActive = true;
            creature.ragdoll.SetState(Ragdoll.State.Destabilized);
            effectHappening = true;
        }
        
        void ExecuteCrucio(Creature creature)
        {
            followTransform = creature.ragdoll.targetPart.transform;
            GameManager.local.StartCoroutine(SetupCast(creature.ragdoll.targetPart.transform.position, ModEntry.crucioCastEffect));
        }
        public override void ExecuteAfterInstantiate()
        {
            CastSpellEffect(hitCreature);
        }
        public override void ExecuteIfCached()
        {
            CastSpellEffect(hitCreature);
        }

        public override void Update()
        {
            if (followTransform && activeCast && activeCast.aliveParticleCount > 0)
            {
                loopSFX.transform.position = usedWand.transform.position;
                endSFX.transform.position = usedWand.transform.position;
                startSFX.transform.position = usedWand.transform.position;
                if (!startSFX.GetComponent<AudioSource>().isPlaying && !loopSFX.GetComponent<AudioSource>().isPlaying)
                {
                    loopSFX.GetComponent<AudioSource>().Play();
                }
                var transform1 = usedWand.flyDirRef.transform;
                var position1 = transform1.position;
                pos0.transform.position = position1;
                var forward = transform1.forward;
                pos1.transform.position = position1 + (forward * (followTransform.position - position1).magnitude * 0.33f);
                pos2.transform.position = position1 + (forward * (followTransform.position - position1).magnitude * 0.66f);
                pos3.position = followTransform.position;
                usedWand.Haptic(1f);
            }

            if (effectHappening && hitCreature)
            {
                if (effectData == null)
                {
                    effectData = Catalog.GetData<EffectData>("ImbueCrucioRagdoll");
                    if (effectData.modules[0] is EffectModuleParticle particleEffectData)
                    {
                        foreach (var child in particleEffectData.effectParticlePrefab
                                     .GetComponentsInChildren<EffectParticleChild>())
                        {
                            Debug.Log("Child: " + child.name);
                            child.linkBaseColor = EffectTarget.Main;
                            child.linkStartGradient = EffectTarget.Main;
                            child.linkEmissionColor = EffectTarget.Main;
                            child.linkStartColor = EffectTarget.Main;
                            child.linkTintColor = EffectTarget.Main;
                            child.useRenderer = EffectTarget.Main;
                        }
                        particleEffectData.effectParticlePrefab.SetVariation(1f);
                    }
                    hitCreature.ragdoll.SetState(Ragdoll.State.Destabilized);
                    if(!hitCreature.brain.isElectrocuted) hitCreature.TryElectrocute(1, 2, false, false, effectData);
                    if(!hitCreature.isPlayer && !hitCreature.isKilled) hitCreature.brain.instance.GetModule<BrainModuleFear>().Panic();
                }
                else
                {
                    if (effectData.modules[0] is EffectModuleParticle particleEffectData)
                    {
                        foreach (var child in particleEffectData.effectParticlePrefab
                                     .GetComponentsInChildren<EffectParticleChild>())
                        {
                            if (particleEffectData.effectParticlePrefab.GetComponentsInChildren<EffectParticleChild>()
                                    .ToList().IndexOf(child) != 0)
                            {
                                Debug.Log("Child: " + child.name);
                                child.linkBaseColor = EffectTarget.Main;
                                child.linkStartGradient = EffectTarget.Main;
                                child.linkEmissionColor = EffectTarget.Main;
                                child.linkStartColor = EffectTarget.Main;
                                child.linkTintColor = EffectTarget.Main;
                                child.useRenderer = EffectTarget.Main;
                            }
                        }
                        particleEffectData.effectParticlePrefab.SetVariation(10f);
                    }
                    hitCreature.ragdoll.SetState(Ragdoll.State.Destabilized);
                    if(!hitCreature.brain.isElectrocuted) hitCreature.TryElectrocute(1, 2, false, false, effectData);
                    if (IsMovingTowards(usedWand.physicBody.rigidBody, hitCreature.ragdoll.targetPart.transform.position, 1f) && Player.currentCreature.locomotion.velocity.magnitude < 1f)
                    {
                        TriggerFearResponse(hitCreature);
                    }
                }
                
            }
        }
        private void TriggerFearResponse(Creature creature)
        {
            var brainModuleFear = creature.brain.instance.GetModule<BrainModuleFear>();
            brainModuleFear.endPanicBrainActiveTime = 1f;
            if(!brainModuleFear.isCowering) brainModuleFear.Panic();
        }
        bool IsMovingTowards(Rigidbody rb, Vector3 targetPosition, float tolerance)
        {
            // Calculate the direction from the Rigidbody's position to the target position
            Vector3 directionToTarget = (targetPosition - rb.position).normalized;

            // Calculate the dot product of the velocity direction and the direction to the target
            float velocityInDirection = Vector3.Dot(rb.velocity, directionToTarget);

            // Check if the velocity is aligned with the direction to the target within tolerance
            return velocityInDirection >= (1f - tolerance);
        }
    }
}