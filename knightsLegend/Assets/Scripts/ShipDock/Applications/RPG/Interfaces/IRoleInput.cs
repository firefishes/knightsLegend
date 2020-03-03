using UnityEngine;

namespace ShipDock.Applications
{
    public interface IRoleInput
    {
        void SetUserInputValue(Vector3 value);
        Vector3 GetUserInputValue();
        Vector3 GetMoveValue();
        void SetDeltaTime(float time);
        void SetCrouching(bool flag);
        void SetMoveValue(Vector3 value);
        void MoveValueNormalize();
        bool IsCrouch();
        bool IsCrouching();
        bool IsJump();
        float UpdateRoleExtraTurnRotation(ref IRoleData roleData);
        void ScaleCapsuleForCrouching(ref ICommonRole roleEntitas, ref IRoleInput roleInput);
        bool HandleGroundedMovement(ref IRoleInput input, ref CommonRoleAnimatorInfo animatorInfo);
        void HandleAirborneMovement(ref IRoleData roleData);
        void UpdateAmout(ref ICommonRole roleEntitas);
        void UpdateMovePhase();
        int RoleMovePhase { get; }
        float ExtraTurnRotationOut { get; }
        float ForwardAmount { get; }
        float TurnAmount { get; }
        Vector3 ExtraGravityForceOut { get; }
    }
}
