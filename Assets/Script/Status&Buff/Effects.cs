using System.Collections.Generic;

public class ProvokeEffect : Effect {
    public override EffectId Id => EffectId.Provoke;

    public override void ActivateEffect(Dictionary<AllObjectInformation, List<Effect>> dictionary) {
        Duration = 20f * AttackerIncreasedEffect * ReceiverEffectPercentReduction;
        CurrentDuration = Duration;
        if (Receiver.CurrentStatus.HasFlag(Status.Provoke)) {
            UpdateEffect<ProvokeEffect>(dictionary, Receiver, CurrentDuration, levelOfAnEffect);
            RemovesItselfFromTheDictionary(dictionary); return;
        } else {
            Receiver.CurrentStatus |= Status.Provoke;
            Receiver.Stats.AddPercentageModifier(-7 * levelOfAnEffect, StatId.Def);
            Receiver.Stats.AddPercentageModifier(3 * levelOfAnEffect, StatId.Atk);
            Receiver.Stats.AddPercentageModifier(3 * levelOfAnEffect, StatId.Matk);
            //Adds some agro points //TODO: in the future
        }
    }

    public override void DeactivateEffect(Dictionary<AllObjectInformation, List<Effect>> dictionary) {
        Receiver.CurrentStatus &= ~Status.Provoke;
        Receiver.Stats.RemovePercentageModifier(-7 * levelOfAnEffect, StatId.Def);
        Receiver.Stats.RemovePercentageModifier(3 * levelOfAnEffect, StatId.Atk);
        Receiver.Stats.RemovePercentageModifier(3 * levelOfAnEffect, StatId.Matk);
    }
}