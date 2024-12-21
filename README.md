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
