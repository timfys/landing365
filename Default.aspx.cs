using System;
using System.Net;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Security.Cryptography;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;

public partial class Default : Page
{
    // -------------------------------------------------------
    // SmartWinners SOAP endpoint (from the WSDL service node)
    // -------------------------------------------------------
    private const string SoapEndpoint =
        "http://isapi.smart-winners.com/smartwinners.dll/soap/ISmartWinners";

    private const string SoapAction =
        "urn:SmartWinners.Intf-ISmartWinners#Signin_With_Phone";

    // Redirect destination when ResultCode == 0
    private const string SuccessUrl =
        "https://www.playerclub365.com/verify-phone";
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
    // -------------------------------------------------------
protected void Page_Load(object sender, EventArgs e)
{
    string categoryId = Request.QueryString["cid"];
    string callGameCategoriesGetResponse = "";
    string categoryName = "";
    string ogDescription = "";
    string gamesGetResponse = "";
    
    gamesGetResponse = CallGamesGet(categoryId);
    var gamesJson = FormatGamesJson(gamesGetResponse);
    string script = "var gamesFromServer = "+gamesJson+";";
    Page.ClientScript.RegisterStartupScript(this.GetType(), "gamesData", script, true);
    
    var titleContent = "Claim Your 10 FREE Coins &#8211; PlayerClub365";
    metaTitle.Text = "<meta name=\"title\" content=\"" + titleContent + "\">";
    title.Text = "<title>" + titleContent + "</title>";
    desc.Text =
        "<meta name=\"description\" content=\"You've been gifted 10 free coins! Claim now and start playing top social casino games risk-free. No purchase necessary.\"/>\n";
    ogDesc.Text =
        "<meta property=\"og:description\" content=\"You've been gifted 10 free coins! Claim now and start playing top social casino games risk-free. No purchase necessary.\">\n";
    ogTitle.Text =
        "<meta property=\"og:title\" content=\"Claim Your 10 FREE Coins – PlayerClub365\">\n";
    
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
                            }
                        }, 100);
                    ";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "showCategoryDiv", showDivScript, true);
                    
                    titleContent = categoryName + " &#8211; Claim Your 10 FREE Coins &#8211; PlayerClub365";
                    metaTitle.Text = "<meta name=\"title\" content=\"" + titleContent + "\">";
                    title.Text = "<title>" + titleContent + "</title>";
                    ogTitle.Text = "<meta property=\"og:title\" content=\"" + titleContent + "\">\n";
                }
                
                if (!string.IsNullOrEmpty(ogDescription))
                {
                    desc.Text = "<meta name=\"description\" content=\"" + ogDescription +
                                  " &#8211; You've been gifted 10 free coins! Claim now and start playing top social casino games risk-free. No purchase necessary.\"/>\n";
                    ogDesc.Text = "<meta property=\"og:description\" content=\"" + ogDescription + 
                                  " &#8211; You've been gifted 10 free coins! Claim now and start playing top social casino games risk-free. No purchase necessary.\">\n";
                }
            }
        }
    }
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
            string gameName = game.ContainsKey("game_name") ? game["game_name"].ToString() ?? "" : "";
            string categoryId = game.ContainsKey("categoryId") ? game["categoryId"].ToString() ?? "" : "";
            string gameImage = game.ContainsKey("game_image") ? game["game_image"].ToString() ?? "" : "";
            
            string encodedSrc = string.IsNullOrEmpty(gameImage) ? "" : ConvertToBase64(gameImage);
            
            // Пропускаем дубликаты по src
            if (uniqueSrc.Contains(encodedSrc))
                continue;
            
            uniqueSrc.Add(encodedSrc);

            formattedGames.Add(new
            {
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
        // --- Route based on result code ---
        switch (resultCode)
        {
            //редиректим на верифи-фон
            case 0:
                Response.Redirect(SuccessUrl2, true);
                break;

            case 1:
                ShowError("A verification SMS has been sent to your number. Please check your messages.");
                break;
            //юзер найден, делаем бонус
            case 2:
                //Response.Redirect(SuccessUrl2, true);
                entityFindResponse = CallEntityFind(countryIso, digitsOnly);
                if (!string.IsNullOrWhiteSpace(entityFindResponse))
                {
                    var match = Regex.Match(entityFindResponse, @"""entityId""\s*:\s*(\d+)");
    
                    if (match.Success)
                    {
                        entityId = match.Groups[1].Value.Replace(" ","");
                    }
                }
                else
                {
                    ShowError("Error while getting user info. Try again later");
                }
                if(entityId != "" && entityId != " ")entityBonusesUpdateResponse = CallEntityBonusesUpdate(entityId);
                if (!string.IsNullOrWhiteSpace(entityBonusesUpdateResponse))
                {
                    var match = Regex.Match(entityBonusesUpdateResponse, @"""ResultMessage""\s*:\s*""([^""]*)""");
    
                    if (match.Success)
                    {
                        entityBonusesUpdateResult = match.Groups[1].Value;
                        if (entityBonusesUpdateResult.Contains("OK"))
                        {
                            // Скрываем контейнер с активным вводом
                            phoneInputContainer.Visible = false;
    
                            // Показываем read-only контейнер
                            readonlyPhoneContainer.Visible = true;
                            btnClaim.Visible = false;

                            // Устанавливаем текст с номером телефона (с кодом страны, если нужно)
                            string countryCode = callingCode; 
                            string phoneNumber = txtPhone.Text;
                            lblReadonlyPhone.Text = "+"+countryCode+" "+phoneNumber;
    
                            ShowSuccess("Bonuses given successfully");
                        }
                        else if (entityBonusesUpdateResult.Contains("insert into customtable199"))
                        {
                            // Аналогично для уже выданных бонусов
                            phoneInputContainer.Visible = false;
                            readonlyPhoneContainer.Visible = true;
                            btnClaim.Visible = false;

                            string countryCode = callingCode;
                            string phoneNumber = txtPhone.Text;
                            lblReadonlyPhone.Text = "+"+countryCode+phoneNumber;
    
                            ShowSuccess("Bonuses already given for this user");
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
                break;

            case 3:
                ShowError("You must wait before requesting another SMS. Please try again later.");
                break;

            case -13:
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

    // -------------------------------------------------------
    private string CallSigninWithPhone(string countryIso, string phoneNumber)
    {
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
      <affiliate_entityID xsi:type=""xsd:int"">0</affiliate_entityID>
    </ns1:Signin_With_Phone>
  </soap:Body>
</soap:Envelope>",

            countryIso,
            phoneNumber);

        using (var client = new WebClient())
        {
            client.Encoding = Encoding.UTF8;
            client.Headers.Add("Content-Type", "text/xml; charset=utf-8");
            client.Headers.Add("SOAPAction", "\"" + SoapAction + "\"");

            byte[] requestBytes  = Encoding.UTF8.GetBytes(soapEnvelope);
            byte[] responseBytes = client.UploadData(SoapEndpoint, "POST", requestBytes);
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
            <Ol_Password xsi:type=""xsd:string"">smart221134</Ol_Password>
            <Lang_Code xsi:type=""xsd:string"">en</Lang_Code>
            <Fields enc:itemType=""xsd:string"" enc:arraySize=""2"" xsi:type=""ns2:ArrayOfString"">
            <item xsi:type=""xsd:string"">game_name</item>
            <item xsi:type=""xsd:string"">categoryId</item>
            <item xsi:type=""xsd:string"">game_image</item>
            </Fields>
            <FilterFields enc:itemType=""xsd:string"" enc:arraySize=""1"" xsi:type=""ns2:ArrayOfString"">
            <item xsi:type=""xsd:string"">categoryId</item></FilterFields>
            <FilterValues enc:itemType=""xsd:string"" enc:arraySize=""1"" xsi:type=""ns2:ArrayOfString"">
            <item xsi:type=""xsd:string"">{0}</item></FilterValues>
            <LimitFrom xsi:type=""xsd:int"">0</LimitFrom>
            <LimitCount xsi:type=""xsd:int"">0</LimitCount>
            </ns1:Games_Get></env:Body></env:Envelope>",

            categoryId != "" && categoryId != null ? categoryId : ">0"
            );

        using (var client = new WebClient())
        {
            client.Encoding = Encoding.UTF8;
            client.Headers.Add("Content-Type", "text/xml; charset=utf-8");
            client.Headers.Add("SOAPAction", "\"" + "urn:Playerclub365.Intf-IPlayerclub365#Games_Get" + "\"");

            byte[] requestBytes  = Encoding.UTF8.GetBytes(soapEnvelope);
            byte[] responseBytes = client.UploadData("http://isapi-sw.profi.chat/casino365.dll/soap/IPlayerclub365", "POST", requestBytes);
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
            <ol_Password xsi:type=""xsd:string"">smart221134</ol_Password>
            <BusinessId xsi:type=""xsd:int"">1</BusinessId>
            <Limit_entities_per_business xsi:type=""xsd:boolean"">false</Limit_entities_per_business>
            <Fields enc:itemType=""xsd:string"" enc:arraySize=""1"" xsi:type=""ns2:ArrayOfString"">
            <item xsi:type=""xsd:string"">entityId</item></Fields>
            <FilterFields enc:itemType=""xsd:string"" enc:arraySize=""2"" xsi:type=""ns2:ArrayOfString"">
            <item xsi:type=""xsd:string"">mobile</item>
            <item xsi:type=""xsd:string"">country</item>
            </FilterFields><FilterValues enc:itemType=""xsd:string"" enc:arraySize=""2"" xsi:type=""ns2:ArrayOfString"">
            <item xsi:type=""xsd:string"">{0}</item>
            <item xsi:type=""xsd:string"">{1}</item></FilterValues>
            <LimitFrom xsi:type=""xsd:int"">0</LimitFrom>
            <LimitCount xsi:type=""xsd:int"">0</LimitCount>
            </ns1:Entity_Find></env:Body></env:Envelope>",

            phoneNumber,
            countryIso);

        using (var client = new WebClient())
        {
            client.Encoding = Encoding.UTF8;
            client.Headers.Add("Content-Type", "text/xml; charset=utf-8");
            client.Headers.Add("SOAPAction", "\"" + "urn:BusinessApiIntf-IBusinessAPI#Entity_Find" + "\"");

            byte[] requestBytes  = Encoding.UTF8.GetBytes(soapEnvelope);
            byte[] responseBytes = client.UploadData("http://5.79.126.74:33322/soap/IBusinessAPI", "POST", requestBytes);
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
                <Ol_Password xsi:type=""xsd:string"">smart221134</Ol_Password>
                <recordId xsi:type=""xsd:int"">0</recordId>
                <EntityId xsi:type=""xsd:int"">{0}</EntityId>
                <BonusType xsi:type=""xsd:int"">15</BonusType>
                <Serial xsi:type=""xsd:int"">0</Serial>
                <Value xsi:type=""xsd:double"">20</Value>
                <NamesArray xsi:nil=""true"" xsi:type=""ns2:ArrayOfString""/>
                <ValuesArray xsi:nil=""true"" xsi:type=""ns2:ArrayOfString""/>
                </ns1:Entity_Bonuses_Update></env:Body></env:Envelope>",

            entityId
            );

        using (var client = new WebClient())
        {
            client.Encoding = Encoding.UTF8;
            client.Headers.Add("Content-Type", "text/xml; charset=utf-8");
            client.Headers.Add("SOAPAction", "\"" + "urn:Playerclub365.Intf-IPlayerclub365#Entity_Bonuses_Update" + "\"");

            byte[] requestBytes  = Encoding.UTF8.GetBytes(soapEnvelope);
            byte[] responseBytes = client.UploadData("http://isapi-sw.profi.chat/casino365.dll/soap/IPlayerclub365", "POST", requestBytes);
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
            <Ol_Password xsi:type=""xsd:string"">smart221134</Ol_Password>
            <Lang_Code xsi:type=""xsd:string"">en</Lang_Code>
            <Fields enc:itemType=""xsd:string"" enc:arraySize=""1"" xsi:type=""ns2:ArrayOfString"">
            <item xsi:type=""xsd:string"">category_name</item>
            <item xsi:type=""xsd:string"">gcs.og_description</item>
            </Fields><FilterFields enc:itemType=""xsd:string"" enc:arraySize=""1"" xsi:type=""ns2:ArrayOfString"">
            <item xsi:type=""xsd:string"">gc.categoryId</item>
            </FilterFields><FilterValues enc:itemType=""xsd:string"" enc:arraySize=""1"" xsi:type=""ns2:ArrayOfString"">
            <item xsi:type=""xsd:string"">{0}</item></FilterValues>
            <LimitFrom xsi:type=""xsd:int"">0</LimitFrom>
            <LimitCount xsi:type=""xsd:int"">0</LimitCount>
            </ns1:Game_Categories_Get></env:Body></env:Envelope>",

            categoryId
            );

        using (var client = new WebClient())
        {
            client.Encoding = Encoding.UTF8;
            client.Headers.Add("Content-Type", "text/xml; charset=utf-8");
            client.Headers.Add("SOAPAction", "\"" + "urn:Playerclub365.Intf-IPlayerclub365#Game_Categories_Get" + "\"");

            byte[] requestBytes  = Encoding.UTF8.GetBytes(soapEnvelope);
            byte[] responseBytes = client.UploadData("http://isapi-sw.profi.chat/casino365.dll/soap/IPlayerclub365", "POST", requestBytes);
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
