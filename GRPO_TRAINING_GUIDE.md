# GRPO 训练数据格式说明

## 概述

本项目现在支持为 GRPO (Group Relative Policy Optimization) 训练收集数据。系统会生成两个 JSON 文件：

1. **`game_state.json`** - 当前游戏状态的实时快照（用于 LLM 推理）
2. **`game_history.json`** - 完整的游戏历史记录（用于 GRPO 训练）

## 数据格式

### game_state.json

这是当前游戏状态的快照，格式已经优化为 LLM-friendly：

```json
{
  "game_state": {
    "turn_number": 3,
    "phase": "PlayerTurn"
  },
  "player_hero": {
    "name": "Edwin VanCleef",
    "current_health": 28,
    "hero_power": {
      "description": "...",
      "cost": 1
    }
  },
  "resources": {
    "available_gold": 5,
    "tavern_tier": 2,
    "tavern_upgrade_cost": "7"
  },
  "board_state": {
    "warband_size": 3,
    "warband": [...],
    "hand_size": 0,
    "hand": [],
    "tavern_available": 3,
    "tavern_entities": [...]
  }
}
```

### game_history.json

这是完整的游戏历史，适合 GRPO 训练：

```json
{
  "game_metadata": {
    "hero_name": "Edwin VanCleef",
    "start_time": 1234567890,
    "end_time": 1234568000,
    "total_turns": 15,
    "final_rank": null
  },
  "turns": [
    {
      "turn_number": 1,
      "phase": "PlayerTurn",
      "state": { /* 完整的游戏状态 JSON */ },
      "action_taken": "BUY Alleycat",
      "battle_result": "Win",
      "health_before_battle": 30,
      "health_after_battle": 30,
      "health_change": 0,
      "reward": 11.0,
      "timestamp": 1234567891
    },
    ...
  ]
}
```

## 字段说明

### Turn History 字段

- **`turn_number`**: 回合编号（从 1 开始）
- **`phase`**: 游戏阶段（StartGame, PlayerTurn, OpponentTurn, EndGame）
- **`state`**: 该回合的完整游戏状态（与 game_state.json 格式相同）
- **`action_taken`**: LLM 建议的动作（需要从 Python agent 中提取）
- **`battle_result`**: 战斗结果（"Win", "Loss", "Tie", "None"）
- **`health_before_battle`**: 战斗前生命值
- **`health_after_battle`**: 战斗后生命值
- **`health_change`**: 生命值变化
- **`reward`**: 计算的奖励值（用于 GRPO 训练）
- **`timestamp`**: Unix 时间戳

### 奖励计算

当前奖励计算公式：

```python
reward = base_reward + health_change * 0.5 + survival_bonus

其中：
- Win: +10.0
- Loss: -5.0
- Tie: +2.0
- 生命值变化: health_change * 0.5
- 存活奖励: +1.0 (如果 health > 0)
```

你可以根据训练效果调整这些权重。

## 如何使用数据进行 GRPO 训练

### 1. 数据收集流程

1. **游戏进行时**：
   - `game_state.json` 实时更新，Python agent 读取并生成动作建议
   - 每个回合开始时，系统记录上一回合的历史数据

2. **游戏结束时**：
   - `game_history.json` 自动保存，包含完整的游戏轨迹

### 2. 动作提取

**重要**：`action_taken` 字段需要从 Python agent 的输出中提取。

建议修改 Python agent (`Openai_caller.py` 或 `Gemma_caller.py`)：

```python
# 在生成动作后，将动作写入一个临时文件
action_file = "last_action.txt"
with open(action_file, 'w') as f:
    f.write(action_text)

# C# 插件可以读取这个文件并更新 game_history.json
```

或者更好的方式：Python agent 直接更新 `game_history.json` 中的 `action_taken` 字段。

### 3. GRPO 训练数据准备

将多个游戏的 `game_history.json` 合并成训练数据集：

