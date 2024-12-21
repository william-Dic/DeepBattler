# Well met, hero! I’m DeepBattler, the Battlegrounds genius AI! 🍻🍻

An advanced Large Language Model (LLM) powered assistant for Hearthstone Battlegrounds that combines a Hearthstone Deck Tracker plugin with state-of-the-art language models to provide real-time strategic coaching, this tool has helped achieve top 0.1% ranking on EU servers and provides real-time voice-assisted coaching for optimal decision making.

## System Components

### 1. Hearthstone Deck Tracker (HDT) Plugin
- Real-time game state monitoring
- JSON-based state representation
- Efficient data capture and processing
- Comprehensive game information tracking

### 2. LLM-Powered Python Agent
- Advanced language model integration
- Real-time strategic analysis
- Natural voice communication
- Adaptive decision-making support

## Setup and Configuration

### Plugin Setup
1. Download the latest release from the releases page
2. Close Hearthstone Deck Tracker if it's running
3. Copy the plugin files to your Hearthstone Deck Tracker plugins folder
   - Default location: `%AppData%\Hearthstone Deck Tracker\Plugins`
4. Open Hearthstone Deck Tracker
5. Enable the plugin in HDT's settings menu under `Options -> Plugins`
6. Configure the `_path` variable in the plugin code:
```csharp
private readonly string _path = @"C:\Your\Absolute\Path\To\game_state.json";
```

### LLM Agent Setup
1. Install required Python packages:
```bash
pip install openai playsound==1.2.2
```
Note: playsound version 1.2.2 is specifically required for compatibility.

2. Configure your OpenAI API key in `DeepBattler.py`:
```python
api_key = "your-openai-api-key-here"
```

3. Launch the LLM agent:
```bash
python DeepBattler.py
```

# Custom Non-Commercial License
Copyright (c) [2024] [William-Dic]
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to use,
copy, modify, and distribute the Software for personal, educational, or
non-commercial purposes only, subject to the following conditions:
1. Non-Commercial Use:
   The Software or any derivative works thereof shall not be used, sold, or distributed for any commercial purposes without prior written permission from the copyright holder.
2. Hearthstone Intellectual Property:
   This Software may reference or use assets, names, or materials from Blizzard Entertainment's Hearthstone. These assets are the intellectual property of Blizzard Entertainment and are used under their policies. Any usage of this Software must comply with Blizzard Entertainment's terms and conditions. This Software is not affiliated with or endorsed by Blizzard Entertainment.
3. Acknowledgment of External Contributions:
   This Software integrates components or utilities from Hearthstone Deck Tracker (HDT), developed by HearthSim. All rights to HDT and its associated components are retained by HearthSim and its contributors. This acknowledgment does not imply ownership of this Software by HearthSim.
4. No Warranty:
   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES, OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT, OR OTHERWISE, ARISING FROM, OUT OF, OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
5. Derivative Works:
   Any modifications or derivative works created from this Software must include this license and retain these restrictions.
6. Redistribution:
   Redistribution of the Software or derivative works must preserve this license file and all copyright notices.
By using this Software, you agree to these terms.
[William-Dic]  
[2024]
