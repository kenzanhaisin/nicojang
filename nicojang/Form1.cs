using Hal.CookieGetterSharp;
using nicojang.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Xml;

namespace nicojang
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //フォント設定
            sftate.FormatFlags = StringFormatFlags.DirectionVertical;
            sftate.Alignment = StringAlignment.Center;
            sftate.LineAlignment = StringAlignment.Center;

            sftateue.FormatFlags = StringFormatFlags.DirectionVertical;
            sftateue.Alignment = StringAlignment.Near;
            sftateue.LineAlignment = StringAlignment.Center;

            sftatesita.FormatFlags = StringFormatFlags.DirectionVertical;
            sftatesita.Alignment = StringAlignment.Far;
            sftatesita.LineAlignment = StringAlignment.Center;

            sfnaka.Alignment = StringAlignment.Center;
            sfnaka.LineAlignment = StringAlignment.Center;

            sfnakaue.Alignment = StringAlignment.Center;

            sfnakasita.Alignment = StringAlignment.Center;
            sfnakasita.LineAlignment = StringAlignment.Far;

            sfnakahidari.Alignment = StringAlignment.Near;
            sfnakahidari.LineAlignment = StringAlignment.Center;

            sfnakamigi.Alignment = StringAlignment.Far;
            sfnakamigi.LineAlignment = StringAlignment.Center;

            openFileDialog1.InitialDirectory = Properties.Settings.Default.zensetteipath;
            saveFileDialog1.InitialDirectory = Properties.Settings.Default.zensetteipath;

            //重軽
            trackBar1.Value = Properties.Settings.Default.omokaru;

            //184
            /*m_mail = Properties.Settings.Default.h184;
            if (m_mail == "184")
            {
                checkBox184.Checked = true;
            }
            else
            {
                checkBox184.Checked = false;
            }*/

            //バージョン取得
            using (StreamReader sr = new StreamReader(new WebClient().OpenRead("http://space.geocities.jp/kenzanhaisin/nicojangversion.txt"), Encoding.GetEncoding("UTF-8")))
            {
                version = sr.ReadToEnd();
            }

            if (version == FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.ToString())
            {
                comboBox1.Enabled = true;
                comboBox1.Items.AddRange(CookieGetter.CreateInstances(true));                               
            }

            s_sankasyaid.Add("-1"); s_sankasyagazouid.Add("");
        }

        private int cheban = 0;

        private string version;

        private string stage = "初期";
        private Mutex mt = new Mutex();
        private Bitmap[] sai = new Bitmap[6] { Resources.saikoro1, Resources.saikoro2, Resources.saikoro3, Resources.saikoro4, Resources.saikoro5, Resources.saikoro6 };

        private StringFormat sftate = new StringFormat();
        private StringFormat sftateue = new StringFormat();
        private StringFormat sftatesita = new StringFormat();
        private StringFormat sfnaka = new StringFormat();
        private StringFormat sfnakaue = new StringFormat();
        private StringFormat sfnakasita = new StringFormat();
        private StringFormat sfnakahidari = new StringFormat();
        private StringFormat sfnakamigi = new StringFormat();

        //private CookieCollection collection;
        private CookieContainer m_cc;//クッキーコンテナ
        private HttpWebRequest req;
        private Cookie ck;

        private string URL_REGEX = "lv[0-9]+";//^http://live.nicovideo.jp/watch/
        private string MAI_SEND = "<thread thread=\"{0}\" version=\"{1}\" res_from=\"-{2}\"/>\0";
        private string COM_SEND = "<chat thread=\"{0}\" ticket=\"{1}\" vpos=\"{2}\" postkey=\"{3}\" mail=\"{4}\" user_id=\"{5}\" premium=\"{6}\">{7}</chat>\0";
        private string COM_SEND0 = "<chat thread=\"{0}\" ticket=\"{1}\" vpos=\"{2}\" postkey=\"{3}\" mail=\"{4}\" user_id=\"{5}\" premium=\"{6}\">{7}</chat>\0";

        private Rectangle syokie = new Rectangle(0, 400, 600, 200);
        private Rectangle zokuyomi = new Rectangle(260, 560, 100, 30);
        private Rectangle zokukaki = new Rectangle(380, 560, 100, 30);
        private Rectangle zokukaku = new Rectangle(500, 560, 50, 30);
        private Rectangle ponreacherea = new Rectangle(420, 528, 38, 15);
        private Rectangle nonerea = new Rectangle(420, 508, 38, 15);
        private Rectangle miginameerea = new Rectangle(403, 197, 30, 206);
        private Rectangle uenamearea = new Rectangle(197, 167, 206, 30);
        private Rectangle hidarinamearea = new Rectangle(167, 197, 30, 206);
        private Rectangle sitanameerea = new Rectangle(197, 403, 206, 30);

        private System.Drawing.Brush[] namanusicolor = new System.Drawing.Brush[10] { System.Drawing.Brushes.Red, System.Drawing.Brushes.Orange, System.Drawing.Brushes.Green, System.Drawing.Brushes.LightBlue, System.Drawing.Brushes.Blue, System.Drawing.Brushes.Purple, System.Drawing.Brushes.Pink, System.Drawing.Brushes.Brown, System.Drawing.Brushes.Gray, System.Drawing.Brushes.Black };

        private bool setuser = false;

        private string[] p_zokusei = new string[20] { "雑談", "セレブ", "自宅警備員", "イケボ", "ギャンブル", "出会い厨", "アウトロー", "音楽", "ゲーム", "ものまね", "FX", "露出", "喧嘩", "", "", "", "", "", "", "" };
        private string[] p_charaid = new string[10];
        private string[] p_charaimei = new string[10];
        private string[] p_charagazouid = new string[10] { "", "", "", "", "", "", "", "", "", "" };
        private Bitmap[] p_charagazou28 = new Bitmap[10] { Properties.Resources.noimage28, Properties.Resources.noimage28, Properties.Resources.noimage28, Properties.Resources.noimage28, Properties.Resources.noimage28, Properties.Resources.noimage28, Properties.Resources.noimage28, Properties.Resources.noimage28, Properties.Resources.noimage28, Properties.Resources.noimage28 };
        private bool[,] p_setteihyo = new bool[20, 9];

        private int sentakuzoku = -1;
        private int sentakuchar = -1;

        private List<string> s_sankasyaid = new List<string>();
        private List<string> s_sankasyagazouid = new List<string>();

        string[] h_sankaid = new string[4] { "", "", "", "" };
        string[] h_sankamei = new string[4] { "", "", "", "" };
        Bitmap[] h_sankagazou = new Bitmap[4] { Properties.Resources.noimage, Properties.Resources.noimage, Properties.Resources.noimage, Properties.Resources.noimage };
        int[] h_sankaten = new int[4] { 25000, 25000, 25000, 25000 };

        private int sai1 = 0;
        private int sai2 = 0;
        private int firstoya = 0;
        private int oya = 0;
        private bool saiketu = false;
        //private string matibyo = "9";

        private Bitmap[,] hai = new Bitmap[9, 9];
        private Bitmap hai_all = new Bitmap(38, 52);
        private int[] haijun = new int[82];

        private int[,] hougaku = new int[,] { { 1, 2, 3, 0 }, { 2, 3, 0, 1 }, { 3, 0, 1, 2 }, { 0, 1, 2, 3 } };
        private int migi = 1;
        private int ue = 2;
        private int hidari = 3;
        private int sita = 0;

        private int jiseki = -1;//-1
        private int nagame = 0;
        private int genzaipaime = 0;
        private int mawariban = 0;//0

        private int[][] tehai = new int[][] { new int[9], new int[9], new int[9], new int[9] };

        private int[][] sutehai = new int[][] { new int[14], new int[14], new int[14], new int[14] };
        private int[] sutemax = new int[4];

        private int[][,] pon = new int[][,] { new int[2, 3], new int[2, 3], new int[2, 3], new int[2, 3] };//[席][１，２番目,値3]
        private int[][] pondarekara = new int[][] { new int[2] { -1, -1 }, new int[2] { -1, -1 }, new int[2] { -1, -1 }, new int[2] { -1, -1 } };//[席][１，２番目]        
        private bool[] ponflag = new bool[4] { false, false, false, false };
        private bool[] ponjusinflag = new bool[4] { false, false, false, false };
        private bool[] ponisi = new bool[4] { false, false, false, false };
        private bool[] ponken = new bool[4] { false, false, false, false };

        private bool[] reachflag = new bool[4] { false, false, false, false };
        private bool[] doblereahantei = new bool[4] { true, true, true, true };
        private bool[] doblerea = new bool[4] { false, false, false, false };
        private bool[] reaippatu = new bool[4] { false, false, false, false };
        private bool[] agariflag = new bool[4] { false, false, false, false };

        private bool[] hoflag = new bool[4] { true, true, true, true };

        private List<string[,]> y_yaku = new List<string[,]>();

        private double[] tokuten = new double[4] { 250, 250, 250, 250 };
        private double[] henten = new double[4] { 0, 0, 0, 0 };

        private int tokuflag = 0;

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            Properties.Settings.Default.omokaru = trackBar1.Value;
            Properties.Settings.Default.Save();

            timer1.Interval = 110 - Properties.Settings.Default.omokaru;
        }

        private SoundPlayer oto_hajime = new SoundPlayer(Properties.Resources.niconicojunjun);
        private SoundPlayer oto_zokuerabi = new SoundPlayer(Properties.Resources.seibetu);
        private SoundPlayer oto_dame = new SoundPlayer(Properties.Resources.timeover);
        private SoundPlayer oto_kettei = new SoundPlayer(Properties.Resources.chatjusin);
        private SoundPlayer oto_saion = new SoundPlayer(Properties.Resources.saion);
        private SoundPlayer oto_torihai = new SoundPlayer(Properties.Resources.nc1277);
        private SoundPlayer oto_ron = new SoundPlayer(Properties.Resources.ron);
        private SoundPlayer oto_ponsuruno = new SoundPlayer(Properties.Resources.ponsuruno);
        private SoundPlayer oto_pon = new SoundPlayer(Properties.Resources.pon);
        private SoundPlayer oto_reach = new SoundPlayer(Properties.Resources.reach);
        private SoundPlayer oto_tumo = new SoundPlayer(Properties.Resources.tumo);
        private SoundPlayer oto_binta = new SoundPlayer(Properties.Resources.binta);
        private SoundPlayer oto_naguri = new SoundPlayer(Properties.Resources.naguri);
        private SoundPlayer oto_keikoku = new SoundPlayer(Properties.Resources.keikoku);

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            mt.WaitOne();

            timer1.Stop();

            mt.ReleaseMutex();
        }

        private void burauzackget()
        {
            mt.WaitOne();

            if (comboBox1.SelectedIndex > -1)
            {
                ck = CookieGetter.CreateInstances(true)[comboBox1.SelectedIndex].GetCookie(new Uri("http://www.nicovideo.jp/"), "user_session");

                if (ck == null)
                {
                    textBoxurl.Enabled = false;
                    MessageBox.Show("クッキーを取得できませんでした。\r\nブラウザでログインし直すか、ブラウザを再起動したり\r\n時間を置くなどして再度お試し下さい。");
                }
                else
                {
                    m_cc = new CookieContainer();
                    m_cc.Add(ck);
                    textBoxurl.Enabled = true;
                }
            }

            mt.ReleaseMutex();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.burauza > -1 && Properties.Settings.Default.burauza < comboBox1.Items.Count)
            {
                comboBox1.SelectedIndex = Properties.Settings.Default.burauza;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.burauza = comboBox1.SelectedIndex;
            Properties.Settings.Default.Save();
            if (comboBox1.SelectedIndex > -1)
            {
                burauzackget();
            }
        }

        private string idseiki = "http://www.nicovideo.jp/user/";
        private string gazouseiki = "http://usericon.nimg.jp/usericon/";
        private string namaeseiki_q = "<h2>.+<small>さん";
        private string namaeseiki_h = "<h2><strong>.+</strong>さん";
        private bool syutokusippai = true;

        private void textBoxurl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;

                mt.WaitOne();

                Match mc = new Regex(URL_REGEX, RegexOptions.IgnoreCase).Match(textBoxurl.Text);

                if (mc.Success)
                {
                    comboBox1.Enabled = false;
                    textBoxurl.Enabled = false;
                    //checkBox184.Enabled = false;

                    m_housounum = mc.Value;

                    try
                    {
                        req = (HttpWebRequest)WebRequest.Create("http://live.nicovideo.jp/api/getplayerstatus?v=" + mc.Value);
                        //req = (HttpWebRequest)WebRequest.Create("http://live.nicovideo.jp/api/getpublishstatus?v=" + mc.Value);
                        req.CookieContainer = m_cc;//取得済みのクッキーコンテナ
                        //XML解析
                        XmlDocument xdoc = new XmlDocument();
                        using (StreamReader sr = new StreamReader(req.GetResponse().GetResponseStream(), Encoding.UTF8))
                        {
                            xdoc.LoadXml(sr.ReadToEnd());
                        }

                        //番組情報
                        XmlNodeList stream = xdoc.DocumentElement.GetElementsByTagName("stream");

                        foreach (XmlNode node in stream[0].ChildNodes)
                        {
                            if (node.Name == "owner_id")
                            {
                                m_userid = node.InnerText;
                                //textBox1.AppendText("オーナーID:" + m_userid + "\r\n");
                            }
                            if (node.Name == "start_time")
                            {
                                m_start_time = node.InnerText;
                                //textBox1.AppendText("スタートタイム:" + m_start_time + "\r\n");
                            }
                            if (node.Name == "broadcast_token")
                            {
                                m_token = node.InnerText;
                                //textBox1.AppendText("トークン:" + m_token + "\r\n");
                            }
                        }

                        foreach (XmlNode node in stream[0].ChildNodes)
                        {
                            if (node.Name == "owner_id")
                            {
                                m_userid = node.InnerText;
                                //textBox1.AppendText("オーナーID:" + m_userid + "\r\n");
                            }
                            if (node.Name == "start_time")
                            {
                                m_start_time = node.InnerText;
                                //textBox1.AppendText("スタートタイム:" + m_start_time + "\r\n");
                            }
                            if (node.Name == "token")
                            {
                                m_token = node.InnerText;
                                //textBox1.AppendText("トークン:" + m_token + "\r\n");
                            }
                        }
                        //m_userid = "24704235";

                        //user情報
                        XmlNodeList user = xdoc.DocumentElement.GetElementsByTagName("user");

                        foreach (XmlNode node in user[0].ChildNodes)
                        {                            
                            if (node.Name == "userLanguage")
                            {
                                m_locale = node.InnerText;
                                //textBox1.AppendText("言語:" + m_locale + "\r\n");
                            }
                            if (node.Name == "user_id")
                            {
                                jibunid = node.InnerText;
                                //textBox1.AppendText("ユーザID:" + jibunid + "\r\n");
                            }                            
                        }
                        //jibunid = "24704235";

                        foreach (XmlNode node in user[0].ChildNodes)
                        {
                            if (node.Name == "is_premium")
                            {
                                if (m_userid == jibunid)
                                {
                                    m_premium = "3";
                                    m_mail = "";
                                    //textBox1.AppendText("放送主確認\r\n");
                                }
                                else if (node.InnerText == "1")
                                {
                                    m_premium = "1";
                                    //textBox1.AppendText("プレミアム会員確認\r\n");
                                }
                                else if (node.InnerText == "0")
                                {
                                    m_premium = "0";
                                    //textBox1.AppendText("一般会員確認\r\n");
                                }

                                break;
                            }
                        }
                        //m_premium = "3";

                        //サーバ情報
                        XmlNodeList ms = xdoc.DocumentElement.GetElementsByTagName("ms");
                        //余談だが、ms.Countは常に0と想定し、ms[0]のみ読む。
                        m_ComSrvAddr = ms[0].ChildNodes[0].InnerText;
                        //textBox1.AppendText("サーバアドレス:" + m_ComSrvAddr + "\r\n");
                        m_ComSrvPort = int.Parse(ms[0].ChildNodes[1].InnerText);
                        //textBox1.AppendText("サーバポート:" + m_ComSrvPort + "\r\n");
                        m_ComSrvThread = ms[0].ChildNodes[2].InnerText;
                        //textBox1.AppendText("スレッド:" + m_ComSrvThread + "\r\n");

                        req = (HttpWebRequest)WebRequest.Create("http://live.nicovideo.jp/api/getpostkey?thread=" + m_ComSrvThread + "&block_no=0");
                        req.CookieContainer = m_cc;//取得済みのクッキーコンテナ    
                        using (StreamReader sr = new StreamReader(req.GetResponse().GetResponseStream(), Encoding.UTF8))
                        {
                            m_postkey = Regex.Replace(sr.ReadToEnd(), "postkey=", "");
                            //textBox1.AppendText("ポストキー:" + m_postkey + "\r\n");
                        }
                        
                        if (jibunid == m_userid && Properties.Settings.Default.zensettei != "にこ")
                        {
                            //前回設定復元
                            using (StringReader srr = new StringReader(Properties.Settings.Default.zensettei))
                            {
                                string line = "";
                                int kazoe = 0;
                                while ((line = srr.ReadLine()) != null)
                                {
                                    if (kazoe < 20)
                                    {
                                        p_zokusei[kazoe] = line;
                                    }
                                    else if (kazoe < 30)
                                    {
                                        p_charaid[kazoe - 20] = line;
                                    }
                                    else if (kazoe < 40)
                                    {
                                        p_charaimei[kazoe - 30] = line;
                                    }
                                    else if (kazoe < 50)
                                    {
                                        p_charagazouid[kazoe - 40] = line;
                                    }
                                    else if (kazoe < 59)
                                    {
                                        string[] stArrayData = line.Split(',');
                                        for (int i = 0; i < 20; i++)
                                        {
                                            if (stArrayData[i] == "1")
                                            {
                                                p_setteihyo[kazoe - 50, i] = true;
                                            }
                                            else
                                            {
                                                p_setteihyo[kazoe - 50, i] = false;
                                            }
                                        }
                                    }

                                    kazoe++;
                                }
                            }

                            //画像取得
                            for (int j = 0; j < 10; j++)
                            {
                                if (p_charagazouid[j] != "")
                                {
                                    try
                                    {
                                        req = (HttpWebRequest)(WebRequest.Create(gazouseiki + p_charagazouid[j]));
                                        req.CookieContainer = m_cc;//取得済みのクッキーコンテナ

                                        g = Graphics.FromImage(p_charagazou28[j]);
                                        g.PixelOffsetMode = PixelOffsetMode.Half;
                                        g.DrawImage(new Bitmap(req.GetResponse().GetResponseStream()), 0, 0, 28, 28);
                                    }
                                    catch
                                    { }
                                }
                            }
                        }

                        //初期値取得
                        comcom(String.Format(MAI_SEND, m_ComSrvThread, th_ver, th_res_from));
                        //参加表明
                        comcom(String.Format(COM_SEND, m_ComSrvThread, m_ticket, m_vpos, m_postkey, m_mail, jibunid, m_premium, "##参加"));

                        //コメント読込開始
                        Thread th = new Thread(comentloop);
                        th.IsBackground = true;
                        th.Start();

                        textBoxchat.Enabled = true;

                        oto_hajime.Play();
                        stage = "設定";

                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("クッキーを取得できないか、番組が終了しています\r\n" + ex.ToString());

                        comboBox1.Enabled = true;
                        textBoxurl.Enabled = true;
                        //checkBox184.Enabled = true;
                    }
                }
                else
                {
                    MessageBox.Show("放送ページのURLを入力して下さい");
                }

                mt.ReleaseMutex();
            }
        }

        private void textBoxcharzoku_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;

                mt.WaitOne();

                if (sentakuzoku > -1)
                {
                    if (textBoxcharzoku.Text.Contains(','))
                    {
                        MessageBox.Show("「,」は使えせん");
                    }
                    else
                    {
                        p_zokusei[sentakuzoku] = textBoxcharzoku.Text;

                        textBoxcharzoku.Text = "";
                        textBoxcharzoku.Visible = false;
                        sentakuzoku = -1;
                    }
                }
                else if (sentakuchar > -1)
                {
                    try
                    {
                        //ユーザページ取得
                        req = (HttpWebRequest)(WebRequest.Create(textBoxcharzoku.Text));
                        req.CookieContainer = m_cc;//取得済みのクッキーコンテナ
                        StreamReader sr = new StreamReader(req.GetResponse().GetResponseStream(), Encoding.UTF8);
                        string stg = sr.ReadToEnd();
                        sr.Close();

                        //番号取得
                        p_charaid[sentakuchar] = new Regex(idseiki + @"\d+", RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(textBoxcharzoku.Text).Value.Replace(idseiki, "");

                        //画像取得
                        if (new Regex(gazouseiki + @"\d+/\d+\.jpg", RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(stg).Success)
                        {
                            p_charagazouid[sentakuchar] = new Regex(gazouseiki + @"\d+/\d+\.jpg", RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(stg).Value.Replace(gazouseiki, "");
                            req = (HttpWebRequest)(WebRequest.Create(gazouseiki + p_charagazouid[sentakuchar]));
                            req.CookieContainer = m_cc;//取得済みのクッキーコンテナ

                            g = Graphics.FromImage(p_charagazou28[sentakuchar]);
                            g.PixelOffsetMode = PixelOffsetMode.Half;
                            g.DrawImage(new Bitmap(req.GetResponse().GetResponseStream()), 0, 0, 28, 28);
                        }
                        else
                        {
                            p_charagazou28[sentakuchar] = Properties.Resources.noimage28;
                        }

                        //名前取得
                        if (new Regex("Qバージョンに変更", RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(stg).Success)
                        {
                            p_charaimei[sentakuchar] = new Regex(namaeseiki_h, RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(stg).Value.Replace("<h2><strong>", "").Replace("</strong>さん", "");
                        }
                        else
                        {
                            p_charaimei[sentakuchar] = new Regex(namaeseiki_q, RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(stg).Value.Replace("<h2>", "").Replace("<small>さん", "");
                        }
                        

                        textBoxcharzoku.Text = "";
                        textBoxcharzoku.Visible = false;
                        sentakuchar = -1;
                        syutokusippai = true;
                    }
                    catch
                    {
                        syutokusippai = false;
                        oto_zokuerabi.Play();
                    }
                }

                mt.ReleaseMutex();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://com.nicovideo.jp/community/co1768971");
        }

        /*
        private void karinet()
        {
            string[] stArrayData = new string[1];
            stArrayData[0] = (string)dataGridView1[0, cheban].Value;
            cheban++;

            XmlDocument xmlDoc = new XmlDocument();
            for (int i = 0; i < stArrayData.Length; i++)
            {
                

            }

            dataGridView1.Rows[cheban - 1].Selected = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                karinet();
            }
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            mt.WaitOne();

            //列ヘッダーかどうか調べる
            if (e.ColumnIndex < 0 && e.RowIndex >= 0)
            {
                //セルを描画する
                e.Paint(e.ClipBounds, DataGridViewPaintParts.All);

                //行番号を描画する範囲を決定する
                //e.AdvancedBorderStyleやe.CellStyle.Paddingは無視しています
                Rectangle indexRect = e.CellBounds;
                indexRect.Inflate(-2, -2);
                //行番号を描画する
                TextRenderer.DrawText(e.Graphics,
                    (e.RowIndex + 1).ToString(),
                    e.CellStyle.Font,
                    indexRect,
                    e.CellStyle.ForeColor,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
                //描画が完了したことを知らせる
                e.Handled = true;

                 
            }

            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.Count - 1;
                //karinet();
            }  

            mt.ReleaseMutex();
        }
        */

        private void textBoxchat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;

                //mt.WaitOne();
                syucom(textBoxchat.Text);
                //comcom(String.Format(COM_SEND, m_ComSrvThread, m_ticket, m_vpos, m_postkey, m_mail, jibunid, m_premium, textBoxchat.Text));
                //comcom(String.Format(COM_SEND0, m_ComSrvThread, m_ticket, m_vpos, m_postkey, "shita blue 184", jibunid, "3", "ははははっははｈ\r\n生麦生米生卵"));
                textBoxchat.Clear();

                //mt.ReleaseMutex();
            }
        }

        /*private void checkBox184_Click(object sender, EventArgs e)
        {
            if (checkBox184.Checked)
            {
                m_mail = "184";
                Properties.Settings.Default.h184 = "184";
                Properties.Settings.Default.Save();
            }
            else
            {
                m_mail = "";
                Properties.Settings.Default.h184 = "";
                Properties.Settings.Default.Save();
            }
        }*/

    }
}
