# DeepBattler - Your BEST LLM Battlegrounds Coach/Friend！🍻🍻 <a id="english"></a>

**[English](#english)** | **[中文](#chinese)** | **[日本語](#japanese)**

### Well met, hero! I'm DeepBattler, the tavern master who brews brilliant plays, belly laughs, and more pep than a dancing Murloc on espresso! 🍻🐟

DeepBattler, a LLM-Driven Hearthstone Battlegrounds enthusiast like us. DeepBattler seamlessly integrates with the Hearthstone Deck Tracker (HDT) plugin to provide you with **real-time strategic advice**. Whether you're aiming to climb the ranks or just improve your game experience, DeepBattler has got your back!

DeepBattler's strength can match that of the **top 0.1% players on EU servers (8K ELO)**, thanks to its insightful, voice-assisted guidance that helps you make the best decisions on the fly. Let’s take your gameplay to the next level!

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

1. **Install the Plugin in HDT**  
   1. Open Hearthstone Deck Tracker (HDT).
   2. Copy the plugin files to the HDT plugins directory:
      - Default location: `%AppData%\Hearthstone Deck Tracker\Plugins`
   3. Activate the Hearthstone deck tracker.
   4. Place the Agent/game_ste.json in the HDT directory under Agent/game_ste.json. If there are no files, enabling the plugin will result in an error. Error prompt path.
   5. Enable plugins under 'Options ->plugins' in HDT.
   
   ![HDT Plugin Setup](https://github.com/user-attachments/assets/23f41637-d517-4b79-87d5-cc6e5009ac24)

### LLM Agent Setup  
1. **Enter the Agent folder to install the required Python packages**  
   ```bash  
   pip install -r requirements.txt
   ```  
   
2. **Add your OpenAI API key and Base-URL in `. env `:**  
   ```python  
   API_KEY=
   Base_URL= https://api.openai.com/v1
   Language="english"
   ```  
   
3. **Launch the LLM agent:**  
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

# DeepBattler - 你的专属大模型酒馆战棋助手！ 🍻🍻 <a id="chinese"></a>

**[English](#english)** | **[中文](#chinese)** | **[日本語](#japanese)**

### 英雄，好久不见！我是DeepBattler——一位既能端出妙计良策，又能端出热茶闲聊的酒馆掌柜，嘴皮子比鱼人还溜，招式比醉拳还灵！🍵🐟 

DeepBattler，是一款专为《炉石传说》酒馆战棋打造的先进助手。由大语言模型（LLM）驱动，集成了海量的游戏数据和随从选择分析，作者也提供了开放的串口，让你可以非常轻易地修改并添加你的偏好。DeepBattler无缝集成《炉石传说》卡组跟踪器（HDT）插件，为你提供**实时战略建议**。无论你是想提升排名还是改善游戏技巧，DeepBattler都能助你一臂之力！

DeepBattler的实力可以匹敌**欧服排名前0.1%的玩家**，得益于其深入的语音辅助指导，帮助你在关键时刻做出最佳决策。让我们一起提升你的游戏水平吧！

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

1. **安装插件到HDT**  
   1. 打开《炉石传说》卡组跟踪器（HDT）。
   2. 将下载DeepBattlerPlugin.dll文件复制到HDT的插件目录：
      - 默认位置：`%AppData%\Hearthstone Deck Tracker\Plugins`
   3. 启动《炉石传说》卡组跟踪器。
   4. 将Agent/game_state.json，放到HDT目录下Agent/game_state.json。若无文件，启用插件会报错。报错提示路径。
   5. 在HDT的 `选项 -> 插件` 下启用插件。 
   ![HDT插件设置](https://github.com/user-attachments/assets/23f41637-d517-4b79-87d5-cc6e5009ac24)

### LLM代理设置  
1. **进入Agent文件夹 装所需的Python包：**  
   ```bash  
   pip install -r requirements.txt
   ```  
   *注意：需要兼容性，请使用 `playsound` 的1.2.2版本。*  
   
2. **在 `.env` 中添加你的OpenAI API密钥 和Base_URL：**  
   ```python  
   API_KEY=
   Base_URL=https://api.openai.com/v1
   Language="english"
   ```  
   
3. **启动LLM代理：**  
   ```bash  
   python DeepBattler.py  
   ```  

## 自定义非商业许可证

© [2024] [William-Dic]

您可以自由地为个人、教育或非商业目的使用、复制、修改和分享本软件。以下是您需要了解的内容：

1. **非商业使用**  
   您可以自由使用和调整本软件，但未经许可不得将其用于商业销售或分发。

2. **《炉石传说》知识产权**  
   本工具使用了暴雪娱乐的《炉石传说》中的资产。使用DeepBattler时，请确保遵守暴雪的条款。本工具与暴雪无关联，也未得到暴雪的认可。

3. **承认外部贡献**  
   DeepBattler包含了HearthSim开发的《炉石传说》卡组跟踪器（HDT）的组件。HDT及其组件的所有权归HearthSim及其贡献者所有。这不意味着HearthSim拥有或认可本工具。

4. **无担保**  
   本软件按“原样”提供。我们对因使用本软件而产生的任何问题不承担责任。

5. **衍生作品**  
   如果您修改或基于本软件开发衍生作品，请包含本许可证并遵守其条款。

6. **再分发**  
   如果您分享本软件或任何衍生作品，请保留本许可证和版权声明。

使用DeepBattler，即表示您同意这些条款。

[William-Dic]  
[2024]

---

# DeepBattler - あなた専用の大型モデル Battlegrounds アシスタント！ 🍻🍻 <a id="japanese"></a>

**[English](#english)** | **[中文](#chinese)** | **[日本語](#japanese)**

### お久しぶりです、英雄！私はDeepBattler、妙案も熱いお茶も提供する酒場のマスターです！口の回転はムルロックより速く、動きは居合斬りよりキレがある…でも足はちゃっかり畳に引っかかるタイプです！

DeepBattlerへようこそ。『ハースストーン』のバトルグラウンド向けに特化した最新のアシスタントです。大型言語モデル（LLM）を搭載し、『ハースストーン』デックトラッカー（HDT）プラグインとシームレスに統合して、**リアルタイムの戦略アドバイス**を提供します。ランキングを上げたい方も、ゲームスキルを向上させたい方も、DeepBattlerがサポートします！

DeepBattlerの実力は**EUサーバーの上位0.1%のプレイヤーに匹敵します**。音声支援ガイダンスにより、重要な場面で最適な判断を下す手助けをします。さあ、一緒にゲームをレベルアップしましょう！

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
1. **HDTへのプラグインのインストール**  
   1. 『ハースストーン』デックトラッカー（HDT）を開きます。
   2. プラグインファイルをHDTのプラグインディレクトリにコピーします：
      - デフォルトの場所：`%AppData%\Hearthstone Deck Tracker\Plugins`
   3. 『ハースストーン』デックトラッカーを起動します。
   4.Agent/game_state.jsonを、HDTディレクトリの下のAgent/game_state.jsonに配置します。ファイルがない場合は、プラグインを有効にするとエラーが発生します。プロンプトパスをエラーしました。
   5. HDTの `オプション -> プラグイン` でプラグインを有効にします。
   
   ![HDTプラグイン設定](https://github.com/user-attachments/assets/23f41637-d517-4b79-87d5-cc6e5009ac24)

### LLMエージェントセットアップ  
1. **必要なPythonパッケージをインストールします：**  
   ```bash  
   pip install -r requirements.txt
   ```  
   
2. **OpenAI APIキーとBase _ URLを`.env `に追加する：**  
   ```python  
   API_KEY=
   Base_URL=https://api.openai.com/v1
   Language="Japanese" 
   ```  
   
3. **LLMエージェントを起動します：**  
   ```bash  
   python DeepBattler.py  
   ```  

#### 「コピー ローカル」プロパティの設定（オプション）

ビルドプロセス中にこれらの依存関係が出力ディレクトリにコピーされるようにするために、**「コピー ローカル」** プロパティを **「True」** に設定します：

1. **参照を展開する**
   - **「ソリューションエクスプローラー」** で、**「参照」**（**References**）ノードを展開します。

2. **プロパティを設定する**
   - 追加した `HearthDb.dll` と `HearthstoneDeckTracker.exe` の参照を選択します。
   - 各参照を右クリックし、**「プロパティ」** を選択します。
   - **プロパティウィンドウ**で、**「コピー ローカル」**（**Copy Local**） を **「True」** に設定します。

#### 注意事項

- **互換性**：使用している `HearthDb.dll` と `HearthstoneDeckTracker.exe` のバージョンが現在の **Hearthstone Deck Tracker (HDT)** のバージョンと互換性があることを確認してください。互換性の問題を避けるためです。
- **プラグインディレクトリ**：上記の手順を完了した後、コンパイルされた `DeepBattlerPlugin.dll` を HDT の `Plugins` フォルダに配置し、HDT がプラグインを正しくロードできるようにしてください。

## カスタム非商用ライセンス

© [2024] [William-Dic]

本ソフトウェアを個人、教育、非商用目的で使用、コピー、修正、共有することが自由に許可されています。以下の内容を確認してください：

1. **非商用利用**  
   本ソフトウェアを自由に使用および調整できますが、許可なく商業目的で販売または配布しないでください。

2. **ハースストーンの知的財産**  
   本ツールは、ブリザード・エンターテイメントのハースストーンのアセットを使用しています。DeepBattlerを使用する際は、ブリザードの利用規約を遵守してください。本ツールはブリザードと提携しておらず、ブリザードからの承認も受けていません。

3. **外部貢献の認識**  
   DeepBattlerには、HearthSimが開発したハースストーンデックトラッカー（HDT）のコンポーネントが含まれています。HDTおよびそのコンポーネントの全ての権利はHearthSimおよびその貢献者に帰属します。これはHearthSimによる所有権や承認を意味するものではありません。

4. **無保証**  
   本ソフトウェアは「現状のまま」提供されます。使用に起因する問題については一切の責任を負いません。

5. **派生作品**  
   本ソフトウェアを修正または基にして派生作品を作成する場合は、このライセンスを含め、その条項に従ってください。

6. **再配布**  
   本ソフトウェアまたはその派生作品を共有する場合は、このライセンスおよび著作権表示を保持してください。

DeepBattlerを使用することで、これらの条件に同意したことになります。

[William-Dic]  
[2024]
