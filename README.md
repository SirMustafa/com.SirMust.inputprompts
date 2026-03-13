# Input Prompts

**TR:**  
`Input Prompts`, TextMeshPro kullanan UI metinlerinde belirli anahtar kelimeleri algılayıp bunları otomatik olarak kontrolcü ikonlarına dönüştüren bir Unity paketidir.

Örneğin metinde:

`Press BUTTONPROMPT to Jump`

yazıyorsa, bu paket `BUTTONPROMPT` ifadesini oyuncunun aktif cihazına göre uygun sprite etiketiyle değiştirir.

Örnek çıktı:

`Press <sprite="Ps5_SpriteSheet" name="DualShock_buttonNorth"> to Jump`

Böylece oyuncu Xbox ile oynuyorsa Xbox tuş ikonu, PlayStation ile oynuyorsa PlayStation tuş ikonu, klavye/fare kullanıyorsa ilgili klavye tuş görseli gösterilebilir.

---

**EN:**  
`Input Prompts` is a Unity package that detects specific keywords inside TextMeshPro UI texts and automatically converts them into controller icons.

For example, if your text contains:

`Press BUTTONPROMPT to Jump`

the package replaces `BUTTONPROMPT` with the correct sprite tag based on the player's current input device.

Example output:

`Press <sprite="Ps5_SpriteSheet" name="DualShock_buttonNorth"> to Jump`

This allows the UI to display an Xbox button icon when playing with Xbox, a PlayStation icon when playing with PlayStation, or keyboard visuals when using keyboard and mouse.

---

## Features

### TR
- TextMeshPro metinleri içinde anahtar kelime yakalama
- Anahtar kelimeyi TMP sprite etiketi ile değiştirme
- Klavye/Fare, Xbox ve PlayStation desteği
- Aktif input cihazına göre doğru ikon gösterimi
- Kolay kurulum ve kullanım

### EN
- Detects keywords inside TextMeshPro texts
- Replaces keywords with TMP sprite tags
- Supports Keyboard/Mouse, Xbox, and PlayStation
- Displays the correct icon based on the active input device
- Easy to set up and use

---

## Installation

### Git URL
Add the package to your project using Unity Package Manager and this Git URL:

```text
https://github.com/SirMustafa/com.SirMust.inputprompts.git
