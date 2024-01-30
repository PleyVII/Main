using System;
public enum EnumType {
    StatId,
    SizeId,
    RaceId,
    RankId,
    ElementId,
    EffectId,
    ConditionalId,
}

public enum StatId {
    Str,
    Agi,
    Vit,
    Crit,
    Spec,
    Int,
    Dex,
    Atk,
    Matk,
    Hit,
    CritChance,
    Def,
    SoftDef,
    MDef,
    SoftMDef,
    Flee,
    Aspd,
    SPoints,
    MaxMana,
    ManaRegen,
    MaxHealth,
    HealthRegen,
    WeaponAtk,
    WeaponRange
}

public enum RaceId {
    Angel,
    Brute,
    Human,
    Demon,
    Dragon,
    Fish,
    Formless,
    Insect,
    Plant,
    Undead
}

public enum SizeId {
    Large,
    Medium,
    Small
}

public enum ElementId {
    Neutral,
    Water,
    Earth,
    Fire,
    Wind,
    Holy,
    Shadow,
    Ghost,
    Undead
}

public enum EffectId {
    Stun,
    Freeze,
    Silence,
    Blind,
    Curse,
    LexAeterna,
    Poison,
    Sleep,
    SlowCasting,
    Provoke
}

public enum RankId {


    Normal,
    Miniboss,
    Boss
}

public enum ConditionalId {
    AllSources,
    Critical,
    Melee,
    Frontstab,
    MeleeFrontstab,
    RangedFrontstab,
    Backstab,
    MeleeBackstab,
    RangedBackstab,
    Poison,
    Attacking,
    Casting,
    Stun,
    Freeze,
    Silence,
    Blind,
    Root,
    Sleep,
    Trapped,
    FastCasting,
    SlowCasting,
    Provoke,
    LexAeterna,
    Stealth,
    Combo,
    LinkGiver,
    LinkReciever,
    LowLife,
    FullLife,
    PostCurse,
}

public enum CharacterClass {
    None, Novice, Swordsman, Mage, Chemist, Acolyte, Thief, Archer, Kingsguard, RoyalTemplar, Guardian, Archimage, Scholar, Enchanter, Creator, Blacksmith, Zoologist,
    Aun, Bishop, Occultist, Assassin, Scoundrel, Reaper, Sniper, Bard, Hunter
}

public enum EquipmentSlot {
    Headgear, Armor, Garment, Handwear, Footgear, Righthand, Lefthand, Necklace, Ring1, Ring2
}

public enum WeaponType {
    Sword,
    Dagger,
    TwoHandedSword,
    Spear,
    TwoHandedSpear,
    Axe,
    TwoHandedAxe,
    Mace,
    Staff,
    Bow,
    MusicalInstrument,
    Whip,
    Book,
    Shuriken
}

[Flags]
public enum Status {
    NoStatus = 0,
    Poison = 1 << 0,
    FastCasting = 1 << 1,
    SlowCasting = 1 << 2,
    Provoke = 1 << 3,
    LexAeterna = 1 << 4,
    Blind = 1 << 5,
    Stealth = 1 << 6,
    Combo = 1 << 7,
    LinkGiver = 1 << 8,
    LinkReciever = 1 << 9,
    LowLife = 1 << 10,
    FullLife = 1 << 11,
}

[Flags]
public enum State {
    CanDoAction = 0,
    Moving = 1 << 0,
    AutoAttacking = 1 << 1,
    Attacking = 1 << 2,
    Casting = 1 << 3,
    Charging = 1 << 4,
    Stun = 1 << 5,
    Freeze = 1 << 6,
    Silence = 1 << 7,
    Root = 1 << 8,
    Sleep = 1 << 9,
    Trapped = 1 << 10,
    PreCurse = 1 << 11,
    PostCurse = 1 << 12,



    // Combined states for convenience
    MovingOrAttackingx2OrCastingOrCharging = Moving | AutoAttacking | Attacking | Casting | Charging,
    Attackingx2OrCastingOrCharging = AutoAttacking | Attacking | Casting | Charging,
    AttackingOrCastingOrCharging = Attacking | Casting | Charging,
    CastingOrCharging = Casting | Charging,
    PreventsMovement = Stun | Freeze | Sleep | Root | Trapped | PostCurse, // Add others as needed
    PreventsCasting = Stun | Freeze | Sleep | Silence | PreCurse | PostCurse, // Add others as needed
    PreventsAttack = Stun | Freeze | Sleep | PreCurse | PostCurse, // Add others as needed
    CanMove = ~PreventsMovement, // Represents being able to move but for some reason you are not supposed to use it for getting actual Can Move
    CanCast = ~PreventsCasting, // Represents being able to cast but for some reason you are not supposed to use it for getting actual Can Cast outside
    CanAttack = ~PreventsAttack, // Represents being able to attack but for some reason you are not supposed to use it for getting actual Can Attack outside
}

public enum TimerType_Skill {
    NotCastableOrChargeable,
    Castable,
    Chargable,
}
public enum TargetingType_Skill {
    TargetedAtGround,
    TargetedAtCharacter,
    NonTargeted,
}

public class UniversalEnum {
    public EnumType Type { get; private set; }
    private Enum value;

    public UniversalEnum(EnumType type, Enum value) {
        Type = type;
        this.value = value;
    }

    public Type GetValue<Type>() where Type : Enum {
        if (typeof(Type).Name != this.Type.ToString()) {
            throw new InvalidOperationException($"Current type is {this.Type}, not {typeof(Type).Name}.");
        }
        return (Type)value;
    }

    public void SetValue<Type>(Type newValue) where Type : Enum {
        this.Type = (EnumType)Enum.Parse(typeof(EnumType), typeof(Type).Name);
        value = newValue;
    }
}