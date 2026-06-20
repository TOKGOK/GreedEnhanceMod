using System.Collections;
using System.Linq;
using UnityEngine;

namespace GreedEnhanceMod
{
    /// <summary>
    /// 贪婪增强 Mod
    /// 增强七宗罪挑战中的贪婪之书（ID 1504）：
    /// 1. 能力选择房间可以全选所有能力（不销毁其他能力）
    /// 2. Boss 房间面对 3 只 BOSS（原版 2 只）
    /// 3. 保留原有的宝箱全拿效果
    /// </summary>
    public class Main : SimpleModBehaviour
    {
        private const string ModVersion = "0.1.5";
        private const int GreedBookID = 1504;
        private const string LogPrefix = "[GreedEnhanceMod]";

        private BattleObject _currentBO;

        public override void OnModLoaded()
        {
            Debug.Log($"{LogPrefix} V{ModVersion} 已加载：增强贪婪之书效果。");
            BattleObject.OnGameStart += OnGameStart;
            BattleObject.OnLevelStart += OnLevelStart;
        }

        /// <summary>
        /// 每帧更新：持续监控能力选择对象，设置 dontDestroyOther
        /// </summary>
        private void Update()
        {
            if (_currentBO == null || !HasGreedBook(_currentBO))
                return;

            if (!IsAbilityRoom(_currentBO.currentRoom))
                return;

            // 每帧检查并设置所有能力对象
            foreach (var obj in _currentBO.chooseObjects)
            {
                if (obj is UnitObjectOther other && !other.hasDead)
                {
                    other.dontDestroyOther = true;
                }
            }
        }

        public override void OnModUnloaded()
        {
            BattleObject.OnGameStart -= OnGameStart;
            BattleObject.OnLevelStart -= OnLevelStart;
            Debug.Log($"{LogPrefix} V{ModVersion} 已卸载。");
        }

        /// <summary>
        /// 游戏开始时：如果携带贪婪之书，增加 moreBoss 使 Boss 数量从 2 变为 3
        /// 贪婪之书本身设置 moreBoss=1（1本体+1克隆=2只）
        /// 我们再 +1（1本体+2克隆=3只）
        /// </summary>
        private void OnGameStart(BattleObject bo)
        {
            _currentBO = bo;

            if (HasGreedBook(bo))
            {
                bo.moreBoss += 1;
                Debug.Log($"{LogPrefix} 贪婪之书激活：Boss 数量 +1（当前 moreBoss={bo.moreBoss}）");
            }
        }

        /// <summary>
        /// 关卡开始时：记录当前 BattleObject 用于 Update 监控
        /// </summary>
        private void OnLevelStart(BattleObject bo)
        {
            _currentBO = bo;
        }

        /// <summary>
        /// 检查是否携带贪婪之书
        /// </summary>
        private bool HasGreedBook(BattleObject bo)
        {
            return bo.currentBookSkill != null &&
                   bo.currentBookSkill.Any(s => s.id == GreedBookID);
        }

        /// <summary>
        /// 判断是否为能力选择房间
        /// 覆盖所有涉及能力选择的房间类型
        /// </summary>
        private bool IsAbilityRoom(RoomType room)
        {
            switch (room)
            {
                // 普通能力选择
                case RoomType.Ability:
                // 稀有能力选择
                case RoomType.RareAbility:
                // 天使能力（R3）
                case RoomType.Angle:
                // 恶魔能力（用HP购买）
                case RoomType.Devil:
                // 护甲能力
                case RoomType.Shield:
                // 魔神真言
                case RoomType.DemonGod:
                // 武器房间
                case RoomType.Weapon:
                // 英雄房间
                case RoomType.Hero:
                // 问号房间（谜题）
                case RoomType.Puzzle:
                // 指定类型能力房间
                case RoomType.Bomb:
                case RoomType.Bat:
                case RoomType.Lightning:
                case RoomType.Spawn:
                case RoomType.Burn:
                case RoomType.Shuriken:
                case RoomType.Prop:
                case RoomType.Forest:
                case RoomType.Invincible:
                case RoomType.Emoji:
                case RoomType.Cannon:
                    return true;
                default:
                    return false;
            }
        }
    }
}
