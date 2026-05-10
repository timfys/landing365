using System;
using System.Net;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Engines;
using System.Configuration;
public partial class Default : Page
{
    string SmartWinnersApi = ConfigurationManager.AppSettings[HttpContext.Current.Request.Url.AbsoluteUri.Contains("www.playerclub365.com") ? "SmartWinnersApiLive" : "SmartWinnersApi"];
    string PlayerApi = ConfigurationManager.AppSettings[HttpContext.Current.Request.Url.AbsoluteUri.Contains("www.playerclub365.com") ? "PlayerApiLive" : "PlayerApi"];
    string BusinessApi = ConfigurationManager.AppSettings[HttpContext.Current.Request.Url.AbsoluteUri.Contains("www.playerclub365.com") ? "BusinessApiLive" : "BusinessApi"];
    string Pwd = ConfigurationManager.AppSettings[HttpContext.Current.Request.Url.AbsoluteUri.Contains("www.playerclub365.com") ? "PwdLive" : "Pwd"];
    string SuccessUrl = ConfigurationManager.AppSettings[HttpContext.Current.Request.Url.AbsoluteUri.Contains("www.playerclub365.com") ? "SuccessUrlLive" : "SuccessUrl"];
    string SignInUrl = ConfigurationManager.AppSettings[HttpContext.Current.Request.Url.AbsoluteUri.Contains("www.playerclub365.com") ? "SignInUrlLive" : "SignInUrl"];
    string BaseUrl = ConfigurationManager.AppSettings[HttpContext.Current.Request.Url.AbsoluteUri.Contains("www.playerclub365.com") ? "BaseUrlLive" : "BaseUrl"];
    
    private const string SoapAction =
        "urn:SmartWinners.Intf-ISmartWinners#Signin_With_Phone";
    private static string _cachedCountriesJson = null;
    private static DateTime _cacheTimestamp = DateTime.MinValue;
    private static readonly object _cacheLock = new object();
    public static string GetUserIsoFromCloudFlare(HttpContext context)
    {
        var host = context.Request.Url.Host;

        if (!string.IsNullOrEmpty(host) &&
            host.IndexOf("player", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return context.Request.Headers["CF-IPCountry"];
        }

        return "IL";
    }
    public static string GetClientIp(HttpContext context)
    {
        // 1. Пытаемся взять заголовок от Cloudflare (раз вы его используете)
        string ip = context.Request.Headers["CF-Connecting-IP"];

        // 2. Если Cloudflare нет, проверяем стандартный заголовок прокси
        if (string.IsNullOrEmpty(ip))
        {
            ip = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        }

        // 3. Если и там пусто, берем прямой адрес
        if (string.IsNullOrEmpty(ip))
        {
            ip = context.Request.UserHostAddress;
        }

        // Если в X_FORWARDED_FOR пришел список IP через запятую, берем первый
        if (!string.IsNullOrEmpty(ip) && ip.Contains(","))
        {
            ip = ip.Split(',')[0].Trim();
        }

        return ip;
    }
    // -------------------------------------------------------
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            DataBind();
            string baseUrl = ConfigurationManager.AppSettings[HttpContext.Current.Request.Url.AbsoluteUri.Contains("www.playerclub365.com") ? "BaseUrlLive" : "BaseUrl"];
            btnPlay.OnClientClick = "window.location = '"+baseUrl+"'; return false;";
        }
        string clientIp = GetClientIp(HttpContext.Current);
        HttpCookie ipCookie = new HttpCookie("clientIp", clientIp);
        ipCookie.Expires = DateTime.Now.AddDays(1);
        ipCookie.Path = "/";
        Response.Cookies.Add(ipCookie);
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetNoStore();
        Response.Cache.SetExpires(DateTime.UtcNow.AddSeconds(-1));
        string categoryId = Request.QueryString["cid"];
        string callGameCategoriesGetResponse = "";
        string categoryName = "";
        string ogDescription = "";
        string gamesGetResponse = "";
        btnPlay.Visible = false;
        btnClaim2.Visible = false;
        LoadCountriesToJavaScript();
        string aid = Request.QueryString["aid"];
        var affiliateScriptResponse = "";
        var affiliateScript = "";
        if (aid != null && aid != "")
        {
            affiliateScriptResponse = CallEntityFindAffiliate(aid);
            if (!string.IsNullOrWhiteSpace(affiliateScriptResponse))
            {
                var match1 = Regex.Match(
                    affiliateScriptResponse,
                    @"""customfield165""\s*:\s*""((?:\\.|[^""])*)""",
                    RegexOptions.Singleline);
                if (match1.Success)
                {
                    affiliateScript = match1.Groups[1].Value;
                    affiliateScript = HttpUtility.HtmlDecode(affiliateScript);
                    litAffiliateScript.Text = affiliateScript;
                }
            }
        }

