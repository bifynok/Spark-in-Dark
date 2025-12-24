# Spark in Dark

**Spark in Dark** — хардкорна покрокова тактична rogue-like гра з елементами вибору шляху.

## Про гру

- **Жанр:** Rogue-like, Tactical Turn-Based RPG  
- **Натхнення:** Heroes of Might and Magic III, King Arthur: Knight's Tale, Slay the Spire  
- **Платформа:** PC (Windows / macOS / Linux)
- **Рушій:** Unity
- **Статус:** In development

Гравець керує невеликим загоном, обираючи шлях на процедурно-генерованій мапі. Кожне рішення має наслідки, а смерть означає кінець забігу.

Детальніше можна прочитати тут: [Spark in Dark (Дизайн-документ)](Spark%20in%20Dark%20(%D0%94%D0%B8%D0%B7%D0%B0%D0%B9%D0%BD%20%D0%B4%D0%BE%D0%BA%D1%83%D0%BC%D0%B5%D0%BD%D1%82).pdf)

## Поточна реалізація

На даний момент у проєкті реалізовано:

- Процедурна мапа з вузлами (battle / event / shop / treasure тощо)
- Вибір шляху з блокуванням альтернатив
- Збереження та відновлення стану мапи між сценами
- Additive-завантаження сцен (Main Menu / Map / Battle)
- Базова UI-навігація (миша + клавіатура)

## Архітектура

Основні системи:

- **[GameManager](Spark%20in%20Dark/Assets/Scripts/Core/GameManager.cs) / [RunManager](Spark%20in%20Dark/Assets/Scripts/Core/RunManager.cs)** — керування життєвим циклом гри та забігу  
- **[SceneLoader](Spark%20in%20Dark/Assets/Scripts/Core/SceneLoader.cs)** — завантажувач сцен в адитивному режимі  
- **[MapManager](Spark%20in%20Dark/Assets/Scripts/Map/MapManager.cs)** — генерація, логіка та відновлення мапи 
- **[MapNode](Spark%20in%20Dark/Assets/Scripts/Map/MapNode.cs) / [MapNodeView](Spark%20in%20Dark/Assets/Scripts/Map/MapNodeView.cs)** — сутність та зовнішність вузлів мапи  
- **[GlobalEventSystem](Spark%20in%20Dark/Assets/Scripts/Core/GlobalEventSystem.cs) / [GlobalAudioListener](Spark%20in%20Dark/Assets/Scripts/Core/GlobalAudioListener.cs)** — глобальні сервіси  

## Найближчі плани

- Рефакторинг наявного коду
- Реалізація базової бойової системи
- Додавання сцени налаштувань
- Реалізація базового збереження/завантаження прогресу