```python
import json
import glob

# 收集所有游戏历史
game_files = glob.glob("Agent/game_history_*.json")  # 如果重命名的话
# 或者使用单个文件（每次游戏覆盖）

# 转换为 GRPO 训练格式
training_data = []
for game_file in game_files:
    with open(game_file, 'r') as f:
        game_data = json.load(f)
        
    # 提取状态-动作-奖励序列
    for turn in game_data['turns']:
        training_data.append({
            'state': turn['state'],
            'action': turn['action_taken'],
            'reward': turn['reward'],
            'next_state': None  # 可以从下一个 turn 获取
        })
```

### 4. GRPO 训练建议

#### 状态表示

- **当前格式**：JSON 对象，包含所有游戏信息
- **建议**：可以转换为向量表示（embedding）或特征向量
- **考虑**：使用状态哈希或简化表示来减少维度

#### 动作空间

当前动作类型：
- `BUY <minion_name>`
- `SELL <minion_name>`
- `UPGRADE`
- `ROLL`
- `FREEZE`
- `HEROPOWER`

**建议**：
- 将动作标准化为固定格式
- 考虑动作参数（如购买哪个随从）
- 可能需要将动作编码为离散动作空间

#### 奖励设计

当前奖励基于：
- 战斗结果（Win/Loss/Tie）
- 生命值变化
- 存活状态

**可以改进**：
- 添加最终排名奖励（如果可以获得）
- 考虑长期奖励（discounted future rewards）
- 添加探索奖励（鼓励尝试新策略）

#### 训练流程

1. **收集数据**：玩多局游戏，收集 `game_history.json`
2. **数据预处理**：
   - 提取状态-动作-奖励序列
   - 标准化状态表示
   - 编码动作空间
3. **GRPO 训练**：
   - 使用收集的数据训练策略模型
   - 可以结合在线学习和离线学习
4. **评估**：
   - 测试训练后的模型
   - 收集新的游戏数据
   - 迭代改进

### 5. 改进建议

#### 短期改进

1. **动作提取**：实现 Python agent 与 C# 插件之间的通信机制
2. **最终排名**：尝试从 HDT API 获取最终排名信息
3. **数据验证**：添加数据完整性检查

#### 长期改进

1. **状态压缩**：使用特征工程或神经网络压缩状态表示
2. **动作标准化**：定义标准化的动作格式和验证
3. **奖励调优**：根据训练效果调整奖励函数
4. **多游戏数据**：支持批量收集和处理多个游戏的数据

## 示例代码

### Python: 更新动作到历史文件

```python
import json

def update_action_in_history(action_text, history_file="game_history.json"):
    try:
        with open(history_file, 'r') as f:
            history = json.load(f)
        
        # 找到最后一个未填充动作的回合
        for turn in reversed(history['turns']):
            if not turn.get('action_taken'):
                turn['action_taken'] = action_text
                break
        
        with open(history_file, 'w') as f:
            json.dump(history, f, indent=2)
    except Exception as e:
        print(f"Error updating action: {e}")
```

### Python: 转换为 GRPO 训练格式

```python
def prepare_grpo_data(history_file):
    with open(history_file, 'r') as f:
        game_data = json.load(f)
    
    episodes = []
    current_episode = []
    
    for i, turn in enumerate(game_data['turns']):
        state = turn['state']
        action = turn['action_taken']
        reward = turn['reward']
        
        # 获取下一个状态（如果存在）
        next_state = None
        if i + 1 < len(game_data['turns']):
            next_state = game_data['turns'][i + 1]['state']
        
        current_episode.append({
            'state': state,
            'action': action,
            'reward': reward,
            'next_state': next_state,
            'done': (next_state is None)
        })
    
    return current_episode
```

## 注意事项

1. **数据完整性**：确保 `action_taken` 字段被正确填充
2. **文件路径**：确保 C# 插件和 Python agent 使用相同的文件路径
3. **并发访问**：注意文件读写时的并发问题
4. **数据量**：GRPO 训练通常需要大量数据，建议收集至少数百局游戏的数据

## 下一步

1. 实现动作提取机制
2. 收集初始训练数据
3. 设计并实现 GRPO 训练脚本
4. 迭代改进模型和奖励函数

