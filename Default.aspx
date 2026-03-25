<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default" ContentType="text/html" ResponseEncoding="utf-8" EnableEventValidation="false" %>
<!DOCTYPE html>
<html lang="en" class="overflow-x-hidden">
<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />

  <asp:Literal ID="title" runat="server"></asp:Literal>
  <asp:Literal ID="metaTitle" runat="server"></asp:Literal>
  <asp:Literal ID="desc" runat="server"></asp:Literal>
  <meta name="keywords" content="social casino, free coins, online slots, casino games, risk-free gaming, playerclub365, welcome bonus, no deposit bonus">
  <meta name="author" content="PlayerClub365">
  <meta name="robots" content="index, follow">
  <meta name="theme-color" content="#0B0F12">
  <link rel="canonical" href="https://playerclub365.com/">

  <!-- Open Graph -->
  <meta property="og:type" content="website">
  <meta property="og:url" content="https://playerclub365.com/">
  <asp:Literal ID="ogTitle" runat="server"></asp:Literal>
  <asp:Literal ID="ogDesc" runat="server"></asp:Literal>
  <meta property="og:image" content="https://placehold.co/1200x630/0B0F12/FFD700?text=CLAIM+10+FREE+COINS">
  <meta property="og:site_name" content="PlayerClub365">

  <!-- Twitter -->
  <meta property="twitter:card" content="summary_large_image">
  <meta property="twitter:url" content="https://playerclub365.com/">
  <meta property="twitter:title" content="Claim Your 10 FREE Coins – PlayerClub365">
  <meta property="twitter:description" content="You've been gifted 10 free coins! Claim now and start playing top social casino games risk-free.">
  <meta property="twitter:image" content="https://placehold.co/1200x630/0B0F12/FFD700?text=CLAIM+10+FREE+COINS">

  <!-- Icons -->
  <link rel="icon" type="image/png" href="https://placehold.co/32x32/FFD700/000000?text=P">
  <link rel="apple-touch-icon" href="https://placehold.co/180x180/FFD700/000000?text=P">

  <!-- Fonts -->
  <link rel="preconnect" href="https://fonts.googleapis.com">
  <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
  <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800;900&display=swap" rel="stylesheet">

  <!-- Tailwind CSS -->
  <script src="https://cdn.tailwindcss.com"></script>

  <!-- Canvas Confetti -->
  <script src="https://cdn.jsdelivr.net/npm/canvas-confetti@1.6.0/dist/confetti.browser.min.js"></script>

  <script>
    tailwind.config = {
      theme: {
        extend: {
          colors: {
            'brand-bg': '#0B0F12',
            'brand-panel': '#0E2B30',
            'brand-panel-light': '#13353A',
            'brand-border': '#1E4A50',
            'brand-gold': '#FFD700',
            'brand-gold-hover': '#FFEA70',
            'brand-gold-dark': '#B8860B',
            'brand-text-muted': '#9AA6AC',
            'brand-white': '#FFFFFF',
          },
          fontFamily: {
            sans: ['"Segoe UI"', 'system-ui', 'sans-serif'],
          },
          backgroundImage: {
            'luxury-gradient': 'linear-gradient(180deg, #0F2A2E 0%, #0B0F12 100%)',
            'gold-gradient': 'linear-gradient(135deg, #FFD700 0%, #FFA500 100%)',
          },
          animation: {
            'fadeInUp': 'fadeInUp 1.2s ease-out',
            'explode': 'explode 0.8s cubic-bezier(0.34, 1.56, 0.64, 1) forwards',
            'shimmer': 'shimmer 2.5s linear infinite',
            'scroll': 'scroll 30s linear infinite',
            'pulse-fast': 'pulse 2s infinite',
          },
          keyframes: {
            fadeInUp: {
              '0%': { opacity: '0', transform: 'translateY(40px)' },
              '100%': { opacity: '1', transform: 'translateY(0)' },
            },
            explode: {
              '0%': { opacity: '0', transform: 'scale(0.5) translateY(20px)' },
              '50%': { opacity: '1', transform: 'scale(1.1) translateY(0)' },
              '100%': { opacity: '1', transform: 'scale(1) translateY(0)' }
            },
            shimmer: {
              '0%, 100%': { opacity: '1' },
              '50%': { opacity: '0.7' }
            },
            scroll: {
              '0%': { transform: 'translateX(0)' },
              '100%': { transform: 'translateX(-50%)' },
            },
          }
        }
      }
    }
  </script>

  <style>
    body { background-color: #0B0F12; color: #FFFFFF; }
    .no-scrollbar::-webkit-scrollbar { display: none; }
    .no-scrollbar { -ms-overflow-style: none; scrollbar-width: none; }

    /* Loading spinner overlay */
    #loadingOverlay {
      display: none;
      position: fixed;
      inset: 0;
      background: rgba(11,15,18,0.85);
      z-index: 9999;
      flex-direction: column;
      align-items: center;
      justify-content: center;
    }
    #loadingOverlay.active { display: flex; }
    .spinner {
      width: 56px; height: 56px;
      border: 5px solid #1E4A50;
      border-top-color: #FFD700;
      border-radius: 50%;
      animation: spin 0.9s linear infinite;
    }
    @keyframes spin { to { transform: rotate(360deg); } }

    /* Error message */
    #errorMsg {
      display: none;
      background: #3d0c0c;
      border: 1px solid #c0392b;
      color: #ff7675;
      border-radius: 12px;
      padding: 12px 18px;
      margin-bottom: 16px;
      font-size: 0.95rem;
      text-align: left;
    }
    #successMsg {
      display: none;
      background: darkgreen;
      border: 1px solid darkolivegreen;
      color: yellowgreen;
      border-radius: 12px;
      padding: 12px 18px;
      margin-bottom: 16px;
      font-size: 0.95rem;
      text-align: left;
    }
    .particles {
      position: fixed;
      inset: 0;
      pointer-events: none;
      background: transparent;
      z-index: 0;
    }
    #errorMsg.active { display: block; }
    #successMsg.active { display: block; }

  </style>
