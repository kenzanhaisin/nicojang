using Hal.CookieGetterSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace nicojang
{
    public partial class Form1 : Form
    {
        //自分ID
        private string jibunid = "";
        //ユーザ情報
        private string m_userid = "";//ユーザID
        //コメントサーバ情報
        private string m_ComSrvAddr = "";//コメントサーバのアドレス
        private int m_ComSrvPort;//コメントサーバのポート
        private string m_ComSrvThread = "";//スレッドID

        private string m_housounum = "";

        private string m_ticket = "";
        private string m_vpos = "";
        private string m_postkey = "";
        private string m_mail = "";
        private string m_premium = "";
        private string m_locale = "";
        private string m_server_time = "";
        private string m_token = "";
        private int m_post100 = 0;

        private int last_num = 0;
        private int now_num = 0;

        private byte[] resBytes = new byte[1048576];

        private int mMillSec = 500;//スレッドms
        private List<string> comedame = new List<string>();
        private string th_res_from = "1000";
        private string th_ver = "20061206";

        private string m_start_time = "";//開始時刻        

        private delegate void dldl();
        //private Mutex thmt = new Mutex();

        private string stflag = "##属設";
        //private string yobi = "";

        private void comentloop()
        {           
            while (true)
            {
                Invoke(new dldl(delegate
                {
                    mt.WaitOne();
                    //comcom(String.Format(MAI_SEND, m_ComSrvThread, th_ver, th_res_from));
                    mt.ReleaseMutex();
                }));
                Thread.Sleep(mMillSec);  
            }               
        }

        private void comcom(string bun)
        {
            int resSize = 0;
            //Socketの作成
            using (Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                //接続
                sock.Connect(m_ComSrvAddr, m_ComSrvPort);

                //以下、送信処理
                byte[] puchData = Encoding.UTF8.GetBytes(bun);
                sock.Send(puchData, puchData.Length, SocketFlags.None);

                resSize = sock.Receive(resBytes, resBytes.Length, SocketFlags.None);
            }

            if (resSize != 0)
            {
                string[] stArrayData = Encoding.UTF8.GetString(resBytes, 0, resSize).Replace('\0', '\n').Split('\n');
                XmlDocument xmlDoc = new XmlDocument();
                for (int i = 0; i < stArrayData.Length; i++)
                {
                    bool sippai = true;
                    //try
                    //{
                    try
                    {
                        xmlDoc.LoadXml(stArrayData[i]);
                    }
                    catch { sippai = false; }

                    if (sippai)
                    {
                        if (xmlDoc.DocumentElement.Name == "thread")
                        {
                            m_ticket = xmlDoc.DocumentElement.GetAttribute("ticket");
                            m_server_time = xmlDoc.DocumentElement.GetAttribute("server_time");

                            if (new Regex("last_res", RegexOptions.IgnoreCase).Match(stArrayData[i]).Success)
                            {
                                last_num = int.Parse(xmlDoc.DocumentElement.GetAttribute("last_res"));
                                th_res_from = (last_num - now_num + 3).ToString();
                            }

                            //vpos(放送経過時間[sec]*100)を算出
                            //コメントサーバ開始時間
                            //コメント投稿時間(1コメゲッターなのでここでは0secですね)
                            m_vpos = ((Int64.Parse(m_server_time) - Int64.Parse(m_start_time)) * 100).ToString();
                            labelkeikajikan.Text = new TimeSpan(0, 0, (int.Parse(m_server_time) - int.Parse(m_start_time))).ToString();

                            //postkey更新
                            if (m_post100 < now_num / 100)
                            {
                                m_post100 = now_num / 100;
                                req = (HttpWebRequest)WebRequest.Create("http://live.nicovideo.jp/api/getpostkey?thread=" + m_ComSrvThread + "&block_no=" + m_post100.ToString());
                                req.CookieContainer = m_cc;//取得済みのクッキーコンテナ    
                                using (StreamReader sr = new StreamReader(req.GetResponse().GetResponseStream(), Encoding.UTF8))
                                {
                                    m_postkey = Regex.Replace(sr.ReadToEnd(), "postkey=", "");
                                }
                            }

                            if (new Regex("resultcode", RegexOptions.IgnoreCase).Match(stArrayData[i]).Success)
                            {
                                if (xmlDoc.DocumentElement.GetAttribute("resultcode") != "0")
                                {
                                    //textBox1.AppendText(stArrayData[i] + "\r\n");
                                }
                            }
                        }
                        else if (xmlDoc.DocumentElement.Name == "chat")
                        {                            
                            //最新番号取得
                            if (int.Parse(xmlDoc.DocumentElement.GetAttribute("no")) > now_num)
                            {
                                now_num = int.Parse(xmlDoc.DocumentElement.GetAttribute("no"));
                                labelcount.Text = now_num.ToString();

                                if ("##" == new Regex("^..", RegexOptions.Singleline).Match(xmlDoc.DocumentElement.InnerText).Value)
                                {
                                    if ("##参加" == new Regex("^....", RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(xmlDoc.DocumentElement.InnerText).Value)
                                    {//有暗号誰でも
                                        if (choufukuhantei(xmlDoc.DocumentElement.GetAttribute("user_id")))
                                        {
                                            //ユーザページ取得
                                            req = (HttpWebRequest)(WebRequest.Create("http://www.nicovideo.jp/user/" + xmlDoc.DocumentElement.GetAttribute("user_id")));
                                            req.CookieContainer = m_cc;//取得済みのクッキーコンテナ

                                            StreamReader sr = new StreamReader(req.GetResponse().GetResponseStream(), Encoding.UTF8);
                                            string stg = sr.ReadToEnd();
                                            sr.Close();

                                            //ID代入
                                            s_sankasyaid.Add(xmlDoc.DocumentElement.GetAttribute("user_id"));
                                            //画像ID取得
                                            if (new Regex(gazouseiki + @"\d+/\d+\.jpg", RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(stg).Success)
                                            {
                                                s_sankasyagazouid.Add(new Regex(gazouseiki + @"\d+/\d+\.jpg", RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(stg).Value.Replace(gazouseiki, ""));
                                            }
                                            else
                                            {
                                                s_sankasyagazouid.Add("");
                                            }

                                            //名前取得
                                            if (new Regex("Qバージョンに変更", RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(stg).Success)
                                            {
                                                listBox1.Items.Add(new Regex(namaeseiki_h, RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(stg).Value.Replace("<h2><strong>", "").Replace("</strong>さん", ""));
                                            }
                                            else
                                            {
                                                listBox1.Items.Add(new Regex(namaeseiki_q, RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(stg).Value.Replace("<h2>", "").Replace("<small>さん", ""));
                                            }
                                        }
                                    }
                                    else if (stflag == new Regex("^....", RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(xmlDoc.DocumentElement.InnerText).Value && "3" == xmlDoc.DocumentElement.GetAttribute("premium"))
                                    {//無暗号主のみ
                                        string[] sthai = xmlDoc.DocumentElement.InnerText.Split(',');
                                        switch (stflag)
                                        {
                                            case "##属設":
                                                stflag = "##牌設";

                                                if (m_userid != jibunid)
                                                {
                                                    for (int ii = 1; ii <= 20; ii++)
                                                    {
                                                        p_zokusei[ii - 1] = sthai[ii];
                                                    }
                                                }

                                                break;

                                            case "##牌設":
                                                stflag = "##値設";

                                                if (m_userid != jibunid)
                                                {
                                                    for (int jj = 1; jj <= 10; jj++)
                                                    {
                                                        try
                                                        {
                                                            p_charaid[jj - 1] = sthai[jj];

                                                            req = (HttpWebRequest)(WebRequest.Create(idseiki + p_charaid[jj - 1]));
                                                            req.CookieContainer = m_cc;//取得済みのクッキーコンテナ
                                                            StreamReader sr = new StreamReader(req.GetResponse().GetResponseStream(), Encoding.UTF8);
                                                            string stg = sr.ReadToEnd();
                                                            sr.Close();

                                                            //画像取得
                                                            if (new Regex(gazouseiki + @"\d+/\d+\.jpg", RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(stg).Success)
                                                            {
                                                                p_charagazouid[jj - 1] = new Regex(gazouseiki + @"\d+/\d+\.jpg", RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(stg).Value.Replace(gazouseiki, "");
                                                                req = (HttpWebRequest)(WebRequest.Create(gazouseiki + p_charagazouid[jj - 1]));
                                                                req.CookieContainer = m_cc;//取得済みのクッキーコンテナ

                                                                g = Graphics.FromImage(p_charagazou28[jj - 1]);
                                                                g.PixelOffsetMode = PixelOffsetMode.Half;
                                                                g.DrawImage(new Bitmap(req.GetResponse().GetResponseStream()), 0, 0, 28, 28);
                                                            }
                                                            else
                                                            {
                                                                p_charagazou28[jj - 1] = Properties.Resources.noimage28;
                                                            }

                                                            //名前取得
                                                            if (new Regex("Qバージョンに変更", RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(stg).Success)
                                                            {
                                                                p_charaimei[jj - 1] = new Regex(namaeseiki_h, RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(stg).Value.Replace("<h2><strong>", "").Replace("</strong>さん", "");
                                                            }
                                                            else
                                                            {
                                                                p_charaimei[jj - 1] = new Regex(namaeseiki_q, RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(stg).Value.Replace("<h2>", "").Replace("<small>さん", "");
                                                            }
                                                        }
                                                        catch
                                                        {
                                                            p_charagazouid[jj - 1] = "";
                                                            p_charaimei[jj - 1] = "";
                                                            p_charagazou28[jj - 1] = Properties.Resources.noimage28;
                                                        }
                                                    }
                                                }

                                                haidukuri();

                                                break;

                                            case "##値設":
                                                stflag = "##面子";

                                                if (m_userid != jibunid)
                                                {
                                                    int nekazu = 1;
                                                    for (int ii = 0; ii < 20; ii++)
                                                    {
                                                        for (int jj = 0; jj < 9; jj++)
                                                        {
                                                            if (sthai[nekazu] == "1")
                                                            {
                                                                p_setteihyo[ii, jj] = true;
                                                            }
                                                            else if (sthai[nekazu] == "0")
                                                            {
                                                                p_setteihyo[ii, jj] = false;
                                                            }
                                                            nekazu++;
                                                        }
                                                    }

                                                    oto_kettei.Play();
                                                    setuser = true;
                                                }

                                                break;

                                            case "##面子":
                                                stflag = "##サイ";

                                                if (jibunid != m_userid)
                                                {
                                                    jiseki = -1;

                                                    string[] abc = xmlDoc.DocumentElement.InnerText.Split(',');
                                                    for (int jj = 1; jj <= 4; jj++)
                                                    {
                                                        h_sankaid[jj - 1] = abc[jj];

                                                        if (jibunid == h_sankaid[jj - 1])
                                                        {
                                                            jiseki = jj - 1;
                                                            nagame = jj - 1;
                                                        }

                                                        if (h_sankaid[jj - 1] != "-1")
                                                        {
                                                            req = (HttpWebRequest)(WebRequest.Create(idseiki + h_sankaid[jj - 1]));
                                                            req.CookieContainer = m_cc;//取得済みのクッキーコンテナ
                                                            StreamReader sr = new StreamReader(req.GetResponse().GetResponseStream(), Encoding.UTF8);
                                                            string stg = sr.ReadToEnd();
                                                            sr.Close();

                                                            //画像取得
                                                            if (new Regex(gazouseiki + @"\d+/\d+\.jpg", RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(stg).Success)
                                                            {
                                                                req = (HttpWebRequest)(WebRequest.Create(gazouseiki + new Regex(gazouseiki + @"\d+/\d+\.jpg", RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(stg).Value.Replace(gazouseiki, "")));
                                                                req.CookieContainer = m_cc;//取得済みのクッキーコンテナ

                                                                h_sankagazou[jj - 1] = new Bitmap(req.GetResponse().GetResponseStream());
                                                            }
                                                            else
                                                            {
                                                                h_sankagazou[jj - 1] = Properties.Resources.noimage28;
                                                            }

                                                            //名前取得
                                                            if (new Regex("Qバージョンに変更", RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(stg).Success)
                                                            {
                                                                h_sankamei[jj - 1] = new Regex(namaeseiki_h, RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(stg).Value.Replace("<h2><strong>", "").Replace("</strong>さん", "");
                                                            }
                                                            else
                                                            {
                                                                h_sankamei[jj - 1] = new Regex(namaeseiki_q, RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(stg).Value.Replace("<h2>", "").Replace("<small>さん", "");
                                                            }
                                                        }
                                                        else
                                                        {
                                                            h_sankagazou[jj - 1] = Properties.Resources.noimage28;
                                                            switch (jj-1)
                                                            {
                                                                case 0:
                                                                h_sankamei[jj - 1] = "人工無能東";
                                                                break;

                                                                case 1:
                                                                h_sankamei[jj - 1] = "人工無能北";
                                                                break;

                                                                case 2:
                                                                h_sankamei[jj - 1] = "人工無能西";
                                                                break;

                                                                case 3:
                                                                h_sankamei[jj - 1] = "人工無能南";
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                                break;

                                            case "##サイ":
                                                stflag = "##配牌";

                                                if (jibunid != m_userid)
                                                {
                                                    sai1 = int.Parse(xmlDoc.DocumentElement.InnerText.Split(',')[1]);
                                                    sai2 = int.Parse(xmlDoc.DocumentElement.InnerText.Split(',')[2]);
                                                    oya = (sai1 + sai2) % 4;
                                                    mawariban = oya;
                                                    firstoya = oya;
                                                    hougakuire();

                                                    stage = "親決";
                                                    Thread th = new Thread(saimati);
                                                    th.IsBackground = true;
                                                    th.Start();
                                                }
                                                break;

                                            case "##配牌":
                                                if (jibunid != m_userid)
                                                {
                                                    string[] ab3 = xmlDoc.DocumentElement.InnerText.Split(',');
                                                    for (int ii = 1; ii <= 82; ii++)
                                                    {
                                                        haijun[ii - 1] = int.Parse(ab3[ii]);
                                                    }
                                                }

                                                for (int ii = 0; ii < 8; ii++)
                                                {
                                                    tehai[0][ii] = haijun[ii * 4];
                                                    tehai[1][ii] = haijun[ii * 4 + 1];
                                                    tehai[2][ii] = haijun[ii * 4 + 2];
                                                    tehai[3][ii] = haijun[ii * 4 + 3];
                                                }

                                                tehai[0][8] = 100;
                                                tehai[1][8] = 100;
                                                tehai[2][8] = 100;
                                                tehai[3][8] = 100;

                                                Array.Sort(tehai[0]);
                                                Array.Sort(tehai[1]);
                                                Array.Sort(tehai[2]);
                                                Array.Sort(tehai[3]);

                                                for (int x = 0; x < 14; x++)
                                                {
                                                    sutehai[0][x] = 100;
                                                    sutehai[1][x] = 100;
                                                    sutehai[2][x] = 100;
                                                    sutehai[3][x] = 100;
                                                }

                                                sutemax = new int[4] { 0, 0, 0, 0 };

                                                for (int x = 0; x < 4; x++)
                                                {
                                                    for (int y = 0; y < 2; y++)
                                                    {
                                                        for (int z = 0; z < 3; z++)
                                                        {
                                                            pon[x][y, z] = 100;
                                                        }
                                                    }
                                                }

                                                pondarekara = new int[][] { new int[2] { -1, -1 }, new int[2] { -1, -1 }, new int[2] { -1, -1 }, new int[2] { -1, -1 } };
                                                ponflag = new bool[4] { false, false, false, false };
                                                ponjusinflag = new bool[4] { false, false, false, false };
                                                ponisi = new bool[4] { false, false, false, false };
                                                ponken = new bool[4] { false, false, false, false };

                                                reachflag = new bool[4] { false, false, false, false };
                                                doblereahantei = new bool[4] { true, true, true, true };
                                                doblerea = new bool[4] { false, false, false, false };
                                                reaippatu = new bool[4] { false, false, false, false };
                                                agariflag = new bool[4] { false, false, false, false };
                                                hoflag = new bool[4] { true, true, true, true };

                                                genzaipaime = 32;
                                                mawariban = oya;
                                                tehai[oya][8] = haijun[32];
                                                agarihantei(mawariban);

                                                stage = "対局";

                                                if (agariflag[mawariban])
                                                {
                                                    oto_tumo.Play();
                                                    Thread th = new Thread(tokuhatu);
                                                    th.IsBackground = true;
                                                    th.Start();
                                                }
                                                else
                                                {
                                                    if (mawariban == jiseki)
                                                    {
                                                        oto_torihai.Play();
                                                    }

                                                    if (reachflag[mawariban])
                                                    {
                                                        haisutesyori(8);
                                                    }
                                                    else
                                                    {
                                                        //人工無能
                                                        if (h_sankaid[mawariban] == "-1")
                                                        {
                                                            haisutesyori(8);
                                                        }

                                                        if (mawariban != jiseki)
                                                        {
                                                            //dataGridView1.Rows.Add("<chat thread=\"1235863729\" no=\"" + (dataGridView1.Rows.Count + 1).ToString() + "\" vpos=\"2564\" date=\"1357743067\" date_usec=\"698579\" user_id=\"" + h_sankaid[mawariban].ToString() + "\" premium=\"1\" locale=\"ja-jp\">" + angouka("捨牌,0," + mawariban.ToString() + "," + Environment.TickCount.ToString()) + "</chat>");
                                                        }
                                                    }
                                                }

                                                break;
                                            
                                        }
                                    }
                                    else
                                    {//有暗号他
                                        switch (new Regex("^....", RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(xmlDoc.DocumentElement.InnerText).Value)
                                        {
                                            case "##捨牌":
                                                if (jibunid != xmlDoc.DocumentElement.GetAttribute("user_id") && h_sankaid[int.Parse(xmlDoc.DocumentElement.InnerText.Split(',')[2])] == xmlDoc.DocumentElement.GetAttribute("user_id"))
                                                {
                                                    haisutesyori(int.Parse(xmlDoc.DocumentElement.InnerText.Split(',')[1]));
                                                }
                                                break;

                                            case "##ポン":
                                                if (h_sankaid[int.Parse(xmlDoc.DocumentElement.InnerText.Split(',')[1])] == xmlDoc.DocumentElement.GetAttribute("user_id"))
                                                {
                                                    ponisi[int.Parse(xmlDoc.DocumentElement.InnerText.Split(',')[1])] = true;
                                                    ponjusinflag[int.Parse(xmlDoc.DocumentElement.InnerText.Split(',')[1])] = false;

                                                    ponuketuke();
                                                }
                                                break;

                                            case "##ノン":
                                                if (h_sankaid[int.Parse(xmlDoc.DocumentElement.InnerText.Split(',')[1])] == xmlDoc.DocumentElement.GetAttribute("user_id"))
                                                {
                                                    ponisi[int.Parse(xmlDoc.DocumentElement.InnerText.Split(',')[1])] = false;
                                                    ponjusinflag[int.Parse(xmlDoc.DocumentElement.InnerText.Split(',')[1])] = false;

                                                    ponuketuke();
                                                }

                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

        private void syucom(string s)
        {
            //req = (HttpWebRequest)WebRequest.Create("http://live.nicovideo.jp/api/getplayerstatus?v=" + m_housounum);

            req = (HttpWebRequest)WebRequest.Create("http://live.nicovideo.jp/api/getpublishstatus?v=" + m_housounum);
            req.CookieContainer = m_cc;//取得済みのクッキーコンテナ
            //番組情報
            XmlDocument xdoc = new XmlDocument();
            using (StreamReader sr = new StreamReader(req.GetResponse().GetResponseStream(), Encoding.UTF8))
            {
                xdoc.LoadXml(sr.ReadToEnd());
            }
            XmlNodeList stream = xdoc.DocumentElement.GetElementsByTagName("stream");
            
            foreach (XmlNode node in stream[0].ChildNodes)
            {
                if (node.Name == "broadcast_token")
                {
                    m_token = node.InnerText;
                    //textBox1.AppendText("トークン:" + m_token + "\r\n");
                }
            }

            //string status = GetHtml_Use_user_session(
            //     new Uri("http://live.nicovideo.jp/api/getplayerstatus?v=" + UserStatus.LiveID), Is.user_session);
            req = (HttpWebRequest)(WebRequest.Create("http://live.nicovideo.jp/api/broadcast/" + m_housounum + "?body=" + Uri.EscapeUriString(s) + "&mail=184" + "&token=" + m_token));
            //req.Method = "POST";
            //req.ContentType = "application/x-www-form-urlencoded";
            req.CookieContainer = m_cc;//取得済みのクッキーコンテ
            req.GetResponse().GetResponseStream();
        }

        //CspParametersオブジェクトの作成
        //private CspParameters cp = new CspParameters();
        //CspParametersを指定してRSACryptoServiceProviderオブジェクトを作成
        /*
        private RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        //private string kagi = "n!c0jAn";
        private string angouka(string bun)
        {
            rsa.FromXmlString(Properties.Settings.Default.publickye);
            return Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(bun), false));
        }
        private string fukugouka(string bun)
        {
            rsa.FromXmlString(Properties.Settings.Default.privateKey);
            try
            {
                return Encoding.UTF8.GetString(rsa.Decrypt(Convert.FromBase64String(bun), false));
            }
            catch
            {
                return "";
            }
            
        }
        */

        //Unixタイムに変換
        private Int64 GetUnixTime(DateTime targetTime)
        {
            return (Int64)((targetTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
        }

        private bool choufukuhantei(string na)
        {
            for (int i = 0; i < s_sankasyaid.Count; i++)
            {
                if (s_sankasyaid[i] == na)
                {
                    return false;
                }
            }

            return true;
        }

        private void hougakuire()
        {
            migi = hougaku[nagame, 0];
            ue = hougaku[nagame, 1];
            hidari = hougaku[nagame, 2];
            sita = hougaku[nagame, 3];
        }

        private void saimati()
        {
            Thread.Sleep(1000);
            saiketu = true;
        }

        private void ponuketuke()
        {            
            if (!ponjusinflag[0] && !ponjusinflag[1] && !ponjusinflag[2] && !ponjusinflag[3])
            {
                if (!ponisi[0] && !ponisi[1] && !ponisi[2] && !ponisi[3])
                {
                    ponflag = new bool[4] { false, false, false, false };
                    //ponisi = new bool[4] { false, false, false, false }; 

                    genzaipaime++;
                    if (genzaipaime != 82)
                    {                        
                        mawariban++;
                        if (mawariban == 4)
                        {
                            mawariban = 0;
                        }

                        tehai[mawariban][8] = haijun[genzaipaime];
                        agarihantei(mawariban);
                        if (agariflag[mawariban])
                        {
                            oto_tumo.Play();
                            Thread th = new Thread(tokuhatu);
                            th.IsBackground = true;
                            th.Start();                           
                        }
                        else if (mawariban == jiseki || jiseki == -1)
                        {
                            oto_torihai.Play();
                        }
                        else
                        {
                            //dataGridView1.Rows.Add("<chat thread=\"1235863729\" no=\"" + (dataGridView1.Rows.Count + 1).ToString() + "\" vpos=\"2564\" date=\"1357743067\" date_usec=\"698579\" user_id=\"" + h_sankaid[mawariban].ToString() + "\" premium=\"1\" locale=\"ja-jp\">" + angouka("捨牌,0," + mawariban.ToString() + "," + Environment.TickCount.ToString()) + "</chat>");
                        }
                    }
                    else
                    {
                        if (jibunid == m_userid)
                        {
                            Thread th = new Thread(kyokureset);
                            th.IsBackground = true;
                            th.Start();
                        }
                    }
                }
                else
                {
                    hoflag = new bool[4] { false, false, false, false };
                    doblereahantei = new bool[4] { false, false, false, false };
                    reaippatu = new bool[4] { false, false, false, false };
                    
                    int kaisyu;
                    for (int ii = 0; ii < 3; ii++)
                    {
                        kaisyu = mawariban + 1 + ii;
                        if (kaisyu > 3)
                        {
                            kaisyu -= 4;
                        }

                        if (ponisi[kaisyu])
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                if (tehai[kaisyu][j] / 10 == sutehai[mawariban][sutemax[mawariban] - 1] / 10)
                                {
                                    if (tehai[kaisyu][j + 1] / 10 == sutehai[mawariban][sutemax[mawariban] - 1] / 10)
                                    {
                                        if (pondarekara[kaisyu][0] == -1)
                                        {
                                            if (mawariban - kaisyu == -1 || mawariban - kaisyu == 3)
                                            {
                                                pon[kaisyu][0, 0] = tehai[kaisyu][j];
                                                pon[kaisyu][0, 1] = tehai[kaisyu][j + 1];
                                                pon[kaisyu][0, 2] = sutehai[mawariban][sutemax[mawariban] - 1];
                                            }
                                            else if (mawariban - kaisyu == -2 || mawariban - kaisyu == 2)
                                            {
                                                pon[kaisyu][0, 0] = tehai[kaisyu][j];
                                                pon[kaisyu][0, 1] = sutehai[mawariban][sutemax[mawariban] - 1];
                                                pon[kaisyu][0, 2] = tehai[kaisyu][j + 1];
                                            }
                                            else if (mawariban - kaisyu == 1 || mawariban - kaisyu == -3)
                                            {
                                                pon[kaisyu][0, 0] = sutehai[mawariban][sutemax[mawariban] - 1];
                                                pon[kaisyu][0, 1] = tehai[kaisyu][j];
                                                pon[kaisyu][0, 2] = tehai[kaisyu][j + 1];
                                            }                                           
                                            
                                            pondarekara[kaisyu][0] = mawariban;
                                        }
                                        else if (pondarekara[kaisyu][1] == -1)
                                        {
                                            if (mawariban - kaisyu == -1 || mawariban - kaisyu == 3)
                                            {
                                                pon[kaisyu][1, 0] = tehai[kaisyu][j];
                                                pon[kaisyu][1, 1] = tehai[kaisyu][j + 1];
                                                pon[kaisyu][1, 2] = sutehai[mawariban][sutemax[mawariban] - 1];
                                            }
                                            else if (mawariban - kaisyu == -2 || mawariban - kaisyu == 2)
                                            {
                                                pon[kaisyu][1, 0] = tehai[kaisyu][j];
                                                pon[kaisyu][1, 1] = sutehai[mawariban][sutemax[mawariban] - 1];
                                                pon[kaisyu][1, 2] = tehai[kaisyu][j + 1];
                                            }
                                            else if (mawariban - kaisyu == 1 || mawariban - kaisyu == -3)
                                            {
                                                pon[kaisyu][1, 0] = sutehai[mawariban][sutemax[mawariban] - 1];
                                                pon[kaisyu][1, 1] = tehai[kaisyu][j];
                                                pon[kaisyu][1, 2] = tehai[kaisyu][j + 1];
                                            }

                                            pondarekara[kaisyu][1] = mawariban;
                                        }

                                        tehai[kaisyu][j] = 100;
                                        tehai[kaisyu][j + 1] = 100;
                                    }
                                    else
                                    {
                                        int alban = -1;
                                        for (int i = 0; i < 8; i++)
                                        {
                                            if (tehai[kaisyu][i] == 90)
                                            {
                                                alban = i;
                                                break;
                                            }

                                        }

                                        if (pondarekara[kaisyu][0] == -1)
                                        {
                                            if (mawariban - kaisyu == -1 || mawariban - kaisyu == 3)
                                            {
                                                pon[kaisyu][0, 0] = tehai[kaisyu][j];
                                                pon[kaisyu][0, 1] = tehai[kaisyu][alban];
                                                pon[kaisyu][0, 2] = sutehai[mawariban][sutemax[mawariban] - 1];
                                            }
                                            else if (mawariban - kaisyu == -2 || mawariban - kaisyu == 2)
                                            {
                                                pon[kaisyu][0, 0] = tehai[kaisyu][j];
                                                pon[kaisyu][0, 1] = sutehai[mawariban][sutemax[mawariban] - 1];
                                                pon[kaisyu][0, 2] = tehai[kaisyu][alban];
                                            }
                                            else if (mawariban - kaisyu == 1 || mawariban - kaisyu == -3)
                                            {
                                                pon[kaisyu][0, 0] = sutehai[mawariban][sutemax[mawariban] - 1];
                                                pon[kaisyu][0, 1] = tehai[kaisyu][j];
                                                pon[kaisyu][0, 2] = tehai[kaisyu][alban];
                                            }
                                            
                                            pondarekara[kaisyu][0] = mawariban;
                                        }
                                        else if (pondarekara[kaisyu][1] == -1)
                                        {
                                            if (mawariban - kaisyu == -1 || mawariban - kaisyu == 3)
                                            {
                                                pon[kaisyu][1, 0] = tehai[kaisyu][j];
                                                pon[kaisyu][1, 1] = tehai[kaisyu][alban];
                                                pon[kaisyu][1, 2] = sutehai[mawariban][sutemax[mawariban] - 1];
                                            }
                                            else if (mawariban - kaisyu == -2 || mawariban - kaisyu == 2)
                                            {
                                                pon[kaisyu][1, 0] = tehai[kaisyu][j];
                                                pon[kaisyu][1, 1] = sutehai[mawariban][sutemax[mawariban] - 1];
                                                pon[kaisyu][1, 2] = tehai[kaisyu][alban];
                                            }
                                            else if (mawariban - kaisyu == 1 || mawariban - kaisyu == -3)
                                            {
                                                pon[kaisyu][1, 0] = sutehai[mawariban][sutemax[mawariban] - 1];
                                                pon[kaisyu][1, 1] = tehai[kaisyu][j];
                                                pon[kaisyu][1, 2] = tehai[kaisyu][alban];
                                            }

                                            pondarekara[kaisyu][1] = mawariban;
                                        }

                                        tehai[kaisyu][j] = 100;
                                        tehai[kaisyu][alban] = 100;                                        
                                    }

                                    ponflag = new bool[4] { false, false, false, false };
                                    ponisi = new bool[4] { false, false, false, false }; 

                                    Array.Sort(tehai[kaisyu]);
                                    sutehai[mawariban][sutemax[mawariban] - 1] = 100;
                                    sutemax[mawariban]--;
                                    
                                    oto_pon.Play();
                                    ponken[kaisyu] = true;
                                    mawariban = kaisyu;
                                   
                                    if (jiseki != mawariban)
                                    {
                                        //dataGridView1.Rows.Add("<chat thread=\"1235863729\" no=\"" + (dataGridView1.Rows.Count + 1).ToString() + "\" vpos=\"2564\" date=\"1357743067\" date_usec=\"698579\" user_id=\"" + h_sankaid[mawariban].ToString() + "\" premium=\"1\" locale=\"ja-jp\">" + angouka("捨牌,0," + mawariban.ToString() + "," + Environment.TickCount.ToString()) + "</chat>");
                                    }
                                    break;
                                }
                            }
                        }
                    }                    
                }                  
            }
        }

    }
}