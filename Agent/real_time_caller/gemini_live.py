import asyncio
import json
import os
import sys
import pyaudio
import argparse
import traceback
import base64
import io
import wave
import time
from pathlib import Path
from collections import deque

from google import genai
from dotenv import load_dotenv

# STT removed - using generate_content API for text output

# Load environment variables from .env file
load_dotenv()

if sys.version_info < (3, 11, 0):
    import taskgroup, exceptiongroup
    asyncio.TaskGroup = taskgroup.TaskGroup
    asyncio.ExceptionGroup = exceptiongroup.ExceptionGroup

# Audio Configuration
FORMAT = pyaudio.paInt16
CHANNELS = 1
SEND_SAMPLE_RATE = 16000
RECEIVE_SAMPLE_RATE = 24000
CHUNK_SIZE = 1024

MODEL = "models/gemini-2.0-flash-live-001"

# Initialize Client
api_key = os.environ.get('GEMINI_API_KEY')
if not api_key:
    print("Error: GEMINI_API_KEY not found in environment variables or .env file.")
    sys.exit(1)

client = genai.Client(http_options={"api_version": "v1beta"}, api_key=api_key)

# File paths
BASE_DIR = Path(__file__).parent.parent
REAL_TIME_CALLER_DIR = Path(__file__).parent
LATEST_GAME_STATE_FILE = REAL_TIME_CALLER_DIR / "latest_game_state.json"
GAME_STATE_FILE = BASE_DIR / "game_state.json"
PROMPT_FILE = BASE_DIR / "util" / "Prompt.txt"
AGENT_OUTPUT_FILE = REAL_TIME_CALLER_DIR / "agent_output.txt"

def load_system_prompt():
    """Load base system prompt from Prompt.txt file."""
    try:
        if PROMPT_FILE.exists():
            with open(PROMPT_FILE, 'r', encoding='utf-8') as f:
                prompt = f.read().strip()
                return prompt
        else:
            return "You are DeepBattler, a helpful Hearthstone Battlegrounds assistant. " \
                   "You are in real-time conversation mode. Always analyze the game state JSON " \
                   "provided in the system prompt and give strategic advice based on it."
    except Exception as e:
        print(f"Warning: Could not load prompt file: {e}")
        return "You are DeepBattler, a helpful Hearthstone Battlegrounds assistant."

def build_system_prompt_with_game_state():
    """Build complete system prompt including current game state."""
    base_prompt = load_system_prompt()
    
    # Load current game state
    game_state = load_game_state()
    
    if game_state:
        # Format game state as JSON
        game_state_json = json.dumps(game_state, indent=2, ensure_ascii=False)
        
        # Extract key info for quick reference
        phase = game_state.get("game_state", {}).get("phase", "Unknown")
        turn = game_state.get("game_state", {}).get("turn_number", 0)
        hero = game_state.get("player_hero", {}).get("name", "Unknown")
        health = game_state.get("player_hero", {}).get("current_health", 0)
        gold = game_state.get("resources", {}).get("available_gold", 0)
        tavern_tier = game_state.get("resources", {}).get("tavern_tier", 0)
        
        # Extract detailed information for natural description
        warband = game_state.get("board_state", {}).get("warband", [])
        hand = game_state.get("board_state", {}).get("hand", [])
        tavern_entities = game_state.get("board_state", {}).get("tavern_entities", [])
        hero_power = game_state.get("player_hero", {}).get("hero_power", {})
        hero_power_desc = hero_power.get("description", "")
        tavern_upgrade_cost = game_state.get("resources", {}).get("tavern_upgrade_cost", "None")
        
        # Build natural language description of game state
        warband_desc = f"{len(warband)} minions" if warband else "no minions"
        hand_desc = f"{len(hand)} cards" if hand else "no cards"
        tavern_desc = f"{len(tavern_entities)} options available"
        
        # Build complete prompt with game state (described naturally, not as a file)
        full_prompt = f"""{base_prompt}

================================================================================
CURRENT GAME STATE - USE THIS INFORMATION TO ANSWER QUESTIONS:
================================================================================

You are currently playing as {hero} in Turn {turn}, {phase} phase.

HERO STATUS:
- Hero: {hero}
- Health: {health} HP
- Hero Power: {hero_power_desc}

RESOURCES:
- Available Gold: {gold}
- Tavern Tier: {tavern_tier}
- Tavern Upgrade Cost: {tavern_upgrade_cost}

BOARD STATUS:
- Warband: {warband_desc} on the battlefield
- Hand: {hand_desc} in hand
- Tavern: {tavern_desc} for purchase

DETAILED GAME DATA:
{game_state_json}

================================================================================

IMPORTANT: This is the REAL-TIME game state. Always use the exact details from above when answering questions. Reference the specific hero name ({hero}), minion names, gold amount ({gold}), and all other current game information."""
        
        print(f"[DEBUG] ✓ Built system prompt with game state (Turn: {turn}, Phase: {phase}, Hero: {hero})")
        return full_prompt
    else:
        # No game state available - casual chat mode
        full_prompt = f"""{base_prompt}

================================================================================
CURRENT STATUS: No active game - Casual Chat Mode
================================================================================

You are currently in casual chat mode. The user is not playing a game right now, so feel free to have a friendly, casual conversation. You can:
- Chat about Hearthstone Battlegrounds in general
- Share tips and strategies
- Discuss heroes, minions, or synergies
- Just have a friendly conversation

Keep it light, fun, and conversational. Be yourself - a witty tavern master who loves talking about the game!"""
        
        print(f"[DEBUG] ✓ Built system prompt for casual chat mode (no game state)")
        return full_prompt

