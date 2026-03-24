import React from 'react';
import { motion } from 'motion/react';
import { 
  TrendingUp, Globe, ShieldCheck, Coins, Target, Award, 
  PieChart, AlertTriangle, CheckCircle2, DollarSign, Users, BarChart3
} from 'lucide-react';

const Section = ({ title, children, icon: Icon, delay = 0 }: any) => (
  <motion.section 
    initial={{ opacity: 0, y: 20 }}
    whileInView={{ opacity: 1, y: 0 }}
    viewport={{ once: true, margin: "-100px" }}
    transition={{ duration: 0.5, delay }}
    className="mb-16 bg-slate-900/50 border border-slate-800 rounded-2xl p-8 shadow-xl"
  >
    <div className="flex items-center gap-4 mb-6 border-b border-slate-800 pb-4">
      {Icon && <div className="p-3 bg-amber-500/10 text-amber-500 rounded-xl"><Icon size={28} /></div>}
      <h2 className="text-3xl font-bold text-white">{title}</h2>
    </div>
    <div className="text-slate-300 leading-relaxed space-y-4 text-lg">
      {children}
    </div>
  </motion.section>
);

const Card = ({ title, value, subtitle }: any) => (
  <div className="bg-slate-800/50 border border-slate-700 p-6 rounded-xl text-center">
    <h3 className="text-slate-400 text-sm font-medium mb-2">{title}</h3>
    <div className="text-3xl font-bold text-amber-500 mb-1">{value}</div>
    {subtitle && <div className="text-slate-500 text-sm">{subtitle}</div>}
  </div>
);