</head>
<body class="bg-brand-bg text-white font-sans overflow-x-hidden relative min-h-screen flex flex-col">
<div class="particles"></div>  
  <div id="loadingOverlay">
    <div class="spinner mb-5"></div>
    <p class="text-brand-gold font-bold text-lg tracking-wide">Verifying your number...</p>
  </div>

<div class="min-h-screen flex flex-col font-sans overflow-x-hidden relative bg-brand-bg">

<!-- Gradient background layer -->
<div class="absolute inset-0 bg-luxury-gradient z-0 pointer-events-none"></div>

<!-- Page content -->
<div class="relative z-10">
    <!-- ===== HEADER ===== -->
    <header class="text-center px-0 pt-12 pb-4 flex flex-col items-center justify-center relative z-10 w-full overflow-hidden">
      <p class="text-xl md:text-3xl max-w-4xl mx-auto mb-2 leading-relaxed font-bold text-white uppercase tracking-wide animate-fadeInUp">
        You have received a welcome bonus
      </p>
      <h1 class="text-[10vw] sm:text-8xl md:text-8xl lg:text-9xl font-black text-brand-gold drop-shadow-2xl mb-6 tracking-tighter pb-2 leading-tight flex items-center justify-center gap-1 md:gap-4 whitespace-nowrap w-full animate-explode opacity-0" style="animation-delay:0.2s">
        <span class="inline-block transform hover:scale-110 transition-transform">
          <svg class="w-16 h-16 md:w-24 md:h-24 text-brand-gold" fill="currentColor" viewBox="0 0 24 24"><circle cx="12" cy="12" r="10"/><text x="12" y="16" text-anchor="middle" font-size="10" fill="#0B0F12" font-weight="bold">$</text></svg>
        </span>
        <span class="text-brand-gold drop-shadow-[0_0_25px_rgba(255,215,0,0.5)]">10 FREE COINS</span>
        <span class="inline-block transform hover:scale-110 transition-transform">
          <svg class="w-16 h-16 md:w-24 md:h-24 text-brand-gold" fill="currentColor" viewBox="0 0 24 24"><circle cx="12" cy="12" r="10"/><text x="12" y="16" text-anchor="middle" font-size="10" fill="#0B0F12" font-weight="bold">$</text></svg>
        </span>
      </h1>
      <p class="text-lg md:text-2xl max-w-3xl mx-auto leading-relaxed text-brand-text-muted px-4 animate-fadeInUp" style="animation-delay:0.5s">
        Your exclusive welcome gift is reserved. Claim it below to start playing your favorite games risk-free!
      </p>
    </header>

    <!-- ===== PROMO / FORM CARD ===== -->
    <section class="px-4 text-center mb-10 relative z-10">
      <div class="max-w-3xl mx-auto bg-brand-panel border border-brand-border rounded-3xl p-5 md:p-10 relative shadow-2xl">

        <h2 class="text-2xl md:text-5xl text-white font-black mb-4 leading-tight tracking-tight">
          Claim Gift &amp; Play Instantly
        </h2>
        <p class="text-base md:text-xl text-brand-text-muted mb-8 max-w-2xl mx-auto">
          Select your country and enter your number to unlock 10 Free Coins.
        </p>

        <!-- Error message placeholder -->
        <div id="errorMsg"></div>
        <div id="successMsg"></div>

        <!-- ===== ASP.NET FORM ===== -->
        <form id="claimForm" runat="server" class="mb-6 max-w-2xl mx-auto">

       <div id="phoneInputContainer" runat="server" class="relative mb-6 flex flex-row gap-2 md:gap-3 h-14 md:h-20 w-full">

            <!-- Country ISO Dropdown -->
            <div class="relative w-[20%] md:w-30 shrink-0 h-full">
              <input type="hidden" id="callingCode" name="callingCode" runat="server" value="" />
              <select id="ddlCountry" name="ddlCountry" runat="server"
                class="w-full h-full pl-2 pr-6 md:pl-4 md:pr-10 bg-white border-2 border-gray-200 rounded-xl text-black font-bold focus:outline-none focus:border-brand-gold appearance-none cursor-pointer text-base md:text-xl shadow-inner text-center md:text-left">
                <option value="">Loading...</option>
              </select>
              <div class="absolute inset-y-0 right-2 flex items-center pointer-events-none">
                <svg class="w-4 h-4 text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"/>
                </svg>
              </div>
            </div>

            <!-- Phone Number Input -->
            <div class="flex-1 relative h-full min-w-0">
              <asp:TextBox ID="txtPhone" runat="server"
                TextMode="SingleLine"
                placeholder="Phone Number"
                CssClass="w-full h-full px-4 md:px-6 text-lg md:text-2xl border-2 border-gray-200 rounded-xl bg-white text-black font-bold tracking-wide focus:outline-none focus:border-brand-gold focus:ring-4 focus:ring-brand-gold/20 transition-all placeholder:text-gray-400 shadow-inner "
                autocomplete="tel" 
                 MaxLength="15"
                 onkeypress="return event.charCode >= 48 && event.charCode <= 57"
                           />
            </div>

          </div>
          <!-- Read-only контейнер для отображения после успеха (скрыт по умолчанию) -->
          <div id="readonlyPhoneContainer" runat="server" visible="false" class="relative mb-6 h-14 md:h-20 w-full">
              <div class="w-full h-full px-4 md:px-6 bg-gray-100 border-2 border-gray-300 rounded-xl flex items-center text-black font-bold text-lg md:text-2xl shadow-inner cursor-not-allowed opacity-75">
                  <asp:Label ID="lblReadonlyPhone" runat="server" Text="" />
              </div>
          </div>

          <!-- Submit Button -->
          <asp:Button ID="btnClaim" runat="server"
            Text="Collect and Play &#8594;"
            OnClick="btnClaim_Click"
            OnClientClick="return showLoading();"
            CssClass="w-full bg-brand-gold text-brand-bg py-3 md:py-4 px-4 md:px-6 text-lg md:text-2xl font-black uppercase tracking-widest rounded-xl cursor-pointer transition-all transform hover:-translate-y-1 active:translate-y-1 flex items-center justify-center gap-2 relative overflow-hidden border-b-4 border-brand-gold-dark hover:border-b-[6px] shadow-xl whitespace-nowrap" />

        </form>

        <div class="flex items-center justify-center gap-2 text-xs md:text-sm font-bold text-brand-text-muted/70">
          <svg class="w-4 h-4 text-brand-gold" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z"/>
          </svg>
          <span>100% Secure &amp; Spam-Free</span>
        </div>

        <!-- Server-side error label (hidden by default, shown via code-behind if needed) -->
        <asp:Label ID="lblError" runat="server" Visible="false"
          CssClass="block mt-4 text-red-400 text-sm font-semibold" />
        <asp:Label ID="lblSuccess" runat="server" Visible="false"
          CssClass="block mt-4 text-green-400 text-sm font-semibold" />
      </div>
    </section>
      <div class="text-center mb-10 hidden" id="categoryNameDiv">
        <h3 id="categoryName" class="text-2xl md:text-3xl text-white font-bold mb-3">Recently played</h3>
        <div class="w-20 h-1 bg-brand-gold mx-auto rounded-full"></div>
      </div> 