        gamesGetResponse = CallGamesGet(categoryId);
        var gamesJson = FormatGamesJson(gamesGetResponse);
        string script = "var gamesFromServer = " + gamesJson + ";";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "gamesData", script, true);

        var titleContent = "Claim Your 20 free Coins - PlayerClub365";
        metaTitle.Text = "<meta name=\"title\" content=\"" + titleContent + "\">";
        title.Text = "<title>" + titleContent + "</title>";
        desc.Text =
            "<meta name=\"description\" content=\"You've been gifted 20 free coins! Claim now and start playing top social casino games risk-free. No purchase necessary.\"/>\n";
        ogDesc.Text =
            "<meta property=\"og:description\" content=\"You've been gifted 20 free coins! Claim now and start playing top social casino games risk-free. No purchase necessary.\">\n";
        ogTitle.Text =
            "<meta property=\"og:title\" content=\"Claim Your 20 free Coins - PlayerClub365\">\n";

        string iso = GetUserIsoFromCloudFlare(HttpContext.Current);
        Page.ClientScript.RegisterStartupScript(
            this.GetType(),
            "cfIso",
            "window.__cfIso = '" + iso + "';",
            true
        );

        if (!string.IsNullOrEmpty(categoryId))
        {
            callGameCategoriesGetResponse = CallGameCategoriesGet(categoryId);

            // Правильно парсим JSON из SOAP ответа
            string jsonData = ExtractJsonFromSoapResponse(callGameCategoriesGetResponse);

            if (!string.IsNullOrEmpty(jsonData))
            {
                var serializer = new JavaScriptSerializer();
                var categoriesList = serializer.Deserialize<List<Dictionary<string, object>>>(jsonData);

                if (categoriesList != null && categoriesList.Count > 0)
                {
                    var category = categoriesList[0];

                    if (category.ContainsKey("category_name"))
                    {
                        categoryName = category["category_name"].ToString() ?? "";
                    }

                    if (category.ContainsKey("og_description"))
                    {
                        ogDescription = category["og_description"].ToString() ?? "";
                    }

                    if (!string.IsNullOrEmpty(categoryName))
                    {
                        // Используем ClientScript вместо ScriptManager для надежности
                        string showDivScript = @"
                        setTimeout(function() {
                            var div = document.getElementById('categoryNameDiv');
                            if(div) {
                                div.classList.remove('hidden');
                                var h = document.getElementById('categoryName');
                                if(h) {
                                    h.innerText = '" + EscapeJsString(categoryName) + @"';
                                }
                                var d = document.getElementById('categoryDescription');
                                if(h) {
                                    d.innerText = '" + EscapeJsString(ogDescription) + @"';
                                }
                            }
                        }, 100);
                    ";
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "showCategoryDiv", showDivScript, true);

                        titleContent = categoryName + " Claim Your 20 free Coins - PlayerClub365";
                        metaTitle.Text = "<meta name=\"title\" content=\"" + titleContent + "\">";
                        title.Text = "<title>" + titleContent + "</title>";
                        ogTitle.Text = "<meta property=\"og:title\" content=\"" + titleContent + "\">\n";
                    }

                    if (!string.IsNullOrEmpty(ogDescription))
                    {
                        desc.Text = "<meta name=\"description\" content=\"" + ogDescription +
                                    " You've been gifted 20 free coins! Claim now and start playing top social casino games risk-free. No purchase necessary.\"/>\n";
                        ogDesc.Text = "<meta property=\"og:description\" content=\"" + ogDescription +
                                      " You've been gifted 20 free coins! Claim now and start playing top social casino games risk-free. No purchase necessary.\">\n";
                    }
                }
            }
        }

        var cookie = Request.Cookies["cred"];
        var entityBonusesCheckResponse = "";
        string currentEntityId = null;

        bool hasValidCredential = false;

        if (cookie != null && !string.IsNullOrWhiteSpace(cookie.Value))
        {
            try
            {
                string base64Token = HttpUtility.UrlDecode(cookie.Value);
                var credential = DecryptCredential(base64Token);

                if (credential != null && credential.EntityId > 0)
                {
                    currentEntityId = credential.EntityId.ToString();
                    hasValidCredential = true;
                }
            }
            catch
            {
                // cookie битый → игнорируем полностью
                hasValidCredential = false;
                currentEntityId = null;
            }
        }

// DEFAULT STATE (важно для SEO и стабильности)
        bool hasBonusField = false;

        if (hasValidCredential)
        {
            try
            {
                CallLog(currentEntityId);
                entityBonusesCheckResponse = CheckIfBonusExists(currentEntityId);

                if (!string.IsNullOrWhiteSpace(entityBonusesCheckResponse))
                {
                    hasBonusField = entityBonusesCheckResponse.Contains("customfield203");
                }
            }
            catch
            {
                entityBonusesCheckResponse = null;
            }
        }

// === UI LOGIC ===

// нет данных вообще → считаем как guest (одинаково для Google + новых пользователей)
        if (!hasValidCredential || string.IsNullOrWhiteSpace(entityBonusesCheckResponse))
        {
            phoneInputContainer.Visible = true;
            readonlyPhoneContainer.Visible = false;
            txtPhone.Visible = true;

            btnClaim.Visible = true;
            btnPlay.Visible = false;
            btnClaim2.Visible = false;

            return;
        }

// валидный пользователь
        phoneInputContainer.Visible = false;
        readonlyPhoneContainer.Visible = false;

        if (!hasBonusField)
        {
            btnClaim.Visible = false;
            btnPlay.Visible = false;
            btnClaim2.Visible = true;
        }
        else
        {
            btnClaim.Visible = false;
            btnPlay.Visible = true;
            btnClaim2.Visible = false;

            ShowSuccess("Bonuses already given for this user");
        }
    }

    private Credential DecryptCredential(string token)
{
    if (string.IsNullOrEmpty(token))
        return null;

    try
    {
        byte[] blob = Convert.FromBase64String(token);

        if (blob.Length < 28)
            throw new CryptographicException("incorrect length");

        byte[] iv = blob.Take(12).ToArray();
        byte[] tag = blob.Skip(12).Take(16).ToArray();
        byte[] cipher = blob.Skip(28).ToArray();

        byte[] key = Convert.FromBase64String("pkCURTfUHR2wg1QporIC4MPGxqubPsWb+nREtkTsNus=");

        byte[] input = new byte[cipher.Length + tag.Length];
        Array.Copy(cipher, 0, input, 0, cipher.Length);
        Array.Copy(tag, 0, input, cipher.Length, tag.Length);

        var gcm = new GcmBlockCipher(new AesEngine());
        gcm.Init(false, new AeadParameters(new KeyParameter(key), 128, iv));

        byte[] plain = new byte[gcm.GetOutputSize(input.Length)];
        int len = gcm.ProcessBytes(input, 0, input.Length, plain, 0);
        gcm.DoFinal(plain, len);

        string json = Encoding.UTF8.GetString(plain).TrimEnd('\0');

        // **ИСПРАВЛЯЕМ JSON** (удаляем лишнюю кавычку)
        json = Regex.Replace(json, @"""EntityId"":(\d+)""", m => "\"EntityId\":"+m.Groups[1].Value);
        // **ДЕСЕРИАЛИЗУЕМ В СЛОВАРЬ**
        var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        var dict = serializer.Deserialize<Dictionary<string, object>>(json);

        // **УДАЛЯЕМ СЛУЖЕБНЫЕ ПОЛЯ**
        dict.Remove("keyVersion");
        dict.Remove("schemaVersion");

        // **ИЗВЛЕКАЕМ EntityId**
        int entityId = Convert.ToInt32(dict["EntityId"]);

        // **ВОЗВРАЩАЕМ ОБЪЕКТ Credential**
        return new Credential
        {
            EntityId = entityId,
            Username = dict["Username"].ToString(),
            Password = dict["Password"].ToString(),
            Lid = dict.ContainsKey("Lid") ? dict["Lid"].ToString() : "0",
            AffiliateId = dict.ContainsKey("AffiliateId") ? Convert.ToInt32(dict["AffiliateId"]) : 0
        };
    }
    catch (Exception ex)
    {
        throw new CryptographicException("Ошибка: " + ex.Message);
    }
}
private void LoadCountriesToJavaScript()
{
    string cacheFilePath = Server.MapPath("~/App_Data/countries_cache.json");

    if (File.Exists(cacheFilePath))
    {
        try
        {
            string jsonData = File.ReadAllText(cacheFilePath, Encoding.UTF8);
            var serializer = new JavaScriptSerializer();
            var countries = serializer.Deserialize<List<Dictionary<string, object>>>(jsonData);

            ddlCountry.Items.Clear();
            foreach (var c in countries)
            {
                if (!c.ContainsKey("ISO3166") || !c.ContainsKey("CallingCode") || !c.ContainsKey("CountryName"))
                    continue;

                string iso = c["ISO3166"].ToString();
                string callingCode = c["CallingCode"].ToString();
                string countryName = c["CountryName"].ToString();

                var item = new ListItem("+"+callingCode, iso);
                item.Attributes.Add("data-code", callingCode);
                ddlCountry.Items.Add(item);
            }

            // Устанавливаем страну по умолчанию (по ISO или CloudFlare)
            string defaultIso = GetUserIsoFromCloudFlare(HttpContext.Current);
            var defaultItem = ddlCountry.Items.FindByValue(defaultIso);
            if (defaultItem != null)
            {
                ddlCountry.SelectedValue = defaultIso;
                callingCode.Value = defaultItem.Attributes["data-code"];
            }
            return;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError("Error loading countries: " + ex.Message);
        }
    }

    // Если что-то пошло не так
    ddlCountry.Items.Clear();
    ddlCountry.Items.Add(new ListItem("Error loading countries", ""));
}