def load_game_state():
    """Load current game state from latest_game_state.json only.
    
    No fallback - if file doesn't exist, it means game is still loading.
    Returns the game state dict from latest_game_state.json if available, None otherwise.
    """
    # Only read from latest_game_state.json (written by C# plugin)
    try:
        file_path = LATEST_GAME_STATE_FILE
        print(f"[DEBUG] Checking file: {file_path}")
        print(f"[DEBUG] File exists: {file_path.exists()}")
        
        if file_path.exists():
            # Use utf-8-sig to handle UTF-8 BOM if present
            with open(file_path, 'r', encoding='utf-8-sig') as f:
                content = f.read().strip()
                print(f"[DEBUG] File size: {len(content)} bytes")
                
                # Check if file is empty or just empty JSON object
                if not content or content == "{}":
                    print(f"[DEBUG] ⚠️ File is empty or contains empty JSON object")
                    return None
                
                game_state = json.loads(content)
                # Validate that we got valid game state
                if isinstance(game_state, dict) and 'game_state' in game_state:
                    turn = game_state.get("game_state", {}).get("turn_number", 0)
                    phase = game_state.get("game_state", {}).get("phase", "Unknown")
                    print(f"[DEBUG] ✓ Loaded game state from: {file_path.name} (Turn: {turn}, Phase: {phase})")
                    return game_state
                else:
                    print(f"[DEBUG] ⚠️ Invalid game state structure - missing 'game_state' key")
                    print(f"[DEBUG] Keys found: {list(game_state.keys()) if isinstance(game_state, dict) else 'Not a dict'}")
                    return None
        else:
            # File doesn't exist - game is still loading, don't use fallback
            print(f"[DEBUG] ⚠️ File does not exist: {file_path}")
            return None
    except json.JSONDecodeError as e:
        print(f"[DEBUG] ✗ Invalid JSON in latest_game_state.json: {e}")
        traceback.print_exc()
        return None
    except Exception as e:
        print(f"[DEBUG] ✗ Could not load from latest_game_state.json: {e}")
        traceback.print_exc()
        return None

def is_recruitment_phase(game_state):
    """Check if we're in recruitment phase based on game state."""
    if not game_state:
        return False
    
    phase = game_state.get("game_state", {}).get("phase", "")
    return phase == "PlayerTurn"

