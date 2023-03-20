

public enum InteractType
{
    Tap,
    Hold,
    Release
}


public enum ItemSize
{
    Small,
    Medium,
    Large,
    ExtraLarge
}


public enum RecycleType
{
    Combustible,
    NonCombustible,
    ToxicSubstances,
    Energy,
    CannotRecycle
}


public enum ItemType
{
    // small item
    Balloon,
    Energy,
    MachineParts,
    SmallMeteorite,
    RadioactiveSubstances,
    // medium item 
    MediumMeteorite,
    HighTechComponents,
    WasteBattery,
    // large item
    DrinkCan,
    SpaceshipIronPlate,
    // extra large item
    SpaceCrap,
    EnemyShipCore,
    AbandonedSatellite
}


public enum MachineStateType
{
    Idle,
    Working,
    Broken,
    NoPower
}


public enum VFXType
{
    Recycle,
}


public enum SFXType
{
    PlayerMove,
    
}


public enum BGMType
{
    ChooseCharacter,
    MainGamePlay,
}