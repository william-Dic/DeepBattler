<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>DeepBattler Multilingual Page</title>
    <style>
        body {
            font-family: Arial, sans-serif;
        }
        .content {
            display: none;
        }
        .content.active {
            display: block;
        }
        .language-selector {
            margin-bottom: 20px;
        }
    </style>
</head>
<body>
    <h1>DeepBattler Multilingual Page</h1>
    <div class="language-selector">
        <label for="language">Choose your language: </label>
        <select id="language">
            <option value="en" selected>English</option>
            <option value="zh">中文</option>
            <option value="jp">日本語</option>
        </select>
    </div>

    <div id="content-en" class="content active">
        <h2>Well met, hero!</h2>
        <p>I’m DeepBattler, your LLM-Powered Battlegrounds Coach! 🍻🍻</p>
    </div>

    <div id="content-zh" class="content">
        <h2>英雄，好久不见！</h2>
        <p>我是DeepBattler，你的Battlegrounds LLM智能教练！🍻🍻</p>
    </div>

    <div id="content-jp" class="content">
        <h2>お久しぶりです、英雄！</h2>
        <p>私はDeepBattler、バトルグラウンドのAIコーチです！🍻🍻</p>
    </div>

    <script>
        const languageSelector = document.getElementById('language');
        const contentSections = document.querySelectorAll('.content');

        languageSelector.addEventListener('change', (event) => {
            const selectedLanguage = event.target.value;
            contentSections.forEach(section => {
                section.classList.remove('active');
                if (section.id === `content-${selectedLanguage}`) {
                    section.classList.add('active');
                }
            });
        });
    </script>
</body>
</html>
