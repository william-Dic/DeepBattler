```markdown
# DeepBattler - LLM-Powered Battlegrounds Coach! 🍻🍻

**[English](#english)** | [**中文**](#中文) | [**日本語**](#日本語)

---

## English

### Well met, hero! I’m DeepBattler, your LLM-Powered Battlegrounds Coach! 🍻🍻  

DeepBattler is an advanced Large Language Model (LLM)-powered assistant tailored for Hearthstone Battlegrounds. Integrating seamlessly with a Hearthstone Deck Tracker (HDT) plugin, it leverages cutting-edge LLM capabilities to deliver **real-time strategic coaching**. This tool has propelled players into the **top 0.1% rankings on EU servers** and offers real-time voice-assisted guidance for optimal decision-making.  

![DeepBattler Interface](https://github.com/user-attachments/assets/daff2ce4-c499-4b9f-8232-8819e4f3e6da)

## System Components  

### 1. Hearthstone Deck Tracker (HDT) Plugin  
- Monitors game state in real-time  
- Outputs JSON-based state representations  
- Captures and processes data efficiently  
- Provides comprehensive game insights  

### 2. LLM-Powered Python Agent  
- Integrates advanced language model capabilities  
- Performs real-time strategic analysis  
- Supports natural voice communication  
- Offers adaptive, scenario-based decision-making  

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

Permission is granted free of charge to any individual to use, copy, modify, and distribute this software for personal, educational, or non-commercial purposes, under the following terms:  

1. **Non-Commercial Use**  
   The Software or its derivatives may not be used, sold, or distributed for commercial purposes without prior written permission from the copyright holder.  

2. **Hearthstone Intellectual Property**  
   This Software may reference or utilize assets, names, or materials from Blizzard Entertainment's Hearthstone. All such assets remain the intellectual property of Blizzard Entertainment. Usage of this Software must comply with Blizzard Entertainment's terms and conditions. This Software is not affiliated with or endorsed by Blizzard Entertainment.  

3. **Acknowledgment of External Contributions**  
   This Software incorporates components from Hearthstone Deck Tracker (HDT), developed by HearthSim. All rights to HDT and its components remain with HearthSim and its contributors. This acknowledgment does not imply any ownership or endorsement of this Software by HearthSim.  

4. **No Warranty**  
   This Software is provided "as is," without any express or implied warranties, including but not limited to fitness for a particular purpose or non-infringement. The authors shall not be held liable for any claims, damages, or other liabilities arising from the use or misuse of this Software.  

5. **Derivative Works**  
   Any modifications or derivative works must include this license and comply with its restrictions.  

6. **Redistribution**  
   Redistribution of this Software or its derivatives must retain this license file and copyright notices.  

By using this Software, you agree to these terms.  

[William-Dic]  
[2024]

---

## 中文

### 英雄，欢迎！我是DeepBattler，你的LLM驱动战场教练！🍻🍻

DeepBattler是一款先进的大型语言模型（LLM）驱动的助手，专为《炉石传说》战场模式量身定制。它无缝集成了《炉石传说》卡组跟踪器（HDT）插件，利用尖端的LLM能力提供**实时战略指导**。这款工具已帮助玩家进入**欧服排名前0.1%**，并提供实时语音辅助指导，帮助做出最佳决策。

![DeepBattler界面](https://github.com/user-attachments/assets/daff2ce4-c499-4b9f-8232-8819e4f3e6da)

## 系统组件  

### 1. 《炉石传说》卡组跟踪器（HDT）插件  
- 实时监控游戏状态  
- 输出基于JSON的状态表示  
- 高效捕获和处理数据  
- 提供全面的游戏洞察  

### 2. LLM驱动的Python代理  
- 集成先进的语言模型功能  
- 执行实时战略分析  
- 支持自然语音通信  
- 提供自适应的情景决策  

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

## 自定义非商业许可协议  

© [2024] [William-Dic]  

特此免费授权任何个人出于个人、教育或非商业目的使用、复制、修改和分发本软件及其衍生作品，遵循以下条款：  

1. **非商业使用**  
   未经版权所有者事先书面许可，软件或其衍生作品不得用于、出售或分发商业用途。  

2. **《炉石传说》知识产权**  
   本软件可能引用或使用暴雪娱乐《炉石传说》的资产、名称或材料。所有此类资产均为暴雪娱乐的知识产权。使用本软件必须遵守暴雪娱乐的条款和条件。本软件未与暴雪娱乐关联或获得其认可。  

3. **外部贡献的认可**  
   本软件包含《炉石传说》卡组跟踪器（HDT）开发者HearthSim的组件。HDT及其组件的所有权利归HearthSim及其贡献者所有。此认可不意味着HearthSim对本软件拥有任何所有权或认可。  

4. **无担保**  
   本软件按“原样”提供，不附任何明示或暗示的担保，包括但不限于适销性或特定用途适用性。作者不对因使用或误用本软件而引起的任何索赔、损害或其他责任负责。  

5. **衍生作品**  
   任何修改或衍生作品必须包含本许可协议并遵守其限制。  

6. **再分发**  
   再分发本软件或其衍生作品必须保留本许可文件和版权声明。  

使用本软件即表示您同意这些条款。  

[William-Dic]  
[2024]

---

## 日本語

### 英雄よ、よく来た！私はDeepBattler、あなたのLLM搭載バトルグラウンドコーチです！🍻🍻

DeepBattlerは、『ハースストーン』のバトルグラウンド向けに特化した高度な大規模言語モデル（LLM）搭載アシスタントです。『ハースストーン』デックトラッカー（HDT）プラグインとシームレスに統合し、最先端のLLM機能を活用して**リアルタイムの戦略コーチング**を提供します。このツールはプレイヤーを**EUサーバーの上位0.1%のランキング**に押し上げ、最適な意思決定のためのリアルタイム音声支援ガイダンスを提供します。

![DeepBattlerインターフェース](https://github.com/user-attachments/assets/daff2ce4-c499-4b9f-8232-8819e4f3e6da)

## システムコンポーネント  

### 1. 『ハースストーン』デックトラッカー（HDT）プラグイン  
- ゲーム状態をリアルタイムで監視  
- JSONベースの状態表現を出力  
- データを効率的にキャプチャおよび処理  
- 包括的なゲームインサイトを提供  

### 2. LLM搭載Pythonエージェント  
- 先進的な言語モデル機能を統合  
- リアルタイムの戦略分析を実行  
- 自然な音声コミュニケーションをサポート  
- 適応型のシナリオベース意思決定を提供  

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

---

## カスタム非商用ライセンス  

© [2024] [William-Dic]  

本ソフトウェアおよびその派生物を個人、教育、非商用目的で使用、コピー、変更、配布することを、以下の条件に従い無償で許可します：  

1. **非商用利用**  
   本ソフトウェアまたはその派生物は、著作権者の事前の書面による許可なく、商業目的で使用、販売、配布することはできません。  

2. **『ハースストーン』の知的財産**  
   本ソフトウェアは、ブリザード・エンターテイメントの『ハースストーン』の資産、名称、資料を参照または利用する場合があります。これらの資産はすべてブリザード・エンターテイメントの知的財産です。本ソフトウェアの使用は、ブリザード・エンターテイメントの利用規約に従う必要があります。本ソフトウェアはブリザード・エンターテイメントと提携しておらず、同社によって承認されているものではありません。  

3. **外部貢献の認識**  
   本ソフトウェアは、HearthSimが開発した『ハースストーン』デックトラッカー（HDT）のコンポーネントを含んでいます。HDTおよびそのコンポーネントに対するすべての権利はHearthSimおよびその貢献者に帰属します。この認識は、HearthSimが本ソフトウェアを所有または承認していることを意味するものではありません。  

4. **無保証**  
   本ソフトウェアは「現状のまま」提供され、特定の目的への適合性や非侵害を含む明示的または暗示的な保証は一切ありません。作者は、本ソフトウェアの使用または誤用に起因するいかなる請求、損害、その他の責任についても一切責任を負いません。  

5. **派生作品**  
   いかなる修正または派生作品も、このライセンスを含み、その制限に従う必要があります。  

6. **再配布**  
   本ソフトウェアまたはその派生作品を再配布する場合、このライセンスファイルおよび著作権表示を保持する必要があります。  

本ソフトウェアを使用することにより、これらの条件に同意したものとみなされます。  

[William-Dic]  
[2024]

