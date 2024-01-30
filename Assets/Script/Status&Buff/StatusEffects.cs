using System.Collections.Generic;
public class StunEffect : Effect {
    public override EffectId Id => EffectId.Stun;

    public override void ActivateEffect(Dictionary<AllObjectInformation, List<Effect>> dictionary) {
        if (Receiver.CurrentState.HasFlag(State.Stun)) {
            RemovesItselfFromTheDictionary(dictionary); return;
        } else {
            Receiver.ApplyState(State.Stun);
            Duration = Duration * (1 - ((1.5f * ReceiverEffectPercentReduction) + Receiver.Stats.Vit - (1.5f * AttackerIncreasedEffect)) / 150);
            CurrentDuration = Duration;
        }
    }

    public override void DeactivateEffect(Dictionary<AllObjectInformation, List<Effect>> dictionary) {
        if (Receiver.CurrentState.HasFlag(State.Stun)) Receiver.RemoveState(State.Stun);
    }
}

public class FreezeEffect : Effect {
    public override EffectId Id => EffectId.Freeze;

    public override void ActivateEffect(Dictionary<AllObjectInformation, List<Effect>> dictionary) {
        if (Receiver.CurrentState.HasFlag(State.Freeze)) {
            RemovesItselfFromTheDictionary(dictionary); return;
        } else if (Receiver.BodyElement != ElementId.Undead || Receiver.BodyElement != ElementId.Water) {
            EffectsManager.Instance.RemoveSpecificEffect<CurseEffect>(Receiver);
            Receiver.ApplyState(State.Freeze);
            Receiver.ChangeBodyArmorElementFromStatus(ElementId.Water);
            Duration = Duration * AttackerIncreasedEffect * ReceiverEffectPercentReduction;
            CurrentDuration = Duration;
            Receiver.OnBeingHit += DeactivateDelegate;
        }
    }

    public override void DeactivateEffect(Dictionary<AllObjectInformation, List<Effect>> dictionary) {
        Receiver.RemoveState(State.Freeze);
        Receiver.ChangeBodyArmorElementFromStatus(null);
        Receiver.OnBeingHit -= DeactivateDelegate;
    }
}

public class BlindEffect : Effect {
    public override EffectId Id => EffectId.Blind;

    public override void ActivateEffect(Dictionary<AllObjectInformation, List<Effect>> dictionary) {
        Receiver.CurrentStatus |= Status.Blind;
        Duration = Duration * AttackerIncreasedEffect * ReceiverEffectPercentReduction;
        CurrentDuration = Duration;
    }

    public override void DeactivateEffect(Dictionary<AllObjectInformation, List<Effect>> dictionary) {
        Receiver.CurrentStatus &= ~Status.Blind;
    }
}

public class CurseEffect : Effect {
    public override EffectId Id => EffectId.Curse;

    public override void ActivateEffect(Dictionary<AllObjectInformation, List<Effect>> dictionary) {
        if (Receiver.CurrentState.HasFlag(State.PreCurse) || Receiver.CurrentState.HasFlag(State.PostCurse)) {
            RemovesItselfFromTheDictionary(dictionary);
            return;
        } else if (Receiver.BodyElement != ElementId.Undead || Receiver.BodyElement != ElementId.Water) {
            EffectsManager.Instance.RemoveSpecificEffect<FreezeEffect>(Receiver);
            Receiver.ApplyState(State.PreCurse);
            CurrentDuration = 10f;
        }
    }

    protected override void CustomDurationEffect() {
        if (CurrentDuration <= 5f) {
            Receiver.ApplyState(State.PostCurse);
            Receiver.ChangeBodyArmorElementFromStatus(ElementId.Earth);
            Receiver.OnBeingHit += DeactivateDelegate;
            IsCustomDurationEffectUsed = true;
            Duration *= AttackerIncreasedEffect * ReceiverEffectPercentReduction;
            CurrentDuration = Duration;
        }
    }

    public override void DeactivateEffect(Dictionary<AllObjectInformation, List<Effect>> dictionary) {
        if (Receiver.CurrentState.HasFlag(State.PreCurse)) Receiver.RemoveState(State.PreCurse);
        if (Receiver.CurrentState.HasFlag(State.PostCurse)) {
            Receiver.RemoveState(State.PostCurse);
            Receiver.ChangeBodyArmorElementFromStatus(null);
        }
        Receiver.OnBeingHit -= DeactivateDelegate;
    }
}