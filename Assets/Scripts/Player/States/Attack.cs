namespace Player.States
{
public class Attack: PlayerState
{
    public Attack(PlayerController p): base(p) { }

    public override void Enter() {

    }
}

public enum AttackType {
    LeftJab,
    RightCross,
    Uppercut,
    JumpKick
}
}