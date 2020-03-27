using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    public interface IRoleInput
    {
        void AddEntitasCallback(int phaseName, Action callback);
        void SetUserInputValue(Vector3 value);
        void SetUserInputValue(string key, bool value);
        bool GetUserInputValue(string key);
        void SetDeltaTime(float time);
        void SetCrouching(bool flag);
        void SetMoveValue(Vector3 value);
        void MoveValueNormalize();
        void SetInputPhase(int phaseName, bool isCheckFullPhase = true);
        void NextPhase();
        void ResetEntitasCalled(int phaseName);
        void AdvancedInputPhase(int rolePhase, int allowCalled);
        void ExecuteBySceneComponent(ref Action sceneCompMethod, int calledMustValue = 1);
        void UpdateAmout(ref ICommonRole roleEntitas);
        void HandleAirborneMovement(ref IRoleData roleData);
        void ScaleCapsuleForCrouching(ref ICommonRole roleEntitas, ref IRoleInput roleInput);
        float UpdateRoleExtraTurnRotation(ref IRoleData roleData);
        bool HandleGroundedMovement(ref IRoleInput input, ref CommonRoleAnimatorInfo animatorInfo);
        bool ShouldGetUserInput { get; set; }
        bool IsCrouch();
        bool IsCrouching();
        bool IsJump();
        List<int> FullRoleInputPhases { get; set; }
        int RoleInputPhase { get; }
        int RoleInputType { get; set; }
        float ExtraTurnRotationOut { get; }
        float ForwardAmount { get; }
        float TurnAmount { get; }
        Vector3 GetUserInputValue();
        Vector3 GetMoveValue();
        Vector3 ExtraGravityForceOut { get; }
        ICommonRole RoleEntitas { get; }
    }
}
