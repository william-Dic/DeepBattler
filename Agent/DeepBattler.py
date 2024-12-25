import time
import os
import json
import tempfile
from playsound import playsound
from openai import OpenAI
from dotenv import load_dotenv
load_dotenv()
class GetAction:
    def __init__(self, api_key, game_state_file='game_state.json', prompt_file='Prompt.txt'):
        self.api_key = api_key
        self.game_state_file = game_state_file
        self.prompt_file = prompt_file
        self.client = OpenAI(
            api_key=api_key,
            base_url=os.getenv("Base_URL")
            )
        self.system_message = self.get_system_message()
        self.game_state = {}
        self.last_action = ""
        self.language=os.environ.get("Language", "English")

    def load_game_state(self):
        try:
            with open(self.game_state_file, 'r', encoding='utf-8') as file:
                return json.load(file)
        except:
            return {}

    def get_system_message(self):
        try:
            with open(self.prompt_file, 'r', encoding='utf-8') as file:
                return file.read().strip()
        except FileNotFoundError:
            print(f"Error: {self.prompt_file} not found. Using default system message.")
            return "Default system message: Please provide a valid Prompt.txt file."

    def get_action(self, game_state):
        messages = [
            {
                "role": "user",
                "content": [
                    {
                        "type": "text",
                        "text": f"{self.system_message}\nGame state:\n{game_state} Please respond in {self.language}"
                    }
                ]
            }
        ]
        response = self.client.chat.completions.create(
            model="o1-mini", 
            messages=messages,
        )
        return response.choices[0].message.content.strip()

    def speak_action(self, action_text):
        if not action_text:
            return

        fd, temp_mp3_path = tempfile.mkstemp(suffix=".mp3", prefix="DeepBattler_")
        os.close(fd)  

        try:
            r = self.client.audio.speech.create(
                model="tts-1", 
                voice="echo",
                input=action_text,
            )
            r.stream_to_file(temp_mp3_path)

            playsound(temp_mp3_path)
        except Exception as e:
            print(f"[Error in speak_action] {e}")
        finally:
            if os.path.exists(temp_mp3_path):
                os.remove(temp_mp3_path)

def watch_game_state(api_key, game_state_file, prompt_file):
    action_instance = GetAction(api_key, game_state_file, prompt_file)
    
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
                print("[watcher] Coins=0, skip calling OpenAI.")
                continue

            new_action = action_instance.get_action(new_game_state)
            if new_action:
                print("[watcher] New Action:", new_action)
                action_instance.speak_action(new_action)

if __name__ == "__main__":

    api_key = os.getenv('API_KEY') 
    game_state_file = "game_state.json"
    prompt_file = "Prompt.txt"

    if not api_key:
        print("Error: The OPENAI_API_KEY environment variable is not set.")
    else:
        watch_game_state(api_key, game_state_file, prompt_file)
