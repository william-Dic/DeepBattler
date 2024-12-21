Here’s the complete GitHub `README.md` code for your multilingual project with a clickable language selection option. Since GitHub doesn't support interactive JavaScript in `README.md` files, this version uses Markdown tabs provided by tools like [GitHub Markdown Viewer Plus](https://github.com/mattn/github-markdown-viewer) or GitHub's code viewers with collapsible sections:

---

### `README.md` with Multilingual Tabs

```markdown
# DeepBattler: Your LLM-Powered Battlegrounds Coach! 🍻🍻

DeepBattler is an advanced Large Language Model (LLM)-powered assistant for Hearthstone Battlegrounds. It integrates seamlessly with a Hearthstone Deck Tracker (HDT) plugin, leveraging cutting-edge LLM capabilities to deliver **real-time strategic coaching**.  

This tool has propelled players into the **top 0.1% rankings on EU servers** and offers real-time voice-assisted guidance for optimal decision-making.  

---

## 🌐 Languages  

<details open>
<summary>🇬🇧 English (Default)</summary>

### Well met, hero!  
I’m DeepBattler, your LLM-Powered Battlegrounds Coach! 🍻🍻  

DeepBattler combines a Hearthstone Deck Tracker plugin with SOTA LLM technology for **real-time strategic coaching**, helping players dominate Battlegrounds and climb the rankings.  

#### Key Features:  
- Monitors game state in real time.  
- Provides JSON-based state representation.  
- Delivers adaptive decision-making support and natural voice coaching.  

</details>

<details>
<summary>🇨🇳 中文</summary>

### 英雄，好久不见！  
我是DeepBattler，你的Battlegrounds LLM智能教练！🍻🍻  

DeepBattler结合了Hearthstone Deck Tracker插件和最先进的LLM技术，提供**实时战略指导**，帮助玩家称霸酒馆战棋并攀登排名。  

#### 主要功能：  
- 实时监控游戏状态。  
- 提供JSON格式的状态表示。  
- 提供自适应决策支持和自然语音指导。  

</details>

<details>
<summary>🇯🇵 日本語</summary>

### お久しぶりです、英雄！  
私はDeepBattler、バトルグラウンドのAIコーチです！🍻🍻  

DeepBattlerはHearthstone Deck Trackerプラグインと最先端のLLM技術を組み合わせ、**リアルタイム戦略コーチング**を提供します。プレイヤーがバトルグラウンドで支配し、ランキングを上げるのをサポートします。  

#### 主な特徴:  
- ゲーム状態をリアルタイムで監視。  
- JSON形式で状態を表現。  
- 適応型意思決定サポートと自然な音声コーチングを提供。  

</details>

---

## 🎮 How to Use  

### 1. Plugin Setup
- Open the `DeepBattlerPlugin/Class1.cs` file and configure the `_path` variable:
  ```csharp
  private readonly string _path = @"C:\Your\Absolute\Path\To\game_state.json";
  ```
- Compile the plugin and copy the `.dll` file to the Hearthstone Deck Tracker `Plugins` folder.
- Enable the plugin in the HDT `Options -> Plugins` menu.

### 2. LLM Agent Setup
- Install the required Python packages:
  ```bash
  pip install openai playsound==1.2.2
  ```
- Configure your OpenAI API key in `DeepBattler.py`:
  ```python
  api_key = "your-openai-api-key-here"
  ```
- Launch the LLM agent:
  ```bash
  python DeepBattler.py
  ```

---

## 🛡️ License  

This project is licensed under a **Custom Non-Commercial License**. See the [LICENSE](LICENSE) file for details.

---

Enjoy the game, hero! Let DeepBattler guide you to victory! 🐟🍻
```

---

### Key Features:
1. **Collapsible Language Sections**: The `<details>` and `<summary>` tags provide collapsible language sections, making the content cleaner and allowing users to choose their preferred language.
2. **Default Language (English)**: The `open` attribute in `<details>` ensures English is expanded by default.
3. **GitHub-Compatible Markdown**: Fully supported by GitHub, rendering it user-friendly.

### How to Use:
- Save the code above into your `README.md` file.
- When viewed on GitHub, users can click to expand their desired language while keeping English as the default.

Let me know if you need more assistance!
