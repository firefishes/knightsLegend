using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    public interface IRPGRoleInput : IRoleInput
    {
        void AddEntitasCallback(int phaseName, Action callback);
        void ActiveEntitasPhase(int phaseName, bool isActive);
        void SetInputPhase(int phaseName, bool isCheckFullPhase = true);
        void ResetEntitasCalled(int phaseName);
        void AdvancedInputPhase(int rolePhase, int allowCalled);
        void ExecuteBySceneComponent(ref Action sceneCompMethod, int calledMustValue = 1);
        void UpdateAmout(ICommonRole roleEntitas);
        void HandleAirborneMovement(ref IRoleData roleData);
        void ScaleCapsuleForCrouching(ICommonRole roleEntitas, ref IRoleInput roleInput);
        float UpdateRoleExtraTurnRotation(ref IRoleData roleData);
        bool HandleGroundedMovement(ref IRoleInput input, ref CommonRoleAnimatorInfo animatorInfo);
        int RoleInputPhase { get; }
        float ExtraTurnRotationRef { get; }
        float ForwardAmount { get; }
        float TurnAmount { get; }
        Vector3 ExtraGravityForceRef { get; }
    }

    public interface IRoleInput
    {
        void SetUserInputValue(Vector3 value);
        void SetUserInputValue(string key, bool value);
        bool GetUserInputValue(string key);
        void SetDeltaTime(float time);
        void SetMoveValue(Vector3 value);
        void MoveValueNormalize();
        bool ShouldGetUserInput { get; set; }
        bool IsCrouch();
        bool IsCrouching();
        void SetCrouching(bool flag);
        bool IsJump();
        void AddForceMove(IForceMover mover);
        List<int> FullRoleInputPhases { get; set; }
        int RoleInputType { get; set; }
        Vector3 GetUserInputValue();
        Vector3 GetMoveValue();
        ICommonRole RoleEntitas { get; }
        Vector3 ForceMove { get; }
    }
}
