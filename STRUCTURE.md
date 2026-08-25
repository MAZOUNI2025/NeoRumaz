# بنية NeoRumaz

## مبدأ الملكية

React هو إطار الشاشة فقط، وBabylon.js يملك الـcanvas والمشهد والكاميرا والخامات والإضاءة. منطق اللعبة سيبقى في كائنات TypeScript مستقلة داخل `client/src/game/`، بحيث لا تعتمد قواعد اللعب على React أو على عناصر DOM. كل كائن لعب يملك mesh أو مجموعة meshes الخاصة به ويهتم بتحديثها وتحريرها.

## الوحدات

| الوحدة | المسؤولية | لا تملك |
| --- | --- | --- |
| `GameWorld` | تركيب المشهد، دورة التحديث، إدارة الحالة العامة، وربط الأنظمة | تفاصيل أزرار React |
| `GameState` | حالات menu/run/paused/gameOver/shop، النتيجة، العملات، revive | إنشاء meshes مباشرة |
| `InputManager` | تحويل keyboard/pointer/touch إلى أفعال semantic مع تنظيف المستمعات | تغيير score أو الحفظ |
| `Player` | lane target، الحركة، القفز، shield، الحالة، mesh والظل | قواعد التوليد |
| `ObstacleManager` | pooling للعوائق، اختيار موجات آمنة، الحركة وإعادة التدوير | تفاصيل HUD |
| `PickupManager` | pooling للعملات والقوة، دوران/توهج، جمع ومكافآت | التنقل بين الشاشات |
| `TrackManager` | مقاطع الطريق، خطوط المسارات، حواف الطريق، props الخلفية وإعادة الاستخدام | الحفظ |
| `CameraController` | متابعة اللاعب، التنعيم، FOV المحدود، وإعادة الضبط | اتخاذ قرار collision |
| `EffectsManager` | حلقات جمع، انفجار، shield pulse، speed streaks منخفضة الكلفة | تحديث ملفات الحفظ |
| `AudioManager` | فتح سياق الصوت بعد gesture، أصوات UI/collect/hit/jump، mute | منطق اللعب |
| `SaveManager` | versioned localStorage، التحقق والترحيل، حفظ atomically | إدارة الـDOM |
| `MonetizationService` | واجهة offline-safe لمحاكاة rewarded/interstitial، حماية callback المكرر | إيقاف gameplay |
| `UIController` | ربط HUD والقائمة والمتجر وشاشات pause/game over مع أحداث GameWorld | تحريك اللاعب مباشرة |
| `scene.ts` | إنشاء Babylon scene وإرجاع `GameHandle` وdispose آمن | حفظ أو قواعد تجارية |

## دورة الحالة

الحالات المسموح بها هي `menu -> run -> paused -> run`, و`run -> gameOver -> run`, و`menu -> shop -> menu`. لا يُسمح بتشغيل تحديث العالم أثناء `menu`, `paused`, أو `gameOver`. عند game over تُحسب مكافأة الجولة مرة واحدة، ثم تُحفظ النتيجة والعملات قبل عرض الخيارات.

## نموذج البيانات

```ts
interface SaveData {
  version: 1;
  coins: number;
  bestScore: number;
  selectedCharacter: number;
  unlockedCharacters: number[];
  musicEnabled: boolean;
  runsSinceInterstitial: number;
}
```

الشخصيات cosmetic في النسخة الأولى مع اختلافات مظهرية خفيفة فقط حتى لا تصبح التجارة pay-to-win. عناصر العالم procedural: road tiles، barrier، drone، coin، shield، city blocks، billboard، player silhouette. الأصول المولدة تستخدم للهوية البصرية والبطاقات/الشعارات أو كخامات خفيفة إن لزم، بينما لا يعتمد اللعب على تحميل GLB.

## التكامل مع React

`GameCanvas.tsx` ينشئ Engine مرة واحدة ويحمي StrictMode، ثم يستدعي `createGameScene(engine, canvas)`. يبدأ render loop بعد جاهزية المشهد، ويزيل listeners ويستدعي `dispose()` عند unmount. `App.tsx` يعرض canvas الكامل مع طبقة UI DOM خفيفة فوقه عند الحاجة، ويظل كل منطق الحركة والتوليد داخل `game/`.
