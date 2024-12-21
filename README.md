# Well met, hero! I’m DeepBattler, your LLM-Powered Battlegrounds Coach! 🍻🍻  

DeepBattler is an advanced Large Language Model (LLM)-powered assistant tailored for Hearthstone Battlegrounds. Integrating seamlessly with a Hearthstone Deck Tracker (HDT) plugin, it leverages cutting-edge LLM capabilities to deliver **real-time strategic coaching**. This tool has propelled players into the **top 0.1% rankings on EU servers** and offers real-time voice-assisted guidance for optimal decision-making.  

<img width="1143" alt="e9adcf966ffdfa1fea40d6ca0c844e3" src="https://github.com/user-attachments/assets/daff2ce4-c499-4b9f-8232-8819e4f3e6da" />  

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

<img width="825" alt="79d99dfe1e91824af626b3d9145a156" src="https://github.com/user-attachments/assets/23f41637-d517-4b79-87d5-cc6e5009ac24" />  

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

Copyright (c) [2024] [William-Dic]  

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
