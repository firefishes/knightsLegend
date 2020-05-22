#define G_LOG

using ShipDock.Pooling;
using ShipDock.Testers;
using UnityEngine;

namespace KLGame
{
    public class PlayerHit : ProcessHit
    {

        public PlayerHit() : base()
        {
        }

        public override void ToPool()
        {
            Pooling<PlayerHit>.To(this);
        }

        public override void Reinit(IKLRole initiator)
        {
            base.Reinit(initiator);

            PlayerRole = Initiator;
        }

        public override void Revert()
        {
            base.Revert();

            PlayerRole = default;
        }

        public override void OnProcessing()
        {
            if (PlayerRole != default)
            {
                if (mEnemyRole != default && HitInfoScope.CheckScope(mEnemyRole.Position))
                {
                    Tester.Instance.Log(KLTester.Instance, KLTester.LOG0, mEnemyRole != default, "log: Player attack enemy ".Append(mEnemyRole.Name));
                    
                    IKLRole role = mEnemyRole as IKLRole;
                    BattleUnit data = role.BattleDataUnit;
                    data.UpdateFlaws(10, out int result);//TODO 这里需要再看下如何合理的修改，既要保证正确检测碰撞，又要正确触发AI预判

                    if ((result == 1) || (role.DefenceType == 0f))
                    {
                        role.UnderAttack();
                    }
                    AfterProcessing?.Invoke();
                    PlayerRole.HitSomeOne = true;
                    ForceMover.Create().SetMover(mEnemyRole, new Vector3(PlayerRole.WeapontPos.x, 0, PlayerRole.WeapontPos.z) * 0.4f, 0.1f);
                }
            }
            ToPool();
        }

        public IKLRole PlayerRole { get; set; }
    }

}