// Вспомогательный метод для извлечения JSON из SOAP ответа
private string ExtractJsonFromSoapResponse(string soapResponse)
{
    if (string.IsNullOrWhiteSpace(soapResponse))
        return "";
    
    try
    {
        var soapDoc = new XmlDocument();
        soapDoc.LoadXml(soapResponse);
        
        XmlNode returnNode = soapDoc.GetElementsByTagName("return")[0];
        if (returnNode == null)
            return "";
        
        string jsonData = returnNode.InnerText.Trim();
        
        if (jsonData.StartsWith("<![CDATA["))
        {
            jsonData = jsonData.Substring(9, jsonData.Length - 12);
        }
        
        return jsonData;
    }
    catch (Exception ex)
    {
        System.Diagnostics.Trace.TraceError("Error extracting JSON from SOAP: " + ex.Message);
        return "";
    }
}
    private string EscapeJsString(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "";
    
        return input
            .Replace("\\", "\\\\")
            .Replace("'", "\\'")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r");
    }
private string FormatGamesJson(string gamesGetResponse)
{
    if (string.IsNullOrWhiteSpace(gamesGetResponse))
        return "[]";

    try
    {
        var soapDoc = new XmlDocument();
        soapDoc.LoadXml(gamesGetResponse);

        XmlNode returnNode = soapDoc.GetElementsByTagName("return")[0];
        if (returnNode == null)
            return "[]";

        string jsonData = returnNode.InnerText.Trim();

        if (jsonData.StartsWith("<![CDATA["))
        {
            jsonData = jsonData.Substring(9, jsonData.Length - 12);
        }

        var serializer = new JavaScriptSerializer();
        var gamesList = serializer.Deserialize<List<Dictionary<string, object>>>(jsonData);

        var formattedGames = new List<object>();
        var uniqueSrc = new HashSet<string>(); // Для отслеживания уникальных src
        string[] defaultColors = { "FF69B4", "1E90FF", "8A2BE2", "FFD700", "DC143C", "708090", "20B2AA", "FF4500" };
        int colorIndex = 0;

        foreach (var game in gamesList)
        {
            string id = game.ContainsKey("gameId") ? game["gameId"].ToString() ?? "" : "";
            string gameName = game.ContainsKey("game_name") ? game["game_name"].ToString() ?? "" : "";
            string categoryId = game.ContainsKey("categoryId") ? game["categoryId"].ToString() ?? "" : "";
            string gameImage = "https://www.playerclub365.com/images/posters/"+id+".jpg";
            
            string encodedSrc = string.IsNullOrEmpty(gameImage) ? "" : ConvertToBase64(gameImage);
            
            // Пропускаем дубликаты по src
            if (uniqueSrc.Contains(encodedSrc))
                continue;
            
            uniqueSrc.Add(encodedSrc);

            formattedGames.Add(new
            {
                id = id,
                name = gameName,
                cid = categoryId,
                color = defaultColors[colorIndex % defaultColors.Length],
                src = encodedSrc
            });
            colorIndex++;
        }

        return serializer.Serialize(formattedGames);
    }
    catch (Exception ex)
    {
        System.Diagnostics.Trace.TraceError("Error formatting games data: " + ex.Message);
        return "[]";
    }
}
    private string ConvertToBase64(string text)
    {
        if (string.IsNullOrEmpty(text))
            return "";
    
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
        return Convert.ToBase64String(bytes);
    }
    
    // -------------------------------------------------------
    public static string EncryptObject<T>(T obj) where T : class
    {
        try
        {
            var serializer = new JavaScriptSerializer();
            return EncryptString(serializer.Serialize(obj));
        }
        catch
        {
            return null;
        }
    }
    public static string EncryptString(string text)
    {
        byte[] key = new Guid("08E694F1-C188-4A79-B158-768EA3A887FB").ToByteArray();
        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.GenerateIV(); // важно!

            using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
            using (var msEncrypt = new MemoryStream())
            {
                // записываем IV в начало
                msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (var swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(text);
                }

                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }
    public static string DecryptString(string cipherText)
    {
        byte[] key = new Guid("08E694F1-C188-4A79-B158-768EA3A887FB").ToByteArray();
        byte[] fullCipher = Convert.FromBase64String(cipherText);

        using (var aesAlg = Aes.Create())
        {
            byte[] iv = new byte[aesAlg.BlockSize / 8];
            byte[] cipher = new byte[fullCipher.Length - iv.Length];

            // извлекаем IV
            Array.Copy(fullCipher, iv, iv.Length);
            Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            aesAlg.Key = key;
            aesAlg.IV = iv;

            using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
            using (var msDecrypt = new MemoryStream(cipher))
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (var srDecrypt = new StreamReader(csDecrypt))
            {
                return srDecrypt.ReadToEnd();
            }
        }
    }
    protected void btnClaim_Click(object sender, EventArgs e)
    {
        string phone      = txtPhone.Text.Trim();
        string countryIso = Request.Form["ddlCountry"].Trim() ?? string.Empty;
        string callingCode = (Request.Form["callingCode"] ?? "").Trim();
        string toEncode = "+"+callingCode + phone;
        string encrypted = EncryptObject(toEncode);
        string encoded = HttpUtility.UrlEncode(encrypted);

        string SuccessUrl2 = SuccessUrl + "?user=" + encoded;
        if (string.IsNullOrWhiteSpace(phone))
        {
            ShowError("Please enter your phone number.");
            return;
        }

        // Strip any non-digit characters the user may have typed
        string digitsOnly = System.Text.RegularExpressions.Regex.Replace(phone, @"\D", "");
    	digitsOnly = digitsOnly.TrimStart('0');
        if (digitsOnly.Length < 3)
        {
            ShowError("Phone number must be at least 3 digits.");
            return;
        }

        // --- Call the SOAP service ---
        int    resultCode;
        string rawResponse;

        try
        {
            rawResponse = CallSigninWithPhone(countryIso, digitsOnly);
            resultCode  = ParseResultCode(rawResponse);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError("SmartWinners SOAP error: " + ex.Message);
            ShowError("A network error occurred. Please try again.");
            return;
        }
        var entityFindResponse = "";
        string entityId = "";
        var entityBonusesUpdateResponse = "";
        var entityBonusesUpdateResult = "";
        var updateEntityResponse = "";
        // --- Route based on result code ---
        switch (resultCode)
        {
            //редиректим на верифи-фон
            case 0:
                var regex = new Regex("\"EntityId\":(\\d+)");
                var match = regex.Match(rawResponse);
                entityId = match.Success ? match.Groups[1].Value : null;
                var logResp = CallLog(entityId);
                updateEntityResponse = UpdateEntityInformation(entityId, countryIso);
                entityBonusesUpdateResponse = CallEntityBonusesUpdate(entityId);
                Response.Redirect(SuccessUrl2, true);
                break;

            case 1:
                ShowError("A verification SMS has been sent to your number. Please check your messages.");
                break;
            //юзер найден, делаем бонус
            case 2:
                regex = new Regex("\"EntityId\":(\\d+)");
                match = regex.Match(rawResponse);
                entityId = match.Success ? match.Groups[1].Value : null;
                logResp = CallLog(entityId);
                updateEntityResponse = UpdateEntityInformation(entityId, countryIso);
                entityBonusesUpdateResponse = CallEntityBonusesUpdate(entityId);
                Response.Redirect(SignInUrl, true);
                break;

            case 3:
                regex = new Regex("\"EntityId\":(\\d+)");
                match = regex.Match(rawResponse);
                entityId = match.Success ? match.Groups[1].Value : null;
                logResp = CallLog(entityId);
                updateEntityResponse = UpdateEntityInformation(entityId, countryIso);
                entityBonusesUpdateResponse = CallEntityBonusesUpdate(entityId);
                Response.Redirect(SuccessUrl2, true);                break;

            case -13:
                regex = new Regex("\"EntityId\":(\\d+)");
                match = regex.Match(rawResponse);
                entityId = match.Success ? match.Groups[1].Value : null;
                logResp = CallLog(entityId);
                updateEntityResponse = UpdateEntityInformation(entityId, countryIso);
                entityBonusesUpdateResponse = CallEntityBonusesUpdate(entityId);
                Response.Redirect(SuccessUrl2, true);
                break;

            case -14:
                ShowError("The verification code is incorrect or the message was not found. Please try again.");
                break;

            case -16:
                ShowError("Mobile number not found. Please check the number and try again.");
                break;

            case 390231:
                ShowError("Phone number must be at least 3 digits.");
                break;

            case 394835:
                ShowError("The country you selected appears to be incorrect. Please try again.");
                break;

            case -2:
                ShowError("A database error occurred on our end. Please try again shortly.");
                break;

            case 80005:
            case 80007:
                ShowError("An unexpected service error occurred (code: " + resultCode + "). Please try again.");
                break;

            default:
                ShowError("!!!An unexpected error occurred (code: " + resultCode + "). Please try again.");
                break;
        }
    }

    protected void btnClaim2_Click(object sender, EventArgs e)
    {
        var cookie = Request.Cookies["cred"];
        var entityBonusesCheckResponse = "";
        var entityBonusesCheckResult = "";
        var entityId = "";

        if (cookie != null)
        {
            try
            {
                string base64Token = HttpUtility.UrlDecode(cookie.Value);
                var credential = DecryptCredential(base64Token);
                entityId = credential.EntityId.ToString();
                if (entityId != "" && entityId != " ") entityBonusesCheckResponse = CallEntityBonusesUpdate(entityId);
                if (!string.IsNullOrWhiteSpace(entityBonusesCheckResponse))
                {
                    var match = Regex.Match(entityBonusesCheckResponse, @"""ResultMessage""\s*:\s*""([^""]*)""");

                    if (match.Success)
                    {
                        entityBonusesCheckResult = match.Groups[1].Value;
                        if (entityBonusesCheckResult.Contains("OK"))
                        {
                            phoneInputContainer.Visible = false;
                            readonlyPhoneContainer.Visible = false;
                            btnClaim.Visible = false;
                            
                            ShowSuccess("Bonuses given successfully");
                            string redirectScript = @"
                                setTimeout(function() {
                                    window.location.href = '"+BaseUrl+@"';
                                }, 1000);
                            ";

                            // Вставляем скрипт на страницу
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "redirectAfterSuccess",
                                redirectScript, true);
                        }
                        else if (entityBonusesCheckResult.Contains("insert into customtable199"))
                        {
                            // Аналогично для уже выданных бонусов
                            phoneInputContainer.Visible = false;
                            readonlyPhoneContainer.Visible = true;
                            btnClaim.Visible = false;
    
                            ShowSuccess("Bonuses already given for this user");
                            string redirectScript = @"
                                setTimeout(function() {
                                    window.location.href = '"+BaseUrl+@"';
                                }, 1000);
                            ";

                            // Вставляем скрипт на страницу
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "redirectAfterSuccess", redirectScript, true);
                        }
                        else
                        {
                            ShowError("Error while giving bonuses. Try again later");
                        }
                    }
                }
                else
                {
                    ShowError("Error while giving bonuses. Try again later");
                }
            }
            catch
            {

            }
        }
    }

    // -------------------------------------------------------
    private string CallSigninWithPhone(string countryIso, string phoneNumber)
    {
        string affiliateId = Request.QueryString["aid"];
        string soapEnvelope = string.Format(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope
    xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/""
    xmlns:soapenc=""http://schemas.xmlsoap.org/soap/encoding/""
    xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
    xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
    soap:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
  <soap:Body>
    <ns1:Signin_With_Phone xmlns:ns1=""urn:SmartWinners.Intf-ISmartWinners"">
      <CountryISO xsi:type=""xsd:string"">{0}</CountryISO>
      <PhoneNumber xsi:type=""xsd:string"">{1}</PhoneNumber>
      <VerificationCode xsi:type=""xsd:string""></VerificationCode>
      <affiliate_entityID xsi:type=""xsd:int"">{2}</affiliate_entityID>
    </ns1:Signin_With_Phone>
  </soap:Body>
</soap:Envelope>",
            countryIso,
            phoneNumber, (affiliateId != null && affiliateId != "" ? affiliateId : "0"));

        using (var client = new WebClient())
        {
            client.Encoding = Encoding.UTF8;
            client.Headers.Add("Content-Type", "text/xml; charset=utf-8");
            client.Headers.Add("SOAPAction", "\"" + SoapAction + "\"");

            byte[] requestBytes  = Encoding.UTF8.GetBytes(soapEnvelope);
            byte[] responseBytes = client.UploadData(SmartWinnersApi, "POST", requestBytes);
            return Encoding.UTF8.GetString(responseBytes);
        }
    }
    
    private string CallGamesGet(string categoryId)
    {
        string soapEnvelope = string.Format(
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <env:Envelope xmlns:env=""http://www.w3.org/2003/05/soap-envelope"" 
            xmlns:ns1=""urn:Playerclub365.Intf-IPlayerclub365""
             xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
            xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
             xmlns:enc=""http://www.w3.org/2003/05/soap-encoding"" 
            xmlns:ns2=""urn:CommonWSTypes"">
            <env:Body>
            <ns1:Games_Get env:encodingStyle=""http://www.w3.org/2003/05/soap-encoding"">
            <Ol_EntityId xsi:type=""xsd:int"">27827</Ol_EntityId>
            <Ol_Username xsi:type=""xsd:string"">442033973506</Ol_Username>
            <Ol_Password xsi:type=""xsd:string"">{0}</Ol_Password>
            <Lang_Code xsi:type=""xsd:string"">en</Lang_Code>
            <Fields enc:itemType=""xsd:string"" enc:arraySize=""2"" xsi:type=""ns2:ArrayOfString"">
            <item xsi:type=""xsd:string"">pg.gameId</item>
            <item xsi:type=""xsd:string"">game_name</item>
            <item xsi:type=""xsd:string"">categoryId</item>
            </Fields>
            <FilterFields enc:itemType=""xsd:string"" enc:arraySize=""1"" xsi:type=""ns2:ArrayOfString"">
            <item xsi:type=""xsd:string"">categoryId</item></FilterFields>
            <FilterValues enc:itemType=""xsd:string"" enc:arraySize=""1"" xsi:type=""ns2:ArrayOfString"">
            <item xsi:type=""xsd:string"">{1}</item></FilterValues>
            <LimitFrom xsi:type=""xsd:int"">0</LimitFrom>
            <LimitCount xsi:type=""xsd:int"">20</LimitCount>
            </ns1:Games_Get></env:Body></env:Envelope>",
            Pwd,
            categoryId != "" && categoryId != null ? categoryId : ">0"
            );

        using (var client = new WebClient())
        {
            client.Encoding = Encoding.UTF8;
            client.Headers.Add("Content-Type", "text/xml; charset=utf-8");
            client.Headers.Add("SOAPAction", "\"" + "urn:Playerclub365.Intf-IPlayerclub365#Games_Get" + "\"");

            byte[] requestBytes  = Encoding.UTF8.GetBytes(soapEnvelope);
            byte[] responseBytes = client.UploadData(PlayerApi, "POST", requestBytes);
            return Encoding.UTF8.GetString(responseBytes);
        }
    }
    
    private string CallEntityFind(string countryIso, string phoneNumber)
    {
        string soapEnvelope = string.Format(
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <env:Envelope xmlns:env=""http://www.w3.org/2003/05/soap-envelope""
             xmlns:ns1=""urn:BusinessApiIntf-IBusinessAPI""
             xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
             xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
             xmlns:enc=""http://www.w3.org/2003/05/soap-encoding""
             xmlns:ns2=""urn:CommonWSTypes"">
            <env:Body>
            <ns1:Entity_Find env:encodingStyle=""http://www.w3.org/2003/05/soap-encoding"">
            <ol_EntityId xsi:type=""xsd:int"">27827</ol_EntityId>
            <ol_UserName xsi:type=""xsd:string"">442033973506</ol_UserName>
            <ol_Password xsi:type=""xsd:string"">{0}</ol_Password>
            <BusinessId xsi:type=""xsd:int"">1</BusinessId>
            <Limit_entities_per_business xsi:type=""xsd:boolean"">false</Limit_entities_per_business>
            <Fields enc:itemType=""xsd:string"" enc:arraySize=""1"" xsi:type=""ns2:ArrayOfString"">
            <item xsi:type=""xsd:string"">entityId</item></Fields>
            <FilterFields enc:itemType=""xsd:string"" enc:arraySize=""2"" xsi:type=""ns2:ArrayOfString"">
            <item xsi:type=""xsd:string"">mobile</item>
            <item xsi:type=""xsd:string"">country</item>
            </FilterFields><FilterValues enc:itemType=""xsd:string"" enc:arraySize=""2"" xsi:type=""ns2:ArrayOfString"">
            <item xsi:type=""xsd:string"">{1}</item>
            <item xsi:type=""xsd:string"">{2}</item></FilterValues>
            <LimitFrom xsi:type=""xsd:int"">0</LimitFrom>
            <LimitCount xsi:type=""xsd:int"">0</LimitCount>
            </ns1:Entity_Find></env:Body></env:Envelope>",
            Pwd,
            phoneNumber,
            countryIso);

        using (var client = new WebClient())
        {
            client.Encoding = Encoding.UTF8;
            client.Headers.Add("Content-Type", "text/xml; charset=utf-8");
            client.Headers.Add("SOAPAction", "\"" + "urn:BusinessApiIntf-IBusinessAPI#Entity_Find" + "\"");

            byte[] requestBytes  = Encoding.UTF8.GetBytes(soapEnvelope);
            byte[] responseBytes = client.UploadData(BusinessApi, "POST", requestBytes);
            return Encoding.UTF8.GetString(responseBytes);
        }
    }
    
    private string CallEntityFindAffiliate(string aid)
    {
        string soapEnvelope = string.Format(
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <env:Envelope xmlns:env=""http://www.w3.org/2003/05/soap-envelope""
             xmlns:ns1=""urn:BusinessApiIntf-IBusinessAPI""
             xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
             xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
             xmlns:enc=""http://www.w3.org/2003/05/soap-encoding""
             xmlns:ns2=""urn:CommonWSTypes"">
            <env:Body>
            <ns1:Entity_Find env:encodingStyle=""http://www.w3.org/2003/05/soap-encoding"">
            <ol_EntityId xsi:type=""xsd:int"">27827</ol_EntityId>
            <ol_UserName xsi:type=""xsd:string"">442033973506</ol_UserName>
            <ol_Password xsi:type=""xsd:string"">{0}</ol_Password>
            <BusinessId xsi:type=""xsd:int"">1</BusinessId>
            <Limit_entities_per_business xsi:type=""xsd:boolean"">false</Limit_entities_per_business>
            <Fields enc:itemType=""xsd:string"" enc:arraySize=""1"" xsi:type=""ns2:ArrayOfString"">
            <item xsi:type=""xsd:string"">customfield165</item></Fields>
            <FilterFields enc:itemType=""xsd:string"" enc:arraySize=""2"" xsi:type=""ns2:ArrayOfString"">
            <item xsi:type=""xsd:string"">entityId</item>
            </FilterFields><FilterValues enc:itemType=""xsd:string"" enc:arraySize=""2"" xsi:type=""ns2:ArrayOfString"">
            <item xsi:type=""xsd:string"">{1}</item></FilterValues>
            <LimitFrom xsi:type=""xsd:int"">0</LimitFrom>
            <LimitCount xsi:type=""xsd:int"">0</LimitCount>
            </ns1:Entity_Find></env:Body></env:Envelope>",
            Pwd,
            aid
            );

        using (var client = new WebClient())
        {
            client.Encoding = Encoding.UTF8;
            client.Headers.Add("Content-Type", "text/xml; charset=utf-8");
            client.Headers.Add("SOAPAction", "\"" + "urn:BusinessApiIntf-IBusinessAPI#Entity_Find" + "\"");

            byte[] requestBytes  = Encoding.UTF8.GetBytes(soapEnvelope);
            byte[] responseBytes = client.UploadData(BusinessApi, "POST", requestBytes);
            return Encoding.UTF8.GetString(responseBytes);
        }
    }
    private string UpdateEntityInformation(string entityId, string countryISO)
    {
        string domain = HttpContext.Current.Request.Url.AbsoluteUri.Replace("https://", "").Replace("http://", "");
        int endIndex = domain.IndexOf('/');
        if (endIndex > 0)
        {
            domain = domain.Substring(0, endIndex);
        }
        string clientIp = "";
        if (Request.Cookies["clientIp"] != null)
        {
              clientIp = Request.Cookies["clientIp"].Value.ToString();
        }

        string referrerUrl = "";
        if(Request.Cookies["Referer"] != null)
            referrerUrl = Request.Cookies["Referer"].Value;
        string soapEnvelope = string.Format(
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <env:Envelope xmlns:env=""http://www.w3.org/2003/05/soap-envelope"" 
                xmlns:ns1=""urn:BusinessApiIntf-IBusinessAPI"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                 xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                 xmlns:enc=""http://www.w3.org/2003/05/soap-encoding""
                 xmlns:ns2=""urn:CommonWSTypes""><env:Body>
                <ns1:Entity_Update env:encodingStyle=""http://www.w3.org/2003/05/soap-encoding"">
                <ol_EntityID xsi:type=""xsd:int"">27827</ol_EntityID>
                <ol_Username xsi:type=""xsd:string"">442033973506</ol_Username>
                <ol_Password xsi:type=""xsd:string"">{0}</ol_Password>
                <EntityId xsi:type=""xsd:int"">{1}</EntityId>
                <NamesArray enc:itemType=""xsd:string"" enc:arraySize=""5"" xsi:type=""ns2:ArrayOfString"">
                <item xsi:type=""xsd:string"">customfield9</item>
                <item xsi:type=""xsd:string"">customfield71</item>
                <item xsi:type=""xsd:string"">customfield68</item>
                <item xsi:type=""xsd:string"">customfield67</item>
                <item xsi:type=""xsd:string"">categoryId</item>
                </NamesArray>
                <ValuesArray enc:itemType=""xsd:string"" enc:arraySize=""5"" xsi:type=""ns2:ArrayOfString"">
                <item xsi:type=""xsd:string"">{2}</item>
                <item xsi:type=""xsd:string"">{3}</item>
                <item xsi:type=""xsd:string"">{4}</item>
                <item xsi:type=""xsd:string"">{5}</item>
                <item xsi:type=""xsd:string"">123</item>
                </ValuesArray><ImageFields xsi:nil=""true"" xsi:type=""ns2:ArrayOfString""/>
                <ImageValues xsi:nil=""true"" xsi:type=""ns2:ArrayOfString""/>
                </ns1:Entity_Update></env:Body></env:Envelope>",
            Pwd,
            entityId,
            clientIp,
            referrerUrl,
            domain,
            "EN"
            );

        using (var client = new WebClient())
        {
            client.Encoding = Encoding.UTF8;
            client.Headers.Add("Content-Type", "text/xml; charset=utf-8");
            client.Headers.Add("SOAPAction", "\"" + "urn:BusinessApiIntf-IBusinessAPI#Entity_Update" + "\"");

            byte[] requestBytes  = Encoding.UTF8.GetBytes(soapEnvelope);
            byte[] responseBytes = client.UploadData(BusinessApi, "POST", requestBytes);
            return Encoding.UTF8.GetString(responseBytes);
        }
    } 
    private int GetOsFromUserAgent(string userAgent)
    {
        if (userAgent.Contains("Android"))
            return 1; // Android
        else if (userAgent.Contains("iPhone") || userAgent.Contains("iPad") || userAgent.Contains("iPod"))
            return 2; // iOS
        else if (userAgent.Contains("Macintosh") || userAgent.Contains("Mac OS X"))
            return 0; // Web browser (Mac)
        else if (userAgent.Contains("Linux"))
            return 5; // Web browser (Linux)
        else if (userAgent.Contains("Windows"))
            return 3; // Windows
        else
            return 4; // По умолчанию: Web browser
    }
    private string CallLog(string entityId)
    {
    string scheme = HttpContext.Current.Request.Url.Scheme; // "http" или "https"
    string host = HttpContext.Current.Request.Url.Host;
    int port = HttpContext.Current.Request.Url.Port;
    string path = HttpContext.Current.Request.Path;
    string query = HttpContext.Current.Request.Url.Query;

    // Убираем "default.aspx" из пути, если он есть
    if (!string.IsNullOrEmpty(path) && path.EndsWith("/default.aspx", StringComparison.OrdinalIgnoreCase))
    {
        path = path.Replace("/default.aspx", "");
    }

    // Формируем полный URL
    string domain = scheme+"://"+host;

    domain += path+query;

        string clientIp = "";
        if (Request.Cookies["clientIp"] != null)
        {
              clientIp = Request.Cookies["clientIp"].Value.ToString();
        }

        string referrerUrl = "";
        if(Request.Cookies["Referer"] != null)
            referrerUrl = Request.Cookies["Referer"].Value;
        string soapEnvelope = string.Format(
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
<env:Envelope xmlns:env=""http://www.w3.org/2003/05/soap-envelope""
 xmlns:ns1=""urn:BusinessApiIntf-IBusinessAPI""
 xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
 xmlns:enc=""http://www.w3.org/2003/05/soap-encoding""
 xmlns:ns2=""urn:CommonWSTypes"">
<env:Body><ns1:Entity_Logtraffic env:encodingStyle=""http://www.w3.org/2003/05/soap-encoding"">
<ol_EntityID xsi:type=""xsd:int"">{0}</ol_EntityID>
<device xsi:type=""xsd:int"">{1}</device>
<System xsi:type=""xsd:string"">{2}</System>
<IP xsi:type=""xsd:string"">{3}</IP>
<URL xsi:type=""xsd:string"">{4}</URL>
<NamesArray enc:itemType=""xsd:string"" enc:arraySize=""2"" xsi:type=""ns2:ArrayOfString"">
<item xsi:type=""xsd:string"">referer</item>
<item xsi:type=""xsd:string"">acceptLanguage</item>
</NamesArray>
<ValuesArray enc:itemType=""xsd:string"" enc:arraySize=""2"" xsi:type=""ns2:ArrayOfString"">
<item xsi:type=""xsd:string"">{5}</item>
<item xsi:type=""xsd:string"">{6}</item></ValuesArray>
</ns1:Entity_Logtraffic></env:Body></env:Envelope>",
            entityId,
            GetOsFromUserAgent(Request.UserAgent).ToString(),
            Request.UserAgent,
            clientIp,
            domain,
            referrerUrl,
            "EN"
            );

        using (var client = new WebClient())
        {
            client.Encoding = Encoding.UTF8;
            client.Headers.Add("Content-Type", "text/xml; charset=utf-8");
            client.Headers.Add("SOAPAction", "\"" + "urn:BusinessApiIntf-IBusinessAPI#Entity_Logtraffic" + "\"");

            byte[] requestBytes  = Encoding.UTF8.GetBytes(soapEnvelope);
            byte[] responseBytes = client.UploadData(BusinessApi, "POST", requestBytes);
            return Encoding.UTF8.GetString(responseBytes);
        }
    }
    private string CheckIfBonusExists(string entityId)
    {
        string soapEnvelope = string.Format(
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
<env:Envelope xmlns:env=""http://www.w3.org/2003/05/soap-envelope""
 xmlns:ns1=""urn:BusinessApiIntf-IBusinessAPI""
 xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
 xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
 xmlns:enc=""http://www.w3.org/2003/05/soap-encoding""
 xmlns:ns2=""urn:CommonWSTypes"">
<env:Body><ns1:CustomFields_Tables_Get env:encodingStyle=""http://www.w3.org/2003/05/soap-encoding"">
<ol_EntityID xsi:type=""xsd:int"">27827</ol_EntityID>
<ol_Username xsi:type=""xsd:string"">442033973506</ol_Username>
<ol_Password xsi:type=""xsd:string"">{0}</ol_Password>
<TableID xsi:type=""xsd:int"">199</TableID>
<ParentRecordID xsi:type=""xsd:int"">{1}</ParentRecordID>
<Fields enc:itemType=""xsd:string"" enc:arraySize=""3"" xsi:type=""ns2:ArrayOfString"">
<item xsi:type=""xsd:string"">customfield201</item>
<item xsi:type=""xsd:string"">customfield202</item>
<item xsi:type=""xsd:string"">customfield203</item>
</Fields><FilterFields enc:itemType=""xsd:string"" enc:arraySize=""1"" xsi:type=""ns2:ArrayOfString"">
<item xsi:type=""xsd:string"">customfield203</item></FilterFields>
<FilterValues enc:itemType=""xsd:string"" enc:arraySize=""1"" xsi:type=""ns2:ArrayOfString"">
<item xsi:type=""xsd:string"">&gt;0</item></FilterValues>
<LimitFrom xsi:type=""xsd:int"">0</LimitFrom>
<LimitCount xsi:type=""xsd:int"">0</LimitCount>
</ns1:CustomFields_Tables_Get></env:Body></env:Envelope>",
            Pwd,
            entityId
            );

        using (var client = new WebClient())
        {
            client.Encoding = Encoding.UTF8;
            client.Headers.Add("Content-Type", "text/xml; charset=utf-8");
            client.Headers.Add("SOAPAction", "\"" + "urn:BusinessApiIntf-IBusinessAPI#Entity_Find" + "\"");

            byte[] requestBytes  = Encoding.UTF8.GetBytes(soapEnvelope);
            byte[] responseBytes = client.UploadData(BusinessApi, "POST", requestBytes);
            return Encoding.UTF8.GetString(responseBytes);
        }
    }
    
    private string CallEntityBonusesUpdate(string entityId)
    {
        string soapEnvelope = string.Format(
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <env:Envelope xmlns:env=""http://www.w3.org/2003/05/soap-envelope""
                 xmlns:ns1=""urn:Playerclub365.Intf-IPlayerclub365""
                 xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                 xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                 xmlns:ns2=""urn:CommonWSTypes""
                 xmlns:enc=""http://www.w3.org/2003/05/soap-encoding"">
                <env:Body><ns1:Entity_Bonuses_Update env:encodingStyle=""http://www.w3.org/2003/05/soap-encoding"">
                <Ol_EntityId xsi:type=""xsd:int"">27827</Ol_EntityId>
                <Ol_Username xsi:type=""xsd:string"">442033973506</Ol_Username>
                <Ol_Password xsi:type=""xsd:string"">{0}</Ol_Password>
                <recordId xsi:type=""xsd:int"">0</recordId>
                <EntityId xsi:type=""xsd:int"">{1}</EntityId>
                <BonusType xsi:type=""xsd:int"">15</BonusType>
                <Serial xsi:type=""xsd:int"">0</Serial>
                <Value xsi:type=""xsd:double"">20</Value>
                <NamesArray enc:itemType=""xsd:string"" enc:arraySize=""1"" xsi:type=""ns2:ArrayOfString"">
				<item xsi:type=""xsd:string"">CustomField206</item>
				</NamesArray>
                <ValuesArray enc:itemType=""xsd:string"" enc:arraySize=""1"" xsi:type=""ns2:ArrayOfString"">
				<item xsi:type=""xsd:string"">{2}</item>
				</ValuesArray>
                </ns1:Entity_Bonuses_Update></env:Body></env:Envelope>",
            Pwd,
            entityId,
			DateTime.Now.AddMonths(6).ToString("yyyy-MM-dd")
            );

        using (var client = new WebClient())
        {
            client.Encoding = Encoding.UTF8;
            client.Headers.Add("Content-Type", "text/xml; charset=utf-8");
            client.Headers.Add("SOAPAction", "\"" + "urn:Playerclub365.Intf-IPlayerclub365#Entity_Bonuses_Update" + "\"");

            byte[] requestBytes  = Encoding.UTF8.GetBytes(soapEnvelope);
            byte[] responseBytes = client.UploadData(PlayerApi, "POST", requestBytes);
            return Encoding.UTF8.GetString(responseBytes);
        }
    }
    
    private string CallGameCategoriesGet(string categoryId)
    {
        string soapEnvelope = string.Format(
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <env:Envelope xmlns:env=""http://www.w3.org/2003/05/soap-envelope""
             xmlns:ns1=""urn:Playerclub365.Intf-IPlayerclub365""
             xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
             xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
             xmlns:enc=""http://www.w3.org/2003/05/soap-encoding""
             xmlns:ns2=""urn:CommonWSTypes"">
            <env:Body><ns1:Game_Categories_Get env:encodingStyle=""http://www.w3.org/2003/05/soap-encoding"">
            <Ol_EntityId xsi:type=""xsd:int"">27827</Ol_EntityId>
            <Ol_Username xsi:type=""xsd:string"">442033973506</Ol_Username>
            <Ol_Password xsi:type=""xsd:string"">{0}</Ol_Password>
            <Lang_Code xsi:type=""xsd:string"">en</Lang_Code>
            <Fields enc:itemType=""xsd:string"" enc:arraySize=""1"" xsi:type=""ns2:ArrayOfString"">
            <item xsi:type=""xsd:string"">category_name</item>
            <item xsi:type=""xsd:string"">gcs.og_description</item>
            </Fields><FilterFields enc:itemType=""xsd:string"" enc:arraySize=""1"" xsi:type=""ns2:ArrayOfString"">
            <item xsi:type=""xsd:string"">gc.categoryId</item>
            </FilterFields><FilterValues enc:itemType=""xsd:string"" enc:arraySize=""1"" xsi:type=""ns2:ArrayOfString"">
            <item xsi:type=""xsd:string"">{1}</item></FilterValues>
            <LimitFrom xsi:type=""xsd:int"">0</LimitFrom>
            <LimitCount xsi:type=""xsd:int"">0</LimitCount>
            </ns1:Game_Categories_Get></env:Body></env:Envelope>",
            Pwd,
            categoryId
            );

        using (var client = new WebClient())
        {
            client.Encoding = Encoding.UTF8;
            client.Headers.Add("Content-Type", "text/xml; charset=utf-8");
            client.Headers.Add("SOAPAction", "\"" + "urn:Playerclub365.Intf-IPlayerclub365#Game_Categories_Get" + "\"");

            byte[] requestBytes  = Encoding.UTF8.GetBytes(soapEnvelope);
            byte[] responseBytes = client.UploadData(PlayerApi, "POST", requestBytes);
            return Encoding.UTF8.GetString(responseBytes);
        }
    }
    // -------------------------------------------------------
    private int ParseResultCode(string rawSoapResponse)
    {
        if (string.IsNullOrWhiteSpace(rawSoapResponse))
            throw new InvalidOperationException("Empty response from SmartWinners service.");

        try
        {
            var soapDoc = new XmlDocument();
            soapDoc.LoadXml(rawSoapResponse);

            XmlNode returnNode = null;
            foreach (XmlNode node in soapDoc.GetElementsByTagName("return"))
            {
                returnNode = node;
                break;
            }

            if (returnNode == null)
                throw new InvalidOperationException("Could not locate <return> in SOAP response.");

            string innerXml = returnNode.InnerText.Trim();

            if (innerXml.StartsWith("<"))
            {
                var innerDoc = new XmlDocument();
                innerDoc.LoadXml("<root>" + innerXml + "</root>");

                XmlNode rcNode = innerDoc.SelectSingleNode("//ResultCode");
                if (rcNode != null)
                {
                    int code;
                    if (int.TryParse(rcNode.InnerText.Trim(), out code))
                        return code;
                }
            }

            var match = System.Text.RegularExpressions.Regex.Match(
                innerXml,
                @"ResultCode[""\:\s>]+(-?\d+)",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (match.Success)
            {
                int fallbackCode;
                if (int.TryParse(match.Groups[1].Value, out fallbackCode))
                    return fallbackCode;
            }

            throw new InvalidOperationException(
                "Could not parse ResultCode from response: " +
                innerXml.Substring(0, Math.Min(200, innerXml.Length)));
        }
        catch (XmlException xmlEx)
        {
            throw new InvalidOperationException("Invalid XML in SOAP response: " + xmlEx.Message);
        }
    }

    // -------------------------------------------------------
    private void ShowError(string message)
    {
        lblError.Text    = message;
        lblError.Visible = true;

        ScriptManager.RegisterStartupScript(
            this, GetType(), "hideLoader",
            "document.getElementById('loadingOverlay').classList.remove('active');",
            true);
    }
    private void ShowSuccess(string message)
    {
        lblSuccess.Text    = message;
        lblSuccess.Visible = true;

        ScriptManager.RegisterStartupScript(
            this, GetType(), "hideLoader",
            "document.getElementById('loadingOverlay').classList.remove('active');",
            true);
    }

    // -------------------------------------------------------
    private static string EscapeXml(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return value
            .Replace("&",  "&amp;")
            .Replace("<",  "&lt;")
            .Replace(">",  "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'",  "&apos;");
    }
}
public class Credential
{
    public int EntityId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Lid { get; set; }
    public int AffiliateId { get; set; }
}
