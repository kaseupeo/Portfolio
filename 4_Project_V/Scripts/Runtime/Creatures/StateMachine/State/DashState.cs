public class DashState : State<Creature>
{
    private PlayerController _playerController;

    protected override void Init()
    {
        _playerController = Creature.GetComponent<PlayerController>();
    }

    public override void Enter()
    {
        if (_playerController)
            _playerController.enabled = false;
    }

    public override void Exit()
    {
        if (_playerController)
            _playerController.enabled = true;
    }
}