def build_context_message(game_state):
    """Build a context message with current game state."""
    if not game_state:
        return "⚠️ WARNING: Current game state is not available. Please wait for the game to start."
    
    # Format game state as a clear, structured message
    context = "\n" + "=" * 70 + "\n"
    context += "📊 CURRENT GAME STATE (JSON) - YOU MUST USE THIS DATA:\n"
    context += "=" * 70 + "\n"
    context += json.dumps(game_state, indent=2, ensure_ascii=False)
    context += "\n" + "=" * 70 + "\n"
    
    # Extract key information for quick reference
    phase = game_state.get("game_state", {}).get("phase", "Unknown")
    turn = game_state.get("game_state", {}).get("turn_number", 0)
    hero = game_state.get("player_hero", {}).get("name", "Unknown")
    health = game_state.get("player_hero", {}).get("current_health", 0)
    gold = game_state.get("resources", {}).get("available_gold", 0)
    tavern_tier = game_state.get("resources", {}).get("tavern_tier", 0)
    warband_size = game_state.get("board_state", {}).get("warband_size", 0)
    tavern_available = game_state.get("board_state", {}).get("tavern_available", 0)
    
    context += f"\n📋 QUICK SUMMARY (for quick reference):\n"
    context += f"  • Turn: {turn}\n"
    context += f"  • Phase: {phase}\n"
    context += f"  • Hero: {hero}\n"
    context += f"  • Health: {health} HP\n"
    context += f"  • Gold: {gold}\n"
    context += f"  • Tavern Tier: {tavern_tier}\n"
    context += f"  • Minions on Board: {warband_size}/7\n"
    context += f"  • Tavern Options Available: {tavern_available}\n"
    
    # Add recruitment phase check
    if is_recruitment_phase(game_state):
        context += f"\n✅ STATUS: Currently in RECRUITMENT PHASE - Player can buy minions, upgrade tavern, etc.\n"
    else:
        context += f"\n⏸️ STATUS: NOT in recruitment phase - Player may be in combat or selecting a hero.\n"
    
    context += "\n" + "=" * 70 + "\n"
    context += "IMPORTANT: Use the JSON data above to answer the user's question. Reference specific details from the game state!\n"
    context += "=" * 70 + "\n"
    
    return context

# Base system prompt (will be enhanced with game state at runtime)
BASE_SYSTEM_PROMPT = load_system_prompt()

pya = pyaudio.PyAudio()

