namespace ShipDock.Applications
{
    public static class UserInputPhases
    {
        public const int ROLE_INPUT_PHASE_NONE = -1;
        public const int ROLE_INPUT_PHASE_MOVE_READY = 0;
        public const int ROLE_INPUT_PHASE_AMOUT_EXTRAN_TURN = 1;
        public const int ROLE_INPUT_PHASE_CHECK_GROUND = 2;
        public const int ROLE_INPUT_PHASE_SCALE_CAPSULE = 3;
        public const int ROLE_INPUT_PHASE_CHECK_CROUCH = 4;
        public const int ROLE_INPUT_PHASE_AFTER_MOVE = 5;
        public const int ROLE_INPUT_PHASE_UNDERATTACKED = 100;
    }
}
