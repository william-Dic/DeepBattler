"""
Test script to check if Gemini API can return text directly.
This will help us decide whether to use dual API calls (one for audio, one for text).
"""

import asyncio
import os
import sys
from pathlib import Path
from dotenv import load_dotenv

# Add parent directory to path
sys.path.insert(0, str(Path(__file__).parent.parent))

from google import genai

load_dotenv()

api_key = os.environ.get('GEMINI_API_KEY')
if not api_key:
    print("Error: GEMINI_API_KEY not found")
    sys.exit(1)

client = genai.Client(http_options={"api_version": "v1beta"}, api_key=api_key)

async def test_text_response():
    """Test if we can get text response from Gemini Live API."""
    print("Testing Gemini Live API with TEXT response modality...")
    
    MODEL = "models/gemini-2.0-flash-live-001"
    
    # Test 1: Try TEXT only (but Gemini Live might only support AUDIO)
    print("\n=== Test 1: TEXT only (may not be supported) ===")
    try:
        config_text_only = {
            "response_modalities": ["TEXT"],
            "system_instruction": "You are a helpful assistant. Respond briefly."
        }
        
        async with client.aio.live.connect(model=MODEL, config=config_text_only) as session:
            # Send a test message using send_realtime_input
            test_message = "Say hello in one sentence."
            print(f"Sending: {test_message}")
            
            # Try using send_realtime_input instead of send
            try:
                await session.send_realtime_input(input={"mime_type": "text/plain", "data": test_message.encode('utf-8')})
            except AttributeError:
                # Fallback to send if send_realtime_input doesn't exist
                await session.send(input={"mime_type": "text/plain", "data": test_message.encode('utf-8')})
            
            # Receive response
            turn = session.receive()
            async for response in turn:
                print(f"Response type: {type(response)}")
                print(f"Response attributes: {dir(response)}")
                
                # Check for text
                if hasattr(response, 'text') and response.text:
                    print(f"✓ Got TEXT: {response.text}")
                elif hasattr(response, 'data'):
                    print(f"Got DATA (audio): {len(response.data)} bytes")
                else:
                    print(f"Response: {response}")
                    
    except Exception as e:
        print(f"✗ Test 1 failed: {e}")
        import traceback
        traceback.print_exc()
    
    # Test 2: Try TEXT and AUDIO (may not be supported)
    print("\n=== Test 2: TEXT and AUDIO (may not be supported) ===")
    try:
        config_both = {
            "response_modalities": ["TEXT", "AUDIO"],
            "system_instruction": "You are a helpful assistant. Respond briefly."
        }
        
        async with client.aio.live.connect(model=MODEL, config=config_both) as session:
            test_message = "Say hello in one sentence."
            print(f"Sending: {test_message}")
            
            try:
                await session.send_realtime_input(input={"mime_type": "text/plain", "data": test_message.encode('utf-8')})
            except AttributeError:
                await session.send(input={"mime_type": "text/plain", "data": test_message.encode('utf-8')})
            
            turn = session.receive()
            async for response in turn:
                print(f"Response type: {type(response)}")
                
                if hasattr(response, 'text') and response.text:
                    print(f"✓ Got TEXT: {response.text}")
                elif hasattr(response, 'data'):
                    print(f"Got DATA (audio): {len(response.data)} bytes")
                else:
                    print(f"Response: {response}")
                    
    except Exception as e:
        print(f"✗ Test 2 failed: {e}")
        import traceback
        traceback.print_exc()
    
    # Test 3: Use regular generate_content API (non-live) for text
    print("\n=== Test 3: Regular generate_content API (text only) ===")
    try:
        response = client.models.generate_content(
            model="gemini-2.0-flash-exp",
            contents="Say hello in one sentence."
        )
        print(f"✓ Got TEXT from generate_content: {response.text}")
        return True
    except Exception as e:
        print(f"✗ Test 3 failed: {e}")
        import traceback
        traceback.print_exc()
        return False

if __name__ == "__main__":
    result = asyncio.run(test_text_response())
    if result:
        print("\n✓ Text API is available! We can use dual API calls.")
    else:
        print("\n✗ Text API test failed. May need to use STT instead.")