export default function App() {
  return (
    <div className="min-h-screen bg-slate-950 text-slate-50 font-sans selection:bg-amber-500/30">
      {/* Hero */}
      <header className="relative overflow-hidden py-32 flex items-center justify-center text-center px-4">
        <div className="absolute inset-0 bg-[radial-gradient(ellipse_at_top,_var(--tw-gradient-stops))] from-amber-900/20 via-slate-950 to-slate-950"></div>
        <div className="relative z-10 max-w-4xl mx-auto">
          <motion.div
            initial={{ opacity: 0, scale: 0.9 }}
            animate={{ opacity: 1, scale: 1 }}
            transition={{ duration: 0.7 }}
          >
            <h1 className="text-6xl md:text-8xl font-black mb-6 tracking-tight text-transparent bg-clip-text bg-gradient-to-r from-amber-200 to-amber-500">
              PlayerClub365
            </h1>
            <p className="text-2xl md:text-3xl text-slate-300 font-light mb-8">
              הצעת השקעה להרחבת פעילות קזינו אונליין
            </p>
            <div className="inline-flex items-center gap-2 bg-amber-500/10 text-amber-400 px-6 py-3 rounded-full border border-amber-500/20 font-medium">
              <Coins size={20} />
              <span>יעד גיוס: 2,000,000 דולר</span>
            </div>
          </motion.div>
        </div>
      </header>

      <main className="max-w-5xl mx-auto px-4 py-12">
        {/* 1. תקציר מנהלים */}
        <Section title="1. תקציר מנהלים" icon={Target}>
          <p>
            <strong>PlayerClub365</strong> היא פלטפורמת קזינו אונליין הפועלת כיום במודל <strong>Social Casino</strong> עם תשתית טכנולוגית מלאה שכבר פותחה ומוכנה להפעלה.
          </p>
          <p>
            החברה מבקשת לגייס <strong>עד 2,000,000 דולר</strong> בשני שלבים, לצורך מעבר לפעילות <strong>קזינו עם כסף אמיתי</strong> תחת רישיון הימורים בינלאומי.
          </p>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mt-6">
            {[
              'קבלת רישיון הימורים בינלאומי (Anjouan)',
              'הקמת מאגר נזילות לתשלומי זכיות',
              'שיווק גלובלי וגיוס שחקנים',
              'שיתופי פעולה עם אפיליאייטים ומשפיענים',
              'הרחבת פעילות החברה',
              'הכנה לקבלת רישיון Tier-1 בעתיד'
            ].map((item, i) => (
              <div key={i} className="flex items-start gap-3 bg-slate-800/30 p-4 rounded-lg border border-slate-700/50">
                <CheckCircle2 className="text-amber-500 shrink-0 mt-0.5" size={20} />
                <span>{item}</span>
              </div>
            ))}
          </div>
          <p className="mt-6 text-xl font-medium text-amber-400">
            המטרה היא להפוך את PlayerClub365 למפעיל קזינו בינלאומי רווחי הפועל בשווקים מתפתחים עם פוטנציאל צמיחה גבוה.
          </p>
        </Section>

        {/* 2. סקירת החברה */}
        <Section title="2. סקירת החברה" icon={Award}>
          <p>
            PlayerClub365 פועלת בתחום <strong>משחקי הקזינו אונליין</strong> ומציעה חוויית משחק מבוססת דפדפן ומובייל.
          </p>
          <div className="bg-slate-800/50 p-6 rounded-xl border border-slate-700 my-6">
            <h3 className="text-xl font-bold text-white mb-4">הפלטפורמה כבר כוללת:</h3>
            <ul className="grid grid-cols-1 sm:grid-cols-2 gap-3">
              {[
                'מערכת קזינו מלאה לניהול משחקים',
                'מערכת חשבונות ושמירת יתרות לשחקנים',
                'אינטגרציה עם ספקי משחקים',
                'מערכת תשלומים',
                'התאמה מלאה למובייל',
                'מנגנוני הגנה מפני הונאות',
                'מערכת CRM לניהול שחקנים'
              ].map((item, i) => (
                <li key={i} className="flex items-center gap-2">
                  <div className="w-1.5 h-1.5 rounded-full bg-amber-500"></div>
                  <span>{item}</span>
                </li>
              ))}
            </ul>
          </div>
          <p>
            פיתוח מערכת כזו מאפס עולה לרוב בין <strong>200,000 ל-500,000 דולר</strong>. במקרה של PlayerClub365 הפלטפורמה כבר קיימת ולכן ההשקעה תופנה בעיקר ל<strong>שיווק וגיוס שחקנים</strong>.
          </p>
        </Section>

        {/* 3. הזדמנות שוק */}
        <Section title="3. הזדמנות שוק" icon={TrendingUp}>
          <p>שוק ההימורים המקוונים ממשיך לצמוח במהירות.</p>
          
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6 my-8">
            <Card title="שווי שוק 2024" value="~$95B" subtitle="מיליארד דולר" />
            <Card title="שווי שוק 2027" value="~$135B" subtitle="מיליארד דולר" />
            <Card title="שווי שוק 2030" value="~$170B" subtitle="מיליארד דולר" />
          </div>

          <h3 className="text-xl font-bold text-white mb-4">הצמיחה נובעת מ:</h3>
          <div className="flex flex-wrap gap-3">
            {['שימוש הולך וגדל במובייל', 'תשלומים באמצעות קריפטו', 'פתיחת שווקים מתפתחים', 'גידול בפופולריות של משחקי אונליין'].map((item, i) => (
              <span key={i} className="bg-slate-800 px-4 py-2 rounded-full text-sm border border-slate-700">{item}</span>
            ))}
          </div>
          <p className="mt-6 text-amber-400 font-medium">גם מפעילים קטנים יחסית יכולים להגיע להכנסות של מיליוני דולרים בשנה.</p>
        </Section>

        {/* 4. שווקי יעד */}
        <Section title="4. שווקי יעד" icon={Globe}>
          <p className="mb-6">הפעילות תתמקד בעיקר בשווקים מתפתחים בהם הביקוש גבוה והתחרות נמוכה יחסית. שווקים אלו מונים מאות מיליוני משתמשים פוטנציאליים.</p>
          
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
            <div className="bg-slate-800/40 p-5 rounded-xl border border-slate-700">
              <h4 className="text-amber-500 font-bold text-lg mb-3 border-b border-slate-700 pb-2">אמריקה הלטינית</h4>
              <ul className="space-y-2 text-slate-300">
                <li>ברזיל</li>
                <li>מקסיקו</li>
                <li>צ'ילה</li>
                <li>פרו</li>
              </ul>
            </div>
            <div className="bg-slate-800/40 p-5 rounded-xl border border-slate-700">
              <h4 className="text-amber-500 font-bold text-lg mb-3 border-b border-slate-700 pb-2">אסיה</h4>
              <ul className="space-y-2 text-slate-300">
                <li>הודו</li>
                <li>פיליפינים</li>
                <li>וייטנאם</li>
                <li>תאילנד</li>
              </ul>
            </div>
            <div className="bg-slate-800/40 p-5 rounded-xl border border-slate-700">
              <h4 className="text-amber-500 font-bold text-lg mb-3 border-b border-slate-700 pb-2">אפריקה</h4>
              <ul className="space-y-2 text-slate-300">
                <li>ניגריה</li>
                <li>קניה</li>
                <li>דרום אפריקה</li>
              </ul>
            </div>
            <div className="bg-slate-800/40 p-5 rounded-xl border border-slate-700">
              <h4 className="text-amber-500 font-bold text-lg mb-3 border-b border-slate-700 pb-2">מזרח אירופה</h4>
              <ul className="space-y-2 text-slate-300">
                <li>אוקראינה</li>
                <li>גאורגיה</li>
                <li>סרביה</li>
              </ul>
            </div>
          </div>
        </Section>

        {/* 5. מודל הכנסות */}
        <Section title="5. מודל הכנסות" icon={DollarSign}>
          <p>קזינו אונליין מרוויחים באמצעות <strong>יתרון סטטיסטי של הבית (House Edge)</strong>.</p>
          
          <div className="overflow-x-auto my-6">
            <table className="w-full text-right border-collapse">
              <thead>
                <tr className="border-b border-slate-700 text-slate-400">
                  <th className="py-3 px-4 font-medium">סוג משחק</th>
                  <th className="py-3 px-4 font-medium">יתרון לבית</th>
                </tr>
              </thead>
              <tbody>
                <tr className="border-b border-slate-800/50 bg-slate-800/20">
                  <td className="py-3 px-4">Slots</td>
                  <td className="py-3 px-4 text-amber-400 font-medium">3% עד 6%</td>
                </tr>
                <tr className="border-b border-slate-800/50">
                  <td className="py-3 px-4">Roulette</td>
                  <td className="py-3 px-4 text-amber-400 font-medium">כ-2.7%</td>
                </tr>
                <tr className="border-b border-slate-800/50 bg-slate-800/20">
                  <td className="py-3 px-4">Blackjack</td>
                  <td className="py-3 px-4 text-amber-400 font-medium">1% עד 2%</td>
                </tr>
              </tbody>
            </table>
          </div>

          <div className="bg-gradient-to-br from-slate-800 to-slate-900 p-6 rounded-xl border border-slate-700">
            <p className="text-xl mb-4">הרווח הממוצע של קזינו הוא כ- <strong className="text-amber-500">4% מסך ההימורים</strong>.</p>
            <div className="flex flex-col sm:flex-row items-center gap-4 text-center">
              <div className="flex-1 bg-slate-950/50 p-4 rounded-lg border border-slate-800 w-full">
                <div className="text-sm text-slate-400 mb-1">אם שחקנים מהמרים בחודש:</div>
                <div className="text-2xl font-bold text-white">$10,000,000</div>
              </div>
              <div className="text-slate-500">➔</div>
              <div className="flex-1 bg-amber-500/10 p-4 rounded-lg border border-amber-500/20 w-full">
                <div className="text-sm text-amber-500/80 mb-1">הרווח הצפוי (GGR):</div>
                <div className="text-2xl font-bold text-amber-500">$400,000</div>
              </div>
            </div>
          </div>
        </Section>

        {/* 6. יתרונות תחרותיים */}
        <Section title="6. יתרונות תחרותיים" icon={ShieldCheck}>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="bg-slate-800/30 p-6 rounded-xl border border-slate-700">
              <h3 className="text-xl font-bold text-amber-500 mb-2">פלטפורמה קיימת</h3>
              <p className="text-slate-300">המערכת כבר פותחה ומוכנה להפעלה.</p>
            </div>
            <div className="bg-slate-800/30 p-6 rounded-xl border border-slate-700">
              <h3 className="text-xl font-bold text-amber-500 mb-2">זמן כניסה קצר לשוק</h3>
              <p className="text-slate-300">לאחר קבלת הרישיון ניתן להשיק פעילות תוך זמן קצר.</p>
            </div>
            <div className="bg-slate-800/30 p-6 rounded-xl border border-slate-700">
              <h3 className="text-xl font-bold text-amber-500 mb-2">ניסיון טכנולוגי</h3>
              <p className="text-slate-300">המייסד בעל ניסיון רב בפיתוח מערכות תוכנה ותשתיות אינטרנט.</p>
            </div>
            <div className="bg-slate-800/30 p-6 rounded-xl border border-slate-700">
              <h3 className="text-xl font-bold text-amber-500 mb-2">סיכון נמוך יותר</h3>
              <p className="text-slate-300">רוב סטארטאפי הקזינו נכשלים בשלב הפיתוח. כאן המוצר כבר קיים.</p>
            </div>
          </div>
        </Section>

        {/* 7. אסטרטגיית רישוי */}
        <Section title="7. אסטרטגיית רישוי" icon={Award}>
          <div className="space-y-8">
            <div className="relative pl-8 md:pl-0 md:pr-8 border-r-2 border-amber-500/30">
              <div className="absolute top-0 right-[-9px] w-4 h-4 rounded-full bg-amber-500"></div>
              <h3 className="text-2xl font-bold text-white mb-2">שלב ראשון: רישיון Anjouan</h3>
              <p className="mb-4">קבלת רישיון הימורים בינלאומי Anjouan.</p>
              <div className="bg-slate-800/40 p-5 rounded-xl border border-slate-700 mb-4">
                <h4 className="font-bold text-slate-200 mb-3">יתרונות הרישיון:</h4>
                <ul className="grid grid-cols-1 sm:grid-cols-2 gap-2">
                  <li className="flex items-center gap-2"><CheckCircle2 size={16} className="text-amber-500"/> עלות נמוכה יחסית</li>
                  <li className="flex items-center gap-2"><CheckCircle2 size={16} className="text-amber-500"/> זמן אישור קצר (2-4 שבועות)</li>
                  <li className="flex items-center gap-2"><CheckCircle2 size={16} className="text-amber-500"/> אפשרות פעילות בינלאומית</li>
                  <li className="flex items-center gap-2"><CheckCircle2 size={16} className="text-amber-500"/> תמיכה בתשלומי קריפטו</li>
                </ul>
              </div>
            </div>

            <div className="relative pl-8 md:pl-0 md:pr-8 border-r-2 border-slate-700">
              <div className="absolute top-0 right-[-9px] w-4 h-4 rounded-full bg-slate-600"></div>
              <h3 className="text-2xl font-bold text-white mb-2">שלב שני: רישיון Tier-1</h3>
              <p className="mb-4">שדרוג לרישיון Tier-1 כגון Malta Gaming Authority או Isle of Man.</p>
              <p className="text-amber-400">רישיון כזה יאפשר פעילות בשווקים מוסדרים ויגדיל את שווי החברה.</p>
            </div>
          </div>
        </Section>

        {/* 8. מבנה ההשקעה */}
        <Section title="8. מבנה ההשקעה" icon={PieChart}>
          <p className="text-xl mb-6 text-center">החברה מבקשת לגייס <strong className="text-amber-500 text-2xl">$2,000,000</strong> בשני שלבים.</p>
          
          <div className="flex flex-col md:flex-row gap-6 justify-center">
            <div className="bg-slate-800/50 border border-slate-700 p-8 rounded-2xl flex-1 text-center relative overflow-hidden">
              <div className="absolute top-0 left-0 w-full h-1 bg-blue-500"></div>
              <h3 className="text-2xl font-bold text-white mb-2">שלב Seed</h3>
              <div className="text-4xl font-black text-blue-400 my-4">$800,000</div>
              <p className="text-slate-400">מעבר לקזינו עם כסף אמיתי</p>
            </div>
            
            <div className="bg-slate-800/50 border border-slate-700 p-8 rounded-2xl flex-1 text-center relative overflow-hidden">
              <div className="absolute top-0 left-0 w-full h-1 bg-emerald-500"></div>
              <h3 className="text-2xl font-bold text-white mb-2">שלב Growth</h3>
              <div className="text-4xl font-black text-emerald-400 my-4">$1,200,000</div>
              <p className="text-slate-400">הרחבת פעילות מהירה</p>
            </div>
          </div>
        </Section>

        {/* 9 & 10. שימוש בכספים */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8 mb-16">
          <motion.div 
            initial={{ opacity: 0, x: 20 }}
            whileInView={{ opacity: 1, x: 0 }}
            viewport={{ once: true }}
            className="bg-slate-900/50 border border-slate-800 rounded-2xl p-8 shadow-xl"
          >
            <h2 className="text-2xl font-bold text-white mb-6 flex items-center gap-3">
              <div className="w-3 h-8 bg-blue-500 rounded-full"></div>
              9. שימוש בכספי Seed
            </h2>
            <table className="w-full text-right mb-6">
              <tbody>
                {[
                  ['רישיון והוצאות משפטיות', '$60,000'],
                  ['מערכות תשלום', '$25,000'],
                  ['מאגר נזילות לזכיות', '$300,000'],
                  ['שיווק וגיוס שחקנים', '$355,000'],
                  ['תפעול ורגולציה', '$60,000']
                ].map(([item, amount], i) => (
                  <tr key={i} className="border-b border-slate-800/50 last:border-0">
                    <td className="py-3 text-slate-300">{item}</td>
                    <td className="py-3 font-bold text-blue-400">{amount}</td>
                  </tr>
                ))}
              </tbody>
            </table>
            <div className="bg-blue-950/30 p-4 rounded-xl border border-blue-900/50">
              <h4 className="font-bold text-blue-400 mb-2">יעדי השלב:</h4>
              <ul className="text-sm space-y-1 text-slate-300 list-disc list-inside">
                <li>השקת פעילות עם רישיון</li>
                <li>גיוס 2,000–3,000 שחקנים פעילים</li>
                <li>הגעה להיקף הימורים של 3-5 מיליון דולר בחודש</li>
                <li>בדיקת ערוצי שיווק</li>
              </ul>
            </div>
          </motion.div>

          <motion.div 
            initial={{ opacity: 0, x: -20 }}
            whileInView={{ opacity: 1, x: 0 }}
            viewport={{ once: true }}
            className="bg-slate-900/50 border border-slate-800 rounded-2xl p-8 shadow-xl"
          >
            <h2 className="text-2xl font-bold text-white mb-6 flex items-center gap-3">
              <div className="w-3 h-8 bg-emerald-500 rounded-full"></div>
              10. שימוש בכספי Growth
            </h2>
            <table className="w-full text-right mb-6">
              <tbody>
                {[
                  ['שיווק והרחבת פעילות', '$800,000'],
                  ['הגדלת מאגר נזילות', '$250,000'],
                  ['שיתופי פעולה עם אפיליאייטים', '$100,000'],
                  ['הרחבת פעילות החברה', '$50,000']
                ].map(([item, amount], i) => (
                  <tr key={i} className="border-b border-slate-800/50 last:border-0">
                    <td className="py-3 text-slate-300">{item}</td>
                    <td className="py-3 font-bold text-emerald-400">{amount}</td>
                  </tr>
                ))}
              </tbody>
            </table>
            <div className="bg-emerald-950/30 p-4 rounded-xl border border-emerald-900/50">
              <h4 className="font-bold text-emerald-400 mb-2">יעדי השלב:</h4>
              <ul className="text-sm space-y-1 text-slate-300 list-disc list-inside">
                <li>10,000 שחקנים פעילים</li>
                <li>20 מיליון דולר הימורים חודשיים</li>
                <li>הכנסות של 800,000 דולר בחודש</li>
                <li>התחלת תהליך רישוי Tier-1</li>
              </ul>
            </div>
          </motion.div>
        </div>

        {/* 11. תחזית פיננסית */}
        <Section title="11. תחזית פיננסית" icon={BarChart3}>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            {[
              { year: 'שנה ראשונה', players: '4,000', bets: '$6M', rev: '$240,000' },
              { year: 'שנה שנייה', players: '12,000', bets: '$24M', rev: '$960,000', highlight: true },
              { year: 'שנה שלישית', players: '25,000', bets: '$50M', rev: '$2,000,000', highlight: true }
            ].map((data, i) => (
              <div key={i} className={`p-6 rounded-2xl border ${data.highlight && i===2 ? 'bg-amber-500/10 border-amber-500/30' : 'bg-slate-800/40 border-slate-700'}`}>
                <h3 className={`text-xl font-bold mb-4 text-center ${data.highlight && i===2 ? 'text-amber-400' : 'text-white'}`}>{data.year}</h3>
                <div className="space-y-4">
                  <div>
                    <div className="text-sm text-slate-400">שחקנים פעילים</div>
                    <div className="text-2xl font-bold">{data.players}</div>
                  </div>
                  <div>
                    <div className="text-sm text-slate-400">הימורים חודשיים</div>
                    <div className="text-2xl font-bold text-blue-400">{data.bets}</div>
                  </div>
                  <div className="pt-4 border-t border-slate-700/50">
                    <div className="text-sm text-slate-400">הכנסות חודשיות</div>
                    <div className="text-3xl font-black text-emerald-400">{data.rev}</div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </Section>

        {/* 12. רווחיות */}
        <Section title="12. רווחיות" icon={TrendingUp}>
          <div className="flex flex-col md:flex-row gap-8 items-center">
            <div className="flex-1 w-full">
              <h3 className="text-xl font-bold text-white mb-4">מבנה עלויות ממוצע:</h3>
              <div className="space-y-3">
                {[
                  { label: 'עמלות אפיליאייט', value: '35%', color: 'bg-rose-500' },
                  { label: 'שיווק', value: '15%', color: 'bg-blue-500' },
                  { label: 'עמלות תשלום', value: '8%', color: 'bg-purple-500' },
                  { label: 'תפעול', value: '7%', color: 'bg-slate-500' },
                ].map((item, i) => (
                  <div key={i}>
                    <div className="flex justify-between text-sm mb-1">
                      <span>{item.label}</span>
                      <span className="font-bold">{item.value}</span>
                    </div>
                    <div className="w-full bg-slate-800 rounded-full h-2">
                      <div className={`${item.color} h-2 rounded-full`} style={{ width: item.value }}></div>
                    </div>
                  </div>
                ))}
                <div className="pt-4 mt-4 border-t border-slate-700 flex justify-between font-bold text-lg">
                  <span>סה״כ עלויות:</span>
                  <span className="text-rose-400">כ-65%</span>
                </div>
              </div>
            </div>
            
            <div className="flex-1 bg-gradient-to-br from-emerald-900/40 to-slate-900 border border-emerald-500/20 p-8 rounded-2xl w-full text-center">
              <h3 className="text-lg text-slate-300 mb-6">לדוגמה בשנה השלישית:</h3>
              <div className="space-y-6">
                <div>
                  <div className="text-sm text-slate-400">הכנסות חודשיות</div>
                  <div className="text-2xl font-bold text-white">$2,000,000</div>
                </div>
                <div>
                  <div className="text-sm text-slate-400">רווח חודשי</div>
                  <div className="text-3xl font-bold text-emerald-400">~$700,000</div>
                </div>
                <div className="pt-6 border-t border-emerald-500/20">
                  <div className="text-sm text-emerald-500/80 uppercase tracking-wider font-bold mb-1">רווח שנתי צפוי</div>
                  <div className="text-5xl font-black text-emerald-400">~$8.4M</div>
                </div>
              </div>
            </div>
          </div>
        </Section>

        {/* 13. מבנה מניות */}
        <Section title="13. מבנה מניות (Cap Table)" icon={Users}>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
            <div className="bg-slate-800/30 p-6 rounded-xl border border-slate-700">
              <h3 className="text-xl font-bold text-white mb-4 text-center">לאחר סבב Seed</h3>
              <div className="flex justify-center items-center gap-8">
                <div className="text-center">
                  <div className="w-24 h-24 rounded-full border-4 border-amber-500 flex items-center justify-center text-2xl font-bold mb-2">80%</div>
                  <div className="text-slate-300">המייסד</div>
                </div>
                <div className="text-center">
                  <div className="w-20 h-20 rounded-full border-4 border-blue-500 flex items-center justify-center text-xl font-bold mb-2">20%</div>
                  <div className="text-slate-300">משקיע Seed</div>
                </div>
              </div>
            </div>
            
            <div className="bg-slate-800/30 p-6 rounded-xl border border-slate-700">
              <h3 className="text-xl font-bold text-white mb-4 text-center">לאחר סבב Growth</h3>
              <div className="flex justify-center items-center gap-6">
                <div className="text-center">
                  <div className="w-20 h-20 rounded-full border-4 border-amber-500 flex items-center justify-center text-xl font-bold mb-2">~69%</div>
                  <div className="text-slate-300">המייסד</div>
                </div>
                <div className="text-center">
                  <div className="w-16 h-16 rounded-full border-4 border-blue-500 flex items-center justify-center font-bold mb-2">~17%</div>
                  <div className="text-slate-300">משקיע Seed</div>
                </div>
                <div className="text-center">
                  <div className="w-16 h-16 rounded-full border-4 border-emerald-500 flex items-center justify-center font-bold mb-2">~13%</div>
                  <div className="text-slate-300">משקיע Growth</div>
                </div>
              </div>
            </div>
          </div>
        </Section>

        {/* 14. אפשרויות Exit */}
        <Section title="14. אפשרויות Exit" icon={TrendingUp}>
          <p className="text-xl text-center mb-8">חברות קזינו אונליין נמכרות בדרך כלל לפי מכפיל של <strong className="text-amber-500">4 עד 8 פעמים</strong> הרווח השנתי.</p>
          
          <div className="bg-gradient-to-r from-amber-900/20 via-slate-800/50 to-amber-900/20 border border-amber-500/20 p-8 rounded-2xl text-center max-w-2xl mx-auto">
            <h3 className="text-slate-400 mb-2">לדוגמה, עם רווח שנתי של 8 מיליון דולר:</h3>
            <div className="text-sm text-amber-500/80 font-bold uppercase tracking-widest mb-2">שווי חברה אפשרי באקזיט</div>
            <div className="text-5xl md:text-6xl font-black text-transparent bg-clip-text bg-gradient-to-r from-amber-200 to-amber-500">
              $32M - $64M
            </div>
          </div>
        </Section>

        {/* 15. סיכונים */}
        <Section title="15. סיכונים ודרכי התמודדות" icon={AlertTriangle}>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
            <div>
              <h3 className="text-xl font-bold text-rose-400 mb-4 flex items-center gap-2">
                <AlertTriangle size={20} /> סיכונים אפשריים
              </h3>
              <ul className="space-y-3">
                {[
                  'שינויי רגולציה בשווקי היעד',
                  'מגבלות מצד ספקי תשלום וסליקה',
                  'תחרות גבוהה בשוק הגלובלי',
                  'הונאות שחקנים (Fraud)'
                ].map((item, i) => (
                  <li key={i} className="flex items-center gap-3 bg-rose-950/20 p-3 rounded-lg border border-rose-900/30">
                    <div className="w-2 h-2 rounded-full bg-rose-500"></div>
                    <span className="text-slate-300">{item}</span>
                  </li>
                ))}
              </ul>
            </div>
            <div>
              <h3 className="text-xl font-bold text-emerald-400 mb-4 flex items-center gap-2">
                <ShieldCheck size={20} /> דרכי התמודדות
              </h3>
              <ul className="space-y-3">
                {[
                  'פיזור שווקים גיאוגרפי',
                  'עבודה עם מספר ספקי תשלום במקביל',
                  'מנגנוני זיהוי הונאות מתקדמים',
                  'בדיקות KYC קפדניות'
                ].map((item, i) => (
                  <li key={i} className="flex items-center gap-3 bg-emerald-950/20 p-3 rounded-lg border border-emerald-900/30">
                    <CheckCircle2 size={18} className="text-emerald-500" />
                    <span className="text-slate-300">{item}</span>
                  </li>
                ))}
              </ul>
            </div>
          </div>
        </Section>

        {/* 16. סיכום */}
        <Section title="16. סיכום" icon={Target}>
          <div className="text-center max-w-3xl mx-auto">
            <p className="text-xl mb-8">
              <strong>PlayerClub365</strong> מציעה הזדמנות השקעה ייחודית בזכות:
            </p>
            
            <div className="grid grid-cols-2 gap-4 mb-8 text-right">
              <div className="bg-slate-800/40 p-4 rounded-xl border border-slate-700 flex items-center gap-3">
                <div className="p-2 bg-amber-500/10 rounded-lg text-amber-500"><CheckCircle2 size={20}/></div>
                <span className="font-medium">פלטפורמה מוכנה להפעלה</span>
              </div>
              <div className="bg-slate-800/40 p-4 rounded-xl border border-slate-700 flex items-center gap-3">
                <div className="p-2 bg-amber-500/10 rounded-lg text-amber-500"><CheckCircle2 size={20}/></div>
                <span className="font-medium">זמן כניסה מהיר לשוק</span>
              </div>
              <div className="bg-slate-800/40 p-4 rounded-xl border border-slate-700 flex items-center gap-3">
                <div className="p-2 bg-amber-500/10 rounded-lg text-amber-500"><CheckCircle2 size={20}/></div>
                <span className="font-medium">שוק עולמי עצום וצומח</span>
              </div>
              <div className="bg-slate-800/40 p-4 rounded-xl border border-slate-700 flex items-center gap-3">
                <div className="p-2 bg-amber-500/10 rounded-lg text-amber-500"><CheckCircle2 size={20}/></div>
                <span className="font-medium">מודל עסקי רווחי מאוד</span>
              </div>
            </div>

            <div className="bg-amber-500 text-slate-950 p-8 rounded-2xl font-bold text-xl md:text-2xl shadow-xl shadow-amber-500/20">
              באמצעות השקעה של עד 2 מיליון דולר, החברה שואפת להפוך למפעיל קזינו בינלאומי רווחי בתוך מספר שנים.
            </div>
          </div>
        </Section>
      </main>
      
      <footer className="border-t border-slate-800 py-8 text-center text-slate-500 text-sm">
        <p>© {new Date().getFullYear()} PlayerClub365. כל הזכויות שמורות.</p>
        <p className="mt-2">מסמך זה מהווה הצעת השקעה ואינו מהווה ייעוץ פיננסי.</p>
      </footer>
    </div>
  );
}
