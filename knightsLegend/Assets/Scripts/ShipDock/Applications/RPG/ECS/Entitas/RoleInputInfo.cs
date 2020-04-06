﻿using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    public class RoleEntitasCallbacker
    {
        public int called;
        public Action callbacker;
    }

    public interface IForceMover
    {
        Vector3 GetMoverVector();
    }

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
        private KeyValueList<string, bool> mInputKeys;
        private RoleEntitasCallbacker mEntitasCallbacker;
        private KeyValueList<int, RoleEntitasCallbacker> mEntitasCallbackers;

        public RoleInputInfo(ICommonRole roleEntitas)
        {
            mInputKeys = new KeyValueList<string, bool>();
            RoleEntitas = roleEntitas;
        }

        public void AddEntitasCallback(int phaseName, Action callback)
        {
            if(mEntitasCallbackers == default)
            {
                mEntitasCallbackers = new KeyValueList<int, RoleEntitasCallbacker>();
            }
            RoleEntitasCallbacker item;
            if (mEntitasCallbackers.IsContainsKey(phaseName))
            {
                item = mEntitasCallbackers[phaseName];
            }
            else
            {
                item = new RoleEntitasCallbacker();
                mEntitasCallbackers[phaseName] = item;
            }
            item.callbacker += callback;
            Debug.Log(RoleEntitas + " " + phaseName);
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
            bool isNameGrounded = animatorInfo.IsMovementBlendTree;
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

        public void AdvancedInputPhase(int rolePhase, int allowCalled)
        {
            if(mEntitasCallbackers != default)
            {
                mEntitasCallbacker = mEntitasCallbackers[rolePhase];
                if(mEntitasCallbacker != default)
                {
                    if(allowCalled == 0)
                    {
                        mEntitasCallbacker.callbacker?.Invoke();
                    }
                    else
                    {
                        if (mEntitasCallbacker.called < allowCalled)
                        {
                            mEntitasCallbacker.callbacker?.Invoke();
                            mEntitasCallbacker.called++;
                        }
                    }
                }
            }
        }

        public void ExecuteBySceneComponent(ref Action sceneCompMethod, int calledMustValue = 1)
        {
            if (mEntitasCallbackers != default)
            {
                mEntitasCallbacker = mEntitasCallbackers[RoleInputPhase];
                if(mEntitasCallbacker != default)
                {
                    if(mEntitasCallbacker.called >= calledMustValue)
                    {
                        sceneCompMethod?.Invoke();
                        mEntitasCallbacker.called = 0;
                    }
                    else
                    {
                        sceneCompMethod?.Invoke();
                    }
                }
                else
                {
                    sceneCompMethod?.Invoke();
                }
            }
        }

        public void ResetEntitasCalled(int phaseName)
        {
            if (mEntitasCallbackers != default)
            {
                mEntitasCallbacker = mEntitasCallbackers[RoleInputPhase];
                if (mEntitasCallbacker != default)
                {
                    mEntitasCallbacker.called = 0;
                }
            }
        }

        public void ResetMovePhase()
        {
            RoleInputPhase = 0;
        }

        public void SetDeltaTime(float time)
        {
            deltaTime = time;
        }

        public void SetUserInputValue(string key, bool value)
        {
            mInputKeys[key] = value;
        }

        public bool GetUserInputValue(string key)
        {
            return mInputKeys[key];
        }

        public void SetInputPhase(int phaseName, bool isCheckFullPhase = true)
        {
            if(isCheckFullPhase && FullRoleInputPhases.IndexOf(RoleInputPhase) >= 0)
            {
                return;
            }
            RoleInputPhase = phaseName;
        }

        public void NextPhase()
        {
            RoleInputPhase++;
        }

        public void AddForceMove(IForceMover mover)
        {
            ForceMove += mover.GetMoverVector();
        }

        private Queue<int> PhaseWillSet { get; set; } = new Queue<int>();

        public List<int> FullRoleInputPhases { get; set; }
        public bool ShouldGetUserInput { get; set; }
        public int RoleInputType { get; set; } = 0;
        public int RoleInputPhase { get; private set; } = UserInputPhases.ROLE_INPUT_PHASE_NONE;
        public float TurnAmount { get; private set; }
        public float ForwardAmount { get; private set; }
        public float ExtraTurnRotationOut { get; private set; }
        public Vector3 ExtraGravityForceOut { get; private set; }
        public ICommonRole RoleEntitas { get; private set; }
        public Vector3 ForceMove { get; private set; }
    }

}