<!-- ===== GAME SLIDER ===== -->
<div class="w-full py-2 relative overflow-hidden mb-10">
  <div class="absolute top-0 left-0 w-12 md:w-32 h-full bg-gradient-to-r from-brand-bg/80 to-transparent z-10 pointer-events-none"></div>
  <div class="absolute top-0 right-0 w-12 md:w-32 h-full bg-gradient-to-l from-brand-bg/80 to-transparent z-10 pointer-events-none"></div>
  
  <div id="gameSlider" class="overflow-hidden relative">
    <div id="sliderTrack" class="flex w-max gap-4 px-4" style="transition: none;">      <!-- Games will be dynamically inserted here -->
    </div>
  </div>
</div>
    <!-- ===== BENEFITS ===== -->
    <section class="px-4 mb-16 relative z-10">
      <div class="text-center mb-10">
        <h2 class="text-3xl md:text-4xl text-white font-bold mb-3">Why Claim Now?</h2>
        <div class="w-20 h-1 bg-brand-gold mx-auto rounded-full"></div>
      </div>
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 max-w-6xl mx-auto">
        <div class="bg-brand-panel border border-brand-border rounded-2xl p-6 hover:-translate-y-2 transition-all duration-300 hover:border-brand-gold/30 hover:shadow-xl">
          <div class="mb-4 p-3 bg-brand-bg rounded-full w-fit">
            <svg class="w-6 h-6 text-brand-gold" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v13m0-13V6a4 4 0 00-4-4H6a2 2 0 00-2 2v2m8 0V6a4 4 0 014-4h2a2 2 0 012 2v2M5 20h14a2 2 0 002-2V10a2 2 0 00-2-2H5a2 2 0 00-2 2v8a2 2 0 002 2z"/></svg>
          </div>
          <h3 class="text-brand-gold text-xl font-bold mb-2">10 Free Coins</h3>
          <p class="text-brand-text-muted text-sm leading-relaxed">Start spinning the moment you claim! No waiting, instant fun.</p>
        </div>
        <div class="bg-brand-panel border border-brand-border rounded-2xl p-6 hover:-translate-y-2 transition-all duration-300 hover:border-brand-gold/30 hover:shadow-xl">
          <div class="mb-4 p-3 bg-brand-bg rounded-full w-fit">
            <svg class="w-6 h-6 text-brand-gold" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 3v4M3 5h4M6 17v4m-2-2h4m5-16l2.286 6.857L21 12l-5.714 2.143L13 21l-2.286-6.857L5 12l5.714-2.143L13 3z"/></svg>
          </div>
          <h3 class="text-brand-gold text-xl font-bold mb-2">7000+ Top Games</h3>
          <p class="text-brand-text-muted text-sm leading-relaxed">Access premium slots, live tables, and arcade games risk-free.</p>
        </div>
        <div class="bg-brand-panel border border-brand-border rounded-2xl p-6 hover:-translate-y-2 transition-all duration-300 hover:border-brand-gold/30 hover:shadow-xl">
          <div class="mb-4 p-3 bg-brand-bg rounded-full w-fit">
            <svg class="w-6 h-6 text-brand-gold" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4M7.835 4.697a3.42 3.42 0 001.946-.806 3.42 3.42 0 014.438 0 3.42 3.42 0 001.946.806 3.42 3.42 0 013.138 3.138 3.42 3.42 0 00.806 1.946 3.42 3.42 0 010 4.438 3.42 3.42 0 00-.806 1.946 3.42 3.42 0 01-3.138 3.138 3.42 3.42 0 00-1.946.806 3.42 3.42 0 01-4.438 0 3.42 3.42 0 00-1.946-.806 3.42 3.42 0 01-3.138-3.138 3.42 3.42 0 00-.806-1.946 3.42 3.42 0 010-4.438 3.42 3.42 0 00.806-1.946 3.42 3.42 0 013.138-3.138z"/></svg>
          </div>
          <h3 class="text-brand-gold text-xl font-bold mb-2">Daily Winners</h3>
          <p class="text-brand-text-muted text-sm leading-relaxed">Join thousands of players winning virtual jackpots every day.</p>
        </div>
        <div class="bg-brand-panel border border-brand-border rounded-2xl p-6 hover:-translate-y-2 transition-all duration-300 hover:border-brand-gold/30 hover:shadow-xl">
          <div class="mb-4 p-3 bg-brand-bg rounded-full w-fit">
            <svg class="w-6 h-6 text-brand-gold" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z"/></svg>
          </div>
          <h3 class="text-brand-gold text-xl font-bold mb-2">100% Risk-Free</h3>
          <p class="text-brand-text-muted text-sm leading-relaxed">No real money gambling. Just pure entertainment and excitement.</p>
        </div>
      </div>
    </section>

    <!-- ===== FAQ ===== -->
    <section class="px-4 mb-16 relative z-10">
      <div class="max-w-3xl mx-auto">
        <div class="text-center mb-10">
          <h3 class="text-3xl md:text-4xl font-black text-white uppercase tracking-tight mb-2">Frequent Questions</h3>
          <p class="text-brand-text-muted">Everything you need to know about your free gift.</p>
        </div>
        <div class="flex flex-col gap-4" id="faqList">
          <div class="border border-brand-border rounded-2xl overflow-hidden bg-brand-panel/20 hover:bg-brand-panel/40 transition-all">
            <button onclick="toggleFaq(this)" class="w-full py-5 px-6 flex items-center justify-between text-left focus:outline-none group">
              <span class="font-bold text-lg text-white group-hover:text-brand-gold-hover">How do I start playing with my free coins?</span>
              <span class="faq-icon text-brand-text-muted text-2xl">+</span>
            </button>
            <div class="faq-body overflow-hidden max-h-0 transition-all duration-300 ease-in-out">
              <div class="px-6 pb-6 pt-0 text-brand-text-muted text-base leading-relaxed border-t border-brand-border/50 mt-2 pt-4">
                Simply enter your mobile number in the form above. We will send you a secure verification link via SMS. Once verified, the 10 coins are instantly added to your balance.
              </div>
            </div>
          </div>
          <div class="border border-brand-border rounded-2xl overflow-hidden bg-brand-panel/20 hover:bg-brand-panel/40 transition-all">
            <button onclick="toggleFaq(this)" class="w-full py-5 px-6 flex items-center justify-between text-left focus:outline-none group">
              <span class="font-bold text-lg text-white group-hover:text-brand-gold-hover">Is no purchase necessary?</span>
              <span class="faq-icon text-brand-text-muted text-2xl">+</span>
            </button>
            <div class="faq-body overflow-hidden max-h-0 transition-all duration-300 ease-in-out">
              <div class="px-6 pb-6 pt-0 text-brand-text-muted text-base leading-relaxed border-t border-brand-border/50 mt-2 pt-4">
                Correct! The 10 free coins are a welcome gift for our new community members. No deposit or purchase is required to claim this specific offer.
              </div>
            </div>
          </div>
          <div class="border border-brand-border rounded-2xl overflow-hidden bg-brand-panel/20 hover:bg-brand-panel/40 transition-all">
            <button onclick="toggleFaq(this)" class="w-full py-5 px-6 flex items-center justify-between text-left focus:outline-none group">
              <span class="font-bold text-lg text-white group-hover:text-brand-gold-hover">Can I win real money?</span>
              <span class="faq-icon text-brand-text-muted text-2xl">+</span>
            </button>
            <div class="faq-body overflow-hidden max-h-0 transition-all duration-300 ease-in-out">
              <div class="px-6 pb-6 pt-0 text-brand-text-muted text-base leading-relaxed border-t border-brand-border/50 mt-2 pt-4">
                PlayerClub365 is a social casino for entertainment purposes. While you can win virtual coins and climb the leaderboards, our platform does not offer real money gambling or cash prizes.
              </div>
            </div>
          </div>
          <div class="border border-brand-border rounded-2xl overflow-hidden bg-brand-panel/20 hover:bg-brand-panel/40 transition-all">
            <button onclick="toggleFaq(this)" class="w-full py-5 px-6 flex items-center justify-between text-left focus:outline-none group">
              <span class="font-bold text-lg text-white group-hover:text-brand-gold-hover">Is my personal data safe?</span>
              <span class="faq-icon text-brand-text-muted text-2xl">+</span>
            </button>
            <div class="faq-body overflow-hidden max-h-0 transition-all duration-300 ease-in-out">
              <div class="px-6 pb-6 pt-0 text-brand-text-muted text-base leading-relaxed border-t border-brand-border/50 mt-2 pt-4">
                Absolutely. We use industry-standard SSL encryption to protect your data. Your mobile number is used strictly for account verification to prevent bots and duplicate claims.
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
</div>
    <!-- ===== FOOTER ===== -->
    <footer class="text-center py-12 px-4 border-t border-brand-border bg-brand-bg relative z-10">
      <div class="mb-8">
        <span class="inline-block text-xs md:text-sm font-semibold text-brand-text-muted uppercase tracking-widest border border-brand-border px-4 py-1.5 rounded-full bg-brand-panel/50">
          Exclusive Offer | 21+ Only | Entertainment Only
        </span>
      </div>
      <div class="mb-6">
        <span class="text-xl font-black tracking-tighter text-white">PLAYERCLUB<span class="text-brand-gold">365</span></span>
      </div>
      <div class="max-w-2xl mx-auto space-y-6">
        <div class="flex flex-col items-center gap-2 text-xs text-brand-text-muted/60">
          <div class="flex items-center gap-1.5 opacity-80 mb-1">
            <svg class="w-3 h-3 text-brand-gold" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M5 9V7a5 5 0 0110 0v2a2 2 0 012 2v5a2 2 0 01-2 2H5a2 2 0 01-2-2v-5a2 2 0 012-2zm8-2v2H7V7a3 3 0 016 0z" clip-rule="evenodd"/></svg>
            <span>Secure 256-bit SSL Encryption</span>
          </div>
          <p class="leading-relaxed">By continuing, you agree to our Terms &amp; Privacy Policy.<br/>Virtual coins have no cash value.</p>
        </div>
        <p class="text-brand-text-muted/40 text-xs">© <%= DateTime.Now.Year %> PlayerClub365 • 100% Risk-Free Social Casino • Virtual Coins for Fun Only</p>
      </div>
    </footer>

  </div><!-- end app wrapper -->
