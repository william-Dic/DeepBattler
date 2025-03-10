import os
import json
import time
import tempfile
from playsound import playsound
import keras_hub
import keras

# Set up Keras backend
os.environ["KERAS_BACKEND"] = "jax"
os.environ["XLA_PYTHON_CLIENT_MEM_FRACTION"] = "1.00"

# Load Gemma Model
model_name = "gemma2_instruct_2b_en"
gemma_lm = keras_hub.models.GemmaCausalLM.from_preset(model_name)

class GetAction:
    def __init__(self, game_state_file='game_state.json', prompt_file='Prompt.txt'):
        self.game_state_file = game_state_file
        self.prompt_file = prompt_file
        self.system_message = self.get_system_message()

    def load_game_state(self):
        """Load game state from JSON file."""
        try:
            with open(self.game_state_file, 'r', encoding='utf-8') as file:
                return json.load(file)
        except:
            return {}

    def get_system_message(self):
        """Retrieve system prompt from the Prompt.txt file."""
        try:
            with open(self.prompt_file, 'r', encoding='utf-8') as file:
                return file.read().strip()
        except FileNotFoundError:
            print(f"Error: {self.prompt_file} not found. Using default system message.")
            return "Default system message: Please provide a valid Prompt.txt file."

    def get_action(self, game_state):
        """Generate action using the Gemma model."""
        input_text = f"{self.system_message}\nGame state:\n{json.dumps(game_state, indent=2)}"
        
        # Tokenize and run inference with Gemma
        tokenized = gemma_lm.tokenizer(input_text, return_tensors="np")
        output_tokens = gemma_lm.generate(tokenized["input_ids"], max_length=150)
        action_text = gemma_lm.tokenizer.decode(output_tokens[0], skip_special_tokens=True)

        return action_text.strip()

    def speak_action(self, action_text):
        """Text-to-Speech function to vocalize the generated action."""
        if not action_text:
            return

        fd, temp_mp3_path = tempfile.mkstemp(suffix=".mp3", prefix="DeepBattler_")
        os.close(fd)

        try:
            from gtts import gTTS
            tts = gTTS(text=action_text, lang="en")
            tts.save(temp_mp3_path)
            playsound(temp_mp3_path)
        except Exception as e:
            print(f"[Error in speak_action] {e}")
        finally:
            if os.path.exists(temp_mp3_path):
                os.remove(temp_mp3_path)

def watch_game_state(game_state_file, prompt_file):
    """Monitors changes in game state and generates responses accordingly."""
    action_instance = GetAction(game_state_file, prompt_file)
    
    last_mtime = os.path.getmtime(game_state_file) if os.path.exists(game_state_file) else None

    print("Start watching game_state.json...")

    while True:
        time.sleep(1)

        if not os.path.exists(game_state_file):
            if last_mtime is not None:
                print("[watcher] game_state.json deleted!")
                last_mtime = None
            continue

        current_mtime = os.path.getmtime(game_state_file)

        if current_mtime != last_mtime:
            last_mtime = current_mtime

            print("[watcher] game_state.json changed, reloading...")

            new_game_state = action_instance.load_game_state()
            if not new_game_state:
                print("[watcher] game_state.json is empty or invalid.")
                continue

            coins = new_game_state.get("Coins", 0)
            if coins == 0:
                print("[watcher] Coins=0, skipping Gemma response.")
                continue

            new_action = action_instance.get_action(new_game_state)
            if new_action:
                print("[watcher] New Action:", new_action)
                action_instance.speak_action(new_action)

if __name__ == "__main__":
    game_state_file = "game_state.json"
    prompt_file = "Prompt.txt"
    
    watch_game_state(game_state_file, prompt_file)
