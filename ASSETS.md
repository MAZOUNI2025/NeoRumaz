# أصول NeoRumaz

## اتجاه الفن

هوية **premium cyber-city runner** مناسبة للشاشات العمودية والأفقية، مع لقطة لعب خلفية مرتفعة وطريق ثلاثي المسارات واضح. لوحة الألوان الأساسية هي graphite `#101827` للخلفية والطريق، cyan `#42E8FF` للخطوط والعملات، magenta `#FF4FD8` للعوائق والتنبيهات، amber `#FFC857` للقوة والمكافآت، وoff-white للنص. الخامات نظيفة وحادة، بإضاءة emissive محدودة، وبدون fog أو motion blur أو lens flare أو particle spam.

## المرجع المرئي

| الأصل | الحالة | الاستخدام |
| --- | --- | --- |
| `reference.png` | مولد ومحفوظ في `/home/ubuntu/work/NeoRumaz/reference.png` | مرجع الكاميرا، المقياس، الكثافة، UI، والألوان أثناء البناء والفحص |
| `neorumaz-symbol` | `/manus-storage/neorumaz-symbol_49b3ba69.png` | رمز العلامة في العنوان وfavicon |
| `neorumaz-menu-city` | `/manus-storage/neorumaz-menu-city_9f20a22d.jpg` | خلفية شاشة القائمة المرئية |
| `neorumaz-courier-vanta` | `/manus-storage/neorumaz-courier-vanta_17506b9c.png` | بطاقة الشخصية الافتراضية |
| `neorumaz-courier-lyra` | `/manus-storage/neorumaz-courier-lyra_eabc52b6.png` | بطاقة شخصية قابلة للفتح |
| `neorumaz-courier-oren` | `/manus-storage/neorumaz-courier-oren_b5f1c92f.png` | بطاقة شخصية قابلة للفتح |
| `neorumaz-cairo-menu` | `/manus-storage/neorumaz-cairo-menu_5f2d1ac3.jpg` | خلفية قائمة Nile Circuit المصرية |
| `neorumaz-nile-garage` | `/manus-storage/neorumaz-nile-garage_80c7869a.jpg` | خلفية Runner Garage المطلة على النيل |
| `neorumaz-scarab-nile-icon` | `/manus-storage/neorumaz-scarab-nile-icon_d1b62af8.png` | شارة Scarab Shield وNile Rush داخل الـHUD |
| `neorumaz-cairo-route-panel` | `/manus-storage/neorumaz-cairo-route-panel_660de126.png` | لوحة طريق قاهرية تستخدم كـbillboard وعلامة عقد |

الصورة المرجعية تمثل لقطة لعب فعلية: لاعب courier في الوسط، barrier برتقالي في اليسار، drone magenta في اليمين، صف خمس عملات cyan في الوسط، وshield amber فوق اليسار-الوسط، مع طريق ثلاثي المسارات، مدينة ليلية، HUD للنتيجة والعملات، pause، وhint للقفز.

## أصول ستُنفذ إجرائيًا

| الأصل | التقنية | المتطلبات |
| --- | --- | --- |
| الطريق والحوامل | Babylon meshes وخامات emissive | ثلاثة مسارات، حواجز منخفضة، خطوط cyan قابلة للتكرار |
| اللاعب | مجموعة meshes بسيطة مع خامات graphite/cyan/magenta | silhouette واضح، run bob، jump arc، shield ring |
| barrier | boxes/beams procedural | orange striped material، قراءة فورية، collider منطقي |
| drone | boxes + four rotor discs | magenta glow، موضع فوق المسار، collider عادل |
| coin | cylinder/torus مع emissive cyan | دوران ثابت، arc patterns، effect عند الجمع |
| shield | torus + inner disc amber | مدة مؤقتة، ring حول اللاعب، حماية من ضربة واحدة/زمن قصير |
| المدينة | blocks بسيطة متعددة الارتفاعات | طبقات skyline، نوافذ cyan قليلة، لا تحميل نماذج خارجية |
| props | billboards/vans مبسطة | خارج مسارات اللعب، كثافة منخفضة |

## الأصول المولدة المنفذة

| الأصل | الغرض | حالة |
| --- | --- | --- |
| شعار NeoRumaz | عنوان القائمة وبطاقة الهوية | منفذ عبر `neorumaz-symbol` |
| خلفية city panel | الخلفية الرئيسية للقائمة | منفذ عبر `neorumaz-menu-city` |
| بطاقات الشخصيات | عرض المتجر، مع 3 شخصيات مميزة | منفذ عبر صور Vanta وLyra وOren |

لا تُحفظ ملفات الصور الكبيرة داخل شجرة المشروع النهائية إذا كان WebDev يوفر تخزينًا خارجيًا؛ تُرفع الأصول النهائية إلى تخزين WebDev وتُسجل روابطها هنا بعد الرفع.

## Cairo After Dark

الأصول الجديدة تجعل Cairo/Nile بيئة قابلة للرؤية في القائمة والمتجر والـHUD، بينما تظل عوائق اللعب والعناصر الرئيسية procedural كي تكون قابلة لإعادة التدوير ولا تعتمد على تحميل نماذج كبيرة. يُستخدم رمز scarab كإشارة حماية وظيفية، وتبقى العناصر التراثية مجردة وحديثة حتى لا تتحول البيئة إلى كليشيه سياحي.