class GameStateAudioLoop:
    def __init__(self):
        self.audio_in_queue = None
        self.out_queue = None
        self.session = None  # Audio session
        self.text_session = None  # Text session (parallel)
        self.conversation_history = []  # Store recent conversation for context
        self.max_history = 5  # Keep last 5 exchanges
        self.last_system_prompt_hash = None  # Track the hash of game state used in system prompt
        self.should_reconnect = False  # Flag to trigger reconnection
        self.reconnect_lock = asyncio.Lock()  # Lock to prevent multiple reconnections
        
        # Text output - using generate_content API
        self.current_text_output = ""
        self.text_output_lock = asyncio.Lock()
        self.conversation_context = []  # Store conversation for text API
        self.last_agent_response_time = 0  # Track when agent last responded
        self.user_input_buffer = ""  # Buffer to collect user input (for context)


    async def listen_audio(self):
        """Capture audio from microphone and send to both audio and text sessions."""
        try:
            try:
                mic_info = pya.get_default_input_device_info()
                self.audio_stream = await asyncio.to_thread(
                    pya.open,
                    format=FORMAT,
                    channels=CHANNELS,
                    rate=SEND_SAMPLE_RATE,
                    input=True,
                    input_device_index=mic_info["index"],
                    frames_per_buffer=CHUNK_SIZE,
                )
                if __debug__:
                    kwargs = {"exception_on_overflow": False}
                else:
                    kwargs = {}
                while True:
                    try:
                        data = await asyncio.to_thread(self.audio_stream.read, CHUNK_SIZE, **kwargs)
                        # Send to audio session
                        await self.out_queue.put({"data": data, "mime_type": "audio/pcm"})
                    except asyncio.CancelledError:
                        raise
                    except Exception as e:
                        print(f"Error reading audio: {e}")
                        await asyncio.sleep(0.1)
            except Exception as e:
                print(f"Error initializing audio input: {e}")
                traceback.print_exc()
                # Keep the task alive but don't send audio
                while True:
                    await asyncio.sleep(1.0)
        except asyncio.CancelledError:
            raise
        except Exception as e:
            print(f"Fatal error in listen_audio: {e}")
            traceback.print_exc()
            raise

    async def send_realtime(self):
        """Send real-time audio data to the session."""
        try:
            while True:
                try:
                    msg = await self.out_queue.get()
                    if self.session:
                        await self.session.send(input=msg)
                except asyncio.CancelledError:
                    raise
                except Exception as e:
                    print(f"Error sending realtime data: {e}")
                    await asyncio.sleep(0.1)
        except asyncio.CancelledError:
            raise
        except Exception as e:
            print(f"Fatal error in send_realtime: {e}")
            traceback.print_exc()
            raise

    # STT methods removed - using generate_content API instead
    
    async def receive_audio(self):
        """Receive audio responses from the session and generate text in parallel."""
        agent_speaking = False
        response_start_time = None
        
        while True:
            try:
                if not self.session:
                    await asyncio.sleep(0.1)
                    continue
                try:
                    turn = self.session.receive()
                    async for response in turn:
                        try:
                            if data := response.data:
                                # Send to audio playback queue
                                self.audio_in_queue.put_nowait(data)
                                
                                # Track when agent starts speaking
                                if not agent_speaking:
                                    agent_speaking = True
                                    response_start_time = time.time()
                                    print("[INFO] Agent started speaking, generating text response...")
                                    # Trigger text generation in parallel (only once per response)
                                    # Use a flag to prevent multiple calls
                                    if not hasattr(self, '_text_generation_in_progress'):
                                        self._text_generation_in_progress = False
                                    
                                    if not self._text_generation_in_progress:
                                        self._text_generation_in_progress = True
                                        asyncio.create_task(self.generate_text_response_parallel())
                                
                                continue
                        except Exception as e:
                            print(f"Error processing audio response: {e}")
                            break
                    
                    # Agent finished speaking
                    if agent_speaking:
                        agent_speaking = False
                        self.last_agent_response_time = time.time()
                        # Reset flag to allow next response
                        if hasattr(self, '_text_generation_in_progress'):
                            self._text_generation_in_progress = False
                        print("[INFO] Agent finished speaking")

                    # Clear queue on interruption
                    while not self.audio_in_queue.empty():
                        try:
                            self.audio_in_queue.get_nowait()
                        except:
                            break
                except StopAsyncIteration:
                    # Normal end of stream
                    await asyncio.sleep(0.1)
                    continue
                except Exception as e:
                    print(f"Error in receive_audio stream: {e}")
                    await asyncio.sleep(0.1)
            except asyncio.CancelledError:
                raise
            except Exception as e:
                print(f"Error receiving audio: {e}")
                traceback.print_exc()
                await asyncio.sleep(0.1)
    
    async def generate_text_response_parallel(self):
        """Generate text response using generate_content API in parallel with audio."""
        try:
            # Build system prompt with current game state
            system_prompt = build_system_prompt_with_game_state()
            
            # Build conversation context for generate_content API
            # Note: We use the same system prompt and conversation history as Live API
            contents = []
            
            # Add system instruction (Gemini API format)
            # For generate_content, we can include system instruction in the first user message
            first_message = f"{system_prompt}\n\nBased on the current game state above, provide a brief strategic suggestion (1-2 sentences max)."
            contents.append({
                "role": "user",
                "parts": [{"text": first_message}]
            })
            
            # Add conversation history (last few exchanges)
            for exchange in self.conversation_context[-4:]:  # Last 4 messages (2 exchanges)
                contents.append(exchange)
            
            # If we have recent user input, add it
            if self.user_input_buffer:
                contents.append({
                    "role": "user",
                    "parts": [{"text": self.user_input_buffer}]
                })
            
            # Call generate_content API with gemini-2.5-flash-lite
            response = await asyncio.to_thread(
                client.models.generate_content,
                model="gemini-2.5-flash-lite",
                contents=contents,
                config={
                    "temperature": 0.7,
                    "max_output_tokens": 150,  # Keep it brief for window display
                }
            )
            
            if response and response.text:
                text_output = response.text.strip()
                
                # Format text output for display
                formatted_output = self.format_text_for_display(text_output)
                
                async with self.text_output_lock:
                    self.current_text_output = formatted_output
                    await self._update_output_file()
                    print(f"[TEXT] ✓ Generated text response: {text_output[:80]}...")
                
                # Update conversation history
                if self.user_input_buffer:
                    self.conversation_context.append({
                        "role": "user",
                        "parts": [{"text": self.user_input_buffer}]
                    })
                    self.user_input_buffer = ""  # Clear buffer
                
                self.conversation_context.append({
                    "role": "model",
                    "parts": [{"text": text_output}]
                })
                
                # Keep history manageable (last 10 messages = 5 exchanges)
                if len(self.conversation_context) > 10:
                    self.conversation_context = self.conversation_context[-10:]
                
        except Exception as e:
            print(f"[WARNING] Could not generate text response: {e}")
            # Don't crash, just log
            import traceback
            traceback.print_exc()
    
    def format_text_for_display(self, text):
        """Format text output for display in the window with better formatting."""
        # Clean up the text
        text = text.strip()
        
        # Remove excessive whitespace
        import re
        text = re.sub(r'\s+', ' ', text)
        
        # Split into sentences (handle multiple punctuation marks)
        sentences = re.split(r'([.!?]+)', text)
        formatted_lines = []
        current_sentence = ""
        
        for i, part in enumerate(sentences):
            if not part:
                continue
            
            # Check if this is punctuation
            if re.match(r'^[.!?]+$', part):
                current_sentence += part
                if current_sentence.strip():
                    formatted_lines.append(current_sentence.strip())
                    current_sentence = ""
            else:
                current_sentence += part
        
        # Add any remaining text
        if current_sentence.strip():
            formatted_lines.append(current_sentence.strip())
        
        # Format with clear line breaks and bullet points for better readability
        if len(formatted_lines) > 1:
            # Multiple sentences - format as bullet points
            result = []
            for sentence in formatted_lines:
                if sentence:
                    result.append(f"• {sentence}")
            formatted_text = '\n\n'.join(result)
        else:
            # Single sentence - just display it
            formatted_text = formatted_lines[0] if formatted_lines else text
        
        return formatted_text
    
    async def _update_output_file(self):
        """Update agent output file with current text output."""
        try:
            if self.current_text_output:
                with open(AGENT_OUTPUT_FILE, 'w', encoding='utf-8') as f:
                    f.write(self.current_text_output)
        except Exception as e:
            print(f"Error writing agent output: {e}")

    async def play_audio(self):
        """Play received audio responses."""
        try:
            try:
                stream = await asyncio.to_thread(
                    pya.open,
                    format=FORMAT,
                    channels=CHANNELS,
                    rate=RECEIVE_SAMPLE_RATE,
                    output=True,
                )
                while True:
                    try:
                        bytestream = await self.audio_in_queue.get()
                        await asyncio.to_thread(stream.write, bytestream)
                    except asyncio.CancelledError:
                        raise
                    except Exception as e:
                        print(f"Error playing audio: {e}")
                        await asyncio.sleep(0.1)
            except Exception as e:
                print(f"Error initializing audio output: {e}")
                traceback.print_exc()
                # Keep the task alive but don't play audio
                while True:
                    await asyncio.sleep(1.0)
        except asyncio.CancelledError:
            raise
        except Exception as e:
            print(f"Fatal error in play_audio: {e}")
            traceback.print_exc()
            raise

    async def monitor_game_state(self):
        """Monitor latest_game_state.json for changes and update system prompt when needed."""
        last_state_hash = None
        
        try:
            while True:
                try:
                    await asyncio.sleep(2.0)  # Check every 2 seconds
                    
                    if not self.session:
                        continue
                    
                    # Load from latest_game_state.json
                    game_state = load_game_state()
                    if not game_state:
                        continue
                    
                    # Create a simple hash of the state to detect changes
                    state_str = json.dumps(game_state, sort_keys=True)
                    current_hash = hash(state_str)
                    current_turn = game_state.get("game_state", {}).get("turn_number", 0)
                    
                    # Check if state changed
                    if current_hash != last_state_hash:
                        last_state_hash = current_hash
                        print(f"[DEBUG] Game state changed (Turn: {current_turn})")
                        
                        # Check if we need to update system prompt (game state content changed)
                        needs_update = False
                        if self.last_system_prompt_hash is None:
                            # First time or system prompt was built without game state
                            # If we now have game state, we need to update
                            if game_state and 'game_state' in game_state:
                                needs_update = True
                                print(f"[DEBUG] 🔄 Game state appeared (was None, now has state), system prompt needs update")
                        elif current_hash != self.last_system_prompt_hash:
                            # Game state content changed - need to update system prompt
                            needs_update = True
                            print(f"[DEBUG] 🔄 Game state content changed, system prompt needs update")
                        
                        if needs_update:
                            # Set flag to trigger reconnection with updated system prompt
                            async with self.reconnect_lock:
                                if not self.should_reconnect:
                                    self.should_reconnect = True
                                    print(f"[INFO] 🔄 Will reconnect to update system prompt with latest game state")
                        
                except asyncio.CancelledError:
                    raise
                except Exception as e:
                    print(f"Error in game state monitor: {e}")
                    await asyncio.sleep(1.0)
        except asyncio.CancelledError:
            raise
        except Exception as e:
            print(f"Fatal error in monitor_game_state: {e}")
            traceback.print_exc()
            raise

    async def run_session(self):
        """Run a single session with current game state."""
        # Build system prompt with current game state
        print(f"[DEBUG] ========== Building system prompt ==========")
        print(f"[DEBUG] File path: {LATEST_GAME_STATE_FILE}")
        print(f"[DEBUG] Absolute path: {LATEST_GAME_STATE_FILE.absolute()}")
        
        system_prompt_with_state = build_system_prompt_with_game_state()
        
        # Track the hash of game state used in this system prompt
        game_state = load_game_state()
        if game_state:
            state_str = json.dumps(game_state, sort_keys=True)
            self.last_system_prompt_hash = hash(state_str)
            turn = game_state.get("game_state", {}).get("turn_number", 0)
            phase = game_state.get("game_state", {}).get("phase", "Unknown")
            print(f"[DEBUG] ✓ System prompt built with game state (Turn: {turn}, Phase: {phase})")
            print(f"[DEBUG] System prompt hash: {self.last_system_prompt_hash}")
        else:
            self.last_system_prompt_hash = None
            print(f"[DEBUG] ⚠️ System prompt built WITHOUT game state")
        
        # Create config for audio session only
        # Note: Gemini Live API doesn't support TEXT response mode
        # We'll use generate_content API in parallel for text output
        audio_config = {
            "response_modalities": ["AUDIO"],
            "system_instruction": system_prompt_with_state
        }
        
        print(f"[DEBUG] Connecting to Gemini Live (Audio) with system prompt (length: {len(system_prompt_with_state)} chars)")
        print(f"[INFO] Text output will use generate_content API in parallel (when user speech is detected)")
        
        # Create audio session
        audio_session_ctx = client.aio.live.connect(model=MODEL, config=audio_config)
        audio_session = await audio_session_ctx.__aenter__()
        self.session = audio_session
        
        try:
            self.audio_in_queue = asyncio.Queue()
            self.out_queue = asyncio.Queue(maxsize=5)

            # Game state is already in system prompt
            game_state = load_game_state()
            if game_state:
                phase = game_state.get("game_state", {}).get("phase", "Unknown")
                turn = game_state.get("game_state", {}).get("turn_number", 0)
                hero = game_state.get("player_hero", {}).get("name", "Unknown")
                health = game_state.get("player_hero", {}).get("current_health", 30)
                gold = game_state.get("resources", {}).get("available_gold", 0)
                print(f"[INFO] ✓ DeepBattler is ready!")
                print(f"[INFO]   📊 Current game state loaded:")
                print(f"[INFO]     - Turn: {turn}, Phase: {phase}")
                print(f"[INFO]     - Hero: {hero}, Health: {health} HP, Gold: {gold}")
                print(f"[INFO] 🎤 Speak to the microphone to ask questions\n")
            else:
                print("[INFO] 💬 DeepBattler is ready for casual chat!")
                print(f"[INFO]   - No active game detected - let's chat!")
                print(f"[INFO] 🎤 Speak to the microphone to start a conversation\n")

            # Create all tasks (voice-only mode, no text input)
            tasks = []
            try:
                async with asyncio.TaskGroup() as tg:
                    send_realtime_task = tg.create_task(self.send_realtime())
                    tasks.append(("send_realtime", send_realtime_task))
                    
                    listen_audio_task = tg.create_task(self.listen_audio())
                    tasks.append(("listen_audio", listen_audio_task))
                    
                    receive_audio_task = tg.create_task(self.receive_audio())
                    tasks.append(("receive_audio", receive_audio_task))
                    
                    play_audio_task = tg.create_task(self.play_audio())
                    tasks.append(("play_audio", play_audio_task))
                    
                    # Note: Text generation will be triggered when we detect user speech
                    # We'll use generate_content API in parallel (not a separate session)
                    
                    monitor_task = tg.create_task(self.monitor_game_state())
                    tasks.append(("monitor_game_state", monitor_task))
                    
                    # Keep running until cancelled or reconnection needed
                    print(f"[INFO] 🎤 Voice-only mode active. Speak to the microphone to ask questions.")
                    print(f"[INFO] 🔄 System prompt will auto-update when game state changes.")
                    print(f"[INFO] Press Ctrl+C to exit.\n")
                    
                    # Wait for monitor task or reconnection signal
                    while True:
                        await asyncio.sleep(1.0)
                        if self.should_reconnect:
                            print(f"[INFO] 🔄 Reconnection requested, closing current session...")
                            # Cancel all tasks to trigger reconnection
                            for task in tasks:
                                if not task.done():
                                    task.cancel()
                            break
            except Exception as e:
                # Handle exceptions from TaskGroup
                if not self.should_reconnect:  # Only print error if not intentional reconnection
                    print(f"Error in TaskGroup: {e}")
                    traceback.print_exception(type(e), e, e.__traceback__)
                    # Don't raise if it's an intentional reconnection
        finally:
            # Clean up audio session
            if audio_session_ctx and audio_session:
                try:
                    await audio_session_ctx.__aexit__(None, None, None)
                except:
                    pass

    async def run(self):
        """Main run loop with automatic reconnection for system prompt updates."""
        try:
            # Clear latest_game_state.json content at startup to avoid using stale data
            try:
                # Create directory if it doesn't exist
                LATEST_GAME_STATE_FILE.parent.mkdir(parents=True, exist_ok=True)
                # Write empty JSON object to clear the file
                with open(LATEST_GAME_STATE_FILE, 'w', encoding='utf-8') as f:
                    json.dump({}, f, ensure_ascii=False)
                print(f"[INFO] 🗑️  Cleared content of {LATEST_GAME_STATE_FILE.name} at startup")
            except Exception as e:
                print(f"[DEBUG] Could not clear latest_game_state.json: {e}")
            
            print(f"[INFO] 🎮 Ready! If no game is active, we'll chat casually. When a game starts, I'll provide strategic advice.")
            
            # Run session loop with automatic reconnection
            while True:
                try:
                    await self.run_session()
                    
                    # If reconnection was requested, reset flag and reconnect
                    if self.should_reconnect:
                        async with self.reconnect_lock:
                            self.should_reconnect = False
                        print(f"[INFO] 🔄 Reconnecting with updated system prompt...")
                        await asyncio.sleep(2.0)  # Brief delay before reconnection
                        continue
                    else:
                        # Normal exit
                        break
                        
                except asyncio.CancelledError:
                    raise
                except Exception as e:
                    if not self.should_reconnect:
                        print(f"Error in session: {e}")
                        traceback.print_exc()
                        break
                    # If reconnection was requested, continue loop
                    await asyncio.sleep(2.0)

        except asyncio.CancelledError:
            print("Cancelled by user")
        except ExceptionGroup as eg:
            print("ExceptionGroup caught:")
            for exc in eg.exceptions:
                print(f"  Exception: {exc}")
                traceback.print_exception(type(exc), exc, exc.__traceback__)
        except Exception as e:
            print(f"Error in run(): {e}")
            traceback.print_exc()
        finally:
            # Cleanup
            if hasattr(self, 'audio_stream') and self.audio_stream:
                try:
                    self.audio_stream.close()
                except:
                    pass
            print("Exiting...")


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="DeepBattler Real-time Assistant")
    parser.add_argument(
        "--game-state",
        type=str,
        default=str(GAME_STATE_FILE),
        help="Path to game_state.json file",
    )
    parser.add_argument(
        "--prompt",
        type=str,
        default=str(PROMPT_FILE),
        help="Path to Prompt.txt file",
    )
    args = parser.parse_args()
    
    # Update file paths if provided
    if args.game_state:
        GAME_STATE_FILE = Path(args.game_state)
    if args.prompt:
        PROMPT_FILE = Path(args.prompt)
    
    main = GameStateAudioLoop()
    asyncio.run(main.run())
