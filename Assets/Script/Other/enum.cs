

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
    Recyclable,
    Energy,
    CannotRecycle
}


public enum ItemType
{
    // small item
    Apple,
    PaperBall,
    Balloon,
    Energy,
    GlassBottle,
    MachineParts,
    RadioactiveSubstances,
    // medium item 
    HighTechComponents,
    CannedApple,
    // large item
    SpaceshipIronPlate,
    DrinkCan,
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
    PlayerLanding,
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


public enum PairUnitState
{
    Unpair,
    Paired,
    Ready,
    FinalCheck,
}