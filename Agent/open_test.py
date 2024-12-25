import os
from openai import OpenAI

from dotenv import load_dotenv
load_dotenv()

client = OpenAI(api_key=os.environ.get("API_KEY"),base_url=os.getenv("Base_URL"))
language=os.environ.get("Language", "English")
chat_completion = client.chat.completions.create(
    messages=[
    {
      "role": "user",
      "content": "Say this is a test"+f"Please respond in {language}. ", 
    },

],
model="gpt-3.5-turbo",
)
# if ok  will print  "This is a test. How can I assist you further?""
print(chat_completion.choices[0].message.content)