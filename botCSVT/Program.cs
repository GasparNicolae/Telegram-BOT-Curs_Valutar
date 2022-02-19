using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot;
using System.Text.RegularExpressions;
using Telegram.Bot.Types.ReplyMarkups;


namespace botCSVT
{
    class PreclucrareDate
    {
        private decimal val;

        public void CheckRates(string var)
        {
            try
            {
                PrimireDate Primire = new PrimireDate();
                val = Primire.GetRates(var);

            }
            catch
            {
                Console.WriteLine("Eroare");
            }
        }

        public decimal Prim { get => val; set => val = value; }
    }

    class PrimireDate
    {
        public decimal GetRates(string var)
        {

            decimal numar = -1;
            XmlNodeList temp_node = xmlDocument.SelectNodes("//*[@currency]");

            foreach (XmlNode xn in temp_node)
            {
                //XmlNode nod = xn["currency"].ParentNode;
                if (xn.Attributes["currency"].Value.ToString() == var)
                {
                    string t = xn.Attributes["rate"].Value.ToString();
                    t = t.Replace(".", ",");
                    numar = Math.Round(decimal.Parse(t), 4, MidpointRounding.AwayFromZero);
                }
            }
            return numar;

        }

        public PrimireDate()
        {
            SetCurrentURL();
            xmlDocument = GetXML(CurrentURL);
        }

        private string CurrentURL;
        private readonly XmlDocument xmlDocument;

        private void SetCurrentURL()
        {
            CurrentURL = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml?631c486b0380abf02038de01a202e3e2";
        }

