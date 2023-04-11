

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
    AbandonedSatellite,
    //Add new furniture
    Table,
    Chair,
    BrokenTable1,
    BrokenTable2
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
    PlayerCollision,
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


public enum SceneIndex
{
    MainMenu = 0,
    Tutorial = 1,
    FirstLevel1 = 2,
    None
}