<script>
  // Game data array
  const gamesData = [
    { name: "Sweet Bonanza", cid:"12", color: "FF69B4", src: "aHR0cHM6Ly9jZG4uYWVzZ2FtaW5nYXNpYS5jb20vZ2FtZV9waWMvcHAvZW4vbWFpbi92czIwZnJ1aXRzdy92czIwZnJ1aXRzd19uYXJyb3cuanBn" },
    { name: "Big Bass Vegas", cid:"12", color: "1E90FF", src: "aHR0cHM6Ly9jZG4uYWVzZ2FtaW5nYXNpYS5jb20vZ2FtZV9waWMvcHAvZW4vbWFpbi92czEwdHhiaWdiYXNzL3ZzMTB0eGJpZ2Jhc3NfbmFycm93LmpwZw==" },
    { name: "Blackjack Live", cid:"33", color: "1a1a1a", src: "aHR0cHM6Ly9zdGF0aWMuY2RuZXUtc3RhdC5jb20vcmVzb3VyY2VzL3NpdGVwaWNzdGJzL29wX2dhbGF4c3lzL2dhbWVfaW1nXzUvQmxhY2tqYWNrQ2xhc3NpYy5qcGc=" },
    { name: "Aviatrix", cid:"12", color: "8A2BE2", src: "aHR0cHM6Ly9zdGF0aWMuY2RuZXUtc3RhdC5jb20vcmVzb3VyY2VzL3NpdGVwaWNzdGJzL3NyZW50L2dhbWVfaW1nXzUvU3ByaWJlQXZpYXRvci5qcGc=" },
    { name: "Blackjack VIP 27", cid:"33", color: "DC143C", src: "aHR0cHM6Ly9zdGF0aWMuY2RuZXUtc3RhdC5jb20vcmVzb3VyY2VzL3NpdGVwaWNzdGJzL29wX2V2b2x1dGlvbl9sb2JieS9nYW1lX2ltZ181LzE1NTExLmpwZw==" },
    { name: "Jelly Candy", cid:"12", color: "FFD700", src: "aHR0cHM6Ly9jZG4uYWVzZ2FtaW5nYXNpYS5jb20vZ2FtZV9waWMvcHAvZW4vbWFpbi92czVqZWxseWMvdnM1amVsbHljX25hcnJvdy5qcGc=" },
    { name: "Fat Panda", cid:"12", color: "708090", src: "aHR0cHM6Ly9jZG4uYWVzZ2FtaW5nYXNpYS5jb20vZ2FtZV9waWMvcHAvZW4vbWFpbi92czIwYmVlZmVkL3ZzMjBiZWVmZWRfbmFycm93LmpwZw==" }
  ];

  // Get category ID from URL parameter
  function getCategoryIdFromUrl() {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('cid');
  }

  // Filter games by category ID
  function filterGamesByCategory(games, categoryId) {
    if (!categoryId) return games; // No category filter, return all games
    
    const filteredGames = games.filter(game => game.cid === categoryId);
    
    // If filtered games array is empty, return all games
    if (filteredGames.length === 0) {
      console.log('No games found for category ' + categoryId + ', showing all games');
      return games;
    }
    
    console.log('Showing ' + filteredGames.length + ' games for category ' + categoryId);
    return filteredGames;
  }

  // Create game card DOM element
  function createGameCard(game) {
    const cardDiv = document.createElement('div');
    cardDiv.className = 'w-24 md:w-28 aspect-[3/4] relative rounded-lg overflow-hidden shadow-lg border border-white/10 hover:border-brand-gold transition-all duration-300 transform hover:scale-105 flex-shrink-0';
    
    const img = document.createElement('img');
    img.src = `https://www.playerclub365.com/images/poster.ashx?src=${game.src}`;
    img.className = 'w-full h-full object-cover';
    img.alt = game.name;
    
    const overlayDiv = document.createElement('div');
    overlayDiv.className = 'absolute bottom-0 left-0 right-0 p-1.5 bg-gradient-to-t from-black/90 to-transparent';
    
    const nameP = document.createElement('p');
    nameP.className = 'text-white text-[9px] md:text-[10px] font-bold text-center drop-shadow-md truncate';
    nameP.textContent = game.name;
    
    overlayDiv.appendChild(nameP);
    cardDiv.appendChild(img);
    cardDiv.appendChild(overlayDiv);
    
    return cardDiv;
  }

  // Initialize infinite slider with filtered games
  function initInfiniteSlider() {
    const sliderTrack = document.getElementById('sliderTrack');
    if (!sliderTrack) {
      console.error('Slider track not found');
      return;
    }
    
    // Get category ID from URL and filter games
    const categoryId = getCategoryIdFromUrl();
    const filteredGames = filterGamesByCategory(gamesData, categoryId);
    
    // Clear existing content
    while (sliderTrack.firstChild) {
      sliderTrack.removeChild(sliderTrack.firstChild);
    }
    
    // Create 3 copies of the filtered games array for seamless infinite scroll
    // More copies for smoother scrolling if there are few games
    const copyCount = filteredGames.length < 5 ? 12 : 6;
    
    for (let i = 0; i < copyCount; i++) {
      filteredGames.forEach(game => {
        const gameCard = createGameCard(game);
        sliderTrack.appendChild(gameCard);
      });
    }
    
    let currentPosition = 0;
    let animationId = null;
    let isAnimating = true;
    let lastTimestamp = 0;
    let speed = 0.6; // Scroll speed
    
    // Calculate the width of one full set of games
    function getOneSetWidth() {
      const firstGame = sliderTrack.children[0];
      if (!firstGame) return 0;
      
      const gameWidth = firstGame.offsetWidth + 16; // width + gap (gap-4 = 16px)
      return filteredGames.length * gameWidth;
    }
    
    // Move first set of games to the end
    function moveFirstSetToEnd() {
      const gamesPerSet = filteredGames.length;
      const childrenToMove = [];
      
      // Collect first set of games
      for (let i = 0; i < gamesPerSet; i++) {
        if (sliderTrack.children[0]) {
          childrenToMove.push(sliderTrack.children[0]);
        }
      }
      
      // Append them to the end
      childrenToMove.forEach(child => {
        sliderTrack.appendChild(child);
      });
    }
    
    // Scroll animation function with time-based movement
    function scrollSlider(timestamp) {
      if (!isAnimating) return;
      
      if (lastTimestamp === 0) {
        lastTimestamp = timestamp;
        animationId = requestAnimationFrame(scrollSlider);
        return;
      }
      
      const delta = Math.min(50, timestamp - lastTimestamp);
      const moveDistance = speed * (delta / 16.67);
      
      currentPosition -= moveDistance;
      lastTimestamp = timestamp;
      
      const oneSetWidth = getOneSetWidth();
      
      // Reset position when we've scrolled past one full set
      if (Math.abs(currentPosition) >= oneSetWidth) {
        currentPosition = 0;
        
        // Move first set to end seamlessly
        moveFirstSetToEnd();
        
        // Reset transform
        sliderTrack.style.transform = `translateX(0px)`;
        lastTimestamp = timestamp;
        animationId = requestAnimationFrame(scrollSlider);
        return;
      }
      
      sliderTrack.style.transform = `translateX(${currentPosition}px)`;
      animationId = requestAnimationFrame(scrollSlider);
    }
    
    // Start animation
    function startAnimation() {
      if (animationId) {
        cancelAnimationFrame(animationId);
      }
      isAnimating = true;
      lastTimestamp = 0;
      animationId = requestAnimationFrame(scrollSlider);
    }
    
    // Stop animation
    function stopAnimation() {
      isAnimating = false;
      if (animationId) {
        cancelAnimationFrame(animationId);
        animationId = null;
      }
    }
    
    // Start the animation immediately
    startAnimation();
    
    // Pause animation on hover
    const sliderContainer = document.getElementById('gameSlider');
    if (sliderContainer) {
      sliderContainer.addEventListener('mouseenter', () => {
        stopAnimation();
      });
      
      sliderContainer.addEventListener('mouseleave', () => {
        startAnimation();
      });
    }
    
    // Handle visibility change to save resources
    document.addEventListener('visibilitychange', function() {
      if (document.hidden) {
        stopAnimation();
      } else {
        startAnimation();
      }
    });
    
    // Handle window resize
    let resizeTimeout;
    window.addEventListener('resize', () => {
      clearTimeout(resizeTimeout);
      resizeTimeout = setTimeout(() => {
        const wasAnimating = isAnimating;
        stopAnimation();
        
        currentPosition = 0;
        sliderTrack.style.transform = `translateX(0px)`;
        
        // Restart animation if it was animating
        if (wasAnimating) {
          startAnimation();
        }
      }, 250);
    });
  }
  
  // Initialize when DOM is ready
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initInfiniteSlider);
  } else {
    initInfiniteSlider();
  }
