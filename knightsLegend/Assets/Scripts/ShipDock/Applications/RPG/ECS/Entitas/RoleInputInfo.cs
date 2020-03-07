using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    [Serializable]
    public class RoleInputInfo : IRoleInput
    {
        public bool jump;
        public bool crouch;
        public bool crouching;
        public float deltaTime;
        public float turnSpeed;
        public Vector3 move;
        public Vector3 userInput;

        private float mUserInputTime;
        private List<IUserInputPhase> mInputPhases;

        public RoleInputInfo(ICommonRole roleEntitas)
        {
            mInputPhases = new List<IUserInputPhase>()
            {
                new RoleMovePhase(),
                new RoleAmoutExtranTurn(roleEntitas),
                new RoleInputDefaultPhase(),
                new RoleScaleCapsulePhase(roleEntitas),
                new RoleInputDefaultPhase()
            };
            int max = mInputPhases.Count;
            for (int i = 0; i < max; i++)
            {
                mInputPhases[i].SetRoleInput(this);
            }
        }

        public void UpdateAmout(ref ICommonRole roleEntitas)
        {
            move = Vector3.ProjectOnPlane(move, roleEntitas.GroundNormal);
            TurnAmount = Mathf.Atan2(move.x, move.z);
            ForwardAmount = move.z;
        }

        public float UpdateRoleExtraTurnRotation(ref IRoleData roleData)
        {
            turnSpeed = Mathf.Lerp(roleData.StationaryTurnSpeed, roleData.MovingTurnSpeed, ForwardAmount);
            ExtraTurnRotationOut = TurnAmount * turnSpeed * deltaTime;
            return ExtraTurnRotationOut;
        }

        public void HandleAirborneMovement(ref IRoleData roleData)
        {
            ExtraGravityForceOut = (Physics.gravity * roleData.GravityMultiplier) - Physics.gravity;
        }

        public bool HandleGroundedMovement(ref IRoleInput input, ref CommonRoleAnimatorInfo animatorInfo)
        {
            // check whether conditions are right to allow a jump:
            bool isNameGrounded = animatorInfo.IsMainBlendTree;
            bool result = input.IsJump() && !input.IsCrouch() && isNameGrounded;
            return result;
        }

        public void ScaleCapsuleForCrouching(ref ICommonRole roleEntitas, ref IRoleInput roleInput)
        {
            roleEntitas.IsGroundedAndCrouch = roleEntitas.IsGrounded && roleInput.IsCrouch();
            if (roleEntitas.IsGroundedAndCrouch)
            {
                if (roleInput.IsCrouching())
                {
                    return;
                }
                CommonRoleMustSubgroup subgroup = roleEntitas.RoleMustSubgroup;
                subgroup.capsuleHeight /= 2f;
                subgroup.capsuleCenter /= 2f;
                roleEntitas.RoleMustSubgroup = subgroup;
            }
        }

        public Vector3 GetUserInputValue()
        {
            return userInput;
        }

        public void SetUserInputValue(Vector3 value)
        {
            userInput = value;
        }

        public bool IsCrouch()
        {
            return crouch;
        }

        public bool IsCrouching()
        {
            return crouching;
        }

        public void SetCrouching(bool flag)
        {
            crouching = flag;
        }

        public void SetMoveValue(Vector3 value)
        {
            move = value;
        }

        public Vector3 GetMoveValue()
        {
            return move;
        }

        public void MoveValueNormalize()
        {
            move.Normalize();
        }

        public bool IsJump()
        {
            return jump;
        }

        public void AdvancedInputPhase()
        {
            RoleMovePhase++;
            if(RoleMovePhase >= 5)
            {
                RoleMovePhase = 0;
            }
        }

        public void ResetMovePhase()
        {
            RoleMovePhase = 0;
        }

        public void SetDeltaTime(float time)
        {
            deltaTime = time;
        }

        public IUserInputPhase GetUserInputPhase()
        {
            return (RoleMovePhase >= 0) && (RoleMovePhase < mInputPhases.Count) ? mInputPhases[RoleMovePhase] : default;
        }

        public bool ShouldGetUserInput { get; set; }
        public int RoleMovePhase { get; private set; }
        public float TurnAmount { get; private set; }
        public float ForwardAmount { get; private set; }
        public float ExtraTurnRotationOut { get; private set; }
        public Vector3 ExtraGravityForceOut { get; private set; }
    }

}