        private XmlDocument GetXML(string CurrentURL)
        {
            using (WebClient client = new WebClient())
            {
                string xmlContent = client.DownloadString(CurrentURL);

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xmlContent);

                return xmlDocument;
            }
        }
    }

    class Robot
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("741572773:AAH4ZdtR3Yssmj3cj3TILkuRCEsRXH-qw5c");


        static void Main(string[] args)
        {

            Bot.OnMessage += Bot_OnMessage;
            Bot.OnMessageEdited += Bot_OnMessage;
            Bot.OnInlineQuery += Bot_OnInlineQueryReceived;


            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();

        }

        public static void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text && e.Message.Text == "/start")
            {
                Bot.SendTextMessageAsync(
                   e.Message.Chat.Id,
                   "💶 Sunt un robot inline care cunoaște cursul valutar.\nVezi /lista pentru valutele disponibile.",
                   parseMode: Telegram.Bot.Types.Enums.ParseMode.Html
                   );
            }

            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text && e.Message.Text == "/lista" || e.Message.Text == "/start lista")
            {

                Bot.SendTextMessageAsync(
                    e.Message.Chat.Id,
                    "<i>Lista valutelor disponibile:</i>\n\n" +
                    "<b>🇷🇴 RON\n" +
                    "🇪🇺 EUR\n" +
                    "🇺🇸 USD\n" +
                    "🇯🇵 JPY\n" +
                    "🇧🇬 BGN\n" +
                    "🇨🇿 CZK\n" +
                    "🇩🇰 DKK\n" +
                    "🇬🇧 GBP\n" +
                    "🇭🇺 HUF\n" +
                    "🇵🇱 PLN\n" +
                    "🇸🇪 SEK\n" +
                    "🇨🇭 CHF\n" +
                    "🇮🇸 ISK\n" +
                    "🇳🇴 NOK\n" +
                    "🇭🇷 HRK\n" +
                    "🇷🇺 RUB\n" +
                    "🇹🇷 TRY\n" +
                    "🇦🇺 AUD\n" +
                    "🇧🇷 BRL\n" +
                    "🇨🇦 CAD\n" +
                    "🇨🇳 CNY\n" +
                    "🇭🇰 HKD\n" +
                    "🇮🇩 IDR\n" +
                    "🇮🇱 ILS\n" +
                    "🇮🇳 INR\n" +
                    "🇰🇷 KRW\n" +
                    "🇲🇽 MXN\n" +
                    "🇲🇾 MYR\n" +
                    "🇳🇿 NZD\n" +
                    "🇵🇭 PHP\n" +
                    "🇸🇬 SGD\n" +
                    "🇹🇭 THB\n" +
                    "🇿🇦 ZAR</b>\n\n" +
                    "ex: <code>@CSVTbot 12 EUR RON</code>",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html
                    );
            }

            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text && e.Message.Text != "/lista" && e.Message.Text != "/start" && e.Message.Text != "/start lista")
            {
                var inlineKeyboard = new InlineKeyboardMarkup(
                    new[]
                    {
                        new [] // first row
                        {
                            InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Vezi cum", "RON PLN"),
                        },
                    });
                Bot.SendTextMessageAsync(
                    e.Message.Chat.Id,
                    "<b>Funcționez doar inline !</b>\n\n",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                    replyMarkup: inlineKeyboard
                    );

            }
        }

        public static async void Bot_OnInlineQueryReceived(object sender, Telegram.Bot.Args.InlineQueryEventArgs t)
        {
            if (t.InlineQuery.Query != "" && t.InlineQuery.Query.Count(x => x == ' ') == 1 && !t.InlineQuery.Query.Any(c => char.IsDigit(c))) {
            var inn = t.InlineQuery.Query.Substring(t.InlineQuery.Query.IndexOf(" ") + 1).ToUpper();
            var din = t.InlineQuery.Query.Substring(0, t.InlineQuery.Query.IndexOf(" ")).ToUpper();
            decimal dind = -1, innd = -1;

            PreclucrareDate Primi = new PreclucrareDate();

            Primi.CheckRates(din);
            var Rate = Primi.Prim;
            dind = Rate;
            //Console.WriteLine(Rate);

                if (din == "EUR") dind = 1;

            Primi.CheckRates(inn);
            Rate = Primi.Prim;
            innd = Rate;
            //Console.WriteLine(Rate);

                var nr = innd / dind;

                if (inn == "EUR") { innd = 1 / dind; nr = innd; }

            if (innd > 0 && dind > 0)
            {

                //Console.WriteLine(din + "-" + inn);

                Telegram.Bot.Types.InlineQueryResults.InlineQueryResultBase[] results = {


                new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultArticle(
                    id: "1",
                    title: "1 " + din + " = " + Math.Round(nr, 2, MidpointRounding.AwayFromZero).ToString() + " " + inn,
                    inputMessageContent: new Telegram.Bot.Types.InlineQueryResults.InputTextMessageContent(
                        "1 " + din + " = " + Math.Round(nr, 2, MidpointRounding.AwayFromZero).ToString() + " " + inn)
                    ),

                };

                await Bot.AnswerInlineQueryAsync(
                    t.InlineQuery.Id,
                    results,
                    isPersonal: true,
                    switchPmText: "VEZI LISTĂ VALUTE",
                    switchPmParameter: "lista",
                    cacheTime: 0);

                din = null;
                inn = null;
            }
        }

            else if (t.InlineQuery.Query != "" && t.InlineQuery.Query.Count(x => x == ' ') == 2)
            {
                var ori = t.InlineQuery.Query.Substring(0, t.InlineQuery.Query.IndexOf(" "));
                var text = t.InlineQuery.Query.Substring(t.InlineQuery.Query.IndexOf(" ") + 1);
                var inn = text.Substring(text.IndexOf(" ") + 1).ToUpper();
                var din = text.Substring(0, text.IndexOf(" ")).ToUpper();
                decimal dind = -1, innd = -1;

                if(ori.Contains("."))ori = ori.Replace(".", ",");

                //Console.WriteLine(ori + "-");

                PreclucrareDate Primi = new PreclucrareDate();

                Primi.CheckRates(din);
                var Rate = Primi.Prim;
                dind = Rate;
                //Console.WriteLine(Rate);

                if (din == "EUR") dind = 1;

                Primi.CheckRates(inn);
                Rate = Primi.Prim;
                innd = Rate;
                //Console.WriteLine(Rate);

                var nr = innd / dind;

                if (inn == "EUR") { innd = 1 / dind; nr = innd; }

                if (innd > 0 && dind > 0)
                {

                    //Console.WriteLine(din + "-" + inn);

                    Telegram.Bot.Types.InlineQueryResults.InlineQueryResultBase[] results = {


                new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultArticle(
                    id: "1",
                    title: ori + " " + din + " = " + Math.Round(nr*Math.Round(decimal.Parse(ori), 2, MidpointRounding.AwayFromZero), 2, MidpointRounding.AwayFromZero).ToString() + " " + inn,
                    inputMessageContent: new Telegram.Bot.Types.InlineQueryResults.InputTextMessageContent(
                        ori + " " + din + " = " + Math.Round(nr*Math.Round(decimal.Parse(ori), 2, MidpointRounding.AwayFromZero), 2, MidpointRounding.AwayFromZero).ToString() + " " + inn)
                    ),

                };

                    await Bot.AnswerInlineQueryAsync(
                        t.InlineQuery.Id,
                        results,
                        isPersonal: true,
                        switchPmText: "VEZI LISTĂ VALUTE",
                        switchPmParameter: "lista",
                        cacheTime: 0);

                    din = null;
                    inn = null;
                }
            }
            
        }
    
    }
}