</script>
  <script>
    // Confetti on load
    // ── Load countries from JSON ──────────────────────────────
    function loadCountries() {
      var sel = document.getElementById('<%= ddlCountry.ClientID %>');
      var hiddenCode = document.getElementById('<%= callingCode.ClientID %>');
    
      fetch('https://www.playerclub365.com/countries.json')
        .then(function(r) { return r.json(); })
        .then(function(data) {
          sel.innerHTML = '';
          data.forEach(function(c) {
            if (!c.ISO3166 || c.ISO3166 === 'empty' || !c.CountryName) return;
            var opt = document.createElement('option');
            opt.value = c.ISO3166;
            opt.setAttribute("data-code", c.CallingCode);
            opt.text  = '+' + c.CallingCode;
            sel.appendChild(opt);
          });
    
          var iso = window.__cfIso || 'EN';
          if (sel.querySelector('option[value="' + iso + '"]')) {
            sel.value = iso;
            var selected = sel.options[sel.selectedIndex];
            hiddenCode.value = selected.getAttribute('data-code') || '';
          }
        })
        .catch(function() {
          sel.innerHTML = '<option value="">-- Select country --</option>';
        });
    }
    document.addEventListener('DOMContentLoaded', function () {
    
      var sel = document.getElementById('<%= ddlCountry.ClientID %>');
      var hiddenCode = document.getElementById('<%= callingCode.ClientID %>');
    
      sel.addEventListener('change', function () {
        var selected = this.options[this.selectedIndex];
        hiddenCode.value = selected.getAttribute('data-code') || '';
      });
    
    });

    window.addEventListener('load', function () {
      loadCountries();

      // Confetti
      setTimeout(function () {
        if (window.confetti) {
          var defaults = { origin: { x: 0.5, y: 0.25 }, zIndex: 100, colors: ['#FFD700', '#FFA500', '#FFFFFF', '#32CD32', '#FF4500'], gravity: 0.8, scalar: 1.2, disableForReducedMotion: true };
          window.confetti({ ...defaults, particleCount: 75, spread: 100, startVelocity: 60, decay: 0.9 });
          window.confetti({ ...defaults, particleCount: 60, spread: 180, startVelocity: 50, decay: 0.92 });
          window.confetti({ ...defaults, particleCount: 105, spread: 80, startVelocity: 75, decay: 0.91, scalar: 0.8 });
        }
      }, 600);

      // Show server-side error in the styled div if lblError is visible
      var lbl = document.getElementById('<%= lblError.ClientID %>');
      if (lbl && lbl.style.display !== 'none' && lbl.innerText.trim() !== '') {
        var errDiv = document.getElementById('errorMsg');
        errDiv.innerText = lbl.innerText;
        errDiv.classList.add('active');
        lbl.style.display = 'none';
      }
      var lblSuccess = document.getElementById('<%= lblSuccess.ClientID %>');
      if (lblSuccess && lblSuccess.style.display !== 'none' && lblSuccess.innerText.trim() !== '') {
        var successDiv = document.getElementById('successMsg');
        successDiv.innerText = lblSuccess.innerText;
        successDiv.classList.add('active');
        lblSuccess.style.display = 'none';
      }
    });

    // Show loading overlay on submit
    function showLoading() {
      var phone = document.getElementById('<%= txtPhone.ClientID %>').value.trim();
      if (!phone) {
        var errDiv = document.getElementById('errorMsg');
        errDiv.innerText = 'Please enter your phone number.';
        errDiv.classList.add('active');
        return false;
      }
      document.getElementById('errorMsg').classList.remove('active');
      document.getElementById('loadingOverlay').classList.add('active');
      return true;
    }

    // FAQ accordion
    function toggleFaq(btn) {
      var body = btn.nextElementSibling;
      var icon = btn.querySelector('.faq-icon');
      var isOpen = body.style.maxHeight && body.style.maxHeight !== '0px';
      // Close all
      document.querySelectorAll('.faq-body').forEach(function (b) { b.style.maxHeight = '0px'; });
      document.querySelectorAll('.faq-icon').forEach(function (i) { i.textContent = '+'; });
      document.querySelectorAll('#faqList > div').forEach(function (d) {
        d.classList.remove('bg-brand-panel', 'border-brand-gold/50');
        d.classList.add('bg-brand-panel/20');
      });
      if (!isOpen) {
        body.style.maxHeight = body.scrollHeight + 200 + 'px';
        icon.textContent = '\u2212';
        btn.closest('div').classList.add('bg-brand-panel', 'border-brand-gold/50');
        btn.closest('div').classList.remove('bg-brand-panel/20');
      }
    }
  </script>

</body>
</html>
