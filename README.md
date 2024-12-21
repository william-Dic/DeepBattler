# DeepBattler - Your Ultimate Battlegrounds Coach Powered by LLM! 🍻🍻

| **[English](#english)** | [**中文**](#chinese) | [**日本語**](#japanese)** |
|:-----------------------:|:--------------------:|:------------------------:|


---

## English <a id="english"></a>

### Hey there, Hero! I’m DeepBattler, Your Personal Battlegrounds Coach! 🍻🍻

DeepBattler, the cutting-edge assistant designed specifically for Hearthstone Battlegrounds enthusiasts. Powered by an advanced Large Language Model (LLM), DeepBattler seamlessly integrates with the Hearthstone Deck Tracker (HDT) plugin to provide you with **real-time strategic advice**. Whether you're aiming to climb the ranks or just improve your game, DeepBattler has got your back!

DeepBattler's strength can match that of the **top 0.1% players on EU servers**, thanks to its insightful, voice-assisted guidance that helps you make the best decisions on the fly. Let’s take your gameplay to the next level!

## System Components  

### 1. Hearthstone Deck Tracker (HDT) Plugin  
- **Real-Time Monitoring:** Keeps track of your game state as it happens  
- **JSON Outputs:** Provides clear, structured data  
- **Efficient Data Handling:** Ensures smooth performance  
- **In-Depth Insights:** Offers comprehensive analysis of your gameplay  

### 2. LLM-Powered Python Agent  
- **Advanced Analysis:** Utilizes powerful language model capabilities  
- **Strategic Advice:** Gives you real-time tactical recommendations  
- **Voice Communication:** Interact naturally with voice commands  
- **Adaptive Decisions:** Adjusts strategies based on different game scenarios  

## Setup and Configuration  

### Plugin Setup  
1. Open the `DeepBattlerPlugin/Class1.cs` file.  
2. Set the `_path` variable to your absolute game state file path:  
   ```csharp  
   private readonly string _path = @"C:\Your\Absolute\Path\To\game_state.json";  
   ```  
3. Build the plugin. The compiled `DeepBattlerPlugin.dll` will be located under `DeepBattlerPlugin/bin/Debug`.  
4. Open Hearthstone Deck Tracker (HDT).  
5. Copy the plugin files to the HDT plugins directory:  
   - Default location: `%AppData%\Hearthstone Deck Tracker\Plugins`  
6. Launch Hearthstone Deck Tracker.  
7. Enable the plugin in HDT under `Options -> Plugins`.  

![HDT Plugin Setup](https://github.com/user-attachments/assets/23f41637-d517-4b79-87d5-cc6e5009ac24)

### LLM Agent Setup  
1. Install the required Python packages:  
   ```bash  
   pip install openai playsound==1.2.2  
   ```  
   *Note: Version 1.2.2 of `playsound` is required for compatibility.*  

2. Add your OpenAI API key in `DeepBattler.py`:  
   ```python  
   api_key = "your-openai-api-key-here"  
   ```  

3. Launch the LLM agent:  
   ```bash  
   python DeepBattler.py  
   ```  

---

## Custom Non-Commercial License  

© [2024] [William-Dic]  

You’re free to use, copy, modify, and share this software for personal, educational, or non-commercial purposes. Here’s what you need to know:

1. **Non-Commercial Use**  
   Feel free to use and tweak the software, but don’t sell or distribute it commercially without permission.

2. **Hearthstone Intellectual Property**  
   This tool uses assets from Blizzard Entertainment’s Hearthstone. Make sure to follow Blizzard’s terms when using DeepBattler. This tool isn’t affiliated with or endorsed by Blizzard.

3. **Acknowledgment of External Contributions**  
   DeepBattler includes components from Hearthstone Deck Tracker (HDT) by HearthSim. All rights to HDT belong to HearthSim and its contributors. This doesn’t imply any ownership or endorsement by HearthSim.

4. **No Warranty**  
   The software is provided "as is." We aren’t responsible for any issues that arise from using it.

5. **Derivative Works**  
   If you modify or build upon this software, include this license and follow its terms.

6. **Redistribution**  
   If you share the software or any derivatives, keep this license and the copyright notices.

By using DeepBattler, you agree to these terms.

[William-Dic]  
[2024]

---

## 中文 <a id="chinese"></a>

### 酒馆里随便找个位置坐坐！我是DeepBattler，你的专属酒馆战棋助手！🍻🍻

DeepBattler，是一款专为《炉石传说》战场模式打造的先进助手。由大型语言模型（LLM）驱动，DeepBattler无缝集成《炉石传说》卡组跟踪器（HDT）插件，为你提供**实时战略建议**。无论你是想提升排名还是改善游戏技巧，DeepBattler都能助你一臂之力！

DeepBattler的实力可以匹敌**欧服排名前0.1%**的玩家，得益于其深入的语音辅助指导，帮助你在关键时刻做出最佳决策。让我们一起提升你的游戏水平吧！

## 系统组件  

### 1. 《炉石传说》卡组跟踪器（HDT）插件  
- **实时监控:** 实时跟踪你的游戏状态  
- **JSON输出:** 提供清晰、结构化的数据  
- **高效数据处理:** 确保流畅运行  
- **深入洞察:** 提供全面的游戏分析  

### 2. LLM驱动的Python代理  
- **高级分析:** 利用强大的语言模型功能  
- **战略建议:** 提供实时战术建议  
- **语音通信:** 自然的语音交互  
- **自适应决策:** 根据不同游戏情境调整策略  

## 设置与配置  

### 插件设置  
1. 打开 `DeepBattlerPlugin/Class1.cs` 文件。  
2. 将 `_path` 变量设置为你的游戏状态文件的绝对路径：  
   ```csharp  
   private readonly string _path = @"C:\Your\Absolute\Path\To\game_state.json";  
   ```  
3. 构建插件。编译后的 `DeepBattlerPlugin.dll` 位于 `DeepBattlerPlugin/bin/Debug` 目录下。  
4. 打开《炉石传说》卡组跟踪器（HDT）。  
5. 将插件文件复制到HDT的插件目录：  
   - 默认位置：`%AppData%\Hearthstone Deck Tracker\Plugins`  
6. 启动《炉石传说》卡组跟踪器。  
7. 在HDT的 `选项 -> 插件` 下启用插件。  

![HDT插件设置](https://github.com/user-attachments/assets/23f41637-d517-4b79-87d5-cc6e5009ac24)

### LLM代理设置  
1. 安装所需的Python包：  
   ```bash  
   pip install openai playsound==1.2.2  
   ```  
   *注意：需要兼容性，请使用 `playsound` 的1.2.2版本。*  

2. 在 `DeepBattler.py` 中添加你的OpenAI API密钥：  
   ```python  
   api_key = "your-openai-api-key-here"  
   ```  

3. 启动LLM代理：  
   ```bash  
   python DeepBattler.py  
   ```  

---

## 日本語 <a id="japanese"></a>

### 英雄よ、ようこそ！私はDeepBattler、あなた専用のバトルグラウンドコーチです！🍻🍻

DeepBattlerへようこそ。『ハースストーン』のバトルグラウンド向けに特化した最新のアシスタントです。大型言語モデル（LLM）を搭載し、『ハースストーン』デックトラッカー（HDT）プラグインとシームレスに統合して、**リアルタイムの戦略アドバイス**を提供します。ランキングを上げたい方も、ゲームスキルを向上させたい方も、DeepBattlerがサポートします！

DeepBattlerの実力は**EUサーバーの上位0.1%**のプレイヤーに匹敵します。音声支援ガイダンスにより、重要な場面で最適な判断を下す手助けをします。さあ、一緒にゲームをレベルアップしましょう！

## システムコンポーネント  

### 1. 『ハースストーン』デックトラッカー（HDT）プラグイン  
- **リアルタイム監視:** ゲーム状態をリアルタイムで追跡  
- **JSON出力:** 明確で構造化されたデータを提供  
- **効率的なデータ処理:** スムーズなパフォーマンスを保証  
- **詳細なインサイト:** ゲームの分析を包括的に提供  

### 2. LLM搭載Pythonエージェント  
- **高度な分析:** 強力な言語モデル機能を活用  
- **戦略的アドバイス:** リアルタイムで戦術的な提案を提供  
- **音声コミュニケーション:** 自然な音声インタラクション  
- **適応型の意思決定:** ゲームの状況に応じて戦略を調整  

## セットアップと構成  

### プラグインセットアップ  
1. `DeepBattlerPlugin/Class1.cs` ファイルを開きます。  
2. `_path` 変数をゲーム状態ファイルの絶対パスに設定します：  
   ```csharp  
   private readonly string _path = @"C:\Your\Absolute\Path\To\game_state.json";  
   ```  
3. プラグインをビルドします。コンパイルされた `DeepBattlerPlugin.dll` は `DeepBattlerPlugin/bin/Debug` に配置されます。  
4. 『ハースストーン』デックトラッカー（HDT）を開きます。  
5. プラグインファイルをHDTのプラグインディレクトリにコピーします：  
   - デフォルトの場所：`%AppData%\Hearthstone Deck Tracker\Plugins`  
6. 『ハースストーン』デックトラッカーを起動します。  
7. HDTの `オプション -> プラグイン` でプラグインを有効にします。  

![HDTプラグイン設定](https://github.com/user-attachments/assets/23f41637-d517-4b79-87d5-cc6e5009ac24)

### LLMエージェントセットアップ  
1. 必要なPythonパッケージをインストールします：  
   ```bash  
   pip install openai playsound==1.2.2  
   ```  
   *注意：互換性のため、`playsound` のバージョン1.2.2が必要です。*  

2. `DeepBattler.py` にOpenAI APIキーを追加します：  
   ```python  
   api_key = "your-openai-api-key-here"  
   ```  

3. LLMエージェントを起動します：  
   ```bash  
   python DeepBattler.py  
   ```  

</div>
