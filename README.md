# Input Prompts

## Türkçe

Input Prompts, oyunlarda `"Press X to Jump"` gibi metinlerde geçen tuş bilgisini, kullanılan input cihazına göre ikon olarak göstermenizi sağlayan bir Unity paketidir.

Paketi indirdikten sonra TextMeshPro componenti bulunan objeye paketle birlikte gelen `SetTextToBox` scriptini ekleyin. Script içindeki `Input Action Reference` alanına hangi input action’ın gösterilmesini istiyorsanız onu seçin. `Sprite Library` alanına ise paketle birlikte gelen ScriptableObject’i ekleyin. Bu ScriptableObject ikonları tutar. Temel kullanım bu kadar basittir.

Kolaylık olması için pakete Xelu Free Icons kullanılarak hazırlanmış hazır bir kurulum eklenmiştir. İsterseniz bunu doğrudan kullanabilirsiniz.

Kendi ikonlarınızı kullanmak isterseniz önce ikonlarınızı bir sprite sheet haline getirin. Daha sonra sprite sheet’i Unity içinde `Sprite Editor` ile açıp eşit parçalara bölün. Ardından sprite sheet’e sağ tıklayıp `Create > TextMeshPro > Sprite Asset` seçeneği ile TMP Sprite Asset oluşturun. Son olarak bu asset’i paketle birlikte gelen `Input Prompt Sprite Library` içindeki uygun alana yerleştirin.

Umarım faydalı olur.

---

## English

Input Prompts is a Unity package that lets you display the correct icon in texts like `"Press X to Jump"` based on the active input device.

After installing the package, add the `SetTextToBox` script to the object that has a TextMeshPro component. In the script, assign the `Input Action Reference` for the input you want to display. Then assign the ScriptableObject included with the package to the `Sprite Library` field. This ScriptableObject stores the icons. That is all you need for the basic setup.

For convenience, the package already includes a ready-to-use setup based on Xelu Free Icons. You can use it directly if you want.

If you want to use your own icons, first turn them into a sprite sheet. Then open the sprite sheet in Unity with `Sprite Editor` and slice it into equal parts. After that, right click the sprite sheet and create a TMP Sprite Asset with `Create > TextMeshPro > Sprite Asset`. Finally, place that asset into the correct field inside the included `Input Prompt Sprite Library`.

Hope it helps.
