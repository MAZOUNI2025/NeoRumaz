# ذاكرة NeoRumaz

## اكتشافات المصدر

المصدر هو مشروع Unity 2020.3.26f1 باسم Lets Run، مبني كلعبة 2D sprite endless runner. يحتوي على مشهدي MainMenu وGameplay، ومدير حفظ، مدير قائمة، اختيار تسعة أبطال، مولد خريطة قائم على tiles، 21 مجموعة obstacle holder، power-up باسم Trex، نجوم كعملة، صوت ومؤثرات انفجار، ومدخلات swipe/keyboard. اللاعب الأصلي يبدل بين مستويين أفقيين ويقفز، بينما الكاميرا تتحرك على محور X والـobstacles تتحرك عكسيًا.

## عيوب المصدر التي لن تُنقل كما هي

لن تُنقل قيمة الاختبار الأولية 9000 عملة. لن يُستخدم BinaryFormatter أو catch صامت. لن تُنقل مشكلة `Input.touches.Length < 0`. لن يعتمد التوليد على flag واحد أو على قائمة scene-authored ثابتة. لن تُكرر أصوات الموت. لن تُستخدم lookup-by-name كاعتماد أساسي في طبقة اللعب.

## قرار المنصة

المشروع المستهدف في GitHub فارغ عمليًا باستثناء README. بيئة التنفيذ المتاحة لهذا المسار هي لعبة متصفح Babylon.js داخل WebDev React، لذلك سيُحافظ على تجربة mobile-first ويُوثق أن APK Unity native غير مُنتج ما لم تظهر بيئة بناء فعلية.

## قرارات التصميم

تم اعتماد طريق ثلاثي المسارات بدل مستويين، مع إبقاء الفعلين الأساسيين: التبديل والقفز. تمت إضافة shield واضح كتحسين مباشر لفكرة Trex، مع coins وspeed tiers وrevive تجريبي offline-safe. الشخصيات cosmetic وليست pay-to-win. لا يوجد backend أو multiplayer أو حسابات.

## خطوات لاحقة

بعد تهيئة WebDev: إضافة Babylon، نسخ GameCanvas lifecycle-safe، وضع createGameScene وGameWorld والأنظمة المستقلة، ثم توليد أصول الهوية والبطاقات ورفعها عند الحاجة. يجب اختبار `?demo` و`pnpm check` ولقطات المعاينة قبل checkpoint والدفع إلى GitHub.
