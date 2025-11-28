# DeepBattler - Your BEST LLM Battlegrounds Coach/Friend！🍻🍻 <a id="english"></a>

**[English](#english)** | **[中文](#chinese)** | **[日本語](#japanese)**

## [❗❗❗Update: We are trying using GRPO to train a new SOTA RL Policy, check the other branch!]

### Well met, hero! I'm DeepBattler, the tavern master who brews brilliant plays, belly laughs, and more pep than a dancing Murloc on espresso! 🍻🐟

DeepBattler, a LLM-Driven Hearthstone Battlegrounds enthusiast like us. DeepBattler seamlessly integrates with the Hearthstone Deck Tracker (HDT) plugin to provide you with **real-time strategic advice**. Whether you're aiming to climb the ranks or just improve your game experience, DeepBattler has got your back!

DeepBattler's strength can match that of the **top 0.1% players on EU servers (8K ELO)**, thanks to its insightful, voice-assisted guidance that helps you make the best decisions on the fly. Let’s take your gameplay to the next level!

**Demos can be found here! [YouTube Link](https://www.youtube.com/watch?v=A9XKPx1COfc&t=66s)**

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
1. **Open the `DeepBattlerPlugin/DeepBattlerPlugin.csproj` file.**  
   - Instead of modifying individual class files, ensure your project references are correctly set up in the `.csproj` file.

2. **Add Dependencies:**  
   To ensure **DeepBattlerPlugin** functions correctly, you only need to add the following two dependencies:
   
   1. **HearthDb.dll**
   2. **HearthstoneDeckTracker.exe**

   #### Adding Dependencies to Your Visual Studio Project
   
   Follow these steps to add the two dependencies to your Visual Studio project:
   
   1. **Open Your Project**
      - Open your plugin project in Visual Studio (e.g., `DeepBattlerPlugin`).
   
   2. **Add References**
      - Right-click on the project name and select **"Add"** > **"Reference..."**.
   
   3. **Browse and Select Dependencies**
      - In the **"Reference Manager"** window, select the **"Browse"** tab.
      - Click the **"Browse"** button and navigate to the directory containing `HearthDb.dll` and `HearthstoneDeckTracker.exe`.
        - **HearthDb.dll**: Typically located in the HDT installation directory.
        - **HearthstoneDeckTracker.exe**: Also located in the HDT installation directory.
      - Select both files and click **"Add"**.
   
   4. **Confirm Addition**
      - After adding, click **"OK"** to confirm the references.
   
   #### Setting "Copy Local" Property (Optional)
   
   To ensure these dependencies are copied to the output directory during the build process, set their **"Copy Local"** property to **"True"**:
   
   1. **Expand References**
      - In the **"Solution Explorer"**, expand the **"References"** node.
   
   2. **Set Properties**
      - Select the recently added `HearthDb.dll` and `HearthstoneDeckTracker.exe` references.
      - Right-click each reference and choose **"Properties"**.
      - In the **Properties** window, set **"Copy Local"** to **"True"**.
   
   #### Important Notes
   
   - **Compatibility**: Ensure that the versions of `HearthDb.dll` and `HearthstoneDeckTracker.exe` you are using are compatible with your current version of **Hearthstone Deck Tracker (HDT)** to avoid potential compatibility issues.
   - **Plugin Directory**: After completing the above steps, make sure to place the compiled `DeepBattlerPlugin.dll` into HDT's `Plugins` folder so that HDT can correctly load your plugin.

3. **Configure the Plugin Path**  
   - Open the `DeepBattlerPlugin/DeepBattlerPlugin.csproj` file.
   - Set the `_path` variable to your absolute game state file path:
     ```csharp  
     private readonly string _path = @"C:\Your\Absolute\Path\To\game_state.json";  
     ```  

4. **Build the Plugin**  
   - Build the plugin. The compiled `DeepBattlerPlugin.dll` will be located under `DeepBattlerPlugin/bin/Debug`.

5. **Install the Plugin in HDT**  
   1. Open Hearthstone Deck Tracker (HDT).
   2. Copy the plugin files to the HDT plugins directory:
      - Default location: `%AppData%\Hearthstone Deck Tracker\Plugins`
   3. Launch Hearthstone Deck Tracker.
   4. Enable the plugin in HDT under `Options -> Plugins`.
   
   ![HDT Plugin Setup](https://github.com/user-attachments/assets/23f41637-d517-4b79-87d5-cc6e5009ac24)

### LLM Agent Setup  

#### Using OpenAI GPT  
1. **Install the required Python packages:**  
   ```bash  
   pip install openai playsound==1.2.2  
   ```  
   *Note: Version 1.2.2 of `playsound` is required for compatibility.*  
   
2. **Add your OpenAI API key in `Openai_caller.py`:**  
   ```python  
   api_key = "your-openai-api-key-here"  
   ```  
   
3. **Launch the LLM agent:**  
   ```bash  
   python Openai_caller.py
   ```  

---

#### Using Google Gemma  
1. **Install the required Python packages:**  
   ```bash  
   pip install keras_hub jax keras gtts playsound==1.2.2
   ```  
   *Note: Version 1.2.2 of `playsound` is required for compatibility.*  

2. **Set up the Gemma environment:**  
   Your script (`Gemma_caller.py`) includes the following environment configurations:
   ```python
   import os
   os.environ["KERAS_BACKEND"] = "jax"
   os.environ["XLA_PYTHON_CLIENT_MEM_FRACTION"] = "1.00"
   ```

3. **Prepare the necessary files:**
   - `game_state.json`: A JSON file to provide the current game state.
   - `Prompt.txt`: A text file containing the system prompt for Gemma.

4. **Run the Gemma agent:**
   ```bash
   python Gemma_caller.py
   ```

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
1. **打开 `DeepBattlerPlugin/DeepBattlerPlugin.csproj` 文件。**  
   - 不再修改单个类文件，而是确保项目引用在 `.csproj` 文件中正确设置。

2. **添加依赖项:**  
   为了确保 **DeepBattlerPlugin** 正常运行，您仅需添加以下两个依赖项：
   
   1. **HearthDb.dll**
   2. **HearthstoneDeckTracker.exe**

   #### 将依赖项添加到 Visual Studio 项目
   
   请按照以下步骤在 Visual Studio 中添加这两个依赖项：
   
   1. **打开您的项目**
      - 在 Visual Studio 中打开您的插件项目（例如，DeepBattlerPlugin）。
   
   2. **添加引用**
      - 右键点击项目名称，选择 **“添加”** > **“引用...”**。
   
   3. **浏览并选择依赖项**
      - 在弹出的 **“引用管理器”** 窗口中，选择 **“浏览”** 选项卡。
      - 点击 **“浏览”** 按钮，导航到包含 `HearthDb.dll` 和 `HearthstoneDeckTracker.exe` 的目录。
        - **HearthDb.dll**：通常位于 HDT 的安装目录下。
        - **HearthstoneDeckTracker.exe**：同样位于 HDT 的安装目录中。
      - 选择这两个文件后，点击 **“添加”**。
   
   4. **确认添加**
      - 添加完毕后，点击 **“确定”** 以确认引用。
   
   #### 设置“复制到本地”属性（可选）
   
   为了确保在构建项目时，这些依赖项会被复制到输出目录，您可以设置它们的 **“复制到本地”** 属性：
   
   1. **展开引用**
      - 在 **“解决方案资源管理器”** 中，展开 **“引用”**（**References**）。
   
   2. **设置属性**
      - 选择刚刚添加的 `HearthDb.dll` 和 `HearthstoneDeckTracker.exe` 引用。
      - 右键点击每个引用，选择 **“属性”**。
      - 在属性窗口中，将 **“复制到本地”**（**Copy Local**） 设置为 **“True”**。
   
   #### 注意事项
   
   - **兼容性**：确保您使用的 `HearthDb.dll` 和 `HearthstoneDeckTracker.exe` 版本与您当前的 **Hearthstone Deck Tracker (HDT)** 版本兼容，以避免潜在的兼容性问题。
   - **插件目录**：完成上述步骤后，确保将编译生成的 `DeepBattlerPlugin.dll` 放置在 HDT 的 `Plugins` 文件夹中，以便 HDT 能够正确加载您的插件。

3. **配置插件路径**  
   - 打开 `DeepBattlerPlugin/DeepBattlerPlugin.csproj` 文件。
   - 将 `_path` 变量设置为你的 `game_state.json` 的绝对路径：
     ```csharp  
     private readonly string _path = @"C:\Your\Absolute\Path\To\game_state.json";  
     ```  

4. **构建插件**  
   - 构建插件。编译后的 `DeepBattlerPlugin.dll` 位于 `DeepBattlerPlugin/bin/Debug` 目录下。

5. **安装插件到HDT**  
   1. 打开《炉石传说》卡组跟踪器（HDT）。
   2. 将插件文件复制到HDT的插件目录：
      - 默认位置：`%AppData%\Hearthstone Deck Tracker\Plugins`
   3. 启动《炉石传说》卡组跟踪器。
   4. 在HDT的 `选项 -> 插件` 下启用插件。
   
   ![HDT插件设置](https://github.com/user-attachments/assets/23f41637-d517-4b79-87d5-cc6e5009ac24)

### LLM代理设置  
1. **安装所需的Python包：**  
   ```bash  
   pip install openai playsound==1.2.2  
   ```  
   *注意：需要兼容性，请使用 `playsound` 的1.2.2版本。*  
   
2. **在 `DeepBattler.py` 中添加你的OpenAI API密钥：**  
   ```python  
   api_key = "your-openai-api-key-here"  
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
1. **`DeepBattlerPlugin/DeepBattlerPlugin.csproj` ファイルを開きます。**  
   - 個々のクラスファイルを変更する代わりに、`.csproj` ファイル内でプロジェクトの参照が正しく設定されていることを確認してください。

2. **依存関係を追加する:**  
   **DeepBattlerPlugin** が正しく機能するために、以下の2つの依存関係を追加する必要があります：
   
   1. **HearthDb.dll**
   2. **HearthstoneDeckTracker.exe**

   #### Visual Studio プロジェクトに依存関係を追加する方法
   
   以下の手順に従って、Visual Studio プロジェクトにこれらの依存関係を追加してください：
   
   1. **プロジェクトを開く**
      - Visual Studio でプラグインプロジェクト（例：DeepBattlerPlugin）を開きます。
   
   2. **参照を追加する**
      - プロジェクト名を右クリックし、**「追加」** > **「参照...」** を選択します。
   
   3. **依存関係をブラウズして選択する**
      - ポップアップした **「参照マネージャー」** ウィンドウで、**「ブラウズ」** タブを選択します。
      - **「ブラウズ」** ボタンをクリックし、`HearthDb.dll` と `HearthstoneDeckTracker.exe` が含まれるディレクトリに移動します。
        - **HearthDb.dll**：通常、HDTのインストールディレクトリにあります。
        - **HearthstoneDeckTracker.exe**：同様に、HDTのインストールディレクトリにあります。
      - 両方のファイルを選択し、**「追加」** をクリックします。
   
   4. **追加を確認する**
      - 追加が完了したら、**「OK」** をクリックして参照を確認します。
   
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

3. **プラグインパスの設定**  
   - `DeepBattlerPlugin/DeepBattlerPlugin.csproj` ファイルを開きます。
   - `_path` 変数をあなたの `game_state.json` の絶対パスに設定します：
     ```csharp  
     private readonly string _path = @"C:\Your\Absolute\Path\To\game_state.json";  
     ```  

4. **プラグインをビルドする**  
   - プラグインをビルドします。コンパイルされた `DeepBattlerPlugin.dll` は `DeepBattlerPlugin/bin/Debug` に配置されます。

5. **HDTへのプラグインのインストール**  
   1. 『ハースストーン』デックトラッカー（HDT）を開きます。
   2. プラグインファイルをHDTのプラグインディレクトリにコピーします：
      - デフォルトの場所：`%AppData%\Hearthstone Deck Tracker\Plugins`
   3. 『ハースストーン』デックトラッカーを起動します。
   4. HDTの `オプション -> プラグイン` でプラグインを有効にします。
   
   ![HDTプラグイン設定](https://github.com/user-attachments/assets/23f41637-d517-4b79-87d5-cc6e5009ac24)

### LLMエージェントのセットアップ

## OpenAI GPTを使用する場合

1. **必要なPythonパッケージをインストールします:**
```bash
pip install openai playsound==1.2.2
```
*注意：互換性のため、`playsound`のバージョンは必ず`1.2.2`を使用してください。*

2. **OpenAIのAPIキーを`Openai_caller.py`に設定します:**
```python
api_key = "your-openai-api-key-here"
```

3. **LLMエージェントを起動します:**
```bash
python Openai_caller.py
```

---

## Google Gemmaのセットアップ方法

Gemmaを使ったLLMエージェントの設定および起動方法は以下の通りです。

1. **必要なPythonパッケージをインストールします:**
```bash
pip install keras_hub jax keras gtts playsound==1.2.2
```
*注意：playsoundは互換性のため必ずバージョン1.2.2を使用してください。*

2. **Gemma用の環境を設定します:**  
スクリプト（`Gemma_caller.py`）に以下の環境設定を含めてください:
```python
import os

os.environ["XLA_PYTHON_CLIENT_MEM_FRACTION"] = "1.00"
os.environ["KERAS_BACKEND"] = "jax"
```

3. **Gemma用の必要ファイルを準備します:**  
以下のファイルが必要です:

- `game_state.json`：リアルタイムのゲーム状態データ（JSON形式）
- `Prompt.txt`：Gemmaのシステムプロンプトを記載したテキストファイル

4. **Gemmaエージェントを起動します:**
```bash
python Gemma_caller.py
```

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
