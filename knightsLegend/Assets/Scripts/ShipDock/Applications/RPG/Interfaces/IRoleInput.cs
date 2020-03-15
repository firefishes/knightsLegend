using UnityEngine;

namespace ShipDock.Applications
{
    public interface IRoleInput
    {
        IUserInputPhase GetUserInputPhase();
        void SetUserInputValue(Vector3 value);
        void SetUserInputValue(string key, bool value);
        bool GetUserInputValue(string key);
        void SetDeltaTime(float time);
        void SetCrouching(bool flag);
        void SetMoveValue(Vector3 value);
        void MoveValueNormalize();
        void AdvancedInputPhase();
        void UpdateAmout(ref ICommonRole roleEntitas);
        void HandleAirborneMovement(ref IRoleData roleData);
        void ScaleCapsuleForCrouching(ref ICommonRole roleEntitas, ref IRoleInput roleInput);
        float UpdateRoleExtraTurnRotation(ref IRoleData roleData);
        bool HandleGroundedMovement(ref IRoleInput input, ref CommonRoleAnimatorInfo animatorInfo);
        bool ShouldGetUserInput { get; set; }
        bool IsCrouch();
        bool IsCrouching();
        bool IsJump();
        int RoleMovePhase { get; }
        float ExtraTurnRotationOut { get; }
        float ForwardAmount { get; }
        float TurnAmount { get; }
        Vector3 GetUserInputValue();
        Vector3 GetMoveValue();
        Vector3 ExtraGravityForceOut { get; }
    }
}
