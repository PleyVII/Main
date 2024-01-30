using System.Collections.Generic;

public interface IEffect {
    EffectId Id { get; }
    //Just here for a reminder of overriding those methods in ever subclass
    public void ActivateEffect(Dictionary<AllObjectInformation, List<Effect>> dictionary);
    public void DeactivateEffect(Dictionary<AllObjectInformation, List<Effect>> dictionary);
}