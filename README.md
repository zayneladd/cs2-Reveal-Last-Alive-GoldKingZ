## .:[ Join Our Discord For Support ]:.

<a href="https://discord.com/invite/U7AuQhu"><img src="https://discord.com/api/guilds/651838917687115806/widget.png?style=banner2"></a>

# [CS2] Reveal-Last-Alive-GoldKingZ (1.0.0)

Reveal Last Player Alive By Glow/Chicken

![glow](https://github.com/user-attachments/assets/badb4691-d0f4-433e-808c-8ebb7176ed25)


---

## 📦 Dependencies
[![Metamod:Source](https://img.shields.io/badge/Metamod:Source-2d2d2d?logo=sourceengine)](https://www.sourcemm.net)

[![CounterStrikeSharp](https://img.shields.io/badge/CounterStrikeSharp-83358F)](https://github.com/roflmuffin/CounterStrikeSharp)

[![MultiAddonManager](https://img.shields.io/badge/MultiAddonManager-181717?logo=github&logoColor=white)](https://github.com/Source2ZE/MultiAddonManager) [Optional: If You Want Custom Sounds]

[![JSON](https://img.shields.io/badge/JSON-000000?logo=json)](https://www.newtonsoft.com/json) [Included in zip]


---

## 📥 Installation

### Plugin Installation
1. Download the latest `Reveal-Last-Alive-GoldKingZ.x.x.x.zip` release
2. Extract contents to your `csgo` directory
3. Configure settings in `Reveal-Last-Alive-GoldKingZ/config/config.json`
4. Restart your server

---

## 🛠️ `config.json`


<details open>
<summary><b>Main Config</b> (Click to expand 🔽)</summary>

| Property | Description | Values | Required |  
|----------|-------------|--------|----------|
| `RevealLastPlayerOnTeam` | Team to reveal last player | `1`-CT<br>`2`-T | - |
| `Play_Sound` | Sound played on reveal | Sound path/event<br>`""`-Disabled | - |
| `Sound_Volume` | Volume for non-"sounds/" audio | `0%` to `100%` | When using without sounds/ |
| `Chicken_Enable` | Enable chicken indicator | `true`-Yes<br>`false`-No | - |
| `Chicken_GlowType` | Chicken glow visibility | `true`-Near crosshair<br>`false`-Always | `Chicken_Enable=true` |
| `Chicken_GlowRange` | Max chicken glow distance | Number (e.g. `5000`) | `Chicken_Enable=true` |
| `Chicken_Size` | Chicken model size | Number (e.g. `10`) | `Chicken_Enable=true` |
| `Chicken_GlowColor` | Chicken glow color | Hex code (e.g. `#14ff00`) | `Chicken_Enable=true` |
| `Player_Enable` | Enable player glow | `true`-Yes<br>`false`-No | - |
| `Player_GlowType` | Player glow visibility | `true`-Near crosshair<br>`false`-Always | `Player_Enable=true` |
| `Player_GlowRange` | Max player glow distance | Number (e.g. `5000`) | `Player_Enable=true` |
| `Player_GlowColor` | Player glow color | Hex code (e.g. `#14ff00`) | `Player_Enable=true` |

</details>


<details>
<summary><b>Utilities Config</b> (Click to expand 🔽)</summary>

| Property | Description | Values | Required |  
|----------|-------------|--------|----------|
| `AutoUpdateSignatures` | Auto-Update Signatures | `true`-Yes<br>`false`-No | - |
| `EnableDebug` | Debug Mode | `true`-Enable<br>`false`-Disable | - |

</details>

---


## 📜 Changelog

<details>
<summary><b>📋 View Version History</b> (Click to expand 🔽)</summary>

### [1.0.0]
- Initial plugin release

</details>

---
